using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model;
using Utl;
using Entity;

namespace BLL
{
    public class Report
    {
        private List<int> SetProjectSelectedList(List<int> selectedprojects)
        {
            if (selectedprojects == null)
            {
                selectedprojects = BLL.CRM_Logical.GetUserInvolveProject().Select(p => p.ID).ToList();
            }
            return selectedprojects;
        }

        //public List<AjaxViewProjectWeekPerformance> GetTotalMonthsPerformance(Project project)
        //{
        //    var totaldeals = from d in CH.DB.Deals where d.ProjectID == project.ID && d.Abandoned==false select d;
        //    var totaltarget = from t in CH.DB.TargetOfMonths where t.ProjectID == project.ID  select t;
        //    var signdeals = totaldeals.Where(t=>t.SignDate!=null).GroupBy(g => g.SignDate.Value.Month);
        //    var checkdeals = totaldeals.Where(t => t.ActualPaymentDate != null).GroupBy(g => g.SignDate.Value.Month);
        //    var startmonth = project.StartDate.Month;
        //    var endmonth = project.ConferenceEndDate.Month;

        //    List<AjaxViewProjectMonthPerformance>
        //    while (startmonth <= endmonth)
        //    {
                  
        //    }

        //}
        static List<AjaxViewProjectWeekPerformance> GetWeeksByMonth(int month, int year,List<Deal> totaldeals,Project p,List<TargetOfWeek> totaltarget)
        {
            DateTime startdate = new DateTime(year, month, 1);

            while (startdate.DayOfWeek != DayOfWeek.Monday)
            {
                startdate = startdate.AddDays(-1);
            }
            var weeks = new List<AjaxViewProjectWeekPerformance>();
            var enddate = startdate.AddDays(7);
            while (enddate.Month <= month)
             {
                 var week = new AjaxViewProjectWeekPerformance() { StartDate =  startdate, EndDate= enddate };
                 week.ProjectName = p.Name;
                 week.ProjectID = p.ID;
                 week.Manager = p.Manager;
                 totaldeals = totaldeals.FindAll(d => d.ProjectID == p.ID);
                var weektarget = totaltarget.FindAll(t=>t.StartDate.ToShortDateString() == startdate.ToShortDateString() && t.ProjectID==p.ID);
                week.CheckInTarget = weektarget.Sum(s => s.CheckIn);
                week.DealInTarget = weektarget.Sum(s => s.Deal);
                 week.DealIn = totaldeals.FindAll(d => d.SignDate >= startdate && d.SignDate < enddate).Sum(s => s.Payment);
                 week.CheckIn = totaldeals.FindAll(d => d.ActualPaymentDate >= startdate && d.ActualPaymentDate < enddate).Sum(s => s.Income);
                 weeks.Add(week);
                 
                 startdate = startdate.AddDays(7);
                 enddate = enddate.AddDays(7);
             }

             return weeks;
          
        }

        public static List<AjaxViewProjectMonthPerformance> GetProjectProgressList(List<int> selectedprojects,int month,int year)
        {
            List<Project> projects;
            if (selectedprojects == null)
            {
                return new List<AjaxViewProjectMonthPerformance>();
            }
            else
            {
                projects = CH.GetAllData<Project>(p => selectedprojects.Any(sp => sp == p.ID));
            }
            var totalmonthtatget = from t in CH.DB.TargetOfMonths where selectedprojects.Any(sp => sp == t.ProjectID) select t;
            var totaldeals = from d in CH.DB.Deals where selectedprojects.Any(sp => sp == d.ProjectID && d.Abandoned == false) select d;
            var totalweektargets = CH.GetAllData<TargetOfWeek>(t => selectedprojects.Any(sp => sp == t.ProjectID));

            var monthcheckindeals = from d in totaldeals where d.ActualPaymentDate != null && d.ActualPaymentDate.Value.Month == month select d;

            var monthdealindeals = from d in totaldeals where d.SignDate != null && d.SignDate.Value.Month == month select d;

            var list = new List<AjaxViewProjectMonthPerformance>();
            foreach(var p in projects)
            {
                var pv = new AjaxViewProjectMonthPerformance();
                pv.ProjectName = p.Name;
               var mchekcin = monthcheckindeals.Where(d=>d.ProjectID == p.ID).ToList();
                pv.CheckIn = mchekcin.Sum(s => s.Income);
                var mdealin = monthdealindeals.Where(d => d.ProjectID == p.ID).ToList();
                pv.DealIn = mdealin.Sum(s => s.Payment);
                pv.ProjectID = p.ID;
                var ctarget = from t in totalmonthtatget where t.ProjectID == p.ID select t;
                if(ctarget.Count()>0)
                {
                    pv.CheckInTarget = ctarget.First().CheckIn;
                    pv.DealInTarget = ctarget.First().Deal;
                }
               
                var weeks = GetWeeksByMonth(month, year, totaldeals.ToList(), p, totalweektargets);
                 
                pv.AjaxViewProjectWeekPerformances = weeks;
                list.Add(pv);
            }
            return list;
        }

        public static List<ViewProjectMemberProgressAmount> GetMemberProgressList(List<int> selectedprojects, bool? isActivated, DateTime? startdate, DateTime? enddate)
        {
            startdate = startdate == null ? new DateTime(1, 1, 1) : startdate;
            enddate = enddate == null ? new DateTime(9999, 1, 1) : startdate;

            if (selectedprojects == null)
            {
                selectedprojects = BLL.CRM_Logical.GetUserInvolveProject().Select(p => p.ID).ToList();
            }

            var list = new List<ViewProjectMemberProgressAmount>();
            var totaldeals = from d in CH.DB.Deals
                             where selectedprojects.Any(sp => sp == d.ProjectID)
                             select d;

            var deals = from d in totaldeals where d.SignDate >= startdate && d.SignDate <= enddate select d;

            var totaltargetofmonths = from t in CH.DB.TargetOfMonthForMembers
                                      where selectedprojects.Any(sp => sp == t.ProjectID)
                                      select t;

            var targetofmonths = from t in totaltargetofmonths
                                 where (t.StartDate >= startdate && t.StartDate <= enddate) || (t.StartDate >= startdate && t.StartDate <= enddate)
                                 select t;

            var projects = from p in CH.DB.Projects
                           where selectedprojects.Any(sp => sp == p.ID) && p.EndDate >= startdate && p.EndDate <= enddate
                           select p;



            foreach (var p in projects.ToList())
            {
                ViewProjectMemberProgressAmount pm = new ViewProjectMemberProgressAmount();
                pm.Project = p;
                var tms = from m in p.Members where p.IsActived == true select m;
                var memlist = new List<ViewMemberProgressAmount>();
                foreach (var m in tms)
                {
                    ViewMemberProgressAmount v = new ViewMemberProgressAmount();
                    v.Member = m;
                    var projectdeals = from d in deals where d.Sales == m.Name && d.ProjectID == p.ID select d;
                    var projecttargets = from t in targetofmonths where t.MemberID == m.ID && t.ProjectID == p.ID select t;

                    var projecttotaldeals = from d in totaldeals where d.Sales == m.Name && d.ProjectID == p.ID select d;
                    var projecttotaltargets = from t in totaltargetofmonths where t.MemberID == m.ID && t.ProjectID == p.ID select t;

                    if (projectdeals.Count() > 0)
                    {
                        v.CheckIn = projectdeals.Sum(d => d.Income);
                        v.DealIn = projectdeals.Sum(d => d.Payment);
                    }

                    if (projecttargets.Count() > 0)
                    {
                        v.CheckInTarget = projecttargets.Sum(t => t.CheckIn);
                        v.DealInTarget = projecttargets.Sum(t => t.Deal);
                    }

                    if (projecttotaldeals.Count() > 0)
                    {
                        v.TotalCheckIn = projecttotaldeals.Sum(d => d.Income);
                        v.TotalDealIn = projecttotaldeals.Sum(d => d.Payment);

                    }
                    if (projecttotaltargets.Count() > 0)
                    {
                        v.TotalDealinTarget = projecttotaltargets.Sum(t => t.Deal);
                    }
                    memlist.Add(v);
                }
                pm.ViewMemberProgressAmounts = memlist;
                list.Add(pm);
            }

            return list;

        }

        public static ViewPerformanceData GetMemberPerformanceIndex(List<int> selectedproject, int? month, List<string> members)
        {
            if (month == null) month = DateTime.Now.Month;

            var ps = CH.GetAllData<Project>(p => selectedproject.Contains(p.ID));

            DateTime startdate = new DateTime(DateTime.Now.Year, month.Value, 1);
            DateTime enddate = startdate.EndOfMonth();
            while (enddate.Month != startdate.Month)
            {
                var tempdate = enddate.AddDays(7);
                if (tempdate.Month == startdate.Month)
                    enddate = tempdate;
            }

            while (startdate.DayOfWeek != DayOfWeek.Monday)
            {
                startdate = startdate.AddDays(-1);
            }
            while (enddate.DayOfWeek != DayOfWeek.Friday)
            {
                enddate = enddate.AddDays(-1);
            }

            //enddate按照凌晨0点算， 所以结束时间多加1天
            enddate.AddDays(1);

            var deals = from d in CH.DB.Deals where d.Abandoned == false && d.ActualPaymentDate >= startdate && d.ActualPaymentDate <= enddate && members.Contains(d.Sales) select d;
            var signdeals = from d in CH.DB.Deals where d.Abandoned == false && d.SignDate >= startdate && d.SignDate <= enddate && members.Contains(d.Sales) select d;
            var targets = from t in CH.DB.TargetOfMonthForMembers where t.StartDate.Month == month && members.Contains(t.Member.Name) select t;
            var leads = from c in CH.DB.Leads where c.CreatedDate >= startdate && c.CreatedDate <= enddate select c;
            var phoneinfos = Utl.Utl.GetCallsInfoForPerformanceDataRows(startdate, enddate, members);

            var calllists = CH.GetAllData<LeadCall>(c => c.CallDate >= startdate && c.CallDate <= enddate && c.LeadCallType.Code >= 30 && members.Contains(c.Member.Name));
          

            var day = startdate;

            ViewPerformanceData data = new ViewPerformanceData();
            data.Deals = deals.ToList();
            data.SignedDeals = signdeals.ToList();
            data.Month = month.Value;
            data.Leads = leads.ToList();
            data.TargetOfMonthForMembers = targets.ToList();
            data.LeadCalls = calllists.ToList();
            data.StartDate = startdate;
            data.EndDate = enddate;
            data.ViewPhoneInfos = phoneinfos.ToList();

            return data;
        }

        public static ViewMemberPerformance GetSingleMemberPerformance(ViewPerformanceData records, string m)
        {
            var dcls = records.LeadCalls.FindAll(c => c.Member.Name == m).Distinct(new LeadCallLeadDistinct());
            var calllists = new List<LeadCall>();
            foreach (var c in dcls)
            {
                calllists.Add(c);
            }

            var contact = Employee.GetProfile("Contact", m).ToString();
            var callistbymember = calllists.GroupBy(o => o.CallDate.ToShortDateString());
            var extension = Employee.GetProfile("Contact", m).ToString();
            var leads = records.Leads.FindAll(c=>c.Creator == m);
            var phonecallmember = records.ViewPhoneInfos.FindAll(p => p["phone"].ToString() == extension).GroupBy(o => o["startdate"].ToString());
            DateTime temp = records.StartDate;
            var daylist = new List<ViewMemberDayWorkload>();
            var weeklist = new List<ViewMemberDayWorkload>();

            //计算日考核
            while (temp <= records.EndDate)
            {
                if (temp.IsWorkingday())
                {

                    var datestring = temp.ToShortDateString();
                    var workload = new ViewMemberDayWorkload();

                    //计算fax out
                    var tempcalllist = callistbymember.Where(c => c.Key == datestring);
                    //解开grouping
                    foreach (var tc in tempcalllist)
                    {
                        //只计算新lead的fax out
                        workload.FaxoutCount = tc.Count();
                    }
                    //计算
                    var tempdurantion = phonecallmember.Where(p => DateTime.Parse(p.Key).ToShortDateString() == datestring);
                    //解开grouping
                    foreach (var tc in tempdurantion)
                    {
                        var munites = tc.Sum(s => TimeSpan.Parse(s["duration"].ToString()).TotalMinutes);
                        workload.OnPhoneDuration = TimeSpan.FromMinutes(munites);
                    }
                    workload.Name = m;
                    workload.Day = datestring;
                    daylist.Add(workload);
                }
                temp = temp.AddDays(1);
            }


            //计算周考核
            DateTime tempstart = records.StartDate;
            DateTime tempend = records.StartDate.AddDays(7);
            var listweek = new List<ViewMemberWeekWorkload>();
            while (tempstart <= records.EndDate)
            {
                var week = new ViewMemberWeekWorkload(records.SignedDeals,tempstart, tempend);
                week.Leads = leads.FindAll(c => c.CreatedDate >= tempstart && c.CreatedDate <= tempend);
                var list = daylist.FindAll(f => DateTime.Parse(f.Day) <= tempend && DateTime.Parse(f.Day) >= tempstart);
                var totalmumite = list.Sum(s => s.OnPhoneDuration.TotalMinutes);
                week.OnPhoneDuration = TimeSpan.FromMinutes(totalmumite);
                week.FaxoutCount = list.Sum(s => s.FaxoutCount);
                listweek.Add(week);
                tempstart = tempend;
                tempend = tempstart.AddDays(7);
            }

            var data = new ViewMemberPerformance()
            {
                Name = m,
                Deals = records.Deals.FindAll(d => d.Sales == m),
                LeadCalls = records.LeadCalls.FindAll(l => l.Member.Name == m),
                Month = records.Month,
                TargetOfMonthForMembers = records.TargetOfMonthForMembers.FindAll(t => t.Member.Name == m),
                ViewMemberDayWorkloads = daylist,
                Leads = records.Leads.FindAll(c => c.Creator == m),
                ViewMemberWeekWorkloads = listweek
            };

            return data;
        }
    }
}