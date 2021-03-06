﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Entity;
using Utl;


namespace Model
{
    public class AjaxProgress : AjaxStatistics
    {
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
    }

    public class AjaxMonthTotalProgressStatistics : AjaxProgress
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


         [Display(Name = "人均DealIn")]
        public decimal PerMemberDealIn
        {
            get
            {
                if (MemberCounts == 0|| TotalDealIn==null) return 0;
                return   Math.Round(TotalDealIn.Value/MemberCounts,1);
            }
        }
        [Display(Name = "人均Faxout数量")]
        public double PerFaxout
        {
            get
            {
                if (MemberCounts == 0) return 0;
                return  Math.Round((double)(FaxOutCount/MemberCounts),1);
            }
        }
        [Display(Name = "人均Checkin")]
        public decimal PerMemberCheckIn
        {
            get
            {
                if (MemberCounts == 0 || TotalCheckIn==null) return 0;
                return Math.Round(TotalCheckIn.Value / MemberCounts,1);
            }
        }

    }

    public class AjaxMonthProjectProgressStatistics : AjaxProgress
    {
        [Display(Name = "月份")]
        public int Month { get; set; }
        public int Year { get; set; }

        public override IEnumerable<Deal> Deals
        {
            set
            {
                base.Deals = value.Where(d=>d.ProjectID ==_project.ID);
            }
        }

        public override IEnumerable<LeadCall> Faxouts
        {
            set
            {
                base.Faxouts = value.Where(d => d.ProjectID == _project.ID).ToList();
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

        [Display(Name = "项目名")]
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


        [Display(Name = "人均DealIn")]
        public decimal PerMemberDealIn
        {
            get
            {
                if (MemberCounts == 0 || TotalDealIn == null) return 0;
                return Math.Round(TotalDealIn.Value / MemberCounts, 1);
            }
        }
        [Display(Name = "人均Call数量")]
        public double PerMemberCall
        {
            get
            {
                if (MemberCounts == 0) return 0;
                return Math.Round((double)(FaxOutCount / MemberCounts), 1);
            }
        }
        [Display(Name = "人均CheckIn")]
        public decimal PerMemberCheckIn
        {
            get
            {
                if (MemberCounts == 0 || TotalCheckIn == null) return 0;
                return Math.Round(TotalCheckIn.Value / MemberCounts, 1);
            }
        }

        [Display(Name = "个人项目CheckIn总额")]
        public decimal TotalMemberCheckIn
        { get; set;}

    }

    public class AjaxWeekProjectProgressStatistics : AjaxProgress
     {
        [Display(Name="剩余工作周")]
        public int WeekLeft { get; set; } 
        [Display(Name = "项目CheckIn总额")]
        public decimal? TotalProjectCheckIn
        { get; set; }

         [Display(Name = "月份")]
         public int Month { get; set; }
         public int Year { get; set; }

         public override IEnumerable<Deal> Deals
         {
             set
             {
                 base.Deals = value.Where(d => d.ProjectID == _project.ID);
             }
         }

         public override  IEnumerable<LeadCall> Faxouts
         {
             set
             {
                 base.Faxouts = value.Where(d => d.ProjectID == _project.ID).ToList();
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


         public int? ProjectID { get { return _project.ID; } }

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


         [Display(Name = "人平均DealIn")]
         public decimal PerMemberDealIn
         {
             get
             {
                 if (MemberCounts == 0 || TotalDealIn == null) return 0;
                 return Math.Round(TotalDealIn.Value / MemberCounts, 1);
             }
         }
         [Display(Name = "人均Call数量")]
         public double PerMemberCall
         {
             get
             {
                 if (MemberCounts == 0) return 0;
                 return Math.Round((double)(FaxOutCount / MemberCounts), 1);
             }
         }
         [Display(Name = "人均Checkin")]
         public decimal PerMemberCheckIn
         {
             get
             {
                 if (MemberCounts == 0 || TotalCheckIn == null) return 0;
                 return Math.Round(TotalCheckIn.Value / MemberCounts, 1);
             }
         }
     }

    public class AjaxWeekMemberProgressStatistics : AjaxProgress
    {
        [Display(Name = "项目CheckIn总额")]
        public decimal? TotalProjectCheckIn
        { get; set; }

        public override DateTime StartDate { get; set; }

        public override DateTime EndDate { get; set; }

        [Display(Name = "销售姓名")]
        public string Name { get; set; }

    
    }

    public class AjaxWeekTotalProgressStatistics : AjaxProgress
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

        [Display(Name = "人平均DealIn")]
        public decimal PerMemberDealIn
        {
            get
            {
                if (MemberCounts == 0 || TotalDealIn == null) return 0;
                return Math.Round(TotalDealIn.Value / MemberCounts, 1);
            }
        }

        [Display(Name = "人均Call数量")]
        public double PerMemberCall
        {
            get
            {
                if (MemberCounts == 0) return 0;
                return Math.Round((double)(FaxOutCount / MemberCounts), 1);
            }
        }

        [Display(Name = "人平均Checkin")]
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