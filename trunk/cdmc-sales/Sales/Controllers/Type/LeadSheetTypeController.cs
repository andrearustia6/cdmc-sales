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
            return View(CH.GetAllData<LeadCallSheetType>());
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<LeadCallSheetType>(id));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(LeadCallSheetType item)
        {
            if (ModelState.IsValid)
            {
                CH.Create<LeadCallSheetType>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<LeadCallSheetType>(id));
        }

        [HttpPost]
        public ActionResult Edit(LeadCallSheetType item)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<LeadCallSheetType>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<LeadCallSheetType>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<LeadCallSheetType>(id);
            return RedirectToAction("Index");
        }
    }
}