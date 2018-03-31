using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Utility;

namespace 服务器端接收程序.Util
{
    class UdpV88Server
    {
        private UdpClient udpcRecv;
        private Thread thrRecv;
        static UdpV88Server udpServer;

        public static UdpV88Server getInstance()
        {
            if (udpServer == null)
            {
                udpServer = new UdpV88Server();
            } return udpServer;
        }

        private UdpV88Server()
        {
        }

        public void StartListen(string ip, string port)
        {
            IPEndPoint localIpep = new IPEndPoint(
                  IPAddress.Parse(ip), int.Parse(port)); // 本机IP和监听端口号

            udpcRecv = new UdpClient(localIpep);

            thrRecv = new Thread(ReceiveMessage);
            thrRecv.Start();
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="obj"></param>
        private void ReceiveMessage(object obj)
        {
            IPEndPoint remoteIpep = new IPEndPoint(IPAddress.Any, 0);
            while (true)
            {
                try
                {
                    byte[] bytRecv = udpcRecv.Receive(ref remoteIpep);
                  //  string message = Encoding.ASCII.GetString(bytRecv, 0, bytRecv.Length);
                  

                    V88CommunicationThread.getInstance().receiveTask(bytRecv);
                  //  LogManager.AddDebug("入口接收的数据=============     " + message);

                    byte[] sendBtyes = Encoding.ASCII.GetBytes("@222");  //需要回发这几个字符
                    udpcRecv.Send(sendBtyes, sendBtyes.Length, remoteIpep);
                }
                catch (Exception ex)
                {
                    LogMg.AddError(ex.ToString());
                    break;
                }
            }
        }

        public void close()
        {
            if (udpcRecv != null)
            {
                udpcRecv.Close();
            }
            if (thrRecv != null)
            {
                thrRecv.Abort();
            }
        }
    }
}
