using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using Utl;
using BLL;
using Entity;
using Sales.Model;
namespace Sales.Controllers
{
    public class PerformanceController : Controller
    {
        public ActionResult Index(int? month)
        {
            ViewBag.Month = month;
            PopulateItems();
            return View();
        }
        [GridAction]
        public ActionResult _SelectManagerIndex(int? month)
        {
            if (month == null) month = DateTime.Now.Month;
            // if (month == null) month = 5;
            PopulateItems();
            var list = CRM_Logical._EmployeePerformance.GetManagerLeadsPerformances(month.Value);
            var data = list.ToList();


            return View(new GridModel(data));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _UpdateManagerScore(int id, int? month)
        {
            if (month == null) month = DateTime.Now.Month;
            _ManagerScore model = new _ManagerScore();
            ManagerScore newmodel = new ManagerScore();
            if (id > 0)
            {

                if (TryUpdateModel(model))
                {
                    newmodel.ID = id;
                    newmodel.TargetName = model.TargetName;
                    newmodel.Assigner = model.Assigner;
                    newmodel.Item1Score = model.Item1Score;
                    newmodel.Item2Score = model.Item2Score;
                    newmodel.Item3Score = model.Item3Score;
                    newmodel.Item4Score = model.Item4Score;
                    newmodel.Item5Score = model.Item5Score;
                    newmodel.Item6Score = model.Item6Score;
                    newmodel.Item7Score = model.Item7Score;
                    newmodel.Item8Score = model.Item8Score;
                    newmodel.Item9Score = model.Item9Score;
                    
                    newmodel.Month = month;
                    newmodel.Year = DateTime.Now.Year;
                    
                    CH.Edit<ManagerScore>(newmodel);
                }
                //var model = CH.GetDataById<ManagerScore>(id);
                //model.Item1Score = Item1ScoreString;
                //CH.Edit<ManagerScore>(model);
            }
            else
            {
                
                if (TryUpdateModel(model))
                {
                    //ManagerScore newmodel = new ManagerScore();
                    //newmodel.Item1Score = clientmodel.Item1Score;
                    //newmodel.ID = model.ID;
                    newmodel.TargetName = model.TargetName;
                    newmodel.Assigner = model.Assigner;
                    newmodel.Item1Score = model.Item1Score;
                    newmodel.Item2Score = model.Item2Score;
                    newmodel.Item3Score = model.Item3Score;
                    newmodel.Item4Score = model.Item4Score;
                    newmodel.Item5Score = model.Item5Score;
                    newmodel.Item6Score = model.Item6Score;
                    newmodel.Item7Score = model.Item7Score;
                    newmodel.Item8Score = model.Item8Score;
                    newmodel.Item9Score = model.Item9Score;
                    
                    newmodel.Month = month;
                    newmodel.Year = DateTime.Now.Year;
                    CH.Create<ManagerScore>(newmodel);
                }
            }

            PopulateItems();
            var list = CRM_Logical._EmployeePerformance.GetManagerLeadsPerformances(month.Value);
            var data = list.ToList();


            return View(new GridModel(data));

        }
        private void PopulateItems()
        {
            ViewData["Item1s"] = from m in CRM_Logical.GetItem1()
                                 select m;
            ViewData["Item2s"] = from m in CRM_Logical.GetItem2()
                                 select m;

            ViewData["Item3s"] = from m in CRM_Logical.GetItem3()
                                 select m;
            ViewData["Item4s"] = from m in CRM_Logical.GetItem4()
                                 select m;
            ViewData["Item5s"] = from m in CRM_Logical.GetItem5()
                                 select m;
            ViewData["Item6s"] = from m in CRM_Logical.GetItem6()
                                 select m;
            ViewData["Item7s"] = from m in CRM_Logical.GetItem7()
                                 select m;
            ViewData["Item8s"] = from m in CRM_Logical.GetItem8()
                                 select m;
            ViewData["Item9s"] = from m in CRM_Logical.GetItem9()
                                 select m;
        }
        [GridAction]
        public ActionResult _SelectLeadIndex(int? month)
        {
            if (month == null) month = DateTime.Now.Month;
           // if (month == null) month = 5;
            var list = CRM_Logical._EmployeePerformance.GetTeamLeadsPerformances(month.Value);
            var data = list.ToList();
            return View(new GridModel(data));
        }

        [GridAction]
        public ActionResult _SelectSalesIndex(int? month)
        {
           if (month == null) month = DateTime.Now.Month;
           // if (month == null) month = 5;
            var list = CRM_Logical._EmployeePerformance.GetSalesPerformances(month.Value);
            var data = list.ToList();
            return View(new GridModel(data));
        }

       

    }
}
