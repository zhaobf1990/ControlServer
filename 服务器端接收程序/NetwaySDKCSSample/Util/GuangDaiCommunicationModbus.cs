using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility;
using System.Net.Sockets;
using 服务器端接收程序.MyForm.GPRSControl;
using log4net;

namespace 服务器端接收程序.Util
{
    class GuangDaiCommunicationModbus : modbus
    {
      
        public static ModbusReturn readdata(GuangDaiService.CorePlatformWebServiceClient ws, String wscId, byte address, ushort RegisterStartAdd, int ModbusfunctionCode, string dataType, string DecodeOrder, ModbusReturn modbusReturn)
        {
            modbusReturn.sendTime = DateTime.Now;
            SendFc03(ws, wscId, address, RegisterStartAdd, ModbusfunctionCode, dataType, DecodeOrder, modbusReturn);
            modbusReturn.RecTime = DateTime.Now;
           
            return modbusReturn;
        }

        public static ModbusReturn writedata(GuangDaiService.CorePlatformWebServiceClient ws, String wscId, byte address, ushort RegisterStartAdd, int ModbusfunctionCode, string dataType, string DecodeOrder, short value, ModbusReturn modbusReturn)
        {
            short[] values = new short[2];

            values[0] = value;
            modbusReturn.sendTime = DateTime.Now;
            SendFc16(ws, wscId, address, RegisterStartAdd, 1, dataType, values, modbusReturn);
            modbusReturn.RecTime = DateTime.Now;
            return modbusReturn;
        }

        /// <summary>
        /// 读寄存器或线圈
        /// </summary>
        /// <param name="address">485总线上的设备地址</param>
        /// <param name="RegisterStartAdd">寄存器起始地址      寄存器个数为1，方法内部已写死。</param>
        /// <param name="ModbusfunctionCode">功能码   </param>
        /// <param name="dataType">寄存器类型     1代表16位无符号二进制的寄存器，2代表32位的寄存器</param>
        /// <param name="DecodeOrder">获取的数据的高低位顺序</param>
        /// <param name="receiveTimeout">超时时间 </param>
        /// <param name="channel">GPRS控制器的通道号，也是串口号</param>
        /// <param name="values">返回的value值   虽然是个数组，但数据只有一个，下标为0</param>
        /// <returns></returns>
        private static void SendFc03(GuangDaiService.CorePlatformWebServiceClient ws, String wscId, byte address, ushort RegisterStartAdd,
            int ModbusfunctionCode, string dataType, string DecodeOrder, ModbusReturn modbusReturn)
        {
            int dataLength = 2;    //16位无符号二进制的寄存器数据长度为两个字节     默认为16位寄存器 
            if (dataType == "2")   //说明是"32位无符号二进制"   所以要用4个字节
            {
                dataLength = 4;
            }
            LogMg.AddDebug("dataLength=" + dataLength);

            byte[] message = new byte[8];
            //Function 3 response buffer:
            byte[] response = new byte[5 + dataLength * 1];

            //Build outgoing modbus message:
            LogMg.AddDebug(" BuildMessage(" + address + ", " + ModbusfunctionCode + ", " + RegisterStartAdd + ", 1, ref message);");
            BuildMessage(address, (byte)ModbusfunctionCode, RegisterStartAdd, ushort.Parse(dataType), ref message);
            //Send modbus message to Serial Port:
            //
            try
            {
                //   GuangDaiService.transmitTransparentlyResult result = ws.transmitTransparently("160512027", 1, "01030098000105E5"); 
                modbusReturn.sendMsg = message;
                String hexCommand = CommonUtil.byteToHexStr(message, "");
                GuangDaiService.transmitTransparentlyResult result = ws.transmitTransparently(wscId, (int)address, hexCommand);
                modbusReturn.RecMsg = response;
                if (result.result == true)
                {
                    response = result.value;
                    modbusReturn.RecMsg = result.value;
                }
                else
                {
                    modbusReturn.ErrorMsg = result.message;
                    modbusReturn.success = false;
                    return;
                }
                ////日志 
                //for (int i = 0; i < response.Length; i++)
                //{
                //    msg += response[i] + " ";
                //}
                //LogManager.AddDebug("functionCode=" + ModbusfunctionCode + "     response.length=" + response.Length + "    response=" + msg);
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
                LogMg.AddDebug("返回的数据长度datalength=" + datalength);
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
                    modbusReturn.ErrorMsg = "解析返回的modbus时出错，modbus指令为：" + response;
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



        /// <summary>
        /// 写寄存器
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="address"></param>
        /// <param name="start"></param>
        /// <param name="registers"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static void SendFc16(GuangDaiService.CorePlatformWebServiceClient ws, String wscId, byte address, ushort start, ushort registers, string dataType, short[] values, ModbusReturn modbusReturn)
        {
            LogMg.AddDebug("SendFc16(Socket socket,String  " + wscId + " byte " + address + ", ushort " + start + ", ushort " + registers + ", string " + dataType + ", short " + values[0] + ")");
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
                GuangDaiService.transmitTransparentlyResult result = ws.transmitTransparently(wscId, (int)address, CommonUtil.byteToHexStr(message, ""));
                modbusReturn.RecMsg = result.value;
                if (result.result == true)
                {
                    response = result.value;
                }
                else
                {
                    modbusReturn.ErrorMsg = result.message;
                    modbusReturn.success = false;
                    return;
                }
                //DTU.SendUserData(protocol, socket, message, message.Length, tel, channel);
                ////sp.Write(message, 0, message.Length);
                //GetResponse(protocol, socket, receiveTimeout, ref response);
            }
            catch (Exception err)
            {
                modbusReturn.ErrorMsg = "Error in write event: " + err.Message;
                modbusReturn.success = false;
                return;
            }
            //Evaluate message:
            if (CheckResponse(response))
            {
                modbusReturn.ErrorMsg = "Write successful";
                modbusReturn.success = true;
                return;
            }
            else
            {
                modbusReturn.ErrorMsg = "CRC error";
                modbusReturn.success = false;
                return;
            }
            /* }
             else
             {
                 modbusStatus = "Serial port not open";
                 return false;
             }
             * */
        }
    }



}
