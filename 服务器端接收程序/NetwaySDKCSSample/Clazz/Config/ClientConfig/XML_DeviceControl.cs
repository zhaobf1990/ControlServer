using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using 服务器端接收程序.Config;
using Utility;

namespace 服务器端接收程序.Clazz.Config.ClientConfig
{
    public class XML_DeviceControl : CSDataStandard.Config.DeviceControl
    {
        public int UniqueId { get; set; }
        public int StationUniqueId { get; set; }
        public string StationName
        {
            get
            {
                string name = "";

                try
                {
                    XML_Station s = SysConfig.clientConfig.AllStation.SingleOrDefault(c => c.UniqueId == StationUniqueId);
                    if (s != null)
                    {
                        name = s.StationName;
                    }
                }
                catch (Exception ex)
                {
                    LogMg.AddError(ex);
                }
                return name;
            }

        }
    }
}
