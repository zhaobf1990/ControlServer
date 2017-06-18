using System;
using System.Collections.Generic;
using System.Text;

namespace Homewell.SDK
{
    /// <summary>
    /// 对应UDC_NW_VALUE_DEFINE.h
    /// </summary>
    public static partial class UDCConstDefine
    {
        /* 加密算法类型 */
        public const int UDC_NW_ENCRYPT_UNSUPPORT	= 0; // 不支持的加密方式
        public const int UDC_NW_ENCRYPT_AES64		= 1; // 64位AES
        public const int UDC_NW_ENCRYPT_AES128		= 2; // 128位AES
        public const int UDC_NW_ENCRYPT_TEA16		= 3; // 16轮TEA
        public const int UDC_NW_ENCRYPT_NO			= 4; // 明文, 不加密
        public const int UDC_NW_ENCRYPT_RSV_GOV		= 5; // 预留的政府加密算法
        public const int UDC_NW_ENCRYPT_RSV_ARMY	= 6; // 预留的军方加密算法

        public const string UDC_NW_ENCRYPT_STR_AES64	= "AES-64";  // 64位AES
        public const string UDC_NW_ENCRYPT_STR_AES128	= "AES-128"; // 128位AES
        public const string UDC_NW_ENCRYPT_STR_TEA16	= "TEA-16";  // 16轮TEA
        public const string UDC_NW_ENCRYPT_STR_NO		= "NONE";    // 明文, 不加密


        /* 3G支持类型 */
        public const int UDC_NW_3G_NO				= 1; // 不支持
        public const int UDC_NW_3G_TD_CDMA			= 2; // TD-CDMA
        public const int UDC_NW_3G_WCDMA			= 3; // WCDMA
        public const int UDC_NW_3G_CDMA2000			= 4; // CDMA2000

        public const string UDC_NW_3G_STR_NO			= "NONE";     // 不支持
        public const string UDC_NW_3G_STR_TD_CDMA		= "TD-SCDMA"; // TD-SCDMA
        public const string UDC_NW_3G_STR_WCDMA			= "WCDMA";    // WCDMA
        public const string UDC_NW_3G_STR_CDMA2000		= "CDMA2000"; // CDMA2000

        /* 网由状态 */
        public const int UDC_NW_STATUS_ONLINE		= 1; // 在线
        public const int UDC_NW_STATUS_OFFLINE		= 2; // 离线

        /* 网由信息存储类型 */
        public const int UDC_NW_STORAGE_NO			= 1; // 无
        public const int UDC_NW_STORAGE_SD			= 2; // SD存储
        public const int UDC_NW_STORAGE_HD			= 3; // Hdisk存储

        public const string UDC_NW_STORAGE_STR_NO		= "NONE"; // 无
        public const string UDC_NW_STORAGE_STR_SD		= "SD";   // SD存储
        public const string UDC_NW_STORAGE_STR_HD		= "HD";   // Hdisk存储

        /* 网由配置文件类型 */
        public const int UDC_NW_CONFIG_ALL			= 1; // 所有
        public const int UDC_NW_CONFIG_NETWORK		= 2; // 网络
        public const int UDC_NW_CONFIG_WSN			= 3; // WSN
        public const int UDC_NW_CONFIG_GENERAL		= 4; // 通用

        public const string UDC_NW_CONFIG_STR_ALL			= "All";     // 所有
        public const string UDC_NW_CONFIG_STR_NETWORK		= "Network"; // 网络
        public const string UDC_NW_CONFIG_STR_WSN			= "WSN";     // WSN
        public const string UDC_NW_CONFIG_STR_GENERAL		= "General"; // 通用

        /* 网由通用命令类型 */
        public const int UDC_NW_COMMAND_WIFI			= 1	; //wifi命令
        public const string UDC_NW_COMMAND_STR_WIFI		= "Wifi"; //wifi命令

        /* 获取节点列表类型 */
        public const int UDC_NW_GETNODELIST_BLACK	= 1; // 获取在黑名单中的节点
        public const int UDC_NW_GETNODELIST_WHITE	= 2; // 获取在白名单中的节点
        public const int UDC_NW_GETNODELIST_UNADD	= 3; // 获取未添加节点

        public const string UDC_NW_GETNODELIST_STR_BLACK	= "inBlackList"; // 获取在黑名单中的节点
        public const string UDC_NW_GETNODELIST_STR_WHITE	= "inWhiteList"; // 获取在白名单中的节点
        public const string UDC_NW_GETNODELIST_STR_UNADD	= "un-added";    // 获取未添加节点

        /* 节点类型 */
        public const int UDC_NW_NODETYPE_NORMAL_OPEN	= 1; // 常开类
        public const int UDC_NW_NODETYPE_NORMAL_CLOSE	= 2; // 常闭类
        public const int UDC_NW_NODETYPE_SMOKESENSE		= 3; // 烟感

        /* 节点状态 */
        public const int UDC_NODE_STATUS_ONLINE		    = 0x00000001; // 在线
        public const int UDC_NODE_STATUS_PASSEDBY	    = 0x00000002; // 被旁路
        public const int UDC_NODE_STATUS_SENSOR_CHANGED	= 0x00000004; // 传感器类型发生变化

        /* 节点协议类型 */
        public const int UDC_NODE_PROTOCOL_JH1		= 1; // JH一代节点协议
        public const int UDC_NODE_PROTOCOL_WSN		= 2; // 国家WSN标准协议

        public const string UDC_NODE_PROTOCOL_STR_JH1	= "Homewell-V1";          // JH一代节点协议
        public const string UDC_NODE_PROTOCOL_STR_WSN	= "WSN-NationalStandard"; // 国家WSN标准协议

        /* 节点连接类型 */
        public const int UDC_NODE_CONN_MODE_PERSISTENT	= 1; // 持续组网，类似TCP
        public const int UDC_NODE_CONN_MODE_TEMPORARY	= 2; // 数据包式，类似UDP

        public const string UDC_NODE_CONN_MODE_STR_PERSISTENT	= "Persistent"; // 持续组网，类似TCP
        public const string UDC_NODE_CONN_MODE_STR_TEMPORARY	= "Temporary";  // 数据包式，类似UDP

        /* 节点工作模式 */
        public const int UDC_NODE_RUNMODE_NETWORK	= 1; // 组网
        public const int UDC_NODE_RUNMODE_P2P		= 2; // 点对点

        public const string UDC_NODE_RUNMODE_STR_NETWORK	= "Networking"; // 组网
        public const string UDC_NODE_RUNMODE_STR_P2P		= "P2P";        // 点对点

        /* 节点挂起类型 */
        public const int UDC_NODE_SUSPEND_TEMP_ON_NW	= 1; // 网由临时禁止
        public const int UDC_NODE_SUSPEND_NODE_SELF		= 2; // 节点本身挂起

        public const string UDC_NODE_SUSPEND_STR_TEMP_ON_NW	= "Netway"; // 网由临时禁止
        public const string UDC_NODE_SUSPEND_STR_NODE_SELF	= "Node";   // 节点本身挂起

        /* 节点恢复类型 */
        public const int UDC_NODE_RESUME_NW			= 1; // 恢复网由临时禁止
        public const int UDC_NODE_RESUME_SELF		= 2; // 恢复节点本身挂起

        /* 节点优先级 */
        public const int UDC_NW_NODEPRIORITY_NORMAL	= 1; // 正常
        public const int UDC_NW_NODEPRIORITY_HIGH	= 2; // 高优先级

        public const string UDC_NW_NODEPRIORITY_STR_NORMAL	= "Normal"; // 正常
        public const string UDC_NW_NODEPRIORITY_STR_HIGH	= "High";   // 高优先级

        /* 传感器类型 */
        public const int UDC_SENSOR_UNDEF			= 0xFFFF; // 未定义
        public const int UDC_SENSOR_TEMPERATURE		= 1; // 温度
        public const int UDC_SENSOR_HUMIDITY		= 2; // 湿度
        public const int UDC_SENSOR_WINDSPEED		= 3; // 风速
        public const int UDC_SENSOR_WIND_DIRECTION	= 4; // 风向
        public const int UDC_SENSOR_CO2				= 5; // 二氧化碳
        public const int UDC_SENSOR_LIGHT			= 6; // 光照
        public const int UDC_SENSOR_DOOR			= 7; // 门禁
        public const int UDC_SENSOR_WATER_LOGGING	= 8; // 水浸
        public const int UDC_SENSOR_ALARM_BUTTON	= 9; // 报警按钮

        /* 传感器数据类型 */
        public const int UDC_SENSOR_DATATYPE_UNDEF	= 0; // 未使用
        public const int UDC_SENSOR_DATATYPE_BOOL	= 1; // 开关
        public const int UDC_SENSOR_DATATYPE_NUMBER	= 2; // 数值
        public const int UDC_SENSOR_DATATYPE_FLOAT	= 3; // 浮点
        public const int UDC_SENSOR_DATATYPE_DOUBLE	= 4; // 双精度

        /* 传感器编号 */
        public const int UDC_SENSORALL	= -1; // 所有传感器
        public const int UDC_SENSOR1	=  0; // 传感器1
        public const int UDC_SENSOR2	=  1; // 传感器2
        public const int UDC_SENSOR3	=  2; // 传感器3
        public const int UDC_SENSOR4	=  3; // 传感器4
        public const int UDC_SENSOR5	=  4; // 传感器5
        public const int UDC_SENSOR6	=  5; // 传感器6

        /* 黑白名单类型 */
        public const int UDC_BLACKLIST	= 1; // 黑名单
        public const int UDC_WHITELIST	= 2; // 白名单

        public const string UDC_BLACKLIST_STR	= "black"; // 黑名单
        public const string UDC_WHITELIST_STR	= "white"; // 白名单

        /* 黑白名单操作类型 */
        public const int UDC_SETBLACKWHITE_ON		= 1; // 启用
        public const int UDC_SETBLACKWHITE_OFF		= 2; // 关闭

        public const string UDC_SETBLACKWHITE_STR_ON	= "on";  // 启用
        public const string UDC_SETBLACKWHITE_STR_OFF	= "off"; // 关闭

        /* 黑白名单节点操作类型 */
        public const int UDC_BLACKWHITE_ADD			= 1; // 添加
        public const int UDC_BLACKWHITE_DEL			= 2; // 删除

        public const string UDC_BLACKWHITE_STR_ADD		= "add";    // 添加
        public const string UDC_BLACKWHITE_STR_DEL		= "delete"; // 删除

        /* 向节点下发控制命令类型 */
        public const int UDC_COMMAND_ONOFF			= 1; // 开关
        public const int UDC_COMMAND_SETRANG		= 2; // 设置阀值
        public const int UDC_COMMAND_SETINTERVAL	= 3; // 设置数据发送间隔
        public const int UDC_COMMAND_VALUE_CTRL		= 4; // 数值开关

        /* 通告消息类型 */
        public const int UDC_MSGTYPE_NODE_STATUS    = 1; // 节点状态
        public const int UDC_MSGTYPE_STOREFULL		= 2; // 存储卡满
        public const int UDC_MSGTYPE_STOREEXP		= 3; // 存储卡异常
        public const int UDC_MSGTYPE_WSNEXP			= 4; // WSN主节点模块异常
        public const int UDC_MSGTYPE_WSNRECOVER		= 5; // WSN主节点模块恢复 
        public const int UDC_MSGTYPE_HIGHTEMP		= 6; // 温度过高 
        public const int UDC_MSGTYPE_UPGRADEFINISH	= 7; // 升级完成
        public const int UDC_MSGTYPE_UPGRADEFAILED	= 8; // 升级失败 
        public const int UDC_MSGTYPE_NODEGROUPINFO	= 9; // 节点组网信息 

        /* 网由主动模式 */
        public const int UDC_NW_AUTO_MODEL_ON		= 1; // 启用
        public const int UDC_NW_AUTO_MODEL_OFF		= 2; // 关闭

        /* 日志类型 */
        public const int UDC_NW_LOG_ALL				= 1; // 全部
        public const int UDC_NW_LOG_SECURITY		= 2; // 安全
        public const int UDC_NW_LOG_DEVICE			= 3; // 设备
        public const int UDC_NW_LOG_SYSTEM			= 4; // 系统
        public const int UDC_NW_LOG_BUSINESS		= 5; // 业务

        public const string UDC_NW_LOG_STR_ALL				= "All";      // 全部
        public const string UDC_NW_LOG_STR_SECURITY			= "Security"; // 安全
        public const string UDC_NW_LOG_STR_DEVICE			= "Device";   // 设备
        public const string UDC_NW_LOG_STR_SYSTEM			= "System";   // 系统
        public const string UDC_NW_LOG_STR_BUSINESS			= "Business"; // 业务

        /* 压缩算法类型 */
        public const int UDC_NW_COMPRESS_NO			= 1; // 不压缩
        public const int UDC_NW_COMPRESS_ZIP		= 2; // ZIP压缩

        /* 升级服务器类型 */
        public const int UDC_NW_UPGRADE_SERVER_HTTP	= 1; // HTTP
        public const int UDC_NW_UPGRADE_SERVER_FTP	= 2; // FTP

        public const string UDC_NW_UPGRADE_SERVER_STR_HTTP	= "HTTP"; // HTTP
        public const string UDC_NW_UPGRADE_SERVER_STR_FTP	= "FTP"; // FTP

        /* 自动升级操作 */
        public const int UDC_NW_AUTO_UPGRADE_ON		= 1; // 开启
        public const int UDC_NW_AUTO_UPGRADE_OFF	= 2; // 关闭

        public const string UDC_NW_AUTO_UPGRADE_STR_ON		= "enable";  // 开启
        public const string UDC_NW_AUTO_UPGRADE_STR_OFF		= "disable"; // 关闭

        /* 自动升级类型 */
        public const int UDC_NW_AUTO_UPGRADE_STARTUP		= 1; // 每次开机检查
        public const int UDC_NW_AUTO_UPGRADE_FIXEDTIME	    = 2; // 固定时间检查

        public const string UDC_NW_AUTO_UPGRADE_STR_STARTUP		= "boot-time";  // 每次开机检查
        public const string UDC_NW_AUTO_UPGRADE_STR_FIXEDTIME	= "fixed-time"; // 固定时间检查

        /* 星期 */
        public const int UDC_WEEK_SUN				= 0; //周日
        public const int UDC_WEEK_MON				= 1; //周一
        public const int UDC_WEEK_TUES				= 2; //周二
        public const int UDC_WEEK_WED				= 3; //周三
        public const int UDC_WEEK_THURS				= 4; //周四
        public const int UDC_WEEK_FRI				= 5; //周五
        public const int UDC_WEEK_SAT				= 6; //周六

        public const string UDC_WEEK_STR_SUN			= "Sunday";    //周日
        public const string UDC_WEEK_STR_MON			= "Monday";    //周一
        public const string UDC_WEEK_STR_TUES			= "Tuesday";   //周二
        public const string UDC_WEEK_STR_WED			= "Wednesday"; //周三
        public const string UDC_WEEK_STR_THURS			= "Thursday";  //周四
        public const string UDC_WEEK_STR_FRI			= "Friday";    //周五
        public const string UDC_WEEK_STR_SAT			= "Saturday";  //周六

        /* 节点数据类型 */
        public const int UDC_NW_NODE_DATA_ALL		= 0; // 所有数据
        public const int UDC_NW_NODE_DATA_REAL		= 1; // 实时数据
        public const int UDC_NW_NODE_DATA_ALARM		= 2; // 报警数据

        public const string UDC_NW_NODE_DATA_STR_ALL	= "All";      // 所有数据
        public const string UDC_NW_NODE_DATA_STR_REAL	= "Realtime"; // 实时数据
        public const string UDC_NW_NODE_DATA_STR_ALARM	= "Alarm";    // 报警数据


        /* Wsn拓扑 */
        public const int UDC_NW_WSNTOPOLOGY_ALL          = 0; //获取所有节点wsn拓扑
        public const int UDC_NW_WSNTOPOLOGY_LEVEL        = 1; //获取当前层次的所有节点
        public const int UDC_NW_WSNTOPOLOGY_NODE         = 2; //获取当前节点的所有拓扑
    }
}
