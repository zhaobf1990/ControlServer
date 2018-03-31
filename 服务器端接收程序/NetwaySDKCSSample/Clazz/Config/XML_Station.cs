using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using 服务器端接收程序.Config;

namespace 服务器端接收程序.Clazz.Config
{
    public class XML_Station
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Unique { get; set; }    
        /// <summary>
        /// 站点ID
        /// </summary>
        public int StationId { get; set; }
        /// <summary>
        /// 站点名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// OrgId
        /// </summary>
        public string OrgId { get; set; }
        public Clazz.Config.XML_Org Org
        {
            get
            {
                return SysConfig.orgConfig.GetOrgByOrgId(OrgId);
            }
        }
        public string Tel { get; set; }
        public List<Clazz.Config.XML_Test> ListTest
        {
            get
            {
                return SysConfig.DTU_StationConfig.AllTests.Where(c => c.StationUnique == Unique).ToList();
                //return SysConfig.DTU_StationConfig.GetTestsByStationUnique(Unique);
            }
        }
        public string Protocol { get; set; }
    }
}
