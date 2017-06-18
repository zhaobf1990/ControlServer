using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Utility;
using 服务器端接收程序.Util;
using System.Net;
using System.Data.SqlClient;
using System.Timers;
using 服务器端接收程序.Config;
using 服务器端接收程序.MyForm.GPRSControl;
using System.Threading;


namespace 服务器端接收程序.MyForm.Config
{
    public partial class FormConfig : UserControl
    {

        private System.Timers.Timer AutoStartTimer;
        MainConfig _mainConfig = new MainConfig();

        private Thread threadPoolMsg;
        public FormConfig()
        {
            InitializeComponent();

            read();

            ///自动启动服务的定时器
            AutoStartTimer = new System.Timers.Timer();//实例化Timer类，
            AutoStartTimer.Interval = SysConfig.userProfile.AutoStart;         //设置间隔时间为30秒；
            AutoStartTimer.Elapsed += new System.Timers.ElapsedEventHandler(AutoStartHandler);//到达时间的时候执行事件； 
            AutoStartTimer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)； 
            AutoStartTimer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件； 

            threadPoolMsg = new Thread(new ThreadStart(ShowPoolMsg));
            threadPoolMsg.IsBackground = true;
            threadPoolMsg.Start();
        }

        private void ShowPoolMsg()
        {
            while (true)
            {
                Thread.Sleep(10000);
                if (ControlCommandThread.PoolA != null)
                {
                    lblAmin.BeginInvoke(new Action<Label, int>(UploadControl), lblAmin, ControlCommandThread.PoolA.GetMin());
                    lblAmax.BeginInvoke(new Action<Label, int>(UploadControl), lblAmax, ControlCommandThread.PoolA.GetMax());
                    lblApublicpool.BeginInvoke(new Action<Label, int>(UploadControl), lblApublicpool, ControlCommandThread.PoolA.GetPublicPoolCount());
                    lblAfreequeue.BeginInvoke(new Action<Label, int>(UploadControl), lblAfreequeue, ControlCommandThread.PoolA.GetFreeQueueCount());
                    lblAworking.BeginInvoke(new Action<Label, int>(UploadControl), lblAworking, ControlCommandThread.PoolA.GetWorkingCount());
                    lblAwaitlist.BeginInvoke(new Action<Label, int>(UploadControl), lblAwaitlist, ControlCommandThread.PoolA.GetWaitListCount());
                }
                if (AutoCollectionThread.PoolB != null)
                {
                    lblBmin.BeginInvoke(new Action<Label, int>(UploadControl), lblBmin, AutoCollectionThread.PoolB.GetMin());
                    lblBmax.BeginInvoke(new Action<Label, int>(UploadControl), lblBmax, AutoCollectionThread.PoolB.GetMax());
                    lblBpublicpool.BeginInvoke(new Action<Label, int>(UploadControl), lblBpublicpool, AutoCollectionThread.PoolB.GetPublicPoolCount());
                    lblBfreequeue.BeginInvoke(new Action<Label, int>(UploadControl), lblBfreequeue, AutoCollectionThread.PoolB.GetFreeQueueCount());
                    lblBworking.BeginInvoke(new Action<Label, int>(UploadControl), lblBworking, AutoCollectionThread.PoolB.GetWorkingCount());
                    lblBwaitlist.BeginInvoke(new Action<Label, int>(UploadControl), lblBwaitlist, AutoCollectionThread.PoolB.GetWaitListCount());
                }
                if (HeartbeatThread.PoolC != null)
                {
                    lblCmin.BeginInvoke(new Action<Label, int>(UploadControl), lblCmin, HeartbeatThread.PoolC.GetMin());
                    lblCmax.BeginInvoke(new Action<Label, int>(UploadControl), lblCmax, HeartbeatThread.PoolC.GetMax());
                    lblCpublicpool.BeginInvoke(new Action<Label, int>(UploadControl), lblCpublicpool, HeartbeatThread.PoolC.GetPublicPoolCount());
                    lblCfreequeue.BeginInvoke(new Action<Label, int>(UploadControl), lblCfreequeue, HeartbeatThread.PoolC.GetFreeQueueCount());
                    lblCworking.BeginInvoke(new Action<Label, int>(UploadControl), lblCworking, HeartbeatThread.PoolC.GetWorkingCount());
                    lblCwaitlist.BeginInvoke(new Action<Label, int>(UploadControl), lblCwaitlist, HeartbeatThread.PoolC.GetWaitListCount());
                }
                if (GPRSControl.DTU_NetServer.PoolD != null)
                {
                    lblDmin.BeginInvoke(new Action<Label, int>(UploadControl), lblDmin, GPRSControl.DTU_NetServer.PoolD.GetMin());
                    lblDmax.BeginInvoke(new Action<Label, int>(UploadControl), lblDmax, GPRSControl.DTU_NetServer.PoolD.GetMax());
                    lblDpublicpool.BeginInvoke(new Action<Label, int>(UploadControl), lblDpublicpool, GPRSControl.DTU_NetServer.PoolD.GetPublicPoolCount());
                    lblDfreequeue.BeginInvoke(new Action<Label, int>(UploadControl), lblDfreequeue, GPRSControl.DTU_NetServer.PoolD.GetFreeQueueCount());
                    lblDworking.BeginInvoke(new Action<Label, int>(UploadControl), lblDworking, GPRSControl.DTU_NetServer.PoolD.GetWorkingCount());
                    lblDwaitlist.BeginInvoke(new Action<Label, int>(UploadControl), lblDwaitlist, GPRSControl.DTU_NetServer.PoolD.GetWaitListCount());
                }
                if (V88CommunicationThread.getInstance().PoolE != null)
                {
                    lblEmin.BeginInvoke(new Action<Label, int>(UploadControl), lblEmin, V88CommunicationThread.getInstance().PoolE.GetMin());
                    lblEmax.BeginInvoke(new Action<Label, int>(UploadControl), lblEmax, V88CommunicationThread.getInstance().PoolE.GetMax());
                    lblEpublicpool.BeginInvoke(new Action<Label, int>(UploadControl), lblEpublicpool, V88CommunicationThread.getInstance().PoolE.GetPublicPoolCount());
                    lblEfreequeue.BeginInvoke(new Action<Label, int>(UploadControl), lblEfreequeue, V88CommunicationThread.getInstance().PoolE.GetFreeQueueCount());
                    lblEworking.BeginInvoke(new Action<Label, int>(UploadControl), lblEworking, V88CommunicationThread.getInstance().PoolE.GetWorkingCount());
                    lblEwaitlist.BeginInvoke(new Action<Label, int>(UploadControl), lblEwaitlist, V88CommunicationThread.getInstance().PoolE.GetWaitListCount());
                }
                //lblAinterval.Text = (SysConfig.userProfile.GongKuangConfigInterval / 1000).ToString();
                //lblBinterval.Text = (SysConfig.userProfile.RequestToClientInterval / 1000).ToString();
                //lblCinterval.Text = (SysConfig.userProfile.HeartbeatInterval / 1000).ToString();
                lblAinterval.BeginInvoke(new Action<Label, int>(UploadControl), lblAinterval, (int)SysConfig.userProfile.GongKuangConfigInterval / 1000);
                lblBinterval.BeginInvoke(new Action<Label, int>(UploadControl), lblBinterval, (int)SysConfig.userProfile.RequestToClientInterval / 1000);
                lblCinterval.BeginInvoke(new Action<Label, int>(UploadControl), lblCinterval, (int)SysConfig.userProfile.HeartbeatInterval / 1000);


            }
        }

        private void UploadControl(Label label, int msg)
        {
            label.Text = msg.ToString();
        }
        /// <summary>
        /// 自动启动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoStartHandler(object sender, ElapsedEventArgs e)
        {
            if (btn_Start.Enabled)
            {
                btn_Start.PerformClick();
            }

        }


        //服务开启
        private void btn_Start_Click(object sender, EventArgs e)
        {
            try
            {
                if (test_connection() == false)
                {
                    listBox1.Items.Add("连接数据库失败");
                    listBox1.Items.Add("请确保数据库服务已开启,并且数据库连接填写正确");
                    return;
                }

                ServerSocketHelper.Init();
                ServerSocketHelper.StartListen(txt_ServerIp.Text.Trim(), txt_Port_SWS.Text.Trim());


                GPRSControl.DTU_NetServer.Init();
                // GPRSControl.GPRSControlNetServer.StartListen("192.168.1.8","5678");
                GPRSControl.DTU_NetServer.StartListen(txt_ServerIp.Text.Trim(), txt_port_DTU.Text.Trim());

                //V88CommunicationThread.getInstance().Start();
                //UdpV88Server.getInstance().StartListen(txt_ServerIp.Text.Trim(), txt_port_udp.Text.Trim());

                //心跳线程开启
                HeartbeatThread.Start();
                //主动发指令的线程开启
              ControlCommandThread.Start();
                //定时发指令的线程开启
              AutoCollectionThread.Start(); 

                save();

                btn_Start.Enabled = false;
                btn_Stop.Enabled = true;

                ///关闭自动启动服务的定时器
                if (AutoStartTimer != null)
                {
                    AutoStartTimer.Stop();
                }
                listBox1.Items.Add("服务已成功开启");

            }
            catch (Exception ex)
            {
                ServerSocketHelper.Close();
                //  DTU_NetServer.Close();

                LogMg.AddError(ex);
                listBox1.Items.Add(ex.Message);
                //  MessageBox.Show(ex.Message);
            }
        }


        /// <summary>
        /// 读取配置信息
        /// </summary>
        private void read()
        {
            txt_db_address.Text = SysConfig.userProfile.DbAddress;
            txt_db_name.Text = SysConfig.userProfile.DbName;
            txt_db_username.Text = SysConfig.userProfile.DbUserName;
            txt_db_pwd.Text = SysConfig.userProfile.DbPassword;
            txt_ServerIp.Text = SysConfig.userProfile.serverIp;
            txt_port_DBCJ.Text = SysConfig.userProfile.DBCJPort;
            txt_Port_SWS.Text = SysConfig.userProfile.SWSPort;
            txt_port_DTU.Text = SysConfig.userProfile.DTUPort;
            txt_port_udp.Text = SysConfig.userProfile.UDPPort;
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        private void save()
        {
            SysConfig.userProfile.DbAddress = txt_db_address.Text.Trim();
            SysConfig.userProfile.DbName = txt_db_name.Text.Trim();
            SysConfig.userProfile.DbUserName = txt_db_username.Text.Trim();
            SysConfig.userProfile.DbPassword = txt_db_pwd.Text.Trim();

            SysConfig.userProfile.serverIp = txt_ServerIp.Text.Trim();
            SysConfig.userProfile.SWSPort = txt_Port_SWS.Text.Trim();
            SysConfig.userProfile.DBCJPort = txt_port_DBCJ.Text.Trim();
            SysConfig.userProfile.DTUPort = txt_port_DTU.Text.Trim();
            SysConfig.userProfile.UDPPort = txt_port_udp.Text.Trim();

            ///保存配置
            SysConfig.SaveConfig();
        }



        //服务停止 
        private void btn_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                ServerSocketHelper.Close();
                //  DTU_NetServer.Close();

                UdpV88Server.getInstance().close();

                btn_Start.Enabled = true;
                btn_Stop.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 测试数据库连接   如果连接成功, 并立即使用
        /// </summary>
        private bool test_connection()
        {
            bool flag = false;
            try
            {
                SqlConnection con = Utility.ConnectStringHelper.GetConnection(txt_db_address.Text.Trim(), txt_db_name.Text.Trim(), txt_db_username.Text.Trim(), txt_db_pwd.Text.Trim());
                con.Open();
                con.Close();
                flag = true;

                SysConfig.userProfile.DbAddress = txt_db_address.Text.Trim();
                SysConfig.userProfile.DbUserName = txt_db_username.Text.Trim();
                SysConfig.userProfile.DbPassword = txt_db_pwd.Text.Trim();

                SysConfig.SaveConfig();
            }
            catch (Exception)
            {
                flag = false;
            }
            return flag;
        }

        /// <summary>
        /// 测试连接并立即使用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_test_connection_Click(object sender, EventArgs e)
        {
            if (test_connection() == true)
            {
                try
                {
                    save();
                }
                catch (Exception ex)
                {
                    LogMg.AddError(ex);
                    DEBUG.ThrowException(ex);
                }
                MessageBox.Show("连接数据库成功");
            }
            else
            {
                MessageBox.Show("连接数据库失败");
            }
        }

        /// <summary>
        /// 重新读取配置文件 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ReadConfig_Click(object sender, EventArgs e)
        {
            MessageBox.Show("开始读取配置");
            new Thread(new ThreadStart(readConfig)).Start(); 
        }

        private void readConfig() {
            SysConfig.ReadConfig();
            BeginInvoke(new Action<String>(showMessageBox), "读取配置成功");
        }

        private void btn_Config_Click(object sender, EventArgs e)
        {
            if (_mainConfig.IsDisposed)
            {
                _mainConfig = new MainConfig();
            }
            _mainConfig.Show();
            _mainConfig.Activate();
        }

        /// <summary>
        /// 从数据库生成配置文件到XML
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDbToXML_Click(object sender, EventArgs e)
        {
            MessageBox.Show("此动作需要半分钟左右，之后会弹出失败或成功的对话框");
            new Thread(new ThreadStart(dbToXML)).Start();
        }
        private void dbToXML()
        {
            DateTime start = DateTime.Now;
            LogMg.AddDebug("准备开始从数据库生成站点和检测点配置");
            //if (SysConfig.DTU_StationConfig.GenerateFromDbToXML() == true)
            if (SysConfig.DTU_StationConfig.GenerateFromDbToXML() == true && SysConfig.GD_Config.GenerateConfig() == true)
            {
                LogMg.AddDebug("从数据库生成站点和检测点配置到XML成功");
                //MessageBox.Show("");
                SysConfig.ReadConfig();
                DTU_ClientManager.Update();
                BeginInvoke(new Action<String>(showMessageBox), "已成功从数据库生成配置");
            }
            else
            {
                LogMg.AddDebug("从数据库生成站点和检测点配置失败");
                BeginInvoke(new Action<String>(showMessageBox), "从数据库生成站点和检测点配置失败，请查看日志文件");
            }
            DateTime end = DateTime.Now;
            LogMg.AddInfo("生成xml并读取配置到内存耗时：" + DateUtil.datediff(start, end));
        }

        private void showMessageBox(String str)
        {
            MessageBox.Show(str);
        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void btnASetMin_Click(object sender, EventArgs e)
        {
            SetMinValue(ControlCommandThread.PoolA, tbAmin);
        }

        private void SetMinValue(TaskPool pool, TextBox tb)
        {
            try
            {
                int value = int.Parse(tb.Text);
                pool.Setminthread(value);
                switch (pool.Name)
                {
                    case "A":
                        SysConfig.userProfile.ControlCommandThreadPool_Min = value;
                        break;
                    case "B":
                        SysConfig.userProfile.AutoCollectionThreadPool_Min = value;
                        break;
                    case "C":
                        SysConfig.userProfile.HeartbeatThreadPool_Min = value;
                        break;
                    case "D":
                        SysConfig.userProfile.NetServerThreadPool_Min = value;
                        break;
                    case "E":
                        SysConfig.userProfile.V88ThreadPool_Min = value;
                        break;
                }
                SysConfig.userProfile.SaveConfig();

                MessageBox.Show("设置成功，立即生效");

            }
            catch (Exception)
            {
                MessageBox.Show("请输入整数");
            }
        }
        private void SetMaxValue(TaskPool pool, TextBox tb)
        {
            try
            {
                int value = int.Parse(tb.Text);
                pool.Setmaxthread(value);
                switch (pool.Name)
                {
                    case "A":
                        SysConfig.userProfile.ControlCommandThreadPool_Max = value;
                        break;
                    case "B":
                        SysConfig.userProfile.AutoCollectionThreadPool_Max = value;
                        break;
                    case "C":
                        SysConfig.userProfile.HeartbeatThreadPool_Max = value;
                        break;
                    case "D":
                        SysConfig.userProfile.NetServerThreadPool_Max = value;
                        break;
                    case "E":
                        SysConfig.userProfile.V88ThreadPool_Max = value;
                        break;
                }
                SysConfig.userProfile.SaveConfig();
                MessageBox.Show("设置成功，立即生效");
            }
            catch (Exception)
            {
                MessageBox.Show("请输入整数");
            }
        }

        private void btnAsetMax_Click(object sender, EventArgs e)
        {
            SetMaxValue(ControlCommandThread.PoolA, tbAmax);
        }

        private void btnBSetMin_Click(object sender, EventArgs e)
        {
            SetMinValue(AutoCollectionThread.PoolB, tbBmin);
        }

        private void btnBsetMax_Click(object sender, EventArgs e)
        {
            SetMaxValue(AutoCollectionThread.PoolB, tbBmax);
        }

        private void btnCSetMin_Click(object sender, EventArgs e)
        {
            SetMinValue(HeartbeatThread.PoolC, tbCmin);
        }

        private void btnCsetMax_Click(object sender, EventArgs e)
        {
            SetMaxValue(HeartbeatThread.PoolC, tbCmax);
        }

        private void btnAinterval_Click(object sender, EventArgs e)
        {
            try
            {
                SysConfig.userProfile.GongKuangConfigInterval = double.Parse(tbAinterval.Text) * 1000;
                MessageBox.Show("已立即生效");
            }
            catch (Exception)
            {
                MessageBox.Show("您输入的格式有误,请输入数字,单位秒");
            }
            SysConfig.SaveConfig();
        }

        private void btnBinterval_Click(object sender, EventArgs e)
        {
            try
            {
                SysConfig.userProfile.RequestToClientInterval = double.Parse(tbBinterval.Text) * 1000;

                MessageBox.Show("已立即生效");
            }
            catch (Exception)
            {
                MessageBox.Show("您输入的格式有误,请输入数字,单位秒");
            }
            SysConfig.SaveConfig();
        }
        private void btnCinterval_Click(object sender, EventArgs e)
        {
            try
            {
                SysConfig.userProfile.HeartbeatInterval = double.Parse(tbCinterval.Text) * 1000;

                MessageBox.Show("已立即生效");
            }
            catch (Exception)
            {
                MessageBox.Show("您输入的格式有误,请输入数字,单位秒");
            }
            SysConfig.SaveConfig();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SetMinValue(GPRSControl.DTU_NetServer.PoolD, tbDmin);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SetMaxValue(GPRSControl.DTU_NetServer.PoolD, tbDmax);
        }

        private void btnESetMin_Click(object sender, EventArgs e)
        {
            SetMaxValue(V88CommunicationThread.getInstance().PoolE, tbEmin);
        }

        private void btnEsetMax_Click(object sender, EventArgs e)
        {
            SetMaxValue(V88CommunicationThread.getInstance().PoolE, tbEmax);
        }






    }
}
