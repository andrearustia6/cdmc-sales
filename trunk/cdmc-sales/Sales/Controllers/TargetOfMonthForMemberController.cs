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
using BLL;
using System.Data.Entity.Infrastructure;
using Telerik.Web.Mvc;
using Model;

namespace Sales.Controllers
{
    [SalesRequired]
    public class TargetOfMonthForMemberController : Controller
    {
        protected override void Dispose(bool disposing)
        {
            CH.DB.Dispose();
            base.Dispose(disposing);
        }

        public ViewResult Index(List<int> selectedprojects, bool? isActivated, DateTime? startdate, DateTime? enddate)
        {
            startdate = startdate == null ? new DateTime(1, 1, 1) : startdate;
            enddate = enddate == null ? new DateTime(9999, 1, 1) : startdate;

            if (selectedprojects != null)
            {
                var ts = from t in CH.DB.TargetOfMonthForMembers
                         where selectedprojects.Any(sp => sp == t.ProjectID)
                         select t;
                return View(ts.OrderByDescending(t => t.CreatedDate).ToList());
            }
            else
            {
                var ps = CRM_Logical.GetUserInvolveProject();
                var rs = CH.GetAllData<TargetOfMonthForMember>(r => ps.Any(sp => sp.ID == r.ProjectID) && r.CreatedDate >= startdate && r.CreatedDate <= enddate);
                return View(rs);
            }

        }



        public ViewResult MyTargetIndex(int? projectid)
        {
            var list = CH.DB.ChangeTracker.Entries<TargetOfMonthForMember>().ToList();
            projectid = this.TrySetProjectIDForUser(projectid);
            ViewBag.ProjectID = projectid;
            string name = Employee.CurrentUserName;
            if (projectid != null)
            {
                list = CH.DB.ChangeTracker.Entries<TargetOfMonthForMember>().ToList();
                //var data = CH.GetAllData<TargetOfMonthForMember>(t => t.ProjectID == projectid && t.Member.Name == Employee.CurrentUserName);

                //return View(data);
                var data = from t in CH.DB.TargetOfMonthForMembers.AsNoTracking()
                           where t.ProjectID == projectid && t.Member.Name == name
                           select t;

                var td = data.OrderByDescending(o => o.EndDate).ToList();

                list = CH.DB.ChangeTracker.Entries<TargetOfMonthForMember>().ToList();
                if (list.Count > 0)
                {
                    var id = list.First().Entity.ID;
                }
                return View(td);
            }
            return View();

        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<TargetOfMonthForMember>(id));
        }

        public ActionResult Create(int? projectid)
        {
            projectid = this.TrySetProjectIDForUser(projectid);
            ViewBag.ProjectID = projectid;
            var name = Employee.CurrentUserName;
            if (Employee.EqualToLeader() || Employee.EqualToSales())
            {
                return View();
            }
            else
                return View(@"~\views\shared\Error.cshtml", null, "没有权限");


        }

        [HttpPost]
        public ActionResult Create(TargetOfMonthForMember item)
        {
            this.AddErrorStateIfTargetOfMonthNoValid(item);
            if (ModelState.IsValid)
            {
                CH.Create<TargetOfMonthForMember>(item);
                return RedirectToAction("MyTargetIndex", new { projectid = item.ProjectID });
            }
            ViewBag.ProjectID = item.ProjectID;
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            var data = CH.GetDataById<TargetOfMonthForMember>(id);
            var list = CH.DB.ChangeTracker.Entries<TargetOfMonthForMember>().ToList();

            //var data = CH.DB.TargetOfMonthForMembers.AsNoTracking().Single(x => x.ID == id);
            list = CH.DB.ChangeTracker.Entries<TargetOfMonthForMember>().ToList();
            return View(data);
        }

        [HttpPost]
        public ActionResult Edit(TargetOfMonthForMember item)
        {

            //this.AddErrorStateIfTargetOfMonthNoValid(item);
            CH.DB.SaveChanges();

            CH.DB.ChangeTracker.DetectChanges();
            var list = CH.DB.ChangeTracker.Entries<TargetOfMonthForMember>().ToList();
            if (list.Count > 0)
            {
                var attacth = list.First().Entity;
                CH.DB.Detach(attacth);
            }

            list = CH.DB.ChangeTracker.Entries<TargetOfMonthForMember>().ToList();
            if (ModelState.IsValid)
            {

                CH.Edit<TargetOfMonthForMember>(item);
                return RedirectToAction("MyTargetIndex", new { projectid = item.ProjectID });
            }
            ViewBag.ProjectID = item.ProjectID;
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<TargetOfMonthForMember>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var item = CH.GetDataById<TargetOfMonthForMember>(id);
            CH.Delete<TargetOfMonthForMember>(id);
            return RedirectToAction("MyTargetIndex", new { projectid = item.ProjectID });
        }

        public ActionResult ConfirmList(int? projectid)
        {
            //var list = CH.DB.ChangeTracker.Entries<TargetOfMonthForMember>().ToList();
            //projectid = this.TrySetProjectIDForUser(projectid);
            //ViewBag.ProjectID = projectid;
            //if (projectid != null)
            //{
            //    list = CH.DB.ChangeTracker.Entries<TargetOfMonthForMember>().ToList();
            //    var data = from t in CH.DB.TargetOfMonthForMembers.AsNoTracking()
            //               where t.ProjectID == projectid
            //               select t;

            //    var td = data.OrderByDescending(o => o.EndDate).ToList().Where(s => s.IsConfirm != true);

            //    list = CH.DB.ChangeTracker.Entries<TargetOfMonthForMember>().ToList();
            //    if (list.Count > 0)
            //    {
            //        var id = list.First().Entity.ID;
            //    }
            //    return View(td.ToList());
            //}
            return View();
        }

        public ActionResult Confirm(int id)
        {
            return View(CH.GetDataById<TargetOfMonthForMember>(id));
        }

        [HttpPost, ActionName("Confirm")]
        public ActionResult Confirmed(int id)
        {
            var item = CH.GetDataById<TargetOfMonthForMember>(id);
            item.IsConfirm = true;
            CH.Edit<TargetOfMonthForMember>(item);
            return RedirectToAction("ConfirmList", item.ProjectID);
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
            var item = CH.GetDataById<TargetOfMonthForMember>(id);
            item.IsConfirm = true;
            CH.Edit<TargetOfMonthForMember>(item);

            return View(new GridModel(getData()));
        }

        public List<AjaxTargetOfMonthForMember> getData()
        {
            return (from db in CH.DB.TargetOfMonthForMembers
                    orderby db.ID descending
                    select new AjaxTargetOfMonthForMember
                    {
                        ID = db.ID,
                        IsConfirm = db.IsConfirm == true ? "是" : "否",
                        SalesBriefName = db.Project.SalesBriefName,
                        Deal = db.Deal,
                        BaseDeal = db.BaseDeal,
                        CheckIn = db.CheckIn,
                        EndDate = db.EndDate,
                        MemberName = db.Member.Name,
                        StartDate = db.StartDate
                    }).ToList();
        }
    }
}