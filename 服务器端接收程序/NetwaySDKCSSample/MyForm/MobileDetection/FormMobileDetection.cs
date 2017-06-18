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
using Utility;
using CSDataStandard.Enum;
using 服务器端接收程序.Config;

namespace 服务器端接收程序.MyForm.MobileDetection
{
    public partial class FormMobileDetection : UserControl
    {
        public FormMobileDetection()
        {
            InitializeComponent();

            ServerSocketHelper.MobileDetectionCallBack += RecieveCallBack;
        }

        //接收到客户端发来的数据,并向客户端返回消息
        private void RecieveCallBack(Socket RSocket, string msg)
        {
            try
            {
                //Socket RSocket = (Socket)ar.AsyncState;

                bool flag = this.Save(msg, RSocket);

                S_To_C_Data<object> s_to_c_data = new S_To_C_Data<object>();
                s_to_c_data.Flag = HandleFlag.MobileDetection;
                s_to_c_data.Success = flag;
                string json = Utility.JsonHelper.JsonSerializer<S_To_C_Data<object>>(s_to_c_data);

                RSocket.Send(Encoding.Unicode.GetBytes(json));
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex.ToString());
                DEBUG.ThrowException(ex);
            }
        }

        private bool Save(string json, Socket RSocket)
        {
            bool isSuccess = true;
            string stationName = "";   //站点名称
            C_To_S_Data<CSDataStandard.Transfer.RealRec> c_to_s_data = Utility.JsonHelper.JsonDeserialize<C_To_S_Data<CSDataStandard.Transfer.RealRec>>(json);



            Clazz.Config.XML_Org _org = SysConfig.orgConfig.GetOrgByOrgId(c_to_s_data.OrgId);

            if (_org == null)
            {
                //将信息写入到日志文件中    orgid为***的污水厂不存在 
                LogMg.AddError("OrgId:\"{0}\"不存在");
                isSuccess = false;
            }
            else
            {
                try
                {
                    SWSDataContext SWS = new SWSDataContext(Util.ServerSocketHelper.GetConnection(_org.DBName));     //建立一个分厂数据源提供程序实例

                    //查找站点名称
                    country_station _station = SWS.country_station.SingleOrDefault(c => c.id == c_to_s_data.StationId);
                    if (_station != null)
                    {
                        stationName = _station.name;    //站点名称
                    }

                    //遍历数据   并把数据添加到数据库中
                    List<testrec> list = new List<testrec>();
                    List<test> listTest = new List<test>();
                    foreach (CSDataStandard.Transfer.RealRec item in c_to_s_data.Data)
                    {
                        //判断检测当前数据在数据库中是否已经存在
                        if (SWS.realrec.SingleOrDefault(c => c.testid == item.TestId && c.testtime == item.TestTime) == null)
                        {
                            testrec _testrec = new testrec();
                            _testrec.testid = item.TestId;
                            _testrec.testtime = item.TestTime;
                            _testrec.value = (decimal)item.Value;
                            list.Add(_testrec);
                        }

                        //修改test表
                        SWS.ExecuteCommand(string.Format("update test set  [value]={0} where testid={1}", item.Value, item.TestId));

                    }

                    SWS.testrec.InsertAllOnSubmit(list);
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

            lock (lb_msg)
            {
                if (lb_msg.Items.Count > 200)
                    lb_msg.Items.Clear();
                this.lb_msg.Items.Add(string.Format("时间:{0}     客户端:{1}     数据行数{2}      保存{3}", DateTime.Now.ToString(), stationName, c_to_s_data.Data.Count, isSuccess ? "成功" : "失败"));
            }

            return isSuccess;
        }
    }
}
