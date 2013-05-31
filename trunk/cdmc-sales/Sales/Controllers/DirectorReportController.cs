using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utl;
using Sales.Model;
using Entity;
using BLL;
using System.Web.UI.WebControls;

namespace Sales.Controllers
{
    [DirectorRequired]
    public class DirectorReportController : Controller
    {
        public JsonResult _GetIncomebyProjectInMonth()
        {
             var totaldeals = CRM_Logical.GetDeals(true);
            totaldeals = totaldeals.Where(w => w.Income > 0);

              var dealprojectmonth = from d in totaldeals
                                   group d by new { d.ActualPaymentDate.Value.Month, d.ActualPaymentDate.Value.Year,d.Project.Name_CH }
                           into grp

                           select new 
                           {
                               Year = grp.Key.Year,
                               Month = grp.Key.Month,
                               IncomeAmount = grp.Sum(c => c.Income),
                               ProjectName = grp.Key.Name_CH,
                               Category = grp.Key.Year.ToString()+"年"+grp.Key.Month.ToString()+"月",
                           };
            var ps = from p in CH.DB.Projects.Where(p=>p.IsActived==true) 
                     select new {
                         Name =p.Name_CH,
                         Categories = string.Join(",", dealprojectmonth.Where(w=>w.ProjectName == p.Name_CH).Select(s=>s.Category) ),
                         IncomeAmount = string.Join(",", dealprojectmonth.Where(w => w.ProjectName == p.Name_CH).Select(s => s.IncomeAmount)),
                     };
                   
            //var data = from p in ps{}
            //                       from p in ps 
            //                        group d by new { d.ActualPaymentDate.Value.Month, d.ActualPaymentDate.Value.Year,d.Project.Name_CH }
            return Json(ps);
    
        }

        public ActionResult DealGroupByProject()
        {
            return View(CRM_Logical._Project.GetAllProjectPerformance());
        }

        //public ActionResult DealGroupByProject(int? year, int? month)
        //{
        //    _DealByProjectData data = new _DealByProjectData();
        //    if (year == null) year = DateTime.Now.Year;
        //    if (month == null) month = DateTime.Now.Month;

        //   var totaldeals = CRM_Logical.GetDeals(true);
        //    totaldeals = totaldeals.Where(w => w.Income > 0);
        //    var deals = totaldeals.Where(d => d.ActualPaymentDate.Value.Year == year && d.ActualPaymentDate.Value.Month == month);


        //    var ps = from p in CH.DB.Projects select p;
        //    var mems = from m in CH.DB.Members select m;
        //    var dealbyproject = from d in deals
        //                        group d by new { d.Project.Name_CH, d.Project.ID }
        //                            into grp

        //                            select new _DealByProject
        //                            {
        //                                MemberCount = mems.Count(m => m.ProjectID == grp.Key.ID),
        //                                IncomeAmount = grp.Sum(c => c.Income),
        //                                DealAmount = grp.Sum(c => c.Payment),
        //                                ProjectName = grp.Key.Name_CH,
        //                                TotalIncomeAmount = totaldeals.Where(w => w.ProjectID == grp.Key.ID).Sum(s => s.Income)
        //                            };

        //    data._DealByProject = dealbyproject;
        //    var l = new List<_DealByProjectInMonth>();
        //    data._DealByProjectInMonth = l.AsQueryable();
        //    return View(data);
        //}

        //public ActionResult DealGroupByProject(int? year, int? month)
        //{
        //    _DealByProjectData data = new _DealByProjectData();
        //    if (year == null) year = DateTime.Now.Year;
        //    if (month == null) month = DateTime.Now.Month;

        //    var totaldeals = CRM_Logical.GetDeals(true);
        //    totaldeals = totaldeals.Where(w => w.Income > 0);
        //    var deals = totaldeals.Where(d => d.ActualPaymentDate.Value.Year == year && d.ActualPaymentDate.Value.Month == month);

           
        //    var ps = from p in CH.DB.Projects select p;
        //    var mems = from m in CH.DB.Members select m;
        //    var dealbyproject = from d in deals
        //               group d by new { d.Project.Name_CH,d.Project.ID  }
        //                   into grp
                          
        //                   select new _DealByProject
        //                   {
        //                       MemberCount = mems.Count(m=>m.ProjectID==grp.Key.ID),
        //                       IncomeAmount = grp.Sum(c => c.Income),
        //                       DealAmount = grp.Sum(c => c.Payment),
        //                       ProjectName = grp.Key.Name_CH,
        //                       TotalIncomeAmount = totaldeals.Where(w => w.ProjectID == grp.Key.ID).Sum(s => s.Income)
        //                   };

        //    data._DealByProject = dealbyproject;
        //    var l = new List<_DealByProjectInMonth>();
        //    data._DealByProjectInMonth = l.AsQueryable();
        //    return View(data);
        //}

        public ActionResult DealGroupBySales(int? year, int? month)
        {
            if(year==null)year = DateTime.Now.Year;
            if(month==null)month = DateTime.Now.Month;
            var totaldeals = CRM_Logical.GetDeals(true);
            totaldeals = totaldeals.Where(w => w.Income > 0);
             var deals = totaldeals.Where(d =>d.ActualPaymentDate.Value.Year == year && d.ActualPaymentDate.Value.Month== month);
             var data = from d in deals
                         group d by new { d.Sales,d.Project.Name_CH}
                         into grp
                         select new _DealBySales {
                               Sales = grp.Key.Sales,
                               IncomeAmount = grp.Sum(c=>c.Income),
                               DealAmount = grp.Sum(c => c.Payment),
                               ProjectName = grp.Key.Name_CH,
                               TotalIncomeAmount = totaldeals.Where(w => w.Sales == grp.Key.Sales).Sum(s => s.Income)
                           };

            return View(data);
        }

        public ActionResult Index()
        {
            return View();
        }

    }
}
