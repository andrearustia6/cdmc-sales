using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class AjaxPerformance:AjaxStatistics
    {
        public string Name { get; set; }
        public int performanceRate { get; set; }
        public string ProjectName { get; set; }
        public string TL { get; set; }
        public string Manager { get; set; }
        public virtual int FaxStandard {get;set;}
        public virtual int LeadStandard {get;set;}
        public virtual double Hours { get; set; }
        public int AssignedScore { get; set; }

    }

    public class AjaxLeadPerformance : AjaxPerformance
    {
        public override int LeadStandard { get  {  return 70;  } }
        public override int FaxStandard { get { return 35; } }
        public override double Hours { get { return 7.5; } }
    }


    public class AjaxSalesPerformance : AjaxPerformance
    {
        public override int LeadStandard { get { return 105; } }
        public override int FaxStandard { get { return 50; } }
        public override double Hours { get { return 10; } }
    }

    public class AjaxManagerPerformance : AjaxPerformance
    {
        public override int LeadStandard { get { return 105; } }
        public override int FaxStandard { get { return 50; } }
        public override double Hours { get { return 10; } }


        public double AverageHours { get; set; }
        public int AverageFax { get; set; }
        public int AverageLead { get; set; }
    }
}