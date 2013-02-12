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
    public class AssignPerformanceScoreController : Controller
    {
        protected override void Dispose(bool disposing)
        {
            CH.DB.Dispose();
            base.Dispose(disposing);
        }


        public ActionResult index()
        {
            return View();

        }
        [GridAction]
        public ActionResult _SelectIndex()
        {
            return View(new GridModel(CH.GetAllData<AssignPerformanceScore>()));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveAjaxEditing(int id)
        {
            var item = CH.GetDataById<AssignPerformanceScore>(id);
            if (TryUpdateModel(item))
            {
                CH.Edit<AssignPerformanceScore>(item);
            }
            return View(new GridModel(CH.GetAllData<AssignPerformanceScore>()));
        }
        [AcceptVerbs(HttpVerbs.Post)]

        [GridAction]
        public ActionResult _InsertAjaxEditing()
        {
            var item = new AssignPerformanceScore();

            if (TryUpdateModel(item))
            {
                CH.Create<AssignPerformanceScore>(item);
            }
            //Rebind the grid       
            return View(new GridModel(CH.GetAllData<AssignPerformanceScore>()));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _DeleteAjaxEditing(int id)
        {
            CH.Delete<AssignPerformanceScore>(id);
            return View(new GridModel(CH.GetAllData<AssignPerformanceScore>()));
        }

    }
}