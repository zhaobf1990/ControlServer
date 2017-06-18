using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using 服务器端接收程序.MyForm.GPRSControl;

namespace 服务器端接收程序.Util
{
    /// <summary>
    /// 检测客户端的连接是否断开
    /// </summary>
    public class CheckConnect
    {
        Thread thread = null;

        /// <summary>
        /// 开始
        /// </summary>
        public void Start()
        {
            thread = new Thread(CheckCon);
            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// 检查客户端连接
        /// </summary>
        private void CheckCon()
        {
            while (true)
            {
                if (DTU_ClientManager.Clients != null)
                {
                    int len = DTU_ClientManager.Clients.Count;
                    for (int i = len - 1; i >= 0; i--)
                    {
                        using (MyLock mylock = new MyLock(DTU_ClientManager.Clients[i], 5000, false))
                        {
                            if (mylock.IsTimeout == false)
                            {
                                try
                                {
                                    int n = DTU_ClientManager.Clients[i].socket.Send(new byte[] { 1 }, SocketFlags.None);
                                }
                                catch (Exception e)
                                {
                                    DTU_ClientManager.DeleteClient(DTU_ClientManager.Clients[i]);
                                }
                            }
                        }

                    }
                }
                Thread.Sleep(20000);//
            }
        }


    }
}
