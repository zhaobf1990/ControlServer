using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Threading;
using System.Windows.Forms;
using Homewell;
using Homewell.SDK;
using Lassalle.Flow;
using System.Linq;
using System.Data.SqlClient;
using System.Configuration;
using Utility;
using DBCJ = 服务器端接收程序.Clazz.Config;
using 服务器端接收程序.Util;
using 服务器端接收程序.Config;

namespace 服务器端接收程序
{
    public partial class MainForm : Form
    {
        private ToolTip tooltip = new ToolTip();

        private GlobalOperaCallback globalOperaCallback;
        private RealDataCallback realDataCallback;
        private AlarmDataCallback alarmDataCallback;
        private SDK_Register_Info registerInfo;
        private delegate void UpdateUI(string str);
        private List<Clazz.Config.XML_Org> allOrg;
        private List<DBCJ.XML_DBCJTest> ALLTestList;
        private delegate void SaveDataToDatabase(SWSDataContext db, realrec _realrec);
        private Thread GenerateFromDbToXMLThread;


        public MainForm()
        {
            InitializeComponent();

           

            //加载农村项目接收页面
            MyForm.Country.FormCountry _country = new MyForm.Country.FormCountry();
            _country.Dock = DockStyle.Fill;
            this.tabPage2.Controls.Add(_country);


            //加载屏幕取词接收页面
            MyForm.PMQC.FormPMQC _pmqc = new MyForm.PMQC.FormPMQC();
            _pmqc.Dock = DockStyle.Fill;
            this.tabPage3.Controls.Add(_pmqc);

            //加载配置页面
            MyForm.Config.FormConfig _config = new MyForm.Config.FormConfig();
            _config.Dock = DockStyle.Fill;
            this.tabPage4.Controls.Add(_config);

            //移动检测页面
            MyForm.MobileDetection.FormMobileDetection _MobileDetection = new MyForm.MobileDetection.FormMobileDetection();
            _MobileDetection.Dock = DockStyle.Fill;
            this.tabPage5.Controls.Add(_MobileDetection);

            //注册电表采集socket服务初始化
            ServerSocketHelper.DBCJInit += NW_Init;
            ServerSocketHelper.DBCJClose += NWUnInit;

            //接收客户端外网IP
            Util.ReceiveIp receiveIp = new ReceiveIp();

            //设备控制
            MyForm.DeviceControl.FormDeviceControl _deviceControl = new MyForm.DeviceControl.FormDeviceControl();
            _deviceControl.Dock = DockStyle.Fill;
            this.tabPage6.Controls.Add(_deviceControl);

            ///显示在线的客户端连接
            MyForm.ConnectInfo.ConnectInfo _connectInfo = new MyForm.ConnectInfo.ConnectInfo();
            _connectInfo.Dock = DockStyle.Fill;
            this.tabPage7.Controls.Add(_connectInfo);

            //显示采用DTU模式的数据采集信息
            MyForm.DTU.DTUDataInfo _dataInfo = new MyForm.DTU.DTUDataInfo();
            _dataInfo.Dock = DockStyle.Fill;
            this.tabPage8.Controls.Add(_dataInfo);
              
            ///检测客户端连接
            Util.CheckConnect check = new Util.CheckConnect();
            check.Start();

            GenerateFromDbToXMLThread = new Thread(new ThreadStart(GenerateXML));
            GenerateFromDbToXMLThread.IsBackground = true;
            GenerateFromDbToXMLThread.Start();


        }

        private void GenerateXML()
        {
            while (true)
            {
                Thread.Sleep(30 * 60 * 1000);  //10分钟   
                SysConfig.DTU_StationConfig.GenerateFromDbToXML();
                SysConfig.DTU_StationConfig.ReadConfig();
                V88CommunicationThread.getInstance().readConfig();
                DTU_ClientManager.Update();
            }

        }

        public void NW_Init()
        {
            try
            {

                allOrg = SysConfig.orgConfig.Orgs; ;
                ALLTestList = SysConfig.DBCJconfig.ListTest;
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
                DEBUG.ThrowException(ex);
            }

            NetwaySDK.NW_Init();
            globalOperaCallback = DoOnGlobalOperation;
            realDataCallback = DoOnRealData;
            alarmDataCallback = DoOnAlarmData;
            NetwaySDK.NW_SetGlobalOperaCallBack(globalOperaCallback, IntPtr.Zero); //设置全局操作回调
            NetwaySDK.NW_SetRealCallBack(realDataCallback, IntPtr.Zero);           //设置实时数据回调
            NetwaySDK.NW_SetAlarmCallBack(alarmDataCallback, IntPtr.Zero);         //设置警报回调
            NetwaySDK.NW_StartListen("0.0.0.0", 9000);
        }

        private void NWUnInit()
        {
            NetwaySDK.NW_SetRealCallBack(null, IntPtr.Zero);           //设置实时数据回调
            NetwaySDK.NW_SetAlarmCallBack(null, IntPtr.Zero);         //设置警报回调
        }

        // 初始化SDK
        private void MainForm_Load(object sender, EventArgs e)
        {
        }

        // 通用回调处理函数
        private int DoOnGlobalOperation(uint handle, int seq, int opera, int result, ref Global_Opera_t info, IntPtr user, int resverd)
        {
            switch (opera)
            {
                case (int)SDKOperation.JH_SYSTEM_LOGIN:
                    DoOnLoginResponse(handle, seq, result, info);
                    break;
                case (int)SDKOperation.JH_SYSTEM_DISCONNECT_OFFLINE:
                    break;
                case (int)SDKOperation.JH_SYSTEM_RECONNECT_LOGIN:
                    break;
                case (int)SDKOperation.JH_SYSTEM_LOGOUT:
                    break;
                case (int)SDKOperation.JH_SYSTEM_NETWAY_REGISTER:
                    DoOnNetwayRegister(handle, seq, result, info);
                    break;
                default:
                    break;
            }

            return 0;
        }

        //时间转换函数
        private DateTime JHTimeToDateTime(JH_Time jhTime)
        {
            return new DateTime(jhTime.year + 1900,
                                jhTime.month,
                                jhTime.day,
                                jhTime.hour,
                                jhTime.min,
                                jhTime.sec);
        }

        // 实时数据回调 目前支持6路传感器,以后通道数会增加
        private int DoOnRealData(uint handle, int seq, ref SDK_Node_Real_Data data, IntPtr user)
        {

            try
            {
                //找出数据节点Id所对应的分厂
                SDK_Node_Real_Data _data = data;
                List<DBCJ.XML_DBCJTest> tests = ALLTestList.Where(c => c.NodeId == _data.nodeid).ToList();

                //如果没有这个采集设备的信息    则丢弃这个设所采集的数据    直接return
                if (tests == null)
                {
                    BeginInvoke(new UpdateUI(WriteStatus), string.Format("节点Id为 {0} 的设备匹配不到对应的分厂", data.nodeid));
                    return 0;
                }
                Clazz.Config.XML_Org org = allOrg.SingleOrDefault(c => c.OrgId == tests[0].OrgId);
                BeginInvoke(new UpdateUI(WriteStatus), string.Format("实时数据 节点Id：{0} 企业名称:{1} 数据库名称:{2}", data.nodeid, org.Name, org.DBName));

                SWSDataContext db = new SWSDataContext(ServerSocketHelper.GetConnection(org.DBName));       //分厂数据库实例

                for (int i = 0; i < data.sensorData.Length; i++)
                {
                    //检测该通道是否有效
                    if (data.sensorData[i].valid == 0)
                    {
                        continue;
                    }

                    BeginInvoke(new UpdateUI(WriteStatus),
                                string.Format("通道{0}, 类型{1}, 数据:{2}, 时间:{3}",
                                              i + 1,
                                              data.sensorData[i].type,
                                              data.sensorData[i].data,
                                              JHTimeToDateTime(data.sensorData[i].time).ToString()));


                    //这里有空改成用线程去保存数据到数据库
                    #region 用异步的方式将数据保存到数据库

                    realrec _realrec = new 服务器端接收程序.realrec();
                    _realrec.testid = Convert.ToInt32(tests.SingleOrDefault(c => c.NodeId == _data.nodeid).TestId);//检测点ID
                    _realrec.testtime = JHTimeToDateTime(data.sensorData[i].time);//时间
                    _realrec.value = (decimal)data.sensorData[i].data;//获取的电量                 

                    BeginInvoke(new SaveDataToDatabase(Save), db, _realrec);

                    #endregion
                }


            }
            catch (Exception e)
            {
                LogMg.AddError(e);
            }
            return 0;
        }

        // 报警数据回调 目前支持6路传感器，以后通道数会增加
        private int DoOnAlarmData(uint handle, int seq, ref SDK_Node_Alarm_Data data, IntPtr user)
        {
            BeginInvoke(new UpdateUI(WriteStatus), string.Format("报警数据 节点Id：{0} ", data.nodeid));
            for (int i = 0; i < data.sensorData.Length; i++)
            {
                if (data.sensorData[i].valid == 0)
                {
                    continue;
                }

                BeginInvoke(new UpdateUI(WriteStatus),
                            string.Format("通道{0}, 类型{1}, 数据:{2}, 时间:{3}",
                                          i + 1,
                                          data.sensorData[i].type,
                                          data.sensorData[i].data,
                                          JHTimeToDateTime(data.sensorData[i].time).ToString()));
            }
            return 0;
        }

        private void WriteStatus(string status)
        {
            lb_msg.Items.Add(status);
            if (lb_msg.Items.Count > 200)
            {
                for (int i = 0; i < 100; i++)
                    lb_msg.Items.RemoveAt(i);
            }
        }

        private void Save(SWSDataContext db, realrec _realrec)
        {
            try
            {
                db.realrec.InsertOnSubmit(_realrec);
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
            }

        }

        // 主动注册处理
        private void DoOnNetwayRegister(uint handle, int seq, int result, Global_Opera_t info)
        {
            registerInfo = (SDK_Register_Info)Marshal.PtrToStructure(info.param1,
                                                                     typeof(SDK_Register_Info));
            BeginInvoke(new UpdateUI(WriteStatus),
                        string.Format("收到网由注册请求，来自{0} 地址{1}",
                                      registerInfo.id,
                                      registerInfo.ip + ':' + registerInfo.port));


            NetwaySDK.NW_Login(registerInfo.ip, (ushort)registerInfo.port, edtName.Text, edtPassword.Text,
                               UDCConstDefine.UDC_NW_ENCRYPT_NO, (int)NetwayMode.ACTIVE_MODE);

            BeginInvoke(new UpdateUI(WriteStatus),
                        string.Format("开始登录网由，用户名:{0} 密码:{1}",
                                      edtName.Text,
                                      edtPassword.Text));
        }

        // 登陆回应处理
        private void DoOnLoginResponse(uint handle, int seq, int result, Global_Opera_t info)
        {
            if (result == (int)SDKErrorCode.SDK_ERROR_SUCCESS)
            {
                NetwaySDK.NW_SubscribeAllNodeData(handle, UDCConstDefine.UDC_NW_NODE_DATA_ALL, 1); //1为订阅所有数据
                BeginInvoke(new UpdateUI(WriteStatus), string.Format("登录成功，订阅所有数据"));
            }
            else if (result == (int)SDKErrorCode.SDK_ERROR_USER_PASSWORD_INVALID)
            {
                BeginInvoke(new UpdateUI(WriteStatus), string.Format("登录失败，密码错误"));
            }
            else if (result == (int)SDKErrorCode.SDK_ERROR_USER_INVALID_STATUS)
            {
                BeginInvoke(new UpdateUI(WriteStatus), string.Format("登录失败，用户会话状态异常"));
            }
            else if (result == (int)SDKErrorCode.SDK_ERROR_USER_NOT_EXIST)
            {
                BeginInvoke(new UpdateUI(WriteStatus), string.Format("登录失败，用户不存在"));
            }
            else if (result == (int)SDKErrorCode.SDK_ERROR_USER_NOT_LOGIN)
            {
                BeginInvoke(new UpdateUI(WriteStatus), string.Format("登录失败，用户未登录"));
            }

        }



        private void txt_clear_Click(object sender, EventArgs e)
        {
            if (lb_msg.Items.Count > 0)
            {
                lb_msg.Items.Clear();
            }

        }

        //提示
        private void txt_port_MouseEnter(object sender, EventArgs e)
        {
            tooltip.Show("提供给外部用于传输的端口", (Control)sender);
        }


    }
}
