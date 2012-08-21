using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Entity;
using Sales;

namespace Sales.Controllers.Type
{ 
    public class PaymentTypeController : Controller
    {
        private DB db = new DB();

        //
        // GET: /PaymentType/

        public ViewResult Index()
        {
            return View(db.PaymentType.ToList());
        }

        //
        // GET: /PaymentType/Details/5

        public ViewResult Details(int id)
        {
            PaymentType paymenttype = db.PaymentType.Find(id);
            return View(paymenttype);
        }

        //
        // GET: /PaymentType/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /PaymentType/Create

        [HttpPost]
        public ActionResult Create(PaymentType paymenttype)
        {
            if (ModelState.IsValid)
            {
                db.PaymentType.Add(paymenttype);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(paymenttype);
        }
        
        //
        // GET: /PaymentType/Edit/5
 
        public ActionResult Edit(int id)
        {
            PaymentType paymenttype = db.PaymentType.Find(id);
            return View(paymenttype);
        }

        //
        // POST: /PaymentType/Edit/5

        [HttpPost]
        public ActionResult Edit(PaymentType paymenttype)
        {
            if (ModelState.IsValid)
            {
                db.Entry(paymenttype).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(paymenttype);
        }

        //
        // GET: /PaymentType/Delete/5
 
        public ActionResult Delete(int id)
        {
            PaymentType paymenttype = db.PaymentType.Find(id);
            return View(paymenttype);
        }

        //
        // POST: /PaymentType/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            PaymentType paymenttype = db.PaymentType.Find(id);
            db.PaymentType.Remove(paymenttype);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}