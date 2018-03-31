using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using Utility;
using 服务器端接收程序.MyForm.GPRSControl;

namespace 服务器端接收程序.Util
{
    /// <summary>
    /// 通讯设备类型， 通讯区分具体的通讯设备调用具体的通讯协议类,      ZG_DTU或HD_DTU或者其他的。
    /// </summary>
    public class DTU
    {
        /// <summary>
        /// 佐格DTU
        /// </summary>
        public static readonly string _ZG_DTU = "ZG_DTU";
        /// <summary>
        /// 宏电DTU
        /// </summary>
        public static readonly string _HD_DTU = "HD_DTU";
        /// <summary>
        /// 金博通DTU
        /// </summary>
        public static readonly string _JBT_DTU = "JBT_DTU";
        /// <summary>
        /// GPRS控制器   浙江大学城市学院江老师开发的版本。
        /// </summary>
        public static readonly string _GPRS_CONTROL = "GPRS_CONTROL";

        /// <summary>
        /// 解析注册的数据是DTU协议还是DDP协议
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string CheckProtocolByRegisterData(byte[] buffer, int len)
        {
            if (ZG_DTU.CheckProtocolByRegisterData(buffer, len) == true)
            {
                return _ZG_DTU;
            }
            if (HD_DTU.CheckProtocolByRegisterData(buffer, len) == true)
            {
                return _HD_DTU;
            }
            if (JBT_DTU.CheckProtocolByRegisterData(buffer, len) == true)
            {
                return _JBT_DTU;
            }
            //如果不符合上面的协议标准  则返回空字符串
            LogMg.AddDebug("检验注册数据格式不正确 ,即不符合佐格DTU协议,也不符合宏电DTU协议,也不符号金博通DTU协议");
            return "";
        }


        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="timeout"></param>
        /// <param name="content"></param>
        /// <param name="conLen"></param>
        /// <param name="type"></param>
        /// <param name="tel"></param>
        /// <returns></returns>
        public static bool rdata(ref string protocol, Socket socket, int timeout, ref byte[] content, ref int conLen, ref byte type, ref string tel)
        {
            if (protocol == DTU._ZG_DTU)
            {
                return ZG_DTU.rdata(socket, timeout, ref content, ref conLen, ref type, ref tel);    //1秒超时   等待注册数据
            }
            else if (protocol == DTU._HD_DTU)
            {
                return HD_DTU.rdata(socket, timeout, ref content, ref conLen, ref type, ref tel);    //1秒超时   等待注册数据
            }
            else if (protocol == DTU._JBT_DTU)
            {
                return JBT_DTU.rdata(socket, timeout, ref content, ref conLen, ref type, ref tel);    //1秒超时   等待注册数据
            }
            else
            {
                //在DTU未注册之前, socket接收都是调用以下的代码   ,
                try
                {
                    byte[] buffer = new byte[GlobalPara.SEND_DATA_LENGTH];  //接收数据的数组 
                    socket.ReceiveTimeout = timeout;
                    int len = socket.Receive(buffer, buffer.Length, SocketFlags.None);
                    protocol = DTU.CheckProtocolByRegisterData(buffer, len);
                    if (protocol == DTU._ZG_DTU)
                    {
                        return ZG_DTU.UnPack(buffer, len, ref content, ref conLen, ref type, ref tel);
                    }
                    else if (protocol == DTU._HD_DTU)
                    {
                        return HD_DTU.UnPack(buffer, len, ref content, ref conLen, ref type, ref tel);
                    }
                    else if (protocol == DTU._JBT_DTU)
                    {
                        return JBT_DTU.UnPack(buffer, len, ref content, ref conLen, ref type, ref tel);
                    }
                    else
                    {
                        return false;
                    }

                }
                catch (SocketException)
                {
                    return false;
                }
                catch (Exception ex)
                {
                    LogMg.AddError(ex);
                    return false;
                }
            }

        }

        /// <summary>
        /// 处理数据
        /// </summary>
        /// <param name="protocol"></param>
        /// <param name="clientSocket"></param>
        /// <param name="content"></param>
        /// <param name="conLen"></param>
        /// <param name="type"></param>
        /// <param name="tel"></param>
        public static bool HandlerData(string protocol, Socket clientSocket, byte[] content, int conLen, byte type, string tel)
        {
            if (protocol == DTU._ZG_DTU)
            {
                ZG_DTU.HandlerData(clientSocket, content, conLen, type, tel);
                return true;
            }
            else if (protocol == DTU._HD_DTU)
            {
                HD_DTU.HandlerData(clientSocket, content, conLen, type, tel);
                return true;
            }
            else if (protocol == DTU._JBT_DTU)
            {
                JBT_DTU.HandlerData(clientSocket, content, conLen, type, tel);
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="content"></param>
        /// <param name="conLen"></param>
        /// <param name="type"></param>
        public static void SendUserData(string protocol, Socket socket, byte[] content, int conLen, string tel, byte channel)
        {
            if (protocol == DTU._ZG_DTU)
            {
                ZG_DTU.Send(socket, content, conLen, ZG_DTU.USER_DATA);
            }
            else if (protocol == DTU._HD_DTU)
            {
                HD_DTU.Send(socket, content, conLen, HD_DTU.DSC_User_Data, tel);
            }
            else if (protocol == DTU._JBT_DTU)
            {
                JBT_DTU.SendUserData_Master(socket, content, conLen, tel);
            }
            else if (protocol == DTU._GPRS_CONTROL)
            {
                GPRS_Protocol.Send(socket, channel, content, conLen);
            }
        }

        /// <summary>
        /// 响应注册数据或心跳数据
        /// </summary>
        /// <param name="protocol"></param>
        /// <param name="socket"></param>
        /// <param name="tel"></param>
        /// <returns></returns>
        public static bool ResponseRegisterOrHeart(string protocol, Socket socket, string tel)
        {
            if (protocol == DTU._ZG_DTU)
            {
                return ZG_DTU.HeartbeatDataHandler(socket);
            }
            else if (protocol == DTU._HD_DTU)
            {
                return HD_DTU.ResponseRegister(socket, tel);
            }
            else if (protocol == DTU._JBT_DTU)
            {
                return JBT_DTU.Response_Reigster(socket, tel);
            }
            else
            {
                return false;
            }
        }
    }
}
