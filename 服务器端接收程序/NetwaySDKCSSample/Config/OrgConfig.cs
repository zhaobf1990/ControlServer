using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility;
using System.Xml;
using System.IO;

namespace 服务器端接收程序.Config
{
    public class OrgConfig
    {
        //文件路径
        private static string filePath;

        /// <summary>
        /// 文件路径
        /// </summary>
        /// <param name="path"></param>
        public OrgConfig(string path)
        {
            filePath = path;

            Orgs = new List<Clazz.Config.XML_Org>();
        }

        public List<Clazz.Config.XML_Org> Orgs { get; set; }

        /// <summary>
        /// 读取配置文件数据
        /// </summary>
        public void ReadConfig()
        {
            lock (Orgs)
            {
                Orgs = GetAllOrg();
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
        /// 根据orgId返回Org.XML中的对象    如果不存在则返回null
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public Clazz.Config.XML_Org GetOrgByOrgId(string orgId)
        {
            Clazz.Config.XML_Org org = null;
            try
            {
                org = Orgs.SingleOrDefault(c => c.OrgId == orgId);
            }
            catch (Exception e)
            {
                LogMg.AddError(e);
                DEBUG.MsgBox(e.ToString());
            }
            return org;
        }


        /// <summary>
        /// 获取所有的Org
        /// </summary>
        /// <returns></returns>
        private List<Clazz.Config.XML_Org> GetAllOrg()
        {
            List<Clazz.Config.XML_Org> list = new List<Clazz.Config.XML_Org>();
            try
            {
                XmlNodeList xnl = Utility.XmlHelper.GetXmlNodeListByXpath(filePath, "/Config/Orgs/Org");
                foreach (XmlNode item in xnl)
                {
                    XmlElement xe = (XmlElement)item;
                    Clazz.Config.XML_Org org = new Clazz.Config.XML_Org();
                    org.OrgId = xe.GetAttribute("OrgId");
                    org.Name = xe.GetAttribute("Name");
                    org.DBName = xe.GetAttribute("DBName");
                    org.gdServerCfg = xe.GetAttribute("gdServerCfg");
                    list.Add(org);
                }
            }
            catch (Exception e)
            {
                LogMg.AddError(e);
                DEBUG.MsgBox(e.ToString());
            }
            return list;
        }
    }
}
