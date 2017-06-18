using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using Utility;

namespace 服务器端接收程序.Util
{
    /// <summary>
    /// 金博通DTU
    /// </summary>
    public class JBT_DTU
    {
        ///摄像头是备用串口，另一个是主串口
        ///// <summary>
        ///// 主用串口  
        ///// </summary>
        //public readonly int Master = 0;
        ///// <summary>
        ///// 备用串口
        ///// </summary>
        //public readonly int Master = "Master";


        #region function_code 功能码
        /// <summary>
        /// DTU主串口向服务器上传数据
        /// </summary>
        public static readonly byte DTU_to_Server_SendData_Master = 0x01;
        /// <summary>
        /// DTU向服务器发送注册或心跳数据
        /// </summary>
        public static readonly byte DTU_to_Server_Register = 0x02;
        /// <summary>
        /// 服务器向DTU响应注册或心跳数据
        /// </summary>
        public static readonly byte Server_to_DTU_Register = 0x03;
        /// <summary>
        /// DTU备用串口向服务器上传数据
        /// </summary>
        public static readonly byte DTU_to_Server_SendData_Assistant = 0x11;
        /// <summary>
        /// 服务器向DTU备用串口下发数据
        /// </summary>
        public static readonly byte Server_to_DTU_SendData_Assistant = 0x12;

        #endregion

        /// <summary>
        /// 帧头0
        /// </summary>
        public static readonly byte Head0 = 0xA8;
        /// <summary>
        /// 帧头1
        /// </summary>
        public static readonly byte Head1 = 0x81;

        public static readonly int ReceiveLength = 4096;  //理论上一个包只有1028，但考虑到可能会发生多个包在一起的现象，所以接收长度就给得大了点

        /// <summary>
        /// 打包备用串口用户数据    也就是摄像头的数据
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="contentLen">内容长度</param>
        /// <param name="SerialNum">主用串口还是备用串口，这两个串口的协议一点区别</param>
        private static bool PackUserData_Master(byte[] content, int contentLen, string tel, ref byte[] pack, ref int packLen)
        {
            pack[0] = Head0;
            pack[1] = Head1;
            if (tel.Length != 11) //手机号必须是11位的
            {
                return false;
            }
            byte[] b_tel = Encoding.ASCII.GetBytes(tel);
            Buffer.BlockCopy(b_tel, 0, pack, 2, 11);
            pack[13] = 0x30;//区号
            pack[14] = 0x30;//区号  
            pack[15] = 0x30;//区号
            pack[16] = 0x30;//区号
            pack[17] = Server_to_DTU_SendData_Assistant; //功能码
            pack[18] = 0x00;   //在配置DTU的时候区号一律不启用
            GenerateCheckCode(pack);
            Buffer.BlockCopy(content, 0, pack, 20, contentLen);
            packLen = 20 + contentLen;
            return true;
        }

        public static bool UnPack(byte[] pack, int packLen, ref byte[] content, ref int conLen, ref byte type, ref string tel)
        {
            try
            {
                int i;
                for (i = 0; i < packLen; i++)
                {
                    if (pack[i] == JBT_DTU.Head0 && pack[i + 1] == JBT_DTU.Head1)
                    {
                        break;
                    }
                }
                tel = Encoding.ASCII.GetString(pack, i + 2, 11);
                //手机号后四位是区号， 没什么用，就不读取了
                type = pack[i + 2 + 11 + 4];

                Buffer.BlockCopy(pack, 20, content, 0, packLen - 20);
                conLen = packLen - 20;

                //处理多个包的情况 ，暂时不处理  ，有空了再写
                //int contentIndex;
                //for (contentIndex = 20; i < packLen - 1; i++)
                //{
                //    if (pack[i] == JBT_DTU.Head0 && pack[i + 1] == JBT_DTU.Head1)
                //    {
                //        if (i+2)
                //            break;
                //    }
                //}
                //if (i == (packLen - 1))   //pack中只有一个包
                //{
                //    Buffer.BlockCopy(pack, 20, content, 0, packLen - 20);
                //    conLen = packLen - 20;
                //}
                //else    //当出现有多个包连在一起的时候会执行下面的语句
                //{
                //    Buffer.BlockCopy(pack, 20, content, 0, i - 20);
                //    conLen = i - 20;
                //}

                return true;
            }
            catch (Exception ex)
            {
                //LogManager.AddError(ex);
                return false;
            }

        }

        public static void HandlerData(Socket clientSocket, byte[] content, int conLen, byte type, string tel)
        {
            if (type == HD_DTU.DTU_REGISTER_DATA)
            {
                if (Response_Reigster(clientSocket, tel))
                {
                    DTU_ClientManager.AddClient(tel, clientSocket);
                }
                else
                {
                    LogMg.AddDebug("tel=" + tel + "  响应客户端注册数据失败");
                }

            }
        }

        /// <summary>
        /// 回应注册
        /// </summary>
        /// <param name="ReceiveBuffer">接收到的注册数据或心跳数据</param>
        /// <param name="len">接收到的字节数</param>
        public static bool Response_Reigster(Socket socket, string tel)
        {
            try
            {
                byte[] sendBytes = new byte[20];
                //两字节的头
                sendBytes[0] = 0xA8;
                sendBytes[1] = 0x81;
                //11个字节的手机号
                if (tel == null && tel.Length != 11)
                {
                    return false;
                }
                Buffer.BlockCopy(Encoding.ASCII.GetBytes(tel), 0, sendBytes, 2, 11);
                //四个字节的区号
                sendBytes[13] = 0x30;
                sendBytes[14] = 0x30;
                sendBytes[15] = 0x30;
                sendBytes[16] = 0x30;
                //一个字节的功能码
                sendBytes[17] = JBT_DTU.Server_to_DTU_Register;
                //一个字节的区号启用标志
                sendBytes[18] = 0x00;
                //生成校验码
                GenerateCheckCode(sendBytes);
                int n = socket.Send(sendBytes, sendBytes.Length, SocketFlags.None);

                return n > 0;
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
                return false;
            }
        }


        /// <summary>
        /// 发送主用串口数据
        /// </summary>
        /// <param name="socket">套接字</param>
        /// <param name="content">内容</param>
        /// <param name="contentLen">内容长度 </param>    
        /// <param name="tel">手机号</param>       
        /// <returns></returns>
        public static int SendUserData_Master(Socket socket, byte[] content, int contentLen, string tel)
        {
            byte[] pack = new byte[GlobalPara.SEND_DATA_LENGTH];
            int packLen = 0;

            Buffer.BlockCopy(content, 0, pack, 0, contentLen);
            packLen = contentLen;
            // bool flag = Pack(content, contentLen, tel, function_code, SerialNum, ref pack, ref packLen); 
            //if (!flag) { return 0; }
            //这里的内容是透传的，所以content外面没有包任何协议。
            int n = Send(socket, pack, packLen);
            return n;
        }

        /// <summary>
        /// 发送备用串口数据   也就是摄像头的数据
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="content"></param>
        /// <param name="contentLen"></param>
        /// <param name="tel"></param>
        /// <returns></returns>
        public static int SendUserData_Assistant(Socket socket, byte[] content, int contentLen, string tel)
        {
            byte[] pack = new byte[GlobalPara.SEND_DATA_LENGTH];
            int packLen = 0;

            bool flag = PackUserData_Master(content, contentLen, tel, ref pack, ref packLen);
            if (!flag) { return 0; }

            int n = Send(socket, pack, packLen);
            return n;
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

        ///// <summary>
        ///// 当收到注册信息无误后, 应调用此方法回应DTU
        ///// </summary>
        //public static void ResponseRegister(Socket socket, string tel)
        //{
        //    //应答客户端
        //    byte[] sendBytes = new byte[16];
        //    sendBytes[0] = HD_DTU.HEAD;
        //    sendBytes[1] = HD_DTU.DSC_REGISTER_DATA;
        //    sendBytes[2] = 0;
        //    sendBytes[3] = 0x10;
        //    byte[] b_tel = Encoding.ASCII.GetBytes(tel);
        //    Buffer.BlockCopy(b_tel, 0, sendBytes, 4, 11);
        //    sendBytes[4 + 11] = HD_DTU.END;
        //    int n = Send(socket, sendBytes, 16);
        //}


        /// <summary>
        /// 生成校验码
        /// </summary>
        /// <param name="command"></param>
        private static void GenerateCheckCode(byte[] command)
        {
            int CheckCode = 0;
            for (int i = 0; i < 19; i++)
            {
                CheckCode += command[i];
            }
            command[19] = (byte)(CheckCode % 256);    //低位
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="socket">套按字</param>
        /// <param name="timeout">接收超时（毫秒）</param>
        /// <param name="content">接收的内容</param>
        /// <param name="conLen">内容长度</param>
        /// <param name="type">功能码</param>
        /// <param name="tel">手机号</param>
        /// <returns></returns>
        public static bool rdata(Socket socket, int timeout, ref byte[] content, ref int conLen, ref byte type, ref string tel)
        {
            byte[] buffer = new byte[JBT_DTU.ReceiveLength];  //接收数据的数组 
            socket.ReceiveTimeout = timeout;
            int packLen = socket.Receive(buffer, buffer.Length, SocketFlags.None);
            return UnPack(buffer, packLen, ref content, ref conLen, ref type, ref tel);
        }

        public static bool CheckProtocolByRegisterData(byte[] pack, int len)
        {
            try
            {
                int i;
                for (i = 0; i < len; i++)
                {
                    if (pack[i] == JBT_DTU.Head0 && pack[i + 1] == JBT_DTU.Head1)    //先找到头
                    {
                        break;
                    }
                }
                if (i == len)
                {
                    return false;
                }
                if (pack[i + 17] != DTU_to_Server_Register)
                {
                    return false;
                }
                return CheckCheckCode(pack.Skip(i).Take(20).ToArray());

            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
                return false;
            }
        }

        /// <summary>
        /// 检验校验码是否正确
        /// </summary>
        /// <param name="pack"></param>
        /// <returns></returns>
        private static bool CheckCheckCode(byte[] pack)
        {
            int CheckCode = 0;
            for (int i = 0; i < 19; i++)
            {
                CheckCode += pack[i];
            }
            return pack[19] == (byte)(CheckCode % 256);
        }
    }
}
