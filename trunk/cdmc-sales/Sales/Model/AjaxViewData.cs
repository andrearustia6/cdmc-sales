using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    //项目的
    public class AjaxViewProjecMonthPerformanceData
    {
        public int? ProjectID { get; set; }
        [Display(Name = "项目名称")]
        public string ProjectName { get; set; }

        [Display(Name = "版块负责人")]
        public string Manager { get; set; }

        [Display(Name = "CheckIn月目标")]
        public decimal CheckInTarget { get; set; }

        [Display(Name = "DealIn月目标")]
        public decimal DealInTarget { get; set; }

        [Display(Name = "实际月CheckIn")]
        public decimal CheckIn { get; set; }

        [Display(Name = "实际月DealIn")]
        public decimal DealIn { get; set; }

        [Display(Name = "项目总目标")]
        public decimal TotalDealInTarget { get; set; }

        public List<AjaxViewProjectWeekPerformance> AjaxViewProjectWeekPerformances { get; set; }


    }

    public class AjaxViewProjectWeekPerformance
    {
        public int? ProjectID { get; set; }
        [Display(Name = "项目名称")]
        public string ProjectName { get; set; }

        [Display(Name = "版块负责人")]
        public string Manager { get; set; }

        [Display(Name = "开始时间")]
        public DateTime StartDate { get; set; }

        [Display(Name = "结束时间")]
        public DateTime EndDate { get; set; }

        [Display(Name = "CheckIn周目标")]
        public decimal CheckInTarget { get; set; }

        [Display(Name = "DealIn周目标")]
        public decimal DealInTarget { get; set; }

        [Display(Name = "实际周CheckIn")]
        public decimal CheckIn { get; set; }

        [Display(Name = "实际周DealIn")]
        public decimal DealIn { get; set; }
    }

    public class AjaxViewCallListData
    {
        public int? ProjectID { get; set; }
        [Display(Name = "人名")]
        public string LeadName { get { return Utl.Utl.GetFullName(LeadNameCH, LeadNameEN); } }
        public  string LeadNameCH { get; set; }
        public string LeadNameEN { get; set; }
        public int? LeadID { get; set; }

        [Display(Name = "客户职位")]
        public string Title { get; set; }

        [Display(Name = "成熟度")]
        public string Progress { get; set; }

        [Display(Name = "公司名")]
        public string CompanyName { get { return Utl.Utl.GetFullName(CompanyNameCH, CompanyNameEN); } }
        public string CompanyNameCH { get; set; }
        public string CompanyNameEN { get; set; }
   
        public string Mobile { get; set; }

        [Display(Name = "核心业务")]
        public string Categorys { get; set; }

        [Display(Name = "移动电话")]
        public string MobileDisplay
        {
            get
            {

                var m = Mobile; if (string.IsNullOrEmpty(m)) return string.Empty;
                string start = string.Empty;
                if ( m.Length > 3)
                {
                    var hide = m.Substring(3, m.Length - 3);
                    var hidecount = hide.Count();

                    for (int i = 0; i < hidecount; i++)
                    {
                        start += "*";
                    }


                }
                return m.Substring(0, 3) + start;
            }
        }

        [Display(Name = "客户直线")]
        public string ContactDisplay
        {
            get
            {

                var m = Contact;
                if (string.IsNullOrEmpty(m)) return string.Empty;
                if (m.Length <= 3) return m;
                string start = string.Empty;
                if (!string.IsNullOrEmpty(m) && m.Length > 3)
                {
                    var hide = m.Substring(3, m.Length - 3);
                    var hidecount = hide.Count();

                    for (int i = 0; i < hidecount; i++)
                    {
                        start += "*";
                    }


                }
                return m.Substring(0, 3) + start;
            }
        }
   
        public string Contact { get; set; }

        [Display(Name = "所属客户关系")]
        public int? CompanyRelationshipID { get; set; }

        [Display(Name = "致电类型")]
        public string LeadCallType { get; set; }

        public int CallTypeCode { get; set; }
        [Display(Name = "致电销售")]
        public string Member  { get; set; }


        [Required, Display(Name = "致电时间")]
        public DateTime CallDate { get; set; }

        [Display(Name = "是否有效")]
        public bool FaxOut{ get; set; } 

        [Display(Name = "结果描述")]
        public string Result { get; set; }

        [Display(Name = "回电时间")]
        public DateTime? CallBackDate { get; set; }
    }
}