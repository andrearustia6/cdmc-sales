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

namespace Sales.Controllers
{
    public class SubCompanyController : Controller
    {
        protected override void Dispose(bool disposing)
        {
            CH.DB.Dispose();
            base.Dispose(disposing);
        }

        public ViewResult Index()
        {
            return View(CH.GetAllData<SubCompany>());
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<SubCompany>(id));
        }

        public ActionResult Create(int? projectid)
        {
            ViewBag.projectID = projectid;
            return View();
        }

        [HttpPost]
        public ActionResult Create(SubCompany item, int? projectid)
        {
            if (ModelState.IsValid)
            {
                CH.Create<SubCompany>(item);
                return RedirectToAction("Management", "Project", new { id = projectid, tabindex = 3 });
            }
            return View(item);
        }
        public ActionResult Edit(int id, int? projectid)
        {
            ViewBag.projectID = projectid;
            return View(CH.GetDataById<SubCompany>(id));
        }

        [HttpPost]
        public ActionResult Edit(SubCompany item, int? projectid)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<SubCompany>(item);
                return RedirectToAction("Management", "Project", new { id = projectid, tabindex = 3 });
            }
            return View(item);
        }

        public ActionResult Delete(int id, int? projectid)
        {
            var count = from c in CH.DB.Leads
                        where c.SubCompanyID == id
                        select c;
            ViewBag.projectID = projectid;
            if (count.Count() > 0)
                return View(@"~\views\shared\Error.cshtml", null, SR.CannotDelete);
            else
                return View(CH.GetDataById<SubCompany>(id));


        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id, int? projectid)
        {
            CH.Delete<SubCompany>(id);
            return RedirectToAction("Management", "Project", new { id = projectid, tabindex = 3 });
        }
    }
}