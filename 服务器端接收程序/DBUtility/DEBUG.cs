using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;

namespace Utility
{
    /// <summary>
    /// 消息对话框类
    /// </summary>
    public class DEBUG
    {
        /// <summary>
        /// 弹出对话框
        /// </summary>
        /// <param name="msg"></param>
        public static void MsgBox(string msg)
        {
            if (DEBUG_MessageBox_Switch())
            {
                MessageBox.Show(msg);
            }
        }

        /// <summary>
        /// 调试对话框开关
        /// </summary>
        /// <returns></returns>
        private static bool DEBUG_MessageBox_Switch()
        {
            string DEBUG_MessageBox_Switch = ConfigurationManager.AppSettings["DEBUG_MessageBox_Switch"].ToString();
            return DEBUG_MessageBox_Switch.ToLower() == "true";
        }

        /// <summary>
        /// 抛出异常
        /// </summary>
        /// <param name="ex"></param>
        public static void ThrowException(Exception ex)
        {
            if (DEBUG_Exception_Switch()) {                
                throw ex;
            }
        }

        private static bool DEBUG_Exception_Switch()
        {
            string DEBUG_Exception_Switch = ConfigurationManager.AppSettings["DEBUG_ThorwException_Switch"].ToString();
            return DEBUG_Exception_Switch.ToLower() == "true";
        }
    }
}
