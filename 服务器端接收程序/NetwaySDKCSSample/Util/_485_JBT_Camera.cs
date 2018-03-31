using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using 服务器端接收程序.Clazz;
using Utility;
using System.Threading;

namespace 服务器端接收程序.Util
{
    /// <summary>
    /// 485线上的通讯协议  金博通的摄像头
    /// </summary>
    public class _485_JBT_Camera
    {
        private static byte HEAD = 0x7E;
        private static byte ESC = 0x1b;

        /// <summary>
        /// 拍摄照片命令
        /// </summary>
        /// <returns></returns>
        private static byte[] GetTakePhotoCommand(ref int len)
        {
            byte[] command = new byte[16];
            command[0] = 0x7E;
            command[1] = 0x10;
            command[2] = 0x00;
            command[3] = 0x00;
            command[4] = 0x03;
            command[7] = 0x7E;
            len = GenerateCheckCode(command, 8);
            return command;
        }
        /// <summary>
        /// 读取图像数据包总数
        /// </summary>
        private static byte[] GetPicPackCountCommand(ref int len)
        {
            byte[] command = new byte[16];
            command[0] = 0x7E;
            command[1] = 0x10;
            command[2] = 0x13;
            command[3] = 0x00;
            command[4] = 0x03;
            command[7] = 0x7E;
            len = GenerateCheckCode(command, 8);
            return command;
        }
        /// <summary>
        /// 读缓存数据包命令
        /// </summary>
        /// <returns></returns>
        private static byte[] GetReadPictureCommand(byte index, ref int len)
        {
            byte[] command = new byte[16];
            command[0] = 0x7E;
            command[1] = 0x10;
            command[2] = 0x02;
            command[3] = 0x00;
            command[4] = index;
            command[7] = 0x7E;
            len = Escapes(command, 8);
            len = GenerateCheckCode(command, len);
            return command;
        }





        /// <summary> 
        /// 生成校验码
        /// </summary>
        /// <param name="command">字节数组</param>
        /// <param name="len">长度</param>
        /// <returns>返回数组的长度</returns>
        private static int GenerateCheckCode(byte[] command, int len)
        {
            int outLen = len;
            ushort CheckCode = 0;
            for (int i = 1; i < len - 3; i++)
            {
                CheckCode += command[i];
            }
            command[len - 3] = (byte)(CheckCode >> 8);    //高位
            command[len - 2] = (byte)(CheckCode % 256);    //低位

            if (command[len - 2] == HEAD || command[len - 2] == ESC)  //对低位进行转义
            {
                command[len] = command[len - 1];
                command[len - 1] = (byte)(ESC ^ command[len - 2]);
                command[len - 2] = ESC;
                outLen++;
            }

            if (command[len - 3] == HEAD || command[len - 3] == ESC)  //对高位进行转义
            {
                command[len] = command[len - 1];
                command[len - 1] = command[len - 2];
                command[len - 2] = (byte)(ESC ^ command[len - 3]);
                command[len - 3] = ESC;
                outLen++;
            }
            return outLen;
        }

        /// <summary>
        /// 转义
        /// </summary>
        /// <param name="arr">转义前数组</param>
        /// <param name="len">转义前数组长度</param>
        /// <param name="outArr">转义后数组</param>
        /// <param name="outLen">转义后数组长度</param>
        /// <returns>转义后的数组长度</returns>
        private static int Escapes(byte[] arr, int len)
        {
            int outLen = len;
            for (int i = len - 2; i > 0; i--)
            {
                if (arr[i] == HEAD || arr[i] == ESC)
                {
                    for (int j = len - 1; j > i; j--) //把其他元素向后移
                    {
                        arr[j + 1] = arr[j];
                    }
                    arr[i + 1] = (byte)(arr[i] ^ ESC);
                    arr[i] = ESC;
                    outLen++;
                }
            }
            return outLen;
        }


        /// <summary>
        /// 拍照
        /// </summary>
        /// <param name="client"></param>
        /// <param name="timeout"></param>
        public static bool TakePhoto(DTUClientInfo client, int timeout, ref byte[] imageBytes)
        {
            //这个方法主要包含三个步骤，
            //第一步：发送拍照指令
            //第二步：发送“获取照片包数量”的指令
            //第三步：循环去读取每一个照片包内容
            byte[] rubbish = new byte[JBT_DTU.ReceiveLength];

            byte packcount = 0x00;
            int n = 1;   //当发送的命令没接收到，这个变量就会+1，直到他等于5 

            int commandLen1 = 0;
            byte[] command1 = GetTakePhotoCommand(ref commandLen1);
            try
            {
                client.socket.Receive(rubbish, JBT_DTU.ReceiveLength, SocketFlags.None);
            }
            catch (Exception) { }
            //第一步：发送拍照指令
            JBT_DTU.SendUserData_Assistant(client.socket, command1, commandLen1, client.TelOrGprsId);
            while (n <= 5)
            {
                try
                {
                    byte[] content = new byte[JBT_DTU.ReceiveLength];
                    int conLen = 0;
                    byte type = new byte();
                    string tel = "";
                    Thread.Sleep(3000);
                    bool flag = JBT_DTU.rdata(client.socket, timeout, ref content, ref conLen, ref type, ref tel);

                    if (flag == false)
                    {
                        n++;
                        continue;
                    }
                    if (type != JBT_DTU.DTU_to_Server_SendData_Assistant)
                    {
                        n++;
                        continue;
                    }
                    if (conLen != 7)
                    {
                        n++;
                        continue;
                    }
                    break;
                }
                catch (SocketException ex) { n++; LogMg.AddError(ex); }
                catch (Exception ex)
                {
                    n++;
                    LogMg.AddError(ex);
                }
            }
            DTU_ClientManager.UpdateLastVisitTime(client, DateTime.Now);
            if (n > 5)
                return false;  //如果读取5次都读不到数据，就表示这次拍照失败

            LogMg.AddError("拍照命令   拍照次数=" + n);

            n = 1;

            int commandLen2 = 0;
            //第二步：发送“获取照片包数量”的指令
            byte[] command2 = GetPicPackCountCommand(ref commandLen2);
            JBT_DTU.SendUserData_Assistant(client.socket, command2, commandLen2, client.TelOrGprsId);
            while (n <= 5)
            {
                try
                {
                    byte[] content = new byte[4096];
                    int conLen = 0;
                    byte type = new byte();
                    string tel = "";
                    Thread.Sleep(1000);
                    bool flag = JBT_DTU.rdata(client.socket, timeout, ref content, ref conLen, ref type, ref tel);
                    if (flag == false)
                    {
                        n++;
                        continue;
                    }
                    if (type != JBT_DTU.DTU_to_Server_SendData_Assistant)
                    {
                        n++;
                        continue;
                    }
                    if (conLen != 8)
                    {
                        n++;
                        continue;
                    }
                    packcount = content[4];
                    break;
                }
                catch (SocketException ex) { n++; LogMg.AddError(ex); }
                catch (Exception ex)
                {
                    n++;
                    LogMg.AddError(ex);
                }
            }
            if (n > 5)
                return false;
            LogMg.AddError("获取照片包数量   n=" + n);


            imageBytes = new byte[packcount * 1000];
            int picIndex = 0;
            //第三步：循环去读取每一个照片包内容
            for (byte i = 0x00; i < packcount; i++)
            {
                n = 1;

                int commandLen3 = 0;
                byte[] command3 = GetReadPictureCommand(i, ref commandLen3);
                JBT_DTU.SendUserData_Assistant(client.socket, command3, commandLen3, client.TelOrGprsId);
                while (n <= 5)
                {
                    try
                    {
                        byte[] content = new byte[4096];
                        int conLen = 0;
                        byte type = new byte();
                        string tel = "";
                        Thread.Sleep(1000);
                        bool flag = JBT_DTU.rdata(client.socket, timeout, ref content, ref conLen, ref type, ref tel);
                        if (flag == false)
                        {
                            n++;
                            continue;
                        }
                        if (type != JBT_DTU.DTU_to_Server_SendData_Assistant)
                        {
                            n++;
                            continue;
                        }
                        if (content[0] != HEAD || content[1007] != HEAD)   //
                        {
                            n++;
                            continue;
                        }
                        if (content[04] != i)   //如果包号不对
                        {
                            n++;
                            continue;
                        }
                        Buffer.BlockCopy(content, 5, imageBytes, picIndex, 1000);
                        picIndex += 1000;
                        break;
                    }
                    catch (SocketException ex)
                    {
                        n++; LogMg.AddError(ex);
                        LogMg.AddError("循环读取包   n=" + n);
                    }
                    catch (Exception ex)
                    {
                        n++;
                        LogMg.AddError(ex);
                    }
                }
                if (n > 5)
                    return false;
            }
            //ImageUtil.SaveFormBytes(imageBytes, GetImageLen(imageBytes));
            //len = GetImageLen(imageBytes);
            return true;
        }

        private static int GetImageLen(byte[] image_byte)
        {
            int len = image_byte.Length;
            int i = 0;
            for (i = len; i >= 2; i--)
            {
                if (image_byte[i - 1] == 0xD9 && image_byte[i - 2] == 0xFF)
                {
                    break;
                }
            }
            return i;
        }

        /// <summary>
        /// 把命令写到日志 
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="len"></param>
        private static void AddToLog(byte[] arr, int len)
        {
            string str = "";
            for (int i = 0; i < len; i++)
            {
                str += " " + arr[i];
            }
            LogMg.AddDebug(str);
        }


    }
}
