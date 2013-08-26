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
//    public class AssignPerformanceScoreController : Controller
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
//            List<AssignPerformanceScore> list;
//            if (Employee.CurrentRole.Level < 1000)
//                list = CH.GetAllData<AssignPerformanceScore>(a => a.Assigner == Employee.CurrentUserName);
//            else
//                list = CH.GetAllData<AssignPerformanceScore>();
//            return View(new GridModel(list));
//        }
//        [AcceptVerbs(HttpVerbs.Post)]
//        [GridAction]
//        public ActionResult _SaveAjaxEditing(int id)
//        {
//            var item = CH.GetDataById<AssignPerformanceScore>(id);
//            if (TryUpdateModel(item))
//            {
//                var data = CH.GetAllData<AssignPerformanceScore>(
//                          aps => aps.TargetName == item.TargetName
//                                 && aps.ID != item.ID
//                                 && aps.Year == item.Year
//                                 && aps.Month == item.Month);
//                if (data == null || data.Count() == 0)
//                {
//                    if (ModelState.IsValid)
//                    {
//                        CH.Edit<AssignPerformanceScore>(item);
//                    }
//                }
//                else
//                {
//                    ModelState.AddModelError("TargetName", "您选择的被考核人当月已经有被打分的记录.");
//                }
//            }
//            List<AssignPerformanceScore> list;
//            if (Employee.CurrentRole.Level < 1000)
//                list = CH.GetAllData<AssignPerformanceScore>(a => a.Assigner == Employee.CurrentUserName);
//            else
//                list = CH.GetAllData<AssignPerformanceScore>();
//            return View(new GridModel(list));
//        }
//        [AcceptVerbs(HttpVerbs.Post)]

//        [GridAction]
//        public ActionResult _InsertAjaxEditing()
//        {
//            var item = new AssignPerformanceScore();
           
//            if (TryUpdateModel(item))
//            {
//                var data = CH.GetAllData<AssignPerformanceScore>(
//                           aps => aps.TargetName == item.TargetName 
//                                  && aps.Year == item.Year
//                                  && aps.Month == item.Month);
//                if (data == null || data.Count() == 0)
//                {
//                    if (ModelState.IsValid)
//                    {
//                        CH.Create<AssignPerformanceScore>(item);
//                    }
//                }
//                else
//                {
//                    ModelState.AddModelError("TargetName", "您选择的被考核人当月已经有被打分的记录.");
//                }
//            }
//            //Rebind the grid       
//            List<AssignPerformanceScore> list;
//            if (Employee.CurrentRole.Level < 1000)
//                list = CH.GetAllData<AssignPerformanceScore>(a => a.Assigner == Employee.CurrentUserName);
//            else
//                list = CH.GetAllData<AssignPerformanceScore>();
//            return View(new GridModel(list));
//        }

//        [AcceptVerbs(HttpVerbs.Post)]
//        [GridAction]
//        public ActionResult _DeleteAjaxEditing(int id)
//        {
//            CH.Delete<AssignPerformanceScore>(id);
//            List<AssignPerformanceScore> list;
//            if (Employee.CurrentRole.Level < 1000)
//                list = CH.GetAllData<AssignPerformanceScore>(a => a.Assigner == Employee.CurrentUserName);
//            else
//                list = CH.GetAllData<AssignPerformanceScore>();
//            return View(new GridModel(list));
//        }
       

//    }
//}