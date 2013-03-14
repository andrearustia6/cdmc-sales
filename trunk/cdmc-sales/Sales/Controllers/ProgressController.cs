using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using Model;
using Utl;

namespace Sales.Controllers
{
    public class ProgressReportController : Controller
    {
        public ActionResult ProjectProgress(int? month, int? year, string selecttype)
        {
            ViewBag.Month = month;
            ViewBag.Year = year;

            return View();
        }

        //Func<bool> f
        [GridAction]
        public ActionResult _MonthProgress()
        {
            var year = DateTime.Now.Year;

            var deals = from d in CH.DB.Deals where d.Project.IsActived == true && d.Abandoned == false select d;
            var targets = from t in CH.DB.TargetOfMonths where t.Project.IsActived == true select t;
            var calls = from c in CH.DB.LeadCalls where c.CompanyRelationship.Project.IsActived == true select c;
            //var companys = from c in CH.DB.CompanyRelationships where c.Project.IsActived == true select c;
            var months = new List<int>();

            while (months.Count < DateTime.Now.Month)
            {
                months.Add(months.Count() + 1);
            }

            var ps = from p in CH.DB.Projects where p.IsActived == true select p;

            //如果是manager 只能看到自己的项目
            if (Employee.EqualToManager())
            {
                var managername = Employee.CurrentUserName;
                ps = ps.Where(p => p.Manager ==managername );
                deals = deals.Where(d => d.Project.Manager == managername);
                targets = targets.Where(d => d.Project.Manager == managername);
                calls = calls.Where(c => c.CompanyRelationship.Project.Manager == managername);
            }

            var list = from i in months
                       select new AjaxMonthTotalProgressStatistics()
                       {
                           Month = i,
                           Year = year,
                           Faxouts = calls.ToList(),
                           Deals = deals,
                           Projects = ps,
                           TotalDealinTargets = targets.Where(t => t.StartDate.Month == i).Sum(s => (decimal?)s.Deal),
                           TotalCheckinTargets = targets.Where(t => t.StartDate.Month == i).Sum(s => (decimal?)s.CheckIn)
                       };

            return View(new GridModel<AjaxMonthTotalProgressStatistics> { Data = list.ToList() });
        }

        [GridAction]
        public ActionResult _ProjectWeekProgress(string startdate)
        {
            if (string.IsNullOrEmpty(startdate))
                return View(new GridModel<AjaxWeekProjectProgressStatistics> { Data = new List<AjaxWeekProjectProgressStatistics>() });

            DateTime startDate = DateTime.Parse(startdate);
            DateTime endDate = startDate.AddDays(7);

            var year = DateTime.Now.Year;
            var deals = from d in CH.DB.Deals where d.Project.IsActived == true && d.Abandoned == false select d;
            var targets = from t in CH.DB.TargetOfWeeks where t.Project.IsActived == true select t;
            var calls = from c in CH.DB.LeadCalls where c.CompanyRelationship.Project.IsActived == true select c;
            //var totalprojectcheckin = from d in CH.DB.Deals where d.Project.IsActived == true && d.Abandoned == false select d;
            var ps = from p in CH.DB.Projects where p.IsActived == true select p;
           

            //如果是manager 只能看到自己的项目
            if (Employee.EqualToManager())
            {
                var managername = Employee.CurrentUserName;
                ps = ps.Where(p => p.Manager == managername);
                deals = deals.Where(d => d.Project.Manager == managername);
                targets = targets.Where(d => d.Project.Manager == managername);
               // totalprojectcheckin = totalprojectcheckin.Where(d => d.Project.Manager == managername);
                calls = calls.Where(c => c.CompanyRelationship.Project.Manager == managername);
            }
            var pslist = ps.ToList();
            var list = from p in pslist
                       select new AjaxWeekProjectProgressStatistics()
                       {
                           WeekLeft = (p.EndDate-DateTime.Now).Days/7,
                           Project = p,
                           StartDate = startDate,
                           EndDate = endDate,
                           Year = year,
                           Faxouts = calls.ToList(),
                           Deals = deals,
                           TotalDealinTargets = targets.Where(t => t.StartDate == startDate && t.ProjectID == p.ID).Sum(s => (decimal?)s.Deal),
                           TotalCheckinTargets = targets.Where(t => t.StartDate == startDate && t.ProjectID == p.ID).Sum(s => (decimal?)s.CheckIn),
                           TotalProjectCheckIn =deals.Where(w=>w.ProjectID==p.ID).Sum(s=>(decimal?)s.Income)
                       };
            var data = list.ToList();

            return View(new GridModel<AjaxWeekProjectProgressStatistics> { Data = data });

        }

        [GridAction]
        public ActionResult _ProjectMonthProgress(string month)
        {
            var year = DateTime.Now.Year;

            int monthid = DateTime.Now.Month;
            if (month != null)
                Int32.TryParse(month, out monthid);

            var deals = from d in CH.DB.Deals where d.Project.IsActived == true && d.Abandoned == false select d;
            var targets = from t in CH.DB.TargetOfMonths where t.Project.IsActived == true && t.StartDate.Month == monthid select t;
            var calls = from c in CH.DB.LeadCalls where c.CompanyRelationship.Project.IsActived == true select c;
            //var companys = from c in CH.DB.CompanyRelationships where c.Project.IsActived == true select c;

            var ps = from p in CH.DB.Projects where p.IsActived == true select p;

            //如果是manager 只能看到自己的项目
            if (Employee.EqualToManager())
            {
                var managername = Employee.CurrentUserName;
                ps = ps.Where(p => p.Manager == managername);
                deals = deals.Where(d => d.Project.Manager == managername);
                targets = targets.Where(d => d.Project.Manager == managername);
                calls = calls.Where(c => c.CompanyRelationship.Project.Manager == managername);
            }

            var pslist = ps.ToList();
            var list = from p in pslist
                       select new AjaxMonthProjectProgressStatistics()
                       {
                           Project = p,
                           Month = monthid,
                           Year = year,
                           Faxouts = calls.ToList(),
                           Deals = deals,
                           TotalDealinTargets = targets.Where(t => t.StartDate.Month == monthid && t.ProjectID == p.ID).Sum(s => (decimal?)s.Deal),
                           TotalCheckinTargets = targets.Where(t => t.StartDate.Month == monthid && t.ProjectID == p.ID).Sum(s => (decimal?)s.CheckIn)
                       };
            var data = list.ToList();
            return View(new GridModel<AjaxMonthProjectProgressStatistics> { Data = data });

        }

        /// <summary>
        /// total week
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        [GridAction]
        public ActionResult _WeekProgress(string month)
        {
            int monthid = DateTime.Now.Month;
            if (month != null)
                Int32.TryParse(month, out monthid);

            DateTime startdate = new DateTime(DateTime.Now.Year, monthid, 1);

            while (startdate.DayOfWeek != DayOfWeek.Monday)
            {
                startdate = startdate.AddDays(-1);
            }

            var weeks = new List<AjaxWeekTotalProgressStatistics>();
            var enddate = startdate.AddDays(7);

            var year = DateTime.Now.Year;
            var deals = from d in CH.DB.Deals where d.Project.IsActived == true && d.Abandoned == false select d;
            var targets = from t in CH.DB.TargetOfWeeks where t.Project.IsActived == true select t;
            var calls = from c in CH.DB.LeadCalls where c.CompanyRelationship.Project.IsActived == true select c;

            //如果是manager 只能看到自己的项目
            if (Employee.EqualToManager())
            {
                var managername = Employee.CurrentUserName;
                deals = deals.Where(d => d.Project.Manager == managername);
                targets = targets.Where(d => d.Project.Manager == managername);
                calls = calls.Where(c => c.CompanyRelationship.Project.Manager == managername);
            }

            while (enddate.Month <= monthid)
            {
                var week = new AjaxWeekTotalProgressStatistics() { StartDate = startdate, EndDate = enddate };


                week.TotalCheckinTargets = targets.Where(t => t.StartDate == startdate).Sum(s => (decimal?)s.CheckIn);
                week.TotalDealinTargets = targets.Where(t => t.StartDate == startdate).Sum(s => (decimal?)s.Deal);
                week.Deals = deals;
                week.Faxouts = calls.ToList();
                weeks.Add(week);

                startdate = startdate.AddDays(7);
                enddate = enddate.AddDays(7);
            }

            return View(new GridModel<AjaxWeekTotalProgressStatistics> { Data = weeks.ToList() });

        }

        [GridAction]
        public ActionResult _MemberWeekProgress(string startdate,int? projectid)
        {
            if (string.IsNullOrEmpty(startdate) || projectid==null)
                return View(new GridModel<AjaxWeekMemberProgressStatistics> { Data = new List<AjaxWeekMemberProgressStatistics>() });

            DateTime startDate = DateTime.Parse(startdate);
            DateTime endDate = startDate.AddDays(7);
            var members = from m in CH.DB.Members where m.ProjectID == projectid select m;
            var memberlist = members.ToList();
            var year = DateTime.Now.Year;
            var deals = from d in CH.DB.Deals where d.ProjectID == projectid && d.Project.IsActived == true && d.Abandoned == false select d;
            var targets = from t in CH.DB.TargetOfWeeks where t.Project.IsActived == true && t.ProjectID == projectid select t;
            var calls = from c in CH.DB.LeadCalls where c.ProjectID == projectid select c;

            var ps = from p in CH.DB.Projects where p.IsActived == true select p;

            //如果是manager 只能看到自己的项目
            if (Employee.EqualToManager())
            {
                var managername = Employee.CurrentUserName;
                deals = deals.Where(d => d.Project.Manager == managername);
                targets = targets.Where(d => d.Project.Manager == managername);
                calls = calls.Where(c => c.CompanyRelationship.Project.Manager == managername);
                memberlist = memberlist.Where(m => m.Project.Manager == managername).ToList();
            }

            var pslist = ps.ToList();
            var list = from m in memberlist
                       select new AjaxWeekMemberProgressStatistics()
                       {
                           Name = m.Name,
                           StartDate = startDate,
                           EndDate = endDate,
                           Faxouts = calls.Where(w=>w.Member.Name == m.Name).ToList(),
                           Deals = deals.Where(c=>c.Sales == m.Name),
                           TotalDealinTargets = targets.Where(t => t.StartDate == startDate && t.Member == m.Name && t.ProjectID == projectid).Sum(s => (decimal?)s.Deal),
                           TotalCheckinTargets = targets.Where(t => t.StartDate == startDate && t.Member == m.Name && t.ProjectID == projectid).Sum(s => (decimal?)s.CheckIn),
                           TotalProjectCheckIn = deals.Where(w => w.Sales == m.Name).Sum(s => (decimal?)s.Income)
                       };

            var data = list.ToList();

            return View(new GridModel<AjaxWeekMemberProgressStatistics> { Data = data });


        }

        public ActionResult Progress()
        {
            return View();
        }
    }
}
