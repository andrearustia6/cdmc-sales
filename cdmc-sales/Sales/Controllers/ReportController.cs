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
        [ManagerRequired]
        public ActionResult PerformanceIndex(int? selectedmonth)
        {
            ViewBag.SelectedMonth = selectedmonth;
            return View();
        }

         ProjectPerformaceData GetPerformanceData(int? month)
        {
            DateTime startdate;
            DateTime enddate;
           
            if (month == null) month = DateTime.Now.Month;
            Utl.Utl.GetMonthActualStartdateAndEnddate(month, out startdate, out enddate);
            DateTime monthstartdate = new DateTime(DateTime.Now.Year, month.Value, 1);
            DateTime monthenddate = monthstartdate.EndOfMonth();


          
             //选择时间段内的数据
            var per = from p in CH.DB.Projects where p.IsActived==true 
                      join crm in CH.DB.CompanyRelationships on p.ID equals crm.ProjectID into crms
                      join m in CH.DB.Members on p.ID equals m.ProjectID into mems
                      join d in CH.DB.Deals on p.ID equals d.ProjectID into ds
                      //join c in CH.DB.LeadCalls on p.ID equals c.ProjectID into lcs
                    
                      join tp in CH.DB.TargetOfMonths on p.ID equals tp.ProjectID into tps
                      join tm in CH.DB.TargetOfMonthForMembers on p.ID equals tm.ProjectID into tms
                      select new 
                      {
                          Project = p,
                          Deals = ds.Where(d => d.ActualPaymentDate < monthenddate && d.ActualPaymentDate >= monthstartdate && d.Abandoned==false),
                          CRMs = crms,
                          Mem=mems,
                          Month = month,
                          TPs = tps.Where(t => month == t.StartDate.Month),
                          TMs = tms.Where(t => t.StartDate.Month == month)
                      };

            var data = new ProjectPerformaceData()
            {
                //Faxouts = per.SelectMany(s => s.Faxouts),
                Deals = per.SelectMany(s => s.Deals),
                CRMs = per.SelectMany(s => s.CRMs),
                StartDate = startdate,
                EndDate = enddate,
                Month = month.Value,
                MonthStartDate = monthstartdate,
                MonthEndDate = monthenddate,
                ProjectTargets = per.SelectMany(s=>s.TPs),
                MemberTargets = per.SelectMany(s=>s.TMs),
                Projects = per.Select(p=>p.Project),
                Members = per.SelectMany(p=>p.Mem)
            };
            data.Leads = data.CRMs.Select(s => s.Company).SelectMany(s => s.Leads).Where(l => l.CreatedDate >= startdate && l.CreatedDate < enddate);
            data.Faxouts =  CRM_Logical.GetProjectFaxoutList(startdate,enddate,CH.DB.Projects.Where(p=>p.IsActived==true).Select(s=>s.ID).ToList());
            return data;

        }

        [GridAction]
        public ActionResult _ManagerMonthPerformanceIndex(int? month)
        {
            
            var data = GetPerformanceData(month);

            var list = from m in data.Projects.Select(s=>s.Manager).Distinct()
                       select new AjaxManagerMonthPerformance()
                       {
                             Name = m,
                             Faxouts = data.Faxouts.Where(l => l.Project.Manager == m),
                             Deals = data.Deals.Where(d => d.Project.Manager == m),
                             Leads = data.CRMs.Where(w=>w.Project.Manager==m).Select(s => s.Company).SelectMany(s => s.Leads).Where(l => l.CreatedDate >= data.StartDate && l.CreatedDate < data.EndDate),
                             StartDate = data.StartDate,
                             EndDate= data.EndDate,
                             Month = data.Month,
                             Members = data.Members.Where(w=>w.Project.Manager==m).Select(s => s.Name).Distinct(),
                             ProjectName = data.Projects.Where(p=>p.Manager == m).Select(s=>s.ProjectCode).Distinct().ToStringList()
                       };
            return View(new GridModel<AjaxManagerMonthPerformance> { Data = list });
        }

        [GridAction]
        public ActionResult _LeadMonthPerformanceIndex(int? month, string manager)
        {
            if(string.IsNullOrEmpty(manager)|| month==null)  return View(new GridModel<AjaxLeadMonthPerformance> { Data = new List<AjaxLeadMonthPerformance>() });

            var assignscores  = from a in CH.DB.AssignPerformanceScores where a.Month==month && a.Year == DateTime.Now.Year select a;
            var assignrates = from a in CH.DB.AssignPerformanceRates where a.Month == month && a.Year == DateTime.Now.Year select a;
            
            var data = GetPerformanceData(month);
            var list = from m in data.Projects.Where(w=>w.Manager==manager).Select(s => s.TeamLeader).Distinct()
                       select new AjaxLeadMonthPerformance()
                       {
                           Name = m,
                           Faxouts = data.Faxouts.Where(l => l.Member.Name == m),//考虑所有所在项目，不进行项目筛选
                           Deals = data.Deals.Where(d => d.Project.TeamLeader == m),//考虑所有所在项目，不进行项目筛选
                           Leads = data.CRMs.Where(w => w.Project.TeamLeader == m).Select(s => s.Company).SelectMany(s => s.Leads).Where(l => l.Creator == m && l.CreatedDate >= data.StartDate && l.CreatedDate < data.EndDate),//考虑所有所在项目，不进行项目筛选
                           StartDate = data.StartDate,
                           EndDate = data.EndDate,
                           Month = month,
                           Members = data.Members.Where(w => w.Project.TeamLeader == m).Select(s => s.Name).Distinct(),//考虑所有所在项目，不进行项目筛选
                           ProjectName = data.Projects.Where(p => p.TeamLeader == m).Select(s => s.ProjectCode).ToStringList(),//考虑所有所在项目，不进行项目筛选
                           TotalCheckinTargets = data.ProjectTargets.Where(t => t.Project.TeamLeader == m).Sum(s => (decimal?)s.CheckIn),//考虑所有所在项目，不进行项目筛选
                           AssignedScore = assignscores.Where(a => a.TargetName == m).Average(s => (double?)s.Score),
                           Rate  = assignrates.Where(a=>a.TargetName == m).Average(s=>(double?)s.Rate) 
                       };
            return View(new GridModel<AjaxLeadMonthPerformance> { Data = list });
           
        }

        [GridAction]
        public ActionResult _SalesMonthPerformanceIndex(int? month, string leader)
        {

            if (string.IsNullOrEmpty(leader) || month == null) return View(new GridModel<AjaxLeadMonthPerformance> { Data = new List<AjaxLeadMonthPerformance>() });
       
            var assignscores  = from a in CH.DB.AssignPerformanceScores where a.Month==month && a.Year == DateTime.Now.Year select a;
            var assignrates = from a in CH.DB.AssignPerformanceRates where a.Month == month && a.Year == DateTime.Now.Year select a;
            
            var data = GetPerformanceData(month);
            var list = from m in data.Projects.Where(w=>w.TeamLeader==leader).SelectMany(s => s.Members).Where(w=>w.Name!=leader).Select(s=>s.Name).Distinct()
                       select new AjaxSalesMonthPerformance()
                       {
                           Name = m,
                           Faxouts = data.Faxouts.Where(l => l.Member.Name == m),
                           Deals = data.Deals.Where(d => d.Sales ==m),
                           Leads = data.CRMs.Where(w => w.Project.TeamLeader == leader).Select(s => s.Company).SelectMany(s => s.Leads).Where(l => l.Creator==m && l.CreatedDate >= data.StartDate && l.CreatedDate < data.EndDate),
                           StartDate = data.StartDate,
                           EndDate = data.EndDate,
                           Month = month,
                           ProjectName = data.Projects.Where(p => p.TeamLeader == leader).Select(s => s.ProjectCode).ToStringList(),
                           TotalCheckinTargets = data.MemberTargets.Where(t => t.Member.Name==m).Sum(s => (decimal?)s.CheckIn),
                           AssignedScore = assignscores.Where(a => a.TargetName == m).Average(s => (double?)s.Score),
                           Rate  = assignrates.Where(a=>a.TargetName == m).Average(s=>(double?)s.Rate) 
                       };
            return View(new GridModel<AjaxSalesMonthPerformance> { Data = list });
        }
         


        public ActionResult MemberProgress(List<int> selectedprojects, bool? isActivated, DateTime? startdate, DateTime? enddate)
        {
            var list = Report.GetMemberProgressList(selectedprojects, isActivated, startdate, enddate);
            return View(list);
        }



        /// <summary>
        /// call list 统计
        /// </summary>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>
        public ActionResult LeadCalls(List<int> selectedprojects, string selecttype, bool? isActivated, DateTime? startdate, DateTime? enddate)
        {
            ViewBag.Right = ReviewRight.CallsSumReview.ToString();
            ViewBag.StartDate = startdate;
            ViewBag.EndDate = enddate;
            ViewBag.SelectedProjects = selectedprojects;
            ViewBag.SelectType = selecttype;

            var result = new TotalLeadCallAmount();

            var vl = new List<ViewLeadCallAmountInProject>();

            List<Project> ps = null;
            if (selectedprojects != null)
            {
                var list = from p in CH.DB.Projects where selectedprojects.Any(sp => sp == p.ID) select p;
                ps = list.ToList();
            }
            else
            {
                return View();
            }

            


           // var cs = Utl.Utl.GetCallsInfo(ps, startdate, enddate);
            var cs = new List<ViewPhoneInfo>();
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
        public ActionResult MemberLeadCalls(List<int> selectedprojects,string selecttype, List<int> selectedcallTypes, bool? isActivated, DateTime? startdate, DateTime? enddate)
        {
            ViewBag.SelectedCallTypes = selectedcallTypes;
            ViewBag.SelectedProjects = selectedprojects;
            ViewBag.SelectType = selecttype;
            ViewBag.Right = ReviewRight.CallsReview.ToString();
            ViewBag.StartDate = startdate;
            ViewBag.EndDate = enddate;
            List<Project> ps;
            if (selectedprojects == null)
            {
                return View();
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
                      where l.ProjectID == projectid && l.CallDate>= startdate && l.CallDate<= enddate
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
            ViewBag.Right = ReviewRight.CallAnalysisReview;
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
