using System;
using System.Collections.Generic;
using System.Text;

namespace Homewell.SDK
{
    /// <summary>
    /// 对应UDC_NW_Length_Define.h
    /// </summary>
    public static partial class UDCConstDefine
    {
        #region 数据中心公共
        public const int UDC_VERSION_LEN	        = 16; // 协议栈版本长度
        public const int UDC_UNIT_NAME_LEN			= 16; // 单元名称长度
        public const int UDC_NAME_LEN				= 64; // 用户名长度
        public const int UDC_IP_LEN					= 256; // IP长度
        public const int UDC_CIPHERTEXT_LEN			= 64; // 密文长度
        public const int UDC_SESSIONKEY_LEN			= 16; // 会话密钥长度
        public const int UDC_DEV_TYPE_LEN			= 32; // 设备类型长度
        public const int UDC_SHORT_NODEID_LEN       = 16; // 短节点ID长度

        public const int UDC_SENSOR_NUM				= 6; // 传感器个数
        public const int UDC_NW_NODELIST_NUM	    = 2000; // 节点列表最大个数
        public const int UDC_NW_BWLST_NUM			= 2000; // 黑白名单列表最大个数
        public const int UDC_NW_SUBSCRB_NUM			= 400; // 批量订阅/取消节点列表最大个数
        public const int UDC_WSN_MAX_PARENT_NUM     = 4; // 每个节点最多父节点个数
        public const int UDC_NW_CACHED_ALARM_NUM    = 400; // 缓存最大报警个数
        public const int UDC_NW_LOG_NUM				= 100; // 日志查询，一次最大返回个数
        #endregion 

        #region 网由应用层协议NWP
        public const int UDC_NW_USER_LEN			= 32; // 用户名长度
        public const int UDC_NW_PSW_LEN				= 64; // 用户密码长度
        public const int UDC_NW_CIPHERTEXT_LEN		= 64; // 密文长度
        public const int UDC_NW_SESSIONKEY_LEN		= 16; // 会话密钥长度
        public const int UDC_NW_NWID_LEN			= 48; // 网由ID长度
        public const int UDC_DEV_ID_LEN				= 16; // 设备编码长度
        public const int UDC_DEV_MANUFACTORY_LEN	= 64; // 厂家名长度
        public const int UDC_DEV_MODEL_LEN			= 64; // 设备型号名长度
        public const int UDC_NW_NW_FACTORY_ID_LEN	= 16; // 网由出厂ID长度
        public const int UDC_NW_NW_BAND_LEN			= 16; // 频段信息长度
        public const int UDC_NW_NW_FW_LEN			= 16; // 固件版本信息长度
        public const int UDC_NW_NW_POS_LEN			= 32; // 位置信息长度
        public const int UDC_NW_NW_GPS_LEN			= 32; // GPS座标信息长度
        public const int UDC_NW_NODEID_LEN			= 48; // 节点ID长度
        public const int UDC_NW_NODE_MODEL_LEN		= 64; // 节点型号长度
        public const int UDC_NW_NODE_USERNAME_LEN	= 16; // 节点登录用户名长度
        public const int UDC_NW_NODE_PASSWORD_LEN	= 16; // 节点登录密码长度
        public const int UDC_NW_MEMO_LEN			= 64; // 描述长度
        public const int UDC_NW_STRMSG_LEN			= 256; // 字符参数长度
        public const int UDC_PT_USER_LEN			= 16; // 平台用户名长度
        public const int UDC_PT_PSW_LEN				= 16; // 平台密码长度
        public const int UDC_NW_CMD_DATA_LEN		= 1024*4; // 命令数据长度
        public const int UDC_NW_CFG_DATA_LEN		= 1024*4; // 配置项数据长度
        public const int UDC_NW_REBOOT_REASON_LEN	= 32; // 重启网由原因长度
        public const int UDC_NW_CFG_NAME_LEN		= 32; // 配置项名称长度
        public const int UDC_NW_CFG_FILE_LEN		= 1024*9; // 配置文件长度
        public const int UDC_NW_UPGRADE_LISTURL_LEN	= 1024; // 升级列表文件URL及文件名长度
        public const int UDC_NW_VERSION_LEN			= 32;   // 版本名长度
        public const int UDC_NW_HIGHPRINODE_DATA_LEN	= 1024*16; // 查询高优先级节点表文件长度
        public const int UDC_NW_LOG_TYPE_LEN			= 16; 
        public const int UDC_NW_LOG_MSG_LEN			= 1024;   // 日志内容长度
        public const int UDC_NW_TOPOLOGY_LEN		= 1024*9; // 拓扑信息长度
        #endregion

    }
}
