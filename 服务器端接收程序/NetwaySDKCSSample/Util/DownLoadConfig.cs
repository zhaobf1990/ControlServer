using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using Utility;
using CSDataStandard;
using 服务器端接收程序.Config;
using CSDataStandard.Config;


namespace 服务器端接收程序.Util
{

    public class DownLoadConfig
    {
        public static void DownLoad(Socket socket, string json)
        {
            S_To_C_Data<ClientConfig> sendObj = new S_To_C_Data<ClientConfig>();
            ClientConfig _clientConfig = new ClientConfig();

            try
            {
                C_To_S_Data<ClientConfig> obj = Utility.JsonHelper.JsonDeserialize<C_To_S_Data<ClientConfig>>(json);
                Clazz.Config.ClientConfig.XML_Station _station = SysConfig.clientConfig.AllStation.SingleOrDefault(c => c.TransferCode == obj.TransferCode);

                if (_station == null)
                {
                    LogMg.AddDebug("TransferCode:" + obj.TransferCode + " 不存在");
                }
                else
                {
                    _clientConfig.OrgId = SysConfig.clientConfig.GetOrgIdByTransferCode(obj.TransferCode);
                    _clientConfig.StationId = SysConfig.clientConfig.GetStationIdByTransferCode(obj.TransferCode);
                    _clientConfig.ListCountryTest = (from c in SysConfig.clientConfig.AllCountryTest
                                                     where c.StationUniqueId == _station.UniqueId
                                                     select new CSDataStandard.Config.CountryTest
                                                     {
                                                         NodeId = c.NodeId,
                                                         TestId = c.TestId,
                                                         Multiple = c.Multiple
                                                     }).ToList();
                    _clientConfig.ListDevice = new List<DeviceControl>();
                    //(from c in SysConfig.clientConfig.AllDevice
                    //                        where c.StationUniqueId == _station.UniqueId
                    //                        select new CSDataStandard.Config.DeviceControl
                    //                        {
                    //                            Number = c.Number,
                    //                            DeviceId = c.DeviceId
                    //                        }).ToList();
                    _clientConfig.ListPMQCTest = (from c in SysConfig.clientConfig.AllPMQCTest
                                                  where c.StationUniqueId == _station.UniqueId
                                                  select new CSDataStandard.Config.PMQCTest
                                                  {
                                                      X = c.X,
                                                      Y = c.Y,
                                                      TestId = c.TestId,
                                                      Id = c.Id,
                                                      Name = c.Name
                                                  }).ToList();
                    _clientConfig.ListMobileDetection = (from c in SysConfig.clientConfig.AllMobileDetection
                                                         where c.StationUniqueId == _station.UniqueId
                                                         select new CSDataStandard.Config.MobileDetection
                                                         {
                                                             TestId = c.TestId,
                                                             TestTarger = c.TestTarger
                                                         }).ToList();
                    _clientConfig.ListMcgsTest = (from c in SysConfig.clientConfig.AllMCGSTest
                                                  where c.StationUniqueId == _station.UniqueId
                                                  select new CSDataStandard.Config.MCGSTest
                                                  {
                                                      TestId = c.TestId,
                                                      ColumnName = c.ColumnName,
                                                      TestName = c.TestName
                                                  }).ToList();
                }
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
            }

            sendObj.Data = new List<ClientConfig>();
            sendObj.Data.Add(_clientConfig);   //添加数据
            sendObj.Flag = CSDataStandard.Enum.HandleFlag.DownLoadConfig;   //类型为  下载配置文件 
            sendObj.Success = true;

            string sendJSON = Utility.JsonHelper.JsonSerializer(sendObj);
            socket.Send(Encoding.Unicode.GetBytes(sendJSON));
        }
    }
}
