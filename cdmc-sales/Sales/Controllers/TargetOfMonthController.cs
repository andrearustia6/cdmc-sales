using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Entity;
using Sales;
using Utl;
using Telerik.Web.Mvc;
using Model;

namespace Sales.Controllers
{
    public class TargetOfMonthController : Controller
    {
        protected override void Dispose(bool disposing)
        {
            CH.DB.Dispose();
            base.Dispose(disposing);
        }

        #region 目标划分
        public ActionResult BreakdownIndex(int? projectid)
        {
            ViewBag.ProjectID = projectid;

            return View(CH.GetAllData<TargetOfMonth>(m => m.ProjectID == projectid).OrderByDescending(o => o.StartDate).ToList());
        }

        public ActionResult AddBreakdown(int? projectid, int? targetofmonthid)
        {
            ViewBag.ProjectID = projectid;
            ViewBag.TargetOfMonthID = targetofmonthid;
            return View("Breakdown", CH.GetAllData<Member>(m => m.ProjectID == projectid));
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
                return RedirectToAction("TargetOfMonthForProject", "Project");
                //return RedirectToAction("management", "project", new { id = projectid, tabindex = 2 });
            }

            return View("Breakdown", CH.GetAllData<Member>(m => m.ProjectID == projectid && m.IsActivated == true));
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

            return View("Breakdown", CH.GetAllData<Member>(m => m.ProjectID == projectid && m.IsActivated == true));
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

                return RedirectToAction("management", "project", new { id = projectid, tabindex = 2 });
            }
            return View("Breakdown", CH.GetAllData<Member>(m => m.ProjectID == projectid));
        }
        #endregion

        public ViewResult Index()
        {
            return View(CH.GetAllData<TargetOfMonth>());
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<TargetOfMonth>(id));
        }

        public ActionResult Create(int? projectid)
        {
            ViewBag.ProjectID = projectid;
            return View();
        }

        [HttpPost]
        public ActionResult Create(TargetOfMonth item)
        {
            this.AddErrorStateIfTargetOfMonthNoValid(item);
            if (ModelState.IsValid)
            {
                CH.Create<TargetOfMonth>(item);
                return RedirectToAction("Management", "Project", new { id = item.ProjectID, tabindex = 2 });
            }
            ViewBag.ProjectID = item.ProjectID;
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<TargetOfMonth>(id));
        }

        [HttpPost]
        public ActionResult Edit(TargetOfMonth item)
        {
            this.AddErrorStateIfTargetOfMonthNoValid(item);
            if (ModelState.IsValid)
            {
                CH.Edit<TargetOfMonth>(item);
                //return RedirectToAction("Management", "Project", new { id = item.ProjectID, tabindex = 2 });
                return RedirectToAction("TargetOfMonthForProject", "Project");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<TargetOfMonth>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var item = CH.GetDataById<TargetOfMonth>(id);
            var pid = item.ProjectID;
            CH.Delete<TargetOfMonth>(id);
            return RedirectToAction("TargetOfMonthForProject", "Project");
            //return RedirectToAction("Management", "Project", new { id = pid, tabindex = 2 });
        }


        public ActionResult ConfirmList(int? projectId)
        {
            ViewBag.selectVal = projectId;
            return View();
        }

        [GridAction]
        public ActionResult _SelectIndex(string filterId, int? projectId)
        {
            return View(new GridModel(getData(filterId, projectId)));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveAjaxEditing(int id)
        {
            var item = CH.GetDataById<TargetOfMonth>(id);
            item.Confirmor = Employee.CurrentUserName;
            item.IsConfirm = true;
            CH.Edit<TargetOfMonth>(item);
            return View(new GridModel(getData()));
        }

        public List<AjaxTargetOfMonth> getData(string filter = "", int? projectId = null)
        {
            var targets = from odb in CH.DB.TargetOfMonths select odb;
            if (filter == "1")//超级板块确认&财务确认
            {
                targets = targets.Where(t => t.IsConfirm == true);
            }
            else if (filter == "2")//超级板块确认&会务未确认
            {
                targets = targets.Where(t => t.IsAdminConfirm == true && (t.IsConfirm == false || t.IsConfirm == null));
            }
            else if (filter == "3")//超级板块未确认&会务未确认
            {
                targets = targets.Where(t => (t.IsAdminConfirm == false || t.IsAdminConfirm == null) && (t.IsConfirm == false || t.IsConfirm == null));
            }

            if (projectId != null)
            {
                targets = targets.Where(t => t.ProjectID == projectId);
            }

            var data = from db in targets

                       select new AjaxTargetOfMonth
                       {
                           ID = db.ID,
                           IsConfirm = db.IsConfirm == true ? "是" : "否",
                           ProjectID = db.ProjectID,
                           ProjectName = db.Project.Name_CH,
                           Deal = db.Deal,
                           BaseDeal = db.BaseDeal,
                           CheckIn = db.CheckIn,
                           EndDate = db.EndDate,
                           StartDate = db.StartDate,
                           IsAdminConfirm = db.IsAdminConfirm == true ? "是" : "否"
                       };
            return data.OrderByDescending(s => s.EndDate).ToList();
        }

        public ActionResult CreateEx(int? projectid)
        {
            ViewBag.ProjectID = projectid;
            return View();
        }

        [HttpPost]
        public ActionResult CreateEx(TargetOfMonth item)
        {
            this.AddErrorStateIfTargetOfMonthNoValid(item);

            if (ModelState.IsValid)
            {
                for (int i = 0; i < 5; i++)
                {
                    item.TargetOfWeeks[i].ProjectID = item.ProjectID;
                    item.TargetOfWeeks[i].TargetOfMonthID = item.ID;
                    item.TargetOfWeeks[i].Member = Employee.GetLoginUserName();
                }

                //选择月第一天为周一
                if (item.StartDate.DayOfWeek == DayOfWeek.Monday)
                {
                    item.TargetOfWeeks[0].StartDate = item.StartDate;
                    item.TargetOfWeeks[0].EndDate = item.StartDate.AddDays(4);
                    item.TargetOfWeeks[1].StartDate = item.StartDate.AddDays(7);
                    item.TargetOfWeeks[1].EndDate = item.StartDate.AddDays(11);
                    item.TargetOfWeeks[2].StartDate = item.StartDate.AddDays(14);
                    item.TargetOfWeeks[2].EndDate = item.StartDate.AddDays(18);
                    item.TargetOfWeeks[3].StartDate = item.StartDate.AddDays(21);
                    item.TargetOfWeeks[3].EndDate = item.StartDate.AddDays(25);
                    if (item.StartDate.AddDays(28) > item.EndDate)
                    {
                        item.TargetOfWeeks[4].StartDate = item.EndDate;
                        item.TargetOfWeeks[4].EndDate = item.EndDate;
                    }
                    else
                    {
                        item.TargetOfWeeks[4].StartDate = item.StartDate.AddDays(28);
                        item.TargetOfWeeks[4].EndDate = item.EndDate;
                    }
                }
                else if (item.StartDate.DayOfWeek == DayOfWeek.Saturday || item.StartDate.DayOfWeek == DayOfWeek.Sunday)//选择月第一天为周末
                {
                    DateTime firstMondayOfMonth = item.StartDate.AddDays(8 - (int)item.StartDate.DayOfWeek);
                    item.TargetOfWeeks[0].StartDate = firstMondayOfMonth;
                    item.TargetOfWeeks[0].EndDate = firstMondayOfMonth.AddDays(4);
                    item.TargetOfWeeks[1].StartDate = firstMondayOfMonth.AddDays(7);
                    item.TargetOfWeeks[1].EndDate = firstMondayOfMonth.AddDays(11);
                    item.TargetOfWeeks[2].StartDate = firstMondayOfMonth.AddDays(14);
                    item.TargetOfWeeks[2].EndDate = firstMondayOfMonth.AddDays(18);
                    item.TargetOfWeeks[3].StartDate = firstMondayOfMonth.AddDays(21);
                    item.TargetOfWeeks[3].EndDate = firstMondayOfMonth.AddDays(25);
                    if (item.StartDate.AddDays(28) > item.EndDate)
                    {
                        item.TargetOfWeeks[4].StartDate = item.EndDate;
                        item.TargetOfWeeks[4].EndDate = item.EndDate;
                    }
                    else
                    {
                        item.TargetOfWeeks[4].StartDate = item.StartDate.AddDays(28);
                        item.TargetOfWeeks[4].EndDate = item.EndDate;
                    }
                }
                else//
                {
                    DateTime firstFridayOfMonth = item.StartDate.AddDays(5 - (int)item.StartDate.DayOfWeek);
                    item.TargetOfWeeks[0].StartDate = item.StartDate;
                    item.TargetOfWeeks[0].EndDate = firstFridayOfMonth;
                    item.TargetOfWeeks[1].StartDate = firstFridayOfMonth.AddDays(4);
                    item.TargetOfWeeks[1].EndDate = firstFridayOfMonth.AddDays(7);
                    item.TargetOfWeeks[2].StartDate = firstFridayOfMonth.AddDays(11);
                    item.TargetOfWeeks[2].EndDate = firstFridayOfMonth.AddDays(14);
                    item.TargetOfWeeks[3].StartDate = firstFridayOfMonth.AddDays(18);
                    item.TargetOfWeeks[3].EndDate = firstFridayOfMonth.AddDays(21);
                    if (item.StartDate.AddDays(24) > item.EndDate)
                    {
                        item.TargetOfWeeks[4].StartDate = item.EndDate;
                        item.TargetOfWeeks[4].EndDate = item.EndDate;
                    }
                    else
                    {
                        item.TargetOfWeeks[4].StartDate = item.StartDate.AddDays(24);
                        item.TargetOfWeeks[4].EndDate = item.EndDate;
                    }
                }
                CH.Create<TargetOfMonth>(item);
                return RedirectToAction("TargetOfMonthForProject", "Project", new { projectid = item.ProjectID });
            }
            return View(item);
        }

        public ActionResult AdminConfirmList(int? projectId)
        {
            ViewBag.selectVal = projectId;
            return View();
        }

        [GridAction]
        public ActionResult _AdminSelectIndex(string filterId, int? projectid)
        {
            return View(new GridModel(getData(filterId, projectid)));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _AdminSaveAjaxEditing(int id)
        {
            var item = CH.GetDataById<TargetOfMonth>(id);
            item.AdminConfirmor = Employee.GetLoginUserName();
            item.IsAdminConfirm = true;
            CH.Edit<TargetOfMonth>(item);
            return View(new GridModel(getData()));
        }



    }
}