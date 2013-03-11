using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entity;
using System.ComponentModel.DataAnnotations;
using Utl;

namespace Model
{
    //时间段的ajax view
    public class AjaxStatistics
    {
        public virtual double CallHours { get; set; }

        public virtual IEnumerable<Lead> Leads { set { _leads = value; } }
        protected IEnumerable<Lead> _leads { get; set; }
        [Display(Name = "添加Lead数量")]
        public int LeadsCount
        {
            get
            {
                if (_leads == null) return 0;
                return _leads.Count();
            }   
        }
        public virtual IEnumerable<LeadCall> LeadCalls { set { _leadCalls = value; } }
        protected IEnumerable<LeadCall> _leadCalls { get; set; }
        [Display(Name = "填写LeadCall数量")]
        public int LeadCallsCount
        {
            get
            {   
                if (_leadCalls == null) return 0;
                return _leadCalls.Where(l => l.CallDate < EndDate && l.CallDate >= StartDate).Count();
            }
        }

        [Display(Name = "FaxOut数量")]
        public int FaxOutCount
        {
            get
            {
                if (_leadCalls == null) return 0;
                return _leadCalls.Where(l => l.CallDate < EndDate && l.CallDate >= StartDate && l.LeadCallType.Code>=40).Count();
            }
        }

        protected IEnumerable<Deal> _deals { get; set; }
        public virtual IEnumerable<Deal> Deals { set { _deals = value; } }
        public IEnumerable<CompanyRelationship> CompanyRelationships { get; set; }

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
            }
        }

        [Display(Name = "DealIn完成度")]
        public double DealInComplatePercetage
        {
            get
            {
                if (TotalDealinTargets == 0 || TotalDealinTargets == null|| TotalDealIn == null)
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
                if (TotalCheckinTargets == 0 || TotalCheckinTargets == null || TotalCheckIn == null)
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
}