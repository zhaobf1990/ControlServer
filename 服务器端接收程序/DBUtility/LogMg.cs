using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Collections;
using System.Threading;

namespace Utility
{
    /// <summary>
    /// 日志类
    /// </summary>
    public class LogMg
    {
        private static readonly string DEBUG = "debug";
        private static readonly string ERROR = "error";
        private static readonly string INFO = "info";

        private static string dir = AppDomain.CurrentDomain.BaseDirectory + @"../../Log/";
        private static Queue<string> queueLog = new Queue<string>();
        //private static Thread thread = new Thread(new ThreadStart(WriteLogHandler));

        public static void AddInfo(String str) {
            Write(str, INFO);
        }

        /// <summary>
        /// 添加调试日志
        /// </summary>
        /// <param name="str"></param>
        public static void AddDebug(string str)
        {
            if (ConfigurationManager.AppSettings["LogLevel"].ToString().ToLower() == DEBUG)
            {
                Write(str, DEBUG);
            }
        }

        /// <summary>
        /// 添加错误日志
        /// </summary>
        /// <param name="str"></param>
        public static void AddError(string str)
        {
            string logLevel = ConfigurationManager.AppSettings["LogLevel"].ToString().ToLower();
            if (logLevel == ERROR || logLevel == DEBUG)
            {
                Write(str, ERROR);
            }
        }

        /// <summary>
        /// 添加错误日志
        /// </summary>
        /// <param name="ex"></param>
        public static void AddError(Exception ex)
        {
            AddError(ex.ToString());
        }

        /// <summary>
        /// 获取日志路径 
        /// </summary>
        /// <returns></returns>
        private static string getPath(string level)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string path = Path.Combine(dir, date + level + ".txt");  //拼接路径 

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            if (!File.Exists(path))
            {
                FileStream fs = File.Create(path);
                fs.Close();
            }
            return path;
        }

        ///// <summary>
        ///// 将信息写入日志文件
        ///// </summary>
        ///// <param name="message"></param>
        //private static void WriteLogHandler()
        //{
        //    while (true)
        //    {
        //        lock (queueLog)
        //        {
        //            int count = queueLog.Count;
        //            for (int i = 0; i < count; i++)
        //            {
        //                Write(queueLog.Dequeue());
        //            }
        //        }
        //        thread.Suspend();
        //    }
        //}

        /// <summary>
        /// 将信息写入日志文件
        /// </summary>
        /// <param name="message"></param>
        ///  /// <param name="level">日志级别</param>
        private static void Write(string message, string level)
        {
            try
            {
                StreamWriter sw = File.AppendText(getPath(level));
                sw.WriteLine("【" + DateTime.Now.ToString() + "】：" + message + System.Environment.NewLine);
                sw.Close();
            }
            catch (Exception)
            {

            }

        }

        ///// <summary>
        ///// 日志开关
        ///// </summary>
        ///// <returns></returns>
        //private static bool LogSwitch()
        //{
        //    string logSwitch = ConfigurationManager.AppSettings["LogLevel"].ToString();
        //    return logSwitch.ToLower() == "true";
        //}
    }
}
