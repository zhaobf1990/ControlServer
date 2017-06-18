using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Utility;
using 服务器端接收程序.SWS_DB;

namespace 服务器端接收程序.Config
{
    public class DTU_StationConfig
    {
        public static string PATH;
        public List<Clazz.Config.XML_Station> Stations { get; set; }
        public List<Clazz.Config.XML_Test> AllTests;

        public DTU_StationConfig(string path)
        {
            PATH = path;
            Stations = new List<Clazz.Config.XML_Station>();
            AllTests = new List<Clazz.Config.XML_Test>();
        }



        /// <summary>
        /// 读取配置文件数据
        /// </summary>
        public void ReadConfig()
        {
            lock (Stations)
            {
                Stations = GetAllStation();
                AllTests = GetAllTests();
            }

        }

        /// <summary>
        /// 保存配置到XML文件
        /// </summary>
        public void SaveConfig()
        {
            //貌似这个类暂时不需要这个保存方法    先把方法写着    反正没有实现
        }

        /// <summary>
        /// 返回所有的Station
        /// </summary>
        /// <returns></returns>
        private List<Clazz.Config.XML_Station> GetAllStation()
        {
            List<Clazz.Config.XML_Station> ListStation = new List<Clazz.Config.XML_Station>();
            try
            {
                XmlNodeList nodeList = Utility.XmlHelper.GetXmlNodeListByXpath(PATH, "/Config/Stations/Station");
                foreach (XmlNode item in nodeList)
                {
                    XmlElement xel = (XmlElement)item;
                    Clazz.Config.XML_Station station = new Clazz.Config.XML_Station();
                    station.Unique = int.Parse(xel.GetAttribute("Unique"));  //主键
                    station.StationId = int.Parse(xel.GetAttribute("StationId"));  //站点Id
                    station.Name = xel.GetAttribute("Name");  //站点名称
                    station.OrgId = xel.GetAttribute("OrgId");
                    station.Tel = xel.GetAttribute("Tel");  //手机号码         
                    station.Protocol = xel.GetAttribute("Protocol");  //手机号码      
                    ListStation.Add(station);
                }
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
                DEBUG.ThrowException(ex);
            }
            return ListStation;
        }


        public Clazz.Config.XML_Station GetStationByTel(string tel)
        {
            Clazz.Config.XML_Station station = null;

            station = Stations.SingleOrDefault(c => c.Tel == tel);

            return station;
        }

        /// <summary>
        /// 获取所有Test
        /// </summary> 
        /// <returns></returns>
        public List<Clazz.Config.XML_Test> GetAllTests()
        {
            List<Clazz.Config.XML_Test> ListTest = new List<Clazz.Config.XML_Test>();
            try
            {
                XmlNodeList nodeList = Utility.XmlHelper.GetXmlNodeListByXpath(PATH, "/Config/Tests/Test");
                foreach (XmlNode item in nodeList)
                {
                    XmlElement xel = (XmlElement)item;
                    Clazz.Config.XML_Test test = new Clazz.Config.XML_Test();
                    test.StationUnique = int.Parse(xel.GetAttribute("StationUnique"));
                    test.StationId = int.Parse(xel.GetAttribute("StationId"));
                    test.RegisterNo = ushort.Parse(xel.GetAttribute("RegisterNo"));
                    test.TestId = int.Parse(xel.GetAttribute("TestId"));
                    test.Multiple = double.Parse(xel.GetAttribute("Multiple"));
                    test.FunctionCode = Convert.ToInt32(xel.GetAttribute("FunctionCode"));
                    test.ReceiveTimeout = Convert.ToInt32(xel.GetAttribute("ReceiveTimeout"));
                    test.DataType = xel.GetAttribute("DataType");
                    test.Address = byte.Parse(xel.GetAttribute("Address"));
                    test.DecodeOrder = xel.GetAttribute("DecodeOrder");
                    test.Min = double.Parse(xel.GetAttribute("Min"));
                    test.Max = double.Parse(xel.GetAttribute("Max"));
                    test.AddNumber = double.Parse(xel.GetAttribute("AddNumber"));
                    ListTest.Add(test);
                }
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
                DEBUG.ThrowException(ex);
            }
            return ListTest;
        }

        #region 从数据库生成配置到XML    其中包含站点配置信息和检测指标信息
        public bool GenerateFromDbToXML()
        {
            List<Clazz.Config.XML_Station> XML_stations = new List<Clazz.Config.XML_Station>();  //从数据库中找出来的站点信息列表
            List<Clazz.Config.XML_Test> XML_Tests = new List<Clazz.Config.XML_Test>();//从数据库中找出来的test信息
            SWS_DBDataContext SWS_DB = new SWS_DBDataContext(ConnectStringHelper.GetConnection(SysConfig.userProfile.DbAddress, SysConfig.userProfile.DbName, SysConfig.userProfile.DbUserName, SysConfig.userProfile.DbPassword));
            List<SWS_DB.org> db_orgs = SWS_DB.org.Where(c => c.org_type == 2 || c.org_type==3).ToList();
            foreach (SWS_DB.org _org in db_orgs)
            {
                SWSDataContext country_db = new SWSDataContext(ConnectStringHelper.GetConnection(SysConfig.userProfile.DbAddress, _org.dbname, SysConfig.userProfile.DbUserName, SysConfig.userProfile.DbPassword));
                try
                {
                    //List<country_station> db_stations = country_db.country_station.Where(c => c.transfer_code != "" && c.transfer_code != "NULL" && c.Protocol != "" && c.deleteflag == false).ToList();
                    List<country_station> db_stations = country_db.ExecuteQuery<country_station>("select * from country_station where transfer_code <> '' and transfer_code <> 'NULL' and Protocol <> '' and deleteflag=0").ToList();
                    List<syscode> syscodes = country_db.syscode.Where(c => c.type == "0060").ToList();
                    foreach (country_station station in db_stations)
                    {
                        try
                        {
                            //将站点信息添加到列表
                            Clazz.Config.XML_Station xml_sta = new Clazz.Config.XML_Station();
                            xml_sta.Unique = XML_stations.Count;
                            xml_sta.StationId = station.id;
                            xml_sta.Name = station.name;
                            xml_sta.OrgId = _org.orgid.ToString();
                            xml_sta.Tel = station.transfer_code;
                            xml_sta.Protocol = syscodes.Single(c => c.code == station.Protocol).name;

                            XML_stations.Add(xml_sta);

                            //找出站点的检测指标
                            List<test> tests = country_db.test.Where(c => c.station_id == station.id && c.means.Contains("自动获取") && c.delete_flag == false).ToList();
                            List<gong_kuang_config> gkcs = country_db.gong_kuang_config.Where(c => c.testid != 0 && c.config_type != "03" && c.config_type != "04").ToList();

                            foreach (test _test in tests)
                            {
                                try
                                {
                                    List<gong_kuang_config> gkc = gkcs.Where(c => c.testid == _test.testid).ToList();  //这个一般只有一行   但配置的时候可能会出现两行，
                                    if (gkc.Count > 0)
                                    {
                                        Clazz.Config.XML_Test xml_t = new Clazz.Config.XML_Test();
                                        xml_t.StationUnique = xml_sta.Unique;
                                        xml_t.StationId = xml_sta.StationId;
                                        xml_t.RegisterNo = ushort.Parse(gkc[0].read_register);
                                        xml_t.TestId = _test.testid;
                                        xml_t.Multiple = gkc[0].Multiple == null ? 1 : (double)gkc[0].Multiple;   //如果没填，则默认为0  
                                        xml_t.FunctionCode = int.Parse(gkc[0].function_code);
                                        xml_t.ReceiveTimeout = (int)gkc[0].receive_timeout;
                                        xml_t.DataType = gkc[0].data_type;
                                        xml_t.Address = byte.Parse(gkc[0].address.ToString());
                                        if (!string.IsNullOrEmpty(gkc[0].decode_order))
                                        {
                                            xml_t.DecodeOrder = gkc[0].decode_order;
                                        }
                                        xml_t.Min = gkc[0].dmin == null ? 0 : (double)gkc[0].dmin;   //如果没填，则默认为0 
                                        xml_t.Max = gkc[0].dmax == null ? 4294967296 : (double)gkc[0].dmax;   //如果没填，则默认为0 
                                        xml_t.AddNumber = gkc[0].AddNumber == null ? 0 : (double)gkc[0].AddNumber;   //如果没填，则默认为0  
                                        xml_t.备注 = _test.name;
                                        XML_Tests.Add(xml_t);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogMg.AddError(ex);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogMg.AddError(ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogMg.AddError(ex);
                }

            }
            SaveConfigToXML(XML_stations, XML_Tests);
            return true;

        }

        /// <summary>
        /// 保存所有站点配置和所有检测指标配置到XML
        /// </summary>
        /// <returns></returns>
        private bool SaveConfigToXML(List<Clazz.Config.XML_Station> stations, List<Clazz.Config.XML_Test> tests)
        {
            try
            {
                XmlDocument XDoc = new XmlDocument();
                XDoc.Load(PATH); //加载XML文档

                XmlElement ListStationsEl = (XmlElement)XDoc.SelectSingleNode(@"/Config/Stations");
                ListStationsEl.RemoveAll();//删除原先所有的站点
                foreach (Clazz.Config.XML_Station station in stations)
                {
                    XmlElement xe = XDoc.CreateElement("Station");
                    xe.SetAttribute("Unique", station.Unique.ToString());
                    xe.SetAttribute("StationId", station.StationId.ToString());
                    xe.SetAttribute("Name", station.Name);
                    xe.SetAttribute("OrgId", station.OrgId);
                    xe.SetAttribute("Tel", station.Tel);
                    xe.SetAttribute("Protocol", station.Protocol);
                    ListStationsEl.AppendChild(xe);
                }

                XmlElement ListTestEl = (XmlElement)XDoc.SelectSingleNode(@"/Config/Tests");
                ListTestEl.RemoveAll();
                foreach (Clazz.Config.XML_Test test in tests)
                {
                    XmlElement xe = XDoc.CreateElement("Test");
                    xe.SetAttribute("StationUnique", test.StationUnique.ToString());
                    xe.SetAttribute("StationId", test.StationId.ToString());
                    xe.SetAttribute("RegisterNo", test.RegisterNo.ToString());
                    xe.SetAttribute("TestId", test.TestId.ToString());
                    xe.SetAttribute("备注", test.备注);
                    xe.SetAttribute("Multiple", test.Multiple.ToString());
                    xe.SetAttribute("FunctionCode", test.FunctionCode.ToString());
                    xe.SetAttribute("ReceiveTimeout", test.ReceiveTimeout.ToString());
                    xe.SetAttribute("DataType", test.DataType.ToString());
                    xe.SetAttribute("Address", test.Address.ToString());
                    if (!string.IsNullOrEmpty(test.DecodeOrder))
                    {
                        xe.SetAttribute("DecodeOrder", test.DecodeOrder.ToString());
                    }
                    xe.SetAttribute("Min", test.Min.ToString());
                    xe.SetAttribute("Max", test.Max.ToString());
                    xe.SetAttribute("AddNumber", test.AddNumber.ToString());
                    ListTestEl.AppendChild(xe);
                }
                XDoc.Save(PATH);
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
                return false;
            }
            return true;
        }
        #endregion
    }
}
