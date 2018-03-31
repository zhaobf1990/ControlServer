using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Utility;
using 服务器端接收程序.Clazz.Config.ClientConfig;

namespace 服务器端接收程序.Config
{
    public class ClientConfig1
    {
        private string Path;

        public ClientConfig1(string path)
        {
            Path = path;

            AllCountryNode = new List<XML_CountryNode>();
            AllCountryTest = new List<XML_CountryTest>();
            AllStation = new List<XML_Station>();
            AllDevice = new List<XML_DeviceControl>();
            AllPMQCTest = new List<XML_PMQCTest>();
            AllMobileDetection = new List<XML_MobileDetection>();
            AllMCGSTest = new List<XML_MCGSTest>();
        }

        /// <summary>
        /// 所有检测节点
        /// </summary>
        public List<XML_CountryNode> AllCountryNode { get; set; }
        /// <summary>
        /// 农村项目检测指标
        /// </summary>
        public List<XML_CountryTest> AllCountryTest { get; set; }
        /// <summary>
        /// 所有站点
        /// </summary>
        public List<XML_Station> AllStation { get; set; }
        ///// <summary>
        ///// 设备控制中的所有设备
        ///// </summary>
        public List<XML_DeviceControl> AllDevice { get; set; }
        /// <summary>
        /// 屏幕取词的所有检测指标 
        /// </summary>
        public List<XML_PMQCTest> AllPMQCTest { get; set; }
        /// <summary>
        /// 所有移动检测的检测指标
        /// </summary>
        public List<XML_MobileDetection> AllMobileDetection { get; set; }
        /// <summary>
        /// 所有McgsTest
        /// </summary>
        public List<XML_MCGSTest> AllMCGSTest { get; set; }


        public void ReadConfig()
        {
            lock (AllCountryNode)
            {
                AllCountryNode = GetAllCountryNode();
            }
            lock (AllCountryTest)
            {
                AllCountryTest = GetAllCountryTest();
            }
            lock (AllStation)
            {
                AllStation = GetAllStation();
            }
            lock (AllDevice)
            {
                AllDevice = GetAllDeviceControl();
            }
            lock (AllPMQCTest)
            {
                AllPMQCTest = GetAllPMQCTest();
            }
            lock (AllMobileDetection)
            {
                AllMobileDetection = GetAllMobileDetection();
            }
            lock (AllMCGSTest)
            {
                AllMCGSTest = GetAllMCGSTest();
            }
        }

        public void SaveConfig()
        {

        }

        #region CountryTest

        /// <summary>
        /// 获取所有检测节点
        /// </summary>
        /// <returns></returns>
        public List<XML_CountryNode> GetAllCountryNode()
        {
            List<XML_CountryNode> _listCountryTest = new List<XML_CountryNode>();
            try
            {
                XmlNodeList nodeList = Utility.XmlHelper.GetXmlNodeListByXpath(Path, "/ClientConfig/CountryTest/Nodes/Node");
                foreach (XmlNode item in nodeList)
                {
                    XmlElement xel = (XmlElement)item;
                    XML_CountryNode node = new XML_CountryNode();

                    node.NodeId = xel.GetAttribute("NodeId");
                    node.Remark = xel.GetAttribute("备注");

                    _listCountryTest.Add(node);
                }
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
                DEBUG.ThrowException(ex);
            }
            return _listCountryTest;
        }

        /// <summary>
        /// 返回所有农村项目检测点
        /// </summary>
        /// <returns></returns>
        public List<XML_CountryTest> GetAllCountryTest()
        {
            List<XML_CountryTest> _listCountryTest = new List<XML_CountryTest>();
            try
            {
                XmlNodeList nodeList = Utility.XmlHelper.GetXmlNodeListByXpath(Path, "/ClientConfig/CountryTest/Test");

                foreach (XmlNode item in nodeList)
                {
                    XmlElement xel = (XmlElement)item;
                    XML_CountryTest countryTest = new XML_CountryTest();

                    countryTest.UniqueId = int.Parse(xel.GetAttribute("UniqueId"));
                    countryTest.StationUniqueId = int.Parse(xel.GetAttribute("StationUniqueId"));
                    countryTest.NodeId = xel.GetAttribute("NodeId");
                    countryTest.TestId = int.Parse(xel.GetAttribute("TestId"));
                    countryTest.Multiple = double.Parse(xel.GetAttribute("Multiple"));
                    countryTest.Remark = xel.GetAttribute("备注");

                    _listCountryTest.Add(countryTest);
                }
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
                DEBUG.ThrowException(ex);
            }
            return _listCountryTest;
        }

        public bool AddCountryTest(XML_CountryTest ct, ref string msg)
        {
            bool flag = true;
            try
            {
                if (AllCountryTest.Exists(c => c.StationUniqueId == ct.StationUniqueId && c.NodeId == ct.NodeId))
                {
                    msg = ct.StationName + " 站点已经存在 " + ct.Remark + " 检测节点";
                    flag = false;
                }
                else if (AllCountryTest.Exists(c => c.StationUniqueId == ct.StationUniqueId && c.TestId == ct.TestId))
                {
                    msg = ct.StationName + " 站点已经存在testid= " + ct.Remark + " 检测节点";
                    flag = false;
                }
                else
                {
                    lock (AllCountryTest)
                    {
                        int uniqueId = AllCountryTest.Max(c => c.UniqueId);
                        ct.UniqueId = uniqueId + 1;
                        AllCountryTest.Add(ct);
                        flag = AddCountryTestToXML(ct);
                    }
                }
            }
            catch (Exception ex)
            {
                flag = false;
                LogMg.AddError(ex);
            }
            return flag;
        }

        /// <summary>
        /// 保存配置到XML
        /// </summary>
        /// <returns></returns>
        private bool AddCountryTestToXML(XML_CountryTest test)
        {
            try
            {
                XmlDocument XDoc = new XmlDocument();
                XDoc.Load(Path); //加载XML文档

                XmlElement ListTestEl = (XmlElement)XDoc.SelectSingleNode(@"/ClientConfig/CountryTest");

                XmlElement xe = XDoc.CreateElement("Test");
                xe.SetAttribute("UniqueId", test.UniqueId.ToString());
                xe.SetAttribute("StationUniqueId", test.StationUniqueId.ToString());
                xe.SetAttribute("NodeId", test.NodeId.ToString());
                xe.SetAttribute("TestId", test.TestId.ToString());
                xe.SetAttribute("备注", test.Remark);
                xe.SetAttribute("Multiple", test.Multiple.ToString());
                ListTestEl.AppendChild(xe);

                XDoc.Save(Path);
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 修改countrytest
        /// </summary>
        /// <param name="_test"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool EditCountryTest(XML_CountryTest _test, ref string msg)
        {
            bool flag = true;
            try
            {
                lock (AllCountryTest)
                {
                    XML_CountryTest s = AllCountryTest.SingleOrDefault(c => c.UniqueId == _test.UniqueId);
                    AllCountryTest.Remove(s);   //先把这个对象删了
                    AllCountryTest.Add(_test);   //再把新的对象添加进去
                    flag = EditCountryTestFromXML(_test);
                }
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
                flag = false;
            }
            return flag;
        }

        /// <summary>
        /// 修改XML中的countrytest
        /// </summary>
        /// <param name="station"></param>
        /// <returns></returns>
        private bool EditCountryTestFromXML(XML_CountryTest _test)
        {
            try
            {
                XmlDocument XDoc = new XmlDocument();
                XDoc.Load(Path); //加载XML文档

                XmlElement ListTestEl = (XmlElement)XDoc.SelectSingleNode(@"/ClientConfig/CountryTest");

                if (ListTestEl != null)
                {
                    foreach (XmlNode item in ListTestEl.ChildNodes)
                    {
                        XmlElement xe = (XmlElement)item;
                        if (xe.GetAttribute("UniqueId") == _test.UniqueId.ToString())
                        {
                            xe.SetAttribute("StationUniqueId", _test.StationUniqueId.ToString());
                            xe.SetAttribute("NodeId", _test.NodeId.ToString());
                            xe.SetAttribute("TestId", _test.TestId.ToString());
                            xe.SetAttribute("备注", _test.Remark);
                            xe.SetAttribute("Multiple", _test.Multiple.ToString());
                        }
                    }
                }
                XDoc.Save(Path);
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 删除countrytest
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool DelCountryTest(int uniqueId, ref string msg)
        {
            XML_CountryTest _test = AllCountryTest.SingleOrDefault(c => c.UniqueId == uniqueId);
            if (_test != null)
            {
                lock (AllCountryTest)
                {
                    AllCountryTest.Remove(_test);
                    DelCountryTestFromXML(uniqueId);
                    return true;
                }
            }
            else
            {
                msg = "不存在这个站点";
                return false;
            }
        }

        /// <summary>
        /// 从XML删除countrytest
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <returns></returns>
        private bool DelCountryTestFromXML(int uniqueId)
        {
            try
            {
                XmlDocument XDoc = new XmlDocument();
                XDoc.Load(Path); //加载XML文档

                XmlElement ListTestEl = (XmlElement)XDoc.SelectSingleNode(@"/ClientConfig/CountryTest");

                if (ListTestEl != null)
                {
                    foreach (XmlNode item in ListTestEl.ChildNodes)
                    {
                        XmlElement xe = (XmlElement)item;
                        if (xe.GetAttribute("UniqueId") == uniqueId.ToString())
                        {
                            ListTestEl.RemoveChild(item);
                        }
                    }
                }

                XDoc.Save(Path);
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
                return false;
            }
            return true;
        }

        #endregion

        #region Station

        /// <summary>
        /// 返回所有站点
        /// </summary>
        /// <returns></returns>
        public List<Clazz.Config.ClientConfig.XML_Station> GetAllStation()
        {
            List<Clazz.Config.ClientConfig.XML_Station> list = new List<Clazz.Config.ClientConfig.XML_Station>();
            try
            {
                XmlNodeList ListNode = Utility.XmlHelper.GetXmlNodeListByXpath(Path, "/ClientConfig/Stations/Station");
                foreach (XmlNode item in ListNode)
                {
                    XmlElement xe = (XmlElement)item;

                    Clazz.Config.ClientConfig.XML_Station _station = new Clazz.Config.ClientConfig.XML_Station();
                    _station.UniqueId = int.Parse(xe.GetAttribute("UniqueId"));
                    _station.TransferCode = xe.GetAttribute("TransferCode");
                    _station.StationId = int.Parse(xe.GetAttribute("StationId"));
                    _station.OrgId = xe.GetAttribute("OrgId");
                    _station.StationName = xe.GetAttribute("站点名称");
                    list.Add(_station);
                }
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
            }
            return list;
        }

        /// <summary>
        /// 添加站点
        /// </summary>
        /// <param name="station"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool AddStation(Clazz.Config.ClientConfig.XML_Station station, ref string msg)
        {
            bool flag = true;
            try
            {
                if (AllStation.Exists(c => c.StationId == station.StationId && c.OrgId == station.OrgId))
                {
                    msg = "已存在站点Id:" + station.StationId;
                    return false;
                }
                if (AllStation.Exists(c => c.TransferCode == station.TransferCode))
                {
                    msg = "已存在传输编码:" + station.TransferCode;
                    return false;
                }
                if (AllStation.Exists(c => c.StationName == station.StationName))
                {
                    msg = "已存在站点名称:" + station.StationName;
                    return false;
                }

                lock (AllStation)
                {
                    int uniqueId = AllStation.Max(c => c.UniqueId);
                    station.UniqueId = uniqueId + 1;
                    AllStation.Add(station);
                    flag = AddStationToXML(station);
                }
            }
            catch (Exception ex)
            {
                flag = false;
                LogMg.AddError(ex);
            }
            return flag;
        }
        /// <summary>
        /// 保存配置到XML
        /// </summary>
        /// <returns></returns>
        private bool AddStationToXML(Clazz.Config.ClientConfig.XML_Station station)
        {
            try
            {
                XmlDocument XDoc = new XmlDocument();
                XDoc.Load(Path); //加载XML文档

                XmlElement ListTestEl = (XmlElement)XDoc.SelectSingleNode(@"/ClientConfig/Stations");

                XmlElement xe = XDoc.CreateElement("Station");
                xe.SetAttribute("UniqueId", station.UniqueId.ToString());
                xe.SetAttribute("TransferCode", station.TransferCode);
                xe.SetAttribute("StationId", station.StationId.ToString());
                xe.SetAttribute("OrgId", station.OrgId);
                xe.SetAttribute("站点名称", station.StationName);
                ListTestEl.AppendChild(xe);

                XDoc.Save(Path);
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
                return false;
            }
            return true;
        }

     

        public bool EditStation(XML_Station station, ref string msg)
        {
            bool flag = true;

            try
            {
                lock (AllStation)
                {
                    XML_Station s = AllStation.SingleOrDefault(c => c.UniqueId == station.UniqueId);
                    AllStation.Remove(s);   //先把这个对象删了
                    AllStation.Add(station);   //再把新的对象添加进去
                    flag = EditStationFromXML(station);
                }
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
                flag = false;
            }
            return flag;
        }

        private bool EditStationFromXML(XML_Station station)
        {
            try
            {
                XmlDocument XDoc = new XmlDocument();
                XDoc.Load(Path); //加载XML文档

                XmlElement ListTestEl = (XmlElement)XDoc.SelectSingleNode(@"/ClientConfig/Stations");

                if (ListTestEl != null)
                {
                    foreach (XmlNode item in ListTestEl.ChildNodes)
                    {
                        XmlElement xe = (XmlElement)item;
                        if (xe.GetAttribute("UniqueId") == station.UniqueId.ToString())
                        {
                            xe.SetAttribute("TransferCode", station.TransferCode);
                            xe.SetAttribute("StationId", station.StationId.ToString());
                            xe.SetAttribute("OrgId", station.OrgId.ToString());
                            xe.SetAttribute("站点名称", station.StationName);
                        }
                    }
                }

                XDoc.Save(Path);
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 删除站点
        /// </summary>
        /// <param name="stationId"></param>
        /// <param name="stationName"></param>
        /// <param name="orgId"></param>
        /// <param name="transferCode"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool DelStation(int uniqueId, ref string msg)
        {
            Clazz.Config.ClientConfig.XML_Station station = AllStation.SingleOrDefault(c => c.UniqueId == uniqueId);
            if (station != null)
            {
                lock (AllStation)
                {
                    AllStation.Remove(station);
                    DelStationFromXML(uniqueId);
                    return true;
                }
            }
            else
            {
                msg = "不存在这个站点";
                return false;
            }
        }

        private bool DelStationFromXML(int uniqueId)
        {
            try
            {
                XmlDocument XDoc = new XmlDocument();
                XDoc.Load(Path); //加载XML文档

                XmlElement ListTestEl = (XmlElement)XDoc.SelectSingleNode(@"/ClientConfig/Stations");

                if (ListTestEl != null)
                {
                    foreach (XmlNode item in ListTestEl.ChildNodes)
                    {
                        XmlElement xe = (XmlElement)item;
                        if (xe.GetAttribute("UniqueId") == uniqueId.ToString())
                        {
                            ListTestEl.RemoveChild(item);
                        }
                    }
                }

                XDoc.Save(Path);
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 根据传输编码获得orgid
        /// </summary>
        /// <param name="transferCode"></param>
        /// <returns></returns>
        public string GetOrgIdByTransferCode(string transferCode)
        {
            string orgid = "";
            try
            {
                Clazz.Config.ClientConfig.XML_Station station = AllStation.SingleOrDefault(c => c.TransferCode == transferCode);
                if (station != null)
                {
                    orgid = station.OrgId;
                }
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
            }
            return orgid;
        }

        /// <summary>
        /// 根据传输编码获得stationid
        /// </summary>
        /// <param name="transferCode"></param>
        /// <returns></returns>
        public int GetStationIdByTransferCode(string transferCode)
        {
            int stationId = 0;
            try
            {
                Clazz.Config.ClientConfig.XML_Station station = AllStation.SingleOrDefault(c => c.TransferCode == transferCode);
                if (station != null)
                {
                    stationId = station.StationId;
                }
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
            }
            return stationId;
        }

        #endregion

        #region 设备控制
        public List<XML_DeviceControl> GetAllDeviceControl()
        {
            List<XML_DeviceControl> list = new List<XML_DeviceControl>();
            try
            {
                XmlNodeList ListNode = Utility.XmlHelper.GetXmlNodeListByXpath(Path, "/ClientConfig/DeviceControl/Device");
                foreach (XmlNode item in ListNode)
                {
                    XmlElement xe = (XmlElement)item;

                    XML_DeviceControl _device = new XML_DeviceControl();
                    _device.UniqueId = int.Parse(xe.GetAttribute("UniqueId"));
                    _device.StationUniqueId = int.Parse(xe.GetAttribute("StationUniqueId"));
                    _device.Number = xe.GetAttribute("Number");
                    _device.DeviceId = int.Parse(xe.GetAttribute("DeviceId"));

                    list.Add(_device);
                }
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
            }
            return list;
        }

        public bool AddDeviceControl(XML_DeviceControl device, ref string msg)
        {
            bool flag = true;
            try
            {
                if (AllDevice.Exists(c => c.StationUniqueId == device.StationUniqueId && c.Number == device.Number))
                {
                    msg = "该站点已存在设备编号:" + device.Number;
                    flag = false;
                }
                else if (AllDevice.Exists(c => c.StationUniqueId == device.StationUniqueId && c.DeviceId == device.DeviceId))
                {
                    msg = "该站点已存在寄存器编号:" + device.DeviceId;
                    flag = false;
                }
                else
                {
                    lock (AllDevice)
                    {
                        int uniqueId = AllDevice.Max(c => c.UniqueId);
                        device.UniqueId = uniqueId + 1;
                        AllDevice.Add(device);
                        flag = AddDeviceControlToXML(device);
                    }
                }
            }
            catch (Exception ex)
            {
                flag = false;
                LogMg.AddError(ex);
            }
            return flag;
        }

        /// <summary>
        /// 保存配置到XML
        /// </summary>
        /// <returns></returns>
        private bool AddDeviceControlToXML(XML_DeviceControl device)
        {
            try
            {
                XmlDocument XDoc = new XmlDocument();
                XDoc.Load(Path); //加载XML文档

                XmlElement ListTestEl = (XmlElement)XDoc.SelectSingleNode(@"/ClientConfig/DeviceControl");

                XmlElement xe = XDoc.CreateElement("Device");
                xe.SetAttribute("UniqueId", device.UniqueId.ToString());
                xe.SetAttribute("StationUniqueId", device.StationUniqueId.ToString());
                xe.SetAttribute("Number", device.Number.ToString());
                xe.SetAttribute("DeviceId", device.DeviceId.ToString());
                ListTestEl.AppendChild(xe);

                XDoc.Save(Path);
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
                return false;
            }
            return true;
        }


        /// <summary>
        /// 修改DeviceControl
        /// </summary>
        /// <param name="_device"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool EditDeviceControl(XML_DeviceControl _device, ref string msg)
        {
            bool flag = true;
            try
            {
                lock (AllDevice)
                {
                    XML_DeviceControl dc = AllDevice.SingleOrDefault(c => c.UniqueId == _device.UniqueId);
                    AllDevice.Remove(dc);   //先把这个对象删了
                    AllDevice.Add(_device);   //再把新的对象添加进去
                    flag = EditDeviceControlFromXML(_device);
                }
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
                flag = false;
            }
            return flag;
        }


        /// <summary>
        /// 修改XML中的DeviceControl
        /// </summary>
        /// <param name="station"></param>
        /// <returns></returns>
        private bool EditDeviceControlFromXML(XML_DeviceControl _device)
        {
            try
            {
                XmlDocument XDoc = new XmlDocument();
                XDoc.Load(Path); //加载XML文档

                XmlElement ListTestEl = (XmlElement)XDoc.SelectSingleNode(@"/ClientConfig/DeviceControl");

                if (ListTestEl != null)
                {
                    foreach (XmlNode item in ListTestEl.ChildNodes)
                    {
                        XmlElement xe = (XmlElement)item;
                        if (xe.GetAttribute("UniqueId") == _device.UniqueId.ToString())
                        {
                            xe.SetAttribute("StationUniqueId", _device.StationUniqueId.ToString());
                            xe.SetAttribute("Number", _device.Number.ToString());
                            xe.SetAttribute("DeviceId", _device.DeviceId.ToString());
                        }
                    }
                }
                XDoc.Save(Path);
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 删除设备控制 
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool DelDeviceControl(int uniqueId, ref string msg)
        {
            XML_DeviceControl _device = AllDevice.SingleOrDefault(c => c.UniqueId == uniqueId);
            if (_device != null)
            {
                lock (AllDevice)
                {
                    AllDevice.Remove(_device);
                    DelCountryTestFromXML(uniqueId);
                    return true;
                }
            }
            else
            {
                msg = "不存在这个站点";
                return false;
            }
        }

        /// <summary>
        /// 从XML删除DeviceControl
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <returns></returns>
        private bool DelDeviceControlFromXML(int uniqueId)
        {
            try
            {
                XmlDocument XDoc = new XmlDocument();
                XDoc.Load(Path); //加载XML文档

                XmlElement ListTestEl = (XmlElement)XDoc.SelectSingleNode(@"/ClientConfig/DeviceControl");

                if (ListTestEl != null)
                {
                    foreach (XmlNode item in ListTestEl.ChildNodes)
                    {
                        XmlElement xe = (XmlElement)item;
                        if (xe.GetAttribute("UniqueId") == uniqueId.ToString())
                        {
                            ListTestEl.RemoveChild(item);
                        }
                    }
                }

                XDoc.Save(Path);
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
                return false;
            }
            return true;
        }


        #endregion

        #region 屏幕取词
        /// <summary>
        /// 获取所有屏幕取词的检测点
        /// </summary>
        /// <returns></returns>
        public List<XML_PMQCTest> GetAllPMQCTest()
        {
            List<XML_PMQCTest> list = new List<XML_PMQCTest>();
            try
            {
                XmlNodeList ListNode = Utility.XmlHelper.GetXmlNodeListByXpath(Path, "/ClientConfig/PMQC/Test");
                foreach (XmlNode item in ListNode)
                {
                    XmlElement xe = (XmlElement)item;

                    XML_PMQCTest _test = new XML_PMQCTest();
                    _test.UniqueId = int.Parse(xe.GetAttribute("UniqueId"));
                    _test.StationUniqueId = int.Parse(xe.GetAttribute("StationUniqueId"));
                    _test.Id = int.Parse(xe.GetAttribute("Id"));
                    _test.Name = xe.GetAttribute("Name");
                    _test.X = int.Parse(xe.GetAttribute("X"));
                    _test.Y = int.Parse(xe.GetAttribute("Y"));
                    _test.TestId = int.Parse(xe.GetAttribute("TestId"));
                    list.Add(_test);
                }
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
            }
            return list;
        }
        #endregion

        #region 移动检测
        public List<XML_MobileDetection> GetAllMobileDetection()
        {
            List<XML_MobileDetection> list = new List<XML_MobileDetection>();
            try
            {
                XmlNodeList ListNode = Utility.XmlHelper.GetXmlNodeListByXpath(Path, "/ClientConfig/MobileDetection/Test");
                foreach (XmlNode item in ListNode)
                {
                    XmlElement xe = (XmlElement)item;

                    XML_MobileDetection _test = new XML_MobileDetection();
                    _test.UniqueId = int.Parse(xe.GetAttribute("UniqueId"));
                    _test.StationUniqueId = int.Parse(xe.GetAttribute("StationUniqueId"));
                    _test.TestId = int.Parse(xe.GetAttribute("TestId"));
                    _test.TestTarger = xe.GetAttribute("检测指标");

                    list.Add(_test);
                }
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
            }
            return list;
        }

        #endregion

        #region MCGS

        /// <summary>
        /// 获取mcgstest
        /// </summary>
        /// <returns></returns>
        public List<XML_MCGSTest> GetAllMCGSTest()
        {
            List<XML_MCGSTest> _listMcgsTest = new List<XML_MCGSTest>();
            try
            {
                XmlNodeList nodeList = Utility.XmlHelper.GetXmlNodeListByXpath(Path, "/ClientConfig/MCGS/Test");

                foreach (XmlNode item in nodeList)
                {
                    XmlElement xel = (XmlElement)item;
                    XML_MCGSTest mcgs = new XML_MCGSTest();
                    mcgs.UniqueId = int.Parse(xel.GetAttribute("UniqueId"));
                    mcgs.StationUniqueId = int.Parse(xel.GetAttribute("StationUniqueId"));
                    mcgs.TestId = int.Parse(xel.GetAttribute("TestId"));
                    mcgs.ColumnName = xel.GetAttribute("ColumnName");
                    mcgs.TestName = xel.GetAttribute("TestName");

                    _listMcgsTest.Add(mcgs);
                }
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
                DEBUG.ThrowException(ex);
            }
            return _listMcgsTest;
        }

        #endregion


      
    }
}
