using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using Utility;

namespace 服务器端接收程序.Config
{
    /// <summary>
    /// 用户配置类
    /// </summary>
    public class UserProfile
    {
        private static string UserProfilePath;

        public UserProfile(string path)
        {
            UserProfilePath = path;

            DbAddress = "";
            DbName = "";
            DbUserName = "";
            DbPassword = "";
            serverIp = "";
            DBCJPort = "";
            SWSPort = "";
            DTUPort = "";
            UDPPort = "";

        }
        /// <summary>
        /// 数据库服务器
        /// </summary>
        public string DbAddress { get; set; }
        /// <summary>
        /// 总厂数据库名
        /// </summary>
        public string DbName { get; set; }
        /// <summary>
        /// 数据库用户名
        /// </summary>
        public string DbUserName { get; set; }

        /// <summary>
        /// 数据库密码
        /// </summary>
        public string DbPassword { get; set; }
        /// <summary>
        /// 电表采集服务IP地址
        /// </summary>
        public string serverIp { get; set; }
        /// <summary>
        /// 智能水务电表采集端口
        /// </summary>
        public string DBCJPort { get; set; }
        /// <summary>
        /// 智能水务检测数据端口
        /// </summary>
        public string SWSPort { get; set; }
        /// <summary>
        /// 智能水务采用DTU模式的服务端口
        /// </summary>
        public string DTUPort { get; set; }
        /// <summary>
        /// 智能水务采用UDP模式的服务端口
        /// </summary>
        public string UDPPort { get; set; }
        /// <summary>
        /// 自动开启服务的延迟时间  单位毫秒
        /// </summary>
        public double AutoStart { get; set; }
        /// <summary>
        /// 向客户端发送请求的时间间隔   单位毫秒
        /// </summary>
        public double RequestToClientInterval { get; set; }
        /// <summary>
        ///-向客户端执行设备控制和工况配置的时间间隔   单位毫秒
        /// </summary>
        public double GongKuangConfigInterval { get; set; }
        /// <summary>
        /// 处理心跳间隔   单位秒
        /// </summary>
        public double HeartbeatInterval { get; set; }
        /// <summary>
        /// 设备控制执行失败的次数超过指定次数后,则不再执行
        /// </summary>
        public int ExecuteFailureCount { get; set; }
        /// <summary>
        /// 处理第一次连接的线程池最小线程数
        /// </summary>
        public int NetServerThreadPool_Min { get; set; }
        /// <summary>
        /// 处理第一次连接的线程池最大线程数
        /// </summary>
        public int NetServerThreadPool_Max { get; set; }
        /// <summary>
        /// 主动发指令线程池最小线程数
        /// </summary>
        public int ControlCommandThreadPool_Min { get; set; }
        /// <summary>
        /// 主动发指令线程池最大线程数
        /// </summary>
        public int ControlCommandThreadPool_Max { get; set; }
        /// <summary>
        /// 定时发指令线程池最小线程数
        /// </summary>
        public int AutoCollectionThreadPool_Min { get; set; }
        /// <summary>
        /// 定时发指令线程池最大线程数
        /// </summary>
        public int AutoCollectionThreadPool_Max { get; set; }
        /// <summary>
        /// 心跳线程池最小线程数
        /// </summary>
        public int HeartbeatThreadPool_Min { get; set; }
        /// <summary>
        /// 心跳线程池最大线程数
        /// </summary>
        public int HeartbeatThreadPool_Max { get; set; }
        /// <summary>
        /// V88线程池最小线程数
        /// </summary>
        public int V88ThreadPool_Min { get; set; }
        /// <summary>
        ///  V88线程池最大线程数
        /// </summary>
        public int V88ThreadPool_Max { get; set; }

        /// <summary>
        /// 读取配置文件数据
        /// </summary>
        public void ReadConfig()
        {
            lock (DbAddress)
            {
                DbAddress = GetItemByKey("dbAddress");
            }
            lock (DbName)
            {
                DbName = GetItemByKey("dbName");
            }
            lock (DbUserName)
            {
                DbUserName = GetItemByKey("dbUserName");
            }
            lock (DbPassword)
            {
                DbPassword = GetItemByKey("dbPassword");
            }
            lock (serverIp)
            {
                serverIp = GetItemByKey("serverIp");
            }
            lock (DBCJPort)
            {
                DBCJPort = GetItemByKey("DBCJPort");
            }
            lock (SWSPort)
            {
                SWSPort = GetItemByKey("SWSPort");
            }
            lock (DTUPort)
            {
                DTUPort = GetItemByKey("DTUPort");
            }
            lock (UDPPort)
            {
                UDPPort = GetItemByKey("UDPPort");
            }
            AutoStart = GetSecond("autoStart");
            RequestToClientInterval = GetSecond("RequestToClientInterval");
            GongKuangConfigInterval = GetSecond("GongKuangConfigInterval");
            HeartbeatInterval = GetSecond("HeartbeatInterval");
            try
            {
                ExecuteFailureCount = int.Parse(GetItemByKey("ExecuteFailureCount"));
                NetServerThreadPool_Min = int.Parse(GetItemByKey("NetServerThreadPool_Min"));
                NetServerThreadPool_Max = int.Parse(GetItemByKey("NetServerThreadPool_Max"));
                ControlCommandThreadPool_Min = int.Parse(GetItemByKey("ControlCommandThreadPool_Min"));
                ControlCommandThreadPool_Max = int.Parse(GetItemByKey("ControlCommandThreadPool_Max"));
                AutoCollectionThreadPool_Min = int.Parse(GetItemByKey("AutoCollectionThreadPool_Min"));
                AutoCollectionThreadPool_Max = int.Parse(GetItemByKey("AutoCollectionThreadPool_Max"));
                HeartbeatThreadPool_Min = int.Parse(GetItemByKey("HeartbeatThreadPool_Min"));
                HeartbeatThreadPool_Max = int.Parse(GetItemByKey("HeartbeatThreadPool_Max"));
                V88ThreadPool_Min = int.Parse(GetItemByKey("V88ThreadPool_Min"));
                V88ThreadPool_Max = int.Parse(GetItemByKey("V88ThreadPool_Max"));
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
            }

        }

        /// <summary>
        /// 保存配置到XML文件
        /// </summary>
        public void SaveConfig()
        {
            SetItemByKey("dbAddress", DbAddress);
            SetItemByKey("dbName", DbName);
            SetItemByKey("dbUserName", DbUserName);
            SetItemByKey("dbPassword", DbPassword);
            SetItemByKey("serverIp", serverIp);
            SetItemByKey("DBCJPort", DBCJPort);
            SetItemByKey("SWSPort", SWSPort);
            SetItemByKey("DTUPort", DTUPort);
            SetItemByKey("UDPPort", UDPPort);
            SetItemByKey("autoStart", Convert.ToString(AutoStart / 1000));
            SetItemByKey("RequestToClientInterval", Convert.ToString(RequestToClientInterval / 1000));
            SetItemByKey("GongKuangConfigInterval", Convert.ToString(GongKuangConfigInterval / 1000));
            SetItemByKey("HeartbeatInterval", Convert.ToString(HeartbeatInterval / 1000));
            SetItemByKey("ExecuteFailureCount", Convert.ToString(ExecuteFailureCount));
            SetItemByKey("NetServerThreadPool_Min", Convert.ToString(NetServerThreadPool_Min));
            SetItemByKey("NetServerThreadPool_Max", Convert.ToString(NetServerThreadPool_Max));
            SetItemByKey("ControlCommandThreadPool_Min", Convert.ToString(ControlCommandThreadPool_Min));
            SetItemByKey("ControlCommandThreadPool_Max", Convert.ToString(ControlCommandThreadPool_Max));
            SetItemByKey("AutoCollectionThreadPool_Min", Convert.ToString(AutoCollectionThreadPool_Min));
            SetItemByKey("AutoCollectionThreadPool_Max", Convert.ToString(AutoCollectionThreadPool_Max));
            SetItemByKey("HeartbeatThreadPool_Min", Convert.ToString(HeartbeatThreadPool_Min));
            SetItemByKey("HeartbeatThreadPool_Max", Convert.ToString(HeartbeatThreadPool_Max));
            SetItemByKey("V88ThreadPool_Min", Convert.ToString(V88ThreadPool_Min));
            SetItemByKey("V88ThreadPool_Max", Convert.ToString(V88ThreadPool_Max));
        }

        /// <summary>
        /// 返回毫秒数
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private double GetSecond(string item)
        {
            double seconds = 0;
            try
            {
                seconds = double.Parse(GetItemByKey(item)) * 1000;
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
            }
            return seconds;
        }


        /// <summary>
        /// 根据Key取值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetItemByKey(string key)
        {
            XmlNodeList ItemList = XmlHelper.GetXmlNodeListByXpath(UserProfilePath, @"/Config/UserProfile/Item");

            string value = "";
            foreach (XmlNode item in ItemList)
            {
                XmlElement EL = (XmlElement)item;
                if (EL.GetAttribute("key") == key)
                {
                    value = EL.GetAttribute("value");
                }
            }
            return value;
        }

        //根据key修改value的值
        private void SetItemByKey(string key, string value)
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(UserProfilePath); //加载XML文档
                XmlNodeList ItemList = xmlDoc.SelectNodes(@"/Config/UserProfile/Item");

                bool Exist = false;
                foreach (XmlNode item in ItemList)
                {
                    XmlElement EL = (XmlElement)item;
                    if (EL.GetAttribute("key") == key)
                    {
                        Exist = true;
                        if (EL.GetAttribute("value") != value)
                        {
                            EL.SetAttribute("value", value);
                        }

                    }
                }
                //不存在   则添加
                if (Exist == false)
                {
                    //代码以后再写
                }


                xmlDoc.Save(UserProfilePath); //保存到XML文档
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
                // MessageBox.Show("保存配置异常");
            }


        }
    }
}
