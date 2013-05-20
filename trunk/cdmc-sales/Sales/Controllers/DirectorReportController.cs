using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utl;
using Sales.Model;
using Entity;
using BLL;

namespace Sales.Controllers
{
    [DirectorRequired]
    public class DirectorReportController : Controller
    {
        
        public ActionResult DealGroupByProject(int? year, int? month)
        {
            _DealByProjectData data = new _DealByProjectData();
            if (year == null) year = DateTime.Now.Year;
            if (month == null) month = DateTime.Now.Month;

            var totaldeals = CRM_Logical.GetDeals(true);
            totaldeals = totaldeals.Where(w => w.Income > 0);
            var deals = totaldeals.Where(d => d.ActualPaymentDate.Value.Year == year && d.ActualPaymentDate.Value.Month == month);

           
            var ps = from p in CH.DB.Projects select p;
            var mems = from m in CH.DB.Members select m;
            var dealbyproject = from d in deals
                       group d by new { d.Project.Name_CH,d.Project.ID  }
                           into grp
                          
                           select new _DealByProject
                           {
                               MemberCount = mems.Count(m=>m.ProjectID==grp.Key.ID),
                               IncomeAmount = grp.Sum(c => c.Income),
                               DealAmount = grp.Sum(c => c.Payment),
                               ProjectName = grp.Key.Name_CH,
                               TotalIncomeAmount = totaldeals.Where(w => w.ProjectID == grp.Key.ID).Sum(s => s.Income)
                           };

            data._DealByProject = dealbyproject;
            var dealprojectmonth = from d in totaldeals
                                   group d by new { d.ActualPaymentDate.Value.Month, d.ActualPaymentDate.Value.Year,d.Project.Name_CH }
                           into grp

                           select new _DealByProject
                           {
                               Year = grp.Key.Year,
                               Month = grp.Key.Month,
                               IncomeAmount = grp.Sum(c => c.Income),
                               ProjectName = grp.Key.Name_CH
                           };
             data._DealByProjectInMonth = dealprojectmonth;
            return View(data);
        }

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
