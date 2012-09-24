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

        protected bool InTheWeek(DateTime d)
        {
            if (d >= StartDate && d <= EndDate)
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

        
        public Member Member { get; set; }
        public List<Deal> Deals
        {
            get
            {
                var data = Project.Deals.FindAll(d => InTheWeek(d.ExpectedPaymentDate) && d.Sales == Member.Name);
                return data;
            }
        }
        public TargetOfWeek Targets
        {
            get
            {
                var data = Member.TargetOfWeeks.FindAll(t=> InTheWeek(t.StartDate));
                return data!=null?data.FirstOrDefault():null;
            }
        }
        public List<Lead> Leads
        {
            get
            {
                return Project.Leads;
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