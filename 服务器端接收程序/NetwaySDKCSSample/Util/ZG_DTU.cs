using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using Utility;

namespace 服务器端接收程序.Util
{
    /// <summary>
    /// 佐格DTU
    /// </summary>
    public class ZG_DTU
    {
        /// <summary>
        /// 心跳数据
        /// </summary>
        public static readonly byte HEARTBEAT_DATA = 0x30;
        /// <summary>
        /// 注册数据
        /// </summary>
        public static readonly byte REGISTER_DATA = 0x31;
        /// <summary>
        /// 用户数据
        /// </summary>
        public static readonly byte USER_DATA = 0x32;

        /// <summary>
        /// 协议头
        /// </summary>
        public static readonly byte HEAD = 0x7b;
        /// <summary>
        /// 协议尾
        /// </summary>
        public static readonly byte END = 0x7d;

        /// <summary>
        /// 打包
        /// </summary>
        /// <param name="content">需要打包的内容</param>
        /// <param name="conLen">需要打包的内容的长度</param>
        /// <param name="pack">打包后的数据</param>
        /// <param name="type">类型</param>
        /// <param name="packLen">打包后的数据的长度</param>    
        /// <param name="DTU_Id">DTU_Id  通常为11位电话号码</param>    
        /// <returns></returns>
        public static void Pack(byte[] content, int conLen, byte type, ref byte[] pack, ref int packLen)
        {
            pack[0] = HEAD;         //头
            pack[1] = type;      //类型 
            byte[] sss = BitConverter.GetBytes(conLen);
            pack[2] = sss[0];
            pack[3] = sss[1];
            Buffer.BlockCopy(content, 0, pack, 4, conLen);   //内容
            pack[4 + +conLen] = END;
            packLen = 4 + +conLen + 1;
        }

        /// <summary>
        /// 拆包
        /// </summary>
        /// <param name="str"></param>
        /// <param name="strcon"></param>
        /// <param name="type"></param>
        /// <param name="strlen"></param>
        /// <param name="conlen"></param>
        /// <returns></returns>
        public static bool UnPack(byte[] pack, int packLen, ref byte[] content, ref int conlen, ref byte type, ref string tel)
        {
            try
            {
                //日志
                string str = "";
                for (int j = 0; j < packLen; j++)
                {
                    str += pack[j] + " ";
                }
                LogMg.AddDebug(str);


                int i;
                for (i = 0; i < pack.Length - 1; i++)
                {
                    if (pack[i] == ZG_DTU.HEAD)
                    {
                        break;
                    }
                }
                type = pack[i + 1];       //类型
                if (type == ZG_DTU.USER_DATA)     //用户数据拆包
                {
                    int low = pack[i + 2];    //低8位
                    int high = pack[i + 3];   //高8位
                    int length = high * 256 + low;  //从手机号码到最后的字节数
                    conlen = length;  //内容长度
                    Buffer.BlockCopy(pack, i + 4, content, 0, conlen);
                }
                else if (type == ZG_DTU.REGISTER_DATA)     //注册数据拆包
                {
                    int low = pack[i + 2];    //低8位
                    int high = pack[i + 3];   //高8位
                    int length = high * 256 + low;  //从手机号码到最后的字节数
                    byte[] byteTel = new byte[11];   //手机号码字节
                    Buffer.BlockCopy(pack, i + 4, byteTel, 0, 11);
                    tel = Encoding.ASCII.GetString(byteTel);
                    conlen = length - 11;  //内容长度 
                    Buffer.BlockCopy(pack, i + 4 + 11, content, 0, conlen);
                }
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="content"></param>
        /// <param name="conLen"></param>
        /// <param name="type"></param>
        public static void Send(Socket socket, byte[] content, int conLen, byte type)
        {
            byte[] pack = new byte[GlobalPara.SEND_DATA_LENGTH];
            int packLen = 0;
            Pack(content, conLen, type, ref pack, ref packLen);
            socket.SendTimeout = GlobalPara.SendTimeOut;    //设置超时
            socket.Send(pack, packLen, SocketFlags.None);
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="timeout"></param>
        /// <param name="content"></param>
        /// <param name="conlen"></param>
        /// <param name="type"></param>
        /// <param name="tel"></param>
        /// <returns></returns>
        public static bool rdata(Socket socket, int timeout, ref byte[] content, ref int conlen, ref byte type, ref string tel)
        {
            byte[] buffer = new byte[GlobalPara.SEND_DATA_LENGTH];  //接收数据的数组 
            socket.ReceiveTimeout = timeout;
            int len = socket.Receive(buffer, buffer.Length, SocketFlags.None);
            return UnPack(buffer, len, ref content, ref conlen, ref type, ref tel);
        }

        /// <summary>
        /// 处理接收到的数据
        /// </summary>
        /// <param name="asyncResult"></param>
        public static void HandlerData(Socket socket, byte[] content, int conLen, byte type, string tel)
        {
            try
            {
                if (type == ZG_DTU.HEARTBEAT_DATA)   //心跳数据
                {
                    HeartbeatDataHandler(socket);
                }
                else if (type == ZG_DTU.REGISTER_DATA)   //注册数据       貌似这里好像不会出现注册数据了,因为注册数据在第一次注册后就不会再注册了
                {
                    DTU_ClientManager.AddClient(tel, socket);
                }
                else if (type == ZG_DTU.USER_DATA)
                {

                }
            }
            catch (Exception ex)
            {
                //LogManager.Add("json字符串:" + data + System.Environment.NewLine + ex.ToString());
                LogMg.AddError(ex);
                DEBUG.ThrowException(ex);
            }
        }


        /// <summary>
        /// 心跳数据处理
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="pack"></param>
        public static bool HeartbeatDataHandler(Socket socket)
        {
            try
            {
                //对心跳做一个回应
                ZG_DTU.Send(socket, new byte[1], 0, ZG_DTU.HEARTBEAT_DATA);
                return true;
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
                DEBUG.MsgBox(ex.ToString());
                return false;
            }
        }

        public static bool CheckProtocolByRegisterData(byte[] pack, int len)
        {
            bool flag = true;
            try
            {
                int i;
                for (i = 0; i < len; i++)
                {
                    if (pack[i] == ZG_DTU.HEAD)    //先找到头
                    {
                        break;
                    }
                }
                byte type = pack[i + 1];
                if (type != ZG_DTU.REGISTER_DATA)    //再判断时否是ZG_DTU的注册类型
                    flag = false;

                int low = pack[i + 2];    //低8位
                int high = pack[i + 3];   //高8位
                int length = high * 256 + low;  //从手机号码到最后的字节数

                if (pack[i + 1 + 2 + length + 1] != END)   //再找到尾   
                {
                    flag = false;
                }

            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
                flag = false;
            }

            return flag;
        }

    }
}
