using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Entity;
using Utilities;
using Utl;
using Telerik.Web.Mvc;

namespace Sales.Controllers
{
    public class CompanyTypeController : Controller
    {
        protected override void Dispose(bool disposing)
        {
            CH.DB.Dispose();
            base.Dispose(disposing);
        }
        public ViewResult Index()
        {
            return View(CH.GetAllData<CompanyType>());
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<CompanyType>(id));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(CompanyType item)
        {
            if (ModelState.IsValid)
            {
                CH.Create<CompanyType>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<CompanyType>(id));
        }

        [HttpPost]
        public ActionResult Edit(CompanyType item)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<CompanyType>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<CompanyType>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<CompanyType>(id);
            return RedirectToAction("Index");
        }
    }
}