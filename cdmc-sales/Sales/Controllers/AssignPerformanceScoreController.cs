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
    public class AssignPerformanceScoreController : Controller
    {
        protected override void Dispose(bool disposing)
        {
            CH.DB.Dispose();
            base.Dispose(disposing);
        }

        public ViewResult Index()
        {
            return View(CH.GetAllData<AssignPerformanceScore>());
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<AssignPerformanceScore>(id));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(AssignPerformanceScore item)
        {
            if (ModelState.IsValid)
            {
                CH.Create<AssignPerformanceScore>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            var data = CH.GetDataById<AssignPerformanceScore>(id);
            return View(data);
        }

        [HttpPost]
        public ActionResult Edit(AssignPerformanceScore item)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<AssignPerformanceScore>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
           return View(CH.GetDataById<AssignPerformanceScore>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<AssignPerformanceScore>(id);
            return RedirectToAction("Index");
        }
    }
}