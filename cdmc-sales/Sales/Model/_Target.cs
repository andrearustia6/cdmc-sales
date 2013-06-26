using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sales.Model
{
    public class _TargetOfMonthStatus
    {
        [Display(Name="项目名称")]
        public  string ProjectName { get; set; }
        [Display(Name = "项目编号")]
        public string ProjectCode { get; set; }
        [Display(Name = "版块负责人")]
        public  string Mangager { get; set; }
        [Display(Name = "已设置月目标")]
        public  bool HasTargetOfMonth { get; set; }
        [Display(Name = "月目标划分到周目标")]
        public  bool HasTargetOfWeek { get; set; }
        
    }
}