using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 服务器端接收程序.Util
{
    /// <summary>
    /// 全局变量参数
    /// </summary>
    public static class GlobalPara
    {
       

        /// <summary>
        /// 发送与接收的数据长度（单位字节）
        /// </summary>
        public static readonly int SEND_DATA_LENGTH = 4096;

        /// <summary>
        /// 发送与接收的内容长度     DTU协议中的Data那一段
        /// </summary>
        public static readonly int CONTENT_LENGTH = SEND_DATA_LENGTH - 16;

        /// <summary>
        /// 发送超时时间
        /// </summary>
        public static readonly int SendTimeOut = 1000;

        /// <summary>
        /// 接收超时时间
        /// </summary>
        public static readonly int ReceiveTimeOut = 1000;
    }
}
