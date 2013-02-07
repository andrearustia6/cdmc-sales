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
        protected override void Dispose(bool disposing)
        {
            CH.DB.Dispose();
            base.Dispose(disposing);
        }

        public ViewResult Index()
        {
            return View(CH.GetAllData<Area>());
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
            var data = CH.GetDataById<Area>(id);
            var list = CH.DB.ChangeTracker.Entries<Area>().ToList();
            return View(data);
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
            var count = from c in CH.DB.Companys
                        where c.AreaID == id
                        select c;

            if (count.Count() > 0)
                return View(@"~\views\shared\Error.cshtml", null, SR.CannotDelete);
            else
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