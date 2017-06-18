using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 服务器端接收程序.Util
{
    /// <summary>
    /// 消息队列 
    /// </summary>
    public class MessageQueue
    {

        private static Queue<string> dataInfoMessages = new Queue<string>();

        private static Queue<string> RegAndLogoutMessage = new Queue<string>();
         
        /// <summary>
        /// 将对象添加到指定消息队列的结尾处
        /// </summary>
        /// <param name="group"></param>
        /// <param name="message"></param>
        private static void EnQueue(Queue<string> group, string message)
        {
            lock (group)
            {
                group.Enqueue(message);
            }
        }

        /// <summary>
        /// 获取指定消息队列中的所有数据   并从原消息队列中删除
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        private static List<string> DeQueue(Queue<string> group)
        {
            List<string> list = new List<string>();
            lock (group)
            {
                if (group.Count > 0)
                {
                    list.Add(group.Dequeue());
                }
            }
            return list;
        }

        /// <summary>
        ///将消息添加到dataInfo队列中
        /// </summary>
        /// <param name="message"></param>
        public static void Enqueue_DataInfo(string message)
        {
            EnQueue(dataInfoMessages, message);
        }

        /// <summary>
        /// 获取dataInfo队列中的消息   并从队列中移除
        /// </summary>
        /// <returns></returns>
        public static List<string> Dequeue_DataInfo()
        {
            return DeQueue(dataInfoMessages);
        } 

        /// <summary>
        ///将消息添加到RegAndLogout队列中
        /// </summary>
        /// <param name="message"></param>
        public static void Enqueue_RegAndLogout(string message)
        {
            EnQueue(RegAndLogoutMessage, message);
        }

        /// <summary>
        /// 获取RegAndLogout队列中的消息   并从队列中移除
        /// </summary>
        /// <returns></returns>
        public static List<string> Dequeue_RegAndLogout()
        {
            return DeQueue(RegAndLogoutMessage);
        }
    }
}
