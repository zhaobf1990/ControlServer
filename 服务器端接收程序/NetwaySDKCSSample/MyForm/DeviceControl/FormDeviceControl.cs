using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using 服务器端接收程序.Util;
using System.Net.Sockets;
using CSDataStandard;

using CSDataStandard.Enum;
using Utility;
using 服务器端接收程序.Config;

namespace 服务器端接收程序.MyForm.DeviceControl
{
    public partial class FormDeviceControl : UserControl
    {

        public FormDeviceControl()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;

            ServerSocketHelper.DeviceControlCallBack += DeviceControlCallBack;
        }

        private void DeviceControlCallBack(Socket socket, string json)
        {

            C_To_S_Data<CSDataStandard.Transfer.DeviceControl> receiveObj = Utility.JsonHelper.JsonDeserialize<C_To_S_Data<CSDataStandard.Transfer.DeviceControl>>(json);

            //获取所有Org
            Clazz.Config.XML_Org org = SysConfig.orgConfig.GetOrgByOrgId(receiveObj.OrgId);
            if (org == null)  //判断Org是否存在
            {
                string msg = "OrgId:" + receiveObj.OrgId + "不存在";
                LogMg.AddError(msg);
                lb_msg.Items.Add(msg);
            }
            else
            {
                try
                {
                    SWSDataContext db = new SWSDataContext(ServerSocketHelper.GetConnection(org.DBName));
                    string stationName = "未知的客户端";
                    country_station station = db.country_station.SingleOrDefault(c => c.id == receiveObj.StationId);
                    if (station != null)
                        stationName = station.name;

                    saveData(db, receiveObj, stationName);

                    sendData(db, socket, stationName, receiveObj.StationId);

                }
                catch (Exception ex)
                {
                    LogMg.AddError(ex);
                    DEBUG.MsgBox(ex.ToString());
                }
            }

        }

        /// <summary>
        /// 保存客户端发来的设备控制信息    ,把状态字段更新到数据库
        /// </summary>
        /// <param name="db"></param>
        /// <param name="receiveObj"></param>
        private void saveData(SWSDataContext db, C_To_S_Data<CSDataStandard.Transfer.DeviceControl> receiveObj, string stationName)
        {
            try
            {
                //接收客户端数据
                foreach (CSDataStandard.Transfer.DeviceControl item in receiveObj.Data)
                {
                    device_control device_control = db.device_control.Where(c => c.id == item.DeviceControlId).SingleOrDefault();
                    if (device_control != null)
                    {                   
                        device_control.execute_result = item.ExecuteResult;
                    }
                }
                db.SubmitChanges();
                //输出消息
               // string msg = string.Format("保存【{0}】客户端发送来的{1}行设备控制执行结果", stationName, receiveObj.Data.Count);
               // lb_msg.BeginInvoke(new Action<string>(printMsg), msg);
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
            }

        }

        private void sendData(SWSDataContext db, Socket socket, string stationName, int stationId)
        {
            //发送设备控制信息到客户端 
            C_To_S_Data<CSDataStandard.Transfer.DeviceControl> sendObj = new C_To_S_Data<CSDataStandard.Transfer.DeviceControl>();
            sendObj.Flag = HandleFlag.DeviceControl;
            sendObj.Data = new List<CSDataStandard.Transfer.DeviceControl>();


            // List<device_control> listDeviceControl = db.device_control.Where(c => c.state == "0" && c.station_id == stationId).ToList();   //0表示未发送
            //把那时不需要发送到客户端的记录的发送状态都置为2
            db.ExecuteCommand("UPDATE  dbo.device_control SET state=2 WHERE station_id=" + stationId + " AND state=0 AND id NOT IN (SELECT MAX(id) AS datetime FROM dbo.device_control WHERE station_id=" + stationId + " AND state=0 GROUP BY testid,gong_kuang_id)");
            //找出需要发送的几行数据 
            List<device_control> listDeviceControl = db.ExecuteQuery<device_control>("SELECT * FROM dbo.device_control WHERE id IN (SELECT MAX(id) AS datetime FROM dbo.device_control WHERE station_id=" + stationId + " AND state=0 GROUP BY testid,gong_kuang_id)").ToList();

            //device_control device_control = listDeviceControl.OrderByDescending(c => c.datetime).FirstOrDefault();
            foreach (device_control device_control in listDeviceControl)
            {
                try
                {
                    gong_kuang_config gongKuang = db.gong_kuang_config.SingleOrDefault(c => c.id == device_control.gong_kuang_id);
                    CSDataStandard.Transfer.DeviceControl transferDeviceControl = new CSDataStandard.Transfer.DeviceControl();
                    transferDeviceControl.Value = device_control.control_code;  //控制的值 
                    transferDeviceControl.Address = (int)gongKuang.address;
                    transferDeviceControl.Register = gongKuang.write_register;
                    transferDeviceControl.DeviceControlId = device_control.id;   //设备控制表的id
                    //transferDeviceControl.DeviceNumber = device_control.device_number;
                    //transferDeviceControl.Datetime = Convert.ToDateTime(device_control.datetime);
                    //transferDeviceControl.ControlCode = device_control.control_code;
                    sendObj.Data.Add(transferDeviceControl);
                }
                catch (Exception ex)
                {
                    LogMg.AddError(ex);
                }
            }
            //更改设备控制表中的数据为已发送状态
            foreach (device_control item in listDeviceControl)
            {
                item.state = "1";
            }
            db.SubmitChanges();    //保存

            //把对象转换为json字符串
            string sendJson = Utility.JsonHelper.JsonSerializer<C_To_S_Data<CSDataStandard.Transfer.DeviceControl>>(sendObj);
            Byte[] msgSend = Encoding.Unicode.GetBytes(sendJson);
            socket.Send(msgSend, msgSend.Length, SocketFlags.None);
        //显示到UI界面
           // string msg = string.Format("发送{0}行数据到【{1}】客户端", sendObj.Data.Count, stationName);
          //  lb_msg.BeginInvoke(new Action<string>(printMsg), msg);
        }

        /// <summary>
        /// 输出消息
        /// </summary>
        /// <param name="msg"></param>
        private void printMsg(string msg)
        {
            ///如果行数大于200行   则清空
            if (lb_msg.Items.Count > 200)
            {
                lb_msg.Items.Clear();
            }
            ///添加消息
            lb_msg.Items.Add("【" + DateTime.Now.ToString() + "】" + msg);
        }

    }
}
