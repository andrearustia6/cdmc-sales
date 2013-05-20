using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sales.Model
{
    public class _DealReport
    {
        public _DealBySales DealBySales { get; set; } 
    }


    public class _DealBase
    {
         [Display(Name = "入账")]
        public decimal? IncomeAmount { get; set; }

        [Display(Name = "出单")]
        public decimal? DealAmount { get; set; }

        public double Percent { get { return Utl.Utl.GetPercent(IncomeAmount.Value, DealAmount.Value); } }

        [Display(Name = "回款/出单比率")]
        public string PercentString { get { return Percent.ToString() + "%"; } }

        [Display(Name = "年")]
        public int? Year { get; set; }

        [Display(Name = "月")]
        public int? Month { get; set; }

      


        [Display(Name = "所有入账总额")]
        public decimal? TotalIncomeAmount { get; set; }
    }

    public class _DealByProject: _DealBase
    {
        [Display(Name = "项目名称")]
        public string ProjectName { get; set; }

        [Display(Name = "项目人数")]
        public int MemberCount { get; set; }

        public string DurationCategory { get { return Year.ToString() + "年" + Month.ToString() + "月"; } }
          [Display(Name = "人均入账")]
        public decimal? Average { get { return Utl.Utl.GetAverage(IncomeAmount, MemberCount); } }
    }
    public class _DealByProjectData
    {

        public IQueryable<_DealByProject> _DealByProject { get; set; }

        public IQueryable<_DealByProjectInMonth> _DealByProjectInMonth { get; set; }
    }


    public class _DealByProjectInMonth 
    {
        [Display(Name = "项目名称")]
        public string ProjectName { get; set; }

        [Display(Name = "项目入账")]
        public decimal? IncomeAmount { get; set; }

        [Display(Name = "年")]
        public int? Year { get; set; }

        [Display(Name = "月")]
        public int? Month { get; set; }

        public string DurationCategory { get { return Year.ToString() + "年" + Month.ToString() + "月"; } }
    }

    public class _DealBySales : _DealBase
    {
        [Display(Name="项目名称")]
        public string ProjectName { get; set; }

        [Display(Name = "出单销售")]
        public string Sales { get; set; }

    
    }
}