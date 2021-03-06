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
   
    
    public class PackageController : Controller
    {
        protected override void Dispose(bool disposing)
        {
            CH.DB.Dispose();
            base.Dispose(disposing);
        }

        public ViewResult Index()
        {
             
            var rate = CH.GetAllData<CurrencyType>(c=>c.Name=="RMB").FirstOrDefault();
            if (rate != null)
                ViewBag.Rate = rate.Rate;
            else
                ViewBag.Rate = 1;

            var ps = from p in CH.DB.Packages.OrderByDescending(o => o.Sequence)
                     select p;
            //foreach (var p in ps)
            //{
            //    CH.DB.Entry(p).Collection(x => x.PackageItems).Load();
            //}
            var list = ps.ToList();
            if(Employee.AsManager())
                return View(list);
            else
                return View(@"~\views\package\indexview.cshtml", list);



            //return View(CH.GetAllData<Package>("PackageItems").AsParallel().OrderByDescending(o => o.Sequence).ToList());
        }
        
        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<Package>(id));
        }
         [DirectorRequired]
        public ActionResult Create()
        {
            return View();
        }
         [DirectorRequired]
        [HttpPost]
        public ActionResult Create(Package item)
        {
            if (ModelState.IsValid)
            {
                CH.Create<Package>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }
         [DirectorRequired]
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<Package>(id));
        }
         [DirectorRequired]
        [HttpPost]
        public ActionResult Edit(Package item)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<Package>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }
         [DirectorRequired]
        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<Package>(id));
        }

         [DirectorRequired]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            
            CH.Delete<Package>(id);
            return RedirectToAction("Index");
        }

         public ActionResult GetParticipantTypeName(int id)
         {
             return Json(CH.GetDataById<Package>(id).ParticipantType.Name_EN);
             
         }
    }
}