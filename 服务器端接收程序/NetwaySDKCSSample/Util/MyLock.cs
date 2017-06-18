using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace 服务器端接收程序.MyForm.GPRSControl
{
    public class MyLock : IDisposable
    { /// <summary>
        /// 默认超时设置
        /// </summary>
        public static int DefaultMillisecondsTimeout = 15000; // 15S
        private object _obj;

        /// <summary>
        /// 构造 
        /// </summary>
        /// <param name="obj">想要锁住的对象</param>
        public MyLock(object obj)
        {
            TryGet(obj, DefaultMillisecondsTimeout, false);
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj">想要锁住的对象</param>
        /// <param name="millisecondsTimeout">超时设置</param>
        public MyLock(object obj, int millisecondsTimeout)
        {
            TryGet(obj, millisecondsTimeout, false);
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="obj">想要锁住的对象</param>
        /// <param name="millisecondsTimeout">超时设置</param>
        /// <param name="throwTimeoutException">是否抛出超时异常</param>
        public MyLock(object obj, int millisecondsTimeout, bool throwTimeoutException)
        {
            TryGet(obj, millisecondsTimeout, throwTimeoutException);
        }

        private void TryGet(object obj, int millisecondsTimeout, bool throwTimeoutException)
        {
            if (Monitor.TryEnter(obj, millisecondsTimeout))
            {
                _obj = obj;
            }
            else
            {
                if (throwTimeoutException)
                {
                    throw new TimeoutException();
                }
            }
        }

        /// <summary>
        /// 销毁，并释放锁
        /// </summary>
        public void Dispose()
        {
            if (_obj != null)
            {
                Monitor.Exit(_obj);
            }
        }

        /// <summary>
        /// 获取在获取锁时是否发生等待超时     如果没有超时，则表示锁成功 
        /// </summary>
        public bool IsTimeout
        {
            get
            {
                return _obj == null;
            }
        }
    }
}
