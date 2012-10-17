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
    public class TargetOfMonthController : Controller
    {

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
            this.AddErrorStateIfStartDateLaterThanEndDate(item.StartDate, item.EndDate);
            if (ModelState.IsValid)
            {
                CH.Create<TargetOfMonth>(item);
                return RedirectToAction("Management", "Project", new { id = item.ProjectID,tabindex=2 });
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
            this.AddErrorStateIfStartDateLaterThanEndDate(item.StartDate, item.EndDate);
            if (ModelState.IsValid)
            {
                CH.Edit<TargetOfMonth>(item);
                return RedirectToAction("Management", "Project", new { id = item.ProjectID });
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

            return RedirectToAction("Management", "Project", new { id = pid });
        }
    }
}