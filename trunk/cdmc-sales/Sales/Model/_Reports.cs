using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Sales.Model;

namespace Model
{
    public class AjaxEmployeeCheckInDistribution
    {
        [Key]
        [Display(Name = "范围")]
        public string Range { get; set; }
        [Display(Name = "数量")]
        public int? Count { get; set; }
    }

    public class AjaxEmployeeCheckInByMonth
    {
        [Key]
        [Display(Name="姓名")]
        public string Name { get; set; }
        public decimal? CurrentMonthChickIn { get; set; }
        public decimal? OneMonthBeforeChickIn { get; set; }

        public decimal? TwoMonthBeforeChickIn { get; set; }
        public decimal? ThreeMonthBeforeChickIn { get; set; }
        public decimal? FourthMonthBeforeChickIn { get; set; }
        public decimal? FifthMonthBeforeChickIn { get; set; }
        public decimal? SixMonthBeforeChickIn { get; set; }

        public decimal? CurrentMonthTarget { get; set; }
        public decimal? OneMonthBeforeTarget { get; set; }
        public decimal? TwoMonthBeforeTarget { get; set; }
        public decimal? ThreeMonthBeforeTarget { get; set; }
        public decimal? FourthMonthBeforeTarget { get; set; }
        public decimal? FifthMonthBeforeTarget { get; set; }
        public decimal? SixMonthBeforeTarget { get; set; }

        public double? CurrentMonthBeforePercent
        {
            get
            {
                if (CurrentMonthTarget == null || CurrentMonthTarget == 0) return 0;
                return Utl.Utl.GetPercent(CurrentMonthChickIn, CurrentMonthTarget);
            }
        }
        public double? OneMonthBeforePercent
        {
            get
            {
                if (OneMonthBeforeTarget == null || OneMonthBeforeTarget == 0) return 0;
                return Utl.Utl.GetPercent(OneMonthBeforeChickIn, OneMonthBeforeTarget);
            }
        }
        public double? TwoMonthBeforePercent
        {
            get
            {
                if (TwoMonthBeforeTarget == null || TwoMonthBeforeTarget == 0) return 0;
                return Utl.Utl.GetPercent(TwoMonthBeforeChickIn, TwoMonthBeforeTarget);
            }
        }
        public double? ThreeMonthBeforePercent
        {
            get
            {
                if (ThreeMonthBeforeTarget == null || ThreeMonthBeforeTarget == 0) return 0;
                return Utl.Utl.GetPercent(ThreeMonthBeforeChickIn, ThreeMonthBeforeTarget);
            }
        }
        public double? FourthMonthBeforePercent
        {
            get
            {
                if (FourthMonthBeforeTarget == null || FourthMonthBeforeTarget == 0) return 0;
                return Utl.Utl.GetPercent(FourthMonthBeforeChickIn, FourthMonthBeforeTarget);
            }
        }
        public double? FifthMonthBeforePercent
        {
            get
            {
                if (FifthMonthBeforeTarget == null || FifthMonthBeforeTarget == 0) return 0;
                return Utl.Utl.GetPercent(FifthMonthBeforeChickIn, FifthMonthBeforeTarget);
            }
        }
        public double? SixMonthBeforePercent
        {
            get
            {
                if (SixMonthBeforeTarget == null || SixMonthBeforeTarget == 0) return 0;
                return Utl.Utl.GetPercent(SixMonthBeforeChickIn, SixMonthBeforeTarget);
            }
        }

        //public IEnumerable<AjaxEmployeeInMonth> ProjectLines { get; set; }
    }

    class AjaxEmployeeInMonth
    {
        public decimal? TotalTarget { get; set; }
        public decimal? CheckinTarget { get; set; }
        public decimal? CheckIn { get; set; }
        public decimal CallCount { get; set; }
        public double Percent { get; set; }
        public DateTime Range { get { return new DateTime(Year, Month.Value, 1); } }
        public int Year { get; set; }
        public int? Month { get; set; }
    }

    public class AjaxProjectCheckInByMonth
    {
        [Key]
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string Manager { get; set; }
        public decimal? CurrentMonthChickIn { get; set; }
        public decimal? OneMonthBeforeChickIn { get; set; }

        public decimal? TwoMonthBeforeChickIn { get; set; }
        public decimal? ThreeMonthBeforeChickIn { get; set; }
        public decimal? FourthMonthBeforeChickIn { get; set; }
        public decimal? FifthMonthBeforeChickIn { get; set; }
        public decimal? SixMonthBeforeChickIn { get; set; }

        public decimal? CurrentMonthTarget { get; set; }
        public decimal? OneMonthBeforeTarget { get; set; }
        public decimal? TwoMonthBeforeTarget { get; set; }
        public decimal? ThreeMonthBeforeTarget { get; set; }
        public decimal? FourthMonthBeforeTarget { get; set; }
        public decimal? FifthMonthBeforeTarget { get; set; }
        public decimal? SixMonthBeforeTarget { get; set; }

        public double? CurrentMonthBeforePercent
        {
            get
            {
                if (CurrentMonthTarget == null || CurrentMonthTarget == 0) return 0;
                return Utl.Utl.GetPercent(CurrentMonthChickIn, CurrentMonthTarget);
            }
        }
        public double? OneMonthBeforePercent
        {
            get
            {
                if (OneMonthBeforeTarget == null || OneMonthBeforeTarget == 0) return 0;
                return Utl.Utl.GetPercent(OneMonthBeforeChickIn, OneMonthBeforeTarget);
            }
        }
        public double? TwoMonthBeforePercent
        {
            get
            {
                if (TwoMonthBeforeTarget == null || TwoMonthBeforeTarget == 0) return 0;
                return Utl.Utl.GetPercent(TwoMonthBeforeChickIn, TwoMonthBeforeTarget);
            }
        }
        public double? ThreeMonthBeforePercent
        {
            get
            {
                if (ThreeMonthBeforeTarget == null || ThreeMonthBeforeTarget == 0) return 0;
                return Utl.Utl.GetPercent(ThreeMonthBeforeChickIn, ThreeMonthBeforeTarget);
            }
        }
        public double? FourthMonthBeforePercent
        {
            get
            {
                if (FourthMonthBeforeTarget == null || FourthMonthBeforeTarget == 0) return 0;
                return Utl.Utl.GetPercent(FourthMonthBeforeChickIn, FourthMonthBeforeTarget);
            }
        }
        public double? FifthMonthBeforePercent
        {
            get
            {
                if (FifthMonthBeforeTarget == null || FifthMonthBeforeTarget == 0) return 0;
                return Utl.Utl.GetPercent(FifthMonthBeforeChickIn, FifthMonthBeforeTarget);
            }
        }
        public double? SixMonthBeforePercent
        {
            get
            {
                if (SixMonthBeforeTarget == null || SixMonthBeforeTarget == 0) return 0;
                return Utl.Utl.GetPercent(SixMonthBeforeChickIn, SixMonthBeforeTarget);
            }
        }

        public IEnumerable<AjaxProjectPerformanceInMonth> ProjectLines { get; set; }
    }


    public class AjaxProjectCheckInMonthByProjectType
    {
        [Display(Name = "考察入账")]
        public decimal? KaoChaCheckIn { get; set; }
        [Display(Name = "Events入账")]
        public decimal? EventsCheckIn { get; set; }
        [Display(Name = "杂志入账")]
        public decimal? MagazineCheckIn { get; set; }
        [Display(Name = "青岛分公司入账")]
        public decimal? QingDaoSubComanyCheckIn { get; set; }
        [Display(Name = "考察入账目标")]
        public decimal? KaoChaTarget { get; set; }
        [Display(Name = "Events入账目标")]
        public decimal? EventsTarget { get; set; }
        [Display(Name = "杂志入账目标")]
        public decimal? MagazineTarget { get; set; }
        [Display(Name = "青岛分公司入账目标")]
        public decimal? QingDaoSubComanyTarget { get; set; }

        [Display(Name = "考察入账完成百分比")]
        public decimal? KaoChaPercent { get; set; }
        [Display(Name = "Events入账完成百分比")]
        public decimal? EventsPercent { get; set; }
        [Display(Name = "杂志入账完成百分比")]
        public decimal? MagazinePercent { get; set; }
        [Display(Name = "青岛分公司入账完成百分比")]
        public decimal? QingDaoSubComanyPercent { get; set; }

        // public double Percent { get; set; }
        public DateTime Range { get { return new DateTime(Year, Month, 1); } }
        public int Year { get; set; }
        public int Month { get; set; }
    }


    public class AjaxProjectPerformanceInMonth
    {
        public decimal? TotalTarget { get; set; }
        public decimal? CheckinTarget { get; set; }
        public decimal? CheckIn { get; set; }
        public decimal CallCount { get; set; }
        public double Percent { get; set; }
        public DateTime Range { get { return new DateTime(Year, Month.Value, 1); } }
        public int Year { get; set; }
        public int? Month { get; set; }
    }

    public class AjaxMember
    {
        public string Name { get; set; }
    }

    public class AjaxProjectPerformance
    {
        public double LeftDays { get; set; }
        public double Duration { get; set; }
        public decimal? Target { get; set; }
        public string ProjectCode { get; set; }
        public string Name_CH { get; set; }
        public IEnumerable<AjaxMember> Memebers { get; set; }
        public int ProjectID { get; set; }
        public IEnumerable<AjaxProjectPerformanceInMonth> AjaxProjectPerformanceInMonths { get; set; }

    }

    public class AjaxProject
    {
        public string ProjectName { get; set; }
        public string ProjectCode { get; set; }
        public int ProjectID { get; set; }
        public IEnumerable<AjaxCRM> CRMs { get; set; }
    }
}