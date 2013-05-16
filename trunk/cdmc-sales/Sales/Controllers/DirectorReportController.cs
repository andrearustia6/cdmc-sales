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
        public ActionResult DealGroupBySales()
        {
            var data = from d in CH.DB.Deals
                       group d by new { d.Sales, d.ActualPaymentDate.Value.Year, d.ActualPaymentDate.Value.Month, d.Project.Name_CH }
                           into grp
                           select new _DealBySales {
                               Sales = grp.Key.Sales,
                               IncomeAmount = grp.Sum(c=>c.Income),
                               Month = grp.Key.Month,
                               Year = grp.Key.Year
                           };
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }

    }
}
