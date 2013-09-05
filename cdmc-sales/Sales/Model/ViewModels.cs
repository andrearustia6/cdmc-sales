using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entity;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Utl;

namespace Model
{
    public class ViewCallListChart
    {
      
        DateTime? StartDate { get; set; }
        DateTime? EndDate { get; set; }
        public Member Member { get; set; }
        public List<ViewCompanyCallSum> ViewCompanyCallSums { get; set; }
        public List<ViewCategoryCallSum> ViewCategoryCallSum { get; set; }
      
    }
    public class ViewCategoryCallSum
    {

        //打了几个Company
        public int CompanyCalledCountNumber { get; set; }

        //Category的名称 
        public string CategoryName { get; set; }

        public string CategoryCountName { get { return CategoryName + ": 已打" + CompanyCalledCountNumber + "个公司"; } }
    }

    public class ViewCompanyCallSum
    {
        //打了几个lead
        public int LeadCalledCountNumber { get; set; }

        //大了X个lead的公司的个数 
        public int CompanyCount { get; set; }

        public string CompanyCountName { get { return "已打" + LeadCalledCountNumber + "个Lead" + "(" + CompanyCount + "家 )"; } }
    }

    public class JosonSalesInputData
    {
        public string SubmitType { get; set; }
        //CRID 为空,为添加新的公司的数据,crid不为空，为存在的公司添加销售数据
        public int? CRID { get; set; }
        public int? ProjectID { get; set; }
        public Company Company { get; set; }
        public Lead Lead { get; set; }
        public LeadCall LeadCall { get; set; }
        public string Message { get; set; }
        public bool Satisfied { get; set; }
    }

    public class  JosonCompany
    {
        public string username { get; set; }
        public Company Company { get; set; }
        public List<Lead> Leads { get; set; }
        public CompanyRelationship CompanyRelationship { get; set; }
        public List<LeadCall> LeadCalls { get; set; }
    }

    public class ViewPhoneInfo
    {
        public TimeSpan Duration { get; set; }
        public string Phone { get; set; }
        public int CallSum { get; set; }
        

    }

    public class ViewContactedLead
    {
        public int? CompanyRelationshipID { get; set; }
        public Lead Lead { get; set; }
        public List<LeadCall> LeadCalls{get ;set;}
        public LeadCall LastCall { get; set; }
        public int ID { get; set; }
    }

    public class ViewLeadCallSumAmount
    {
        [Display(Name="销售姓名")]
        public string Name { get; set; }
        [Display(Name = "销售职级")]
        public string SalesType { get; set; }
        [Display(Name = "出单总额")]
        public decimal DealSum { get; set; }
        [Display(Name = "出单总额")]
        public decimal CheckInSum { get; set; }
        [Display(Name = "通话时间总长")]
        public double DurationSum { get; set; }
        [Display(Name = "Call List 总数")]
        public int CallSum { get; set; }
    }

    public class ViewPerformanceData
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<DataRow> ViewPhoneInfos { get; set; }
        public int Month { get; set; }
        public List<Deal> Deals { get; set; }
        public List<Deal> SignedDeals { get; set; }
        public List<TargetOfMonthForMember> TargetOfMonthForMembers{ get; set; }
        public List<Lead> Leads { get; set; }
        public List<LeadCall> LeadCalls { get; set; }
        public List<ViewMemberDayWorkload> ViewMemberDayWorkloads { get; set; }
    }

    public class ViewMemberPerformance
    {
        //A
        [Display(Name = "月到账目标完成情况(A)")]
        public double TargetCompletePercentage
        {
            get
            {
                if (TargetOfMonthForMembersAmount == 0) return 0;
                var per =  Math.Round((double)(CheckinsAmount / TargetOfMonthForMembersAmount) * 100, 2);

                if (per >= 140) return 70;
                else if (per >= 120) return 60;
                else if (per >= 100) return 50;
                else if (per >= 80) return 40;
                else if (per >= 60) return 30;
                else return 0;
            }
        }
        //B
        [Display(Name = "可打Leads周目标完成情况(B)")]
        public double LeadAddCompletePercentage
        {
            get
            {
                var count = ViewMemberWeekWorkloads.FindAll(f => f.IsLeadAddQualified == false).Count;
                if (count == 0)
                    return 10;
                else if(count ==1)
                    return 5;
                else
                    return 0;


            }
        }
        //D
        [Display(Name = "每天Email/Fax标完成情况(D)")]
        public double DailyFaxOutCompletePercentage
        {
            get
            {
                var count = ViewMemberDayWorkloads.FindAll(f => f.IsFaxOutQualified == false).Count;
                if (count <=3)
                    return 30;
                else if (count <= 6 && count>=4)
                    return 15;
                else
                    return 0;

            }
        }
        //C
        [Display(Name = "每天电话时间(C)")]
        public double DailyOnPhoneCompletePercentage
        {
            get
            {
                var count = ViewMemberDayWorkloads.FindAll(f => f.IsPhoneQualified == false).Count;
                if (count <= 3)
                    return 10;
                else if (count <= 6 && count >= 4)
                    return 5;
                else
                    return 0;

            }
        }
        //E
        public double? _rate;
        [Display(Name = "考核系数(E)")]
        public double Rate
        {
            get
            {
                if (_rate == null)
                {
                    var count = ViewMemberWeekWorkloads.FindAll(f => f.IsWorkloadQualified == false).Count;
                    if (count == 0)
                        _rate = 1.2;
                    else if (count == 1)
                        _rate = 1;
                    else if (count == 2)
                        _rate = 0.8;
                    else
                        _rate = 0.5;
                }

                return _rate.Value;
            }
        }
        //总分
        [Display(Name = "奖金发放比例")]
        public double Total {
            get { return (TargetCompletePercentage + LeadAddCompletePercentage + DailyOnPhoneCompletePercentage + DailyFaxOutCompletePercentage) * Rate; }
        }
        [Display(Name="月份")]
        public int Month { get; set; }
        public List<Deal> Deals { get; set; }

        [Display(Name = "到账总额")]
        public decimal CheckinsAmount { get { return Deals.Sum(s=>s.Payment); } }
        public List<TargetOfMonthForMember> TargetOfMonthForMembers { get; set; }

        [Display(Name = "月到账目标总额")]
        public decimal TargetOfMonthForMembersAmount { get { return TargetOfMonthForMembers.Sum(s => s.CheckIn); } }

        //添加的leads
        public List<Lead> Leads { get; set; }
     
        public List<LeadCall> LeadCalls { get; set; }
        [Display(Name = "Fax Out")]
        public int LeadCallsCount { get { return LeadCalls.Count; } }
        [Display(Name = "员工姓名")]
        public string Name { get; set; }

        public List<ViewMemberDayWorkload> ViewMemberDayWorkloads { get; set; }

        public List<ViewMemberWeekWorkload> ViewMemberWeekWorkloads { get; set; }

        [Display(Name = "不达标天数")]
        public int BadWorkloadDaysCount { get { return ViewMemberDayWorkloads.Where(s => s.IsQualified==false).Count(); } }

        [Display(Name = "可打Leads不达标周数")]
        public int BadLeadAddedWeeksCount { get { return ViewMemberWeekWorkloads.Where(s => s.IsLeadAddQualified == false).Count(); } }

        [Display(Name = "工作量不达标周数")]
        public int BadWorkloadWeeksCount { get { return ViewMemberWeekWorkloads.Where(s => s.IsWorkloadQualified == false).Count(); } }
    }

    public class ViewMemberWeekWorkload
    {
        public ViewMemberWeekWorkload(List<Deal> deals,DateTime startdate,DateTime enddate)
        {
            StartDay = startdate;
            EndDay = enddate;
            var ds = from d in deals where d.SignDate >=  startdate && d.SignDate<= EndDay select d;
            Deals = ds.ToList();
        }
        public List<Deal> Deals { get; set; }
        public int FaxOutWeekStandard
        {
            get
            {

                if (Deals.Count > 0)
                    return 40;
                else
                    return 50;

            }
        }
        static int ResearchCount = 105;

        public string Name { get; set; }
        [Display(Name = "开始时间")]
        public DateTime StartDay{get;set;}
        [Display(Name = "结束时间")]
        public DateTime EndDay { get; set; }
        public List<Lead> Leads { get; set; }



        [Display(Name = "FaxOut数量")]
        public int FaxoutCount { get; set; }

        [Display(Name = "电话时间")]
        public TimeSpan OnPhoneDuration { get; set; }

        [Display(Name = "考核系数是否达标")]
        public bool IsWorkloadQualified
        {
            get
            {
                if (OnPhoneDuration.TotalHours >= 10 || FaxoutCount >= FaxOutWeekStandard)
                    return true;
                else
                    return false;
            }
        }

        [Display(Name = "可打Leads添加是否达标")]
        public bool IsLeadAddQualified
        {
            get
            {
                if (Leads.Count > ResearchCount)
                    return true;
                else
                    return false;
            }
        }
    }

    public class ViewMemberDayWorkload
    {
        //不出单情况10，出单为7
        static int Faxout = 10;
        static int Hours = 2;
        [Display(Name = "日期")]
        public String Day { get; set; }
        [Display(Name = "FaxOut数量")]
        public int FaxoutCount { get; set; }
        [Display(Name = "电话时间")]
        public TimeSpan OnPhoneDuration { get; set; }
        [Display(Name = "是否达标")]
        public bool IsQualified
        {
            get
            {
                if (OnPhoneDuration.TotalHours >= Hours || FaxoutCount >= Faxout)
                    return true;
                else
                    return false;
            }
        }

        [Display(Name = "是否FaxOut达标")]
        public bool IsFaxOutQualified
        {
            get
            {
                if ( FaxoutCount >= Faxout)
                    return true;
                else
                    return false;
            }
        }

        [Display(Name = "是否通话时间达标")]
        public bool IsPhoneQualified
        {
            get
            {
                if (OnPhoneDuration.TotalHours >= Hours )
                    return true;
                else
                    return false;
            }
        }

        [Display(Name = "销售")]
        public string Name { get; set; }
    }

    public class ViewLeadCallAmount
    {
        public Member Member { get; set; }
        public int Others { get; set; }
        public int Blowed { get; set; }
        public int Not_Pitched { get; set; }
        public int Pitched { get; set; }
        public int Full_Pitched { get; set; }
        public int Call_Backed { get; set; }
        public int Waiting_For_Approval { get; set; }
        public int Qualified_Decision { get; set; }
        public int Closed { get; set; }
        public int Cold_Calls { get; set; }
        public int CallListAmount { get; set; }
        public int DMs { get; set; }
        public int New_DMs { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime? startdate { get; set; }
        public DateTime? enddate { get; set; }
        public decimal DealInAmount { get; set; }
        public decimal CheckInAmount { get; set; }
        public string Phone { get; set; }
    }

    public class ViewLeadCallAmountInProject
    {
        public Project project { get; set; }
        public List<ViewLeadCallAmount> LeadCallAmounts { get; set; }
    }

    public class TotalLeadCallAmount
    {
        public List<ViewLeadCallSumAmount> TopSales { get; set; }
        public decimal TopSalesAmount { get; set; }
        public List<ViewLeadCallSumAmount> WorstCallers { get; set; }
        public List<ViewLeadCallSumAmount> TopCallers { get; set; }
        public List<ViewLeadCallAmountInProject> ViewLeadCallAmountInProjects { get; set; }
    }

    public class ViewProjectProgressAmount : ViewProgressAmount
    {
        public decimal RMBDealIn { get; set; }
        public decimal USDDealIn { get; set; }
        public decimal TotalDealInRMB { get { return RMBDealIn + USDDealIn * (decimal)CH.DB.CurrencyTypes.Where(c => c.Name == "USD").FirstOrDefault().Rate; } }
        public Project Project { get; set; }
        public int LeftDay { get; set; }
        public string NameWithCompletePercentage { get { 
            var v =   "["+Project.Name_CH+ "] 出单目标完成: " + DealInPercentage.ToString()+"%" +"  入账目标完成: " + CheckInPercentage.ToString()+"%";
            return v;
        } 
        }
    }

    public class ViewProjectMemberProgressAmount
    {
        public Project Project { get; set; }
        public List<ViewMemberProgressAmount> ViewMemberProgressAmounts { get; set; }
    }

    public class ViewProgressAmount 
    {
        public decimal TotalDealIn { get; set; }
        public decimal TotalCheckIn { get; set; }
        
        public decimal DealIn { get; set; }
        public decimal DealInTarget { get; set; }
        public decimal CheckIn { get; set; }
        public decimal CheckInTarget { get; set; }
        public int DealInPercentage { get; set; }
        public int CheckInPercentage { get; set; }
        public decimal NextDealInTarget { get; set; }
        public decimal NextCheckInTarget { get; set; }
    }

    public class ViewMemberProgressAmount : ViewProgressAmount
    {
        public Member Member { get; set; }
        public decimal? TotalDealinTarget{ get; set;}
    }

    public class ViewMemberLeadToCall
    {
        public LeadCall LeadCall { get; set; }
        public string LeadName { get { return LeadCall.Lead.Name; } }
        public string LeadTitle { get { return LeadCall.Lead.Title; } }
        public string Contact { get { return LeadCall.Lead.Contact; } }
        public string Mobile { get { return LeadCall.Lead.Mobile; } }
        public string Companyname { get { return LeadCall.CompanyRelationship.Company.Name; } }
        public string ProjectCode { get { return LeadCall.CompanyRelationship.Project.ProjectCode; } }
        public string ProjectName { get { return LeadCall.CompanyRelationship.Project.Name; } }
        public DateTime CallDateTime
        {
            get
            {
                var date = LeadCall.CallBackDate.Value;
                if (LeadCall.CompanyRelationship.Company.DistrictNumber != null)
                {
                    var differs = LeadCall.CompanyRelationship.Company.DistrictNumber.TimeDifference;
                    date = date.AddHours(-differs);
                }
                return date;
            }
        }

    }
}