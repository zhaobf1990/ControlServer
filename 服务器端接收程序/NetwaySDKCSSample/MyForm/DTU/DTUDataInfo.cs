using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using 服务器端接收程序.Config;

namespace 服务器端接收程序.MyForm.DTU
{
    public partial class DTUDataInfo : UserControl
    {
        private Thread thread = null;
        public DTUDataInfo()
        {
            InitializeComponent();

            txt_RequestToClientInterval.Text = (SysConfig.userProfile.RequestToClientInterval / 1000).ToString();
            txt_GongKuangConfigInterval.Text = (SysConfig.userProfile.GongKuangConfigInterval / 1000).ToString();

            thread = new Thread(ShowMessage);
            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// 从消息队列中获取数据,并展示到页面上
        /// </summary>
        private void ShowMessage()
        {
            while (true)
            {
               // List<string> messages = 服务器端接收程序.Util.MessageQueue.Dequeue_DataInfo();
                //////////////DTU多了以后，显示的这断代码太卡了，索性不显示了   注释日期2015-1-6   赵佰枫
                //foreach (string item in messages)
                //{
                //    lb_msg.BeginInvoke(new Action<string>(print), item);
                //}
                Thread.Sleep(2000);
            }
        }

        /// <summary>
        /// 显示到页面上
        /// </summary>
        private void print(string message)
        {
            if (lb_msg.Items.Count > 200)
            {
                lb_msg.Items.Clear();
            }
            lb_msg.Items.Add(message);
        }



        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                SysConfig.userProfile.RequestToClientInterval = double.Parse(txt_RequestToClientInterval.Text) * 1000;
                SysConfig.userProfile.GongKuangConfigInterval = double.Parse(txt_GongKuangConfigInterval.Text) * 1000;
                MessageBox.Show("已立即生效");
            }
            catch (Exception)
            {
                MessageBox.Show("您输入的格式有误,请输入数字,单位秒");
            }
            SysConfig.SaveConfig();
        }


    }
}
