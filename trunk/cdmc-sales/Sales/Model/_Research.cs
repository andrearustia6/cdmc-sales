using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Utl;

namespace Sales.Model
{
    public class _MonthDuration
    {
        public static _MonthDuration GetMonthInstance(int? month) {
            if (month == null) month = DateTime.Now.Month;

            _MonthDuration d = new _MonthDuration();
            d.MonthStartDate = new DateTime(DateTime.Now.Year, month.Value, 1);
            d.MonthEndDate = d.MonthStartDate.EndOfMonth();

            d.StartDate1 = d.MonthStartDate;
            while (d.MonthStartDate.DayOfWeek != DayOfWeek.Monday)
            {
                d.StartDate1 = d.StartDate1.AddDays(-1);
            }
            d.EndDate1 = d.StartDate1.AddDays(7);

            d.StartDate2 = d.EndDate1.AddDays(1);
            d.EndDate2 = d.StartDate2.AddDays(7);

            d.StartDate3 = d.EndDate2.AddDays(1);
            d.EndDate3 = d.StartDate3.AddDays(7);

            d.StartDate4 = d.EndDate3.AddDays(1);
            d.EndDate4 = d.StartDate4.AddDays(7);

            if(d.EndDate4.Month == d.StartDate1.Month && )
            d.StartDate2 = d.EndDate1.AddDays(1);
            d.EndDate2 = d.StartDate2.AddDays(7);
           
            while (enddate.Month != startdate.Month)
            {
                var tempdate = enddate.AddDays(7);
                if (tempdate.Month == startdate.Month)
                    enddate = tempdate;
            }

           
            while (enddate.DayOfWeek != DayOfWeek.Friday)
            {
                enddate = enddate.AddDays(-1);
            }
        }
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
    }

    public class _ResearchData 
    {
        public int Month{get;set;}
        public List<string> Weeks{get;set;}
        public IQueryable<_ProjectResearch> _ProjectResearch { get; set; }
        public IQueryable<_UsertResearch> _UsertResearch { get; set; }
    }

    public class _ResearchCount
    {
        [Display(Name = "第1周公司数")]
        public int FirstWeekCompanyCount { get; set; }

        [Display(Name = "第2周公司数")]
        public int SecondWeekCompanyCount { get; set; }

        [Display(Name = "第3周公司数")]
        public int ThirdWeekCompanyCount { get; set; }

        [Display(Name = "第4周公司数")]
        public int FourthWeekCompanyCount { get; set; }

        [Display(Name = "第5周公司数")]
        public int FivethtWeekCompanyCount { get; set; }

        [Display(Name = "第1周Lead数")]
        public int FirstWeekLeadCount { get; set; }

        [Display(Name = "第2周Lead数")]
        public int SecondWeekLeadCount { get; set; }

        [Display(Name = "第3周Lead数")]
        public int ThirdWeekLeadCount { get; set; }

        [Display(Name = "第4周Lead数")]
        public int FourthWeekLeadCount { get; set; }

        [Display(Name = "第5周Lead数")]
        public int FivethtWeekLeadCount { get; set; }
    }

    public class _ProjectResearch : _ResearchCount
    {
        [Display(Name="项目名称")]
        public string ProjectName { get; set; }

        public int MemberCount { get; set; }

        public double CompanyAverage { get; set; }

        public double LeadAverage { get; set; }
    }

    public class _UsertResearch : _ResearchCount
    {
        [Display(Name = "项目名称")]
        public string UserName { get; set; }

        [Display(Name = "入职时间（月）")]
        public int EmployeeDuration { get; set; }
    }
}