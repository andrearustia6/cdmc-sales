﻿using System;
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
        protected override void Dispose(bool disposing)
        {
            CH.DB.Dispose();
            base.Dispose(disposing);
        }

        public ViewResult Index()
        {
            return View(CH.GetAllData<PackageItem>());
        }
          
        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<PackageItem>(id));
        }
           [DirectorRequired]
        public ActionResult Create(int? packageid,string from)
        {
            ViewBag.PackageID = packageid;
            if (from == "package")
            {
                ViewBag.From = "package";
            }
            return View();
        }
           [DirectorRequired]
        [HttpPost]
        public ActionResult Create(PackageItem item)
        {
            if (ModelState.IsValid)
            {
                CH.Create<PackageItem>(item);
                return RedirectToAction("Index", "Package");
            }
            ViewBag.PackageID = item.PackageID;
            return View(item);
        }
           [DirectorRequired]
        public ActionResult Edit(int id)
        {
            var data = CH.GetDataById<PackageItem>(id);
            return View(data);
        }

        [HttpPost]
        [DirectorRequired]
        public ActionResult Edit(PackageItem item)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<PackageItem>(item);
                return RedirectToAction("Index", "Package");
            }
            ViewBag.PackageID = item.PackageID;
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
            return RedirectToAction("Index","Package");
        }
    }
}