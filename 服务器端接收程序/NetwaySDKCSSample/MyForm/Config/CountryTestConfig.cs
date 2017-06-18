using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using 服务器端接收程序.Config;
using Utility;
using 服务器端接收程序.Clazz.Config.ClientConfig;

namespace 服务器端接收程序.MyForm.Config
{
    public partial class CountryTestConfig : UserControl
    {
        public CountryTestConfig()
        {
            InitializeComponent();

            dataGridView1.AutoGenerateColumns = false;
            BindGrid();

            BindCmb();
        }

        private void BindGrid()
        {
            dataGridView1.DataSource = SysConfig.clientConfig.AllCountryTest;
        }

        private void CountryTestConfig_Load(object sender, EventArgs e)
        {

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

            cmb_node.DataSource = SysConfig.clientConfig.AllCountryNode;
            cmb_node.ValueMember = "NodeId";
            cmb_node.DisplayMember = "Remark";
            cmb_node.SelectedIndex = -1;
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            CountryTestModify test = new CountryTestModify();
            test.ShowDialog();

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = SysConfig.clientConfig.AllCountryTest;
        }

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

                CountryTestModify edit = new CountryTestModify();
                edit.UniqueId = Convert.ToInt32(row.Cells["UniqueId"].Value);

                edit.ShowDialog();

                dataGridView1.DataSource = null;
                dataGridView1.DataSource = SysConfig.clientConfig.AllCountryTest;

            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
            }
        }

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

                if (MessageBox.Show("您确定要删除这个检测点!", "确认", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }

                int uniqueId = int.Parse(row.Cells["UniqueId"].Value.ToString());
                string msg = "";
                if (SysConfig.clientConfig.DelCountryTest(uniqueId, ref msg))
                {                   
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = SysConfig.clientConfig.AllCountryTest;
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

        /// <summary>
        /// 查询 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_search_Click(object sender, EventArgs e)
        {
            List<XML_CountryTest> list = SysConfig.clientConfig.AllCountryTest;
            if (cmb_station.SelectedIndex != -1)
            {
                list = list.Where(c => c.StationUniqueId.ToString() == cmb_station.SelectedValue.ToString()).ToList();
            }
            if (cmb_node.SelectedIndex != -1)
            {
                list = list.Where(c => c.NodeId == cmb_node.SelectedValue.ToString()).ToList();
            }
            dataGridView1.DataSource = list;
        }
    }
}
