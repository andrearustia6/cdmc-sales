using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Entity;

namespace Model
{
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
        public virtual string WeekLeadsAdd { get; set; }
        public virtual string WeekCallHours { get; set; }
        public virtual string WeekFaxOut { get; set; }

        public List<string> Members { get; set; }
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

        [Display(Name = "主观评分")]
        public int? AssignedScore { get; set; }

        [Display(Name = "添加Lead不达标周数")]
        public int LeadNotQualifiedWeeksCount
        {
            get
            {
                if (Weeks == null) return 4;
                return Weeks.FindAll(f => f.IsLeadAddedQualified == false).Count;
            }
        }

   

        [Display(Name = "通话时间或者FaxOut不达标周数")]
        public int HoursOrFaxNotQualifiedWeeksCount
        {
            get
            {
                if (Weeks == null) return 4;
                return Weeks.FindAll(f => f.IsFaxOutOrCallHoursQualified == false).Count;
            }
        }

        public virtual int Score { get; set; }

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
        public virtual int FaxOutStandard { get; set; }
        public virtual int LeadsStandard { get; set; }
        public virtual int CallHoursStandard { get; set; }

        public bool IsFaxOutOrCallHoursQualified
        {
            get
            {
                return FaxOutCount > FaxOutStandard;

            }
        }

        public bool IsLeadAddedQualified { get { return LeadsCount > LeadsStandard; } }
    }



    public class AjaxLeadWeekPerformance : AjaxWeekPerformance
    {
        public override int LeadsStandard { get  { return 70;  } }
        public override int FaxOutStandard { get { return 35; } }
        public override double CallHours { get { return 7.5; } }
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
                        LeadCalls = _leadCalls.FindAll(f => f.CallDate >= startdate && f.CallDate < enddate).ToList(),
                        Leads = _leads.Where(f => f.CreatedDate >= startdate && f.CreatedDate < enddate).ToList()
                    };
                    _weeks.Add(ap);
                    startdate = enddate;
                }

                
            }
                return _weeks;
        } }

        [Display(Name="考核分数")]
        public override int Score
        {
            get
            {
                int checkinscore=0;
                int addleadscore = 0;
                int faxcallscore=0;
                if (CheckInComplatePercetage >= 140) checkinscore = 70;
                else if (CheckInComplatePercetage >= 120) checkinscore = 60;
                else if (CheckInComplatePercetage >= 100) checkinscore = 50;
                else if (CheckInComplatePercetage >= 80) checkinscore = 40;
                else if  (CheckInComplatePercetage >= 60) checkinscore = 30;


                if (LeadNotQualifiedWeeksCount == 0) addleadscore = 20;
                else if(LeadNotQualifiedWeeksCount == 1) addleadscore = 15;
                else if (LeadNotQualifiedWeeksCount == 2) addleadscore = 10;
                else if (LeadNotQualifiedWeeksCount == 3) addleadscore = 5;

                if ( HoursOrFaxNotQualifiedWeeksCount  == 0) faxcallscore = 20;
                else if (HoursOrFaxNotQualifiedWeeksCount == 1) faxcallscore = 15;
                else if (HoursOrFaxNotQualifiedWeeksCount == 2) faxcallscore = 10;
                else if (HoursOrFaxNotQualifiedWeeksCount == 3) faxcallscore = 5;
                if (AssignedScore == null) AssignedScore = 0;
                return checkinscore + addleadscore + faxcallscore+ AssignedScore.Value;
            }
        }
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
                        var ap = new AjaxSalesWeekPerformance() {
                            StartDate = startdate,
                            EndDate = enddate,
                            LeadCalls = _leadCalls.FindAll(f => f.CallDate >= startdate && f.CallDate < enddate),
                            Leads = _leads.Where(f => f.CreatedDate >= startdate && f.CreatedDate < enddate).ToList()
                        };
                        _weeks.Add(ap);
                        startdate = enddate;
                    }
                }
                return _weeks;
            }
        }

        public override int Score
        {
            get
            {
                int checkinscore = 0;
                int addleadscore = 0;
                int faxcallscore = 0;
                if (CheckInComplatePercetage >= 140) checkinscore = 70;
                else if (CheckInComplatePercetage >= 120) checkinscore = 60;
                else if (CheckInComplatePercetage >= 100) checkinscore = 50;
                else if (CheckInComplatePercetage >= 80) checkinscore = 40;
                else if (CheckInComplatePercetage >= 60) checkinscore = 30;


                if (LeadNotQualifiedWeeksCount == 0) addleadscore = 20;
                else if (LeadNotQualifiedWeeksCount == 1) addleadscore = 15;
                else if (LeadNotQualifiedWeeksCount == 2) addleadscore = 10;
                else if (LeadNotQualifiedWeeksCount == 3) addleadscore = 5;

                if (HoursOrFaxNotQualifiedWeeksCount == 0) faxcallscore = 20;
                else if (HoursOrFaxNotQualifiedWeeksCount == 1) faxcallscore = 15;
                else if (HoursOrFaxNotQualifiedWeeksCount == 2) faxcallscore = 10;
                else if (HoursOrFaxNotQualifiedWeeksCount == 3) faxcallscore = 5;
                if (AssignedScore == null) AssignedScore = 0;
                return checkinscore + addleadscore + faxcallscore + AssignedScore.Value;
            }
        }
    }

    public class AjaxSalesWeekPerformance : AjaxWeekPerformance
    {
        public override int LeadsStandard { get { return 105; } }
        public override int FaxOutStandard { get { return 50; } }
        public override double CallHours { get { return 10; } }
    }

    public class AjaxManagerMonthPerformance : AjaxMonthPerformance
    {
        [Display(Name = "平均通话时间")]
        public double AverageHours { get {
            if (MemberCount == 0) return 0;
            return CallHours / Members.Count(); } }
        [Display(Name = "平均FaxOut数")]
        public int AverageFax
        {
            get
            {
                if (MemberCount == 0) return 0;
                return FaxOutCount / MemberCount;
            }
        }

        [Display(Name = "平均添加Lead数")]
        public int AverageLead
        {
            get
            {
                if (MemberCount == 0) return 0;
                return LeadsCount / MemberCount;
            }
        }
        //public override List<AjaxWeekPerformance> Weeks
        //{
        //    get
        //    {
        //        if (_weeks == null)
        //        {
        //           DateTime startdate,enddate;
        //            Utl.Utl.GetMonthActualStartdateAndEnddate(Month,out startdate,out enddate);

        //            StartDate = startdate; EndDate = enddate;
        //            while(startdate<EndDate)
        //            {
        //                enddate = startdate.AddDays(7);
        //                var ap = new AjaxManagerWeekPerformance() { StartDate = startdate, EndDate = enddate, LeadCalls = _leadCalls };
        //                _weeks.Add(ap);
        //                startdate = enddate;
        //            }
        //        }
        //        return _weeks;
        //    }
        //}
       
       
      

       
    }

 
}