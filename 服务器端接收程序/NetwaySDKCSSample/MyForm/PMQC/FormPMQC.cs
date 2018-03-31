using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using Utility;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using 服务器端接收程序.Util;
using CSDataStandard;
using CSDataStandard.Enum;
using 服务器端接收程序.Config;

namespace 服务器端接收程序.MyForm.PMQC
{
    public partial class FormPMQC : UserControl
    {
        private Dictionary<string, Socket> dirSocket = new Dictionary<string, Socket>();

        public FormPMQC()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;//不捕获对错误线程的调用

            ServerSocketHelper.PMQCCallBack += RecieveCallBack;
        }

        private void FormPMQC_Load(object sender, EventArgs e)
        {

        }


        //接收到客户端发来的数据,并向客户端返回消息
        private void RecieveCallBack(Socket RSocket, string msg)
        {
            try
            {
                //Socket RSocket = (Socket)ar.AsyncState;

                bool flag = this.Save(msg);   //序列化json  并保存到数据库

                S_To_C_Data<object> s_to_c_data = new S_To_C_Data<object>();
                s_to_c_data.Flag = HandleFlag.PMQC;
                s_to_c_data.Success = flag;
                string json = Utility.JsonHelper.JsonSerializer<S_To_C_Data<object>>(s_to_c_data);

                RSocket.Send(Encoding.Unicode.GetBytes(json), SocketFlags.None);  //保存成功返回1    ,反之则0
                //把消息输出到页面上
                lb_msg.Items.Add(flag ? "保存成功" : "保存失败");
                //同时接收客户端回发的数据，用于回发
                //RSocket.BeginReceive(MsgBuffer, 0, MsgBuffer.Length, 0, new AsyncCallback(RecieveCallBack), RSocket);
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
                DEBUG.ThrowException(ex);
            }
        }

        /// <summary>
        /// 序列化json  并保存到数据库
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private bool Save(string json)
        {

            C_To_S_Data<CSDataStandard.Transfer.RealRec> c_to_s_data = Utility.JsonHelper.JsonDeserialize<C_To_S_Data<CSDataStandard.Transfer.RealRec>>(json);
            lb_msg.Items.Add(string.Format("污水厂编号:{0}    接收数据行数:{1}", c_to_s_data.OrgId, c_to_s_data.Data.Count));

            try
            {
                SWSDataContext SWS = new SWSDataContext(getConByOrgId(c_to_s_data.OrgId));
                //循环从客户端接收到的数据
                foreach (CSDataStandard.Transfer.RealRec item in c_to_s_data.Data)
                {
                    //根据testid找出test记录信息
                    test _test = SWS.test.SingleOrDefault(c => c.testid == item.TestId);
                    if (_test != null)
                    {
                        //把数据转换成数值型
                        double top1 = 0, top2 = 0, btm1 = 0, btm2 = 0;
                        try
                        {
                            top1 = Convert.ToDouble(_test.toplimit1);
                            top2 = Convert.ToDouble(_test.toplimit2);
                            btm1 = Convert.ToDouble(_test.btmlimit1);
                            btm2 = Convert.ToDouble(_test.btmlimit2);
                        }
                        catch (Exception ex)
                        {
                            LogMg.AddError(ex);
                            //throw ex;
                        }

                        char status = '1';
                        //拿检测到的值和 提示上限 提示下限 警报上限 警报下限做比较 
                        if (item.Value >= btm1 && item.Value <= top1)
                        {
                            status = '1';
                        }
                        else if ((item.Value > top1 && item.Value < top2) || (item.Value > btm2 && item.Value < btm1))
                        {
                            status = '2';
                        }
                        else
                        {
                            status = '3';
                        }
                        //修改test表
                        SWS.ExecuteCommand(string.Format("update test set testtime='{0}' , [value]={1} , status='{2}' , means='{3}' where testid={4}", item.TestTime, item.Value, status, "屏幕取词", item.TestId));
                        //插入数据到realrec
                        SWS.ExecuteCommand(string.Format("insert into realrec (testid,testtime,[value],status) values({0},'{1}',{2},'{3}')", item.TestId, item.TestTime, item.Value, status));
                    }
                }
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
                DEBUG.ThrowException(ex);
                return false;
            }

            return true;
        }

        private SqlConnection getConByOrgId(string orgId)
        {


            string dbname = "";
            try
            {
                Clazz.Config.XML_Org _org = SysConfig.orgConfig.GetOrgByOrgId(orgId);
                dbname = _org.DBName;
            }
            catch (Exception ex)
            {
                LogMg.AddError("orgId:" + orgId + System.Environment.NewLine + ex.ToString());
                DEBUG.ThrowException(ex);
            }

            return Util.ServerSocketHelper.GetConnection(dbname);     //返回分厂数据库连接
        }
    }
}
