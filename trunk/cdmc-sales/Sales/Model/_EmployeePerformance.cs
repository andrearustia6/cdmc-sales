using System;
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
                    return TeamLeadPerformanceInWeeks.Count(c => c.IsLeadAddedQualified == false);
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

                return CRM_Logical._EmployeePerformance.GetFaxOutScore(LeadNotQualifiedWeeksCount);
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

        public IEnumerable<_TeamLeadPerformanceInWeek> TeamLeadPerformanceInWeeks { private get; set; }

        [Display(Name = "Faxout详细")]
        public string FaxOutCountString
        {
            get
            {
                if (TeamLeadPerformanceInWeeks != null)
                {
                    var counts = TeamLeadPerformanceInWeeks.Select(s => s.FaxOutCount);
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
                    var counts = TeamLeadPerformanceInWeeks.Select(s => s.LeadsCount);
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


        public bool IsLeadAddedQualified
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
                    var counts = SalesPerformanceInWeeks.Select(s => s.FaxOutCount);
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
                    var counts = SalesPerformanceInWeeks.Select(s => s.LeadsCount);
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


        public bool IsLeadAddedQualified
        {
            get
            {
                return LeadsCount >= LeadAddStandard;
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
                else if (checkinreal / target >= (decimal)1.4)
                    return 0.30;
                else if (checkinreal / target >= (decimal)1.2)
                    return 0.25;
                else if (checkinreal / target >= (decimal)1)
                    return 0.20;
                else if (checkinreal / target >= (decimal)0.8)
                    return 0.15;
                else if (checkinreal / target >= (decimal)0.6)
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

        [Display(Name = "是否确认")]
        public string Confirmed { get; set; }

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
                    Production.Value + PitchPaper.Value + WeeklyMeeting.Value + MonthlyMeeting.Value + Calllist.Value + AddLeads.Value + CheckIn.Value));
            }
        }
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