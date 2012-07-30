using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Entity;
using Utilities;
using Utl;
using Telerik.Web.Mvc;

namespace Sales.Controllers
{
    public class CompanyController : Controller
    {
        public ViewResult Index()
        {
            return View(CH.GetAllData<Company>());
        }
        [GridAction]
        public ActionResult AjaxCompanyIndex()
        {
            return View(new GridModel(CH.GetAllData<Company>()));
        }

        //public ActionResult AjaxCompanyIndex()
        //{
        //    return new DataJsonResult<Company>() { Data = CH.GetAllData<Company>() };
        //}

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<Company>(id));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Company item)
        {
            if (ModelState.IsValid)
            {
                CH.Create<Company>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<Company>(id));
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Company item)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<Company>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<Company>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<Company>(id);
            return RedirectToAction("Index");
        }
    }
}