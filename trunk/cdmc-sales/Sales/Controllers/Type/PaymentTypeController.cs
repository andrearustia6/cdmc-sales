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
    public class PaymentTypeController : Controller
    {
        protected override void Dispose(bool disposing)
        {
            CH.DB.Dispose();
            base.Dispose(disposing);
        }

        public ViewResult Index()
        {
            return View(CH.GetAllData<PaymentType>());
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<PaymentType>(id));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(PaymentType item)
        {
            if (ModelState.IsValid)
            {
                CH.Create<PaymentType>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<PaymentType>(id));
        }

        [HttpPost]
        public ActionResult Edit(PaymentType item)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<PaymentType>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<PaymentType>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<PaymentType>(id);
            return RedirectToAction("Index");
        }
    }
}