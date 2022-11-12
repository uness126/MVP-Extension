using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhaleExtension.Class
{
    public static class DateTimeExtension
    {
        private readonly static PersianCalendar PC = new PersianCalendar();
        public static DateTime PersianCalendarToDateTime(this DateTime dateTime)
        {
            try
            {
                return PC.ToDateTime(PC.GetYear(dateTime), PC.GetMonth(dateTime), PC.GetDayOfMonth(dateTime), 0, 0, 0, 0, GregorianCalendar.ADEra);
            }
            catch (Exception ex)
            {
                Logger.WriteLOG(ex, "PersianCalendarToDateTime");
                throw;
            }
        }

        public static string Convert2Persian(this DateTime date)
        {
            try
            {
                PersianCalendar perdate = new PersianCalendar();
                return perdate.GetYear(date).ToString("00") + "/" + perdate.GetMonth(date).ToString("00") + "/" + perdate.GetDayOfMonth(date).ToString("00");
            }
            catch (Exception ex)
            {
                Logger.WriteLOG(ex, "Convert2Persian");
                throw;
            }
        }
    }
}
