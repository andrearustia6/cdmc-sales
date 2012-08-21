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
    public class LeadTypeController : Controller
    {
        public ViewResult Index()
        {
            return View(CH.GetAllData<LeadType>());
        }



        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<LeadType>(id));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(LeadType item)
        {
            if (ModelState.IsValid)
            {
                CH.Create<LeadType>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<LeadType>(id));
        }

        [HttpPost]
        public ActionResult Edit(LeadType item)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<LeadType>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<LeadType>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<LeadType>(id);
            return RedirectToAction("Index");
        }
    }
}