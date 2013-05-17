using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utl;
using Sales.Model;

namespace Sales.Controllers
{
    [DirectorRequired]
    public class DirectorReportController : Controller
    {
        public ActionResult DealGroupByProject(int? year, int? month)
        {
            if (year == null) year = DateTime.Now.Year;
            if (month == null) month = DateTime.Now.Month;

            var totaldeals = from deal in CH.DB.Deals.Where(d => d.Sales != "sean" && d.Sales != "john" && d.Abandoned == false && d.Income > 0) select deal;
            var deals = from deal in CH.DB.Deals.Where(d => d.Sales != "sean" && d.Sales != "john" && d.ActualPaymentDate.Value.Year == year && d.ActualPaymentDate.Value.Month == month && d.Abandoned == false && d.Income > 0) select deal;
             var ps = from p in CH.DB.Projects select p;
             var mems = from m in CH.DB.Members select m;
            var data = from d in deals
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

            return View(data);
        }

        public ActionResult DealGroupBySales(int? year, int? month)
        {
            if(year==null)year = DateTime.Now.Year;
            if(month==null)month = DateTime.Now.Month;
            var totaldeals = from deal in CH.DB.Deals.Where(d => d.Sales != "sean" && d.Sales != "john" && d.Abandoned == false && d.Income > 0) select deal;
             var deals = from deal in CH.DB.Deals.Where(d =>d.Sales !="sean" && d.Sales !="john" && d.ActualPaymentDate.Value.Year == year && d.ActualPaymentDate.Value.Month== month && d.Abandoned == false && d.Income > 0) select deal;
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
