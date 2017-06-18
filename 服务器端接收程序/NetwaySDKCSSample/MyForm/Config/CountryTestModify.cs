using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using 服务器端接收程序.Config;
using 服务器端接收程序.Clazz.Config.ClientConfig;
using Utility;
using System.Text.RegularExpressions;

namespace 服务器端接收程序.MyForm.Config
{
    public partial class CountryTestModify : Form
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
                    XML_CountryTest ct = SysConfig.clientConfig.AllCountryTest.SingleOrDefault(c => c.UniqueId == value);
                    if (ct != null)
                    {
                        cmb_station.SelectedValue = ct.StationUniqueId;
                        cmb_node.SelectedValue = ct.NodeId;
                        txt_testid.Text = ct.TestId.ToString();
                        txt_multiple.Text = ct.Multiple.ToString();
                    }
                }
                catch (Exception ex)
                {
                    LogMg.AddError(ex);
                }
            }
        }

        public CountryTestModify()
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

            cmb_node.DataSource = SysConfig.clientConfig.AllCountryNode;
            cmb_node.ValueMember = "NodeId";
            cmb_node.DisplayMember = "Remark";
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Validation())
                {
                    return;
                }

                XML_CountryTest ct = new XML_CountryTest();
                ct.StationUniqueId = Convert.ToInt32(cmb_station.SelectedValue);
                ct.NodeId = cmb_node.SelectedValue.ToString();
                ct.Remark = cmb_node.Text;
                ct.TestId = Convert.ToInt32(txt_testid.Text);
                ct.Multiple = Convert.ToInt32(txt_multiple.Text);

                string msg = "";
                if (UniqueId == 0)
                {
                    if (SysConfig.clientConfig.AddCountryTest(ct, ref msg))
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
                    ct.UniqueId = UniqueId;
                    if (SysConfig.clientConfig.EditCountryTest(ct, ref msg))
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
            catch (Exception ex)
            {
                LogMg.AddError(ex);
            }
        }

        /// <summary>
        /// 验证输入的格式 
        /// </summary>
        /// <returns></returns>
        public bool Validation()
        {
            Regex reg = new Regex(@"\d+");
            if (!reg.IsMatch(txt_testid.Text))
            {
                MessageBox.Show("TestId格式有误,请输入一个整数", "提示");
                return false;
            }
            try
            {
                double.Parse(txt_multiple.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("倍数格式有误,请输入一个数字");
                return false;
            }

            return true;
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
