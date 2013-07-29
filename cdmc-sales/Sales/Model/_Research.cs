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
        [Display(Name = "项目编码")]
        public int ProjectID { get; set; }
        [Display(Name = "项目编码")]
        public string ProjectCode { get; set; }
        [Display(Name = "项目名称")]
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
        [Display(Name = "姓名")]
        public string UserName { get; set; }

        [Display(Name = "入职时间（月）")]
        public int EmployeeDuration { get; set; }
    }

    public class _UserResearchDetail
    {
        [Display(Name = "姓名")]
        public string UserName { get; set; }

        [Display(Name = "公司名称")]
        public string CompanyName
        {
            get
            {
                return Utl.Utl.GetFullString(",", CompanyNameEN, CompanyNameCH);
            }
        }
        [Display(Name = "添加销售")]
        public string Creator { get; set; }

        [Display(Name = "公司名称")]
        public string CompanyNameCH { get; set; }

        [Display(Name = "公司名称")]
        public string CompanyNameEN { get; set; }

        [Display(Name = "Lead姓名")]
        public string LeadNameEN { get; set; }

        [Display(Name = "Lead姓名")]
        public string LeadNameCH { get; set; }

        [Display(Name = "移动电话")]
        public string LeadMobile { get; set; }

        [Display(Name = "职位")]
        public string LeadTitle { get; set; }


        public DateTime? CreateDate { get; set; }
        [Display(Name = "Lead姓名")]
        public string LeadName
        {
            get
            {
                return Utl.Utl.GetFullString(",", LeadNameEN, LeadNameCH);
            }
        }

        [Display(Name = "公司总机")]
        public string CompanyContact { get; set; }

        [Display(Name = "Lead直线")]
        public string LeadContact { get; set; }

        [Display(Name = "公司总机")]
        public string HideCompanyContact { get { return Utl.Utl.HidePhoneNumber(CompanyContact); } }

        [Display(Name = "Lead直线")]
        public string HideLeadContact { get { return Utl.Utl.HidePhoneNumber(LeadContact); } }

        [Display(Name = "移动电话")]
        public string HideLeadMobile { get { return Utl.Utl.HidePhoneNumber(LeadMobile); } }
        [Display(Name = "细分行业")]
        public string Categoris { get; set; }

        [Display(Name = "Lead邮件")]
        public string Email { get; set; }

        [Display(Name = "公司调研")]
        public string CompanyDesicription { get; set; }
    }


    public class _CompanyResearchDetail
    {
        public int ID { set; get; }

        [Display(Name = "公司名称")]
        public string CompanyName
        {
            get
            {
                return Utl.Utl.GetFullString(",", CompanyNameEN, CompanyNameCH);
            }
        }
        [Display(Name = "添加销售")]
        public string Creator { get; set; }

        [Display(Name = "添加日期")]
        public DateTime? CreateDate { get; set; }

        [Display(Name = "公司名称")]
        public string CompanyNameCH { get; set; }

        [Display(Name = "公司名称")]
        public string CompanyNameEN { get; set; }

        [Display(Name = "公司总机")]
        public string CompanyContact { get; set; }

        [Display(Name = "细分行业")]
        public string Categoris { get; set; }

        [Display(Name = "公司调研")]
        public string CompanyDesicription { get; set; }

        [Display(Name = "是否有效")]
        public string IsValid { get; set; }

        [Display(Name = "公司点评")]
        public string CompanyReviews { get; set; }

        [Display(Name = "公司简介")]
        public string Description { get; set; }
    }

    public class _LeadResearchDetail
    {
        public int ID { set; get; }

        [Display(Name = "姓名")]
        public string UserName { get; set; }

        [Display(Name = "添加销售")]
        public string Creator { get; set; }

        [Display(Name = "Lead姓名")]
        public string LeadNameEN { get; set; }

        [Display(Name = "Lead姓名")]
        public string LeadNameCH { get; set; }

        [Display(Name = "移动电话")]
        public string LeadMobile { get; set; }

        [Display(Name = "职位")]
        public string LeadTitle { get; set; }

        [Display(Name = "添加日期")]
        public DateTime? CreateDate { get; set; }

        [Display(Name = "Lead姓名")]
        public string LeadName
        {
            get
            {
                return Utl.Utl.GetFullString(",", LeadNameEN, LeadNameCH);
            }
        }

        [Display(Name = "Lead直线")]
        public string LeadContact { get; set; }

        [Display(Name = "Lead直线")]
        public string HideLeadContact { get { return Utl.Utl.HidePhoneNumber(LeadContact); } }

        [Display(Name = "移动电话")]
        public string HideLeadMobile { get { return Utl.Utl.HidePhoneNumber(LeadMobile); } }

        [Display(Name = "Lead邮件")]
        public string Email { get; set; }

        [Display(Name = "是否有效")]
        public string IsValid { get; set; }
    }

}