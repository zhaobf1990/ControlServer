using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Data.SqlClient;
using System.Threading;
using Utility;
using CSDataStandard;
using System.Windows.Forms;
using CSDataStandard.Enum;
using 服务器端接收程序.Config;
using 服务器端接收程序.Clazz.Config.ClientConfig;

namespace 服务器端接收程序.Util
{
    public class ServerSocketHelper
    {

        #region  数据库服务服务器配置

        /// <summary>
        /// 获取分厂数据库连接实例
        /// </summary>
        /// <param name="db_name"></param>
        /// <returns></returns>
        public static SqlConnection GetConnection(string dbname)
        {
            string db_address = SysConfig.userProfile.DbAddress;
            string db_username = SysConfig.userProfile.DbUserName;
            string db_password = SysConfig.userProfile.DbPassword;
            return Utility.ConnectStringHelper.GetConnection(db_address, dbname, db_username, db_password);
        }

        #endregion


        public static string serverIp;

        public static ushort DBCJ_Port;
        public static ushort Country_Port;
        public static ushort PMQC_Port;

        #region socket服务

        /// <summary>
        /// ServerSocket 服务端socket
        /// </summary>
        private static Socket ServerSocket = null;
        private static Thread RecieveThread;
        private static byte[] MsgBuffer = new byte[65535];  //接收数据的数组
        private static byte[] SendBuffer = new byte[65535];  //发送数据的数组

        public static Action DBCJInit = null;
        public static Action DBCJClose = null;
        public static Action ClientConnectHandler = null;          //客户端连接
        public static Action<Socket, string> CountryCallBack = null;
        public static Action<Socket, string> DeviceControlCallBack = null;  //设备控制
        public static Action<Socket, string> PMQCCallBack = null;  //接收屏幕取词数据
        public static Action<Socket, string> MobileDetectionCallBack = null;  //接收移动检测数据
        public static Action<Socket, string> ClientPublicIpCallBack = null;  //接收客户端公网IP
        public static List<Clazz.ClientInfo> ClientSockets = new List<Clazz.ClientInfo>();
        public static Action ClientChangeHander = null;
        /// <summary>
        /// 初始化socket
        /// </summary>
        public static void Init()
        {
            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            if (DBCJInit != null)
            {
                DBCJInit();
            }
        }

        /// <summary>
        /// 关闭socket
        /// </summary>
        public static void Close()
        {
            if (ServerSocket != null)
            {
                ServerSocket.Close();
            }
            if (RecieveThread != null)
            {
                RecieveThread.Abort();
            }

            if (DBCJClose != null)
            {
                DBCJClose();
            }
        }

        /// <summary>
        /// 开始监听
        /// </summary>
        /// <param name="ipEndPoint"></param>
        public static void StartListen(string ip, string port)
        {
            EndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), ushort.Parse(port));
            ServerSocket.Bind(ipEndPoint);
            ServerSocket.Listen(10);

            //开启监听线程
            RecieveThread = new Thread(new ThreadStart(RecieveAccept));//将接受客户端连接的方法委托给线程
            RecieveThread.IsBackground = true;
            RecieveThread.Start();
        }

        // 接收客户端的连接
        private static void RecieveAccept()
        {
            while (true)
            {
                try
                {
                    Socket clientSocket = ServerSocket.Accept();

                    clientSocket.BeginReceive(MsgBuffer, 0, MsgBuffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallBack), clientSocket);

                    if (ClientConnectHandler != null)
                    {
                        ClientConnectHandler();  //执行委托 
                    }
                }
                catch (Exception ex)
                {
                    LogMg.AddError(ex);
                    DEBUG.ThrowException(ex);
                }
            }
        }

        //接收到客户端发来的数据,并向客户端返回消息
        private static void RecieveCallBack(IAsyncResult ar)
        {
            Socket RSocket = null;
            string json = string.Empty;
            try
            {
                RSocket = (Socket)ar.AsyncState;
                int REnd = RSocket.EndReceive(ar);
                json = Encoding.Unicode.GetString(MsgBuffer, 0, REnd);

                if (string.IsNullOrEmpty(json))
                {
                    LogMg.AddError("服务器接收到的数据为空");
                    return;
                }

                C_To_S_Data<object> obj = Utility.JsonHelper.JsonDeserialize<C_To_S_Data<object>>(json);

                AddClient(obj.OrgId, obj.StationId);   //添加或更新客户端信息

                if (obj.Flag == HandleFlag.Country)
                {
                    if (CountryCallBack != null)
                        CountryCallBack(RSocket, json);
                }
                else if (obj.Flag == HandleFlag.DeviceControl)
                {
                    if (DeviceControlCallBack != null)
                        DeviceControlCallBack(RSocket, json);
                }
                else if (obj.Flag == HandleFlag.PMQC)
                {
                    if (PMQCCallBack != null)
                        PMQCCallBack(RSocket, json);
                }
                else if (obj.Flag == HandleFlag.MobileDetection)
                {
                    if (MobileDetectionCallBack != null)
                        MobileDetectionCallBack(RSocket, json);
                }
                else if (obj.Flag == HandleFlag.ClientPublicIp)
                {
                    if (ClientPublicIpCallBack != null)
                        ClientPublicIpCallBack(RSocket, json);
                }
                else if (obj.Flag == HandleFlag.DownLoadConfig)
                {
                    DownLoadConfig.DownLoad(RSocket, json);
                }
            }
            catch (Exception ex)
            {
                LogMg.AddError("json字符串:" + json + System.Environment.NewLine + ex.ToString());
                LogMg.AddError(ex.ToString());
                DEBUG.ThrowException(ex);
            }
        }

        #endregion



        /// <summary>
        /// 添加客户端
        /// </summary>
        private static void AddClient(string orgId, int stationId)
        {
            try
            {
                //如果站点Id为0   那就不用执行下去了
                if (stationId != 0)
                {
                    Clazz.Config.ClientConfig.XML_Station XML_station = SysConfig.clientConfig.AllStation.SingleOrDefault(c => c.StationId == stationId && c.OrgId == orgId);
                    if (XML_station != null)
                    {
                        lock (ClientSockets)
                        {
                            Clazz.ClientInfo client = ClientSockets.SingleOrDefault(c => c.TransferCode == XML_station.TransferCode);
                            if (client == null)
                            {
                                Clazz.Config.XML_Org _org = SysConfig.orgConfig.GetOrgByOrgId(orgId);
                                if (_org == null)
                                {
                                    //将信息写入到日志文件中    orgid为***的污水厂不存在 
                                    LogMg.AddError(String.Format("OrgId:{0}  不存在", orgId));
                                    //isSuccess = false;
                                }
                                else
                                {
                                    SWSDataContext db = new SWSDataContext(GetConnection(_org.DBName));
                                    string name = "未知的客户端";
                                    country_station station = db.country_station.SingleOrDefault(c => c.id == stationId);
                                    if (station != null)
                                    {
                                        name = station.name;
                                    }

                                    client = new Clazz.ClientInfo();
                                    client.TransferCode = XML_station.TransferCode;
                                    client.StationId = stationId;
                                    client.RegisterTime = DateTime.Now;
                                    client.LastVisitTime = DateTime.Now;
                                    client.Name = name;
                                    ClientSockets.Add(client);
                                }
                            }
                            else
                            {
                                client.RegisterTime = DateTime.Now;
                                client.LastVisitTime = DateTime.Now;
                            }
                        }
                    }

                    ///执行委托
                    if (ClientChangeHander != null)
                        ClientChangeHander();
                }
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
            }


        }





        ///// <summary>
        ///// 添加客户端
        ///// </summary>
        //private static void AddClient(string orgId, int stationId)
        //{
        //    //如果站点Id为0   那就不用执行下去了
        //    if (stationId != 0)
        //    {
        //        lock (ClientSockets)
        //        {
        //            Clazz.ClientInfo client = ClientSockets.SingleOrDefault(c => c.StationId == stationId);
        //            if (client == null)
        //            {
        //                Clazz.Config.XML_Org _org = SysConfig.orgConfig.GetOrgByOrgId(orgId);

        //                if (_org == null)
        //                {
        //                    //将信息写入到日志文件中    orgid为***的污水厂不存在 
        //                    LogManager.AddError(String.Format("OrgId:{0}  不存在", orgId));
        //                    //isSuccess = false;
        //                }
        //                else
        //                {
        //                    SWSDataContext db = new SWSDataContext(GetConnection(_org.DBName));
        //                    string name = "未知的客户端";
        //                    country_station station = db.country_station.SingleOrDefault(c => c.id == stationId);
        //                    if (station != null)
        //                    {
        //                        name = station.name;
        //                    }

        //                    client = new Clazz.ClientInfo();
        //                    client.StationId = stationId;
        //                    client.RegisterTime = DateTime.Now;
        //                    client.LastVisitTime = DateTime.Now;
        //                    client.Name = name;
        //                    ClientSockets.Add(client);
        //                }
        //            }
        //            else
        //            {
        //                client.RegisterTime = DateTime.Now;
        //                client.LastVisitTime = DateTime.Now;
        //            }
        //        }

        //        ///执行委托
        //        if (ClientChangeHander != null)
        //            ClientChangeHander();
        //    }
        //}
    }
}