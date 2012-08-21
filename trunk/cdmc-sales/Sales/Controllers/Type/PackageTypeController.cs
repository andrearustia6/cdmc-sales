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
    public class PackageTypeController : Controller
    {
        public ViewResult Index()
        {
            return View(CH.GetAllData<PackageType>());
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<PackageType>(id));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(PackageType item)
        {
            if (ModelState.IsValid)
            {
                CH.Create<PackageType>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<PackageType>(id));
        }

        [HttpPost]
        public ActionResult Edit(PackageType item)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<PackageType>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<PackageType>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<PackageType>(id);
            return RedirectToAction("Index");
        }
    }
}