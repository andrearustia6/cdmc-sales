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
    public class TemplateTypeController : Controller
    {
        public ViewResult Index()
        {
            return View(CH.GetAllData<TemplateType>());
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<TemplateType>(id));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(TemplateType item)
        {
            if (ModelState.IsValid)
            {
                CH.Create<TemplateType>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<TemplateType>(id));
        }

        [HttpPost]
        public ActionResult Edit(TemplateType item)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<TemplateType>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<TemplateType>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<TemplateType>(id);
            return RedirectToAction("Index");
        }
    }
}