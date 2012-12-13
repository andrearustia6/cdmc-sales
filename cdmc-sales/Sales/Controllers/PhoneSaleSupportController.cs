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
    public class PhoneSaleSupportController : Controller
    {
        protected override void Dispose(bool disposing)
        {
            CH.DB.Dispose();
            base.Dispose(disposing);
        }
        public ViewResult Index()
        {
            return View(CH.GetAllData<PhoneSaleSupport>());
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<PhoneSaleSupport>(id));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(PhoneSaleSupport item)
        {
            if (ModelState.IsValid)
            {
                CH.Create<PhoneSaleSupport>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<PhoneSaleSupport>(id));
        }

        [HttpPost]
        public ActionResult Edit(PhoneSaleSupport item)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<PhoneSaleSupport>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<PhoneSaleSupport>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<PhoneSaleSupport>(id);
            return RedirectToAction("Index");
        }
    }
}