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
    public class _DealBySales
    {
        [Display(Name="项目名称")]
        public string ProjectName { get; set; }
        [Display(Name = "入账总额")]
        public decimal IncomeAmount { get; set; }
        [Display(Name = "年")]
        public int? Year { get; set; }
        [Display(Name = "月")]
        public int? Month { get; set; }
        [Display(Name = "出单销售")]
        public string Sales { get; set; }
    }
}