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
        [LeaderRequired]
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
                var ps = BLL.CRM_Logical.GetUserInvolveProject();
                var rs = CH.GetAllData<TargetOfMonthForMember>(r => ps.Any(sp => sp.ID == r.ProjectID) && r.CreatedDate >= startdate && r.CreatedDate <= enddate);
                return View(rs);
            }

        }



        public ViewResult MyTargetIndex(int? projectid)
        {
            projectid = this.TrySetProjectIDForUser(projectid);
            ViewBag.ProjectID = projectid;
            if (projectid != null)
            {
                var data = from t in CH.DB.TargetOfMonthForMembers
                           where t.ProjectID == projectid && t.Member.Name == Employee.CurrentUserName
                           select t;
                return View(data.OrderByDescending(o => o.EndDate).ToList());
            }
            return View();

        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<TargetOfMonthForMember>(id));
        }

        public ActionResult Create()
        {
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
            return View(data);
        }

        [HttpPost]
        public ActionResult Edit(TargetOfMonthForMember item)
        {
            //this.AddErrorStateIfTargetOfMonthNoValid(item);
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
    }
}