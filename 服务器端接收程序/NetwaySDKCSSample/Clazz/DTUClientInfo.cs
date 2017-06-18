using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using 服务器端接收程序.Config;
using 服务器端接收程序.Util;

namespace 服务器端接收程序.Clazz
{
    /// <summary>
    /// 客户端连接信息
    /// </summary>
    public class DTUClientInfo
    {
        /// <summary>
        /// 站点id
        /// </summary>
        public int StationId { get; set; }
        /// <summary>
        /// 手机号码 或者叫 gprsid    （通讯设备的id）
        /// </summary>
        public string TelOrGprsId { get; set; }
        /// <summary>
        /// 客户端连接实例
        /// </summary>
        public Socket socket { get; set; }
        /// <summary>
        /// 是否已注册
        /// </summary>
        public bool Register { get; set; }
        /// <summary>
        /// 最后一次访问时间
        /// </summary>
        public DateTime LastVisitTime { get; set; }
        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime RegisterTime { get; set; }
        /// <summary>
        /// 客户端名称
        /// </summary>
        public string Name
        {
            get
            {
                //Clazz.Config.XML_Station station =SysConfig.StationConfig.GetStationByTel(Tel);
                //if (station != null)
                //{
                //    return station.Name;
                //}
                //else
                //    return "未知的客户端";
                return DTU_ClientManager.GetStationNameByTel(TelOrGprsId);
            }
        }
        /// <summary>
        /// 采用的协议
        /// </summary>
        public string Protocol { get; set; }

    

        public override string ToString()
        {
            return string.Format("客户端注册信息 Tel=:{0}   Socket.Connected={1}   RegisterTime={2}   LastVistTime={3}", TelOrGprsId, socket.Connected, RegisterTime.ToString(), LastVisitTime.ToString());
        }
    }
}
