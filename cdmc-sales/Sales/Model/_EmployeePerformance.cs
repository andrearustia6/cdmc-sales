using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entity;
using System.ComponentModel.DataAnnotations;
using BLL;

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

        [Display(Name = "调研不达标周数")]
        public int LeadNotQualifiedWeeksCount
        {
            get
            {
               return TeamLeadPerformanceInWeeks.Count(c=>c.IsLeadAddedQualified==false);
            }
        }

        [Display(Name = "通话|FaxOut不达标周数")]
        public int HoursOrFaxNotQualifiedWeeksCount
        {
            get
            {
                return TeamLeadPerformanceInWeeks.Count(c => c.IsFaxOutOrCallHoursQualified == false);
            }
        }


        [Display(Name = "考核系数")]
        public double? Rate
        {
            get; set;
        }

        [Display(Name = "CheckIn分数")]
        public int CheckinScore
        {
            get
            {
                return CRM_Logical._EmployeePerformance.GetCheckInScore(CompletePercent);
              
            }
        }

        [Display(Name = "调研Lead分数")]
        public int AddLeadScore
        {
            get
            {
                return CRM_Logical._EmployeePerformance.GetLeadAddScore(LeadNotQualifiedWeeksCount);
            }
        }

        [Display(Name = "通话时间/FaxOut分数")]
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
        public string Name { get; set; }
        public decimal? Target { get; set; }
        public decimal? CheckIn { get; set; }
        public double? CompletePercent { get {
            if (Target == null || Target == 0 || CheckIn==null||CheckIn==0) return 0;

            return Utl.Utl.GetPercent((double)CheckIn, (double)Target);
        }
        }
        public IEnumerable<_TeamLeadPerformanceInWeek> TeamLeadPerformanceInWeeks { private get; set; }

        public string FaxOutCountString
        {
            get
            {
                var counts = TeamLeadPerformanceInWeeks.Select(s => s.FaxOutCount);
                return string.Join(",", counts);
            }
        }

        public string LeadAddCountString
        {
            get
            {
                var counts = TeamLeadPerformanceInWeeks.Select(s => s.LeadsCount);
                return string.Join(",", counts);
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

        [Display(Name = "调研不达标周数")]
        public int LeadNotQualifiedWeeksCount
        {
            get
            {
                return TeamLeadPerformanceInWeeks.Count(c => c.IsLeadAddedQualified == false);
            }
        }

        [Display(Name = "通话|FaxOut不达标周数")]
        public int HoursOrFaxNotQualifiedWeeksCount
        {
            get
            {
                return TeamLeadPerformanceInWeeks.Count(c => c.IsFaxOutOrCallHoursQualified == false);
            }
        }


        [Display(Name = "考核系数")]
        public double? Rate
        {
            get;
            set;
        }

        [Display(Name = "CheckIn分数")]
        public int CheckinScore
        {
            get
            {
                return CRM_Logical._EmployeePerformance.GetCheckInScore(CompletePercent);

            }
        }

        [Display(Name = "调研Lead分数")]
        public int AddLeadScore
        {
            get
            {
                return CRM_Logical._EmployeePerformance.GetLeadAddScore(LeadNotQualifiedWeeksCount);
            }
        }

        [Display(Name = "通话时间/FaxOut分数")]
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
        public string Name { get; set; }
        public decimal? Target { get; set; }
        public decimal? CheckIn { get; set; }
        public double? CompletePercent
        {
            get
            {
                if (Target == null || Target == 0 || CheckIn == null || CheckIn == 0) return 0;

                return Utl.Utl.GetPercent((double)CheckIn, (double)Target);
            }
        }
        public IEnumerable<_TeamLeadPerformanceInWeek> TeamLeadPerformanceInWeeks { private get; set; }

        public string FaxOutCountString
        {
            get
            {
                var counts = TeamLeadPerformanceInWeeks.Select(s => s.FaxOutCount);
                return string.Join(",", counts);
            }
        }

        public string LeadAddCountString
        {
            get
            {
                var counts = TeamLeadPerformanceInWeeks.Select(s => s.LeadsCount);
                return string.Join(",", counts);
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

}