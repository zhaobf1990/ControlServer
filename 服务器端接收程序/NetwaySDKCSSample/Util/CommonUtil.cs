using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 服务器端接收程序.Util
{
    public class CommonUtil
    {
        /// <summary> 
        /// 字节数组转16进制字符串 
        /// </summary> 
        /// <param name="bytes"></param> 
        /// <returns></returns> 
        public static string byteToHexStr(byte[] bytes)
        {
            return byteToHexStr(bytes, bytes.Length);
        }

        /// <summary> 
        /// 字节数组转16进制字符串 
        /// </summary> 
        /// <param name="bytes"></param> 
        /// <returns></returns> 
        public static string byteToHexStr(byte[] bytes, String separator)
        {
            return byteToHexStr(bytes, bytes.Length, separator);
        }

        /// <summary>
        ///  字节数组转16进制字符串 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string byteToHexStr(byte[] bytes, int length)
        {
            return byteToHexStr(bytes, length, " ");
        }

        /// <summary>
        ///  字节数组转16进制字符串 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string byteToHexStr(byte[] bytes, int length, String separator)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < length; i++)
                {
                    returnStr += bytes[i].ToString("X2") + separator;
                }
            }
            return returnStr;
        }
        /// <summary>
        /// 将字符串转换成字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] StrToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }
        /// <summary>
        /// 将整型数字转换成字节数组
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] IntToByteArr(int value)
        {
            string str = value.ToString("x8");
            return StrToHexByte(str);
        }
    }
}
