using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility;
using 服务器端接收程序.SWS_DB;
using 服务器端接收程序.Config;
using log4net;
using 服务器端接收程序.Util;

namespace 服务器端接收程序.Clazz.Config.GuangDai
{
    public class GD_Config
    {
        private static ILog log = LogManager.GetLogger(typeof(GuangDaiCommunicationModbus));
        public List<GD_Station> GD_Stations { get; set; }


        public GD_Config()
        {
            GD_Stations = new List<GD_Station>();
        }
        /// <summary>
        /// 读取配置
        /// </summary>
        public bool GenerateConfig()
        {
            try
            {
                List<GD_Station> gd_stations = new List<GD_Station>();  //从数据库中找出来的站点信息列表
                SWS_DBDataContext SWS_DB = new SWS_DBDataContext(ConnectStringHelper.GetConnection(SysConfig.userProfile.DbAddress, SysConfig.userProfile.DbName, SysConfig.userProfile.DbUserName, SysConfig.userProfile.DbPassword));
                List<guangdai_station_link> links = SWS_DB.guangdai_station_link.ToList();
                List<SWS_DB.org> db_orgs = SWS_DB.org.Where(c => c.org_type == 2 || c.org_type == 3).ToList();
                log.Info("=========================================================================");
                foreach (SWS_DB.org _org in db_orgs)
                {
                    try
                    {
                        log.Info("_org.dbname: " + _org.dbname + "   org.orgid: " + _org.orgid);
                        SWSDataContext country_db = new SWSDataContext(ConnectStringHelper.GetConnection(SysConfig.userProfile.DbAddress, _org.dbname, SysConfig.userProfile.DbUserName, SysConfig.userProfile.DbPassword));
                        List<country_station> stations = country_db.ExecuteQuery<country_station>("select id,name from country_station where deleteflag=0 and jiankongyitiji_version=5").ToList();
                        //List<country_station> stations = country_db.country_station.Where(c => c.deleteflag == false && c.jiankongyitiji_version == 5).ToList();
                        foreach (country_station item in stations)
                        {
                            log.Info("station.name: " + item.name + "   station.id: " + item.id);
                            GD_Station GD_station = new GD_Station();
                            GD_station.tests = new List<XML_Test>();
                            GD_station.dbName = _org.dbname;
                            GD_station.OrgId = _org.orgid;
                            GD_station.name = item.name;
                            GD_station.stationId = item.id;
                            try
                            {
                                guangdai_station_link link = links.Single(c => c.station_id == item.id && c.db_name == _org.dbname && c.type == 1);
                                GD_station.wscId = link.wsid;
                                GD_station.Unique = link.id;
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                            //找出站点的检测指标
                            List<test> tests = country_db.test.Where(c => c.station_id == item.id && c.means.Contains("自动获取") && c.delete_flag == false).ToList();
                            List<gong_kuang_config> gkcs = country_db.gong_kuang_config.Where(c => c.testid != 0 && c.config_type != "03" && c.config_type != "04").ToList();

                            foreach (test _test in tests)
                            {
                                try
                                {
                                    List<gong_kuang_config> gkc = gkcs.Where(c => c.testid == _test.testid).ToList();  //这个一般只有一行   但配置的时候可能会出现两行，
                                    if (gkc.Count > 0)
                                    {
                                        XML_Test gd_test = new XML_Test();
                                        gd_test.StationUnique = GD_station.Unique;
                                        gd_test.RegisterNo = ushort.Parse(gkc[0].read_register);
                                        gd_test.TestId = _test.testid;
                                        gd_test.Multiple = gkc[0].Multiple == null ? 1 : (double)gkc[0].Multiple;   //如果没填，则默认为0  
                                        gd_test.FunctionCode = int.Parse(gkc[0].function_code);
                                        gd_test.ReceiveTimeout = (int)gkc[0].receive_timeout;
                                        gd_test.DataType = gkc[0].data_type;
                                        gd_test.Address = byte.Parse(gkc[0].address.ToString());
                                        if (!string.IsNullOrEmpty(gkc[0].decode_order))
                                        {
                                            gd_test.DecodeOrder = gkc[0].decode_order;
                                        }
                                        gd_test.Min = gkc[0].dmin == null ? 0 : (double)gkc[0].dmin;   //如果没填，则默认为0 
                                        gd_test.Max = gkc[0].dmax == null ? 4294967296 : (double)gkc[0].dmax;   //如果没填，则默认为0 
                                        gd_test.AddNumber = gkc[0].AddNumber == null ? 0 : (double)gkc[0].AddNumber;   //如果没填，则默认为0  
                                        gd_test.备注 = _test.name;
                                        GD_station.tests.Add(gd_test);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogMg.AddError(ex);
                                }
                            }
                            //if (GD_station.stationId == 629)
                            gd_stations.Add(GD_station);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogMg.AddError(ex);
                    }
                }
                log.Info("-----------------------------");
                for (int i = 0; i < gd_stations.Count; i++)
                {
                    GD_Station item = gd_stations[i];
                    log.Info("orgId: " + item.OrgId + "  dbName: " + item.dbName + " stationId: " + item.stationId + " station.name" + item.name);
                }
                this.GD_Stations = gd_stations;
               

                return true;
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
                return false;
            }
        }
    }
}
