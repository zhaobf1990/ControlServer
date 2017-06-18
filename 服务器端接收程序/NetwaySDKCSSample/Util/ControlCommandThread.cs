using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utility;
using 服务器端接收程序.Clazz;
using 服务器端接收程序.Config;
using 服务器端接收程序.Util;
using 服务器端接收程序.Clazz.Config;
using 服务器端接收程序.SWS_DB;

namespace 服务器端接收程序.MyForm.GPRSControl
{
    /// <summary>
    /// 执行control_command表的指令
    /// </summary>
    public class ControlCommandThread
    {
        //类的主要功能说明：循环每个客户端，查找每个客户端是否有需要执行的指令，如果有，则执行指令。


        /// <summary>
        /// 线程池
        /// </summary>
        public static TaskPool PoolA;
        /// <summary>
        /// 循环客户端列表，并分配任务的线程
        /// </summary>
        private static Thread thread;
        /// <summary>
        /// 用于清空缓冲区函数中。
        /// </summary>
        private static byte[] buffer = new byte[1024];

        /// <summary>
        /// 开始这个类的任务     初始化线程池，并开始【分配任务】的线程
        /// </summary>
        public static void Start()
        {
            PoolA = new TaskPool();
            PoolA.Name = "A";
            PoolA.Setminthread(SysConfig.userProfile.ControlCommandThreadPool_Min);
            PoolA.Setmaxthread(SysConfig.userProfile.ControlCommandThreadPool_Max);

            thread = new Thread(new ThreadStart(Work));
            thread.IsBackground = true;
            thread.Start();
        }


        /// <summary>
        /// 循环客户端列表，将客户端添加到线程池队列
        /// </summary>
        private static void Work()
        {
            while (true)
            {
                //循环数据库，根据transfer_code找出客户端，
                //将客户端添加到线程池，
                foreach (XML_Org org in SysConfig.orgConfig.Orgs)
                {
                    try
                    {
                        SWSDataContext db = new SWSDataContext(ConnectStringHelper.GetConnection(SysConfig.userProfile.DbAddress, org.DBName, SysConfig.userProfile.DbUserName, SysConfig.userProfile.DbPassword));
                        添加DTU控制任务(db);
                        添加广岱控制任务(db, org.DBName, org.gdServerCfg);
                        添加广岱透传控制任务(db, org.DBName, org.gdServerCfg);
                    }
                    catch (Exception ex)
                    {
                        LogMg.AddError(ex);
                    }

                }
                Thread.Sleep((int)SysConfig.userProfile.GongKuangConfigInterval);
            }
        }

        private static void 添加广岱控制任务(SWSDataContext db, String dbname, String gdServerCfg)
        {
            try
            {
                //查询出一个小时内有指令的站点的id
                List<int> stations = db.ExecuteQuery<int>("SELECT station_id FROM dbo.control_command where (state=0 OR state IS NULL) and communication_mode=4  group by station_id ").ToList();// tp is not NULL 暂时有拍照的数据
                if (stations.Count == 0) return;
                //循环站点   将站点添加到线程池
                foreach (int stationid in stations)
                {
                    try
                    {
                        //如果站点在【任务队列】和【线程里】里已经存在了，就不继续添加
                        if (!HasExistClientInPool(stationid))
                        {
                            Args arg = new Args() { dbName = dbname, station_id = stationid, gdServerCfg = gdServerCfg };
                            PoolA.AddTaskItem(new WaitCallback(ExecuteGuangDaiOrder), arg);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogMg.AddError(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
            }

        }
        private static void 添加广岱透传控制任务(SWSDataContext db, String dbname, String gdServerCfg)
        {
            try
            {
                //查询出一个小时内有指令的站点的id
                List<int> stations = db.ExecuteQuery<int>("SELECT station_id FROM dbo.control_command where (state=0 OR state IS NULL) and communication_mode=5   group by station_id ").ToList();
                if (stations.Count == 0) return;
                //循环站点   将站点添加到线程池
                foreach (int stationid in stations)
                {
                    try
                    {
                        //如果站点在【任务队列】和【线程里】里已经存在了，就不继续添加
                        if (!HasExistClientInPool(stationid))
                        {
                            Args arg = new Args() { dbName = dbname, station_id = stationid, gdServerCfg = gdServerCfg };
                            PoolA.AddTaskItem(new WaitCallback(ExecuteGuangDai透传Order), arg);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogMg.AddError(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
            }

        }
        private static void 添加DTU控制任务(SWSDataContext db)
        {
            try
            {
                //查询出一个小时内有指令的站点的id
                //List<int> stations = db.ExecuteQuery<int>("SELECT station_id FROM dbo.control_command where (state=0 OR state IS NULL) and communication_mode=2  and datediff(mi,add_datetime,GETDATE())<60 group by station_id ").ToList();
                List<int> stations = db.ExecuteQuery<int>("SELECT station_id FROM dbo.control_command where (state=0 OR state IS NULL) and (communication_mode=2 or tp='3')    group by station_id ").ToList();
                if (stations.Count == 0) return;
                ///站点id和transfer_code的键值对
                List<Station_transferCode> StaTrans = db.ExecuteQuery<Station_transferCode>("SELECT  id as stationId,transfer_code FROM  dbo.country_station WHERE  transfer_code IS NOT NULL AND transfer_code<>''  AND transfer_code<>'null'").ToList();
                //循环站点   将站点添加到线程池
                foreach (int stationid in stations)
                {
                    try
                    {
                        Station_transferCode StaTran = StaTrans.SingleOrDefault(c => c.stationId == stationid);
                        //如果这个站点没有配置dtu    就跳过
                        if (StaTran == null) continue;
                        //根据设备唯一编号 找出 DTUClientInfo
                        DTUClientInfo client = DTU_ClientManager.Clients.SingleOrDefault(c => c.TelOrGprsId == StaTran.transfer_code);
                        if (client == null) continue;

                        //如果客户端在【任务队列】和【线程里】里已经存在了，就不继续添加
                        if (!HasExistClientInPool(client))
                            PoolA.AddTaskItem(new WaitCallback(ExecuteDtuOrder), client);
                    }
                    catch (Exception ex)
                    {
                        LogMg.AddError(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
            }
        }

        private static void ExecuteGuangDaiOrder(object state)
        {
            try
            {
                Args arg = (Args)state;
                SWSDataContext db = new SWSDataContext(ConnectStringHelper.GetConnection(SysConfig.userProfile.DbAddress, arg.dbName, SysConfig.userProfile.DbUserName, SysConfig.userProfile.DbPassword));

                GuangDaiService.CorePlatformWebServiceClient ws = new GuangDaiService.CorePlatformWebServiceClient(arg.gdServerCfg);
                SWS_DBDataContext SWS_DB = new SWS_DBDataContext(ConnectStringHelper.GetConnection(SysConfig.userProfile.DbAddress, SysConfig.userProfile.DbName, SysConfig.userProfile.DbUserName, SysConfig.userProfile.DbPassword));
                List<guangdai_station_link> links = SWS_DB.guangdai_station_link.ToList();
                String wscid = getWscId(arg, db, links);
                if (String.IsNullOrEmpty(wscid))
                {
                    LogMg.AddDebug("站点id=" + arg.station_id + "  找不到对应的wscId");
                    return;
                }
                //把那些不需要执行了的数据 state=1  就当这行指令已经执行过了
                List<int> giveUpCommands = db.ExecuteQuery<int>("SELECT id FROM dbo.control_command  WHERE station_id=" + arg.station_id + " AND (state=0 OR state IS NULL) AND id NOT IN (SELECT MAX(id) FROM dbo.control_command   WHERE  station_id=" + arg.station_id + " AND   (state=0 OR state IS NULL)  GROUP BY gong_kuang_id,read_or_write) ").ToList();
                if (giveUpCommands.Count > 0)
                {
                    db.ExecuteCommand("UPDATE dbo.control_command SET state=1 WHERE id in(" + string.Join(", ", giveUpCommands) + ")");
                } 
                //db.ExecuteCommand("UPDATE dbo.control_command SET state=1 WHERE station_id=" + arg.station_id + " AND (state=0 OR state IS NULL) AND id NOT IN (SELECT MAX(id) FROM dbo.control_command   WHERE  station_id=" + arg.station_id + " AND   (state=0 OR state IS NULL)  GROUP BY gong_kuang_id,read_or_write) ");
                //获取需要执行的指令
                List<control_command> commands = db.ExecuteQuery<control_command>("SELECT * FROM dbo.control_command WHERE id IN(SELECT MAX(id) as id  FROM dbo.control_command   WHERE  station_id=" + arg.station_id + " AND communication_mode=4  AND  tp is NULL AND   (state=0 OR state IS NULL)  GROUP BY gong_kuang_id,read_or_write) ORDER BY add_datetime").ToList();

                TakePhotoTp5(db, ws, arg.station_id, wscid);
                for (int i = 0; i < commands.Count; i++)
                {
                    control_command command = commands[i];
                    if (command.COUNT == null)
                    {
                        command.COUNT = 0;
                    }
                    try
                    {
                        gong_kuang_config gongkuang = db.gong_kuang_config.SingleOrDefault(c => c.id == command.gong_kuang_id);
                        if (gongkuang == null)  //如果找不到command对应和gongkuangid,则把这条command标记为执行失败。
                        {
                            LogMg.AddError("找不到控制命令所对应的工况配置信息");
                            command.execute_result = 0;   //标记为执行失败
                            command.state = 1;   // 就当已经执行过
                            command.execute_comment = "执行失败,找不到控制命令所对应的工况配置信息";
                            command.complete_datetime = DateTime.Now;
                            db.SubmitChanges();
                            continue;
                        }
                        if (command.read_or_write == 0)   //读读读读读读读读读读读
                        {
                            GuangDaiService.config config = ws.doConfig(wscid, Convert.ToInt32(gongkuang.read_register), (int)command.read_or_write, -1, "");
                            command.state = 1;  //标志为已发送
                            if (config.result == true)
                            {
                                command.execute_result = 1;  //标记为执行成功
                                command.value = Convert.ToString(config.value);
                                command.execute_comment = "执行读取操作成功";
                                gongkuang.read_value = Convert.ToString(config.value);
                                gongkuang.execute_comment = "执行读取操作成功";
                                if (gongkuang.config_type == "05")
                                {
                                    try
                                    {
                                        test test = db.test.Single(c => c.testid == gongkuang.testid);
                                        double multiple = gongkuang.Multiple == null ? 1 : (double)gongkuang.Multiple;
                                        double addNumber = gongkuang.AddNumber == null ? 0 : (double)gongkuang.AddNumber;
                                        test.value = Convert.ToString(config.value * multiple + addNumber);
                                        //test.value = Convert.ToString(config.value * gongkuang.Multiple + gongkuang.AddNumber); 
                                    }
                                    catch (Exception ex)
                                    {
                                        LogMg.AddError(ex);
                                        LogMg.AddDebug("找不到工况对应的检测点，gongkuangid=" + gongkuang.id + "   testid=" + gongkuang.testid);
                                    }
                                }
                            }
                            else
                            {
                                command.execute_result = 0;   //标记为执行失败
                                command.execute_comment = "执行读取操作失败";
                                gongkuang.execute_comment = "执行读取操作失败";
                            }
                        }
                        if (command.read_or_write == 1)      //写写写写写写写写写写写写写写写
                        {
                            GuangDaiService.config config = ws.doConfig(wscid, Convert.ToInt32(gongkuang.write_register), (int)command.read_or_write, Convert.ToInt32(command.value), "");
                            LogMg.AddDebug(String.Format(" ws.doConfig({0},{1}, {2}, {3}, {4})   返回结果是   result={5}   value={6}   message={7}  ", wscid, gongkuang.write_register, command.read_or_write, "-1", "", config.result, config.value, config.message));
                            command.state = 1;  //标志为已发送
                            if (config.result == true)
                            {
                                command.execute_result = 1;  //标记为执行成功 
                                command.execute_comment = "执行写操作成功";
                                gongkuang.write_value = command.value;
                                gongkuang.execute_comment = "执行写操作成功";
                            }
                            else
                            {
                                command.execute_result = 0;   //标记为执行失败
                                command.execute_comment = "执行写操作失败";
                                gongkuang.execute_comment = "执行写操作失败";
                            }
                        }
                        gongkuang.execute_datetime = DateTime.Now;
                        command.complete_datetime = DateTime.Now;
                    }
                    catch (Exception ex)
                    {
                        command.execute_result = 0; //标记为执行失败
                        command.execute_comment = "执行命令失败,服务程序出现了异常";
                        // MessageQueue.Enqueue_DataInfo(string.Format("接收时间:【{0}】,站点：{1},   执行工况配置失败.程序出现异常,请查看日志.  ", DateTime.Now, station.name));
                        LogMg.AddError(ex);
                    }
                    finally
                    {
                        command.complete_datetime = DateTime.Now;
                    }
                    try
                    {
                        db.SubmitChanges();
                    }
                    catch (Exception ex)
                    {
                        LogMg.AddError(ex.ToString());

                    }
                }
            }
            catch (Exception ex) { LogMg.AddError(ex); }
        }



        private static void ExecuteGuangDai透传Order(object state)
        {
            try
            {
                Args arg = (Args)state;
                SWSDataContext db = new SWSDataContext(ConnectStringHelper.GetConnection(SysConfig.userProfile.DbAddress, arg.dbName, SysConfig.userProfile.DbUserName, SysConfig.userProfile.DbPassword));

                GuangDaiService.CorePlatformWebServiceClient ws = new GuangDaiService.CorePlatformWebServiceClient(arg.gdServerCfg);
                SWS_DBDataContext SWS_DB = new SWS_DBDataContext(ConnectStringHelper.GetConnection(SysConfig.userProfile.DbAddress, SysConfig.userProfile.DbName, SysConfig.userProfile.DbUserName, SysConfig.userProfile.DbPassword));
                List<guangdai_station_link> links = SWS_DB.guangdai_station_link.ToList();
                String wscid = getWscId(arg, db, links);
                if (String.IsNullOrEmpty(wscid))
                {
                    LogMg.AddDebug("站点id=" + arg.station_id + "  找不到对应的wscId");
                    return;
                }
                //把那些不需要执行了的数据 state=1  就当这行指令已经执行过了
                List<int> giveUpCommands = db.ExecuteQuery<int>("SELECT id FROM dbo.control_command  WHERE station_id=" + arg.station_id + " AND (state=0 OR state IS NULL) AND id NOT IN (SELECT MAX(id) FROM dbo.control_command   WHERE  station_id=" + arg.station_id + " AND   (state=0 OR state IS NULL)  GROUP BY gong_kuang_id,read_or_write) ").ToList();
                if (giveUpCommands.Count > 0)
                {
                    db.ExecuteCommand("UPDATE dbo.control_command SET state=1 WHERE id in(" + string.Join(", ", giveUpCommands) + ")");
                }
                //db.ExecuteCommand("UPDATE dbo.control_command SET state=1 WHERE station_id=" + arg.station_id + " AND (state=0 OR state IS NULL) AND id NOT IN (SELECT MAX(id) FROM dbo.control_command   WHERE  station_id=" + arg.station_id + " AND   (state=0 OR state IS NULL)  GROUP BY gong_kuang_id,read_or_write) ");

                //获取需要执行的指令
                List<control_command> commands = db.ExecuteQuery<control_command>("SELECT * FROM dbo.control_command WHERE id IN(SELECT MAX(id) as id  FROM dbo.control_command   WHERE  station_id=" + arg.station_id + " AND communication_mode=5  AND tp is Null AND  (state=0 OR state IS NULL)  GROUP BY gong_kuang_id,read_or_write) ORDER BY add_datetime").ToList();

                TakePhotoTp5(db, ws, arg.station_id, wscid);
                for (int i = 0; i < commands.Count; i++)
                {
                    control_command command = commands[i];
                    if (command.COUNT == null)
                    {
                        command.COUNT = 0;
                    }
                    try
                    {
                        gong_kuang_config gongkuang = db.gong_kuang_config.SingleOrDefault(c => c.id == command.gong_kuang_id);
                        if (gongkuang == null)  //如果找不到command对应和gongkuangid,则把这条command标记为执行失败。
                        {
                            LogMg.AddError("找不到控制命令所对应的工况配置信息");
                            command.execute_result = 0;   //标记为执行失败
                            command.state = 1;   // 就当已经执行过
                            command.execute_comment = "执行失败,找不到控制命令所对应的工况配置信息";
                            command.complete_datetime = DateTime.Now;
                            db.SubmitChanges();
                            continue;
                        }
                        ModbusReturn modbusReturn = new ModbusReturn();
                        byte address = (byte)gongkuang.address;

                        if (command.read_or_write == 0)   //读读读读读读读读读读读
                        {
                            GuangDaiCommunicationModbus.readdata(ws, wscid, address, ushort.Parse(gongkuang.read_register), int.Parse(gongkuang.function_code), gongkuang.data_type, gongkuang.decode_order, modbusReturn);
                            command.state = 1;  //标志为已发送
                            if (modbusReturn.success == true)
                            {
                                command.execute_result = 1;  //标记为执行成功
                                command.value = Convert.ToString(modbusReturn.value);

                                gongkuang.read_value = Convert.ToString(modbusReturn.value);
                                gongkuang.execute_comment = "执行读取操作成功";
                                if (gongkuang.config_type == "05")
                                {
                                    try
                                    {
                                        test test = db.test.Single(c => c.testid == gongkuang.testid);
                                        double multiple = gongkuang.Multiple == null ? 1 : (double)gongkuang.Multiple;
                                        double addNumber = gongkuang.AddNumber == null ? 0 : (double)gongkuang.AddNumber;
                                        test.value = Convert.ToString(modbusReturn.value * multiple + addNumber);
                                    }
                                    catch (Exception ex)
                                    {
                                        LogMg.AddError(ex);
                                        LogMg.AddDebug("找不到工况对应的检测点，gongkuangid=" + gongkuang.id + "   testid=" + gongkuang.testid);
                                    }
                                }
                            }
                            else
                            {
                                command.execute_result = 0;   //标记为执行失败 
                                gongkuang.execute_comment = "执行读取操作失败";
                            }
                            command.execute_comment = modbusReturn.ToString();
                        }
                        if (command.read_or_write == 1)      //写写写写写写写写写写写写写写写
                        {
                            GuangDaiCommunicationModbus.writedata(ws, wscid, address, ushort.Parse(gongkuang.read_register), int.Parse(gongkuang.function_code), gongkuang.data_type, gongkuang.decode_order, short.Parse(command.value), modbusReturn);
                            //   GuangDaiService.config config = ws.doConfig(wscid, Convert.ToInt32(gongkuang.write_register), (int)command.read_or_write, Convert.ToInt32(command.value), "");
                            // LogManager.AddDebug(String.Format("ws.transmitTransparently({0},{1}, {2}, {3}, {4})   返回结果是 {5}    ", wscid, gongkuang.write_register, command.read_or_write, "-1", "", flag));
                            command.state = 1;  //标志为已发送
                            if (modbusReturn.success == true)
                            {
                                command.execute_result = 1;  //标记为执行成功  
                                gongkuang.write_value = command.value;
                                gongkuang.execute_comment = "执行写操作成功";
                            }
                            else
                            {
                                command.execute_result = 0;   //标记为执行失败 
                                gongkuang.execute_comment = "执行写操作失败";
                            }
                            command.execute_comment = modbusReturn.ToString();
                        }
                        gongkuang.execute_datetime = DateTime.Now;
                        command.complete_datetime = DateTime.Now;
                    }
                    catch (Exception ex)
                    {
                        command.execute_result = 0; //标记为执行失败
                        command.execute_comment = "执行命令失败,服务程序出现了异常";
                        // MessageQueue.Enqueue_DataInfo(string.Format("接收时间:【{0}】,站点：{1},   执行工况配置失败.程序出现异常,请查看日志.  ", DateTime.Now, station.name));
                        LogMg.AddError(ex);
                    }
                    finally
                    {
                        command.complete_datetime = DateTime.Now;
                    }
                    try
                    {
                        db.SubmitChanges();
                    }
                    catch (Exception ex)
                    {
                        LogMg.AddError(ex.ToString());

                    }
                }
            }
            catch (Exception ex)
            {
                Args arg = (Args)state;
                LogMg.AddError("异常数据库是：" + arg.dbName + "   站点id=" + arg.station_id);
                LogMg.AddError(ex);
            }
        }

        /// <summary>
        /// 执行DTU控制任务
        /// </summary>
        /// <param name="state"></param>
        private static void ExecuteDtuOrder(object state)
        {
            //1.锁住客户端对象
            //2.找出客户端对应的数据库，并找出需要执行的控制指令。
            //3.循环发送指令到客户端，再接收返回的数据，存入数据库。
            //4.释放客户端对象
            DTUClientInfo client = (DTUClientInfo)state;
            using (MyLock mylock = new MyLock(client, 20000, false))
            {
                if (mylock.IsTimeout == false)
                {
                    //LogManager.AddDebug("ControlCommandThread线程：" + Thread.CurrentThread.ManagedThreadId + " 开始锁了    是否超时" + mylock.IsTimeout);
                    //找出这个站点对应的数据库,   并获取数据库连接
                    SqlConnection connection = GetDbConnectionByTel(client.TelOrGprsId);
                    if (connection == null)   //找不到对应的数据库连接
                    {
                        LogMg.AddDebug(string.Format("根据Tel={0}的设备唯一id，在sysconfig.xml中找不到对应的数据库连接", client.TelOrGprsId.ToString()));
                        return;
                    }
                    SWSDataContext db = new SWSDataContext(connection);

                    takePhoto(db, client.StationId, client);

                    //把那些不需要执行了的数据 state=1  就当这行指令已经执行过了
                    List<int> giveUpCommands = db.ExecuteQuery<int>("SELECT id FROM dbo.control_command  WHERE station_id=" + client.StationId + " AND (state=0 OR state IS NULL) AND id NOT IN (SELECT MAX(id) FROM dbo.control_command   WHERE  station_id=" + client.StationId + " AND   (state=0 OR state IS NULL)  GROUP BY gong_kuang_id,read_or_write) ").ToList();
                    if (giveUpCommands.Count > 0)
                    {
                        db.ExecuteCommand("UPDATE dbo.control_command SET state=1 WHERE id in(" + string.Join(", ", giveUpCommands) + ")");
                    }
                    //db.ExecuteCommand("UPDATE dbo.control_command SET state=1 WHERE station_id=" + client.StationId + " AND (state=0 OR state IS NULL) AND id NOT IN (SELECT MAX(id) FROM dbo.control_command   WHERE  station_id=" + client.StationId + " AND   (state=0 OR state IS NULL)  GROUP BY gong_kuang_id,read_or_write) ");

                    //获取需要执行的指令
                    List<control_command> commands = db.ExecuteQuery<control_command>("SELECT * FROM dbo.control_command WHERE id IN(SELECT MAX(id) FROM dbo.control_command   WHERE  station_id=" + client.StationId + "  AND communication_mode=2 AND   (state=0 OR state IS NULL)  GROUP BY gong_kuang_id,read_or_write) ORDER BY add_datetime").ToList();

                    List<gong_kuang_config> gongKuangs = db.gong_kuang_config.Where(c => c.station_id == client.StationId).ToList();
                    foreach (control_command command in commands)  //循环发送指令
                    {
                        if (command.COUNT == null)
                        {
                            command.COUNT = 0;
                        }
                        try
                        {
                            gong_kuang_config gongkuang = gongKuangs.SingleOrDefault(c => c.id == command.gong_kuang_id);
                            if (gongkuang == null)  //如果找不到command对应和gongkuangid,则把这条command标记为执行失败。
                            {
                                LogMg.AddError("找不到控制命令所对应的工况配置信息");
                                command.execute_result = 0;   //标记为执行失败
                                command.state = 1;   // 就当已经执行过
                                command.execute_comment = "执行失败,找不到控制命令所对应的工况配置信息";
                                command.complete_datetime = DateTime.Now;
                                db.SubmitChanges();
                                continue;
                            }
                            byte address = (byte)gongkuang.address;
                            ClearSocketCache(client.socket);//清除缓冲区
                            LogMg.AddDebug("hashcode=" + client.socket.GetHashCode());
                            ModbusReturn modbusReturn = new ModbusReturn();
                            if (command.read_or_write == 0)   //读读读读读读读读读读读
                            {
                                while (modbusReturn.success == false && command.COUNT < SysConfig.userProfile.ExecuteFailureCount)
                                {
                                    modbusReturn.clear();
                                    modbus.readdata(client.Protocol, client.socket, "", address, ushort.Parse(gongkuang.read_register), int.Parse(gongkuang.function_code), gongkuang.data_type, gongkuang.decode_order, (int)gongkuang.receive_timeout, modbusReturn);
                                    //LogManager.AddDebug(String.Format("执行读命令   GPRSID:{0}  gongkuangid:{1}   address{2}   读寄存器编号:{3}  读取的值{4}   方法的返回bool值:{5}", client.TelOrGprsId, gongkuang.id, address, gongkuang.read_register, value.ToString(), flag));
                                    command.COUNT++;
                                }
                                command.state = 1;  //标志为已发送
                                if (modbusReturn.success == true)
                                {
                                    DTU_ClientManager.UpdateLastVisitTime(client, DateTime.Now);
                                    command.execute_result = 1;  //标记为执行成功
                                    command.value = Convert.ToString(modbusReturn.value);
                                    gongkuang.read_value = Convert.ToString(modbusReturn.value);
                                    gongkuang.execute_comment = "执行读取操作成功";
                                    if (gongkuang.config_type == "05")
                                    {
                                        try
                                        {
                                            test test = db.test.Single(c => c.testid == gongkuang.testid);
                                            test.value = Convert.ToString(modbusReturn.value * gongkuang.Multiple + gongkuang.AddNumber);
                                        }
                                        catch (Exception ex)
                                        {
                                            LogMg.AddError(ex);
                                            LogMg.AddDebug("找不到工况对应的检测点，gongkuangid=" + gongkuang.id + "   testid=" + gongkuang.testid);
                                        }
                                    }
                                }
                                else
                                {
                                    command.execute_result = 0;   //标记为执行失败 
                                    gongkuang.execute_comment = "执行读取操作失败";
                                }
                                command.execute_comment = modbusReturn.ToString();
                            }
                            else    //写写写写写写写写写写写写写写写
                            {
                                while (modbusReturn.success == false && command.COUNT < SysConfig.userProfile.ExecuteFailureCount)
                                {
                                    modbusReturn.clear();
                                    modbus.writedata(client.Protocol, client.socket, "", address, ushort.Parse(gongkuang.write_register), gongkuang.data_type, (int)gongkuang.receive_timeout, short.Parse(command.value), modbusReturn);
                                    //LogManager.AddDebug(String.Format("执行写命令   GPRSID:{0}   address{1}   写寄存器编号:{2}  写入的值{3}   方法的返回bool值:{4}", client.TelOrGprsId, address, gongkuang.write_register, command.value, flag));
                                    command.COUNT++;
                                }
                                command.state = 1;  //标志为已发送
                                if (modbusReturn.success == true)
                                {
                                    DTU_ClientManager.UpdateLastVisitTime(client, DateTime.Now);
                                    command.execute_result = 1;  //标记为执行成功  
                                    gongkuang.write_value = command.value;
                                    gongkuang.execute_comment = "执行写操作成功";
                                }
                                else
                                {
                                    command.execute_result = 0;   //标记为执行失败 
                                    gongkuang.execute_comment = "执行写操作失败";
                                }
                                command.execute_comment = modbusReturn.ToString();
                            }
                            gongkuang.execute_datetime = DateTime.Now;
                            command.complete_datetime = DateTime.Now;
                        }
                        catch (Exception ex)
                        {
                            command.execute_result = 0; //标记为执行失败
                            command.execute_comment = "执行命令失败,服务程序出现了异常";
                            // MessageQueue.Enqueue_DataInfo(string.Format("接收时间:【{0}】,站点：{1},   执行工况配置失败.程序出现异常,请查看日志.  ", DateTime.Now, station.name));
                            LogMg.AddError(ex);
                        }
                        finally
                        {
                            command.complete_datetime = DateTime.Now;
                        }
                        db.SubmitChanges();
                    }
                    //LogManager.AddDebug("ControlCommandThread线程：" + Thread.CurrentThread.ManagedThreadId + " 释放锁了   ");
                }
                else
                {
                    LogMg.AddDebug("ControlCommandThread 锁失败了");
                }
            }
        }


        /// <summary>
        /// 拍照方法 
        /// </summary>
        /// <param name="db">数据库对象</param>
        /// <param name="stationid">站点id</param>
        /// <param name="client">客户端</param>
        private static void takePhoto(SWSDataContext db, int stationid, Clazz.DTUClientInfo client)
        {
            ///根据站点id去指定的数据库中找拍照指令，如果存在拍照的指令，则调用【_485_JBT_Camera】类的拍照方法。
            try
            {
                List<control_command> jbt_commands = db.control_command.Where(c => c.gong_kuang_id == 0 && c.state == 0 && c.tp == "3" && c.station_id == stationid).OrderBy(c => c.add_datetime).ToList();
                foreach (control_command command in jbt_commands)
                {
                    if (command.COUNT == null)
                    {
                        command.COUNT = 0;
                    }
                    try
                    {
                        bool flag = false;
                        command.state = 1;  //标志为已发送
                        byte[] imageBytes = new byte[1];  //takePhoto方法内部会再次分配图像数组大小                      
                        while (flag == false && command.COUNT < SysConfig.userProfile.ExecuteFailureCount)
                        {
                            flag = _485_JBT_Camera.TakePhoto(client, 5000, ref imageBytes);
                            command.COUNT++;
                            LogMg.AddDebug("拍照失败" + (int)command.COUNT + "次");
                        }
                        if (flag == true)   //如果拍照成功
                        {
                            SavePicture(db, stationid, imageBytes);//保存照片到数据库
                            command.execute_result = 1;  //标记为执行成功
                            command.execute_comment = "拍照成功";
                        }
                        else
                        {
                            command.execute_result = 0;   //标记为执行失败
                            command.execute_comment = "拍照失败";
                        }
                    }
                    catch (Exception ex)
                    {
                        LogMg.AddError(ex);
                        command.execute_result = 0;
                        command.execute_comment = "拍照失败";
                    }
                    finally
                    {
                        command.complete_datetime = DateTime.Now;
                        db.SubmitChanges();
                    }
                }

            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
            }

        }

        private static void TakePhotoTp5(SWSDataContext db, GuangDaiService.CorePlatformWebServiceClient ws, int stationId, string wscid)
        {
            //  List<control_command> commands = db.control_command.Where(c => c.station_id == stationId && c.tp.Trim() == "5" && c.state == 0).ToList();
            List<control_command> commands = db.ExecuteQuery<control_command>("SELECT * FROM dbo.control_command WHERE station_id=" + stationId + " AND state=0 AND tp='5'").ToList();
            foreach (control_command command in commands)
            {
                if (command.COUNT == null)
                {
                    command.COUNT = 0;
                }
                try
                {
                    gong_kuang_config gongkuang = db.gong_kuang_config.SingleOrDefault(c => c.id == command.gong_kuang_id);
                    if (gongkuang == null)  //如果找不到command对应和gongkuangid,则把这条command标记为执行失败。
                    {
                        LogMg.AddError("找不到控制命令所对应的工况配置信息");
                        command.execute_result = 0;   //标记为执行失败
                        command.state = 1;   // 就当已经执行过
                        command.execute_comment = "执行失败,找不到控制命令所对应的工况配置信息";
                        command.complete_datetime = DateTime.Now;
                        db.SubmitChanges();
                        continue;
                    }
                    byte address = (byte)gongkuang.address;

                    GuangDaiService.config config = ws.doConfig(wscid, Convert.ToInt32(gongkuang.write_register), (int)command.read_or_write, Convert.ToInt32(command.value), "");
                    LogMg.AddDebug(String.Format(" ws.doConfig({0},{1}, {2}, {3}, {4})   返回结果是   result={5}   value={6}   message={7}  ", wscid, gongkuang.write_register, command.read_or_write, "-1", "", config.result, config.value, config.message));
                    command.state = 1;  //标志为已发送
                    if (config.result == true)
                    {
                        command.execute_result = 1;  //标记为执行成功 
                        command.execute_comment = "执行写操作成功";
                        gongkuang.write_value = command.value;
                        gongkuang.execute_comment = "执行写操作成功";
                    }
                    else
                    {
                        command.execute_result = 0;   //标记为执行失败
                        command.execute_comment = "执行广岱拍照操作失败";
                        gongkuang.execute_comment = "执行广岱拍照操作失败";
                    }

                    gongkuang.execute_datetime = DateTime.Now;
                    command.complete_datetime = DateTime.Now;
                }
                catch (Exception ex)
                {
                    command.execute_result = 0; //标记为执行失败
                    command.execute_comment = "执行广岱拍照操作失败,服务程序出现了异常";
                    // MessageQueue.Enqueue_DataInfo(string.Format("接收时间:【{0}】,站点：{1},   执行工况配置失败.程序出现异常,请查看日志.  ", DateTime.Now, station.name));
                    LogMg.AddError(ex);
                }
                finally
                {
                    command.complete_datetime = DateTime.Now;
                }
                try
                {
                    db.SubmitChanges();
                }
                catch (Exception ex)
                {
                    LogMg.AddError(ex.ToString());

                }
            }
        }


        /// <summary>
        /// 保存图片到数据库
        /// </summary>
        /// <param name="db"></param>
        /// <param name="stationId"></param>
        /// <param name="imageBytes"></param>
        /// <returns></returns>
        private static bool SavePicture(SWSDataContext db, int stationId, byte[] imageBytes)
        {
            try
            {
                picture_info pic = new picture_info();
                pic.station_id = stationId.ToString();
                pic.title = "在线抓拍";
                pic.contents = "在线抓拍";
                pic.username = "";
                pic.images = imageBytes;
                pic.updatetime = DateTime.Now;
                pic.source = "03";
                pic.type = "01";
                db.picture_info.InsertOnSubmit(pic);
                db.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
                return false;
            }
        }



        /// <summary>
        /// 根据Tel(dtu编号,设备唯一编号)获取数据库连接
        /// </summary>
        /// <returns></returns>
        private static SqlConnection GetDbConnectionByTel(String tel)
        {
            try
            {
                //先根据站点id找到orgid  ,再根据orgid找出dbname
                Clazz.Config.XML_Station xml_station = SysConfig.DTU_StationConfig.Stations.SingleOrDefault(c => c.Tel == tel);
                if (xml_station != null && xml_station.Org != null)
                {
                    return ConnectStringHelper.GetConnection(SysConfig.userProfile.DbAddress, xml_station.Org.DBName, SysConfig.userProfile.DbUserName, SysConfig.userProfile.DbPassword);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 判断客户端在【任务队列】和【线程里】里是否已经存在
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private static bool HasExistClientInPool(object client)
        {
            try
            {

                foreach (服务器端接收程序.MyForm.GPRSControl.TaskPool.Waititem item in PoolA.waitlist)
                {
                    if (item.Context.GetType() == typeof(Args))
                    {
                        if (((Args)item.Context).station_id.GetHashCode() == client.GetHashCode())
                        {
                            return true;
                        }
                    }
                    if (item.Context.GetHashCode() == client.GetHashCode())
                    {
                        return true;
                    }
                }
                //if (PoolA.publicpool.Count > 1)
                //{
                foreach (KeyValuePair<string, Task> item in PoolA.publicpool)
                {
                    try
                    {
                        Task task = (Task)item.Value;
                        if (task.contextdata.GetType() == typeof(Args))
                        {
                            if (((Args)task.contextdata).station_id.GetHashCode() == client.GetHashCode())
                            {
                                return true;
                            }
                        }
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





        /// <summary>
        /// 清空缓存
        /// </summary>
        /// <param name="socket"></param>
        private static void ClearSocketCache(Socket socket)
        {
            try
            {
                socket.Receive(buffer, 1024, SocketFlags.None);
            }
            catch
            {
            }
        }
        /// <summary>
        /// 获取对应的wscid
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="station_id"></param>
        /// <param name="links"></param>
        /// <returns></returns>
        public static String getWscId(Args arg, SWSDataContext db, List<guangdai_station_link> links)
        {
            String wscId = "";
            try
            {
                //List<org> orgs = db.org.ToList();
                //guangdai_station_link link = null;
                //if (orgs.Count > 0)
                //{

                List<guangdai_station_link> link = links.Where(c => c.station_id == arg.station_id && c.db_name == arg.dbName && c.type == 1).ToList();
                //}
                if (link != null && link.Count > 0)
                    wscId = link[0].wsid;
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex.ToString());
            }
            return wscId;
        }
    }


    public class Station_transferCode
    {
        public int stationId { get; set; }
        public string transfer_code { get; set; }
    }

    public class Args
    {
        public String dbName { get; set; }
        public int station_id { get; set; }
        public String gdServerCfg { get; set; }
    }

}
