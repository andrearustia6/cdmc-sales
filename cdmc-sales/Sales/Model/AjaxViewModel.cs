using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Entity;

namespace Model
{
    public class AjaxViewParticipant
    {
        public int ID { get; set; }
        public int? DealID { get; set; }

        [Display(Name = "参会人名称")]
        public string Name { get; set; }

        [Display(Name = "职位")]
        public string Title { get; set; }

        [Display(Name = "性别")]
        public string Gender { get; set; }

        [Display(Name = "直线电话")]
        public string Contact { get; set; }

        [Display(Name = "移动电话")]
        public string Mobile { get; set; }

        [Display(Name = "工作邮箱")]
        public string Email { get; set; }

        [Display(Name = "参会类型")]
        public string  ParticipantTypeName { get { return Utl.Utl.GetFullName(ParticipantTypeNameCH, ParticipantTypeNameEN); } }
        public string ParticipantTypeNameCH { get; set; }
        public string ParticipantTypeNameEN { get; set; }

        public string  ProjectCode { get; set; }
        public int? ProjectID { get; set; }
    }

    public class AjaxViewLeadInProject
    {
        public Lead Lead { get; set; }
        public int ProjectID { get; set; }
    }

    public class AjaxViewProject
    {
        public int ProjectID { get; set; }
        [Display(Name = "项目名称")]
        public string ProjectName { get; set; }
        [Display(Name = "项目编号")]
        public string ProjectCode { get; set; }
        [Display(Name = "开始时间")]
        public string StartDayString { get { return StartDay.ToShortDateString(); } }
        [Display(Name = "结束时间")]
        public string EndDayString { get { return EndDay.ToShortDateString(); } }


        public DateTime StartDay { get; set; }
        public DateTime EndDay { get; set; }


        [Display(Name="剩余工作日")]
        public TimeSpan LeftWorkingDays { get; set; }
        [Display(Name = "版块负责人")]
        public string Manager { get; set; }

        [Display(Name = "团队负责人")]
        public string Lead { get; set; }

        [Display(Name = "项目销售目标")]
        public decimal ProjectTarget { get; set; }

        [Display(Name = "项目总DealIn")]
        public decimal? TotalDealIn { get; set; }

        [Display(Name = "项目总CheckIn")]
        public decimal? TotalCheckIn { get; set; }

        [Display(Name = "项目总公司数")]
        public int TotalCompanysCount { get; set; }



        [Display(Name = "项目总Lead数")]
        public int TotalLeadsCount { get; set; }

        [Display(Name = "项目总Call数")]
        public int TotalCalls { get; set; }

        [Display(Name = "项目总出单数")]
        public int TotalDealCounts { get; set; }

        [Display(Name = "项目完成度")]
        public double ComplatePercetage {
            get {
                if (ProjectTarget == 0 || TotalCheckIn==null)
                    return 0;
                else
                {
                    var p = (double)(TotalCheckIn*100/ProjectTarget);
                     var v = Math.Round(p,2);
                     return v;
                }
                   
            } 
        }

    }
    public class AjaxViewDeal 
    {
        public int ID { get; set; }

        [Display(Name = "客户公司")]
        public string CompanyName { get { return Utl.Utl.GetFullName(CompanyNameCH, CompanyNameEN); } }

         public string CompanyNameEN{get;set;}

         public string CompanyNameCH { get; set; }
        [Display(Name = "项目编号")]
        public string ProjectCode { get; set; }
        
        public int? ProjectID { get; set; }

        [Display(Name = "客户签单人")]
        public string Committer { get; set; }

        [Display(Name = "签单人联系方式")]
        public string CommitterContect { get; set; }

        [Display(Name = "签单人邮箱")]
        public string CommitterEmail { get; set; }

        [Display(Name = "权益描述")]
        public string TicketDescription { get; set; }

        [Display(Name = "Package名称")]
        public string PackageName { get { return Utl.Utl.GetFullName(PackageNameCH, PackageNameEN); } }

        public string PackageNameCH { get; set; }
        public string PackageNameEN { get; set; }

        [Display(Name = "坏账")]
        public bool Abandoned { get; set; }

        [Display(Name = "坏账原因")]
        public string AbandonReason { get; set; }

        [Display(Name = "合约付款日期")]
        public DateTime ExpectedPaymentDate { get; set; }

        [Display(Name = "实际付款日期")]
        public DateTime? ActualPaymentDate { get; set; }

        [Display(Name = "签约日期")]
        public DateTime? SignDate { get; set; }

        [Display(Name = "是否付款")]
        public bool IsClosed { get; set; }

        [Display(Name = "实际入账")]
        public decimal Income { get; set; }

        [Display(Name = "出单人")]
        public string Sales { get; set; }

        [Display(Name = "应付款")]
        public decimal Payment { get; set; }

        [Display(Name = "出单描述")]
        public string PaymentDetail { get; set; }
    }
    public class AjaxViewPercentage
    {
        [Display(Name = "CheckIn目标")]
        public decimal CheckInTarget { get; set; }

        [Display(Name = "DealIn目标")]
        public decimal DealInTarget { get; set; }

        [Display(Name = "实际CheckIn")]
        public decimal CheckIn { get; set; }

        [Display(Name = "实际DealIn")]
        public decimal DealIn { get; set; }

        public string DealComplate
        {
            get
            {
                var p = DealInTarget == 0 ? "0%" : (Math.Round((DealIn / DealInTarget), 2) * 100) + "%";
                return "DealIn完成" + p;
            }
        }

        public string CheckComplate
        {
            get
            {
                var p = CheckInTarget == 0 ? "0%" : (Math.Round((CheckIn / CheckInTarget), 2) * 100) + "%";
                return "CheckIn完成" + p;
            }
        }

        public string ComplatePercentage
        {
            get { return CheckComplate + " " + DealComplate; }
        }
    }
    //项目的
    public class AjaxViewProjectMonthPerformance : AjaxViewPercentage
    {
        public int? ProjectID { get; set; }
        [Display(Name = "项目名称")]
        public string ProjectName { get; set; }

        [Display(Name = "版块负责人")]
        public string Manager { get; set; }

        [Display(Name = "项目总目标")]
        public decimal TotalDealInTarget { get; set; }

        public List<AjaxViewProjectWeekPerformance> AjaxViewProjectWeekPerformances { get; set; }

    }

    public class AjaxViewProjectWeekPerformance : AjaxViewPercentage
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