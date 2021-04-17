using DCGServiceDesk.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DCGServiceDesk.Services
{
    public static class DateTimeConversion
    {
        public static DateTime GetDateFromIM(Incident im, string phase, TimeZoneInfo _timeZoneInfo)
        {
            if (phase == "start")
                return TimeZoneInfo.ConvertTime(im.History.ActiveStatus.CreateDate, _timeZoneInfo);
            else if (phase == "end")
                return TimeZoneInfo.ConvertTime(im.History.ActiveStatus.DueTime, _timeZoneInfo);
            else
                return default(DateTime);
        }
        public static DateTime GetDateFromC(ServiceRequest c, string phase, TimeZoneInfo _timeZoneInfo)
        {
            if (phase == "start")
                return TimeZoneInfo.ConvertTime(c.History.ActiveStatus.CreateDate, _timeZoneInfo);
            else if (phase == "end")
                return TimeZoneInfo.ConvertTime(c.History.ActiveStatus.DueTime, _timeZoneInfo);
            else
                return default(DateTime);
        }
        public static DateTime GetDateFromT(TaskRequest t, string phase, TimeZoneInfo _timeZoneInfo)
        {
            if (phase == "start")
                return TimeZoneInfo.ConvertTime(t.History.ActiveStatus.CreateDate, _timeZoneInfo);
            else if (phase == "end")
                return TimeZoneInfo.ConvertTime(t.History.ActiveStatus.DueTime, _timeZoneInfo);
            else
                return default(DateTime);
        }
        public static DateTime ConvertDate(DateTime SLA, TimeZoneInfo _timeZoneInfo) =>
            TimeZoneInfo.ConvertTime(SLA, _timeZoneInfo);
    }
}
