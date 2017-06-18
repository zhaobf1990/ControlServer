using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 服务器端接收程序.Clazz.Config
{
    /// <summary>
    /// 配置文件中的检测点
    /// </summary>
    public class XML_Test
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int StationUnique { get; set; }
        /// <summary>
        /// 站点编号
        /// </summary>
        public int StationId { get; set; }
        /// <summary>
        /// 寄存器编号 
        /// </summary>
        public ushort RegisterNo { get; set; }
        /// <summary>
        /// 检测点Id
        /// </summary>
        public int TestId { get; set; }
        /// <summary>
        /// 倍数
        /// </summary>
        public double Multiple { get; set; }
        /// <summary>
        /// 功能码  1代表线圈    3代表寄存器
        /// </summary>
        public int FunctionCode { get; set; }
        /// <summary>
        /// 返回超时时间  (单位毫秒)
        /// </summary>
        public int ReceiveTimeout { get; set; }
        /// <summary>
        /// 通道数据类型  (1代表"16位无符号二进制"   2:代表"32位无符号二进制")
        /// </summary>
        public string DataType { get; set; }
        /// <summary>
        /// 从设备地址 
        /// </summary>
        public byte Address { get; set; }
        /// <summary>
        /// DecodeOrder 
        /// </summary>
        public string DecodeOrder { get; set; }
        /// <summary>
        /// 最小取值范围     小于这个值的不要
        /// </summary>
        public double Min { get; set; }
        /// <summary>
        /// 最大取值范围       大于这个值的不要
        /// </summary>
        public double Max { get; set; }
        /// <summary>
        /// （数据纠正）在取到的值基础上再加上这个值
        /// </summary>
        public double AddNumber { get; set; }
        /// <summary>
        /// 通道值
        /// </summary>
        public byte channel { get; set; }

        public string 备注 { get; set; }
    }
}
