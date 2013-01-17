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
    public class ReportController : SalesReportController
    {
        //protected override void Dispose(bool disposing)
        //{
        //    CH.DB.Dispose();
        //    base.Dispose(disposing);
        //}
        public ActionResult MemberProgress(List<int> selectedprojects, bool? isActivated, DateTime? startdate, DateTime? enddate)
        {
            startdate = startdate == null ? new DateTime(1, 1, 1) : startdate;
            enddate = enddate == null ? new DateTime(9999, 1, 1) : startdate;

            if (selectedprojects == null)
            {
                selectedprojects = BLL.CRM_Logical.GetUserInvolveProject().Select(p => p.ID).ToList();
            }

            if (selectedprojects != null)
            {
                var totaldeals = from d in CH.DB.Deals
                                 where selectedprojects.Any(sp => sp == d.ProjectID)
                                 select d;

                var deals = from d in totaldeals where d.SignDate >= startdate && d.SignDate <= enddate select d;

                var totaltargetofmonths = from t in CH.DB.TargetOfMonthForMembers
                                          where selectedprojects.Any(sp => sp == t.ProjectID)
                                          select t;

                var targetofmonths = from t in totaltargetofmonths
                                     where (t.StartDate >= startdate && t.StartDate <= enddate) || (t.StartDate >= startdate && t.StartDate <= enddate)
                                     select t;

                var projects = from p in CH.DB.Projects
                               where selectedprojects.Any(sp => sp == p.ID) && p.EndDate >= startdate && p.EndDate <= enddate
                               select p;

                var list = new List<ViewProjectMemberProgressAmount>();

                foreach (var p in projects.ToList())
                {
                    ViewProjectMemberProgressAmount pm = new ViewProjectMemberProgressAmount();
                    pm.Project = p;
                    var tms = from m in p.Members where p.IsActived==true select m;
                    var memlist = new List<ViewMemberProgressAmount>();
                    foreach (var m in tms)
                    {
                        ViewMemberProgressAmount v = new ViewMemberProgressAmount();
                        v.Member = m;
                        var projectdeals = from d in deals where d.Sales==m.Name && d.ProjectID == p.ID select d;
                        var projecttargets = from t in targetofmonths where t.MemberID == m.ID && t.ProjectID == p.ID select t;

                        var projecttotaldeals = from d in totaldeals where d.Sales == m.Name &&  d.ProjectID == p.ID select d;
                        var projecttotaltargets = from t in totaltargetofmonths where t.MemberID == m.ID &&  t.ProjectID == p.ID select t;

                        if (projectdeals.Count() > 0)
                        {
                            v.CheckIn = projectdeals.Sum(d => d.Income);
                            v.DealIn = projectdeals.Sum(d => d.Payment);
                        }

                        if (projecttargets.Count() > 0)
                        {
                            v.CheckInTarget = projecttargets.Sum(t => t.CheckIn);
                            v.DealInTarget = projecttargets.Sum(t => t.Deal);
                        }

                        if (projecttotaldeals.Count() > 0)
                        {
                            v.TotalCheckIn = projecttotaldeals.Sum(d => d.Income);
                            v.TotalDealIn = projecttotaldeals.Sum(d => d.Payment);

                        }
                        if (projecttotaltargets.Count() > 0)
                        {
                            v.TotalDealinTarget = projecttotaltargets.Sum(t=>t.Deal);
                        }
                        memlist.Add(v);
                    }
                    pm.ViewMemberProgressAmounts = memlist;
                    list.Add(pm);
                }

                return View(list);
            }
            return View();
        }


        public ActionResult ProjectProgress(List<int> selectedprojects, bool? isActivated, DateTime? startdate, DateTime? enddate)
        { 
           startdate = startdate==null?new DateTime(1,1,1):startdate;
           enddate = enddate == null?new DateTime(9999,1,1):startdate;

           if (selectedprojects == null)
           {
               selectedprojects = BLL.CRM_Logical.GetUserInvolveProject().Select(p => p.ID).ToList();
           }
            if(selectedprojects != null)
            {
                var totaldeals = from d in CH.DB.Deals
                            where selectedprojects.Any(sp => sp == d.ProjectID) 
                            select d;

                var deals =  from d  in totaldeals where d.SignDate >= startdate && d.SignDate <= enddate select d;
 
                var totaltargetofmonths = from t in CH.DB.TargetOfMonths
                                     where selectedprojects.Any(sp => sp == t.ProjectID) select t;

                var targetofmonths =  from t in totaltargetofmonths where (t.StartDate >= startdate && t.StartDate <= enddate)||(t.StartDate >= startdate && t.StartDate <= enddate)
                                      select t;

                 var projects =  from p in CH.DB.Projects
                            where selectedprojects.Any(sp => sp == p.ID) && p.EndDate >= startdate && p.EndDate <= enddate
                            select p;

                 var list = new List<ViewProjectProgressAmount>();

                foreach(var p in projects.ToList())
                {
                    ViewProjectProgressAmount v = new ViewProjectProgressAmount();

                    var projectdeals = from d in deals where d.ProjectID == p.ID select d;
                    var projecttargets = from t in targetofmonths where t.ProjectID == p.ID select t;

                    var projecttotaldeals = from d in totaldeals where d.ProjectID == p.ID select d;
                    var projecttotaltargets = from t in totaltargetofmonths where t.ProjectID == p.ID select t;
                   
                 
                    v.Project = p;
                    if (projectdeals.Count() > 0)
                    {
                        v.CheckIn = projectdeals.Sum(d => d.Income);
                        v.DealIn = projectdeals.Sum(d => d.Payment);
                    }

                    if (projecttargets.Count() > 0)
                    {
                        v.CheckInTarget = projecttargets.Sum(t => t.CheckIn);

                        v.CheckInPercentage = (int)((v.CheckIn / v.CheckInTarget)*100);
                        v.DealInTarget = projecttargets.Sum(t => t.Deal);
                        v.DealInPercentage = (int)((v.DealIn / v.DealInTarget) * 100);
                    }
                    if (projecttotaldeals.Count() > 0)
                    {
                        v.TotalCheckIn = projecttotaldeals.Sum(d => d.Income);
                        v.TotalDealIn = projecttotaldeals.Sum(d => d.Payment);
                       
                    }

                   
                    v.LeftDay = (p.EndDate - DateTime.Now).Days;
                    list.Add(v);
                }

                return View(list);
            }
            return View();
        }

     
        //
        // GET: /Report/

        /// <summary>
        /// call list 统计
        /// </summary>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>
        public ActionResult LeadCalls(List<int> selectedprojects, bool? isActivated, DateTime? startdate, DateTime? enddate)
        {
            ViewBag.StartDate = startdate;
            ViewBag.EndDate = enddate;


            var result = new TotalLeadCallAmount();

            var vl = new List<ViewLeadCallAmountInProject>();

            List<Project> ps = null;
            if (selectedprojects != null)
            {
                var list  = from p in CH.DB.Projects where selectedprojects.Any(sp => sp == p.ID) select p;
                ps = list.ToList();
            }
            else
                ps = this.GetProjectByAccount();
             

             var cs = Utl.Utl.GetCallsInfo(ps, startdate, enddate);

             ps.ForEach(p =>
            {
                vl.Add(new ViewLeadCallAmountInProject() { LeadCallAmounts = p.GetProjectMemberLeadCalls(cs, startdate, enddate), project = p });
            });

            result.ViewLeadCallAmountInProjects = vl;

             //get all record
            var allsumrecords = new List<ViewLeadCallAmount>();
            foreach(var p in result.ViewLeadCallAmountInProjects)
            {
                allsumrecords.AddRange(p.LeadCallAmounts);
            }
            var groupsum = allsumrecords.GroupBy(g => g.Member.Name);

            //get top sales
            var sumlist = new List<ViewLeadCallSumAmount>();
            var groupsumlist = groupsum.ToList();
            groupsumlist.ForEach(v =>
            {
                var sum = new  ViewLeadCallSumAmount();
                sum.Name = v.Key;
                sum.SalesType = v.FirstOrDefault().Member.SalesType.Name;
                sum.DealSum = v.Sum(s=>s.DealInAmount);
                sum.DurationSum = v.Sum(s=>s.Duration.TotalHours);
                sum.CallSum = v.Sum(s => s.CallListAmount);
                sumlist.Add(sum);
            });
            var topsales = sumlist.OrderByDescending(o => o.DealSum).ToList();
            if(topsales.Count>10)
            {
                topsales = topsales.Take(10).ToList();
            }
            result.TopSales = topsales;

            //周on phone王前三名需要符合销售经理call list 35个，销售专员call list50个为前提。并需要系统自动排除电话时间倒数三名，前提是call list也没打到35/50的要求（考虑工作日，如4天，为28/40)
            //var allQulifyRecordsForTopOnPhone = allsumrecords.FindAll(r => (r.Member.SalesTypeID == 2 && r.CallListAmount >= 35) || (r.Member.SalesTypeID == 1 && r.CallListAmount >= 50));


            var best = sumlist.OrderByDescending(r => r.DurationSum).ToList();
            if (best.Count() > 10)
                best = best.Take(10).ToList();

            result.TopCallers = best;

            var worst = sumlist.FindAll(w => w.DurationSum > 0).OrderBy(r => r.DurationSum).ToList();

            if (worst.Count() > 10)
                worst = worst.Take(10).ToList();
            result.WorstCallers = worst;
            return View(result);

          
        }

        /// <summary>
        /// call list
        /// </summary>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>
        public ActionResult MemberLeadCalls(List<int> selectedprojects, bool? isActivated, DateTime? startdate, DateTime? enddate)
        {
            ViewBag.StartDate = startdate;
            ViewBag.EndDate = enddate;
            List<Project> ps;
            if (selectedprojects == null)
            {
                ps = this.GetProjectByAccount();
            }
            else
            {
                var lsit = from p in CH.DB.Projects where selectedprojects.Any(sp =>sp== p.ID) select p;
                ps = lsit.ToList();
            }
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

    


    [SalesRequired]
    public class SalesReportController : Controller
    {
        protected override void Dispose(bool disposing)
        {
            CH.DB.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult MemberLeadCallsChart(DateTime? startdate, DateTime? enddate, int? projectid, string charttype)
        {

            ViewBag.StartDate = startdate;
            ViewBag.EndDate = enddate;
            ViewBag.ChartType = charttype;
            projectid = this.TrySetProjectIDForUser(projectid);
            ViewBag.ProjectID = projectid;
            if (projectid != null)
            {
                List<ViewCallListChart> data;
                if (charttype == "Company")
                    data = GetContedtedLeadCompanyChartData(projectid, startdate, enddate);
                else if (charttype == "Category")
                    data = GetContedtedLeadCategoryChartData(projectid, startdate, enddate);
                else
                    data = new List<ViewCallListChart>();
                return View(@"~\views\report\MemberLeadCallsChart.cshtml", data);
            }
            else
            {
                return View(@"~\views\report\MemberLeadCallsChart.cshtml");
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

                var leadcalls = from l in CH.DB.LeadCalls where l.ProjectID == projectid && l.MemberID == m.ID && startdate <= l.CallDate && l.CallDate <= enddate select l;
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
                viewCallListCharts.Add(new ViewCallListChart() { Member = m, ViewCompanyCallSums = companysum.OrderByDescending(c => c.LeadCalledCountNumber).ToList() });

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

            //包含category的已打crm
            var crms = from crm in CH.DB.CompanyRelationships where crm.Categorys.Count > 0 && crm.LeadCalls.Count > 0 && crm.ProjectID == projectid select crm;

            var categorys = from c in CH.DB.Categorys where c.ProjectID == projectid select c;

            //打给有category公司的call
            var allcategoryleadcalls = from l in CH.DB.LeadCalls from crm in crms where l.ProjectID == projectid && l.CompanyRelationshipID == crm.ID && startdate <= l.CallDate && l.CallDate <= enddate select l;

            //有category公司的lead
            var leads = from l in CH.DB.Leads where allcategoryleadcalls.Any(lc => lc.LeadID == l.ID) select l;
            foreach (var m in memberlist)
            {
                var mleadcalls = from l in allcategoryleadcalls where l.MemberID == m.ID select l;

                var categorysum = new List<ViewCategoryCallSum>();

                foreach (var c in categorys)
                {
                    var cs = from crm in crms
                             where crm.Categorys.Any(cate => cate.ID == c.ID) && crm.Members.Any(cm=>cm.ID==m.ID)
                             select crm;

                    if (cs.Count() > 0)
                    {

                        categorysum.Add(new ViewCategoryCallSum() { CategoryName = c.Name, CompanyCalledCountNumber = cs.Count() });
                    }
                }
                viewCallListCharts.Add(new ViewCallListChart() { Member = m, ViewCategoryCallSum = categorysum });

            }
            return viewCallListCharts;
        }
    }
}
