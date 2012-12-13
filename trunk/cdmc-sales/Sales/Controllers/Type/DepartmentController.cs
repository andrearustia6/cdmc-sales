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
    [AdministratorRequired]
    public class DepartmentController : Controller
    {
        protected override void Dispose(bool disposing)
        {
            CH.DB.Dispose();
            base.Dispose(disposing);
        }
        public ViewResult Index()
        {
            return View(CH.GetAllData<Department>());
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<Department>(id));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Department item)
        {
            if (ModelState.IsValid)
            {
                CH.Create<Department>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<Department>(id));
        }

        [HttpPost]
        public ActionResult Edit(Department item)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<Department>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<Department>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<Department>(id);
            return RedirectToAction("Index");
        }
    }
}