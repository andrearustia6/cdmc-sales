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
    public class ClientTypeController : Controller
    {
        public ViewResult Index()
        {
            return View();
        }
        //[GridAction]
        //public ActionResult AjaxClientIndex()
        //{
        //    var data = CH.GetAllData<Client>();
        //    return View(new GridModel(data));
        //}
        [GridAction()]
        public ActionResult AjaxClientTypeIndex()
        {
            var data = CH.GetAllData<LeadtType>();
            return new DataJsonResult<LeadtType>() { Data = data };
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<LeadtType>(id));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(LeadtType item)
        {
            if (ModelState.IsValid)
            {
                CH.Create<LeadtType>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<LeadtType>(id));
        }

        [HttpPost]
        public ActionResult Edit(LeadtType item)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<LeadtType>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<LeadtType>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<LeadtType>(id);
            return RedirectToAction("Index");
        }
    }
}