using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using 服务器端接收程序.MyForm.GPRSControl;
using System.Threading;
using Utility;
using 服务器端接收程序.Config;

namespace 服务器端接收程序.Util
{
    class V88CommunicationThread
    {
        /// <summary>
        /// 考勤数据头
        /// </summary>
        public static byte ATT_HEAD = 0xA5;
        /// <summary>
        /// 考勤数据功能码
        /// </summary>
        public static byte ATT_FUNCTION_CODE = 0x84;

        static V88CommunicationThread instance;

        public static V88CommunicationThread getInstance()
        {
            if (instance == null)
            {
                instance = new V88CommunicationThread();
                instance.keyRegisterPairs.Add("k1", 1101);
                instance.keyRegisterPairs.Add("k2", 1102);
                instance.keyRegisterPairs.Add("k3", 1103);
                instance.keyRegisterPairs.Add("k4", 1104);
                instance.keyRegisterPairs.Add("k5", 1105);
                instance.keyRegisterPairs.Add("k6", 1106);
                instance.keyRegisterPairs.Add("k7", 1107);
                instance.keyRegisterPairs.Add("k8", 1108);
                instance.keyRegisterPairs.Add("k9", 1109);
                instance.keyRegisterPairs.Add("k10", 1110);
                instance.keyRegisterPairs.Add("k11", 1111);
                instance.keyRegisterPairs.Add("k12", 1112);
                instance.keyRegisterPairs.Add("k13", 1113);
                instance.keyRegisterPairs.Add("k14", 1114);
                instance.keyRegisterPairs.Add("k15", 1115);
                instance.keyRegisterPairs.Add("k16", 1116);
                instance.keyRegisterPairs.Add("k17", 1117);
                instance.keyRegisterPairs.Add("k18", 1118);
                instance.keyRegisterPairs.Add("k19", 1119);
                instance.keyRegisterPairs.Add("k20", 1120);
                instance.keyRegisterPairs.Add("k21", 1121);
                instance.keyRegisterPairs.Add("k22", 1122);
                instance.keyRegisterPairs.Add("k23", 1123);
                instance.keyRegisterPairs.Add("k24", 1124);
                instance.keyRegisterPairs.Add("k25", 1125);
                instance.keyRegisterPairs.Add("a1", 1201);
                instance.keyRegisterPairs.Add("a2", 1202);
                instance.keyRegisterPairs.Add("a3", 1203);
                instance.keyRegisterPairs.Add("a4", 1204);
                instance.keyRegisterPairs.Add("a5", 1205);
                instance.keyRegisterPairs.Add("a6", 1206);
                instance.keyRegisterPairs.Add("a7", 1207);
                instance.keyRegisterPairs.Add("a8", 1208);
                instance.keyRegisterPairs.Add("a9", 1209);
                instance.keyRegisterPairs.Add("a10", 1210);
                instance.keyRegisterPairs.Add("a11", 1211);
                instance.keyRegisterPairs.Add("a12", 1212);
                instance.keyRegisterPairs.Add("a13", 1213);
                instance.keyRegisterPairs.Add("a14", 1214);
                instance.keyRegisterPairs.Add("a15", 1215);
                instance.keyRegisterPairs.Add("a16", 1216);
                instance.keyRegisterPairs.Add("a17", 1217);
                instance.keyRegisterPairs.Add("a18", 1218);
                instance.keyRegisterPairs.Add("a19", 1219);
                instance.keyRegisterPairs.Add("a20", 1220);
                instance.keyRegisterPairs.Add("a21", 1221);
                instance.keyRegisterPairs.Add("a22", 1222);
                instance.keyRegisterPairs.Add("a23", 1223);
                instance.keyRegisterPairs.Add("a24", 1224);
                instance.keyRegisterPairs.Add("a25", 1225);
                instance.keyRegisterPairs.Add("p1", 1301);
                instance.keyRegisterPairs.Add("p2", 1302);
                instance.keyRegisterPairs.Add("p3", 1303);
                instance.keyRegisterPairs.Add("p4", 1304);
                instance.keyRegisterPairs.Add("p5", 1305);
                instance.keyRegisterPairs.Add("p6", 1306);
                instance.keyRegisterPairs.Add("p7", 1307);
                instance.keyRegisterPairs.Add("p8", 1308);
                instance.keyRegisterPairs.Add("p9", 1309);
                instance.keyRegisterPairs.Add("p10", 1310);
                instance.keyRegisterPairs.Add("p11", 1311);
                instance.keyRegisterPairs.Add("p12", 1312);
                instance.keyRegisterPairs.Add("p13", 1313);
                instance.keyRegisterPairs.Add("p14", 1314);
                instance.keyRegisterPairs.Add("p15", 1315);
                instance.keyRegisterPairs.Add("p16", 1316);
                instance.keyRegisterPairs.Add("p17", 1317);
                instance.keyRegisterPairs.Add("p18", 1318);
                instance.keyRegisterPairs.Add("p19", 1319);
                instance.keyRegisterPairs.Add("p20", 1320);
                instance.keyRegisterPairs.Add("p21", 1321);
                instance.keyRegisterPairs.Add("p22", 1322);
                instance.keyRegisterPairs.Add("p23", 1323);
                instance.keyRegisterPairs.Add("p24", 1324);
                instance.keyRegisterPairs.Add("p25", 1325);
                instance.keyRegisterPairs.Add("h", 1401);
            }
            return instance;
        }

        /// <summary>
        /// 线程池
        /// </summary>
        public TaskPool PoolE;

        /// <summary>
        /// 开始这个类的任务     初始化线程池，并开始【分配任务】的线程
        /// </summary>
        public void Start()
        {
            PoolE = new TaskPool();
            PoolE.Name = "E";
            PoolE.Setminthread(SysConfig.userProfile.V88ThreadPool_Min);
            PoolE.Setmaxthread(SysConfig.userProfile.V88ThreadPool_Max);
        }

        public List<SWS_DB.guangdai_station_link> V88StationLink = new List<SWS_DB.guangdai_station_link>();
        public Dictionary<string, Int32> keyRegisterPairs = new Dictionary<string, Int32>();

        public void readConfig()
        {
            SWS_DB.SWS_DBDataContext db = new SWS_DB.SWS_DBDataContext(ConnectStringHelper.GetConnection(SysConfig.userProfile.DbAddress, SysConfig.userProfile.DbName, SysConfig.userProfile.DbUserName, SysConfig.userProfile.DbPassword));
            V88StationLink = db.guangdai_station_link.Where(c => c.type == 3).ToList();

        }


        public void receiveTask(byte[] msg)
        {
            PoolE.AddTaskItem(new WaitCallback(handlerMsg), new data(msg));
        }

        private void handlerMsg(object msg)
        {
            data data = (data)msg;
            if (isV88Data(data.bytes))
            {
                handlerV88Msg(Encoding.ASCII.GetString(data.bytes, 0, data.bytes.Length));
            }
            else if (isAttendance(data.bytes))
            {
                handlerAttMsg(data.bytes);
            }
            else
            {
                saveLog(data.bytes);
            }


        }

        private void saveLog(byte[] bytes)
        {
            LogMg.AddDebug("udp协议接收的其他数据=============     " + ToHexString(bytes));
        }

        /// <summary>
        /// 处理考勤数据
        /// </summary>
        /// <param name="bytes"></param>
        private void handlerAttMsg(byte[] bytes)
        {
            try
            {
                LogMg.AddDebug("udp协议接收的考勤数据=============     " + ToHexString(bytes));
                int stationNo = 0;
                int pid = 0;
                int state = 0;
                DateTime date = DateTime.Now;
                parseDataAtt(bytes, ref stationNo, ref pid, ref state, ref date);
                //找出编号对应的站点
                SWS_DB.guangdai_station_link link = V88StationLink.SingleOrDefault(c => Int32.Parse(c.wsid) == stationNo);
                if (link == null)
                {
                    return;
                }
               
                SWSDataContext sws = new SWSDataContext(ConnectStringHelper.GetConnection(SysConfig.userProfile.DbAddress, link.db_name, SysConfig.userProfile.DbUserName, SysConfig.userProfile.DbPassword));
                updateStationLine(sws, (int)link.station_id);
                users user = findUserByPid(sws, pid);
                if (user == null) return;
                country_attendance att = sws.country_attendance.Where(c => c.RFID == pid.ToString() && c.station_id == link.station_id && c.edate != null && DateTime.Now.AddHours(-1).CompareTo(c.edate) < 0).OrderByDescending(c => c.id).FirstOrDefault();
                if (att == null)
                {
                    att = new country_attendance();
                    if (user == null)
                    {
                        att.userid = 0;
                    }
                    else
                    {
                        att.userid = user.userid;
                    }
                    att.station_id = link.station_id;
                    att.sdate = date;
                    att.edate = date;
                    att.type = "01";
                    att.remark = "V88设备的考勤数据";
                    att.RFID = pid.ToString();
                    sws.country_attendance.InsertOnSubmit(att);
                    sws.SubmitChanges();
                }
                else
                {
                    att.edate = date;
                    sws.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                LogMg.AddError(ex);
            }
        }

        ///// <summary>
        ///// 处理考勤数据
        ///// </summary>
        ///// <param name="bytes"></param>
        //private void handlerAttMsg(byte[] bytes)
        //{
        //    try
        //    {
        //        LogManager.AddDebug("udp协议接收的考勤数据=============     " + ToHexString(bytes));
        //        int stationNo = 0;
        //        int pid = 0;
        //        int state = 0;
        //        parseDataAtt(bytes, ref stationNo, ref pid, ref state);
        //        //找出编号对应的站点
        //        SWS_DB.guangdai_station_link link = V88StationLink.SingleOrDefault(c => Int32.Parse(c.wsid) == stationNo);
        //        if (link == null)
        //        {
        //            return;
        //        }
        //        SWSDataContext sws = new SWSDataContext(ConnectStringHelper.GetConnection(SysConfig.userProfile.DbAddress, link.db_name, SysConfig.userProfile.DbUserName, SysConfig.userProfile.DbPassword));
        //        users user = findUserByPid(sws, pid);
        //        if (state == 1)
        //        {
        //            country_attendance att = new country_attendance();
        //            if (user == null)
        //            {
        //                att.userid = 0;
        //            }
        //            else
        //            {
        //                att.userid = user.userid;
        //            }
        //            att.station_id = link.station_id;
        //            att.sdate = DateTime.Now;
        //            att.edate = DateTime.Now;
        //            att.type = "1";
        //            att.remark = "V88设备的考勤数据";
        //            att.RFID = pid.ToString();
        //            sws.country_attendance.InsertOnSubmit(att);
        //            sws.SubmitChanges();
        //        }
        //        else if (state == 0)
        //        {
        //            country_attendance att = null;
        //            if (user == null)
        //            { 
        //                att = sws.country_attendance.Where(c => c.RFID != null && c.RFID == pid.ToString())
        //                    .OrderByDescending(c => c.id).FirstOrDefault();
        //            }
        //            else
        //            {
        //                att = sws.country_attendance.Where(c => c.userid == user.userid).OrderByDescending(c => c.id).FirstOrDefault();
        //            }
        //            if (att == null)
        //            {
        //                return;
        //            }
        //            att.edate = DateTime.Now;
        //            sws.SubmitChanges();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.AddError(ex);
        //    }
        //}

        /// <summary>
        /// 根据pid找用户
        /// </summary>
        /// <param name="sws"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        private users findUserByPid(SWSDataContext sws, int pid)
        {
            users user = null;
            List<users> list = sws.users.ToList();
            if (list != null)
            {
                foreach (users item in list)
                {
                    try
                    {
                        if (item.RFID != null)
                        {
                            if (item.RFID.Contains("<" + pid + ">"))
                            {
                                user = item;
                                break;
                            }
                            else if (Int64.Parse(item.RFID) == pid)
                            {
                                user = item;
                                break;
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                    }
                }

            }
            return user;
        }

        private void handlerV88Msg(string data)
        {
            LogMg.AddDebug("udp协议接收的数据=============     " + data);
            List<String> datas = CaiBao(data);
            DateTime date = DateTime.Now;
            List<Dictionary<string, string>> listDir = parseDataV88(datas, ref date);

            foreach (Dictionary<string, string> item in listDir)
            {
                try
                {   //找出设备编号 
                    String stationNo = getStationNo(item);
                    if (String.IsNullOrEmpty(stationNo))
                    {
                        continue;
                    }
                    //找出编号对应的站点
                    SWS_DB.guangdai_station_link link = V88StationLink.SingleOrDefault(c => c.wsid == stationNo);
                    if (link == null)
                    {
                        continue;
                    }
                    SWSDataContext sws = new SWSDataContext(ConnectStringHelper.GetConnection(SysConfig.userProfile.DbAddress, link.db_name, SysConfig.userProfile.DbUserName, SysConfig.userProfile.DbPassword));
                    updateStationLine(sws, (int)link.station_id);
                    foreach (string key in item.Keys)
                    {
                        //数据
                        KeyValuePair<string, string> data1 = item.Single(c => c.Key == key);
                        try
                        {
                            KeyValuePair<string, int>? keyAndValue = getRegisterByKey(key);
                            if (keyAndValue == null)
                            {
                                continue;
                            }
                            List<gong_kuang_config> gks = sws.gong_kuang_config.Where(c => c.station_id == link.station_id && c.read_register == ((KeyValuePair<string, int>)keyAndValue).Value.ToString()).ToList();
                            foreach (gong_kuang_config gk in gks)
                            {
                                if (gk == null || gk.testid == null || gk.testid == 0)
                                {
                                    continue;
                                }
                                AutoCollectionThread.updateStationOnlineInfo(sws, (int)link.station_id);
                                //保存数据
                                double multiple = gk.Multiple == null ? 1 : (double)gk.Multiple;
                                double addNumber = gk.AddNumber == null ? 0 : (double)gk.AddNumber;
                                String strValue = data1.Value;
                                if (key.StartsWith("k") || key.StartsWith("K"))
                                {
                                    if (strValue == "1")
                                    {
                                        strValue = "0";
                                    }
                                    else
                                    {
                                        strValue = "1";
                                    }
                                }
                                double value = Double.Parse(strValue) * multiple + addNumber;
                                gk.read_value = value.ToString();
                                realrec rec = new realrec();
                                rec.testid = gk.testid;
                                rec.value = (decimal)value;
                                rec.testtime = date;
                                rec.remark = "V88协议的设备数据";
                                sws.realrec.InsertOnSubmit(rec);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogMg.AddError(ex);
                        }
                    }
                    sws.SubmitChanges();
                }
                catch (Exception ex)
                {
                    LogMg.AddError(ex);
                }
            }
        }

        private KeyValuePair<string, int>? getRegisterByKey(string key)
        {
            KeyValuePair<string, int> keyValue = keyRegisterPairs.SingleOrDefault(c => c.Key == key);
            if (!String.IsNullOrEmpty(keyValue.Key))
            {
                return keyValue;
            }
            else
            {
                return null;
            }
        }

        private string getStationNo(Dictionary<string, string> item)
        {
            KeyValuePair<string, string> dir = item.SingleOrDefault(c => c.Key == "st");
            if (!String.IsNullOrEmpty(dir.Key))
            {
                return dir.Value;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 可能会有多个包连在一起,需要调用这个方法拆包
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static List<String> CaiBao(string data)
        {
            List<String> datas = new List<string>();

            if (!String.IsNullOrEmpty(data))
            {
                data = data.ToLower();
                string[] list = data.Split(new string[] { "st:" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < list.Length; i++)
                {
                    if (!list[i].StartsWith("st:"))
                    {
                        list[i] = "st:" + list[i];
                    }
                    if (!String.IsNullOrEmpty(list[i]) && list[i].EndsWith("."))
                    {
                        list[i] = list[i].Substring(0, list[i].Length - 1);
                    }
                    Console.WriteLine(list[i]);
                }
                datas = list.ToList();
            }
            return datas;
        }


        /// <summary>
        /// 将数据解析成键值对
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        private List<Dictionary<string, string>> parseDataV88(List<String> datas, ref DateTime date)
        {
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            foreach (string one in datas)
            {
                Dictionary<string, string> dir = new Dictionary<string, string>();
                try
                {
                    int first = one.IndexOf(";") + 1;
                    int two = one.IndexOf(";", first);
                    date = DateTime.Parse(one.Substring(first, two - first));
                }
                catch (Exception e)
                {
                    LogMg.AddError(e);
                }

                string[] arr = one.Split(';');
                for (int i = 0; i < arr.Length; i++)
                {
                    if (i != 1)
                    {
                        string[] arr2 = arr[i].Split(':');
                        //  Console.WriteLine("key=" + arr2[0] + "   value=" + arr2[1]);
                        dir.Add(arr2[0], arr2[1]);
                    }
                }
                Console.WriteLine();
                list.Add(dir);
            }
            return list;
        }

        /// <summary>
        /// 解析考勤数据
        /// </summary>
        /// <param name="content"></param>
        /// <param name="pid"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static bool parseDataAtt(byte[] content, ref int stationNo, ref int pid, ref int state, ref DateTime date)
        {
            try
            {
                stationNo = (content[4] << 8) + content[5];
                pid = (content[13] << 8) + content[14];
                state = content[15];
                String strDate = content[7] + "/" + content[8] + "/" + content[9] + " " + content[10] + ":" + content[11] + ":" + content[12];
                date = DateTime.Parse(strDate);
                Console.WriteLine(strDate);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 是否是考勤数据
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool isAttendance(byte[] content)
        {
            try
            {
                if (content[0] == ATT_HEAD && content[1] == ATT_HEAD && content[6] == ATT_FUNCTION_CODE)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 判断是否是V88协议数据 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool isV88Data(byte[] content)
        {
            string message = Encoding.ASCII.GetString(content, 0, content.Length);
            try
            {
                if (message.StartsWith("ST:") || message.StartsWith("st:"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

        }

        public static void updateStationLine(SWSDataContext sws, int stationId)
        {
            try
            {
                test test = sws.test.SingleOrDefault(c => c.station_id == stationId && c.test_code == 22);
                test.value = "0";
                sws.SubmitChanges();
            }
            catch (Exception e)
            {
                LogMg.AddError(e);
            }
        }

        /// <summary>
        /// 转16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToHexString(byte[] bytes) // 0xae00cf => "AE00CF "
        {
            string hexString = string.Empty;

            if (bytes != null)
            {

                StringBuilder strB = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {

                    strB.Append(bytes[i].ToString("X2") + " ");

                }

                hexString = strB.ToString();

            } return hexString;

        }

        public class data
        {
            public byte[] bytes;

            public data(byte[] data)
            {
                this.bytes = data;
            }
        }


    }
}
