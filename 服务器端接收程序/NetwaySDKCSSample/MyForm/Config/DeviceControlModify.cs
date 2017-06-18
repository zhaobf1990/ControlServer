using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using clientconfig = 服务器端接收程序.Clazz.Config.ClientConfig;
using 服务器端接收程序.Config;
using Utility;

namespace 服务器端接收程序.MyForm.Config
{
    public partial class DeviceControlModify : Form
    {
        private int _uniqueid;
        public int UniqueId
        {
            get
            {
                return _uniqueid;
            }
            set
            {
                _uniqueid = value;

                try
                {
                    clientconfig.XML_DeviceControl dc = SysConfig.clientConfig.AllDevice.SingleOrDefault(c => c.UniqueId == value);
                    if (dc != null)
                    {
                        cmb_station.SelectedValue = dc.StationUniqueId;
                        txt_number.Text = dc.Number;
                        txt_deviceid.Text = dc.DeviceId.ToString();
                    }
                }
                catch (Exception ex)
                {
                    LogMg.AddError(ex);
                }
            }
        }


        public DeviceControlModify()
        {
            InitializeComponent();

            BindCmb();
        }

        /// <summary>
        /// 绑定两个下拉框 
        /// </summary>
        public void BindCmb()
        {
            cmb_station.DataSource = SysConfig.clientConfig.AllStation;
            cmb_station.ValueMember = "UniqueId";
            cmb_station.DisplayMember = "StationName";
            cmb_station.SelectedIndex = -1;
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            if (!Validation())
            {
                return;
            }

            clientconfig.XML_DeviceControl _device = new clientconfig.XML_DeviceControl();
            _device.StationUniqueId = Convert.ToInt32(cmb_station.SelectedValue);
            _device.Number = txt_number.Text;
            _device.DeviceId = Convert.ToInt32(txt_deviceid.Text);

            string msg = "";
            if (UniqueId == 0)
            {
                if (SysConfig.clientConfig.AddDeviceControl(_device, ref msg))
                {
                    MessageBox.Show("添加成功", "提示");
                    this.Close();
                }
                else
                {
                    MessageBox.Show(msg, "提示");
                }
            }
            else
            {
                _device.UniqueId = UniqueId;
                if (SysConfig.clientConfig.EditDeviceControl(_device, ref msg))
                {
                    MessageBox.Show("修改成功", "提示");
                    this.Close();
                }
                else
                {
                    MessageBox.Show(msg, "提示");
                }
            }
        }


        /// <summary>
        /// 验证输入的格式 
        /// </summary>
        /// <returns></returns>
        public bool Validation()
        {
            try
            {
                double.Parse(txt_deviceid.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("寄存器格式有误,请输入一个数字");
                return false;
            }

            return true;
        }
    }
}
