using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using Utility;

namespace 服务器端接收程序.Util
{
    /// <summary>
    /// 宏电DTU
    /// </summary>
    public class HD_DTU
    {
        /// <summary>
        /// 协议头
        /// </summary>
        private static readonly byte HEAD = 0x7b;
        /// <summary>
        /// 协议尾
        /// </summary>
        private static readonly byte END = 0x7b;

        private static int ReceiveLength = 1024;

        #region DSC端协议包类型
        public static readonly byte DSC_REGISTER_DATA = 0x81;
        public static readonly byte DSC_User_Data = 0x89;
        #endregion
        #region DTU端协议包类型
        public static readonly byte DTU_REGISTER_DATA = 0x01;
        public static readonly byte DTU_User_Data = 0x09;


        #endregion
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
            byte[] buffer = new byte[HD_DTU.ReceiveLength];  //接收数据的数组 
            socket.ReceiveTimeout = timeout;
            int packLen = socket.Receive(buffer, buffer.Length, SocketFlags.None);
            return UnPack(buffer, packLen, ref content, ref conlen, ref type, ref tel);
        }


        /// <summary> 
        /// 发送数据 
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="pack"></param>
        /// <param name="packLen"></param>
        /// <returns></returns>
        public static int Send(Socket socket, byte[] pack, int packLen)
        {
            socket.SendTimeout = GlobalPara.SendTimeOut;    //设置超时
            return socket.Send(pack, packLen, SocketFlags.None);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="content"></param>
        /// <param name="conLen"></param>
        /// <param name="type"></param>
        public static int Send(Socket socket, byte[] content, int conLen, byte type, string tel)
        {

            byte[] pack = new byte[GlobalPara.SEND_DATA_LENGTH];
            int packLen = 0;
            if (type == DSC_User_Data)
            {
                PackUserData(content, conLen, tel, ref pack, ref packLen);
            }
            int n = Send(socket, pack, packLen);
            return n;
        }

        public static void HandlerData(Socket clientSocket, byte[] content, int conLen, byte type, string tel)
        {
            if (type == HD_DTU.DTU_REGISTER_DATA)
            {
                DTU_ClientManager.AddClient(tel, clientSocket);
                ResponseRegister(clientSocket, tel);
            }
        }

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
        public static void PackUserData(byte[] content, int conLen, string tel, ref byte[] pack, ref int packLen)
        {
            pack[0] = HEAD;         //头
            pack[1] = DSC_User_Data;      //类型 
            packLen = conLen + 1 + 1 + 2 + 11 + 1;
            byte[] sss = BitConverter.GetBytes(packLen);
            pack[2] = sss[1];     //高位在前
            pack[3] = sss[0];     //低位在后

            //tel
            byte[] b_tel = new byte[11];
            b_tel = Encoding.ASCII.GetBytes(tel);
            Buffer.BlockCopy(b_tel, 0, pack, 4, 11);


            Buffer.BlockCopy(content, 0, pack, 15, conLen);   //内容
            pack[15 + conLen] = END; //协议尾
        }

        /// <summary>
        /// 检查注册数据是否满足此协议
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static bool CheckProtocolByRegisterData(byte[] pack, int len)
        {
            bool flag = true;
            try
            {
                int i;
                for (i = 0; i < len; i++)
                {
                    if (pack[i] == HEAD)    //先找到头
                    {
                        break;
                    }
                }
                byte type = pack[i + 1];
                if (type != DTU_REGISTER_DATA)    //再判断时否是ZG_DTU的注册类型
                    flag = false;

                int high = pack[i + 2];    //高8位
                int low = pack[i + 3];   //低8位
                int length = high * 256 + low;  //从手机号码到最后的字节数

                if (pack[i + length - 1] != END)   //再找到尾   
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


        /// <summary>
        /// 拆包 
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="pack"></param>
        /// <param name="packLen"></param>
        /// <param name="content"></param>
        /// <param name="conLen"></param>
        /// <param name="type"></param>
        /// <param name="tel"></param>
        /// <returns></returns>
        public static bool UnPack(byte[] pack, int packLen, ref byte[] content, ref int conLen, ref byte type, ref string tel)
        {
            try
            {
                int i;
                for (i = 0; i < packLen; i++)
                {
                    if (pack[i] == HD_DTU.HEAD)
                    {
                        break;
                    }
                }
                type = pack[i + 1];       //类型
                if (type == HD_DTU.DTU_REGISTER_DATA)     //注册数据拆包和  
                {
                    //解析接收到的数据 
                    int high = pack[i + 2];    //高8位
                    int low = pack[i + 3];   //低8位
                    int length = high * 256 + low;  //从手机号码到最后的字节数

                    byte[] b_tel = new byte[11];
                    Buffer.BlockCopy(pack, i + 4, b_tel, 0, 11);
                    tel = Encoding.ASCII.GetString(b_tel);


                }
                else if (type == HD_DTU.DTU_User_Data)     //用户数据拆包   
                {
                    //解析接收到的数据 
                    int high = pack[i + 2];    //高8位
                    int low = pack[i + 3];   //低8位
                    int length = high * 256 + low;  //从手机号码到最后的字节数

                    byte[] b_tel = new byte[11];
                    Buffer.BlockCopy(pack, i + 4, b_tel, 0, 11);
                    tel = Encoding.ASCII.GetString(b_tel);

                    conLen = length - 16;
                    Buffer.BlockCopy(pack, i + 4 + 11, content, 0, conLen);
                }
                else
                {
                    //日志
                    string str = "";
                    for (int j = 0; j < packLen; j++)
                    {
                        str += pack[j] + " ";
                    }
                    LogMg.AddDebug("收到宏电DTU未知类型的数据:" + str);
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
        /// 当收到注册信息无误后, 应调用此方法回应DTU
        /// </summary>
        public static bool ResponseRegister(Socket socket, string tel)
        {
            try
            {
                //应答客户端
                byte[] sendBytes = new byte[16];
                sendBytes[0] = HD_DTU.HEAD;
                sendBytes[1] = HD_DTU.DSC_REGISTER_DATA;
                sendBytes[2] = 0;
                sendBytes[3] = 0x10;
                byte[] b_tel = Encoding.ASCII.GetBytes(tel);
                Buffer.BlockCopy(b_tel, 0, sendBytes, 4, 11);
                sendBytes[4 + 11] = HD_DTU.END;
                int n = Send(socket, sendBytes, 16);
                return n > 0;
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
                return false;
            }
        }
    }

}
