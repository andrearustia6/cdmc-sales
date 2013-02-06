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

namespace Sales.Controllers.Type
{
    public class AccessRightController : Controller
    {
        protected override void Dispose(bool disposing)
        {
            CH.DB.Dispose();
            base.Dispose(disposing);
        }

        public ViewResult Index()
        {
            return View(CH.GetAllData<AccessRight>());
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<AccessRight>(id));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(AccessRight item)
        {
            if (ModelState.IsValid)
            {
                CH.Create<AccessRight>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            var data = CH.GetDataById<AccessRight>(id);
            var list = CH.DB.ChangeTracker.Entries<AccessRight>().ToList();
            return View(data);
        }

        [HttpPost]
        public ActionResult Edit(AccessRight item)
        {
            var list = CH.DB.ChangeTracker.Entries<AccessRight>().ToList();
            if (ModelState.IsValid)
            {
                CH.Edit<AccessRight>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            var count = from c in CH.DB.ProjectRights
                        where c.AccessRights.Any(a=>a.ID == id)
                        select c;

            if (count.Count() > 0)
                return View(@"~\views\shared\Error.cshtml", null, SR.CannotDelete);
            else
                return View(CH.GetDataById<AccessRight>(id));


        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<AccessRight>(id);
            return RedirectToAction("Index");
        }
    }
}