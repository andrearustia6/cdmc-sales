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
            return View();
        }
        [GridAction]
        public ActionResult _SelectManagerIndex(int? month)
        {
            if (month == null) month = DateTime.Now.Month;
            // if (month == null) month = 5;
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
                    newmodel.Responsibility = model.Responsibility;
                    newmodel.Discipline = model.Discipline;
                    newmodel.Excution = model.Excution;
                    newmodel.Targeting = model.Targeting;
                    newmodel.Searching = model.Searching;
                    newmodel.Production = model.Production;
                    newmodel.PitchPaper = model.PitchPaper;
                    newmodel.WeeklyMeeting = model.WeeklyMeeting;
                    newmodel.MonthlyMeeting = model.MonthlyMeeting;
                    
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
                    newmodel.TargetName = model.TargetName;
                    newmodel.Assigner = model.Assigner;
                    newmodel.Responsibility = model.Responsibility;
                    newmodel.Discipline = model.Discipline;
                    newmodel.Excution = model.Excution;
                    newmodel.Targeting = model.Targeting;
                    newmodel.Searching = model.Searching;
                    newmodel.Production = model.Production;
                    newmodel.PitchPaper = model.PitchPaper;
                    newmodel.WeeklyMeeting = model.WeeklyMeeting;
                    newmodel.MonthlyMeeting = model.MonthlyMeeting;
                    
                    newmodel.Month = month;
                    newmodel.Year = DateTime.Now.Year;
                    CH.Create<ManagerScore>(newmodel);
                }
            }

            var list = CRM_Logical._EmployeePerformance.GetManagerLeadsPerformances(month.Value);
            var data = list.ToList();


            return View(new GridModel(data));

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
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _UpdateTeamLeadPerformance(int id, int? month)
        {
            if (month == null) month = DateTime.Now.Month;
            _TeamLeadPerformance model = new _TeamLeadPerformance();
            AssignPerformanceScore newmodel = new AssignPerformanceScore();
            if (id > 0)
            {
                if (TryUpdateModel(model))
                {
                    newmodel.ID = id;
                    newmodel.TargetName = model.Name;
                    newmodel.RateAssigner = model.RateAssigner;
                    newmodel.Rate = model.Rate;
                    newmodel.Score = model.Score;
                    newmodel.Month = month;
                    newmodel.Year = DateTime.Now.Year;

                    CH.Edit<AssignPerformanceScore>(newmodel);
                }
                //var model = CH.GetDataById<ManagerScore>(id);
                //model.Item1Score = Item1ScoreString;
                //CH.Edit<ManagerScore>(model);
            }
            else
            {

                if (TryUpdateModel(model))
                {
                    newmodel.TargetName = model.Name;
                    newmodel.RateAssigner = model.RateAssigner;
                    newmodel.Rate = model.Rate;
                    newmodel.Score = model.Score;
                    newmodel.Month = month;
                    newmodel.Year = DateTime.Now.Year;
                    CH.Create<AssignPerformanceScore>(newmodel);
                }
            }

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
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _UpdateSalesPerformance(int id, int? month)
        {
            if (month == null) month = DateTime.Now.Month;
            _SalesPerformance model = new _SalesPerformance();
            AssignPerformanceScore newmodel = new AssignPerformanceScore();
            if (id > 0)
            {
                if (TryUpdateModel(model))
                {
                    newmodel.ID = id;
                    newmodel.TargetName = model.Name;
                    newmodel.RateAssigner = model.RateAssigner;
                    newmodel.Rate = model.Rate;
                    newmodel.Score = model.Score;
                    newmodel.Month = month;
                    newmodel.Year = DateTime.Now.Year;

                    CH.Edit<AssignPerformanceScore>(newmodel);
                }
                //var model = CH.GetDataById<ManagerScore>(id);
                //model.Item1Score = Item1ScoreString;
                //CH.Edit<ManagerScore>(model);
            }
            else
            {

                if (TryUpdateModel(model))
                {
                    newmodel.TargetName = model.Name;
                    newmodel.Rate = model.Rate;
                    newmodel.RateAssigner = model.RateAssigner;
                    newmodel.Score = model.Score;
                    newmodel.Month = month;
                    newmodel.Year = DateTime.Now.Year;
                    CH.Create<AssignPerformanceScore>(newmodel);
                }
            }

            var list = CRM_Logical._EmployeePerformance.GetSalesPerformances(month.Value);
            var data = list.ToList();
            return View(new GridModel(data));

        }
       

    }
}
