using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Utility;
using 服务器端接收程序.Util;
using 服务器端接收程序.Config;

namespace 服务器端接收程序.MyForm.GPRSControl
{
    /// <summary>
    /// GPRS控制器的网络服务类
    /// </summary>
    public class DTU_NetServer
    {
        ///这个类的功能说明：首先要调用Init方法初始化socket，再调用StartListen在【Socket服务监听线程】中开启监听功能.
        ///然后客户端(DTU)通过网络连接socket服务，然后【Socket服务监听线程】，将这个处理客户端的任务放入本类的线程池】
        ///本类中的线程池的功能：等待客户端发来注册信息，并将客户端添加到客户端列表【DTU_ClientManager】

        /// <summary>
        /// socket主服务
        /// </summary>
        static Socket ServerSocket = null;
        /// <summary>
        /// 监听线程
        /// </summary>
        static Thread RecieveThread = null;
        ///线程池---处理客户端第一次连接并等待注册任务
        public static TaskPool PoolD;

        /// <summary>
        /// 初始化socket
        /// </summary>
        public static void Init()
        {
            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            LogMg.AddDebug("初始化socket成功");

            ///“处理第一次注册并等待注册”的线程池的初始化
            PoolD = new TaskPool();
            PoolD.Name = "D";
            PoolD.Setminthread(SysConfig.userProfile.NetServerThreadPool_Min);
            PoolD.Setmaxthread(SysConfig.userProfile.NetServerThreadPool_Max);
        }
        /// <summary>
        /// 开始监听
        /// </summary>
        /// <param name="ipEndPoint"></param>
        public static bool StartListen(string ip, string port)
        {
            try
            {


                EndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), ushort.Parse(port));
                ServerSocket.Bind(ipEndPoint);
                ServerSocket.Listen(10);
                // LogManager.AddDebug("启动服务成功");

                //开启监听线程
                RecieveThread = new Thread(new ThreadStart(RecieveAccept));//将接受客户端连接的方法委托给线程
                RecieveThread.IsBackground = true;
                RecieveThread.Start();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///接收客户端的连接 
        /// </summary>

        private static void RecieveAccept()
        {
            while (true)
            {
                try
                {
                    Socket clientSocket = ServerSocket.Accept();
                    //LogManager.AddDebug(String.Format("接收到连接"));
                    //这里交给线程池
                    PoolD.AddTaskItem(new WaitCallback(FirstConnect), clientSocket);
                    //   ThreadPool.QueueUserWorkItem(new WaitCallback(FirstConnect), clientSocket);
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 处理第一次注册并等待注册
        /// </summary>
        /// <param name="obj"></param>
        private static void FirstConnect(object obj)
        {
            Socket clientSocket = (Socket)obj;
            //LogManager.AddDebug("连接时Socket.HashCode=" + clientSocket.GetHashCode());
            string protocol = "";
            bool register = false;    //是否已注册 
            DateTime start = DateTime.Now;
            DateTime now = DateTime.Now;
            while (!register && start.AddSeconds(30).CompareTo(now) > 0)
            {
                try
                {
                    byte[] content = new byte[GlobalPara.CONTENT_LENGTH];
                    int conLen = 0;
                    byte type = 0;
                    string tel = "";

                    Util.DTU.rdata(ref protocol, clientSocket, GlobalPara.ReceiveTimeOut, ref content, ref conLen, ref type, ref tel);    //1秒超时   等待注册数据

                    if (Util.DTU.ResponseRegisterOrHeart(protocol, clientSocket, tel))
                    {
                        DTU_ClientManager.AddClient(tel, clientSocket);
                        register = true;
                    }
                    now = DateTime.Now;
                    Thread.Sleep(50);
                }
                catch (SocketException)
                {
                }
                catch (Exception ex)
                {
                    LogMg.AddError(ex);
                }
            }
        }

    }
}
