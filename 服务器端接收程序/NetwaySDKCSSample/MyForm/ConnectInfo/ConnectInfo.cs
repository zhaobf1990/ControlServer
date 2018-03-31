using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using 服务器端接收程序.Util;
using 服务器端接收程序;
using 服务器端接收程序.Clazz;
using Utility;
using 服务器端接收程序.Config;
using 服务器端接收程序.Clazz.Config;


namespace 服务器端接收程序.MyForm.ConnectInfo
{
    public partial class ConnectInfo : UserControl
    {
        private Thread thread_save_online_info;
        private Thread thread_update_gridview;


        public ConnectInfo()
        {
            InitializeComponent();

            dataGridView1.AutoGenerateColumns = false;

            // DTU_ClientManager.ClientChangeHander += ShowClientInfo;
            // ServerSocketHelper.ClientChangeHander += ShowClientInfo;   这两句代码更新的太快了，页面太卡了。


            //创建线程并开启
            thread_save_online_info = new Thread(new ThreadStart(SaveOnlineInfo));
            thread_save_online_info.IsBackground = true;
            thread_save_online_info.Start();

            //创建线程并开启
            thread_update_gridview = new Thread(new ThreadStart(ShowClientInfo));
            thread_update_gridview.IsBackground = true;
            thread_update_gridview.Start();
        }

        /// <summary>
        /// 显示当前在线的客户端信息
        /// </summary>
        private void ShowClientInfo()
        {
            while (true)
            {
                Thread.Sleep(10000);
                dataGridView1.BeginInvoke(new Action(BindData));
            }

        }

        /// <summary>
        /// 绑定数据
        /// </summary>
        private void BindData()
        {
            List<ClientInfo> list = new List<ClientInfo>();
            //这句代码会出现在枚举出错，在枚举的过程中，列表元素被改变了
            //list = DTU_ClientManager.Clients.Select(c =>
            //{
            //    return new ClientInfo
            //    {
            //        Tel = c.Tel,
            //        Name = c.Name,
            //        RegisterTime = c.RegisterTime,
            //        LastVisitTime = c.LastVisitTime
            //    };
            //}).ToList();
            int length = DTU_ClientManager.Clients.Count;
            for (int i = 0; i < length; i++)
            {
                try
                {
                    DTUClientInfo item = DTU_ClientManager.Clients[i];
                    list.Add(new ClientInfo() { StationId = item.StationId, Tel = item.TelOrGprsId, Name = item.Name, RegisterTime = item.RegisterTime, LastVisitTime = item.LastVisitTime });
                }
                catch (Exception)
                {
                }
            }

            list.AddRange(ServerSocketHelper.ClientSockets);

            dataGridView1.DataSource = list;

        }


        /// <summary>
        /// 保存客户端的连接信息到数据库
        /// </summary>
        private void SaveOnlineInfo()
        {
            while (true)
            {
                try
                {

                    foreach (XML_Org org in SysConfig.orgConfig.Orgs)
                    {
                        try
                        {
                            SWSDataContext db = new SWSDataContext(Util.ServerSocketHelper.GetConnection(org.DBName));


                            //采用有屏版电控柜的客户端遍历
                            List<Clazz.Config.ClientConfig.XML_Station> list_station = SysConfig.clientConfig.AllStation.Where(c => c.OrgId == org.OrgId).ToList();
                            for (int i = 0; i < list_station.Count; i++)
                            {

                                ClientInfo client_info = ServerSocketHelper.ClientSockets.SingleOrDefault(c => c.TransferCode == list_station[i].TransferCode);
                                if (client_info != null)
                                {
                                    station_online_info online_info = db.station_online_info.SingleOrDefault(c => c.transfer_code == list_station[i].TransferCode);
                                    if (online_info != null)
                                    {
                                        online_info.name = client_info.Name;
                                        online_info.register_time = client_info.RegisterTime;
                                        online_info.last_visit_time = client_info.LastVisitTime;
                                        online_info.stationid = client_info.StationId;
                                    }
                                    else
                                    {
                                        online_info = new station_online_info();
                                        online_info.transfer_code = client_info.TransferCode;
                                        online_info.name = client_info.Name;
                                        online_info.register_time = client_info.RegisterTime;
                                        online_info.last_visit_time = client_info.LastVisitTime;
                                        online_info.stationid = client_info.StationId;
                                        db.station_online_info.InsertOnSubmit(online_info);
                                    }
                                    List<station_online_info> deletes = db.station_online_info.Where(c => c.stationid == online_info.stationid && c.id != online_info.id).ToList();
                                    if (deletes.Count > 0)
                                    {
                                        db.station_online_info.DeleteAllOnSubmit(deletes);
                                    }
                                }
                            }

                            //采用DTU电控柜的客户端遍历
                            List<XML_Station> list_DTU_Station = SysConfig.DTU_StationConfig.Stations.Where(c => c.OrgId == org.OrgId).ToList();
                            for (int i = 0; i < list_DTU_Station.Count; i++)
                            {
                                DTUClientInfo DTU_clientinfo = DTU_ClientManager.Clients.SingleOrDefault(c => c.TelOrGprsId == list_DTU_Station[i].Tel);
                                if (DTU_clientinfo != null)
                                {
                                    station_online_info online_info = db.station_online_info.SingleOrDefault(c => c.dtu_tel == DTU_clientinfo.TelOrGprsId);
                                    if (online_info != null)
                                    {
                                        online_info.stationid = DTU_clientinfo.StationId;
                                        online_info.name = DTU_clientinfo.Name;
                                        online_info.register_time = DTU_clientinfo.RegisterTime;
                                        online_info.last_visit_time = DTU_clientinfo.LastVisitTime;
                                    }
                                    else
                                    {
                                        online_info = new station_online_info();
                                        online_info.stationid = DTU_clientinfo.StationId;
                                        online_info.name = DTU_clientinfo.Name;
                                        online_info.register_time = DTU_clientinfo.RegisterTime;
                                        online_info.last_visit_time = DTU_clientinfo.LastVisitTime;
                                        online_info.dtu_tel = DTU_clientinfo.TelOrGprsId;
                                        db.station_online_info.InsertOnSubmit(online_info);
                                    }
                                    List<station_online_info> deletes = db.station_online_info.Where(c => c.stationid == online_info.stationid && c.id != online_info.id).ToList();
                                    if (deletes.Count > 0)
                                    {
                                        db.station_online_info.DeleteAllOnSubmit(deletes);
                                    }
                                }
                            }
                            db.SubmitChanges();
                        }
                        catch (Exception ex)
                        {
                            LogMg.AddError("保存客户端在线数据时失败,\r\n" + ex.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogMg.AddError(ex);
                }

                Thread.Sleep(60000);
            }

        }

        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(e.RowBounds.Location.X, e.RowBounds.Location.Y, dataGridView1.RowHeadersWidth - 4, e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
                dataGridView1.RowHeadersDefaultCellStyle.Font,
                rectangle,
                dataGridView1.RowHeadersDefaultCellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }


    }
}
