//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using Entity;
//using Sales;
//using Utl;
//using Telerik.Web.Mvc;

//namespace Sales.Controllers
//{
//    [DirectorRequired]
//    public class AssignPerformanceRateController : Controller
//    {
//        protected override void Dispose(bool disposing)
//        {
//            CH.DB.Dispose();
//            base.Dispose(disposing);
//        }


//        public ActionResult index()
//        {
//            return View();

//        }
//        [GridAction]
//        public ActionResult _SelectIndex()
//        {
//            List<AssignPerformanceRate> list;
//            list = CH.GetAllData<AssignPerformanceRate>();
//            return View(new GridModel(list));
//        }
//        [AcceptVerbs(HttpVerbs.Post)]
//        [GridAction]
//        public ActionResult _SaveAjaxEditing(int id)
//        {
//            var item = CH.GetDataById<AssignPerformanceRate>(id);
//            if (TryUpdateModel(item))
//            {
//                CH.Edit<AssignPerformanceRate>(item);
//            }
//            return View(new GridModel(CH.GetAllData<AssignPerformanceRate>()));
//        }
//        [AcceptVerbs(HttpVerbs.Post)]

//        [GridAction]
//        public ActionResult _InsertAjaxEditing()
//        {
//            var item = new AssignPerformanceRate();

//            if (TryUpdateModel(item))
//            {
//                CH.Create<AssignPerformanceRate>(item);
//            }
//            //Rebind the grid       
//            return View(new GridModel(CH.GetAllData<AssignPerformanceRate>()));
//        }

//        [AcceptVerbs(HttpVerbs.Post)]
//        [GridAction]
//        public ActionResult _DeleteAjaxEditing(int id)
//        {
//            CH.Delete<AssignPerformanceRate>(id);
//            return View(new GridModel(CH.GetAllData<AssignPerformanceRate>()));
//        }

//    }
//}