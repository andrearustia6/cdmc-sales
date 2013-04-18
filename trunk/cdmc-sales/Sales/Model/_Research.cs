using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Utl;

namespace Sales.Model
{
    //public class _ResearchData 
    //{
    //    public int Month{get;set;}
    //    public List<string> Weeks{get;set;}
    //    public IQueryable<_ProjectResearch> _ProjectResearch { get; set; }
    //    public IQueryable<_UsertResearch> _UsertResearch { get; set; }
    //}

    public class _ResearchCount
    {
        public int ID { get; set; }
        [Display(Name = "第1周公司数")]
        public int FirstWeekCompanyCount { get; set; }

        [Display(Name = "第2周公司数")]
        public int SecondWeekCompanyCount { get; set; }

        [Display(Name = "第3周公司数")]
        public int ThirdWeekCompanyCount { get; set; }

        [Display(Name = "第4周公司数")]
        public int FourthWeekCompanyCount { get; set; }

        [Display(Name = "第5周公司数")]
        public int FivethWeekCompanyCount { get; set; }

        [Display(Name = "第1周Lead数")]
        public int FirstWeekLeadCount { get; set; }

        [Display(Name = "第2周Lead数")]
        public int SecondWeekLeadCount { get; set; }

        [Display(Name = "第3周Lead数")]
        public int ThirdWeekLeadCount { get; set; }

        [Display(Name = "第4周Lead数")]
        public int FourthWeekLeadCount { get; set; }

        [Display(Name = "第5周Lead数")]
        public int FivethWeekLeadCount { get; set; }
    }

    public class _ProjectResearch : _ResearchCount
    {
        [Display(Name="项目名称")]
        public string ProjectName { get; set; }
        [Display(Name = "项目人数")]
        public int MemberCount { get; set; }
        [Display(Name = "人均公司添加")]
        public double CompanyAverage { get; set; }
        [Display(Name = "人均Lead添加")]
        public double LeadAverage { get; set; }
    }

    public class _UserResearch : _ResearchCount
    {
        [Display(Name = "项目名称")]
        public string UserName { get; set; }

        [Display(Name = "入职时间（月）")]
        public int EmployeeDuration { get; set; }
    }
}