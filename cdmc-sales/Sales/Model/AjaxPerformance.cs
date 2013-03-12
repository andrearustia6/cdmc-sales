using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Entity;

namespace Model
{

    /// <summary>
    /// 一个月考勤周期的数据
    /// </summary>
    public class ProjectPerformaceData
    {
        public int Month { get; set; }
        public DateTime StartDate{get;set;}
        public DateTime EndDate{get;set;}
        public DateTime MonthStartDate{get;set;}
        public DateTime MonthEndDate{get;set;}
        public IEnumerable<TargetOfMonth> ProjectTargets { get; set; }
        public IEnumerable<TargetOfMonthForMember> MemberTargets { get; set; }
        public IEnumerable<Member> Members{get;set;}
        public IEnumerable<Deal> Deals{get;set;}
        public IEnumerable<LeadCall> LeadCalls{get;set;}
        public IEnumerable<Lead> Leads{get;set;}
        public IEnumerable<CompanyRelationship> CRMs { get; set; }
        public IEnumerable<Project> Projects { get; set; }
    }

    public class AjaxPerformance:AjaxStatistics
    {
        [Display(Name="被考核人")]
        public string Name { get; set; }
        [Display(Name = "职位")]
        public string Title { get; set; }
        public int PerformanceRate { get; set; }
        [Display(Name = "所在项目")]
        public string ProjectName { get; set; }
        public string TL { get; set; }
        public string Manager { get; set; }
        

    }

    public class AjaxMonthPerformance : AjaxPerformance
    {
        double _rate = 1;
        [Display(Name="考核系数")]
        public double? Rate 
        { set
        {
            if(value!=null)
            _rate = value.Value;
        }
            get{return _rate;}}
        public virtual string WeekLeadsAdd { get; set; }
        public virtual string WeekCallHours { get; set; }
        public virtual string WeekFaxOut { get; set; }

        public IEnumerable<string> Members { get; set; }
        public int MemberCount
        {
            get
            {
                if (Members == null) return 0;
                return Members.Count();
            }
        }

        [Display(Name = "月份")]
        public int? Month { get; set; }

           [Display(Name = "CheckIn分数")]
        public int CheckinScore
        {
            get
            {
                if (CheckInComplatePercetage >= 140) return  70;
                else if (CheckInComplatePercetage >= 120) return  60;
                else if (CheckInComplatePercetage >= 100) return  50;
                else if (CheckInComplatePercetage >= 80) return  40;
                else if (CheckInComplatePercetage >= 60) return  30;
                return 0;
            }
        }
        [Display(Name = "调研Lead分数")]
        public int AddLeadScore
        {
            get
            {
                if (LeadNotQualifiedWeeksCount == 0) return 20;
                else if (LeadNotQualifiedWeeksCount == 1) return 15;
                else if (LeadNotQualifiedWeeksCount == 2) return 10;
                else if (LeadNotQualifiedWeeksCount == 3) return 5;
                return 0;
            }
        }
        [Display(Name = "通话时间/FaxOut分数")]
        public int FaxCallScore { get {
            if (HoursOrFaxNotQualifiedWeeksCount == 0) return 20;
            else if (HoursOrFaxNotQualifiedWeeksCount == 1) return 15;
            else if (HoursOrFaxNotQualifiedWeeksCount == 2) return 10;
            else if (HoursOrFaxNotQualifiedWeeksCount == 3) return 5;
            return 0;
        } }
        [Display(Name="考核总分数")]
        public  int Score
        {
            get
            {
                if (AssignedScore == null) AssignedScore = 0;
                return (int)(Rate* (CheckinScore + AddLeadScore + FaxCallScore + AssignedScore.Value));
            }
        }

        [Display(Name = "主观评分")]
        public double? AssignedScore { get; set; }

        [Display(Name = "调研不达标周数")]
        public int LeadNotQualifiedWeeksCount
        {
            get
            {
                if (Weeks == null) return 4;
                return Weeks.FindAll(f => f.IsLeadAddedQualified == false).Count;
            }
        }

   

        [Display(Name = "通话|FaxOut不达标周数")]
        public int HoursOrFaxNotQualifiedWeeksCount
        {
            get
            {
                if (Weeks == null) return 4;
                return Weeks.FindAll(f => f.IsFaxOutOrCallHoursQualified == false).Count;
            }
        }

  

        public override DateTime StartDate
        {
            get
            {
                DateTime startdate, enddate;
                Utl.Utl.GetMonthActualStartdateAndEnddate(Month, out startdate, out enddate);

                return startdate;
            }
           
        }

        public override DateTime EndDate
        {
            get
            {
                DateTime startdate, enddate;
                Utl.Utl.GetMonthActualStartdateAndEnddate(Month, out startdate, out enddate);

                return enddate;
            }
        }

        protected List<AjaxWeekPerformance> _weeks;
        public virtual List<AjaxWeekPerformance> Weeks{get;set;}
    }


    public class AjaxWeekPerformance : AjaxPerformance
    {
        protected int? _faxOutStandard { get; set; }
        protected int? _leadsStandard { get; set; }
        protected int? _callHoursStandard { get; set; }
        public virtual int FaxOutStandard { get; set; }
        public virtual int LeadsStandard { get; set; }
        public virtual int CallHoursStandard { get; set; }

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
                return LeadsCount >= LeadsStandard;
            }
        }
    }



    public class AjaxLeadWeekPerformance : AjaxWeekPerformance
    {


        public override int LeadsStandard
        {
            get
            {
                if (_leadsStandard == null)
                {
                    if (_deals != null && _deals.Count() > 0)
                        _leadsStandard =  60;
                    else
                        _leadsStandard =  70;
                }
                return _leadsStandard.Value;
            }
        }
        public override int FaxOutStandard
        {
            get
            {
                if (_faxOutStandard == null)
                {
                    if (_deals != null && _deals.Where(d=>d.SignDate>=StartDate && d.SignDate< EndDate).Count() > 0)
                        _faxOutStandard = 28;
                    else
                        _faxOutStandard = 35;
                }
                return _faxOutStandard.Value;
            }
        }
        public override double CallHours
        {
            get
            {
                if (_deals!=null && _deals.Count() > 0)
                    return 6;
                else
                    return 7.5;
            }
        }
    }
   
    public class AjaxLeadMonthPerformance : AjaxMonthPerformance
    {
        
        public override string WeekCallHours
        {
            get
            {
                var t = string.Empty;
                Weeks.ForEach(f=>{
                    if(string.IsNullOrEmpty(t))
                        t = f.CallHours.ToString();
                    else
                        t = t + " | " + f.CallHours.ToString();
                });

                    return t;
            }
            
        }

        public override string WeekFaxOut
        {
            get
            {
               var t = string.Empty;
                Weeks.ForEach(f=>{
                    if(string.IsNullOrEmpty(t))
                        t = f.FaxOutCount.ToString();
                    else
                         t = t+" | "+ f.FaxOutCount.ToString();
                });

                    return t;
            }
           
        }

        public override string WeekLeadsAdd
        {
            get
            {
                var t = string.Empty;
                Weeks.ForEach(f =>
                {
                    if (string.IsNullOrEmpty(t))
                        t = f.LeadsCount.ToString();
                    else
                        t = t + " | " + f.LeadsCount.ToString();
                });

                return t;
            }
           
        }

      
        public  override List<AjaxWeekPerformance> Weeks { get {
            if(_weeks==null)
            {
                _weeks = new List<AjaxWeekPerformance>();
                DateTime startdate, enddate;
                Utl.Utl.GetMonthActualStartdateAndEnddate(Month,out startdate,out enddate);
                StartDate = startdate; EndDate = enddate;
                while (startdate < EndDate)
                {
                    enddate = startdate.AddDays(7);
                    var ap = new AjaxLeadWeekPerformance() {
                        StartDate = startdate, EndDate = enddate,
                        LeadCalls = _leadCalls.Where(f => f.CallDate >= startdate && f.CallDate < enddate).ToList(),
                        Leads = _leads.Where(f => f.CreatedDate >= startdate && f.CreatedDate < enddate).ToList(),
                        Deals = _deals.Where(f => f.ActualPaymentDate >= StartDate && f.ActualPaymentDate < EndDate).ToList(),
                    };
                    _weeks.Add(ap);
                    startdate = enddate;
                }
            }
                return _weeks;
        } }
     
    }


    public class AjaxSalesMonthPerformance : AjaxMonthPerformance
    {
        public override string WeekCallHours
        {
            get
            {
                var t = string.Empty;
                Weeks.ForEach(f =>
                {
                    if (string.IsNullOrEmpty(t))
                        t = f.CallHours.ToString();
                    else
                        t = t + " | " + f.CallHours.ToString();
                });

                return t;
            }

        }

        public override string WeekFaxOut
        {
            get
            {
                var t = string.Empty;
                Weeks.ForEach(f =>
                {
                    if (string.IsNullOrEmpty(t))
                        t = f.FaxOutCount.ToString();
                    else
                        t = t + " | " + f.FaxOutCount.ToString();
                });

                return t;
            }

        }

        public override string WeekLeadsAdd
        {
            get
            {

                var t = string.Empty;
                Weeks.ForEach(f =>
                {
                    if (string.IsNullOrEmpty(t))
                        t = f.LeadsCount.ToString();
                    else
                        t = t + " | " + f.LeadsCount.ToString();
                });

                return t;
            }

        }


        public override List<AjaxWeekPerformance> Weeks
        {
            get
            {
                if (_weeks == null)
                {
                    _weeks = new List<AjaxWeekPerformance>();
                    DateTime startdate, enddate;
                    Utl.Utl.GetMonthActualStartdateAndEnddate(Month, out startdate, out enddate);
                    StartDate = startdate; EndDate = enddate;
                    while (startdate < EndDate)
                    {
                        enddate = startdate.AddDays(7);
                        var ap = new AjaxSalesWeekPerformance()
                        {
                            StartDate = startdate,
                            EndDate = enddate,
                            LeadCalls = _leadCalls.Where(f => f.CallDate >= startdate && f.CallDate < enddate),
                            Leads = _leads.Where(f => f.CreatedDate >= startdate && f.CreatedDate < enddate),
                            Deals = _deals.Where(f => f.ActualPaymentDate >= StartDate && f.ActualPaymentDate < EndDate)
                        };
                        _weeks.Add(ap);
                        startdate = enddate;
                    }
                }
                return _weeks;
            }
        }

    }

    public class AjaxSalesWeekPerformance : AjaxWeekPerformance
    {
        public override int LeadsStandard
        {
            get
            {

                if (_leadsStandard == null)
                {
                    if (_deals != null && _deals.Count() > 0)
                        _leadsStandard = 80;
                    else
                        _leadsStandard = 105;
                }
                return _leadsStandard.Value;



            }
        }
        public override int FaxOutStandard { 
            get {
                if (_faxOutStandard == null)
                {
                    if (_deals != null && _deals.Where(d => d.SignDate >= StartDate && d.SignDate < EndDate).Count() > 0)
                        _faxOutStandard = 40;
                    else
                        _faxOutStandard = 50;
                }
                return _faxOutStandard.Value;
            }
        }
        public override double CallHours
        {
            get
            {
                if (_deals != null && _deals.Count() > 0)
                    return 8;
                else
                    return 10;
            }
        }
    }

    public class AjaxManagerMonthPerformance : AjaxMonthPerformance
    {
        [Display(Name = "月人均通话")]
        public double AverageHours { get {
            if (MemberCount == 0) return 0;
            return CallHours / Members.Count(); } }
        [Display(Name = "月人均faxout")]
        public int AverageFax
        {
            get
            {
                if (MemberCount == 0) return 0;
                return FaxOutCount / MemberCount;
            }
        }

        [Display(Name = "月人均添加Lead")]
        public int AverageLead
        {
            get
            {
                if (MemberCount == 0) return 0;
                return LeadsCount / MemberCount;
            }
        }
       
    }

 
}