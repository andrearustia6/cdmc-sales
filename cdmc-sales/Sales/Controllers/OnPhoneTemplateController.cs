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
    public class OnPhoneTemplateController : Controller
    {
        public ViewResult Index()
        {
            return View(CH.GetAllData<OnPhoneTemplate>());
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<OnPhoneTemplate>(id));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(OnPhoneTemplate item)
        {
            if (ModelState.IsValid)
            {
                CH.Create<OnPhoneTemplate>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<OnPhoneTemplate>(id));
        }

        [HttpPost]
        public ActionResult Edit(OnPhoneTemplate item)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<OnPhoneTemplate>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<OnPhoneTemplate>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<OnPhoneTemplate>(id);
            return RedirectToAction("Index");
        }
    }
}