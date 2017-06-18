using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using 服务器端接收程序.Config;
using 服务器端接收程序.Clazz.Config.ClientConfig;
using System.Text.RegularExpressions;
using Utility;

namespace 服务器端接收程序.MyForm.Config
{
    public partial class StationConfig : UserControl
    {
        //验证正整数
        Regex reg = new Regex(@"\d+");

        public StationConfig()
        {
            InitializeComponent();

            dataGridView1.AutoGenerateColumns = false;

            dataGridView1.DataSource = SysConfig.clientConfig.AllStation;
        }

        /// <summary>
        /// 添加站点配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Add_Click(object sender, EventArgs e)
        {
            StationModify add = new StationModify();
            add.ShowDialog();

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = SysConfig.clientConfig.AllStation;
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

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_search_Click(object sender, EventArgs e)
        {
            List<XML_Station> list = SysConfig.clientConfig.AllStation.Where(c => true).ToList();

            if (txt_stationId.Text.Trim() != "")
            {
                try
                {
                    int stationId = int.Parse(txt_stationId.Text.Trim());
                    list = list.Where(c => c.StationId == stationId).ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("站点Id格式不正确,请输入正整数");
                    LogMg.AddError(ex);
                    return;
                }
            }
            if (txt_stationName.Text.Trim() != "")
            {
                list = list.Where(c => c.StationName.Contains(txt_stationName.Text.Trim())).ToList();
            }
            if (txt_orgid.Text.Trim() != "")
            {
                list = list.Where(c => c.OrgId == txt_orgid.Text.Trim()).ToList();
            }
            if (txt_transferCode.Text.Trim() != "")
            {
                list = list.Where(c => c.TransferCode == txt_transferCode.Text.Trim()).ToList();
            }
            dataGridView1.DataSource = list;


            //if()
            //list=SysConfig.clientConfig.AllStation.Where(c=>c.)
        }

        /// <summary>
        /// 行选择事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Edit_Click(object sender, EventArgs e)
        {
            try
            {
                DataGridViewSelectedRowCollection rows = dataGridView1.SelectedRows;
                if (rows == null || rows.Count <= 0)
                {
                    MessageBox.Show("请先选择要修改的行", "提示");
                    return;
                }
                DataGridViewRow row = rows[0];

                StationModify edit = new StationModify();
                edit.UniqueId = Convert.ToInt32(row.Cells["UniqueId"].Value);

                edit.ShowDialog();

                dataGridView1.DataSource = null;
                dataGridView1.DataSource = SysConfig.clientConfig.AllStation;

            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
            }
        }

        /// <summary>
        /// 删除 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Del_Click(object sender, EventArgs e)
        {
            try
            {
                DataGridViewSelectedRowCollection rows = dataGridView1.SelectedRows;

                if (rows == null || rows.Count <= 0)
                {
                    MessageBox.Show("请先选中行", "提示");
                    return;
                }
                DataGridViewRow row = rows[0];

                if (MessageBox.Show("您确定要删除这个站点!", "确认", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }

                int uniqueId = int.Parse(row.Cells["UniqueId"].Value.ToString());
                string msg = "";
                if (SysConfig.clientConfig.DelStation(uniqueId, ref msg))
                {                  
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = SysConfig.clientConfig.AllStation;
                    MessageBox.Show("删除成功");
                }
                else
                {
                    MessageBox.Show(msg);
                }
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
            }

        }


    }
}
