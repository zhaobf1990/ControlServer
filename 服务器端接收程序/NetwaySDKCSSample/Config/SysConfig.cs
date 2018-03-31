using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using 服务器端接收程序.Clazz.Config.GuangDai;
using 服务器端接收程序.Util;
using Utility;


namespace 服务器端接收程序.Config
{
    /// <summary>
    /// 读取与保存系统所有配置文件的类
    /// </summary>
    public class SysConfig
    {
        /// <summary>
        /// 配置文件的路径
        /// </summary>
        private static readonly string SysConfigPath = AppDomain.CurrentDomain.BaseDirectory + "../../Config/SysConfig.xml";
        private static readonly string ClientConfigPath = AppDomain.CurrentDomain.BaseDirectory + "../../Config/ClientConfig.xml";

        public static UserProfile userProfile = new UserProfile(SysConfigPath);
        public static OrgConfig orgConfig = new OrgConfig(SysConfigPath);
        public static DTU_StationConfig DTU_StationConfig = new DTU_StationConfig(SysConfigPath);
        public static DBCJConfig DBCJconfig = new DBCJConfig(SysConfigPath);
        public static ClientConfig1 clientConfig = new ClientConfig1(ClientConfigPath);
        public static GD_Config GD_Config = new GD_Config();

        /// <summary>
        /// 读取配置文件数据
        /// </summary>
        public static void ReadConfig()
        {
            DateTime start = DateTime.Now;
            userProfile.ReadConfig();
            orgConfig.ReadConfig();
            DTU_StationConfig.ReadConfig();

            DBCJconfig.ReadConfig();
            clientConfig.ReadConfig();
            GD_Config.GenerateConfig();
           
            V88CommunicationThread.getInstance().readConfig();
            DateTime end = DateTime.Now;
            LogMg.AddInfo(DateUtil.datediff(start, end));
        }

        /// <summary>
        /// 保存配置到XML文件
        /// </summary>
        public static void SaveConfig()
        {
            userProfile.SaveConfig();
            orgConfig.SaveConfig();
            DTU_StationConfig.SaveConfig();
            DBCJconfig.SaveConfig();
            clientConfig.SaveConfig();
        }
    }
}