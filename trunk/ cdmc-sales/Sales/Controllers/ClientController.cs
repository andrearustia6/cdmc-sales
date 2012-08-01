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
using Telerik.Web.Mvc;

namespace Sales.Controllers
{
    public class ClientController : Controller
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
        public ActionResult AjaxClientIndex()
        {
            var data = CH.GetAllData<Client>();
            return new DataJsonResult<Client>() { Data = data };
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<Client>(id));
        }

        public ActionResult Create(int? companyid)
        {
            ViewBag.CompanyID = companyid;
            return View();
        }

        [HttpPost]
        public ActionResult Create(Client item)
        {
            if (ModelState.IsValid)
            {
                CH.Create<Client>(item);
                return RedirectToAction("Index");
            }
            ViewBag.CompanyID = item.CompanyID;
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<Client>(id));
        }

        [HttpPost]
        public ActionResult Edit(Client item)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<Client>(item);
                return RedirectToAction("Index");
            }
            ViewBag.CompanyID = item.CompanyID;
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<Client>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<Client>(id);
            return RedirectToAction("Index");
        }
    }
}