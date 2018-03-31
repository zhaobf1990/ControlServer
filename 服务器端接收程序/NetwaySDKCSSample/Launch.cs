using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using 服务器端接收程序.Config;
using System.Threading;

namespace 服务器端接收程序
{
    public partial class Launch : Form
    {
        Thread thread;
        private delegate void readFinish();
        public Launch()
        {
            InitializeComponent();


        }

        private void Launch_Load(object sender, EventArgs e)
        {
            thread = new Thread(new ThreadStart(readConfig));
            thread.Start();
            thread.IsBackground = true;
        }

        private void readConfig()
        {
            ///读取配置文件
            SysConfig.ReadConfig();
            BeginInvoke(new readFinish(launchMain));


        }

        private void launchMain()
        {
            MainForm main = new MainForm();
            main.Show();

            this.Hide();
        }

    }
}
