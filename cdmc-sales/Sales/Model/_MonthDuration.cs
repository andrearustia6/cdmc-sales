using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Utl;
using System.ComponentModel.DataAnnotations;
namespace Sales.Model
{
    public class WeekDuration
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int order { get; set; }
    }

    public class MonthDuration
    {
      
        public static MonthDuration GetMonthInstance(int? month)
        {
            if (month == null) month = DateTime.Now.Month;
             
            MonthDuration d = new MonthDuration();
            d.WeekDurations = new List<WeekDuration>();
            d.DurationStrings = new List<string>();
            d.Month = month.Value;
            d.MonthStartDate = new DateTime(DateTime.Now.Year, month.Value, 1);
            d.MonthEndDate = d.MonthStartDate.EndOfMonth();

            d.StartDate1 = d.MonthStartDate;
            if (d.StartDate1.DayOfWeek == DayOfWeek.Saturday || d.StartDate1.DayOfWeek == DayOfWeek.Sunday)
            {
                while (d.StartDate1.DayOfWeek != DayOfWeek.Monday)
                {
                    d.StartDate1 = d.StartDate1.AddDays(1);
                }
            }
            else
            {
                while (d.StartDate1.DayOfWeek != DayOfWeek.Monday)
                {
                    d.StartDate1 = d.StartDate1.AddDays(-1);
                }
            }
           
            d.EndDate1 = d.StartDate1.AddDays(7);
            d.DurationStrings.Add(d.StartDate1.ToShortMonthString() + "~" + d.EndDate1.ToShortMonthString());
            d.WeekDurations.Add(new WeekDuration() { StartDate = d.StartDate1, EndDate=d.EndDate1,order=1});

            d.StartDate2 = d.EndDate1;
            d.EndDate2 = d.StartDate2.AddDays(7);

            d.DurationStrings.Add(d.StartDate2.ToShortMonthString() + "~" + d.EndDate2.ToShortMonthString());
            d.WeekDurations.Add(new WeekDuration() { StartDate = d.StartDate2, EndDate = d.EndDate2, order = 2 });

            d.StartDate3 = d.EndDate2;
            d.EndDate3 = d.StartDate3.AddDays(7);
            d.DurationStrings.Add(d.StartDate3.ToShortMonthString() + "~" + d.EndDate3.ToShortMonthString());
            d.WeekDurations.Add(new WeekDuration() { StartDate = d.StartDate3, EndDate = d.EndDate3, order = 3 });

            d.StartDate4 = d.EndDate3;
            d.EndDate4 = d.StartDate4.AddDays(7);
            d.DurationStrings.Add(d.StartDate4.ToShortMonthString() + "~" + d.EndDate4.ToShortMonthString());
            d.WeekDurations.Add(new WeekDuration() { StartDate = d.StartDate4, EndDate = d.EndDate4, order = 4 });
            if (d.EndDate4.AddDays(5).Month == month)//存在第5周
            {
                d.StartDate5 = d.EndDate4;
                d.EndDate5 = d.StartDate5.Value.AddDays(7);
                d.DurationStrings.Add(d.StartDate5.Value.ToShortMonthString() + "~" + d.EndDate5.Value.ToShortMonthString());
                d.WeekDurations.Add(new WeekDuration() { StartDate = d.StartDate5.Value, EndDate = d.EndDate5.Value, order = 5 });
            }
            else
            {
                d.StartDate5 = d.EndDate5 = d.EndDate4;
            }
          
            return d;
        }
        public static MonthDuration GetMonthInstanceByYearMonth(int? year, int? month)
        {
            if (year == null) year = DateTime.Now.Year;
            if (month == null) month = DateTime.Now.Month;

            MonthDuration d = new MonthDuration();
            d.WeekDurations = new List<WeekDuration>();
            d.DurationStrings = new List<string>();
            d.Month = month.Value;
            d.MonthStartDate = new DateTime(year.Value, month.Value, 1);
            d.MonthEndDate = d.MonthStartDate.EndOfMonth();

            d.StartDate1 = d.MonthStartDate;

            //if (d.StartDate1.DayOfWeek == DayOfWeek.Saturday || d.StartDate1.DayOfWeek == DayOfWeek.Sunday)
            //{
            //    while (d.StartDate1.DayOfWeek != DayOfWeek.Monday)
            //    {
            //        d.StartDate1 = d.StartDate1.AddDays(1);
            //    }
            //}
            //else
            //{
            //    while (d.StartDate1.DayOfWeek != DayOfWeek.Monday)
            //    {
            //        d.StartDate1 = d.StartDate1.AddDays(-1);
            //    }
            //}

            if (d.StartDate1.DayOfWeek != DayOfWeek.Monday)
            {
                while (d.StartDate1.DayOfWeek != DayOfWeek.Monday)
                {
                    d.StartDate1 = d.StartDate1.AddDays(-1);
                }
            }
            //else
            //{
            //    while (d.StartDate1.DayOfWeek != DayOfWeek.Monday)
            //    {
            //        d.StartDate1 = d.StartDate1.AddDays(-1);
            //    }
            //}

            d.EndDate1 = d.StartDate1.AddDays(7);
            d.DurationStrings.Add(d.StartDate1.ToShortMonthString() + "~" + d.EndDate1.ToShortMonthString());
            d.WeekDurations.Add(new WeekDuration() { StartDate = d.StartDate1, EndDate = d.EndDate1, order = 1 });

            d.StartDate2 = d.EndDate1;
            d.EndDate2 = d.StartDate2.AddDays(7);

            d.DurationStrings.Add(d.StartDate2.ToShortMonthString() + "~" + d.EndDate2.ToShortMonthString());
            d.WeekDurations.Add(new WeekDuration() { StartDate = d.StartDate2, EndDate = d.EndDate2, order = 2 });

            d.StartDate3 = d.EndDate2;
            d.EndDate3 = d.StartDate3.AddDays(7);
            d.DurationStrings.Add(d.StartDate3.ToShortMonthString() + "~" + d.EndDate3.ToShortMonthString());
            d.WeekDurations.Add(new WeekDuration() { StartDate = d.StartDate3, EndDate = d.EndDate3, order = 3 });

            d.StartDate4 = d.EndDate3;
            d.EndDate4 = d.StartDate4.AddDays(7);
            d.DurationStrings.Add(d.StartDate4.ToShortMonthString() + "~" + d.EndDate4.ToShortMonthString());
            d.WeekDurations.Add(new WeekDuration() { StartDate = d.StartDate4, EndDate = d.EndDate4, order = 4 });
            if (d.EndDate4.AddDays(5).Month == month)//存在第5周
            {
                d.StartDate5 = d.EndDate4;
                d.EndDate5 = d.StartDate5.Value.AddDays(7);
                d.DurationStrings.Add(d.StartDate5.Value.ToShortMonthString() + "~" + d.EndDate5.Value.ToShortMonthString());
                d.WeekDurations.Add(new WeekDuration() { StartDate = d.StartDate5.Value, EndDate = d.EndDate5.Value, order = 5 });
            }
            else
            {
                d.StartDate5 = d.EndDate5 = d.EndDate4;
            }

            return d;
        }
        [Display(Name="月份")]
        public int Month { get; set; }
        public List<string> DurationStrings { get; set; }
        public DateTime MonthStartDate { get; set; }
        public DateTime MonthEndDate { get; set; }
        public DateTime StartDate1 { get; set; }
        public DateTime EndDate1 { get; set; }
        public DateTime StartDate2 { get; set; }
        public DateTime EndDate2 { get; set; }
        public DateTime StartDate3 { get; set; }
        public DateTime EndDate3 { get; set; }
        public DateTime StartDate4 { get; set; }
        public DateTime EndDate4 { get; set; }
        public DateTime? StartDate5 { get; set; }
        public DateTime? EndDate5 { get; set; }

        public List<WeekDuration> WeekDurations { get; set; }
    }
}