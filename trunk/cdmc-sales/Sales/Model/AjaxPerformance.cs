using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

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
        [Display(Name = "开始时间")]
        public DateTime StartDate { get; set; }
        [Display(Name = "结束时间")]
        public DateTime EndDate { get; set; }
        public virtual int FaxStandard { get; set; }
        public virtual int LeadStandard { get; set; }
        public virtual double Hours { get; set; }
        public virtual bool IsQualified { get; set; }

    }


    public class AjaxLeadPerformance : AjaxWeekPerformance
    {
        public override int LeadStandard { get  {  return 70;  } }
        public override int FaxStandard { get { return 35; } }
        public override double Hours { get { return 7.5; } }
    }


    public class AjaxSalesPerformance : AjaxWeekPerformance
    {
        public override int LeadStandard { get { return 105; } }
        public override int FaxStandard { get { return 50; } }
        public override double Hours { get { return 10; } }
    }

    public class AjaxManagerMonthPerformance : AjaxMonthPerformance
    {
    }

    public class AjaxManagerWeekPerformance : AjaxWeekPerformance
    {
        public double AverageHours { get; set; }
        public int AverageFax { get; set; }
        public int AverageLead { get; set; }
    }
}