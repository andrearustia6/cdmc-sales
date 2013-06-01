using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using Utl;
using BLL;
namespace Sales.Controllers
{
    public class PerformanceController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [GridAction]
        public ActionResult _SelectLeadIndex(int? month)
        {
            //if (month == null) month = DateTime.Now.Month;
            if (month == null) month = 5;
            var list = CRM_Logical._EmployeePerformance.GetTeamLeadsPerformances(month.Value);
            var data = list.ToList();
            return View(new GridModel(data));
        }

    }
}
