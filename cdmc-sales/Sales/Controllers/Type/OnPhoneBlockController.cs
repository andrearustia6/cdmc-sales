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

namespace Sales.Controllers.Type
{
    public class OnPhoneBlockTypeController : Controller
    {
        public ViewResult Index()
        {
            return View(CH.GetAllData<OnPhoneBlockType>());
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<OnPhoneBlockType>(id));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(OnPhoneBlockType item)
        {
            if (ModelState.IsValid)
            {
                CH.Create<OnPhoneBlockType>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<OnPhoneBlockType>(id));
        }

        [HttpPost]
        public ActionResult Edit(OnPhoneBlockType item)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<OnPhoneBlockType>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<OnPhoneBlockType>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<OnPhoneBlockType>(id);
            return RedirectToAction("Index");
        }
    }
}