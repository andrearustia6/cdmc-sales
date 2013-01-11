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
    public class DistrictNumberController : Controller
    {
        protected override void Dispose(bool disposing)
        {
            CH.DB.Dispose();
            base.Dispose(disposing);
        }

        public ViewResult Index()
        {
            return View(CH.GetAllData<DistrictNumber>().OrderBy(o=>o.Country).ToList());
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<DistrictNumber>(id));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(DistrictNumber item)
        {
            if (ModelState.IsValid)
            {
                CH.Create<DistrictNumber>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<DistrictNumber>(id));
        }

        [HttpPost]
        public ActionResult Edit(DistrictNumber item)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<DistrictNumber>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<DistrictNumber>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<DistrictNumber>(id);
            return RedirectToAction("Index");
        }
    }
}