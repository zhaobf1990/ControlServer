using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Utility;
using 服务器端接收程序.Clazz.Config;

namespace 服务器端接收程序.Config
{
    public class DBCJConfig
    {
        private string Path;

        public List<XML_DBCJTest> ListTest { get; set; }

        public DBCJConfig(string path)
        {
            Path = path;

            ListTest = new List<XML_DBCJTest>();
        }

        private List<Clazz.Config.XML_DBCJTest> GetAllTestList()
        {
            List<Clazz.Config.XML_DBCJTest> ListTest = new List<XML_DBCJTest>();
            try
            {
                XmlNodeList listTest = Utility.XmlHelper.GetXmlNodeListByXpath(Path, "/Config/DBCJ/Test");
                foreach (XmlNode item in listTest)
                {
                    XmlElement xel = (XmlElement)item;
                    XML_DBCJTest test = new XML_DBCJTest();
                    test.OrgId = xel.GetAttribute("OrgId");
                    test.TestId = xel.GetAttribute("TestId");
                    test.NodeId = xel.GetAttribute("NodeId");
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

        /// <summary>
        /// 读取配置文件数据
        /// </summary>
        public void ReadConfig()
        {
            lock (ListTest)
            {
                ListTest = GetAllTestList();
            }
           
        }

        /// <summary>
        /// 保存配置到XML文件
        /// </summary>
        public void SaveConfig() {
            //貌似这个类暂时不需要这个保存方法    先把方法写着    反正没有实现
        }

    }
}
