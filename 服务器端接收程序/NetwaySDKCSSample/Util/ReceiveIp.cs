using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using 服务器端接收程序.Util;
using System.Net.Sockets;
using CSDataStandard;
using 服务器端接收程序;
using Utility;
using 服务器端接收程序.Config;

namespace 服务器端接收程序.Util
{
    /// <summary>
    ///  接收客户端外网IP
    /// </summary>
    public class ReceiveIp
    {
        public ReceiveIp()
        {
            ServerSocketHelper.ClientPublicIpCallBack += Save;
        }

        public void Save(Socket socket, string json)
        {
            C_To_S_Data<object> obj = Utility.JsonHelper.JsonDeserialize<C_To_S_Data<object>>(json);

            Clazz.Config.XML_Org _org = SysConfig.orgConfig.GetOrgByOrgId(obj.OrgId);

            if (_org == null)
            {
                //将信息写入到日志文件中    orgid为***的污水厂不存在 
                LogMg.AddError("OrgId:\"{0}\"不存在");
                //isSuccess = false;
            }
            else
            {
                try
                {
                    SWSDataContext SWS = new SWSDataContext(ServerSocketHelper.GetConnection(_org.DBName));     //建立一个分厂数据源提供程序实例
                    country_station station = SWS.country_station.SingleOrDefault(c => c.id == obj.StationId);
                    if (station == null)
                    {
                        LogMg.AddError("StationId: " + obj.StationId + " 不存在");
                    }
                    else
                    {

                        station.ip = obj.Data[0].ToString();     //保存客户端IP地址
                        SWS.SubmitChanges();
                    }
                }
                catch (Exception ex)
                {
                    LogMg.AddError(ex);
                }
            }
        }
    }
}
