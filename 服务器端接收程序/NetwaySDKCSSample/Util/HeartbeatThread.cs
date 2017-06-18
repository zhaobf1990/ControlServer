using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utility;
using 服务器端接收程序.Clazz;
using 服务器端接收程序.Util;
using 服务器端接收程序.Config;

namespace 服务器端接收程序.MyForm.GPRSControl
{
    public class HeartbeatThread
    {
        public static TaskPool PoolC;  //线程池   
        private static Thread thread { get; set; }   //循环客户端列表的线程，也就是分配任务的线程

        public static Action<string> MsgHandler;  //定义一个委托，将接收到字节显示到UI主线程上。

        public static void Start()
        {
            PoolC = new TaskPool();
            PoolC.Name = "C";
            PoolC.Setminthread(SysConfig.userProfile.HeartbeatThreadPool_Min);
            PoolC.Setmaxthread(SysConfig.userProfile.HeartbeatThreadPool_Max);

            thread = new Thread(new ThreadStart(Work));
            thread.IsBackground = true;
            thread.Start();
        }


        private static void Work()
        {
            //每隔一段时间，循环客户端列表，对每个客户端列表进行心跳数据的采集。
            while (true)
            {

                int length = DTU_ClientManager.Clients.Count;
                for (int i = 0; i < length; i++)
                {
                    try
                    {
                        if (!HasExistClientInPool(DTU_ClientManager.Clients[i]))
                            PoolC.AddTaskItem(new WaitCallback(Execute), DTU_ClientManager.Clients[i]);
                    }
                    catch (Exception ex)
                    {
                        LogMg.AddError(ex);
                    } 
                }

                Thread.Sleep((int)SysConfig.userProfile.HeartbeatInterval);
            }
        }

        private static void Execute(object state)
        {
            //锁住客户端连接，接收客户端的心跳数据，
            //如果没有心跳数据则跳过，
            //如果有心跳数据，则将客户端更新到客户端列表。

            DTUClientInfo client = (DTUClientInfo)state;
            DTU_ClientManager.UpdateLastVisitTime(client, DateTime.Now);
            using (MyLock mylock = new MyLock(client, 3000, false))
            {
                if (mylock.IsTimeout == false)   //判断锁是否成功，如果没有超时，则表示锁成功
                {
                    try
                    {
                        byte[] content = new byte[GlobalPara.CONTENT_LENGTH];
                        int conLen = 0;
                        byte type = 0;
                        string tel = "";
                        string protocol = "";
                        Util.DTU.rdata(ref protocol, client.socket, GlobalPara.ReceiveTimeOut, ref content, ref conLen, ref type, ref tel);    //1秒超时

                        Util.DTU.HandlerData(protocol, client.socket, content, conLen, type, tel);

                        byte[] pack = new byte[1024 * 2];
                        client.socket.ReceiveTimeout = 50;

                        int packLen = client.socket.Receive(pack, SocketFlags.None);
                        if (MsgHandler != null)
                        {
                            MsgHandler("收到的心跳：" + CommonUtil.byteToHexStr(pack, packLen));
                        }

                        ////判断收到的数据是不是心跳   如果是心跳数据，则解析出gprsId，并更新客户端列表 
                        //int gprsId = 0;
                        //bool flag = GPRS_Protocol.UnPack_Heartbeat(pack, packLen, ref gprsId);
                        //if (flag)
                        //{
                        //    ClientManager.AddClient(client.socket, gprsId.ToString());
                        //    //pack[1] = Protocol.Type_Heartbeat_StoC;
                        //    //这里重新组个返回心跳包。
                        //    BackHeartBeat(client.socket, gprsId);
                        //}

                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
        /// <summary>
        /// 给客户端回一个心跳确认包
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="gprsId"></param>
        private static void BackHeartBeat(Socket socket, int gprsId)
        {
            byte[] pack = new byte[100];
            int packLen = 0;
            byte[] b_id = CommonUtil.IntToByteArr(gprsId);
            GPRS_Protocol.Pack(b_id, b_id.Length, GPRS_Protocol.Type_Heartbeat_StoC, GPRS_Protocol.Channel_None, ref pack, ref packLen);
            socket.Send(pack, packLen, SocketFlags.None);
        }

        /// <summary>
        /// 判断客户端在【任务队列】和【线程里】里是否已经存在
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private static bool HasExistClientInPool(DTUClientInfo client)
        {
            try
            {
                foreach (TaskPool.Waititem item in PoolC.waitlist)
                {
                    DTUClientInfo content = (DTUClientInfo)item.Context;
                    if (content.GetHashCode() == client.GetHashCode())
                    {
                        return true;
                    }
                }
                //if (PoolC.publicpool.Count > 1)
                //{ 
                    foreach (KeyValuePair<string, Task> item in PoolC.publicpool)
                    {
                        try
                        {
                            Task task = (Task)item.Value;
                            DTUClientInfo content = (DTUClientInfo)task.contextdata;
                            if (content.GetHashCode() == client.GetHashCode())
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
