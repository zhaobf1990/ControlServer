using System;
using System.Collections.Generic;
using System.Text;

namespace Homewell.SDK
{
    public enum SDKErrorCode
    {
        SDK_ERROR_SUCCESS                         = 0,     /*!< 成功 */
        SDK_ERROR_USER_PASSWORD_INVALID          = -1,     /*!< 错误的密码 */
        SDK_ERROR_USER_INVALID_STATUS            = -2,     /*!< 用户会话状态异常 */
        SDK_ERROR_USER_NOT_EXIST                 = -3,     /*!< 用户不存在 */
        SDK_ERROR_USER_NOT_LOGIN                 = -4,     /*!< 用户未登录 */
        SDK_ERROR_UNSUPPORT_SUBSCRIBE_TYPE       = -5,     /*!< 不支持的订阅类型 */
        SDK_ERROR_INVALID_TIME_VALUE             = -6,     /*!< client给定的时间值, 超时值非法 */
        SDK_ERROR_UNSUPPORT_NODELIST_GET_TYPE    = -7,     /*!< 不支持的nodelist获取类型 */
        SDK_ERROR_DECRYPT_FAILED                 = -8,     /*!< 解密失败 */
        SDK_ERROR_UNSUPPORT_ENCRYPT_TYPE         = -9,     /*!< 不支持的加密方式 */
        SDK_ERROR_NODE_UNSUPPORT_COMMAND         = -10,    /*!< 不支持的节点操作命令 */
        SDK_ERROR_INVALID_SENSOR_NO              = -11,    /*!< 无效的传感器编号  */
        SDK_ERROR_INVALID_NODE_ID                = -12,    /*!< 无效的节点ID */
        SDK_ERROR_NONE_THRESHOLD_DATA            = -13,    /*!< 没有阀值数据 */
        SDK_ERROR_USER_ALREADY_EXIST             = -14,    /*!< 用户已经存在 */
        SDK_ERROR_WRONG_OLD_PASSWORD             = -15,    /*!< 错误的旧密码 */
        SDK_ERROR_USER_ALREADY_LOGOUT            = -16,    /*!< 用户已经logout */
        SDK_ERROR_INVALID_PROTOCOL_VERSION       = -17,    /*!< 协议栈版本不兼容 */
        SDK_ERROR_INVALID_BW_CONFIG_TYPE         = -18,    /*!< 不支持的黑白名单配置类型 */
        SDK_ERROR_INVALID_BW_CONFIG_COMMAND      = -19,    /*!< 不支持的黑白名单配置操作类型 */
        SDK_ERROR_USER_DELETE_FORBIDDEN          = -20,    /*!< 禁止删除该用户 */
        SDK_ERROR_UNSUPPORT_CONFIG_NAME          = -21,    /*!< 不支持的配置名称 */
        SDK_ERROR_UNSUPPORT_COMMON_CMD           = -22,    /*!< 不支持的通用命令 */
        SDK_ERROR_USERNAME_NULL                  = -23,    /*!< 用户名为空 */
        SDK_ERROR_SENSOR_NOT_EXIST               = -24,    /*!< 请求的传感器不存在 */  
        SDK_ERROR_INVALID_SEND_INTERVAL          = -25,    /*!< 发送间隔无效 */
        SDK_ERROR_ZERO_SEND_INTERVAL             = -26,    /*!< 发送间隔不能为0 */
        SDK_ERROR_INVALID_INPUT_PARAM            = -27,    /*!< 传入参数错误 */
        SDK_ERROR_SEND_INTERVAL_TOO_SHORT        = -28,    /*!< 设置的发送间隔时间太短 */
        SDK_ERROR_SEND_INTERVAL_TOO_LONG         = -29,    /*!< 设置的发送间隔时间太长 */
        SDK_ERROR_INVALID_NW_ID	                 = -30,    /*!< 错误的网由ID */
        SDK_ERROR_SESSION_TIMEOUT                = -31,    /*!< 会话超时*/
        SDK_ERROR_SESSION_NOT_EXIST              = -32,    /*!< 会话不存在*/
        SDK_ERROR_NODE_NOT_REGISTER              = -33,    /*!< 节点未注册，说明节点从来没有注册成功过*/

        SDK_ERROR_OPERA_TIMEOUT                  = -70,    /*!< 操作超时 */
        SDK_ERROR_INVALID_PARAM                  = -71,    /*!< 错误的参数 */
        SDK_ERROR_CONNECT_FAILED                 = -72,    /*!< 连接网由失败 */
        SDK_ERROR_NETWAY_IS_LOGINED              = -73,    /*!< 设备已经登录 */
        SDK_ERROR_NETWAY_IS_OFFLINE              = -74,    /*!< 设备离线, 设备未登录 */
        SDK_ERROR_LISTEN_FAILED                  = -75,    /*!< 监听端口失败 */
        SDK_ERROR_LISTEN_EXISTED                 = -76,    /*!< 已有端口监听 */
        SDK_ERROR_WORKMODE_ERROR                 = -77,    /*!< 工作模式错误, 即主动登录设备不允许unregister操作 */
        SDK_ERROR_SEND_TIMEOUT                   = -79,    /*!< 发送超时, 本系统内就是没有收到100应答包 */

        SDK_ERROR_TOO_MANY_USER                  = -101,   /*!< 用户连接到达上限 */
        SDK_ERROR_KICKOUT                        = -102,   /*!< 用户被踢出 */
        SDK_ERROR_USER_DELETED                   = -103,   /*!< 用户帐号被删除 */
        SDK_ERROR_MSG_SERVICE_ERROR              = -104,   /*!< 网由内部消息服务错误 */
        SDK_ERROR_WSN_SERVICE_UNAVAILABLE        = -105,   /*!< WSN服务不可用 */
        SDK_ERROR_WSN_SERVICE_ERROR              = -106,   /*!< WSN服务执行失败 */
        SDK_ERROR_DB_SERVICE_UNAVAILABLE         = -107,   /*!< DB服务不可用 */
        SDK_ERROR_DB_SERVICE_ERROR               = -108,   /*!< DB服务执行失败 */
        SDK_ERROR_USER_SERVICE_UNAVAILABLE       = -109,   /*!< User服务不可用 */
        SDK_ERROR_USER_SERVICE_ERROR             = -110,   /*!< User服务错误 */
        SDK_ERROR_UNSUPPORT_FUNCTION             = -111,   /*!< 请求的功能不支持 */
        SDK_ERROR_WSN_NET_TIMEOUT                = -112,   /*!< WSN网络超时 */
        SDK_ERROR_WSN_NODE_SENSOR_NOT_READY      = -113,   /*!< 节点传感器信息没有就位 */
        SDK_ERROR_SET_COMMAND_IN_PROCESSSING     = -114,   /*!< 节点设置命令在执行中 */
        SDK_ERROR_ONLY_SUPPORT_SET_ONE_THRESHOLD = -115,   /*!< 一个请求只能设置一个节点的一个传感器的一种阀值 */
        SDK_ERROR_WSN_NET_OPERATION_FAILED       = -116,   /*!< WSN 网络层操作失败 */
        SDK_ERROR_LOG_XML_ERROR                  = -117,   /*!< Log Xml操作错误 */
        SDK_ERROR_XML_OPERATION_ERROR            = -118,   /*!< Vway中xml操作错误 */
        SDK_ERROR_SERVER_LOGIC_ERROR             = -119,   /*!< Server内部逻辑错误 */
        SDK_ERROR_NODE_OPERATION                 = -120,   /*!< 不支持的节点操作 */
        SDK_ERROR_WAIT_OTHER_MAIN_OPERATION      = -121,   /*!< 其他主节点操作正在进行中 */
        SDK_ERROR_MAIN_NODE_FAILED               = -122,   /*!< 主节点交互失败 */
        SDK_ERROR_MAIN_NODE_SET_FAILED           = -123,   /*!< 主节点设置失败 */
        SDK_ERROR_AUTOREG_UNREGISTERING          = -124,   /*!< 正在取消到平台的主动注册*/ 
        SDK_ERROR_IPC_SERVICE_ERROR              = -125,   /*!< IPC模块逻辑错误*/
        SDK_ERROR_WAIT_WIFI_CONNECTING	         = -126,   /*!< 等待wifi连接中*/

        SDK_ERROR_BAD_RESPONSE                   = -193,   /*!< 错误的回应 */
        SDK_ERROR_UNCOMPRESS_FAILED              = -194,   /*!< 解压缩失败 */
        SDK_ERROR_TOO_MANY_RESPONSES             = -195,   /*!< SDK内部回应过多,请求用户加快做对应处理 */
        SDK_ERROR_NETWAY_NOT_EXIST               = -196,   /*!< 该网由不存在 */
        SDK_ERROR_INVALID_HANDLE                 = -197,   /*!< 无效的网由句柄 */
        SDK_ERROR_TOO_MANY_COMMANDS              = -198,   /*!< SDK内部命令过多 */
        SDK_ERROR_INTERNAL_ERROR                 = -199,   /*!< SDK内部错误  */
        SDK_ERROR_UNKNOWN_FAILED                 = -200,   /*!< 未知的失败类型 */
    }

    public enum SDKOperation
    {
        JH_SYSTEM_OPERATE_BASE                    = 0, 
        JH_SYSTEM_DISCONNECT_OFFLINE              = JH_SYSTEM_OPERATE_BASE+1, /*!< 心跳检测设备断线 */
        JH_SYSTEM_DISCONNECT_UNREGISTER           = JH_SYSTEM_OPERATE_BASE+2, /*!< 设备主动注销 */
        JH_SYSTEM_DISCONNECT_OTHERUSER            = JH_SYSTEM_OPERATE_BASE+3, /*!< 本用户其他地方登录 */
        JH_SYSTEM_DISCONNECT_UNKNOWN              = JH_SYSTEM_OPERATE_BASE+4, /*!< 未知原因断线 */
        JH_SYSTEM_INFOCHANGED                     = JH_SYSTEM_OPERATE_BASE+5, /*!< 设备信息变化  */
        JH_SYSTEM_NETWAY_REGISTER                 = JH_SYSTEM_OPERATE_BASE+7, /*!< 有网由设备主动注册 */
        JH_SYSTEM_LOGIN                           = JH_SYSTEM_OPERATE_BASE+8, /*!< 登录信息操作 */
        JH_SYSTEM_LOGOUT                          = JH_SYSTEM_OPERATE_BASE+9, /*!< 登出信息操作 */
        JH_SYSTEM_RECONNECT_LOGIN                 = JH_SYSTEM_OPERATE_BASE+10, /*!< 断线重连登录操作 */
        JH_SYSTEM_SDK_INTERNAL_STATUS             = JH_SYSTEM_OPERATE_BASE+11, /*!< SDK内部状态通知 */
        JH_SYSTEM_OPERATE_END                     = 100, 
        
        JH_NODE_OPERATE_BASE                      = 101, 
        JH_NODE_ADD                               = JH_NODE_OPERATE_BASE+1, /*!< 增加节点 */
        JH_NODE_DEL                               = JH_NODE_OPERATE_BASE+2, /*!< 删除节点 */
        JH_NODE_SUSPEND                           = JH_NODE_OPERATE_BASE+3, /*!< 挂起节点  */
        JH_NODE_RESUME                            = JH_NODE_OPERATE_BASE+4, /*!< 唤醒节点 */
        JH_NODE_SETBWENABLE                       = JH_NODE_OPERATE_BASE+5, /*!< 设置黑白节点使能 */
        JH_NODE_SETBW                             = JH_NODE_OPERATE_BASE+6, /*!< 对黑白节点进行操作 */
        JH_NODE_SETCOMMANDTONODE                  = JH_NODE_OPERATE_BASE+7, /*!< 向节点下发控制命令 */
        JH_NODE_OPERATE_END                       = 200, 
        
        JH_NW_OPERATE_BASE                        = 201, 
        JH_NW_SETAUTOPARAM                        = JH_NW_OPERATE_BASE+1, /*!< 设置自动模式 */
        JH_NW_TIMESYNCH                           = JH_NW_OPERATE_BASE+2, /*!< 设置时间同步 */
        JH_NW_CHANGEPASSWORD                      = JH_NW_OPERATE_BASE+3, /*!< 修改网由密码 */
        JH_NW_REBOOTDEVICE                        = JH_NW_OPERATE_BASE+4, /*!< 重启设备 */
        JH_NW_SETCONFIG                           = JH_NW_OPERATE_BASE+5, /*!< 网由设置 */
        JH_NW_MANUALUPGRADE                       = JH_NW_OPERATE_BASE+6, /*!< 手动升级 */
        JH_NW_SETUPGRADE                          = JH_NW_OPERATE_BASE+7, /*!< 设置自动升级 */
        JH_NW_SETNODEPRIORITY                     = JH_NW_OPERATE_BASE+8, /*!< 设置节点优先级 */
        JH_NW_IMPORTCONFIGFILE                    = JH_NW_OPERATE_BASE+9, /*!< 导入配置文件 */
        JH_NW_REQUESTREALDATA                     = JH_NW_OPERATE_BASE+10, /*!< 请求实时数据操作 */
        JH_NW_SUBSCRIBEALLNODEDATA                = JH_NW_OPERATE_BASE+11, /*!< 订阅所有节点实时数据 */
        JH_NW_CANCELALLNODEDATA                   = JH_NW_OPERATE_BASE+12, /*!< 取消所有节点实时订阅 */
        JH_NW_BATSUBSCRIBENODEDATA                = JH_NW_OPERATE_BASE+13, /*!< 批量订阅节点数据 */
        JH_NW_BATCANCELNODEDATA                   = JH_NW_OPERATE_BASE+14, /*!< 取消批量订阅节点数据 */
        JH_NW_SETCOMMONCOMMAND                    = JH_NW_OPERATE_BASE+15, /*!< 设置通用命令 */
        JH_NW_OPERATE_END                         = 300, 
        
        JH_SYSTEM_INFO_BASE					      = 301, 
        JH_SYSTEM_INFO_NWINFO                     = JH_SYSTEM_INFO_BASE+1, /*!< 获取网由信息 */
        JH_SYSTEM_INFO_NODELIST                   = JH_SYSTEM_INFO_BASE+2, /*!< 获取节点列表 */
        JH_SYSTEM_INFO_BLACKWHITE                 = JH_SYSTEM_INFO_BASE+3, /*!< 获取黑白节点列表 */
        JH_SYSTEM_INFO_NODECACHEDATA              = JH_SYSTEM_INFO_BASE+4, /*!< 获取节点缓冲数据 */
        JH_SYSTEM_INFO_NODEREALDATA               = JH_SYSTEM_INFO_BASE+5, /*!< 获取节点实时数据 */
        JH_SYSTEM_INFO_NODESETTINGS               = JH_SYSTEM_INFO_BASE+6, /*!< 获取节点设置 */
        JH_SYSTEM_INFO_CONFIG                     = JH_SYSTEM_INFO_BASE+7, /*!< 获取配置 */
        JH_SYSTEM_INFO_CONFIGFILE                 = JH_SYSTEM_INFO_BASE+8, /*!< 获取配置文件 */
        JH_SYSTEM_INFO_QUERYLOG                   = JH_SYSTEM_INFO_BASE+9, /*!< 查询日志 */
        JH_SYSTEM_INFO_PRIORITYLIST	              = JH_SYSTEM_INFO_BASE+10, /*!< 查询优先级列表 */
        JH_SYSTEM_INFO_AUTOPARAM                  = JH_SYSTEM_INFO_BASE+11, /*!< 获取网由主动模式及平台参数 */
        JH_SYSTEM_INFO_TOPOLOGY                   = JH_SYSTEM_INFO_BASE+12, /*!< 获取拓扑信息 */
        JH_SYSTEM_INFO_UPDISCONCACHEDALARM        = JH_SYSTEM_INFO_BASE+13, /*!< 读取网由上行链路断线时缓存的报警数据 */
        JH_SYSTEM_INFO_END                        = 400, 
    }

    public enum NetwayMode
    {
        PASSIVE_MODE = 1,    /*!< 被动模式网由 */
        ACTIVE_MODE  = 2,    /*!< 主动模式网由 */
    }
}
