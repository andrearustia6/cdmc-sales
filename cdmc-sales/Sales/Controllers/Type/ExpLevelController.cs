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
    [AdministratorRequired]
    public class ExpLevelController : Controller
    {

        public ViewResult Index()
        {
            return View(CH.GetAllData<ExpLevel>());
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<ExpLevel>(id));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(ExpLevel item)
        {
            if (ModelState.IsValid)
            {
                CH.Create<ExpLevel>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<ExpLevel>(id));
        }

        [HttpPost]
        public ActionResult Edit(ExpLevel item)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<ExpLevel>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<ExpLevel>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<ExpLevel>(id);
            return RedirectToAction("Index");
        }
    }
}