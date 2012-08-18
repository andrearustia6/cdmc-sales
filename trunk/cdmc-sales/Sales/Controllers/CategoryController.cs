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
    public class CategoryController : Controller
    {
        public ViewResult Index()
        {
            return View();
        }
        //[GridAction]
        //public ActionResult AjaxAreaIndex()
        //{
        //    var data = CH.GetAllData<Area>();
        //    return View(new GridModel(data));
        //}
        [GridAction()]
        public ActionResult AjaxAreaIndex()
        {
            var data = CH.GetAllData<Category>();
            return new DataJsonResult<Category>() { Data = data };
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<Category>(id));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Category item)
        {
            if (ModelState.IsValid)
            {
                CH.Create<Category>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<Category>(id));
        }

        [HttpPost]
        public ActionResult Edit(Category item)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<Category>(item);
                return RedirectToAction("Index");
            }
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
            return RedirectToAction("Index");
        }
    }
}