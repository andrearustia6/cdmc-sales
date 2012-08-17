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
    public class ResearchController : Controller
    {
        public ViewResult Index()
        {
            return View();
        }
        //[GridAction]
        //public ActionResult AjaxResearchIndex()
        //{
        //    var data = CH.GetAllData<Research>();
        //    return View(new GridModel(data));
        //}
        [GridAction()]
        public ActionResult AjaxResearchIndex()
        {
            var data = CH.GetAllData<Research>();
            return new DataJsonResult<Research>() { Data = data };
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<Research>(id));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Research item)
        {
            if (ModelState.IsValid)
            {
                CH.Create<Research>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<Research>(id));
        }

        [HttpPost]
        public ActionResult Edit(Research item)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<Research>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<Research>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<Research>(id);
            return RedirectToAction("Index");
        }
    }
}