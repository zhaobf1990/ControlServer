using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 服务器端接收程序.Clazz.Config.GuangDai
{
    public class GD_Station
    {
        public int Unique { get; set; }
        public String name { get; set; }
        public String wscId { get; set; }
        public int stationId { get; set; }
        public String dbName { get; set; }
        public string OrgId { get; set; }
        public List<XML_Test> tests { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is GD_Station)
            {
                GD_Station person = obj as GD_Station;
                return person.dbName == this.dbName && person.wscId == this.wscId && person.stationId == this.stationId;
            }
            else
                return false; ;
        }
        public override int GetHashCode()
        {
            return (this.dbName + "_" + this.wscId + "_" + this.stationId).GetHashCode();
        }

    }
}
