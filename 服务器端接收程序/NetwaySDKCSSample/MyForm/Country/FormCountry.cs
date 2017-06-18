using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.Data.SqlClient;
using Utility;
using 服务器端接收程序.Util;
using CSDataStandard;
using CSDataStandard.Enum;
using 服务器端接收程序.Config;


namespace 服务器端接收程序.MyForm.Country
{
    public partial class FormCountry : UserControl
    {


        public FormCountry()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;//不捕获对错误线程的调用

            ServerSocketHelper.CountryCallBack += RecieveCallBack;

        }

        //页面加载 
        private void FormCountry_Load(object sender, EventArgs e)
        {

        }

        //接收到客户端发来的数据,并向客户端返回消息
        private void RecieveCallBack(Socket RSocket, string msg)
        {
            try
            {
                //Socket RSocket = (Socket)ar.AsyncState;

                bool flag = this.Save(msg, RSocket);

                S_To_C_Data<object> s_to_c_data = new S_To_C_Data<object>();
                s_to_c_data.Flag = HandleFlag.Country;
                s_to_c_data.Success = flag;
                string json = Utility.JsonHelper.JsonSerializer<S_To_C_Data<object>>(s_to_c_data);

                RSocket.Send(Encoding.Unicode.GetBytes(json));

            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
                DEBUG.ThrowException(ex);
            }
        }

        /// <summary>
        /// 将json字符串转换成对象, 再把对象保存到数据库中
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private bool Save(string json, Socket socket)
        {
            bool isSuccess = true;
            string stationName = "";   //分厂名称
            C_To_S_Data<CSDataStandard.Transfer.RealRec> c_to_s_data = Utility.JsonHelper.JsonDeserialize<C_To_S_Data<CSDataStandard.Transfer.RealRec>>(json);

            Clazz.Config.XML_Org _org = SysConfig.orgConfig.GetOrgByOrgId(c_to_s_data.OrgId);

            if (_org == null)
            {
                //将信息写入到日志文件中    orgid为***的污水厂不存在 
                LogMg.AddError(string.Format("OrgId:{0}不存在", c_to_s_data.OrgId));

                isSuccess = false;
            }
            else
            {
                try
                {
                    SWSDataContext SWS = new SWSDataContext(Util.ServerSocketHelper.GetConnection(_org.DBName));     //建立一个分厂数据源提供程序实例

                    //查找站点名称
                    country_station _station = SWS.country_station.SingleOrDefault(c => c.id == c_to_s_data.StationId);

                    //更新站点的IP
                    _station.ip = ((System.Net.IPEndPoint)socket.RemoteEndPoint).Address.ToString();

                    if (_station != null)
                    {
                        stationName = _station.name;    //站点名称
                    }

                    //遍历数据   并把数据添加到数据库中
                    List<realrec> listrealrec = new List<realrec>();
                    List<testrec> listtestrec = new List<testrec>();
                    foreach (CSDataStandard.Transfer.RealRec item in c_to_s_data.Data)
                    {
                        test test = SWS.test.SingleOrDefault(c => c.testid == item.TestId);
                        if (test == null)
                        {
                            LogMg.AddError(string.Format("testid为 {0} 的检测点不存在", item.TestId));    //记录日志
                            isSuccess = false;
                        }
                        else
                        {
                            if (test.means.Trim() == "屏幕取词" || test.means.Trim() == "自动获取")
                            {
                                realrec _realrec = new realrec();
                                _realrec.testid = item.TestId;
                                _realrec.testtime = item.TestTime;
                                _realrec.value = (decimal)item.Value;
                                listrealrec.Add(_realrec);
                            }
                            if (test.means.Trim() == "检测录入")
                            {
                                testrec _testrec = SWS.testrec.SingleOrDefault(c => c.testid == item.TestId && c.testtime == item.TestTime);
                                //判断检测当前数据在数据库中是否已经存在
                                if (_testrec == null)
                                {
                                    _testrec = new testrec();
                                    _testrec.testid = item.TestId;
                                    _testrec.testtime = item.TestTime;
                                    _testrec.value = (decimal)item.Value;
                                    listtestrec.Add(_testrec);
                                }
                                else
                                {
                                    _testrec.testid = item.TestId;
                                    _testrec.testtime = item.TestTime;
                                    _testrec.value = (decimal)item.Value;
                                }
                            }
                        }
                    }

                    SWS.realrec.InsertAllOnSubmit(listrealrec);
                    SWS.testrec.InsertAllOnSubmit(listtestrec);

                    SWS.SubmitChanges();
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    isSuccess = false;
                    //把错误信息输出到日志文件中
                    LogMg.AddError(ex.ToString());
                    DEBUG.ThrowException(ex);
                }
            }

            //lock (lb_msg)
            //{
            //    if (lb_msg.Items.Count > 200)
            //       lb_msg.Items.Clear();
            //   this.lb_msg.Items.Add(string.Format("时间:{0}     客户端:{1}     数据行数{2}      保存{3}", DateTime.Now.ToString(), stationName, c_to_s_data.Data.Count, isSuccess ? "成功" : "失败"));
            // }

            SaveClientIp(socket, c_to_s_data);

            return isSuccess;
        }

        /// <summary>
        /// 保存客户端的IP地址
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="c_to_s_data"></param>
        public void SaveClientIp(Socket socket, C_To_S_Data<CSDataStandard.Transfer.RealRec> c_to_s_data)
        {
            IPEndPoint ipEndPoint = (IPEndPoint)socket.RemoteEndPoint;
            string ip = ipEndPoint.Address.ToString();

            Clazz.Config.XML_Org _org = SysConfig.orgConfig.GetOrgByOrgId(c_to_s_data.OrgId);

            if (_org == null)
            {
                //将信息写入到日志文件中    orgid为***的污水厂不存在 
                LogMg.AddError(String.Format("OrgId:{0}  不存在", c_to_s_data.OrgId));
                //isSuccess = false;
            }
            else
            {
                try
                {
                    SWSDataContext SWS = new SWSDataContext(ServerSocketHelper.GetConnection(_org.DBName));     //建立一个分厂数据源提供程序实例
                    country_station station = SWS.country_station.SingleOrDefault(c => c.id == c_to_s_data.StationId);
                    if (station == null)
                    {
                        LogMg.AddError("StationId: " + c_to_s_data.StationId + " 不存在");
                    }
                    else
                    {

                        station.ip = ip;     //保存客户端IP地址
                        SWS.SubmitChanges();
                    }
                }
                catch (Exception ex)
                {
                    LogMg.AddError(ex);
                }
            }
        }
    }
}
