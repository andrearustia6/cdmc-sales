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
using Telerik.Web.Mvc;
using System.IO;

namespace Sales.Controllers
{
    [SuperManagerRequired]
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

        public ActionResult TargetOfMonthStatus()
        {
            var list = CRM_Logical._TargetOfMonth.GetCurrentMonthProjectTagetStatus();
            return View(list.ToList());
        }

        public ActionResult ProjectsProgress()
        {
            var list = CRM_Logical._Reports.GetProjectsProgress();
            return View(list.ToList());
        }

        public ActionResult ProjectsCheckInByWeek()
        {
            var list = CRM_Logical._Reports.GetProjectsCheckInByWeek();
            return View(list.ToList());
        }

        public ActionResult ProjectsCheckInLastWeek()
        {
            var list = CRM_Logical._Reports.GetProjectSingleWeekCheckIn(DateTime.Now);
            return View(list);
        }

        public ActionResult ProjectsReportLastWeek(string btnExport)
        {
            var list = CRM_Logical._Reports.GetProjectsReportLastweek(DateTime.Now);
            if (btnExport == "export")
            {
                MemoryStream output = new MemoryStream();
                StreamWriter writer = new StreamWriter(output, System.Text.Encoding.Default);

                writer.Write("sss" + "月");
                writer.WriteLine();
                writer.Flush();
                output.Position = 0;
                return File(output, "text/comma-separated-values", "ProjectsReportLastWeek.csv");
            }
            else
                return View(list);
        }
        /// <summary>
        ///项目入账 
        /// </summary>
        /// <returns></returns>
        public ActionResult ProjectsCheckInByMonth()
        {
            var list = CRM_Logical._Reports.GetProjectsCheckInByMonth();
            return View(list.ToList());
        }

        /// <summary>
        /// 项目入账趋势图
        /// </summary>
        /// <returns></returns>
        public ActionResult ProjectsCheckInByMonthLines()
        {
            var list = CRM_Logical._Reports.GetProjectsCheckInByMonth();
            return View(list.ToList());
        }

         /// <summary>
        /// 个人入账
        /// </summary>
        /// <returns></returns>
        public ActionResult EmployeeCheckInByMonth()
        {
            var list = CRM_Logical._Reports.GetEmployeesCheckInByMonth();
            return View(list.ToList());
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
