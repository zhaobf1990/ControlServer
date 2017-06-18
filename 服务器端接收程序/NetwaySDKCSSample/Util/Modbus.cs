
using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Net.Sockets;
using Utility;
using 服务器端接收程序.Config;

namespace 服务器端接收程序.Util
{
    /// <summary>
    /// Modbus指令的生成与解析
    /// </summary>
    class modbus
    {
        //public static string modbusStatus;

        #region CRC Computation
        protected static void GetCRC(byte[] message, ref byte[] CRC)
        {
            //Function expects a modbus message of any length as well as a 2 byte CRC array in which to 
            //return the CRC values:

            ushort CRCFull = 0xFFFF;
            byte CRCHigh = 0xFF, CRCLow = 0xFF;
            char CRCLSB;

            for (int i = 0; i < (message.Length) - 2; i++)
            {
                CRCFull = (ushort)(CRCFull ^ message[i]);

                for (int j = 0; j < 8; j++)
                {
                    CRCLSB = (char)(CRCFull & 0x0001);
                    CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);

                    if (CRCLSB == 1)
                        CRCFull = (ushort)(CRCFull ^ 0xA001);
                }
            }
            CRC[1] = CRCHigh = (byte)((CRCFull >> 8) & 0xFF);
            CRC[0] = CRCLow = (byte)(CRCFull & 0xFF);
        }
        #endregion

        #region Build Message
        protected static void BuildMessage(byte address, byte type, ushort start, ushort registers, ref byte[] message)
        {
            //Array to receive CRC bytes:
            byte[] CRC = new byte[2];

            message[0] = address;
            message[1] = type;
            message[2] = (byte)(start >> 8);
            message[3] = (byte)start;
            message[4] = (byte)(registers >> 8);
            message[5] = (byte)registers;

            GetCRC(message, ref CRC);
            message[message.Length - 2] = CRC[0];
            message[message.Length - 1] = CRC[1];
        }
        #endregion


        #region Check Response
        protected static bool CheckResponse(byte[] response)
        {
            //Perform a basic CRC check:
            byte[] CRC = new byte[2];
            GetCRC(response, ref CRC);
            if (CRC[0] == response[response.Length - 2] && CRC[1] == response[response.Length - 1])
                return true;
            else
                return false;
        }
        #endregion

        #region Get Response
        protected static void GetResponse(string protocol, Socket socket, int receiveTimeout, ref byte[] response)
        {
            //There is a bug in .Net 2.0 DataReceived Event that prevents people from using this
            //event as an interrupt to handle data (it doesn't fire all of the time).  Therefore
            //we have to use the ReadByte command for a fixed length as it's been shown to be reliable.
            // for (int i = 0; i < response.Length; i++)
            //{
            int len = 0;
            byte type = 0;
            string strtel = new string('0', 11);
            DTU.rdata(ref protocol, socket, receiveTimeout, ref response, ref len, ref type, ref strtel);
            //    response[i] = (byte)();//sp.ReadByte());
            // }
        }
        #endregion

        #region Function 16 - Write Multiple Registers

        /// <summary>
        /// 写寄存器
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="address"></param>
        /// <param name="start"></param>
        /// <param name="registers"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static void SendFc16(string protocol, Socket socket, string tel, byte address, ushort start, ushort registers, string dataType, int receiveTimeout, byte channel, short[] values, ModbusReturn modbusReturn)
        {
            // LogManager.AddDebug("SendFc16(Socket socket, byte " + address + ", ushort " + start + ", ushort " + registers + ", string " + dataType + ", int " + receiveTimeout + ", short " + values[0] + ")");
            int dataLength = 2;    //默认数据长度为两个字节   
            if (dataType == "2")   //说明是"32位无符号二进制"   所以要用4个字节
            {
                dataLength = 4;
            }
            //Message is 1 addr + 1 fcn + 2 start + 2 reg + 1 count + 2 * reg vals + 2 CRC
            byte[] message = new byte[9 + dataLength * registers];
            //Function 16 response is fixed at 8 bytes
            byte[] response = new byte[8];



            try
            {
                //Add bytecount to message:
                message[6] = (byte)(registers * dataLength);
                //Put write values into message prior to sending:
                for (int i = 0; i < registers; i++)
                {
                    for (int j = 0; j < dataLength; j++)
                    {
                        message[7 + (dataLength * i) + j] = (byte)(values[i] >> 8 * (dataLength - j - 1));  //将value[i]转换成指定字节长度的字节数组, (高位在前,低位在后)   message下标从第七个开始
                    }
                    //message[7 + 2 * i] = (byte)(values[i] >> 8);
                    //message[8 + 2 * i] = (byte)(values[i]);
                }

                //Build outgoing message:
                BuildMessage(address, (byte)16, start, registers, ref message);
                //string msg = "";
                //for (int i = 0; i < message.Length; i++)
                //{
                //    msg += "\r\n   message[" + i + "]=   值:" + message[i];
                //}
                //LogManager.AddDebug("写  message.length=" + message.Length + msg);
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
            }

            //Send Modbus message to Serial Port:
            try
            {
                modbusReturn.sendMsg = message;
                DTU.SendUserData(protocol, socket, message, message.Length, tel, channel);
                //sp.Write(message, 0, message.Length);
                GetResponse(protocol, socket, receiveTimeout, ref response);
                modbusReturn.RecMsg = response;
            }
            catch (Exception err)
            {
                modbusReturn.ErrorMsg = "Error in write event: " + err.Message;
                modbusReturn.success = false;
                return ;
            }
            //Evaluate message:
            if (CheckResponse(response))
            {
                modbusReturn.ErrorMsg = "Write successful";
                modbusReturn.success = true;
                return ;
            }
            else
            {
                modbusReturn.ErrorMsg = "CRC error";
                modbusReturn.success = false;
                return ;
            }
            /* }
             else
             {
                 modbusStatus = "Serial port not open";
                 return false;
             }
             * */
        }
        #endregion


        #region Function 3 - Read Registers

        /// <summary>
        /// 读寄存器或线圈
        /// </summary>
        /// <param name="protocol">协议( ZG_DTU、HD_DTU或者JBT_DTU)</param>
        /// <param name="socket">socket</param>
        /// <param name="tel">DTU的Id,长得跟TEL相似，都是11位的</param>
        /// <param name="address">485总线上的设备地址</param>
        /// <param name="RegisterStartAdd">寄存器起始地址      寄存器个数为1，方法内部已写死。</param>
        /// <param name="ModbusfunctionCode">功能码   </param>
        /// <param name="dataType">寄存器类型     1代表16位无符号二进制的寄存器，2代表32位的寄存器</param>
        /// <param name="DecodeOrder">获取的数据的高低位顺序</param>
        /// <param name="receiveTimeout">超时时间 </param>
        /// <param name="channel">GPRS控制器的通道号，也是串口号</param>
        /// <param name="values">返回的value值   虽然是个数组，但数据只有一个，下标为0</param>
        /// <returns></returns>
        public static void SendFc(string protocol, Socket socket, string tel, byte address, ushort RegisterStartAdd,
            int ModbusfunctionCode, string dataType, string DecodeOrder, int receiveTimeout, byte channel, ModbusReturn modbusReturn)
        {
            int dataLength = 2;    //16位无符号二进制的寄存器数据长度为两个字节     默认为16位寄存器 
            if (dataType == "2")   //说明是"32位无符号二进制"   所以要用4个字节
            {
                dataLength = 4;
            }
            //LogManager.AddDebug("dataLength=" + dataLength);

            byte[] message = new byte[8];
            //Function 3 response buffer:
            byte[] response = new byte[5 + dataLength * 1];

            //Build outgoing modbus message:
            // LogManager.AddDebug(" BuildMessage(" + address + ", " + ModbusfunctionCode + ", " + RegisterStartAdd + ", 1, ref message);");
            BuildMessage(address, (byte)ModbusfunctionCode, RegisterStartAdd, ushort.Parse(dataType), ref message);
            //Send modbus message to Serial Port:
            string msg = "";
            try
            {
                modbusReturn.sendMsg = message;
                DTU.SendUserData(protocol, socket, message, message.Length, tel, channel);
                //sp.Write(message, 0, message.Length);
                GetResponse(protocol, socket, receiveTimeout, ref response);
                modbusReturn.RecMsg = response;
            }
            catch (SocketException ex)
            {
                modbusReturn.ErrorMsg = ex.Message;
                modbusReturn.success = false;
                return;
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
                modbusReturn.ErrorMsg = "Error in read event: " + ex.Message;
                modbusReturn.success = false;
                return;
            }
            //处理返回结果
            if (CheckResponse(response))
            {
                int datalength = response[2];  //数据长度(单位:字节)
                //LogManager.AddDebug("返回的数据长度datalength=" + datalength);
                //设置默认解码顺序
                if (string.IsNullOrEmpty(DecodeOrder))
                {
                    if (datalength == 2)
                    {
                        DecodeOrder = "12";
                    }
                    else if (datalength == 4)
                    {//三四个字节为高位，一二两个字节为低位
                        DecodeOrder = "3412";
                    }
                }
                //解析返回的modbus数据
                modbusReturn.value = 0;
                try
                {
                    for (int i = 0; i < DecodeOrder.Length; i++)
                    {
                        int n = Convert.ToInt32(DecodeOrder[i].ToString());
                        modbusReturn.value <<= 8;
                        modbusReturn.value += response[2 + n];
                    }
                }
                catch (Exception)
                {
                    modbusReturn.ErrorMsg = "解析返回的modbus时出错，modbus指令为：" + msg;
                    modbusReturn.success = false;
                    return;
                }
                modbusReturn.ErrorMsg = "Read successful";
                modbusReturn.success = true;
                return;
            }
            else
            {
                modbusReturn.ErrorMsg = "CRC error";
                modbusReturn.success = false;
                return;
            }
        }
        #endregion

        #region Function 3 - Read Registers


        /// <summary>
        /// 读数据
        /// </summary>
        /// <param name="protocol">协议( ZG_DTU、HD_DTU或者JBT_DTU)</param>
        /// <param name="socket">socket</param>
        /// <param name="tel">DTU的Id,长得跟TEL相似，都是11位的</param>
        /// <param name="address">485总线上的设备地址</param>
        /// <param name="RegisterStartAdd">寄存器起始地址      寄存器个数为1，方法内部已写死。</param>
        /// <param name="functionCode">功能码</param>
        /// <param name="dataType">寄存器类型     1代表16位无符号二进制的寄存器，2代表32位的寄存器</param>
        /// <param name="DecodeOrder">获取的数据的高低位顺序</param>
        /// <param name="receiveTimeout">超时时间</param>
        /// <param name="value">返回的value值    </param>
        /// <returns></returns>
        public static ModbusReturn readdata(string protocol, Socket socket, string tel, byte address, ushort RegisterStartAdd,
            int functionCode, string dataType, string DecodeOrder, int receiveTimeout, ModbusReturn modbusReturn)
        {
            return readdata(protocol, socket, tel, address, RegisterStartAdd, functionCode, dataType, DecodeOrder, receiveTimeout, new byte(), modbusReturn);
        }

        /// <summary>
        /// 读数据 （这个方法给浙江大学城市学院江老师开发的GPRS控制器调用，这个方法多了channel  ）
        /// </summary>
        /// <param name="protocol">协议( ZG_DTU、HD_DTU或者JBT_DTU)</param>
        /// <param name="socket">socket</param>
        /// <param name="tel">DTU的Id,长得跟TEL相似，都是11位的</param>
        /// <param name="address">485总线上的设备地址</param>
        /// <param name="RegisterStartAdd">寄存器起始地址      寄存器个数为1，方法内部已写死。</param>
        /// <param name="functionCode">功能码</param>
        /// <param name="dataType">寄存器类型     1代表16位无符号二进制的寄存器，2代表32位的寄存器</param>
        /// <param name="DecodeOrder">获取的数据的高低位顺序</param>
        /// <param name="receiveTimeout">超时时间</param>           
        /// <param name="channel">GPRS控制器的通道号，也是串口号</param>
        /// <returns></returns>
        public static ModbusReturn readdata(string protocol, Socket socket, string tel, byte address, ushort RegisterStartAdd,
            int functionCode, string dataType, string DecodeOrder, int receiveTimeout, byte channel, ModbusReturn modbusReturn)
        {
            modbusReturn.sendTime = DateTime.Now;
            SendFc(protocol, socket, tel, address, RegisterStartAdd, functionCode, dataType, DecodeOrder, receiveTimeout, channel, modbusReturn);
            modbusReturn.RecTime = DateTime.Now;
            return modbusReturn;
        }


        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="protocol">协议( ZG_DTU、HD_DTU或者JBT_DTU)</param>
        /// <param name="socket">socket</param>
        /// <param name="tel">DTU的Id,长得跟电话号码相似，都是11位的</param>
        /// <param name="address">485总线上的设备地址</param>
        /// <param name="RegisterStartAdd">寄存器地址</param>
        /// <param name="dataType">数据类型(1代表16位无符号二进制,2代表32位无符号二进制)</param>
        /// <param name="receiveTimeout">接收超时时间(毫秒)</param>
        /// <param name="value">写入的值</param>
        /// <returns></returns>
        public static ModbusReturn writedata(string protocol, Socket socket, string tel, byte address,
            ushort RegisterStartAdd, string dataType, int receiveTimeout, short value, ModbusReturn modbusReturn)
        {
            return writedata(protocol, socket, tel, address, RegisterStartAdd, dataType, receiveTimeout, new byte(), value, modbusReturn);
        }

        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="protocol">协议( ZG_DTU、HD_DTU或者JBT_DTU)</param>
        /// <param name="socket">socket</param>
        /// <param name="tel">DTU的Id,长得跟电话号码相似，都是11位的</param>
        /// <param name="address">485总线上的设备地址</param>
        /// <param name="RegisterStartAdd">寄存器地址</param>
        /// <param name="dataType">数据类型(1代表16位无符号二进制,2代表32位无符号二进制)</param>
        /// <param name="receiveTimeout">接收超时时间(毫秒)</param>
        /// <param name="channel">GPRS控制器的通道号，也是串口号</param>
        /// <param name="value">写入的值</param>
        /// <returns></returns>
        public static ModbusReturn writedata(string protocol, Socket socket, string tel, byte address,
            ushort RegisterStartAdd, string dataType, int receiveTimeout, byte channel, short value, ModbusReturn modbusReturn)
        {
            short[] values = new short[2];

            values[0] = value;
            modbusReturn.sendTime = DateTime.Now;
            SendFc16(protocol, socket, tel, address, RegisterStartAdd, 1, dataType, receiveTimeout, channel, values, modbusReturn);
            modbusReturn.RecTime = DateTime.Now;
            return modbusReturn;
        }

        #endregion



    }
}

