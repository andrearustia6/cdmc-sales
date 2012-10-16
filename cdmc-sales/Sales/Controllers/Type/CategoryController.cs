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
using Sales.Model;

namespace Sales.Controllers
{
    public class CategoryController : Controller
    {

        public ViewResult Index()
        {
            return View(CH.GetAllData<Category>());
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<Category>(id));
        }

        public ActionResult Create(int? projectid)
        {
            ViewBag.ProjectID = projectid;
            return View();
        }

        [HttpPost]
        public ActionResult Create(Category item)
        {

            this.AddErrorStateIfFieldExist<Category>(item, "Name");

            if (ModelState.IsValid)
            {
                CH.Create<Category>(item);
                return RedirectToAction("index", "Project");
            }
            ViewBag.ProjectID = item.ProjectID;
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<Category>(id));
        }

        [HttpPost]
        public ActionResult Edit(Category item)
        {
            this.AddErrorStateIfFieldExist<Category>(item, "Name");
            if (ModelState.IsValid)
            {
                CH.Edit<Category>(item);
                return RedirectToAction("index", "Project");
            }
            ViewBag.ProjectID = item.ProjectID;
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<Category>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<Category>(id);
            return RedirectToAction("index", "Project");
        }
    }
}