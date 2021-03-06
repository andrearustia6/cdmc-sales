﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entity;
using System.ComponentModel.DataAnnotations;
using BLL;
using System.ComponentModel;
using System.Web.Mvc;

namespace Sales.Model
{
    public class _ManagerPerformance
    {
        public string Name { get; set; }
        public decimal? Target { get; set; }
        public decimal? CheckIn { get; set; }
        public IEnumerable<Project> Projects { private get; set; }
        public string ProjectString { get { return string.Join(",", Projects.Select(s => s.ProjectCode)); } }

    }

    //Lead当月的考核
    public class _TeamLeadPerformance
    {
        [ReadOnly(true)]
        [ScaffoldColumn(false)]
        public int ID { get; set; }
        [ScaffoldColumn(false)]
        public int RoleLevel { get; set; }
        [ScaffoldColumn(false)]
        public string User { get; set; }
        [Display(Name = "调研不达标周数")]
        public int LeadNotQualifiedWeeksCount
        {
            get
            {
                if (TeamLeadPerformanceInWeeks != null)
                    return TeamLeadPerformanceInWeeks.Where(c => c.IsLeadAddedQualified == false).Count();
                else
                    return 0;
            }
        }

        [Display(Name = "通话|FaxOut不达标周数")]
        public int HoursOrFaxNotQualifiedWeeksCount
        {
            get
            {
                if (TeamLeadPerformanceInWeeks != null)
                    return TeamLeadPerformanceInWeeks.Count(c => c.IsFaxOutOrCallHoursQualified == false);
                else
                    return 0;
            }
        }

        [Display(Name = "考核系数")]
        public double? Rate
        {
            get; set;
        }

        [Display(Name = "入账分数")]
        public int CheckinScore
        {
            get
            {
                return CRM_Logical._EmployeePerformance.GetCheckInScore(CompletePercent);
              
            }
        }

        [Display(Name = "调研分数")]
        public int AddLeadScore
        {
            get
            {
                return CRM_Logical._EmployeePerformance.GetLeadAddScore(LeadNotQualifiedWeeksCount);
            }
        }

        [Display(Name = "FaxOut分数")]
        public int FaxCallScore
        {
            get
            {

                return CRM_Logical._EmployeePerformance.GetFaxOutScore(HoursOrFaxNotQualifiedWeeksCount);
            }
        }

        [Display(Name = "考核总分数")]
        public int Score
        {
            get
            {
                if (AssignedScore == null) AssignedScore = 0;
                if (Rate == null) Rate = 1;
                return (int)(Rate * (CheckinScore + AddLeadScore + FaxCallScore + AssignedScore.Value));
            }
        }

        [Display(Name = "主观评分")]
        public double? AssignedScore { get; set; }

        [Display(Name = "员工")]
        public string Name { get; set; }

        [Display(Name = "评分人")]
        public string Assigner { get; set; }

        [Display(Name = "考核系数设置者")]
        [ScaffoldColumn(false)]
        public string RateAssigner { get; set; }

        [Display(Name = "月目标")]
        public decimal? Target { get; set; }

        public string TargetUnSetProjects { get; set; }

        [Display(Name = "月入账额")]
        public decimal? CheckIn { get; set; }
        
        [Display(Name = "入账目标完成百分比")]
        public double? CompletePercent { get {
            if (Target == null || Target == 0 || CheckIn==null||CheckIn==0) return 0;

            return Utl.Utl.GetPercent((double)CheckIn, (double)Target);
        }
        }

        public IEnumerable<_TeamLeadPerformanceInWeek> TeamLeadPerformanceInWeeks { get; set; }
        [Display(Name = "Faxout详细")]
        public string FaxOutCountString
        {
            get
            {
                if (TeamLeadPerformanceInWeeks != null)
                {
                    var counts = TeamLeadPerformanceInWeeks.OrderBy(o=>o.StartDate).Select(s => s.FaxOutCount);
                    return string.Join(",", counts);
                }
                else
                {
                    return string.Join(",", "0");
                }

            }
        }

        [Display(Name = "调研详细")]
        public string LeadAddCountString
        {
            get
            {
                if (TeamLeadPerformanceInWeeks != null)
                {
                    var counts = TeamLeadPerformanceInWeeks.OrderBy(o => o.StartDate).Select(s => s.LeadsCount);
                    return string.Join(",", counts);
                }
                else
                    return string.Join(",", 0);
            }
        }

        public string DealsCountString
        {
            get
            {
                if (TeamLeadPerformanceInWeeks != null)
                {
                    var counts = TeamLeadPerformanceInWeeks.Select(s => s.DealsCount);
                    return string.Join(",", counts);
                }
                else
                    return string.Join(",", 0);
            }
        }
    }

    //Lead当周的考核
    public class _TeamLeadPerformanceInWeek
    {
        //当周的faxout数目
        public int FaxOutCount { get; set; }
        //当周的Lead添加数目
        public int LeadsCount { get; set; }
        //当周出单的数目
        public int DealsCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsFaxOutOrCallHoursQualified
        {
            get
            {

                return FaxOutCount >= FaxOutStandard;
            }
        }


        public bool? IsLeadAddedQualified
        {
            get
            {
                return LeadsCount >= LeadAddStandard;
            }
        }

        /// <summary>
        /// 当周出单三次或以上，28个faxout达标，否则35个faxout达标
        /// </summary>
        public int FaxOutStandard{
            get{

                return CRM_Logical._EmployeePerformance.GetLeadFaxoutStandard(DealsCount);
            }
        }

        /// <summary>
        /// 当周出单三次或以上，60个Lead添加达标，否则70个Lead添加达标
        /// </summary>
        public  int LeadAddStandard
        {
            get
            {
                return CRM_Logical._EmployeePerformance.GetLeadAddStandard(DealsCount);
            }
        }

        /// <summary>
        /// 包含出单时间和入账时间都在内的deals
        /// </summary>
        public IEnumerable<Deal> Deals { get; set; }
        public IEnumerable<LeadCall> Calls { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    //Sales当月的考核
    public class _SalesPerformance
    {
        [ReadOnly(true)]
        [ScaffoldColumn(false)]
        public int ID { get; set; }
        [ScaffoldColumn(false)]
        public string User { get; set; }
        [ScaffoldColumn(false)]
        public int RoleLevel { get; set; }
        [Display(Name = "调研不达标周数")]
        public int LeadNotQualifiedWeeksCount
        {
            get
            {
                if (SalesPerformanceInWeeks != null)
                    return SalesPerformanceInWeeks.Count(c => c.IsLeadAddedQualified == false);
                    //return SalesPerformanceInWeeks.Sum(c => c.LeadAddedQualifiedCount);
                else
                    return 0;
            }
        }

        [Display(Name = "通话|FaxOut不达标周数")]
        public int HoursOrFaxNotQualifiedWeeksCount
        {
            get
            {
                if (SalesPerformanceInWeeks != null)
                    return SalesPerformanceInWeeks.Count(c => c.IsFaxOutOrCallHoursQualified == false);
                    //return SalesPerformanceInWeeks.Sum(c => c.FaxOutOrCallHoursQualifiedCount);
                else
                    return 0;
            }
        }


        [Display(Name = "考核系数")]
        public double? Rate
        {
            get;
            set;
        }

        [Display(Name = "入账分数")]
        public int CheckinScore
        {
            get
            {
                return CRM_Logical._EmployeePerformance.GetCheckInScore(CompletePercent);

            }
        }

        [Display(Name = "调研分数")]
        public int AddLeadScore
        {
            get
            {
                if (SalesPerformanceInWeeks != null)
                {
                    var count = SalesPerformanceInWeeks.Sum(c => c.LeadAddedQualifiedCount);
                    //return CRM_Logical._EmployeePerformance.GetLeadAddScore(LeadNotQualifiedWeeksCount);
                    if (count > 20)
                        return 20;
                    else
                        return count;
                }
                else
                    return 0;
            }
        }

        [Display(Name = "FaxOut分数")]
        public int FaxCallScore
        {
            get
            {
                if (SalesPerformanceInWeeks != null)
                {
                    var count = SalesPerformanceInWeeks.Sum(c => c.FaxOutOrCallHoursQualifiedCount);
                    //return CRM_Logical._EmployeePerformance.GetFaxOutScore(HoursOrFaxNotQualifiedWeeksCount);
                    if (count > 20)
                        return 20;
                    else
                        return count;
                }
                else
                    return 0;
            }
        }

        [Display(Name = "考核总分数")]
        public int Score
        {
            get
            {
                if (AssignedScore == null) AssignedScore = 0;
                if (Rate == null) Rate = 1;
                return (int)(Rate * (CheckinScore + AddLeadScore + FaxCallScore + AssignedScore.Value));
            }
        }

        [Display(Name = "主观评分")]
        public double? AssignedScore { get; set; }
        [Display(Name = "员工")]
        public string Name { get; set; }
        [Display(Name = "考核系数设置者")]
        [ScaffoldColumn(false)]
        public string RateAssigner { get; set; }
        [Display(Name = "月目标")]
        public decimal? Target { get; set; }
        [Display(Name = "月入账额")]
        public decimal? CheckIn { get; set; }

        [Display(Name = "入账目标完成百分比")]
        public double? CompletePercent
        {
            get
            {
                if (Target == null || Target == 0 || CheckIn == null || CheckIn == 0) return 0;

                return Utl.Utl.GetPercent((double)CheckIn, (double)Target);
            }
        }
        public IEnumerable<_SalesPerformanceInWeek> SalesPerformanceInWeeks { private get; set; }

        [Display(Name = "Faxout详细")]
        public string FaxOutCountString
        {
            get
            {
                if (SalesPerformanceInWeeks != null)
                {
                    var counts = SalesPerformanceInWeeks.OrderBy(o=>o.StartDate).Select(s => s.FaxOutCount);
                    return string.Join(",", counts);
                }
                else
                    return string.Join(",", "0");
            }
        }

        [Display(Name = "调研详细")]
        public string LeadAddCountString
        {
            get
            {
                if (SalesPerformanceInWeeks != null)
                {
                    var counts = SalesPerformanceInWeeks.OrderBy(o => o.StartDate).Select(s => s.LeadsCount);
                    return string.Join(",", counts);
                }
                else
                    return string.Join(",", "0");
            }
        }

        public string DealsCountString
        {
            get 
            {
                if (SalesPerformanceInWeeks != null)
                {
                    var counts = SalesPerformanceInWeeks.Select(s => s.DealsCount);
                    return string.Join(",", counts);
                }
                else
                    return string.Join(",", "0");
            }
        }

    }

    //Lead当周的考核
    public class _SalesPerformanceInWeek
    {
        //当周的faxout数目
        public int FaxOutCount { get; set; }
        //当周的Lead添加数目
        public int LeadsCount { get; set; }
        //当周出单的数目
        public int DealsCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsFaxOutOrCallHoursQualified
        {
            get
            {
                return FaxOutCount >= FaxOutStandard;
            }
        }
        public int FaxOutOrCallHoursQualifiedCount
        {
            get
            {
                if (DealsCount >= 3)
                {
                    if (FaxOutCount >= 40)
                        return 5;
                    else
                        return FaxOutCount / 8;
                }
                else
                {
                    if (FaxOutCount >= 50)
                        return 5;
                    else
                        return FaxOutCount / 10;
                }
            }
        }

        public bool IsLeadAddedQualified
        {
            get
            {
                return LeadsCount >= LeadAddStandard;
            }
        }
        public int LeadAddedQualifiedCount
        {
            get
            {
                if (DealsCount >= 3)
                {
                    if (LeadsCount >= 80)
                        return 5;
                    else
                        return LeadsCount / 16;
                }
                else
                {
                    if (LeadsCount >= 105)
                        return 5;
                    else
                        return LeadsCount / 21;
                }
            }
        }

        /// <summary>
        /// 当周出单三次或以上，28个faxout达标，否则35个faxout达标
        /// </summary>
        public int FaxOutStandard
        {
            get
            {
                return CRM_Logical._EmployeePerformance.GetSalesFaxoutStandard(DealsCount);
            }
        }

        /// <summary>
        /// 当周出单三次或以上，60个Lead添加达标，否则70个Lead添加达标
        /// </summary>
        public int LeadAddStandard
        {
            get
            {
                return CRM_Logical._EmployeePerformance.GetSalesAddStandard(DealsCount);
            }
        }

        /// <summary>
        /// 包含出单时间和入账时间都在内的deals
        /// </summary>
        public IEnumerable<Deal> Deals { get; set; }
        public IEnumerable<LeadCall> Calls { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }


    //manager的考核
    public class _ManagerScore
    {
        [ReadOnly(true)]
        [ScaffoldColumn(false)]
        public int ID { get; set; }

        [ScaffoldColumn(false)]
        public int RoleLevel { get; set; }
        //被考核人
        //[ReadOnly(true)]
        [Display(Name = "员工")]
        //[HiddenInput(DisplayValue = false)] //Hide from Edit
        public string TargetName { get; set; }
        [ScaffoldColumn(false)]
        [Display(Name = "评分人")]
        //[HiddenInput(DisplayValue = false)] //Hide from Edit
        public string Assigner { get; set; }

        [ScaffoldColumn(false)]
        public string User { get; set; }

        [Display(Name = "责任心")]
        //[UIHint("ClientItem1"), Required]
        public int? Responsibility { get; set; }

        [Display(Name = "责任心")]
        //[UIHint("ClientItem1"), Required]
        public double ResponsibilityDisp
        {
            get
            {
                if (Responsibility == null)
                    Responsibility = 0;
                return (double)Responsibility / 100;
            }
        }
        
        [Display(Name = "纪律性")]
        public int? Discipline { get; set; }

        [Display(Name = "纪律性")]
        public double? DisciplineDisp
        {
            get
            {
                if (Discipline == null)
                    Discipline = 0;
                return (double)Discipline / 100;
            }
        }

        [Display(Name = "执行能力")]
        public int? Excution { get; set; }

        [Display(Name = "执行能力")]
        public double? ExcutionDisp
        {
            get
            {
                if (Excution == null)
                    Excution = 0;
                return (double)Excution / 100;
            }
        }

        [Display(Name = "目标意识")]
        public int? Targeting { get; set; }
        [Display(Name = "目标意识")]
        public double? TargetingDisp
        {
            get
            {
                if (Targeting == null)
                    Targeting = 0;
                return (double)Targeting / 100;
            }
        }


        [Display(Name = "检查工作状态")]
        public int? Searching { get; set; }
        [Display(Name = "检查工作状态")]
        public double? SearchingDisp
        {
            get
            {
                if (Searching == null)
                    Searching = 0;
                return (double)Searching / 100;
            }
        }


        [Display(Name = "每周项目协调")]
        public int? Production { get; set; }
        [Display(Name = "每周项目协调")]
        public double? ProductionDisp
        {
            get
            {
                if (Production == null)
                    Production = 0;
                return (double)Production / 100;
            }
        }

        [Display(Name = "每周PitchPaper")]
        public int? PitchPaper { get; set; }
        [Display(Name = "每周PitchPaper")]
        public double? PitchPaperDisp
        {
            get
            {
                if (PitchPaper == null)
                    PitchPaper = 0;
                return (double)PitchPaper / 100;
            }
        }

        [Display(Name = "每周例会")]
        public int? WeeklyMeeting { get; set; }
        [Display(Name = "每周例会")]
        public double? WeeklyMeetingDisp
        {
            get
            {
                if (WeeklyMeeting == null)
                    WeeklyMeeting = 0;
                return (double)WeeklyMeeting / 100;
            }
        }

        [Display(Name = "每月通话时间")]
        public int? MonthlyMeeting { get; set; }
        [Display(Name = "每月通话时间")]
        public double? MonthlyMeetingDisp
        {
            get
            {
                if (MonthlyMeeting == null)
                    MonthlyMeeting = 0;
                return (double)MonthlyMeeting / 100;
            }
        }

        [Display(Name = "团队CallList")]
        [HiddenInput(DisplayValue = false)] //Hide from Edit
        //团队Call List月人均平均数标准：销售专员200/人；销售经理140/人
        public double? Calllist//{ get; set; }
        {
            get
            {
                if (leadscount == 0 || salescount == 0)
                    return 0;
                else if ((leadcallcount/leadscount+salescallcount/salescount)>=(200+140))
                    return 0.15;
                else if ((leadcallcount/leadscount+salescallcount/salescount)>=(200+140 - 30))
                    return 0.10;
                else if ((leadcallcount/leadscount+salescallcount/salescount)>=(200+140 - 50))
                    return 0.05;
                else
                    return 0;
            }
        }
       
        /// <summary>
        /// 销售经理的call个数
        /// </summary>
        [ScaffoldColumn(false)]
        public int leadcallcount { get; set; }
        [ScaffoldColumn(false)]
        /// <summary>
        /// 销售专员的call个数
        /// </summary>
        public int salescallcount { get; set; }
        [ScaffoldColumn(false)]
        /// <summary>
        /// 考核人下销售经理人数
        /// </summary>
        public int leadscount { get; set; }
        [ScaffoldColumn(false)]
        /// <summary>
        /// 考核人下销售专员人数
        /// </summary>
        public int salescount { get; set; }
        [ScaffoldColumn(false)]
        /// <summary>
        /// 销售专员新增lead数量
        /// </summary>
        public int salesnewlead { get; set; }
        [ScaffoldColumn(false)]
        /// <summary>
        /// 销售经理新增lead数量
        /// </summary>
        public int leadnewlead { get; set; }

        [Display(Name = "团队新增Leads")]
        [HiddenInput(DisplayValue = false)] //Hide from Edit
        //团队新增Leads月人均平均数标准：销售专员420/人；销售经理280/人
        public double? AddLeads //{ get; set; }
        {
            get
            {
                if (leadscount == 0 || salescount == 0)
                    return 0;
                else if ((salesnewlead / leadscount + salesnewlead / salescount) >= (420 + 280))
                    return 0.15;
                else if ((salesnewlead / leadscount + salesnewlead / salescount) >= (420 + 280 - 50))
                    return 0.10;
                else if ((salesnewlead / leadscount + salesnewlead / salescount) >= (420 + 280 - 100))
                    return 0.05;
                else
                    return 0;
            }
            
        }
        [ScaffoldColumn(false)]
        /// <summary>
        /// 带领的团队完成当月到账目标
        /// </summary>
        public decimal? target { get; set; }
        [ScaffoldColumn(false)]
        /// <summary>
        /// 带领的团队完成当月到账实际
        /// </summary>
        public decimal? checkinreal { get; set; }

        [Display(Name = "团队业绩表现")]
        [HiddenInput(DisplayValue = false)] //Hide from Edit
        public double? CheckIn //{ get; set; }
        {
            get
            {
                if (target == 0 )
                    return 0;
                else if ((checkinreal / target) >= (decimal)1.4)
                    return 0.30;
                else if ((checkinreal / target) >= (decimal)1.2)
                    return 0.25;
                else if ((checkinreal / target) >= (decimal)1)
                    return 0.20;
                else if ((checkinreal / target) >= (decimal)0.8)
                    return 0.15;
                else if ((checkinreal / target) >= (decimal)0.6)
                    return 0.10;
                else
                    return 0;
            }
        }
        [Display(Name = "月")]
        [ScaffoldColumn(false)]
        public int? Month { get; set; }
        [ScaffoldColumn(false)]
        [Display(Name = "年")]
        public int? Year { get; set; }

        [Display(Name = "确认")]
        public string Confirmed { get; set; }

        [Display(Name = "打分")]
        public string Graded { get; set; }

        [Display(Name = "考核系数")]
        public double? Rate { get; set; }

        [Display(Name = "考核总分")]
        public int Score
        {
            get
            {
                if (Responsibility == null)
                    Responsibility = 0;
                if (Discipline == null)
                    Discipline = 0;
                if (Excution == null)
                    Excution = 0;
                if (Targeting == null)
                    Targeting = 0;
                if (Searching == null)
                    Searching = 0;
                if (Production == null)
                    Production = 0;
                if (PitchPaper == null)
                    PitchPaper = 0;
                if (WeeklyMeeting == null)
                    WeeklyMeeting = 0;
                if (MonthlyMeeting == null)
                    MonthlyMeeting = 0;
                if (Rate == null)
                    Rate = 1;
                return (int)(Rate * (Responsibility.Value + Discipline.Value + Excution.Value + Targeting.Value + Searching.Value + 
                    Production.Value + PitchPaper.Value + WeeklyMeeting.Value + MonthlyMeeting.Value + Calllist.Value*100 + AddLeads.Value*100 + CheckIn.Value*100));
            }
        }


    }
    
    public class _PreCommission
    {
        [Key]
        public int ID { get; set; }
        [Display(Name = "提成单号")]
        public string CommID { get; set; }
        [ScaffoldColumn(false)]
        public int RoleLevel { get; set; }
        
        [Display(Name = "中文名")]
        public string TargetNameCN { get; set; }

        [Display(Name = "英文名")]
        [Required]
        public string TargetNameEN { get; set; }
        [Display(Name = "国内外")]
        public string InOut { get; set; }
        [Display(Name = "项目")]
        [Required]
        [Remote("CheckUnique", "Finance", AdditionalFields = "StartDate,ID,TargetNameEN", ErrorMessage = "当月提成已经存在")]//ActionName,Controller,错误信息
        public int? ProjectID { get; set; }

        [Display(Name = "项目")]
        public string ProjectName { get; set; }

        [Display(Name = "开始时间")]
        public DateTime StartDate { get; set; }
        [Display(Name = "结束时间")]
        public DateTime EndDate { get; set; }


        [Display(Name = "小于3000入账金额")]
        [DisplayFormat(DataFormatString = "{0:#,###.00}", ApplyFormatInEditMode = true)]
        public decimal? DelegateLessIncome { get; set; }
        public string DelegateLessIncomeStr
        {
            get
            {
                if (DelegateLessIncome == null)
                    DelegateLessIncome = 0;
                return DelegateLessIncome.Value.ToString("#,##0.00");
            }
        }

        [Display(Name = "小于3000提成率")]
        public double? DelegateLessRate { get; set; }
        [Display(Name = "小于3000提成额")]
        public decimal? DelegateLessCommission { get; set; }


        [Display(Name = "大于3000参会人数")]
        public int? DelegateMoreCount { get; set; }
        [Display(Name = "大于3000入账金额")]
        [DisplayFormat(DataFormatString = "{0:#,###.00}", ApplyFormatInEditMode = true)]
        public decimal? DelegateMoreIncome { get; set; }
        public string DelegateMoreIncomeStr
        {
            get
            {
                if (DelegateMoreIncome == null)
                    DelegateMoreIncome = 0;
                return DelegateMoreIncome.Value.ToString("#,##0.00");
            }
        }

        [Display(Name = "大于3000提成率")]
        public double? DelegateMoreRate { get; set; }
        [Display(Name = "大于3000提成额")]
        public decimal? DelegateMoreCommission { get; set; }


        [Display(Name = "Sponsor入账金额")]
        [DisplayFormat(DataFormatString = "{0:#,###.00}")]
        public decimal? SponsorIncome { get; set; }
        public string SponsorIncomeStr
        {
            get
            {
                if (SponsorIncome == null)
                    SponsorIncome = 0;
                return SponsorIncome.Value.ToString("#,##0.00");
            }
        }

        [Display(Name = "Sponsor提成率")]
        public double? SponsorRate { get; set; }

        [Display(Name = "Sponsor提成额")]
        public decimal? SponsorCommission { get; set; }


        [Display(Name = "Delegate入账金额")]
        [DisplayFormat(DataFormatString = "{0:#,###.00}")]
        public decimal? DelegateIncome { get; set; }
        public string DelegateIncomeStr
        {
            get
            {
                if (DelegateIncome == null)
                    DelegateIncome = 0;
                return DelegateIncome.Value.ToString("#,##0.00");
            }
        }

        [Display(Name = "Delegate提成率")]
        public double? DelegateRate { get; set; }

        [Display(Name = "Delegate提成额")]
        public decimal? DelegateCommission { get; set; }



        [Display(Name = "入账总额")]
        [DisplayFormat(DataFormatString = "{0:#,###.00}", ApplyFormatInEditMode = true)]
        public decimal? Income { get; set; }
        public string IncomeStr
        {
            get
            {
                if (Income == null)
                    Income = 0;
                return Income.Value.ToString("#,##0.00");
            }
        }
        [Display(Name = "冲销金额")]
        public decimal? ReturnIncome { get; set; }

        [Display(Name = "冲销原因")]
        public string ReturnReason { get; set; }

        [Display(Name = "提成比率")]
        public double? CommissionRate { get; set; }

        [Display(Name = "提成总额")]
        [DisplayFormat(DataFormatString = "{0:#,###.00}", ApplyFormatInEditMode = true)]
        public decimal? Commission { get; set; }

        [Display(Name = "扣税")]
        public decimal? Tax { get; set; }

        [Display(Name = "扣奖金")]
        public decimal? Bonus { get; set; }

        [Display(Name = "冲销后总额")]
        public decimal? ActualCommission { get; set; }
        [Display(Name = "预发总额")]
        public decimal? TotalCommission { get; set; }
        
    }
    public class _CommissionProjects
    {
        public int? ID { get; set; }
        public string ProjectCode { get; set; }
    }

    public class _CommissionSales
    {
        public string salesid { get; set; }
        public string sales { get; set; }
    }

    public class _FinalCommission
    {
        [Key]
        public int ID { get; set; }
        [Display(Name = "提成单号")]
        public string CommID { get; set; }
        [ScaffoldColumn(false)]
        public int RoleLevel { get; set; }

        [Display(Name = "中文名")]
        public string TargetNameCN { get; set; }

        [Display(Name = "英文名")]
        [Required]
        [Remote("CheckUniqueForFinalComm", "Finance", AdditionalFields = "ProjectID,TargetNameEN,ID", ErrorMessage = "提成结算已经存在")]//ActionName,Controller,错误信息
        public string TargetNameEN { get; set; }
        [Display(Name = "国内外")]
        public string InOut { get; set; }
        [Display(Name = "项目")]
        [Required]
        public int? ProjectID { get; set; }

        [Display(Name = "项目")]
        public string ProjectName { get; set; }

        [Display(Name = "小于3000入账金额")]
        [DisplayFormat(DataFormatString = "{0:#,###.00}", ApplyFormatInEditMode = true)]
        public decimal? DelegateLessIncome { get; set; }
        public string DelegateLessIncomeStr
        {
            get
            {
                if (DelegateLessIncome == null)
                    DelegateLessIncome = 0;
                return DelegateLessIncome.Value.ToString("#,##0.00");
            }
        }

        [Display(Name = "小于3000提成率")]
        public double? DelegateLessRate { get; set; }
        [Display(Name = "小于3000提成额")]
        public decimal? DelegateLessCommission { get; set; }
        [Display(Name = "小于3000已发放额")]
        public decimal? DelegateLessPayed { get; set; }

        [Display(Name = "小于3000结算额")]
        public decimal? RealDelegateLessComm {
            get
            {
                if (DelegateLessCommission == null)
                    DelegateLessCommission = 0;
                if (DelegateLessPayed == null)
                    DelegateLessPayed = 0;
                return DelegateLessCommission - DelegateLessPayed;
            }
        }

        [Display(Name = "大于3000参会人数")]
        public int? DelegateMoreCount { get; set; }
        [Display(Name = "大于3000入账金额")]
        [DisplayFormat(DataFormatString = "{0:#,###.00}", ApplyFormatInEditMode = true)]
        public decimal? DelegateMoreIncome { get; set; }
        public string DelegateMoreIncomeStr
        {
            get
            {
                if (DelegateMoreIncome == null)
                    DelegateMoreIncome = 0;
                return DelegateMoreIncome.Value.ToString("#,##0.00");
            }
        }

        [Display(Name = "大于3000提成率")]
        public double? DelegateMoreRate { get; set; }
        [Display(Name = "大于3000提成额")]
        public decimal? DelegateMoreCommission { get; set; }
        [Display(Name = "大于3000已发放额")]
        public decimal? DelegateMorePayed { get; set; }
        [Display(Name = "大于3000结算额")]
        public decimal? RealDelegateMoreComm {
            get
            {
                if(DelegateMoreCommission==null)
                    DelegateMoreCommission=0;
                if(DelegateMorePayed==null)
                    DelegateMorePayed=0;
                return DelegateMoreCommission - DelegateMorePayed;
            }
        }

        [Display(Name = "Sponsor入账金额")]
        [DisplayFormat(DataFormatString = "{0:#,###.00}")]
        public decimal? SponsorIncome { get; set; }
        public string SponsorIncomeStr
        {
            get
            {
                if (SponsorIncome == null)
                    SponsorIncome = 0;
                return SponsorIncome.Value.ToString("#,##0.00");
            }
        }

        [Display(Name = "Sponsor提成率")]
        public double? SponsorRate { get; set; }

        [Display(Name = "Sponsor提成额")]
        public decimal? SponsorCommission { get; set; }
        [Display(Name = "Sponsor已发放额")]
        public decimal? SponsorPayed { get; set; }
        [Display(Name = "Sponsor结算额")]
        public decimal? RealSponsorComm {
            get
            {
                if (SponsorCommission == null)
                    SponsorCommission = 0;
                if (SponsorPayed == null)
                    SponsorPayed = 0;
                return SponsorCommission - SponsorPayed;
            }
        }



        [Display(Name = "Delegate入账金额")]
        [DisplayFormat(DataFormatString = "{0:#,###.00}")]
        public decimal? DelegateIncome { get; set; }
        public string DelegateIncomeStr
        {
            get
            {
                if (DelegateIncome == null)
                    DelegateIncome = 0;
                return DelegateIncome.Value.ToString("#,##0.00");
            }
        }

        [Display(Name = "Delegate提成率")]
        public double? DelegateRate { get; set; }

        [Display(Name = "Delegate提成额")]
        public decimal? DelegateCommission { get; set; }
        [Display(Name = "Delegate已发放额")]
        public decimal? DelegatePayed { get; set; }
        [Display(Name = "Delegate结算额")]
        public decimal? RealDelegateComm
        {
            get
            {
                if (DelegateCommission == null)
                    DelegateCommission = 0;
                if (DelegatePayed == null)
                    DelegatePayed = 0;
                return DelegateCommission - DelegatePayed;
            }
        }


        [Display(Name = "入账总额")]
        [DisplayFormat(DataFormatString = "{0:#,##0}", ApplyFormatInEditMode = true)]
        public decimal? Income { get; set; }
        public string IncomeStr
        {
            get
            {
                if (Income == null)
                    Income = 0;
                return Income.Value.ToString("#,##0");
            }
        }
        [Display(Name = "冲销金额")]
        public decimal? ReturnIncome { get; set; }

        [Display(Name = "冲销原因")]
        public string ReturnReason { get; set; }

        [Display(Name = "提成比率")]
        public double? CommissionRate { get; set; }

        [Display(Name = "提成总额")]
        public decimal? Commission { get; set; }

        [Display(Name = "提成总额")]
        public decimal? CommissionA { get; set; }

        [Display(Name = "已发总额")]
        public decimal? CommissionPayed { get; set; }

        [Display(Name = "结算总额")]
        public decimal? TotalCommission { get; set; }

        [Display(Name = "管理提成")]
        public decimal? ManageCommission { get; set; }

        [Display(Name = "其他项")]
        public decimal? OtherCommission { get; set; }

        [Display(Name = "说明项")]
        public decimal? ExplainCommission { get; set; }

        [Display(Name = "扣税")]
        public decimal? Tax { get; set; }

        [Display(Name = "扣奖金")]
        public decimal? Bonus { get; set; }
        
        [Display(Name = "实际结算额")]
        public decimal? ActualCommission { get; set; }
        

    }
    public class _CommissionDeals
    {
        public string DealCode { get; set; }
        public decimal? Income { get; set; }
        [Display(Name = "合约付款日期")]
        public DateTime ExpectedPaymentDate { get; set; }
        [Display(Name = "实际付款日期")]
        public DateTime? ActualPaymentDate { get; set; }
        [Display(Name = "签约日期")]
        public DateTime? SignDate { get; set; }
        
    }
    /// <summary>
    /// 用于考核人打分的下拉框
    /// </summary>
    public class _Item
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}