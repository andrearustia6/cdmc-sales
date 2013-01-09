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


        public ActionResult MemberLeadCallsChart(DateTime? startdate, DateTime? enddate, int?projectid, string charttype)
        {

            ViewBag.StartDate = startdate;
            ViewBag.EndDate = enddate;
            ViewBag.ChartType = charttype;
            projectid = this.TrySetProjectIDForUser(projectid);
            ViewBag.ProjectID = projectid;
            if (projectid != null)
            {
                List<ViewCallListChart>  data;
                if (charttype == "Company")
                    data = GetContedtedLeadCompanyChartData(projectid, startdate, enddate);
                else if (charttype == "Category")
                    data = GetContedtedLeadCategoryChartData(projectid, startdate, enddate);
                else
                    data = new List<ViewCallListChart>();
                return View(data);
            }
            else
            {
                return View();
            }
        }

        private List<ViewCallListChart> GetContedtedLeadCompanyChartData(int? projectid, DateTime? startdate, DateTime? enddate)
        {
            startdate = startdate == null ? new DateTime(1, 1, 1) : startdate;
            enddate = enddate == null ? new DateTime(9999, 1, 1) : enddate;

            var account = Employee.CurrentUser;
 
            var members = from m in CH.DB.Members where m.ProjectID == projectid select m;

            var memberlist = members.ToList();
          
            var viewCallListCharts = new List<ViewCallListChart>();
            foreach (var m in memberlist)
            {

                var leadcalls = from l in CH.DB.LeadCalls where l.ProjectID == projectid && l.MemberID == m.ID && startdate<=l.CallDate && l.CallDate<=enddate select l;
                var leads = from l in CH.DB.Leads where leadcalls.Any(lc => lc.LeadID == l.ID) select l;
                var groupedleads = leads.Distinct().GroupBy(l => l.CompanyID);
                var companysum = new List<ViewCompanyCallSum>();
                foreach (var gl in groupedleads)
                {
                    var leadcount = gl.Count();
                    var cs = companysum.FirstOrDefault(c => c.LeadCalledCountNumber == leadcount);
                    if (cs == null)
                    {
                        cs = new ViewCompanyCallSum() { LeadCalledCountNumber = leadcount, CompanyCount = 1 };
                        companysum.Add(cs);
                    }
                    else
                        cs.CompanyCount++;
                   
                }
                viewCallListCharts.Add(new ViewCallListChart() { Member = m, ViewCompanyCallSums = companysum.OrderByDescending(c=>c.LeadCalledCountNumber).ToList() });
               
            }
          

            return viewCallListCharts;
        }

        private List<ViewCallListChart> GetContedtedLeadCategoryChartData(int? projectid, DateTime? startdate, DateTime? enddate)
        {
            startdate = startdate == null ? new DateTime(1, 1, 1) : startdate;
            enddate = enddate == null ? new DateTime(9999, 1, 1) : enddate;

            var account = Employee.CurrentUser;

            var members = from m in CH.DB.Members where m.ProjectID == projectid select m;

            var memberlist = members.ToList();

            var viewCallListCharts = new List<ViewCallListChart>();
           
            //包含category的crm
            var crms = from crm in CH.DB.CompanyRelationships  where crm.Categorys.Count>0 && crm.ProjectID == projectid  select crm;
            
            var categorys = from c in CH.DB.Categorys where c.ProjectID == projectid select c;

            //打给有category公司的call
            var allcategoryleadcalls = from l in CH.DB.LeadCalls from crm in crms where l.ProjectID == projectid && l.CompanyRelationshipID == crm.ID  && startdate <= l.CallDate && l.CallDate <= enddate select l;
            
            //有category公司的lead
            var leads = from l in CH.DB.Leads where allcategoryleadcalls.Any(lc => lc.LeadID == l.ID) select l;
            foreach (var m in memberlist)
            {
                var mleadcalls = from l in allcategoryleadcalls where  l.MemberID == m.ID select l;

                var categorysum = new List<ViewCategoryCallSum>();

                foreach (var c in categorys)
                {
                    var mcallleads =  from l in leads
                                      from lc in mleadcalls
                                      from crm in crms
                                      where lc.CompanyRelationshipID == crm.ID && l.ID == lc.LeadID && crm.Categorys.Any(cate=>cate.ID == c.ID)
                                               select l;

                    if (mcallleads.Count() > 0)
                    {

                        categorysum.Add(new ViewCategoryCallSum() { CategoryName = c.Name, LeadCalledCountNumber = mcallleads.Distinct().Count() });
                    }
                }
                viewCallListCharts.Add(new ViewCallListChart() { Member = m, ViewCategoryCallSum = categorysum });

            }


            return viewCallListCharts;
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
