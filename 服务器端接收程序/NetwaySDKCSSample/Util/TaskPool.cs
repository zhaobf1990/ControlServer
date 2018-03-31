﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace 服务器端接收程序.MyForm.GPRSControl
{
    public class TaskPool
    {
        public String Name { get; set; }
        /// <summary>
        /// 最大线程数
        /// </summary>
        private int max = 6;
        /// <summary>
        /// 最小线程数
        /// </summary>
        private int min = 1;
        /// <summary>
        /// 当活动线程不足的时候新增线程的增量
        /// </summary>
        private int increment = 2;

        /// <summary>
        /// 所有的线程
        /// </summary>
        public Dictionary<string, Task> publicpool;
        /// <summary>
        /// 空闲线程队列
        /// </summary>
        private Queue<Task> freequeue;
        /// <summary>
        /// 正在工作的线程
        /// </summary>
        private Dictionary<string, Task> working;
        /// <summary>
        /// 等待执行的工作队列
        /// </summary>
        public Queue<Waititem> waitlist;

        /// <summary>
        /// 获取最小线程数
        /// </summary>
        /// <returns></returns>
        public int GetMin()
        {
            return min;
        }
        /// <summary>
        /// 获取最大线程数
        /// </summary>
        /// <returns></returns>
        public int GetMax()
        {
            return max;
        }

        /// <summary>
        /// 获取当前所有的线程数
        /// </summary>
        /// <returns></returns>
        public int GetPublicPoolCount()
        {
            return publicpool.Count;
        }
        /// <summary>
        /// 获取空闲的线程数
        /// </summary>
        /// <returns></returns>
        public int GetFreeQueueCount()
        {
            return freequeue.Count;
        }
        /// <summary>
        /// 获取正在工作的线程数
        /// </summary>
        /// <returns></returns>
        public int GetWorkingCount()
        {
            return working.Count;
        }
        /// <summary>
        /// 获取等待的线程数
        /// </summary>
        /// <returns></returns>
        public int GetWaitListCount()
        {
            return waitlist.Count;
        }



        /// <summary>
        /// 设置最大线程数
        /// </summary>
        /// <param name="Value"></param>
        public void Setmaxthread(int Value)
        {
            lock (this)
            {
                max = Value;
            }
        }
        /// <summary>
        /// 设置最小线程数
        /// </summary>
        /// <param name="Value"></param>
        public void Setminthread(int Value)
        {
            lock (this)
            {
                min = Value;
            }
        }
        /// <summary>
        /// 设置增量
        /// </summary>
        /// <param name="Value"></param>
        public void Setincrement(int Value)
        {
            lock (this)
            {
                increment = Value;
            }
        }
        /// <summary>
        /// 初始化线程池
        /// </summary>
        public TaskPool()
        {
            publicpool = new Dictionary<string, Task>();
            working = new Dictionary<string, Task>();
            freequeue = new Queue<Task>();
            waitlist = new Queue<Waititem>();
            Task t = null;
            //先创建最小线程数的线程
            for (int i = 0; i < min; i++)
            {
                t = new Task();
                //注册线程完成时触发的事件
                t.WorkComplete += new Action<Task>(t_WorkComplete);
                //加入到所有线程的字典中
                publicpool.Add(t.Key, t);
                //因为还没加入具体的工作委托就先放入空闲队列
                freequeue.Enqueue(t);
            }

        }
        /// <summary>
        /// 线程执行完毕后的触发事件
        /// </summary>
        /// <param name="obj"></param>
        void t_WorkComplete(Task obj)
        {
            lock (this)
            {
                //首先因为工作执行完了，所以从正在工作字典里删除
                working.Remove(obj.Key);
                //检查是否有等待执行的操作，如果有等待的优先执行等待的任务
                if (waitlist.Count > 0)
                {
                    //先要注销当前的线程，将其从线程字典删除
                    publicpool.Remove(obj.Key);
                    obj.Close();
                    //从等待任务队列提取一个任务
                    Waititem item = waitlist.Dequeue();
                    Task nt = null;
                    //如果有空闲的线程，就是用空闲的线程来处理
                    if (freequeue.Count > 0)
                    {
                        nt = freequeue.Dequeue();
                    }
                    else
                    {
                        //如果没有空闲的线程就再新创建一个线程来执行
                        nt = new Task();
                        publicpool.Add(nt.Key, nt);
                        nt.WorkComplete += new Action<Task>(t_WorkComplete);
                    }
                    //设置线程的执行委托对象和上下文对象
                    nt.taskWorkItem = item.Works;
                    nt.contextdata = item.Context;
                    //添加到工作字典中
                    working.Add(nt.Key, nt);
                    //唤醒线程开始执行
                    nt.Active();
                }
                else
                {
                    //如果没有等待执行的操作就回收多余的工作线程
                    if (freequeue.Count > min)
                    {
                        //当空闲线程超过最小线程数就回收多余的这一个
                        publicpool.Remove(obj.Key);
                        obj.Close();
                    }
                    else
                    {
                        //如果没超过就把线程从工作字典放入空闲队列
                        obj.contextdata = null;
                        freequeue.Enqueue(obj);
                    }
                }
            }
        }
        /// <summary>
        /// 添加工作委托的方法
        /// </summary>
        /// <param name="TaskItem"></param>
        /// <param name="Context"></param>
        public void AddTaskItem(WaitCallback TaskItem, object Context)
        {
            lock (this)
            {
                Task t = null;
                int len = publicpool.Values.Count;
                //如果线程没有到达最大值
                if (len < max)
                {
                    //如果空闲列表非空
                    if (freequeue.Count > 0)
                    {
                        //从空闲队列pop一个线程
                        t = freequeue.Dequeue();
                        //加入工作字典
                        working.Add(t.Key, t);
                        //设置执行委托
                        t.taskWorkItem = TaskItem;
                        //设置状态对象
                        t.contextdata = Context;
                        //唤醒线程开始执行
                        t.Active();
                        return;
                    }
                    else
                    {
                        //如果没有空闲队列了，就根据增量创建线程
                        for (int i = 0; i < increment; i++)
                        {
                            //判断线程的总量不能超过max
                            if ((len + i) <= max)
                            {
                                t = new Task();
                                //设置完成响应事件
                                t.WorkComplete += new Action<Task>(t_WorkComplete);
                                //加入线程字典
                                publicpool.Add(t.Key, t);
                                //加入空闲队列
                                freequeue.Enqueue(t);
                            }
                            else
                            {
                                break;
                            }
                        }
                        //从空闲队列提出出来设置后开始执行
                        t = freequeue.Dequeue();
                        working.Add(t.Key, t);
                        t.taskWorkItem = TaskItem;
                        t.contextdata = Context;
                        t.Active();
                        return;
                    }
                }
                else
                {
                    //如果线程达到max就把任务加入等待队列
                    waitlist.Enqueue(new Waititem() { Context = Context, Works = TaskItem });
                }
            }
        }

        /// <summary>
        /// 回收资源
        /// </summary>
        public void Dispose()
        {
            //throw new NotImplementedException();
            foreach (Task t in publicpool.Values)
            {
                //关闭所有的线程
                using (t) { t.Close(); }
            }
            publicpool.Clear();
            working.Clear();
            waitlist.Clear();
            freequeue.Clear();
        }
        /// <summary>
        /// 存储等待队列的类
        /// </summary>
        public class Waititem
        {
            public WaitCallback Works { get; set; }
            public object Context { get; set; }
        }

        public String GetPoolDetail()
        {
            return "等待执行的工作队列数:" + waitlist.Count + "个， 正在工作的线程数：" + working.Count + "个, 空闲线程队列数：" + freequeue.Count + "个， 所有的线程数：" + publicpool.Count + "个";
        }

    }
    /// <summary>
    /// 线程包装器类
    /// </summary>
    public class Task : IDisposable
    {
        /// <summary>
        /// 线程锁
        /// </summary>
        private AutoResetEvent locks;
        /// <summary>
        /// 线程对象
        /// </summary>
        private Thread T;
        /// <summary>
        /// 线程体委托
        /// </summary>
        public WaitCallback taskWorkItem;
        /// <summary>
        /// 线程是否工作
        /// </summary>
        private bool working;
        public object contextdata
        {
            get;
            set;
        }
        /// <summary>
        /// 线程完成一次操作的事件
        /// </summary>
        public event Action<Task> WorkComplete;
        /// <summary>
        /// 用于字典的Key
        /// </summary>
        public string Key
        {
            get;
            set;
        }
        /// <summary>
        /// 初始化包装器
        /// </summary>
        public Task()
        {
            //设置线程一进入就阻塞
            locks = new AutoResetEvent(false);
            Key = Guid.NewGuid().ToString();
            //初始化线程对象
            T = new Thread(Work);
            T.IsBackground = true;
            working = true;
            contextdata = new object();
            //开启线程
            T.Start();
        }
        /// <summary>
        /// 唤醒线程
        /// </summary>
        public void Active()
        {
            locks.Set();
        }
        /// <summary>
        /// 设置执行委托和状态对象
        /// </summary>
        /// <param name="action"></param>
        /// <param name="context"></param>
        public void SetWorkItem(WaitCallback action, object context)
        {
            taskWorkItem = action;
            contextdata = context;
        }
        /// <summary>
        /// 线程体包装方法
        /// </summary>
        private void Work()
        {
            while (working)
            {
                //阻塞线程
                locks.WaitOne();
                taskWorkItem(contextdata);
                //完成一次执行，触发事件
                WorkComplete(this);
            }
        }
        /// <summary>
        /// 关闭线程
        /// </summary>
        public void Close()
        {
            working = false;
        }
        /// <summary>
        /// 回收资源
        /// </summary>
        public void Dispose()
        {
            //throw new NotImplementedException();
            try
            {
                T.Abort();
            }
            catch { }
        }


    }
}
