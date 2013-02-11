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
using BLL;
using System.Web.Security;
using Telerik.Web.Mvc;

namespace Sales.Controllers
{
    public class ManagerReportController : ReportController
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
                foreach (var p in ps)
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
        //public ActionResult ProjectProgress(List<int> selectedprojects, List<string> selectedangels, int? month, int? year,string selecttype)
        //{
        //    ViewBag.Month = month;
        //    ViewBag.Year = year;
        //    ViewBag.SelectedProjects = selectedprojects;
        //    ViewBag.SelectedAngels = selectedangels;
        //    ViewBag.SelectType = selecttype;
            
        //    var list = Report.GetProjectProgressList(selectedprojects, month==null?DateTime.Now.Month:month.Value, year==null?DateTime.Now.Year:year.Value);
        //    return View(list);
        //}

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
                var list = from p in CH.DB.Projects where selectedprojects.Any(sp => sp == p.ID) select p;
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
            foreach (var p in result.ViewLeadCallAmountInProjects)
            {
                allsumrecords.AddRange(p.LeadCallAmounts);
            }
            var groupsum = allsumrecords.GroupBy(g => g.Member.Name);

            //get top sales
            var sumlist = new List<ViewLeadCallSumAmount>();
            var groupsumlist = groupsum.ToList();
            groupsumlist.ForEach(v =>
            {
                var sum = new ViewLeadCallSumAmount();
                sum.Name = v.Key;
                sum.SalesType = v.FirstOrDefault().Member.SalesType.Name;
                sum.DealSum = v.Sum(s => s.DealInAmount);
                sum.CheckInSum = v.Sum(s => s.CheckInAmount);
                sum.DurationSum = v.Sum(s => s.Duration.TotalHours);
                sum.CallSum = v.Sum(s => s.CallListAmount);
                sumlist.Add(sum);
            });
            var topsales = sumlist.OrderByDescending(o => o.CheckInSum).ToList();
            if (topsales.Count > 10)
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
        public ActionResult MemberLeadCalls(List<int> selectedprojects, List<int> selectedcallTypes, bool? isActivated, DateTime? startdate, DateTime? enddate,string selecttype)
        {
            ViewBag.SelectedCallTypes = selectedcallTypes;
            ViewBag.SelectedProjects = selectedprojects;
            ViewBag.SelectType = selecttype;
            //ViewBag.SelectedProgress = selectedprogress;
            ViewBag.StartDate = startdate;
            ViewBag.EndDate = enddate;
            List<Project> ps;
            if (selectedprojects == null)
            {
                ps = this.GetProjectByAccount();
            }
            else
            {
                var lsit = from p in CH.DB.Projects where selectedprojects.Any(sp => sp == p.ID) select p;
                ps = lsit.ToList();
            }
            return View(ps);
        }

        [GridAction]
        public ActionResult _MemberLeadCalls(int projectid,string types, DateTime? startdate, DateTime? enddate)
        {
            List<int> selectedcallTypes = new List<int>();

            List<int> selectedprogress= new List<int>();

            if (string.IsNullOrEmpty(types))
            {
                selectedcallTypes = CH.GetAllData<LeadCallType>().Select(s => s.ID).ToList();
            }
            else
            {
                 string[] ts = types.Split('|');
                 foreach (var s in ts)
                 {
                     if(!string.IsNullOrEmpty(s))
                     selectedcallTypes.Add(Int32.Parse(s));
                 }
            }

            var lcs = from l in CH.DB.LeadCalls
                      where l.ProjectID == projectid &&l.Member.IsActivated==true && l.CallDate>= startdate && l.CallDate<= enddate
                      && selectedcallTypes.Any(a=>a==l.LeadCallTypeID)
                      select new AjaxViewCallListData
                      {
                          LeadNameCH = l.Lead.Name_CH,
                          LeadNameEN = l.Lead.Name_EN,
                          CallBackDate = l.CallBackDate,
                          CallDate = l.CallDate,
                          Contact = l.Lead.Contact,
                          Mobile = l.Lead.Mobile,
                          Progress = l.CompanyRelationship.Progress.Name,
                          Title = l.Lead.Title,
                          Categorys = l.CompanyRelationship.CategoryString,
                          CompanyNameCH = l.Lead.Company.Name_CH ,
                          CompanyNameEN = l.Lead.Company.Name_EN,
                           CallTypeCode=l.LeadCallType.Code,
                          //FaxOut = l.FaxOut,
                          LeadCallType = l.LeadCallType.Name,
                          Member = l.Member.Name,
                          Result = l.Result
                      };

            var  data = lcs.OrderByDescending(o=>o.CallDate).ToList();

            return View(new GridModel<AjaxViewCallListData> { Data = data});
        }


        public ActionResult ProjectProgress( int? month, int? year, string selecttype)
        {
            ViewBag.Month = month;
            ViewBag.Year = year;

            return View();
        }

        [GridAction]
        public ActionResult _MonthProgress()
        {
            var  year = DateTime.Now.Year;

            var deals = from d in CH.DB.Deals where d.Project.IsActived == true && d.Abandoned == false select d;
            var targets = from t in CH.DB.TargetOfMonths where t.Project.IsActived == true select t;
            var calls = from c in CH.DB.LeadCalls where c.CompanyRelationship.Project.IsActived == true select c;
            //var companys = from c in CH.DB.CompanyRelationships where c.Project.IsActived == true select c;
            var months = new List<int>();

            while (months.Count < DateTime.Now.Month)
            {
                months.Add(months.Count()+1);
            }

            var ps = from p in CH.DB.Projects where p.IsActived==true select p;
            var list = from i in months
                       select new AjaxMonthTotalProgressStatistics()
                       {
                           Month = i,
                           Year = year,
                           LeadCalls = calls,
                           Deals = deals,
                           Projects = ps,
                           TotalDealinTargets = targets.Where(t=>t.StartDate.Month==i).Sum(s=>(decimal?)s.Deal),
                           TotalCheckinTargets = targets.Where(t => t.StartDate.Month == i).Sum(s => (decimal?)s.CheckIn)
                       };

            return View(new GridModel<AjaxMonthTotalProgressStatistics> { Data = list.ToList() });
        }

        [GridAction]
        public ActionResult _ProjectWeekProgress(string startdate)
        {
            if(string.IsNullOrEmpty(startdate))
                return View(new GridModel<AjaxWeekProjectProgressStatistics> { Data = new List<AjaxWeekProjectProgressStatistics>() });

            DateTime startDate = DateTime.Parse(startdate);
            DateTime endDate = startDate.AddDays(7);

            var year = DateTime.Now.Year;
            var deals = from d in CH.DB.Deals where d.Project.IsActived == true && d.Abandoned == false select d;
            var targets = from t in CH.DB.TargetOfWeeks where t.Project.IsActived == true select t;
            var calls = from c in CH.DB.LeadCalls where c.CompanyRelationship.Project.IsActived == true select c;

            var ps = from p in CH.DB.Projects where p.IsActived == true select p;
            var pslist = ps.ToList();
            var list = from p in pslist
                       select new AjaxWeekProjectProgressStatistics()
                       {
                           Project = p,
                           StartDate = startDate,
                           EndDate = endDate,
                           Year = year,
                           LeadCalls = calls,
                           Deals = deals,
                           TotalDealinTargets = targets.Where(t => t.StartDate == startDate && t.ProjectID == p.ID).Sum(s => (decimal?)s.Deal),
                           TotalCheckinTargets = targets.Where(t => t.StartDate == startDate && t.ProjectID == p.ID).Sum(s => (decimal?)s.CheckIn)
                       };
            var data = list.ToList();

            return View(new GridModel<AjaxWeekProjectProgressStatistics> { Data = data });

        }

        [GridAction]
        public ActionResult _ProjectMonthProgress(string month)
        {
            var year = DateTime.Now.Year;

            int monthid = DateTime.Now.Month;
            if (month != null)
                Int32.TryParse(month, out monthid);

            var deals = from d in CH.DB.Deals where d.Project.IsActived == true && d.Abandoned == false  select d;
            var targets = from t in CH.DB.TargetOfMonths where t.Project.IsActived == true  && t.StartDate.Month == monthid select t;
            var calls = from c in CH.DB.LeadCalls where c.CompanyRelationship.Project.IsActived == true select c;
            //var companys = from c in CH.DB.CompanyRelationships where c.Project.IsActived == true select c;
        
            var ps = from p in CH.DB.Projects where p.IsActived == true select p;
            var pslist = ps.ToList();
            var list = from p in pslist
                       select new AjaxMonthProjectProgressStatistics()
                       {
                           Project = p,
                           Month = monthid,
                           Year = year,
                           LeadCalls = calls,
                           Deals = deals,
                           TotalDealinTargets = targets.Where(t => t.StartDate.Month == monthid && t.ProjectID == p.ID).Sum(s => (decimal?)s.Deal),
                           TotalCheckinTargets = targets.Where(t => t.StartDate.Month == monthid && t.ProjectID == p.ID).Sum(s => (decimal?)s.CheckIn)
                       };
            var data = list.ToList();
            return View(new GridModel<AjaxMonthProjectProgressStatistics> { Data = data });

        }

        [GridAction]
        public ActionResult _WeekProgress(string month)
        {
            int monthid = DateTime.Now.Month;
            if (month != null)
                Int32.TryParse(month, out monthid);

            DateTime startdate = new DateTime(DateTime.Now.Year, monthid, 1);

            while (startdate.DayOfWeek != DayOfWeek.Monday)
            {
                startdate = startdate.AddDays(-1);
            }

            var weeks = new List<AjaxWeekTotalProgressStatistics>();
            var enddate = startdate.AddDays(7);

            var year = DateTime.Now.Year;
            var deals = from d in CH.DB.Deals where d.Project.IsActived == true && d.Abandoned == false select d;
            var targets = from t in CH.DB.TargetOfWeeks where t.Project.IsActived == true select t;
            var calls = from c in CH.DB.LeadCalls where c.CompanyRelationship.Project.IsActived == true select c;

            while (enddate.Month <= monthid)
            {
                var week = new AjaxWeekTotalProgressStatistics() { StartDate = startdate, EndDate = enddate };


                week.TotalCheckinTargets = targets.Where(t => t.StartDate == startdate).Sum(s => (decimal?)s.CheckIn);
                week.TotalDealinTargets = targets.Where(t => t.StartDate == startdate).Sum(s => (decimal?)s.Deal);
                week.Deals = deals;
                week.LeadCalls = calls;

                weeks.Add(week);

                startdate = startdate.AddDays(7);
                enddate = enddate.AddDays(7);
            }

            return View(new GridModel<AjaxWeekTotalProgressStatistics> { Data = weeks.ToList() });

        }

        public ActionResult Progress()
        {
          return View();
        }

        [GridAction]
        public ActionResult _Progress()
        {
            var deals = from d in CH.DB.Deals where d.Project.IsActived == true && d.Abandoned==false select d;
            var targets = from t in CH.DB.TargetOfMonths where t.Project.IsActived == true select t;
            var calls = from c in CH.DB.LeadCalls where c.CompanyRelationship.Project.IsActived == true select c;
            var companys = from c in CH.DB.CompanyRelationships where c.Project.IsActived == true select c;

            var ps = from p in CH.DB.Projects
                     where p.IsActived == true
                     select new AjaxViewProject()
                     {
                         deals = deals,
                         ProjectCode = p.ProjectCode,
                         StartDay = p.StartDate,
                         EndDay = p.EndDate,
                         ProjectName = p.Name_CH,
                         Manager = p.Manager,
                         Lead = p.TeamLeader,
                         ProjectTarget = p.Target,
                         TotalCalls = calls.Where(c => c.ProjectID == p.ID).Count(),
                         TotalCheckIn = deals.Where(d => d.ProjectID == p.ID ).Sum(s => s.Income),
                         TotalDealIn = deals.Where(d => d.ProjectID == p.ID ).Sum(s => s.Payment),
                         TotalDealCounts = deals.Where(d => d.ProjectID == p.ID).Count(),
                         TotalCompanysCount = companys.Where(c => c.ProjectID == p.ID).Count(),
                         ProjectID = p.ID
                     };
            return View(new GridModel<AjaxViewProject> { Data = ps.ToList() });
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
                             where crm.Categorys.Any(cate => cate.ID == c.ID) && crm.Members.Any(cm => cm.ID == m.ID)
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
