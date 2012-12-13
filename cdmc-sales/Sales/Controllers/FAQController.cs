using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Entity;
using Sales;
using Telerik.Web.Mvc;
using Utl;

namespace Sales.Controllers
{
    public class FAQController : Controller
    {
        protected override void Dispose(bool disposing)
        {
            CH.DB.Dispose();
            base.Dispose(disposing);
        }

        public ViewResult Index()
        {
            return View(CH.GetAllData<FAQ>());
        }


        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<FAQ>(id));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(FAQ item)
        {
            if (ModelState.IsValid)
            {
                CH.Create<FAQ>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<FAQ>(id));
        }

        [HttpPost]
        public ActionResult Edit(FAQ item)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<FAQ>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<FAQ>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<FAQ>(id);
            return RedirectToAction("Index");
        }
    }
}