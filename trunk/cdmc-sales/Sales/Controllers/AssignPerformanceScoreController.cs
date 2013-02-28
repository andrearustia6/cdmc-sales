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
            List<AssignPerformanceScore> list;
            if (Employee.CurrentRole.Level < 1000)
                list = CH.GetAllData<AssignPerformanceScore>(a => a.Assigner == Employee.CurrentUserName);
            else
                list = CH.GetAllData<AssignPerformanceScore>();
            return View(new GridModel(list));
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
            List<AssignPerformanceScore> list;
            if (Employee.CurrentRole.Level < 1000)
                list = CH.GetAllData<AssignPerformanceScore>(a => a.Assigner == Employee.CurrentUserName);
            else
                list = CH.GetAllData<AssignPerformanceScore>();
            return View(new GridModel(list));
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
            List<AssignPerformanceScore> list;
            if (Employee.CurrentRole.Level < 1000)
                list = CH.GetAllData<AssignPerformanceScore>(a => a.Assigner == Employee.CurrentUserName);
            else
                list = CH.GetAllData<AssignPerformanceScore>();
            return View(new GridModel(list));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _DeleteAjaxEditing(int id)
        {
            CH.Delete<AssignPerformanceScore>(id);
            List<AssignPerformanceScore> list;
            if (Employee.CurrentRole.Level < 1000)
                list = CH.GetAllData<AssignPerformanceScore>(a => a.Assigner == Employee.CurrentUserName);
            else
                list = CH.GetAllData<AssignPerformanceScore>();
            return View(new GridModel(list));
        }

    }
}