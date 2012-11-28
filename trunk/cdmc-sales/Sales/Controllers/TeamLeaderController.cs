using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utl;
using Entity;

namespace Sales.Controllers
{
    [LeaderRequired(AccessType= AccessType.Equal)]
    public class TeamLeaderController : Controller
    {
        //
        // GET: /TeamLeader/

        public ActionResult Index()
        {
            return View();
        }


        #region 目标划分
        public ActionResult BreakdownIndex(int? projectid)
        {
            projectid = this.TrySetProjectIDForUser(projectid);
            ViewBag.ProjectID = projectid;

            return View(CH.GetAllData<TargetOfMonth>(m => m.ProjectID == projectid));
        }

        public ActionResult AddBreakdown(int? projectid, int? targetofmonthid)
        {
            ViewBag.ProjectID = projectid;
            ViewBag.TargetOfMonthID = targetofmonthid;
            return View(@"~\views\targetofmonth\Breakdown.cshtml", CH.GetAllData<Member>(m => m.ProjectID == projectid));
        }

        [HttpPost]
        public ActionResult AddBreakdown(List<string> checkin, List<string> dealin, int projectid, int TargetOfMonthid, DateTime? startdate, DateTime? enddate, string OldDate)
        {
            ViewBag.ProjectID = projectid;
            ViewBag.TargetOfMonthID = TargetOfMonthid;
            this.AddErrorStateIfStartDateAndEndDateEmpty(startdate, enddate);
            if (ModelState.IsValid)
            {
                this.AddErrorStateIfNotFromMondayToFriday(startdate.Value, enddate.Value);
                this.AddErrorStateIfTargetOfWeekExist(startdate.Value, TargetOfMonthid);
            }
            if (ModelState.IsValid)
            {
                if (checkin != null)
                {
                    for (int i = 0; i < checkin.Count; i++)
                    {
                        var ck = checkin[i].Split('|');
                        var name = ck[0];
                        var ckvalue = ck[1];
                        var dl = dealin[i].Split('|');
                        var dlvalue = dl[1];

                        CH.Create<TargetOfWeek>(new TargetOfWeek() { CheckIn = Decimal.Parse(ckvalue), Deal = Decimal.Parse(dlvalue), EndDate = enddate.Value, Member = name, StartDate = startdate.Value, ProjectID = projectid, TargetOfMonthID = TargetOfMonthid });

                    }
                }

                return RedirectToAction("BreakdownIndex", "teamleader", new { id = projectid});
            }

            return View(@"~\views\targetofmonth\Breakdown.cshtml", CH.GetAllData<Member>(m => m.ProjectID == projectid));
        }

        public ActionResult EditBreakdown(int? projectid, int? targetofmonthid, string startdate)
        {
            ViewBag.ProjectID = projectid;
            ViewBag.TargetOfMonthID = targetofmonthid;
            ViewBag.OldStartDate = startdate;
            ViewBag.EndDate = DateTime.Parse(startdate).AddDays(4).ToShortDateString();

            if (startdate != null)
            {

                var targets = CH.GetAllData<TargetOfWeek>(i => i.StartDate.ToShortDateString() == startdate).OrderBy(i => i.StartDate).ToList();
                ViewBag.Targets = targets;
            }

            return View(@"~\views\targetofmonth\Breakdown.cshtml", CH.GetAllData<Member>(m => m.ProjectID == projectid));
        }

        [HttpPost]
        public ActionResult EditBreakdown(List<string> checkin, List<string> dealin, int projectid, int TargetOfMonthid, DateTime startdate, DateTime enddate, string OldDate)
        {
            ViewBag.ProjectID = projectid;
            ViewBag.TargetOfMonthID = TargetOfMonthid;

            this.AddErrorStateIfStartDateAndEndDateEmpty(startdate, enddate);
            if (ModelState.IsValid)
                this.AddErrorStateIfNotFromMondayToFriday(startdate, enddate);

            if (ModelState.IsValid)
            {
                if (checkin != null)
                {
                    for (int i = 0; i < checkin.Count; i++)
                    {
                        var ck = checkin[i].Split('|');
                        var name = ck[0];
                        var ckvalue = ck[1];
                        var dl = dealin[i].Split('|');
                        var dlvalue = dl[1];
                        var ts = CH.GetAllData<TargetOfWeek>(t => t.Member == name && t.ProjectID == projectid && t.TargetOfMonthID == TargetOfMonthid && startdate == t.StartDate);


                        var item = ts.FirstOrDefault();
                        if (item == null)
                        {
                            CH.Create<TargetOfWeek>(new TargetOfWeek() { CheckIn = Decimal.Parse(ckvalue), Deal = Decimal.Parse(dlvalue), EndDate = enddate, Member = name, StartDate = startdate, ProjectID = projectid, TargetOfMonthID = TargetOfMonthid });
                        }
                        else
                        {
                            item.Deal = Decimal.Parse(dlvalue);
                            item.CheckIn = Decimal.Parse(ckvalue);
                            CH.Edit<TargetOfWeek>(item);
                        }

                    }
                }

                return RedirectToAction("BreakdownIndex", "teamleader", new { id = projectid });
            }

            return View(@"~\views\targetofmonth\Breakdown.cshtml", CH.GetAllData<Member>(m => m.ProjectID == projectid));
        }

        public ActionResult DeleteBreakdown(string startdate, int? projectid)
        {
            ViewBag.StartDate = startdate;
            ViewBag.ProjectID = projectid;
            return View();
        }

        [HttpPost, ActionName("DeleteBreakdown")]
        public ActionResult DeleteConfirmed(string startdate,int? projectid)
        {
           var tws = CH.GetAllData<TargetOfWeek>(t => t.StartDate.ToShortDateString() == startdate && t.ProjectID == projectid);
           tws.ForEach(ti => {
               CH.Delete<TargetOfWeek>(ti.ID);
           });

           return RedirectToAction("BreakdownIndex", "teamleader", new { id = projectid });
        }
        #endregion

    }
}
