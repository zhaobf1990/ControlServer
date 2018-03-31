using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 服务器端接收程序.Clazz.Config.ClientConfig
{
    public class XML_MobileDetection : CSDataStandard.Config.MobileDetection
    {
        public int UniqueId { get; set; }
        public int StationUniqueId { get; set; }
    }
}
