using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace 服务器端接收程序.MyForm.GPRSControl
{
    public class GPRS_Protocol
    {
        private static readonly byte Head = 0x7b;
        private static readonly byte End = 0x7d;

        #region Type
        /// <summary>
        /// 心跳数据: 客户端-->服务器
        /// </summary>
        public static readonly byte Type_Heartbeat_CtoS = 0x30;
        /// <summary>
        /// 心跳数据: 服务器-->客户端
        /// </summary>
        public static readonly byte Type_Heartbeat_StoC = 0x31;
        /// <summary>
        /// 串口1：连接Modbus-RTU设备：服务器-->客户端
        /// </summary>
        public static readonly byte Type_Serial1_StoC = 0x32;
        /// <summary>
        /// 串口1：连接Modbus-RTU设备：客户端-->服务器
        /// </summary>
        public static readonly byte Type_Serial1_CtoS = 0x33;
        /// <summary>
        /// 串口2：连接摄像头1：服务器-->客户端
        /// </summary>
        public static readonly byte Type_Serial2_StoC = 0x34;
        /// <summary>
        /// 串口2：连接摄像头1：客户端-->服务器
        /// </summary>
        public static readonly byte Type_Serial2_CtoS = 0x35;
        /// <summary>
        /// 串口3：连接摄像头2：服务器-->客户端
        /// </summary>
        public static readonly byte Type_Serial3_StoC = 0x36;
        /// <summary>
        /// 串口3：连接摄像头2：客户端-->服务器
        /// </summary>
        public static readonly byte Type_Serial3_CtoS = 0x37;
        /// <summary>
        /// 控制器内存：服务器-->客户端
        /// </summary>
        public static readonly byte Type_ControllerMemory_StoC = 0x38;
        /// <summary>
        /// 控制器内存：客户端-->服务器
        /// </summary>
        public static readonly byte Type_ControllerMemory_CtoS = 0x39;
        #endregion

        #region 通道
        #region 通道

        public static readonly byte Channel_None = 0x00;
        /// <summary>
        /// 串口1
        /// </summary>
        public static readonly byte Channel_1 = 0x01;
        /// <summary>
        /// 串口2
        /// </summary>
        public static readonly byte Channel_2 = 0x02;
        /// <summary>
        /// 串口3
        /// </summary>
        public static readonly byte Channel_3 = 0x03;
        /// <summary>
        /// 控制器内存
        /// </summary>
        public static readonly byte Channel_4 = 0x04;
        #endregion
        #endregion

        /// <summary>
        /// 打包
        /// </summary>
        /// <param name="content">需要打包的内容</param>
        /// <param name="conLen">内容的长度</param>
        /// <param name="type">类型</param>
        /// <param name="channel">通道</param>
        /// <param name="pack">打包后的数据</param>
        /// <param name="packLen">打包后的包长度</param>
        public static void Pack(byte[] content, int conLen, byte type, byte channel, ref byte[] pack, ref int packLen)
        {
            pack = new byte[5 + conLen + 1];
            pack[0] = Head;
            pack[1] = type;
            pack[2] = channel;
            byte[] s = BitConverter.GetBytes(conLen);
            pack[3] = s[1];
            pack[4] = s[0];
            Buffer.BlockCopy(content, 0, pack, 5, conLen);
            pack[5 + conLen] = End;
            packLen = 5 + conLen + 1;
        }

        public static void UnPack(byte[] pack, int packLen, ref byte[] content, ref  int conLen, ref byte type, ref byte channel)
        {
            type = pack[1];
            channel = pack[2];
            conLen = pack[3] * 256 + pack[4];
            Buffer.BlockCopy(pack, 5, content, 0, conLen);
        }
        /// <summary>
        /// 解析心跳包
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="packLen"></param>
        /// <param name="content"></param>
        /// <param name="conLen"></param>
        /// <param name="type"></param>
        /// <param name="channel"></param>
        public static bool UnPack_Heartbeat(byte[] pack, int packLen, ref int gprsId)
        {
            try
            {
                int index = 0;
                //找出包头的下标
                for (index = 0; index < packLen - 1; index++)
                {
                    if (pack[index] == Head)   //找到包头
                    {
                        if (pack[index + 1] == Type_Heartbeat_CtoS || pack[index + 1] == Type_Heartbeat_StoC)    //类型为心跳
                        {
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }

                int conLen = pack[index + 3] * 256 + pack[index + 4];
                byte[] content = new byte[conLen];
                Buffer.BlockCopy(pack, index + 5, content, 0, conLen);
                gprsId = 0;
                //将字节数组转换成int
                for (int i = 0; i < conLen; i++)
                {
                    gprsId = gprsId * 256 + content[i];
                }
                //如果下标没有超过包长，则表示解析成功    否则表示失败
                if (index + 6 + conLen <= packLen)    //心跳包的长度=6个固定字节+id的长度
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception)  //这里可能会捕获到数组越界等异常    都代表解析包失败。
            {
                return false;
            }
        }


        public static int Send(Socket socket, byte channel, byte[] content, int contentLen)
        {
            byte[] pack = new byte[50];
            int packLen = 0;
            if (channel == Channel_1)
            {
                Pack(content, contentLen, Type_Serial1_StoC, Channel_1, ref pack, ref packLen);
            }
            else if (channel == Channel_2)
            {
                Pack(content, contentLen, Type_Serial2_StoC, Channel_2, ref pack, ref packLen);
            }
            else if (channel == Channel_3)
            {
                Pack(content, contentLen, Type_Serial3_StoC, Channel_3, ref pack, ref packLen);
            }
            else if (channel == Channel_4)
            {
                Pack(content, contentLen, Type_ControllerMemory_StoC, Channel_4, ref pack, ref packLen);
            }
            int n = socket.Send(pack, packLen, SocketFlags.None);
            return n;
        }

    }
}
