using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using Utility;
using System.Threading;
using log4net;

namespace 服务器端接收程序
{
    static class Program
    {
        private static ILog log = LogManager.GetLogger(typeof(Program));

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                //处理未捕获的异常   
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                //处理UI线程异常   
                Application.ThreadException += Application_ThreadException;
                //处理非UI线程异常   
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                var aProcessName = Process.GetCurrentProcess().ProcessName;
                if ((Process.GetProcessesByName(aProcessName)).GetUpperBound(0) > 0)
                {
                    MessageBox.Show(@"系统已经在运行中，如果要重新启动，请先从进程中关闭...", @"系统警告", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                else
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Launch());
                    //Login login = new Login();
                    //if (login.ShowDialog() == DialogResult.OK)
                    //{
                    //    Application.Run(new MainForm());
                    //}
                    //Application.Exit();
                }
            }
            catch (Exception ex)
            {
                log.Error("系统崩溃，请重启系统！  errCode=101", ex); 
                MessageBox.Show("系统出现未知异常，请重启系统！  errCode=101");
            }
        }
        ///<summary>
        ///  这就是我们要在发生未处理异常时处理的方法，我这是写出错详细信息到文本，如出错后弹出一个漂亮的出错提示窗体，给大家做个参考
        ///  做法很多，可以是把出错详细信息记录到文本、数据库，发送出错邮件到作者信箱或出错后重新初始化等等
        ///  这就是仁者见仁智者见智，大家自己做了。
        ///</summary>
        ///<param name="sender"> </param>
        ///<param name="e"> </param>
        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            var ex = e.Exception;
            if (ex != null)
            {
                log.Error("系统崩溃，请重启系统！  errCode=102", ex); 
            }

            MessageBox.Show("系统出现未知异常，请重启系统！   errCode=102");
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                log.Error("系统崩溃，请重启系统！  errCode=103", ex); 
            }

            MessageBox.Show("系统出现非UI线程未知异常，请重启系统！   errCode=103");
        }

    }
}
