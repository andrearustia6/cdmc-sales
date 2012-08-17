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
    public class AreaController : Controller
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
            var data = CH.GetAllData<Area>();
            return new DataJsonResult<Area>() { Data = data };
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<Area>(id));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Area item)
        {
            if (ModelState.IsValid)
            {
                CH.Create<Area>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<Area>(id));
        }

        [HttpPost]
        public ActionResult Edit(Area item)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<Area>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<Area>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<Area>(id);
            return RedirectToAction("Index");
        }
    }
}