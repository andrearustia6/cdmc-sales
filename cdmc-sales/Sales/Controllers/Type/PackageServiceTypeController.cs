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
    public class PackageServiceTypeController : Controller
    {
        public ViewResult Index()
        {
            return View(CH.GetAllData<PackageServiceType>());
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<PackageServiceType>(id));
        }
        //
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(PackageServiceType item)
        {
            if (ModelState.IsValid)
            {
                CH.Create<PackageServiceType>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<PackageServiceType>(id));
        }

        [HttpPost]
        public ActionResult Edit(PackageServiceType item)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<PackageServiceType>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<PackageServiceType>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<PackageServiceType>(id);
            return RedirectToAction("Index");
        }
    }
}