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
using BLL;

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
                //return RedirectToAction("TargetOfMonthForProject", "Project");
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
                //return RedirectToAction("TargetOfMonthForProject", "Project");
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
            //return RedirectToAction("TargetOfMonthForProject", "Project");
            return RedirectToAction("Management", "Project", new { id = pid, tabindex = 2 });
        }

        public List<AjaxTargetOfMonth> getData(string filter = "", int? projectId = null)
        {
            var targets = from odb in CH.DB.TargetOfMonths select odb;
            if (filter == "1")//超级版块确认&财务确认
            {
                targets = targets.Where(t => t.IsConfirm == true);
            }
            else if (filter == "2")//超级版块确认&财务未确认
            {
                targets = targets.Where(t => t.IsAdminConfirm == true && (t.IsConfirm == false || t.IsConfirm == null));
            }
            else if (filter == "3")//超级版块未确认&财务未确认
            {
                targets = targets.Where(t => (t.IsAdminConfirm == false || t.IsAdminConfirm == null) && (t.IsConfirm == false || t.IsConfirm == null));
            }
            else if (filter == "4")
            {
                DateTime currentMonth = Convert.ToDateTime(DateTime.Today.ToString("yyyy-MM-01") + " 0:00:00");
                targets = targets.Where(t => t.StartDate == currentMonth);
            }
            else if (filter == "5")
            {
                DateTime previousMonth = Convert.ToDateTime(DateTime.Today.AddMonths(-1).ToString("yyyy-MM-01") + " 0:00:00");
                targets = targets.Where(t => t.StartDate == previousMonth);
            }

            if (projectId != null)
            {

                targets = targets.Where(t => t.ProjectID == projectId);
            }

            var data = from db in targets

                       select new AjaxTargetOfMonth
                       {
                           ID = db.ID,
                           ProjectID = db.ProjectID,
                           Creator = db.Creator,
                           ProjectName = (db.Project.Name_EN ?? string.Empty) + " | " + (db.Project.Name_CH ?? string.Empty),
                           Deal = db.Deal,
                           Manger = db.Project.Manager,
                           BaseDeal = db.BaseDeal,
                           CheckIn = db.CheckIn,
                           EndDate = db.EndDate,
                           StartDate = db.StartDate,
                           IsConfirm = db.IsConfirm == true ? "是" : "否",
                           IsAdminConfirm = db.IsAdminConfirm == true ? "是" : "否",
                           TargetOf1stWeek = db.TargetOf1stWeek,
                           TargetOf2ndWeek = db.TargetOf2ndWeek,
                           TargetOf3rdWeek = db.TargetOf3rdWeek,
                           TargetOf4thWeek = db.TargetOf4thWeek,
                           TargetOf5thWeek = db.TargetOf5thWeek
                       };

            return data.OrderByDescending(s => s.EndDate).ToList();
        }

        public ActionResult EditEx(int id)
        {
            return View(CH.GetDataById<TargetOfMonth>(id));
        }

        public ViewResult DetailsEx(int id)
        {
            return View(CH.GetDataById<TargetOfMonth>(id));
        }

        public ActionResult DeleteEx(int id)
        {
            return View(CH.GetDataById<TargetOfMonth>(id));
        }

        public ActionResult CreateEx(int? projectid)
        {
            ViewBag.ProjectID = projectid;
            return View();
        }

        #region 财务确认项目月目标

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

        #endregion

        #region 超版确认项目月目标

        public ActionResult AdminConfirmList(int? projectId)
        {
            ViewBag.selectVal = projectId;
            return View();
        }

        [GridAction]
        public ActionResult _AdminSelectIndex(string filterId, int? projectid)
        {
            ViewBag.selectVal = projectid;
            return View(new GridModel(getData(filterId, projectid)));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _AdminSaveAjaxEditing(int id, int? projectid)
        {
            var item = CH.GetDataById<TargetOfMonth>(id);
            item.AdminConfirmor = Employee.GetLoginUserName();
            item.IsAdminConfirm = true;
            CH.Edit<TargetOfMonth>(item);
            return View(new GridModel(getData("", projectid)));
        }

        #endregion

        #region 版块编辑项目月目标

        public ActionResult TargetOfMonthForProject(int? projectid)
        {
            //projectid = this.TrySetProjectIDForUser(projectid);

            ViewBag.ProjectID = projectid;
            //return View(CH.GetAllData<TargetOfMonth>().Where(s => s.Project.TeamLeader == Employee.GetLoginUserName()).OrderByDescending(s => s.EndDate).ToList());
            if (projectid == null)
            {
                return View(CH.GetAllData<TargetOfMonth>().Where(s => CRM_Logical.GetUserInvolveProject().Any(m => m.ID == s.ProjectID)).OrderByDescending(s => s.EndDate).ToList());
            }
            else
            {
                return View(CH.GetAllData<TargetOfMonth>().Where(s => s.ProjectID == projectid).OrderByDescending(s => s.EndDate).ToList());
            }

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateEx(TargetOfMonth item)
        {
            this.AddErrorStateIfTargetOfMonthNoValid(item);

            if (ModelState.IsValid)
            {
                CH.Create<TargetOfMonth>(item);
                return RedirectToAction("TargetOfMonthForProject", "TargetOfMonth", new { projectid = item.ProjectID });
            }
            return View("TargetOfMonthForProject", CH.GetAllData<TargetOfMonth>().Where(s => s.ProjectID == item.ProjectID).OrderByDescending(s => s.EndDate).ToList());
        }

        [AcceptVerbs(HttpVerbs.Post), ActionName("DeleteEx")]
        public ActionResult DeleteExConfirmed(int id, int? projectid)
        {
            var item = CH.GetDataById<TargetOfMonth>(id);
            var pid = item.ProjectID;
            CH.Delete<TargetOfMonth>(id);
            var del = CH.GetAllData<TargetOfWeek>().Where(s => s.TargetOfMonthID == item.ID);
            if (del != null && del.Count() > 0)
            {
                CH.DeleteRange<TargetOfWeek>(del.ToList());
            }
            return RedirectToAction("TargetOfMonthForProject", "TargetOfMonth", new { projectid = pid });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditEx(TargetOfMonth item)
        {
            this.AddErrorStateIfTargetOfMonthNoValid(item);
            if (ModelState.IsValid)
            {
                CH.Edit<TargetOfMonth>(item);
                return RedirectToAction("TargetOfMonthForProject", "TargetOfMonth", new { projectid = item.ProjectID });
            }
            return View("TargetOfMonthForProject", CH.GetAllData<TargetOfMonth>().Where(s => s.ProjectID == item.ProjectID).OrderByDescending(s => s.EndDate).ToList());
        }
        #endregion
    }
}