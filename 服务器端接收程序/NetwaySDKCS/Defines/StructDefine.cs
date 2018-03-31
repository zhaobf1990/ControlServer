using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Homewell.SDK
{
    [StructLayout(LayoutKind.Sequential)]
    public struct JH_Time
    {
        public byte year;                   /*!< 年减去1900  */
        public byte month;                  /*!< 1-12  月 */
        public byte day;                    /*!< 1-31  日 */
        public byte hour;                   /*!< 0-23  时 */
        public byte min;                    /*!< 0-59  分 */
        public byte sec;                    /*!< 0-59  秒 */
        public ushort msec;                 /*!< 0-999毫秒 */
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct SDK_NW_LoginInfo
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UDCConstDefine.UDC_NW_NWID_LEN + 1)]
        public string nwId;                      /*!< 网由ID */
           
	    public int   sessionid;                  /*!< 当前会话ID */
	    public int   nodes;                      /*!< 当前节点数 */
	    public int   sessions;                   /*!< 当前会话数 */
	    public int   enc;						 /*!< 支持的加密方式 */
	    public int   expires;                    /*!< 原先的心跳间隔, 单位是秒 */
	    public JH_Time time;                     /*!< 信息产生时间 */
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SDK_NW_InfoChange
    {
        public int nodesOld;                                 /*!< 原来网由拥有的节点数 */
        public int sessionsOld;                              /*!< 原来网由拥有的会话数 */
        public int nodesNew;                                 /*!< 现在网由拥有的节点数 */
        public int sessionsNew;                              /*!< 现在网由拥有的会话数 */
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct SDK_Register_Info
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UDCConstDefine.UDC_IP_LEN + 1)]
        public string ip;                               /*!< 设备注册ip */

        public int port;                                           /*!< 设备注册端口 */
        
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UDCConstDefine.UDC_DEV_TYPE_LEN + 1)]
        public string type;                       /*!< 设备类型 */

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UDCConstDefine.UDC_NW_NWID_LEN + 1)]
        public string id;                          /*!< 网由ID */

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UDCConstDefine.UDC_DEV_MANUFACTORY_LEN + 1)]
        public string manufactory;         /*!< 网由厂家 */

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UDCConstDefine.UDC_DEV_MODEL_LEN + 1)]
        public string model;                     /*!< 设备型号 */

        public int nodes;                                          /*!< 当前节点数 */
        public int sessions;                                       /*!< 当前会话数 */
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct SDK_NW_Info
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UDCConstDefine.UDC_DEV_MANUFACTORY_LEN + 1)]
        public string manufactory;      /*!< 网由厂家 */

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UDCConstDefine.UDC_DEV_MODEL_LEN + 1)]
        public string model;			/*!< 设备型号 */

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UDCConstDefine.UDC_NW_NWID_LEN + 1)]
        public string id;               /*!< 设备出厂ID */

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UDCConstDefine.UDC_NW_NW_BAND_LEN + 1)]
        public string band;             /*!< 频段 */

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UDCConstDefine.UDC_NW_NW_FW_LEN + 1)]
        public string firmware;         /*!< 固件版本 */

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UDCConstDefine.UDC_NW_NW_POS_LEN + 1)]
        public string position;         /*!< 网由的位置信息 */

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UDCConstDefine.UDC_NW_NW_GPS_LEN + 1)]
        public string GPS;              /*!< 网由的GPS坐标 */

        public int nodecount;				                    /*!< 最大支持节点数 */
        public int support3G;                                   /*!< 3G支持 */
        public int storage;                                     /*!< 存储类型 */
        public uint space;                                      /*!< 存储空间, 单位为M */
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct SDK_Node_Entry
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UDCConstDefine.UDC_NW_NODEID_LEN + 1)]
        public string id;              /*!< 节点ID */

        public uint status;                             /*!< 节点状态 */

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = UDCConstDefine.UDC_SENSOR_NUM)]
        public ushort[] sensorType;			 /*!< 传感器类型 */
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct SDK_Node_List
    {
        public int  totalCount;                                     /*!< 节点总个数 */
	    public int  count;                                          /*!< 节点的个数 */

	    [MarshalAs(UnmanagedType.ByValArray, SizeConst = UDCConstDefine.UDC_NW_NODELIST_NUM)]
        public SDK_Node_Entry[] list;            /*!< 节点信息列表 */

    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct SDK_Node_Info
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UDCConstDefine.UDC_NW_NODEID_LEN + 1)]
        public string nodeid;				     /*!< 节点ID */

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UDCConstDefine.UDC_DEV_MANUFACTORY_LEN + 1)]
        public string manufactory;         /*!< 网由厂家 */

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UDCConstDefine.UDC_DEV_MODEL_LEN + 1)]
        public string model;                     /*!< 节点型号 */

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UDCConstDefine.UDC_NW_NODE_USERNAME_LEN + 1)]
        public string username;           /*!< 节点登录用户名 */

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UDCConstDefine.UDC_NW_NODE_PASSWORD_LEN + 1)]
        public string userpass;           /*!< 节点登录密码 */

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UDCConstDefine.UDC_NW_NW_POS_LEN + 1)]
        public string position;                  /*!< 节点的位置 */

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UDCConstDefine.UDC_NW_MEMO_LEN + 1)]
        public string memo;                        /*!< 节点的描述 */

	    public int  type;											 /*!< 节点类型 */
	    public int  conntype;										 /*!< 连接类型 */
	    public int  protocol;                                       /*!< 协议 */
	    public int  runmodel;                                       /*!< 工作模式 */
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct SDK_Node_Id
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UDCConstDefine.UDC_NW_NODEID_LEN + 1)]
        public string nodeid;                    /*!< 节点ID */
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct SDK_NodeId_List
    {
        public int count;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = UDCConstDefine.UDC_NW_NODELIST_NUM)]
        public SDK_Node_Id[] list;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SDK_BlackWhite_Node_List
    {
	    public int   count;                                          /*!< 节点个数 */

	    [MarshalAs(UnmanagedType.ByValArray, SizeConst = UDCConstDefine.UDC_NW_BWLST_NUM)]
        public SDK_Node_Id[] nodes;                 /*!< 节点ID列表 */
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SDK_Subscrib_Node_List
    {
	    public int   count;                                          /*!< 节点个数 */

	    [MarshalAs(UnmanagedType.ByValArray, SizeConst = UDCConstDefine.UDC_NW_SUBSCRB_NUM)]
        public SDK_Node_Id[] nodes;               /*!< 节点ID列表 */
    }

    #region 传感器数据与传感器缓存数据结构体
    [StructLayout(LayoutKind.Sequential)]
    public struct SDK_Sensor_Data
    {
	    public uint valid;                      /*!< 是否使用 */
	    public uint type;						/*!< 特征码 */
	    public float data;                      /*!< 传感器数据 */
	    public float reserved;                  /*!< 保留的数据位 */
	    public JH_Time time;                    /*!< 采集时间 */
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SDK_Sensor_Cache_Data
    {
        public uint valid;                      /*!< 是否使用 */
        public uint type;						/*!< 特征码 */
        public float data;                      /*!< 传感器数据 */
        public float reserved;                  /*!< 保留的数据位 */
        public JH_Time time;                    /*!< 采集时间 */
    }
    #endregion

    #region 节点实时数据与报警数据结构体
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct SDK_Node_Real_Data
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UDCConstDefine.UDC_NW_NODEID_LEN + 1)]
        public string nodeid; /*!< 节点ID */

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = UDCConstDefine.UDC_SENSOR_NUM)]
        public SDK_Sensor_Data[] sensorData;	     /*!< 传感器数据 */
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct SDK_Node_Alarm_Data
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UDCConstDefine.UDC_NW_NODEID_LEN + 1)]
        public string nodeid; /*!< 节点ID */

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = UDCConstDefine.UDC_SENSOR_NUM)]
        public SDK_Sensor_Data[] sensorData;	     /*!< 传感器数据 */
    }
    #endregion

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct SDK_Node_Cache_Data
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UDCConstDefine.UDC_NW_NODEID_LEN + 1)]
        public string nodeid;					/*!< 节点ID */

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = UDCConstDefine.UDC_SENSOR_NUM)]
        public SDK_Sensor_Cache_Data[] sensorData;	/*!< 传感器数据	 */

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SDK_Sensor_Cache_Alarm_Data
    {
        public uint no;                         /*!< 传感器编号 */
        public uint valid;                      /*!< 是否使用 */
        public uint type;						/*!< 特征码 */
        public float data;                      /*!< 传感器数据 */
        public float reserved;                  /*!< 保留的数据位 */
        public JH_Time time;                    /*!< 采集时间 */
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct SDK_Cached_Alarm_Data
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UDCConstDefine.UDC_NW_NODEID_LEN + 1)]
        public string nodeid;

        public SDK_Sensor_Cache_Alarm_Data sensorData;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SDK_Cached_Alarm_List
    {
        public int count;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = UDCConstDefine.UDC_NW_CACHED_ALARM_NUM)]
        public SDK_Cached_Alarm_Data[] list;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SDK_Type_Data
    {
	    public uint  valid;                    /*!< 是否使用 */
	    public uint  type;					   /*!< 特征码 */
	    public float data;                     /*!< 数据 */
    }

    #region 节点设置数据与命令数据结构体
    [StructLayout(LayoutKind.Sequential)]
    public struct SDK_NodeSetting_Data
    {
        public SDK_Type_Data nodeData;						/*!< 节点数据 */

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = UDCConstDefine.UDC_SENSOR_NUM)]
        public SDK_Type_Data[] sensorData;	/*!< 传感器数据 */

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SDK_Command_Data
    {
        public SDK_Type_Data nodeData;						/*!< 节点数据 */

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = UDCConstDefine.UDC_SENSOR_NUM)]
        public SDK_Type_Data[] sensorData;	/*!< 传感器数据 */
    }
    #endregion

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct SDK_AutoParm 
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UDCConstDefine.UDC_NW_NWID_LEN + 1)]
        public string id;                /*!< 网由ID */

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UDCConstDefine.UDC_IP_LEN + 1)]
        public string ip;                     /*!< 平台的IP地址 */

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UDCConstDefine.UDC_PT_USER_LEN + 1)]
        public string user;              /*!< 平台登录用户名 */

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UDCConstDefine.UDC_PT_PSW_LEN + 1)]
        public string password;           /*!< 平台登录密码 */

	    public int   enable;								/*!< 使能网由主动模式 */
	    public uint  port;							/*!< 平台监听的端口 */
	    public int   interval;								/*!< 重试间隔, 单位为秒 */
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct SDK_Notify_Info 
    {
	    public int     message;                 /*!< 通知信息 */
	    public int     param1;                  /*!< 参数1 */
	    public int     param2;                  /*!< 参数2 */

	    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UDCConstDefine.UDC_NW_STRMSG_LEN + 1)]
        public string strMsg1;    /*!< 字符参数1 */

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UDCConstDefine.UDC_NW_STRMSG_LEN + 1)]
        public string strMsg2;    /*!< 字符参数2 */

        public JH_Time time;                    /*!< 通知发生的时间 */
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct SDK_Upgrade_Info
    {
	    public uint enable;                         /*!< 使能 */
	    public uint type;                           /*!< 类型 */
	    public uint week;                           /*!< 每周几升级 */
        public JH_Time time;                        /*!< 当天的升级时间, 年月日为空 */

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UDCConstDefine.UDC_NW_VERSION_LEN + 1)]
        public string version;  /*!< 指定升级版本 */

    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct SDK_NW_Log
    {
        public JH_Time time;                                /*!< 日志产生时间 */
        public int     typeInt;                             /*!< 日志类型整型值 */

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UDCConstDefine.UDC_NW_LOG_TYPE_LEN+1)]
        public string type;         /*!< 日志类型 */

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UDCConstDefine.UDC_NW_LOG_MSG_LEN+1)]
        public string message;       /*!< 日志内容 */

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SDK_Log_Info
    {
	    public int            totalCount;                /*!< 日志总记录数 */
        public int            count;                     /*!< 日志记录数*/

	    [MarshalAs(UnmanagedType.ByValArray, SizeConst = UDCConstDefine.UDC_NW_LOG_NUM)]
        public SDK_NW_Log[] logList;   /*!< 日志列表 */
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct SDK_HighPriority_Info
    {
        public int      len;                                  /*!< 高优先级列表长度 */

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UDCConstDefine.UDC_NW_HIGHPRINODE_DATA_LEN+1)]
        public string data;  /*!< 高优先级列表内容 */
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Global_Opera_t
    {
        public int subType;
        public IntPtr param1;
        public int len1;
        public IntPtr param2;
        public int len2;
    }
}
