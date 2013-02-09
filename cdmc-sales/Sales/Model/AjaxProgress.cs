using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Entity;
using Utl;


namespace Model
{
    //时间段的ajax view
    public class AjaxProgressStatistics
    {
        public virtual IQueryable<LeadCall> LeadCalls { set { _leadCalls = value; } }
         IQueryable<LeadCall> _leadCalls { get; set; }
        [Display(Name = "填写LeadCall数量")]
        public int LeadCallsCount
        {
            get
            {
                if (_leadCalls == null) return 0;
                return _leadCalls.Where(l => l.CallDate < EndDate && l.CallDate >= StartDate).Count();
                }
        }
        [Display(Name = "打过Lead数量")]
        public int CallingLeadCount
        {
            get
            {
                if (_leadCalls == null) return 0;
                var c = _leadCalls.Where(l => l.CallDate < EndDate && l.CallDate >= StartDate).ToList().Distinct(new LeadCallLeadDistinct()).Count();
                return c;
            }
        }

        [Display(Name = "打过公司数量")]
        public int CallingCompanyCount
        {
            get
            {
                if (_leadCalls == null) return 0;
                var c = _leadCalls.Where(l => l.CallDate < EndDate && l.CallDate >= StartDate).ToList().Distinct(new LeadCallCompanyDistinct()).Count();
                return c;
            }
        }
         IQueryable<Deal> _deals { get; set; }
         public virtual IQueryable<Deal> Deals { set { _deals = value; } }
        public IQueryable<CompanyRelationship> CompanyRelationships { get; set; }

        public decimal? TotalDealinTargets { get; set; }

        public decimal? TotalCheckinTargets { get; set; }
        public decimal? TotalCheckIn
        {
            get
            {
                if (_deals == null) return 0;
                return _deals.Where(d => d.Abandoned == false && d.Project.IsActived == true && d.ActualPaymentDate < EndDate && d.ActualPaymentDate >= StartDate).Sum(s => (decimal?)s.Income);
            }
        }
        public decimal? TotalDealIn
        {
            get
            {
                if (_deals == null) return 0;
                return _deals.Where(d => d.Abandoned == false && d.Project.IsActived == true && d.SignDate < EndDate && d.SignDate >= StartDate).Sum(s => (decimal?)s.Payment);
            }
        }

        public int TotalDealsCount
        {
            get
            {
                if (_deals == null) return 0;
                return _deals.Where(d => d.Abandoned == false && d.Project.IsActived == true && d.SignDate < EndDate && d.SignDate >= StartDate).Count();
            }
        }
        public decimal? TotalCompanysCount
        {
            get
            {
            if (CompanyRelationships == null) return 0;
            return CompanyRelationships.Count();
        } }

        [Display(Name = "DealIn完成度")]
        public double DealInComplatePercetage
        {
            get
            {
                if (TotalDealinTargets == 0 || TotalDealIn == null)
                    return 0;
                else
                {
                    var p = (double)(TotalDealIn * 100 / TotalDealinTargets);
                    var v = Math.Round(p, 2);
                    return v;
                }

            }
        }

        [Display(Name = "CheckIn完成度")]
        public double CheckInComplatePercetage
        {
            get
            {
                if (TotalCheckinTargets == 0 || TotalCheckIn == null)
                    return 0;
                else
                {
                    var p = (double)(TotalCheckIn * 100 / TotalCheckinTargets);
                    var v = Math.Round(p, 2);
                    return v;
                }

            }
        }

  
        [Display(Name = "开始时间")]
        public string StartDayString { get { return StartDate.ToShortDateString(); } }
        [Display(Name = "结束时间")]
        public string EndDayString { get { return EndDate.ToShortDateString(); } }
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime EndDate { get; set; }

    }

    public class AjaxMonthTotalProgressStatistics : AjaxProgressStatistics
    {
        [Display(Name = "月份")]
        public int Month { get; set; }
        public int Year { get; set; }

        public override DateTime StartDate
        {
            get
            {
                return new DateTime(Year,Month,1);
            }
        }

        public override DateTime EndDate
        {
            get
            {
                return StartDate.EndOfMonth();
            }
        }
        
        IQueryable<Project> _projects;
        public IQueryable<Project> Projects { set { _projects = value; } }

         [Display(Name = "进行中项目数量")]
        public int ProjectCounts {
            get{
                if (_projects == null) return 0;
                return _projects.Count();
            }}

        IEnumerable<Member> _members;
        [Display(Name = "项目中销售数量")]
        public int MemberCounts
        {
            get
            {
                if (_projects == null) return 0;

                if(_members==null)
                    _members = _projects.SelectMany(s => s.Members).ToList().Distinct(new MemberDistinct());

                return _members.Count();
            }
        }


         [Display(Name = "月个人平均DealIn")]
        public decimal PerMemberDealIn
        {
            get
            {
                if (MemberCounts == 0|| TotalDealIn==null) return 0;
                return   Math.Round(TotalDealIn.Value/MemberCounts,1);
            }
        }
        [Display(Name = "月个人平均Call数量")]
        public double PerMemberCall
        {
            get
            {
                if (MemberCounts == 0) return 0;
                return  Math.Round((double)(LeadCallsCount/MemberCounts),1);
            }
        }
        [Display(Name = "月个人平均Checkin")]
        public decimal PerMemberCheckIn
        {
            get
            {
                if (MemberCounts == 0 || TotalCheckIn==null) return 0;
                return Math.Round(TotalCheckIn.Value / MemberCounts,1);
            }
        }

    }

    public class AjaxMonthProjectProgressStatistics : AjaxProgressStatistics
    {
        [Display(Name = "月份")]
        public int Month { get; set; }
        public int Year { get; set; }

        public override IQueryable<Deal> Deals
        {
            set
            {
                base.Deals = value.Where(d=>d.ProjectID ==_project.ID);
            }
        }

        public override IQueryable<LeadCall> LeadCalls
        {
            set
            {
                base.LeadCalls = value.Where(d => d.ProjectID == _project.ID);
            }
        }

        public override DateTime StartDate
        {
            get
            {
                return new DateTime(Year, Month, 1);
            }
        }

        public override DateTime EndDate
        {
            get
            {
                return StartDate.EndOfMonth();
            }
        }
        Project _project;
        public Project Project { set { _project = value; } }

        [Display(Name = "月份")]
        public string ProjectName { get { return _project.Name_CH; } }
        public string ProjectCode { get { return _project.ProjectCode; } }

        IEnumerable<Member> _members;
        [Display(Name = "项目中销售数量")]
        public int MemberCounts
        {
            get
            {
                if (_project == null || _project.Members==null) return 0;

                if (_members == null)
                    _members = _project.Members.ToList();

                return _members.Count();
            }
        }


        [Display(Name = "月个人平均DealIn")]
        public decimal PerMemberDealIn
        {
            get
            {
                if (MemberCounts == 0 || TotalDealIn == null) return 0;
                return Math.Round(TotalDealIn.Value / MemberCounts, 1);
            }
        }
        [Display(Name = "月个人平均Call数量")]
        public double PerMemberCall
        {
            get
            {
                if (MemberCounts == 0) return 0;
                return Math.Round((double)(LeadCallsCount / MemberCounts), 1);
            }
        }
        [Display(Name = "月个人平均Checkin")]
        public decimal PerMemberCheckIn
        {
            get
            {
                if (MemberCounts == 0 || TotalCheckIn == null) return 0;
                return Math.Round(TotalCheckIn.Value / MemberCounts, 1);
            }
        }

    }

     public class  AjaxWeekProjectProgressStatistics : AjaxProgressStatistics
     {
         [Display(Name = "月份")]
         public int Month { get; set; }
         public int Year { get; set; }

         public override IQueryable<Deal> Deals
         {
             set
             {
                 base.Deals = value.Where(d => d.ProjectID == _project.ID);
             }
         }

         public override IQueryable<LeadCall> LeadCalls
         {
             set
             {
                 base.LeadCalls = value.Where(d => d.ProjectID == _project.ID);
             }
         }

         public override DateTime StartDate { get; set; }

         public override DateTime EndDate { get; set; }

         Project _project;
         public Project Project { set { _project = value; } }

         [Display(Name = "项目名称")]
         public string ProjectName { get { return _project.Name_CH; } }
         [Display(Name = "项目代码")]
         public string ProjectCode { get { return _project.ProjectCode; } }

         IEnumerable<Member> _members;
         [Display(Name = "项目中销售数量")]
         public int MemberCounts
         {
             get
             {
                 if (_project == null || _project.Members == null) return 0;

                 if (_members == null)
                     _members = _project.Members.ToList();

                 return _members.Count();
             }
         }


         [Display(Name = "周个人平均DealIn")]
         public decimal PerMemberDealIn
         {
             get
             {
                 if (MemberCounts == 0 || TotalDealIn == null) return 0;
                 return Math.Round(TotalDealIn.Value / MemberCounts, 1);
             }
         }
         [Display(Name = "周个人平均Call数量")]
         public double PerMemberCall
         {
             get
             {
                 if (MemberCounts == 0) return 0;
                 return Math.Round((double)(LeadCallsCount / MemberCounts), 1);
             }
         }
         [Display(Name = "周个人平均Checkin")]
         public decimal PerMemberCheckIn
         {
             get
             {
                 if (MemberCounts == 0 || TotalCheckIn == null) return 0;
                 return Math.Round(TotalCheckIn.Value / MemberCounts, 1);
             }
         }
     }
    public class AjaxWeekTotalProgressStatistics : AjaxProgressStatistics
    {
        [Display(Name = "月份")]
        public int Month { get; set; }
        public int Year { get; set; }

        public override DateTime StartDate{ get; set; }

        public override DateTime EndDate { get; set; }

        IQueryable<Project> _projects;
        public IQueryable<Project> Projects { set { _projects = value; } }

        [Display(Name = "进行中项目数量")]
        public int ProjectCounts
        {
            get
            {
                if (_projects == null) return 0;
                return _projects.Count();
            }
        }

        IEnumerable<Member> _members;
        [Display(Name = "项目中销售数量")]
        public int MemberCounts
        {
            get
            {
                if (_projects == null) return 0;

                if (_members == null)
                    _members = _projects.SelectMany(s => s.Members).ToList().Distinct(new MemberDistinct());

                return _members.Count();
            }
        }


        [Display(Name = "周个人平均DealIn")]
        public decimal PerMemberDealIn
        {
            get
            {
                if (MemberCounts == 0 || TotalDealIn == null) return 0;
                return Math.Round(TotalDealIn.Value / MemberCounts, 1);
            }
        }
        [Display(Name = "周个人平均Call数量")]
        public double PerMemberCall
        {
            get
            {
                if (MemberCounts == 0) return 0;
                return Math.Round((double)(LeadCallsCount / MemberCounts), 1);
            }
        }
        [Display(Name = "周个人平均Checkin")]
        public decimal PerMemberCheckIn
        {
            get
            {
                if (MemberCounts == 0 || TotalCheckIn == null) return 0;
                return Math.Round(TotalCheckIn.Value / MemberCounts, 1);
            }
        }

    }

}