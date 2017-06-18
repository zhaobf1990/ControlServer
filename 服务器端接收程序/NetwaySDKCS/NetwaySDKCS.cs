using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Homewell.SDK
{
    #region 回调函数
    public delegate int GlobalOperaCallback(uint handle, int seq, int opera, int result, ref Global_Opera_t info, IntPtr user, int resverd);

    public delegate int NotifyInfoCallback(uint handle, int seq, ref SDK_Notify_Info info, IntPtr user);

    public delegate int RealDataCallback(uint handle, int seq, ref SDK_Node_Real_Data data, IntPtr user);

    public delegate int AlarmDataCallback(uint handle, int seq, ref SDK_Node_Alarm_Data data, IntPtr user);
    #endregion

    public class NetwaySDK
    {
        #region 回调绑定函数
        [DllImport("NetwaySDK.dll")]
        public static extern int NW_SetGlobalOperaCallBack(GlobalOperaCallback cb, IntPtr user);

        [DllImport("NetwaySDK.dll")]
        public static extern int NW_SetNotifyCallBack(NotifyInfoCallback cb, IntPtr user);

        [DllImport("NetwaySDK.dll")]
        public static extern int NW_SetRealCallBack(RealDataCallback cb, IntPtr user);

        [DllImport("NetwaySDK.dll")]
        public static extern int NW_SetAlarmCallBack(AlarmDataCallback cb, IntPtr user);
        #endregion

        [DllImport("NetwaySDK.dll")]
        public static extern int NW_Init();

        [DllImport("NetwaySDK.dll")]
        public static extern int NW_UnInit();

        [DllImport("NetwaySDK.dll")]
        public static extern uint NW_GetSdkVersion(ref uint mainVer, ref uint minorVer, ref uint build);

        [DllImport("NetwaySDK.dll")]
        public static extern int NW_StartListen(string ip, ushort port);

        [DllImport("NetwaySDK.dll")]
        public static extern uint NW_Login(string ip, ushort port, string username, string psw, int enc, int mode);

        [DllImport("NetwaySDK.dll")]
        public static extern uint NW_Logout(uint handle);

        [DllImport("NetwaySDK.dll")]
        public static extern uint NW_AddNode(uint handle, SDK_Node_Info info);

        [DllImport("NetwaySDK.dll")]
        public static extern uint NW_DelNode(uint handle, SDK_NodeId_List nodeIdList);

        [DllImport("NetwaySDK.dll")]
        public static extern uint NW_SuspendNode(uint handle, SDK_NodeId_List nodeIdList, uint type);

        [DllImport("NetwaySDK.dll")]
        public static extern uint NW_ResumeNode(uint handle, SDK_NodeId_List nodeIdList, uint type);

        [DllImport("NetwaySDK.dll")]
        public static extern uint NW_SetBlackWhite(uint handle, uint command, uint type);

        [DllImport("NetwaySDK.dll")]
        public static extern uint NW_BlackWhiteNode(uint handle, SDK_NodeId_List nodeIdList, uint command, uint type);

        [DllImport("NetwaySDK.dll")]
        public static extern uint NW_SetCommandToNode(uint handle, string nodeid, uint command, uint subType, SDK_Command_Data data);

        [DllImport("NetwaySDK.dll")]
        public static extern uint NW_SubscribeAllNodeData(uint handle, int type, int enable);

        [DllImport("NetwaySDK.dll")]
        public static extern uint NW_BatSubscribeNodeData(uint handle, int type, SDK_Subscrib_Node_List nodelist, int enable);

        [DllImport("NetwaySDK.dll")]
        public static extern uint NW_SetAutoParam(uint handle, SDK_AutoParm param);

        [DllImport("NetwaySDK.dll")]
        public static extern uint NW_SynchTime(uint handle, JH_Time time, string ip, uint port, int revision);

        [DllImport("NetwaySDK.dll")]
        public static extern uint NW_ChangePassword(uint handle, string oldpsw, string newpsw);

        [DllImport("NetwaySDK.dll")]
        public static extern uint NW_RebootDevice(uint handle, uint delay, string rebootreason, int len);

        [DllImport("NetwaySDK.dll")]
        public static extern uint NW_SetConfig(uint handle, string name, int len, string config, int strlen);

        [DllImport("NetwaySDK.dll")]
        public static extern uint NW_ManualUpgrade(uint handle, int type, string listurl, int len, JH_Time begintime, string ver, int vlen);

        [DllImport("NetwaySDK.dll")]
        public static extern uint NW_SetUpgrade(uint handle, SDK_Upgrade_Info info);

        [DllImport("NetwaySDK.dll")]
        public static extern uint NW_SetNodePriority(uint handle, string nodeid, int priority);

        [DllImport("NetwaySDK.dll")]
        public static extern uint NW_ImportConfigFile(uint handle, int type, string data, int len);

        [DllImport("NetwaySDK.dll")]
        public static extern uint NW_GetNWInfo(uint handle);

        [DllImport("NetwaySDK.dll")]
        public static extern uint NW_GetNodeList(uint handle, int type, int startLine, int count);

        [DllImport("NetwaySDK.dll")]
        public static extern uint NW_GetBlackWhite(uint handle, uint type);

        [DllImport("NetwaySDK.dll")]
        public static extern uint NW_GetNodeCachedData(uint handle, string nodeid, int sensorno);

        [DllImport("NetwaySDK.dll")]
        public static extern uint NW_GetNodeRealData(uint handle, string nodeid, int sensorno, uint timeout);

        [DllImport("NetwaySDK.dll")]
        public static extern uint NW_GetNodeSettings(uint handle, string nodeid, uint command, uint subType, int sensorNO);

        [DllImport("NetwaySDK.dll")]
        public static extern uint NW_CommonCommand(uint handle, int command, string data, int len);

        [DllImport("NetwaySDK.dll")]
        public static extern uint NW_GetConfig(uint handle, string name, int len);

        [DllImport("NetwaySDK.dll")]
        public static extern uint NW_GetConfigFile(uint handle, int type);

        [DllImport("NetwaySDK.dll")]
        public static extern uint NW_QueryLog(uint handle, int type, JH_Time starttime, JH_Time endtime, int startLine, int count);

        [DllImport("NetwaySDK.dll")]
        public static extern uint NW_GetHighPriorityList(uint handle);

        [DllImport("NetwaySDK.dll")]
        public static extern uint NW_GetAutoParam(uint handle);

        [DllImport("NetwaySDK.dll")]
        public static extern uint NW_GetTopology(uint handle);

        [DllImport("NetwaySDK.dll")]
        public static extern uint NW_GetUpDisconCachedAlarm(uint handle, JH_Time starttime, JH_Time endtime, int count);

    }
}
