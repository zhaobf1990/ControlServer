using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 服务器端接收程序.Util
{
    class DateUtil
    {

        public static String datediff(DateTime start, DateTime end) {
            string dateDiff = null;
            TimeSpan ts1 = new TimeSpan(start.Ticks);
            TimeSpan ts2 = new TimeSpan(end.Ticks);
            TimeSpan ts = ts1.Subtract(ts2).Duration();
            //显示时间  
             dateDiff = ts.Days.ToString() + "天"
                    + ts.Hours.ToString() + "小时"
                    + ts.Minutes.ToString() + "分钟"
                    + ts.Seconds.ToString() + "秒";
             return dateDiff;
        }
    }
}
