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
    public class PackageItemController : Controller
    {
        public ViewResult Index()
        {
            return View(CH.GetAllData<PackageItem>());
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<PackageItem>(id));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(PackageItem item)
        {
            if (ModelState.IsValid)
            {
                CH.Create<PackageItem>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            var data = CH.GetDataById<PackageItem>(id);
            return View(data);
        }

        [HttpPost]
        public ActionResult Edit(PackageItem item)
        {

            if (ModelState.IsValid)
            {

                CH.Edit<PackageItem>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<PackageItem>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<PackageItem>(id);
            return RedirectToAction("Index");
        }
    }
}