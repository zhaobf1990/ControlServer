using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 服务器端接收程序.Clazz.Config.ClientConfig
{
    public class XML_Station
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int UniqueId { get; set; }
        /// <summary>
        /// 传输编码
        /// </summary>
        public string TransferCode { get; set; }
        /// <summary>
        /// 站点id
        /// </summary>
        public int StationId { get; set; }
        /// <summary>
        /// OrgId
        /// </summary>
        public string OrgId { get; set; }
        /// <summary>
        /// 站点名称 
        /// </summary>
        public string StationName { get; set; }
    }
}
