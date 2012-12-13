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
    public class SalesTypeController : Controller
    {
        protected override void Dispose(bool disposing)
        {
            CH.DB.Dispose();
            base.Dispose(disposing);
        }
        public ViewResult Index()
        {
            return View(CH.GetAllData<SalesType>());
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<SalesType>(id));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(SalesType item)
        {
            if (ModelState.IsValid)
            {
                CH.Create<SalesType>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<SalesType>(id));
        }

        [HttpPost]
        public ActionResult Edit(SalesType item)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<SalesType>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<SalesType>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<SalesType>(id);
            return RedirectToAction("Index");
        }
    }
}