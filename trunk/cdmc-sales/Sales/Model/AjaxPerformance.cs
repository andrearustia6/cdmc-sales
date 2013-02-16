using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Entity;

namespace Model
{
    public class AjaxPerformance:AjaxStatistics
    {
       
        [Display(Name="被考核人")]
        public string Name { get; set; }
        [Display(Name = "职位")]
        public string Title { get; set; }
        public int PerformanceRate { get; set; }
        [Display(Name = "所在项目")]
        public string ProjectName { get; set; }
        public string TL { get; set; }
        public string Manager { get; set; }
        

    }

    public class AjaxMonthPerformance : AjaxPerformance
    {
        [Display(Name = "月份")]
        public int Month { get; set; }

        [Display(Name = "主观评分")]
        public int AssignedScore { get; set; }

        public List<AjaxWeekPerformance> AjaxWeekPerformances { get; set; }

        public int Score { get { 
            var qc = AjaxWeekPerformances.FindAll(f=>f.IsQualified==false).Count;
            if (qc == 0)
                return 20;
            else if (qc == 1)
                return 15;
            else if (qc == 2)
                return 10;
            else if (qc == 3)
                return 5;
            else
                return 0;
        } }
    }

    public class AjaxWeekPerformance : AjaxPerformance
    {
        public virtual int FaxOutStandard { get; set; }
        public virtual int LeadsStandard { get; set; }
        public virtual int CallHoursStandard { get; set; }

       
        public int FaxoutCount { get; set; }

        public virtual bool IsQualified { get; set; }
        public bool IsFaxOutQualified
        {
            get
            {
                return FaxoutCount > FaxOutStandard;

            }
        }
        public bool IsDealInQualified { get { return DealInComplatePercetage > 100; } }
        public bool IsCheckInInQualified { get { return CheckInComplatePercetage > 100; } }
        public bool IsLeadAddedQualified { get { return LeadsCount > LeadsStandard; } }
        public  bool IsCallHoursQualified { get { return CallHours > CallHoursStandard; } }
    }


    public class AjaxLeadWeekPerformance : AjaxWeekPerformance
    {
        public override int LeadsStandard { get  {  return 70;  } }
        public override int FaxOutStandard { get { return 35; } }
        public override double CallHours { get { return 7.5; } }
    }


    public class AjaxSalesPerformance : AjaxWeekPerformance
    {
        public override int LeadsStandard { get { return 105; } }
        public override int FaxOutStandard { get { return 50; } }
        public override double CallHours { get { return 10; } }
    }

    public class AjaxManagerMonthPerformance : AjaxMonthPerformance
    {
    }

    public class AjaxManagerWeekPerformance : AjaxWeekPerformance
    {
        public IEnumerable <string> members { get; set; }
        public int MemberCount
        {
            get
            {
                if (members == null) return 0;
                return members.Count();
            }
        }

        public double AverageHours { get { return CallHours / members.Count(); } }
        public int AverageFax
        {
            get
            {
                if (MemberCount == 0) return 0;
                return _leadCalls.Where(l => l.LeadCallType.Code >= 30).Count() / MemberCount;
            }
        }
        public int AverageLead { get; set; }

        public override bool IsQualified
        {
            get
            {
                return base.IsQualified;
            }
        }
    }
}