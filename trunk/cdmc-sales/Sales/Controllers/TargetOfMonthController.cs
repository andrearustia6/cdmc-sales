﻿using System;
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

                return RedirectToAction("management", "project", new { id = projectid, tabindex = 2 });
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
                return RedirectToAction("Management", "Project", new { id = item.ProjectID, tabindex = 2 });
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

            return RedirectToAction("Management", "Project", new { id = pid, tabindex = 2 });
        }


        public ActionResult ConfirmList()
        {
            return View();
        }

        [GridAction]
        public ActionResult _SelectIndex()
        {
            return View(new GridModel(getData()));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveAjaxEditing(int id)
        {
            var item = CH.GetDataById<TargetOfMonth>(id);
            item.IsConfirm = true;
            CH.Edit<TargetOfMonth>(item);

            return View(new GridModel(getData()));
        }

        public List<AjaxTargetOfMonth> getData()
        {
            var data = from db in CH.DB.TargetOfMonths
                       where db.IsConfirm == null || db.IsConfirm == false
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
                           StartDate = db.StartDate
                       };
            return data.OrderByDescending(s => s.EndDate).ToList();
        }
    }
}