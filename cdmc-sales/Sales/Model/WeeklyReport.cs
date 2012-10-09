using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entity;
using Utl;

namespace Sales.Model
{
    public class WeeklyItem
    {
        public Project Project { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime SetDate { get; set; }
        public WeeklyItem(Project p, DateTime setdate)
        {
            Project = p;
            SetDate = setdate;
            if (setdate.DayOfWeek == DayOfWeek.Monday)
                StartDate = setdate;
            else
                StartDate = setdate.StartOfWeek();
            EndDate = SetDate.EndOfWeek();
        }

        protected bool InTheWeek(DateTime? d)
        {
            if (d == null) return false;

            if (d >= StartDate && d <= EndDate)
                return true;
            else
                return false;
        }
        protected bool InNextWeek(DateTime? d)
        {
            if (d == null) return false;

            if (d >= StartDate.AddDays(7) && d <= EndDate.AddDays(7))
                return true;
            else
                return false;
        }
        
    }
    public class WeeklyReport : WeeklyItem
    {
        public List<MemberItem> MemberItems { get; set; }

        public WeeklyReport(Project p,DateTime setdate)
            : base(p,setdate)
        {
            MemberItems = new List<MemberItem>();
            p.Members.ForEach(m => {
                var mdata = new MemberItem(p, setdate, m);
                var t1 = mdata.LeadCalls;
                var t2 = mdata.Deals;
                MemberItems.Add(mdata);
            });
        }
    }

    public class MemberItem : WeeklyItem
    {

        public MemberItem(Project p,DateTime setdate, Member m)
            : base(p,setdate)
        {
            Member = m;
        }

        public string Duration { get { return StartDate.ToShortDateString() + "~" + EndDate.ToShortDateString(); } }
        public Member Member { get; set; }

        public decimal DealsAmount
        {
            get
            {
                decimal amount=0;
                Deals.ForEach(d =>
                {
                    amount += d.Payment;
                }); 

                 return amount;
            }
        }
        public decimal IncomesAmount
        {
            get
            {
                decimal amount = 0;
                Deals.ForEach(d =>
                {
                    amount += d.Income;
                });

                return amount;
            }
        }
        public double CompleteRate{
            get { return TargetAmount == 0 ? 100 : (int)((DealsAmount / TargetAmount) * 100); }
        }

        public double CollectRate
        {
            get { return TotalDealsAmount == 0 ? 100 : (int)((TotalIncomesAmount / TotalDealsAmount) * 100); }
        }


        public List<Deal> Deals
        {
            get
            {
                var data = new List<Deal>();
                Project.CompanyRelationships.ForEach(c=>{
                    c.Deals.ForEach(d=>{
                        if(InTheWeek(d.ActualPaymentDate) && d.Sales == Member.Name && d.Abandoned == false)
                        {
                            data.Add(d);
                        }
                    });
                });
             
                return data;
            }
        }
        public TargetOfWeek Target
        {
            get
            {
                var ts = CH.GetAllData<TargetOfWeek>(t=>t.Member == Member.Name);
                var data = ts.FindAll(t => InTheWeek(t.StartDate));
                return data.FirstOrDefault();
            }
        }
        public decimal TargetAmount
        {
            get
            {
                return Target == null ? 0 : Target.Deal;
            }
        }
        public TargetOfWeek NextWeekTarget
        {
            get
            {
                var ts = CH.GetAllData<TargetOfWeek>(t => t.Member == Member.Name);
                var data = ts.FindAll(t => InNextWeek(t.StartDate));
                return data.FirstOrDefault();
            }
        }
        public decimal NextWeekTargetAmount
        {
            get
            {
                return NextWeekTarget==null?0:NextWeekTarget.Deal;
            }
        }

        public List<Deal> NextWeekExpectedDeals
        {
            get
            {
                var data = Project.Deals.FindAll(d => InNextWeek(d.ExpectedPaymentDate) && d.Sales == Member.Name && d.Abandoned == false);
                return data;
            }
        }
   

        public decimal NextWeekExpectedDealsAmount
        {
            get
            {
                decimal amount =0;
                NextWeekExpectedDeals.ForEach(d => { amount += d.Payment; });
                return amount;
            }
        }

        public List<Deal> TotalDeals
        {
            get
            {
                var data = Project.Deals.FindAll(d => d.Sales == Member.Name && d.Abandoned==false);
                return data;
            }
        }

        public decimal TotalDealsAmount
        {
            get
            {
                decimal amount = 0;
                TotalDeals.ForEach(d => { amount += d.Payment; });
                return amount;
            }
        }

        public decimal TotalIncomesAmount
        {
            get
            {
                decimal amount = 0;
                TotalDeals.ForEach(d => { amount += d.Income; });
                return amount;
            }
        }

        List<Lead> _leads;
        public List<Lead> Leads
        {
            get
            {
                if (_leads == null)
                {
                    Project.CompanyRelationships.ForEach(c => { _leads.AddRange(c.Company.Leads); });
               
                }
                return _leads;
            }
        }

        public List<LeadCall> LeadCalls
        {
            get
            {
                var result = new List<LeadCall>();
                var lsall = CH.GetAllData<LeadCall>();
                Leads.ForEach(l =>
                {
                    var ls = lsall.FindAll(lc => lc.LeadID == l.ID);
                    ls.ForEach(call =>
                    {
                        if (InTheWeek(call.CallingTime) && call.Caller.ToLower() == Member.Name.ToLower())
                        {
                            result.Add(call);
                        }
                    });
                });
                return result;
            }
        }
    }
}