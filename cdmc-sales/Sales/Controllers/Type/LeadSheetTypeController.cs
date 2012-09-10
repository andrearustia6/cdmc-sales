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
    public class LeadSheetTypeController : Controller
    {
        public ViewResult Index()
        {
            return View(CH.GetAllData<LeadCallType>());
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<LeadCallType>(id));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(LeadCallType item)
        {
            if (ModelState.IsValid)
            {
                CH.Create<LeadCallType>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<LeadCallType>(id));
        }

        [HttpPost]
        public ActionResult Edit(LeadCallType item)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<LeadCallType>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<LeadCallType>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<LeadCallType>(id);
            return RedirectToAction("Index");
        }
    }
}