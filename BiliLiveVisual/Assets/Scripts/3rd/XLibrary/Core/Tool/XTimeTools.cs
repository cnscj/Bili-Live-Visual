/*************************
 * 
 * 时间,日期操作类
 * 
 **************************/

/**
string.Format("{0:y yy yyy yyyy}",dt); //17 17 2017 2017
string.Format("{0:M MM MMM MMMM}", dt);//4  04 四月 四月
string.Format("{0:d dd ddd dddd}", dt);//1  01 周六 星期六
string.Format("{0:t tt}", dt);//下 下午
string.Format("{0:H HH}", dt);//13 13
string.Format("{0:h hh}", dt);//1  01
string.Format("{0:m mm}", dt);//16 16
string.Format("{0:s ss}", dt);//32 32
string.Format("{0:F FF FFF FFFF FFFFF FFFFFF FFFFFFF}", dt);//1 1  108 108  108   108    108
string.Format("{0:f ff fff ffff fffff ffffff fffffff}", dt);//1 10 108 1080 10800 108000 1080000
string.Format("{0:z zz zzz}", dt);//+8 +08 +08:00

string.Format("{0:yyyy/MM/dd HH:mm:ss.fff}",dt);　　//2017/04/01 13:16:32.108
string.Format("{0:yyyy/MM/dd dddd}", dt);　　　　　　//2017/04/01 星期六
string.Format("{0:yyyy/MM/dd dddd tt hh:mm}", dt); //2017/04/01 星期六 下午 01:16
string.Format("{0:yyyyMMdd}", dt);　　　　　　　　　//20170401
string.Format("{0:yyyy-MM-dd HH:mm:ss.fff}", dt);　//2017-04-01 13:16:32.108
**/
using System;

namespace XLibrary
{
    //XXX:缺少时区的概念
    public static class XTimeTools
    {
        public static readonly DateTime TIME_ORIGIN = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        /// <summary>
        /// 获取时间戳Ms
        /// </summary>
        /// <returns></returns>
        public static long NowTimeStampMs
        {
            get
            {
                return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
            }

        }
        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static long NowTimeStamp
        {
            get
            {
                TimeSpan ts = DateTime.UtcNow - TIME_ORIGIN;
                return Convert.ToInt64(ts.TotalMilliseconds / 1000);
            }

        }

        /// <summary>
        /// 当前DateTime
        /// </summary>
        /// <returns></returns>
        public static DateTime NowDateTime
        {
            get
            {
                return DateTime.Now;
            }
        }

        /// <summary>
        /// 当前时间字符串格式
        /// </summary>
        /// <returns></returns>
        public static string NowTimeStr
        {
            get
            {
                return DateTime.Now.ToString();
            }
        }

        /// <summary>
        /// 取得今天度过了多少秒
        /// </summary>
        /// <returns></returns>
        public static long NowSeconds
        {
            get
            {
                DateTime dataTime = NowDateTime;
                return dataTime.Hour * 3600 + dataTime.Minute * 60 + dataTime.Second;
            }
        }

        //无格式时间202001192212
        public static string NowUnformatTimeStr
        {
            get
            {
                DateTime dt = NowDateTime;
                return string.Format("{0:yyyyMMddHHmmss}", dt);
            }
            
        }
        public static string NowUnformatTimeStrMs
        {
            get
            {
                DateTime dt = NowDateTime;
                return string.Format("{0:yyyyMMddHHmmssfff}", dt);
            }
        }


        public static long GetTimeStamp(DateTime dataTime)
        {
            TimeSpan ts = dataTime.ToUniversalTime() - TIME_ORIGIN;
            return Convert.ToInt64(ts.TotalMilliseconds / 1000);
        }

        //%Y-%m-%d %H:%M:%S或%Y/%m/%d %H:%M:%S格式转时间戳
        public static long GetTimeStamp(string timeStr)
        {
            TimeSpan ts = DateTime.Parse(timeStr).ToUniversalTime() - TIME_ORIGIN;
            return Convert.ToInt64(ts.TotalMilliseconds / 1000);
        }

        public static DateTime GetDateTime(long timeStamp)
        {
            timeStamp = timeStamp >= 10000000000 ? timeStamp / 1000 : timeStamp;
            var date = new DateTime(621355968000000000 + timeStamp * (long)10000000, DateTimeKind.Utc);
            return date.ToLocalTime();
        }

        //%Y-%m-%d %H:%M:%S或%Y/%m/%d %H:%M:%S格式转时间类
        public static DateTime GetDateTime(string timeStr)
        {
            return Convert.ToDateTime(timeStr);
        }

        public static string GetTimeStr(DateTime dataTime)
        {
            return dataTime.ToString();
        }

        public static string GetTimeStr(long timeStamp)
        {
            timeStamp = timeStamp >= 10000000000 ? timeStamp / 1000 : timeStamp;
            var date = new DateTime(621355968000000000 + timeStamp * (long)10000000, DateTimeKind.Utc);
            return date.ToLocalTime().ToString();
        }


        /////////
        public static int GetSecondsByTimeStr(string timeStr)
        {
            DateTime dataTime = GetDateTime(timeStr);
            return dataTime.Hour * 3600 + dataTime.Minute * 60 + dataTime.Second;
        }
        public static DateTime GetDateTimeBySeconds(int seconds)
        {
            int hour = (seconds % 86400) / 3600;
            int minute = ((seconds % 86400) % 3600) / 60;
            int second = (((seconds % 86400) % 3600) % 60);
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, minute, second);
        }
    }
}