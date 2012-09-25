using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entity;

namespace EntityUtl
{
    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt)
        {
            int diff = dt.DayOfWeek - DayOfWeek.Monday; if (diff < 0) { diff += 7; }
            return dt.AddDays(-1 * diff).Date;
        }

        public static DateTime EndOfWeek(this DateTime dt)
        {
            int diff = DayOfWeek.Friday - dt.DayOfWeek;
            if (diff < 0)
            { diff += 7; }
            return dt.AddDays(1 * diff).Date;
        }
        public static DateTime StartOfMonth(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1);
        }

        public static DateTime EndOfMonth(this DateTime dt)
        {
            DateTime firstDayOfTheMonth = new DateTime(dt.Year, dt.Month, 1);
            return firstDayOfTheMonth.AddMonths(1).AddDays(-1);
        }
    }

    public class Utl
    {
        public static string GetFullName(EntityBase e)
        {
            var pe = e.GetType().GetProperty("Name_EN");
            var pc = e.GetType().GetProperty("Name_CH");
            var eno = pe.GetValue(e, null);
            string en = eno == null ? string.Empty : eno.ToString();

            var cho = pc.GetValue(e, null);
            string ch = cho == null ? string.Empty : cho.ToString();
      

            if (!string.IsNullOrEmpty(en) && !string.IsNullOrEmpty(ch))
            {
                return en + "|" + ch;
            }
            else
            {
                return en + ch;
            }
        }

        public static bool CheckPropertyAllNull(object o, params string[] properties)
        {
            foreach (var p in properties)
            {
                var property = o.GetType().GetProperty(p);
                var v = property.GetValue(o, null);
                if (v!=null)
                    return false;
            }
            return true;
        }
    }
}
