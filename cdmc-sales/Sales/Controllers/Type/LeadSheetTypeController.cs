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
            return View(CH.GetAllData<LeadSheetType>());
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<LeadSheetType>(id));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(LeadSheetType item)
        {
            if (ModelState.IsValid)
            {
                CH.Create<LeadSheetType>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<LeadSheetType>(id));
        }

        [HttpPost]
        public ActionResult Edit(LeadSheetType item)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<LeadSheetType>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<LeadSheetType>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<LeadSheetType>(id);
            return RedirectToAction("Index");
        }
    }
}