using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Utility;
using 服务器端接收程序.Config;
using 服务器端接收程序.Clazz.Config.ClientConfig;
using System.Text.RegularExpressions;

namespace 服务器端接收程序.MyForm.Config
{
    public partial class StationModify : Form
    {
        //验证正整数
        Regex reg = new Regex(@"\d+");


        private int _uniqueId;
        public int UniqueId
        {
            get
            {
                return _uniqueId;
            }
            set
            {
                _uniqueId = value;
                try
                {
                    XML_Station s = SysConfig.clientConfig.AllStation.SingleOrDefault(c => c.UniqueId == value);
                    if (s != null)
                    {
                        txt_stationId.Text = s.StationId.ToString();
                        txt_stationName.Text = s.StationName;
                        txt_orgid.Text = s.OrgId;
                        txt_transferCode.Text = s.TransferCode.ToString();
                    }
                    else
                    {
                        MessageBox.Show("找不到这个站点,可能已经被删除");
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    LogMg.AddError(ex);
                }

            }
        }
        public StationModify()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Save_Click(object sender, EventArgs e)
        {   //新增    
            try
            {
                if (!Validation())
                    return;

                XML_Station _station = new XML_Station();
                _station.StationId = int.Parse(txt_stationId.Text);
                _station.StationName = txt_stationName.Text;
                _station.OrgId = txt_orgid.Text;
                _station.TransferCode = txt_transferCode.Text;

                string msg = "";
                if (UniqueId == 0)
                {
                    if (SysConfig.clientConfig.AddStation(_station, ref msg))
                    {
                        MessageBox.Show("添加成功");
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(msg);
                    }
                }
                else
                {
                    //修改  
                    _station.UniqueId = UniqueId;
                    if (SysConfig.clientConfig.EditStation(_station, ref msg))
                    {
                        MessageBox.Show("修改成功");
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                LogMg.AddError(ex);
            }


        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        /// <summary>
        /// 验证输入的格式是否正确
        /// </summary>
        /// <returns></returns>
        private bool Validation()
        {
            if (txt_stationId.Text.Trim() == "")
            {
                MessageBox.Show("站点Id不能为空");
                return false;
            }
            if (!reg.IsMatch(txt_stationId.Text.Trim()))
            {
                MessageBox.Show("站点Id格式不正确,请输入正整数");
                return false;
            }
            if (txt_stationName.Text.Trim() == "")
            {
                MessageBox.Show("站点名称不能为空");
                return false;
            }
            if (txt_orgid.Text.Trim() == "")
            {
                MessageBox.Show("orgid不能为空");
                return false;
            }
            if (txt_transferCode.Text.Trim() == "")
            {
                MessageBox.Show("传输编码不能为空");
                return false;
            }
            return true;
        }
    }
}
