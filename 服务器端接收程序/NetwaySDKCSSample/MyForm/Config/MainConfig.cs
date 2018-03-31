using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 服务器端接收程序.MyForm.Config
{
    public partial class MainConfig : Form
    {
        public MainConfig()
        {
            InitializeComponent();
        }

        private void MainConfig_Load(object sender, EventArgs e)
        {
            //站点配置
            StationConfig _station = new StationConfig();
            _station.Dock = DockStyle.Fill;
            tabPage1.Controls.Add(_station);

            //CountryTest
            CountryTestConfig countrytest = new CountryTestConfig();
            countrytest.Dock = DockStyle.Fill;
            tabPage2.Controls.Add(countrytest);

            ///设备控制
            DeviceControlConfig _deviceControl = new DeviceControlConfig();
            _deviceControl.Dock = DockStyle.Fill;
            tabPage3.Controls.Add(_deviceControl);
        }
    }
}
