using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using Utility;
using 服务器端接收程序.Clazz;
using 服务器端接收程序.Config;
using 服务器端接收程序.Clazz.Config;


namespace 服务器端接收程序.Util
{
    /// <summary>
    /// DTU客户端连接的管理类
    /// </summary>
    public class DTU_ClientManager
    {
        public delegate void ClientsChange();

        public static List<Clazz.DTUClientInfo> Clients = new List<Clazz.DTUClientInfo>();

        public static Action ClientChangeHander = null;


        /// <summary>
        /// 添加客户端连接    (注册客户端)
        /// </summary>
        /// <param name="tel">电话号码</param>
        /// <param name="socket">socket实例</param>
        /// <param name="ip">客户端DTU的IP</param>
        /// <param name="port">客户端DTU端口</param>
        public static void AddClient(string tel, Socket socket)
        {
            LogMg.AddDebug(string.Format("进入AddClient方法  tel:{0}", tel));
            Clazz.DTUClientInfo clientInfo = Clients.SingleOrDefault(c => c.TelOrGprsId == tel);
            XML_Station station = SysConfig.DTU_StationConfig.Stations.SingleOrDefault(c => c.Tel == tel);

            lock (Clients)
            {
                if (clientInfo == null) //如果客户端不存在，则添加，如果存在，则修改。
                {
                    clientInfo = new Clazz.DTUClientInfo();
                    clientInfo.TelOrGprsId = tel;   //手机号码
                    clientInfo.socket = socket;   //客户端连接实例
                    clientInfo.RegisterTime = DateTime.Now;    //注册时间
                    clientInfo.LastVisitTime = DateTime.Now;    //最后一次访问时间  
                    if (station != null)
                    {
                        clientInfo.StationId = station.StationId;  //站点id
                        clientInfo.Protocol = station.Protocol;
                    }
                    Clients.Add(clientInfo);
                }
                else
                {
                    clientInfo.socket = socket;
                    clientInfo.LastVisitTime = DateTime.Now;    //最后一次访问时间
                }
                if (ClientChangeHander != null)
                {
                    ClientChangeHander();
                }
            }
            LogMg.AddDebug(string.Format("退出AddClient方法  tel:{0}", tel));
        }

        /// <summary>
        /// 更新客户端的最后一次访问时间
        /// </summary>
        /// <param name="clientInfo"></param>
        /// <param name="datetime"></param>
        public static void UpdateLastVisitTime(DTUClientInfo clientInfo, DateTime datetime)
        {
            clientInfo.LastVisitTime = datetime;
            if (ClientChangeHander != null)
            {
                ClientChangeHander();
            }
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="clientInfo"></param>
        /// <param name="datetime"></param>
        public static void Update()
        {
            int length = Clients.Count;
            try
            {
                for (int i = 0; i < length; i++)
                {
                    Clazz.Config.XML_Station station = SysConfig.DTU_StationConfig.GetStationByTel(Clients[i].TelOrGprsId);
                    if (station != null)
                    {
                        Clients[i].Protocol = station.Protocol;
                        Clients[i].StationId = station.StationId;
                    }
                    else
                    {
                        Clients[i].Protocol = "";
                        Clients[i].StationId = 0;
                    }
                }
            }
            catch (Exception)
            {
            }
            if (ClientChangeHander != null)
            {
                ClientChangeHander();
            }
        }

        /// <summary>
        /// 删除客户端注册信息
        /// </summary>
        /// <param name="clientInfo"></param>
        public static void DeleteClient(Clazz.DTUClientInfo clientInfo)
        {
            lock (Clients)
            {
                Clients.Remove(clientInfo);
                LogMg.AddDebug("删除客户端注册信息  TEL:" + clientInfo.TelOrGprsId);
                // MessageQueue.Enqueue_RegAndLogout(string.Format("【{0}】: {1}退出系统", DateTime.Now.ToString(), clientInfo.Name));
                if (ClientChangeHander != null)
                {
                    ClientChangeHander();
                }
            }
        }

        /// <summary>
        /// 删除客户端注册信息
        /// </summary>
        /// <param name="clientInfo"></param>
        public static void DeleteAllClient()
        {
            lock (Clients)
            {
                Clients.RemoveAll(c => true);  //删除所有客户端连接
                LogMg.AddDebug("删除所有客户端连接");
                //   MessageQueue.Enqueue_RegAndLogout("所有的客户端连接都已断开");
                if (ClientChangeHander != null)
                {
                    ClientChangeHander();
                }
            }
        }



        /// <summary>
        /// 根据电话号码找出站点名称
        /// </summary>
        /// <param name="tel"></param>
        /// <returns></returns>
        public static string GetStationNameByTel(string tel)
        {
            string stationName = string.Empty;
            Clazz.Config.XML_Station station = SysConfig.DTU_StationConfig.GetStationByTel(tel);
            if (station != null)
            {
                stationName = station.Name;
            }
            else
            {
                bool ExistStation = false;
                foreach (XML_Org item in SysConfig.orgConfig.Orgs)
                {
                    try
                    {
                        SWSDataContext db = new SWSDataContext(ServerSocketHelper.GetConnection(item.DBName));
                        country_station s = db.country_station.SingleOrDefault(c => c.jiankongyitiji_version == 2 && c.transfer_code == tel);
                        if (s != null)
                        {
                            ExistStation = true;
                            stationName = s.name;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogMg.AddError(ex);
                    }
                }
                if (ExistStation == false)
                {
                    stationName = "未知的客户端";
                }
            }
            return stationName;
        }

        /// <summary>
        /// 根据socket 查找这个站点所使用的DTU采用的传输协议
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public static string GetProtocolBySocket(Socket socket)
        {
            string protocol = "";
            try
            {
                DTUClientInfo client = Clients.SingleOrDefault(c => c.socket == socket);
                if (client != null)
                {
                    protocol = client.Protocol;
                }
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
            }
            return protocol;
        }

        /// <summary>
        /// 客户端注册
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="pack"></param>
        /// <param name="length"></param>
        public static void RegisterDataHandler(Socket socket, string tel)
        {
            LogMg.AddDebug(string.Format("进入客户端注册方法RegisterDataHandler    Tel:{0}", tel));
            try
            {
                //  string stationName = DTU_ClientManager.GetStationNameByTel(tel);
                //  if (string.IsNullOrEmpty(stationName))
                //  {
                // MessageQueue.Enqueue_RegAndLogout(string.Format("【{0}】: {1}登录系统", DateTime.Now.ToString(), "未知的客户端"));
                //  }
                // else
                // {
                // MessageQueue.Enqueue_RegAndLogout(string.Format("【{0}】: {1}登录系统", DateTime.Now.ToString(), stationName));
                // }

                DTU_ClientManager.AddClient(tel, socket);
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
            }
            LogMg.AddDebug(string.Format("退出客户端注册方法RegisterDataHandler    Tel:{0}", tel));
        }
    }
}
