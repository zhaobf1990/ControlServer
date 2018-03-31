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

namespace 服务器端接收程序.MyForm.Config
{
    public partial class DeviceControlConfig : UserControl
    {
        public DeviceControlConfig()
        {
            InitializeComponent();

            BindCmb();

            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = SysConfig.clientConfig.AllDevice;
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

        private void btn_search_Click(object sender, EventArgs e)
        {
            List<Clazz.Config.ClientConfig.XML_DeviceControl> list = SysConfig.clientConfig.AllDevice;
            if (cmb_station.SelectedIndex != -1)
            {
                list = list.Where(c => c.StationUniqueId.ToString() == cmb_station.SelectedValue.ToString()).ToList();
            }
            dataGridView1.DataSource = list;
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

                if (MessageBox.Show("您确定要删除这个控制点!", "确认", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }

                int uniqueId = int.Parse(row.Cells["UniqueId"].Value.ToString());
                string msg = "";
                if (SysConfig.clientConfig.DelDeviceControl(uniqueId, ref msg))
                {
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = SysConfig.clientConfig.AllDevice;
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
        /// 添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Add_Click(object sender, EventArgs e)
        {
            DeviceControlModify device = new DeviceControlModify();
            device.ShowDialog();

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = SysConfig.clientConfig.AllDevice;
        }

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

                DeviceControlModify edit = new DeviceControlModify();
                edit.UniqueId = Convert.ToInt32(row.Cells["UniqueId"].Value);

                edit.ShowDialog();

                dataGridView1.DataSource = null;
                dataGridView1.DataSource = SysConfig.clientConfig.AllDevice;

            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
            }
        }
    }
}
