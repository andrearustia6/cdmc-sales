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
    public class Target_MController : Controller
    {

        public ViewResult Index()
        {
            return View(CH.GetAllData<Target_M>());
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<Target_M>(id));
        }

        public ActionResult Create(int? projectid)
        {
            ViewBag.ProjectID = projectid;
            return View();
        }

        [HttpPost]
        public ActionResult Create(Target_M item)
        {
            if (ModelState.IsValid)
            {
                CH.Create<Target_M>(item);
                return RedirectToAction("Management", "Project", new { id = item.ProjectID });
            }
            ViewBag.ProjectID = item.ProjectID;
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<Target_M>(id));
        }

        [HttpPost]
        public ActionResult Edit(Target_M item)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<Target_M>(item);
                return RedirectToAction("Management", "Project", new { id = item.ProjectID });
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<Target_M>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var item = CH.GetDataById<Target_M>(id);
            var pid = item.ProjectID;
            CH.Delete<Target_M>(id);

            return RedirectToAction("Management", "Project", new { id = pid });
        }
    }
}