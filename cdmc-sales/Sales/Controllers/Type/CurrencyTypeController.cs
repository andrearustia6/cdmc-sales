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
    public class CurrencyTypeController : Controller
    {
        protected override void Dispose(bool disposing)
        {
            CH.DB.Dispose();
            base.Dispose(disposing);
        }
        public ViewResult Index()
        {
            return View(CH.GetAllData<CurrencyType>());
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<CurrencyType>(id));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(CurrencyType item)
        {
            if (ModelState.IsValid)
            {
                CH.Create<CurrencyType>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<CurrencyType>(id));
        }

        [HttpPost]
        public ActionResult Edit(CurrencyType item)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<CurrencyType>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            //var count = from c in CH.DB.Packages
            //            where c.pc == id
            //            select c;

            //if (count.Count() > 0)
            //    return View(@"~\views\shared\Error.cshtml", null, SR.CannotDelete);
            //else
                return View(CH.GetDataById<CurrencyType>(id));
    
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<CurrencyType>(id);
            return RedirectToAction("Index");
        }
    }
}