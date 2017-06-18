using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utility;
using 服务器端接收程序.Clazz;
using 服务器端接收程序.Config;
using 服务器端接收程序.Util;
using 服务器端接收程序.Clazz.Config.GuangDai;

namespace 服务器端接收程序.MyForm.GPRSControl
{
    /// <summary>
    /// 定时采集SysConfig.xml中所有的检测点
    /// </summary>
    public class AutoCollectionThread
    {
        /// <summary>
        /// 线程池
        /// </summary>
        public static TaskPool PoolB;
        /// <summary>
        /// 分配任务的线程
        /// </summary>
        private static Thread thread;
        /// <summary>
        /// 用于清空缓冲区函数中。
        /// </summary>
        private static byte[] buffer = new byte[1024];

        /// <summary>
        ///  开始这个类的任务     初始化线程池，并开始【分配任务】的线程
        /// </summary>
        public static void Start()
        {
            PoolB = new TaskPool();
            PoolB.Name = "B";
            PoolB.Setminthread(SysConfig.userProfile.AutoCollectionThreadPool_Min);
            PoolB.Setmaxthread(SysConfig.userProfile.AutoCollectionThreadPool_Max);

            thread = new Thread(new ThreadStart(Work));
            thread.IsBackground = true;
            thread.Start();
        }


        /// <summary>
        /// 分配任务
        /// </summary>
        private static void Work()
        {
            while (true)
            {
                int length = DTU_ClientManager.Clients.Count;

                for (int i = 0; i < length; i++)
                {
                    try
                    {
                        if (DTU_ClientManager.Clients[i].StationId != 0)//如果这个客户端在数据库中有对应的站点id
                        {
                            if (!HasExistClientInPool(DTU_ClientManager.Clients[i]))
                                PoolB.AddTaskItem(new WaitCallback(ExecuteOrder), DTU_ClientManager.Clients[i]);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogMg.AddError(ex);
                    }
                }
                List<GD_Station> gd_stations = SysConfig.GD_Config.GD_Stations;
                for (int i = 0; i < gd_stations.Count; i++)
                {
                    try
                    {
                       // if (!HasExistClientInPool(gd_stations[i].Unique))
                            if (!HasExistClientInPool(gd_stations[i]))
                            PoolB.AddTaskItem(new WaitCallback(ExecuteGDOrder), gd_stations[i]);
                    }
                    catch (Exception ex)
                    {
                        LogMg.AddError(ex);
                    }
                }
                Thread.Sleep((int)SysConfig.userProfile.RequestToClientInterval);
            }
        }

        private static void ExecuteOrder(object state)
        {
            ///第一步：找出需要采集的指标
            ///第二步：循环采集的指标，取出一个指标，
            ///第三步：锁住客户端
            ///第四步：采集
            ///第五步：释放客户端
            ///第六步：回到第二步
            ///结束
            DTUClientInfo client = (DTUClientInfo)state;

            Clazz.Config.XML_Station station = SysConfig.DTU_StationConfig.GetStationByTel(client.TelOrGprsId);
            if (station == null) return;
            List<Clazz.Config.XML_Test> listTest = station.ListTest;
            if (station != null && listTest != null)
            {
                foreach (Clazz.Config.XML_Test test in listTest)
                {
                    using (MyLock mylock = new MyLock(client, 3000, false))   // 在循环内锁
                    {
                        if (mylock.IsTimeout == false)
                        {
                            //LogManager.AddDebug("AutoCollectionThread 线程：" + Thread.CurrentThread.ManagedThreadId + " 开始锁了    是否超时" + mylock.IsTimeout);

                            try
                            {
                                ModbusReturn modbusReturn = new ModbusReturn();
                                int count = 0;  //执行错误的次数
                                while (modbusReturn.success == false && count < SysConfig.userProfile.ExecuteFailureCount)
                                {
                                    modbusReturn.clear();
                                    LogMg.AddDebug(string.Format("开始接收  时间:{0},站点:{1} tel:{2},testid:{3}", DateTime.Now.ToString(), client.Name, client.TelOrGprsId.ToString(), test.TestId));
                                  modbus.readdata(client.Protocol, client.socket, client.TelOrGprsId, test.Address, test.RegisterNo, test.FunctionCode, test.DataType, test.DecodeOrder, test.ReceiveTimeout, modbusReturn);
                                    count++;
                                }

                                if (modbusReturn.success) //接收数据成功
                                {
                                    DTU_ClientManager.UpdateLastVisitTime(client, DateTime.Now);
                                    if (Between(modbusReturn.value * test.Multiple + test.AddNumber, test.Min, test.Max))    //如果值不在取值范围内，则不要
                                    {
                                        SWSDataContext db = new SWSDataContext(Util.ServerSocketHelper.GetConnection(station.Org.DBName));
                                        SaveToDatabase(db, test.TestId, modbusReturn.value * test.Multiple + test.AddNumber);    //保存数据
                                        LogMg.AddDebug(string.Format("接收时间:{0}, tel:{1},value:{2},testid:{3}", DateTime.Now.ToString(), client.TelOrGprsId.ToString(), modbusReturn.value * test.Multiple + test.AddNumber, test.TestId));
                                        // MessageQueue.Enqueue_DataInfo(string.Format("接收时间:【{0}】,站点：{1}，testid:{2},值：{3}", DateTime.Now.ToString(), station.Name, test.TestId, value * test.Multiple + test.AddNumber));
                                    }
                                    else
                                    {
                                        LogMg.AddDebug(string.Format("接收时间:【{0}】,站点：{1}，testid:{2},乘以倍率之后的值：{3}   由于值不在范围内[{4},{5}]，丢弃", DateTime.Now.ToString(), station.Name, test.TestId, modbusReturn.value * test.Multiple + test.AddNumber, test.Min, test.Max));
                                    }
                                }
                                else
                                {
                                    //接收数据失败
                                    LogMg.AddDebug(string.Format("接收数据失败"));
                                    //   MessageQueue.Enqueue_DataInfo(string.Format("接收时间:【{0}】,站点：{1}，testid:{2}, 接收数据失败", DateTime.Now.ToString(), station.Name, test.TestId));
                                }
                            }
                            catch (SocketException)
                            {
                            }
                            catch (Exception ex)
                            {
                                LogMg.AddError(ex);
                                DEBUG.MsgBox(ex.ToString());
                            }
                            //LogManager.AddDebug("AutoCollectionThread 线程：" + Thread.CurrentThread.ManagedThreadId + " 释放锁了   ");
                        }
                        else
                        {
                            LogMg.AddDebug("AutoCollectionThread 锁失败了");
                        }
                    }
                }
            }
        }
        private static void ExecuteGDOrder(object state)
        {
            GuangDaiService.CorePlatformWebServiceClient ws = new GuangDaiService.CorePlatformWebServiceClient("gdServerCfg1");
            GD_Station gd_station = (GD_Station)state;
            if (gd_station != null && gd_station.tests != null)
            {
                foreach (Clazz.Config.XML_Test test in gd_station.tests)
                {
                    try
                    {
                        ModbusReturn modbusReturn = new ModbusReturn();
                        int count = 0;  //执行错误的次数
                        while (modbusReturn.success == false && count < SysConfig.userProfile.ExecuteFailureCount)
                        {
                            modbusReturn.clear();
                            //LogManager.AddDebug(string.Format("开始接收  时间:{0},站点:{1}  testid:{2}", DateTime.Now.ToString(), gd_station.name, test.TestId));
                           GuangDaiCommunicationModbus.readdata(ws, gd_station.wscId, test.Address, test.RegisterNo, test.FunctionCode, test.DataType, test.DecodeOrder, modbusReturn);
                            // result = modbus.readdata(gd_station.Protocol, gd_station.socket, gd_station.TelOrGprsId, test.Address, test.RegisterNo, test.FunctionCode, test.DataType, test.DecodeOrder, test.ReceiveTimeout, ref value);
                            count++;
                        }

                        if (modbusReturn.success) //接收数据成功
                        {
                            if (Between(modbusReturn.value * test.Multiple + test.AddNumber, test.Min, test.Max))    //如果值不在取值范围内，则不要
                            {
                                SWSDataContext db = new SWSDataContext(Util.ServerSocketHelper.GetConnection(gd_station.dbName));
                                SaveToDatabase(db, test.TestId, modbusReturn.value * test.Multiple + test.AddNumber);    //保存数据
                                LogMg.AddDebug(string.Format("接收时间:{0}, 站点:{1},value:{2},testid:{3}", DateTime.Now.ToString(), gd_station.name, modbusReturn.value * test.Multiple + test.AddNumber, test.TestId));
                                // MessageQueue.Enqueue_DataInfo(string.Format("接收时间:【{0}】,站点：{1}，testid:{2},值：{3}", DateTime.Now.ToString(), station.Name, test.TestId, value * test.Multiple + test.AddNumber));
                                updateStationOnlineInfo(db, gd_station.stationId);
                            }
                            else
                            {
                                LogMg.AddDebug(string.Format("接收时间:【{0}】,站点：{1}，testid:{2},乘以倍率之后的值：{3}   由于值不在范围内[{4},{5}]，丢弃", DateTime.Now.ToString(), gd_station.name, test.TestId, modbusReturn.value* test.Multiple + test.AddNumber, test.Min, test.Max));
                            }
                        }
                        else
                        {
                            //接收数据失败
                            LogMg.AddDebug(string.Format( "数据库名：{0}    testid:{1}   接收数据失败",gd_station.dbName,test.TestId));
                            //   MessageQueue.Enqueue_DataInfo(string.Format("接收时间:【{0}】,站点：{1}，testid:{2}, 接收数据失败", DateTime.Now.ToString(), station.Name, test.TestId));
                        }
                    }
                    catch (SocketException)
                    {
                    }
                    catch (Exception ex)
                    {
                        LogMg.AddError(ex);
                        DEBUG.MsgBox(ex.ToString());
                    }
                }
            }
        }
        /// <summary>
        /// 更新站点在线信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="p"></param>
        public static void updateStationOnlineInfo(SWSDataContext db, int stationId)
        {
            try
            {
                country_station station = db.country_station.SingleOrDefault(c => c.id == stationId);
                if (station != null)
                {
                    station_online_info online_info = db.station_online_info.SingleOrDefault(c => c.stationid == stationId);
                    if (online_info != null)
                    {
                        online_info.name = station.name;
                        online_info.register_time = DateTime.Now;
                        online_info.last_visit_time = DateTime.Now;
                        online_info.stationid = stationId;
                    }
                    else
                    {
                        online_info = new station_online_info();
                        online_info.name = station.name;
                        online_info.register_time = DateTime.Now;
                        online_info.last_visit_time = DateTime.Now;
                        online_info.stationid = stationId;
                        db.station_online_info.InsertOnSubmit(online_info);
                    }
                    List<station_online_info> deletes = db.station_online_info.Where(c => c.stationid == online_info.stationid && c.id != online_info.id).ToList();
                    if (deletes.Count > 0)
                    {
                        db.station_online_info.DeleteAllOnSubmit(deletes);
                    }
                    db.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                LogMg.AddError("保存广岱客户端在线信息失败,\r\n" + ex.ToString());
            }
        }



        /// <summary>
        /// 保存数据到数据库
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="value"></param>
        private static void SaveToDatabase(SWSDataContext db, int testid, double value)
        {
            LogMg.AddDebug(string.Format("testid={0}    value={1}", testid, value));
            //  SWSDataContext db = new SWSDataContext(Util.ServerSocketHelper.GetConnection(dbname));
            test test = db.test.SingleOrDefault(c => c.testid == testid);   //查询出检测点
            if (test != null)
            {
                if (test.means.Trim() == "屏幕取词" || test.means.Trim() == "自动获取")
                {
                    realrec realrec = new realrec();
                    realrec.testid = testid;
                    realrec.value = (decimal)value;
                    realrec.testtime = DateTime.Now; 
                    db.realrec.InsertOnSubmit(realrec);
                    db.SubmitChanges();     //提交
                }
            }
        }



        private static bool Between(double value, double min, double max)
        {
            if (value >= min && value <= max)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 判断客户端在【任务队列】和【线程里】里是否已经存在
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private static bool HasExistClientInPool(Object client)
        {
            try
            {
                foreach (TaskPool.Waititem item in PoolB.waitlist)
                {
                    //DTUClientInfo content = (DTUClientInfo)item.Context;
                    if (item.Context.GetHashCode() == client.GetHashCode())
                    {
                        return true;
                    }
                }
                //if (PoolB.publicpool.Count > 1)
                //{
                foreach (KeyValuePair<string, Task> item in PoolB.publicpool)
                {

                    try
                    {
                        Task task = (Task)item.Value;
                        //DTUClientInfo content = (DTUClientInfo)task.contextdata;
                        if (task.contextdata.GetHashCode() == client.GetHashCode())
                        {
                            return true;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                //}
            }
            catch (Exception)
            {
            }
            return false;
        }
    }
}
