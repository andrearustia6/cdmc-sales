﻿using System;
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
using BLL;
using System.Web.Security;

namespace Sales.Controllers
{
     public class ManagerReportController :ReportController
     {
         public ActionResult MemberPerformanceIndex(List<int> selectedproject, int? month, List<string> selectedmembers)
         {
             List<Project> ps = new List<Project>();
             if (selectedproject == null)
             {
                  ps = this.GetProjectByAccount();
                 
                 selectedproject = ps.Select(i => i.ID).ToList();
             }
             if (selectedmembers == null)
             {
                 List<Member> members = new List<Member>();
                 foreach(var p in ps)
                 {
                     if (p.IsActived == true)
                     {
                         members.AddRange(p.Members.FindAll(f => f.IsActivated == true));
                     }
                 }
                 selectedmembers = members.Select(m => m.Name).Distinct().ToList();
             }

             var data = Report.GetMemberPerformanceIndex(selectedproject, month, selectedmembers);

             var list = new List<ViewMemberPerformance>();
             foreach (var m in selectedmembers)
             {
                 var mp = Report.GetSingleMemberPerformance(data, m);
                 list.Add(mp);
             }
             return View(@"~\views\report\MemberPerformanceIndex.cshtml", list);

         }
     }

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
            var list = Report.GetMemberProgressList(selectedprojects, isActivated, startdate, enddate);
            return View(list);
        }

       /// <summary>
       /// 项目进度表
       /// </summary>
       /// <param name="selectedprojects"></param>
       /// <param name="isActivated"></param>
       /// <param name="startdate"></param>
       /// <param name="enddate"></param>
       /// <returns></returns>
        public ActionResult ProjectProgress(List<int> selectedprojects, bool? isActivated, DateTime? startdate, DateTime? enddate)
        { 
           var list = Report.GetProjectProgressList(selectedprojects,isActivated,startdate,enddate);
           return View(list);
        }

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
        /// call list列表
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
