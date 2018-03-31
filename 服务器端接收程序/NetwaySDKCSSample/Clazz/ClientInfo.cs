using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 服务器端接收程序.Clazz
{
    public class ClientInfo
    {
        /// <summary>
        /// 传输编码 
        /// </summary>
        public string TransferCode { get; set; }
        /// <summary>
        /// 站点ID
        /// </summary>
        public int? StationId { get; set; }
        /// <summary>
        /// 站点名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 电话号码   
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// 最后一次访问时间
        /// </summary>
        public DateTime LastVisitTime { get; set; }
        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime RegisterTime { get; set; }
    }
}
