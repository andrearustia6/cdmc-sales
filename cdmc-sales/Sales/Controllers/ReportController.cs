using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;
using Entity;
using Utl;
using System.Web.Profile;
using System.Configuration;
using System.Data.OleDb;
using System.Data;

namespace Sales.Controllers
{
    [LeaderRequired]
    public class ReportController : Controller
    {
        protected override void Dispose(bool disposing)
        {
            CH.DB.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult MemberProgress(DateTime? startdate, DateTime? enddate)
        {
            ViewBag.StartDate = startdate;
            ViewBag.EndDate = enddate;
            var projects = this.GetProjectByAccount();
           
            return View(projects);
        }
        //
        // GET: /Report/

        

        /// <summary>
        /// call list 统计
        /// </summary>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>
        public ActionResult LeadCalls(DateTime? startdate, DateTime? enddate)
        {
           
            ViewBag.StartDate = startdate;
            ViewBag.EndDate = enddate;


            var result = new TotalLeadCallAmount();
            var ps = this.GetProjectByAccount();
            var vl = new List<ViewLeadCallAmountInProject>();

            var cs = Utl.Utl.GetCallsInfo(ps,startdate,enddate);

            ps.ForEach(p =>
            {
                vl.Add(new ViewLeadCallAmountInProject() { LeadCallAmounts = p.GetProjectMemberLeadCalls(cs,startdate, enddate), project = p });

            });
          
            result.ViewLeadCallAmountInProjects = vl;


            return View(result);
        }

        /// <summary>
        /// call list
        /// </summary>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>
        public ActionResult MemberLeadCalls(DateTime? startdate, DateTime? enddate)
        {
            ViewBag.StartDate = startdate;
            ViewBag.EndDate = enddate;
            var ps = this.GetProjectByAccount();
            return View(ps);
        }

        public ActionResult Progress(DateTime? startdate, DateTime? enddate)
        {
            ViewBag.StartDate = startdate;
            ViewBag.EndDate = enddate;
            var ps = this.GetProjectByAccount();
            var data = new List<ViewProjectProgressAmount>();
            ps.ForEach(p =>
            {
                var d = p.GetProjectProgress(startdate,enddate);
                data.Add(d);
            });
            return View(data);
        }

    }
}
