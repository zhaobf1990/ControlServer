using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 服务器端接收程序.Util
{
    /// <summary>
    /// 执行过程中产生的数据
    /// </summary>
    class ModbusReturn
    {
        private DateTime _recTime;
        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime sendTime { get; set; }
        /// <summary>
        /// 发送数据
        /// </summary>
        public byte[] sendMsg { get; set; }
        /// <summary>
        /// 接收时间
        /// </summary>
        public DateTime RecTime
        {
            get { return _recTime; }
            set
            {
                _recTime = value;
                this.duration = (_recTime - sendTime).Seconds;
            }
        }
        /// <summary>
        /// 接收数据
        /// </summary>
        public byte[] RecMsg { get; set; }
        /// <summary>
        /// 持续时长
        /// </summary>
        public int duration { get; set; }
        /// <summary>
        /// 执行结果
        /// </summary>
        public bool success { get; set; }
        /// <summary>
        /// 错误消息
        /// </summary>
        public String ErrorMsg { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public long value { get; set; }

        public ModbusReturn()
        {
            this.success = false;
            this.value = long.MinValue;
        }

        public void clear()
        {
            this.ErrorMsg = "";
            this.value = long.MinValue;
            this.success = false;
        }

        public override string ToString()
        {
            return String.Format("执行结果:【{0}】  持续时间【{1}】  发送时间:【{2}】  发送数据:【{3}】   接收时间:【{4}】   接收数据:【{5}】  错误消息:【{6}】   值:【{7}】"
                , success, duration, sendTime.ToString(), CommonUtil.byteToHexStr(sendMsg), _recTime.ToString(), CommonUtil.byteToHexStr(RecMsg), ErrorMsg, value);
        }
    }
}
