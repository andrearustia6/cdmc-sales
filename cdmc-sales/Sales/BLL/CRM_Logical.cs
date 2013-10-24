using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entity;
using Utl;
using System.Web.Security;
using Model;
using System.Data.Objects;
using Sales.Model;
using System.Web.Mvc;

namespace BLL
{
    public class CRM_Logical
    {
        public static class _EmployeePerformance
        {
            /// <summary>
            /// 获取manager的考核成绩
            /// </summary>
            /// <param name="month"></param>
            /// <returns></returns>
            public static IEnumerable<_ManagerScore> GetManagerLeadsPerformances(int month)
            {
                var user = Employee.CurrentUserName;
                var rolelvl = Employee.CurrentRole.Level;

                if (rolelvl == PoliticsInterfaceRequired.LVL)
                {
                    rolelvl = DirectorRequired.LVL;
                }
                if (rolelvl == ManagerRequired.LVL)
                {
                    rolelvl = DirectorRequired.LVL;
                }

                if (rolelvl >= SuperManagerRequired.LVL || user == "ray")
                {
                    var md = MonthDuration.GetMonthInstance(month);
                    var managers = CH.DB.Projects.Where(w => w.IsActived == true && !string.IsNullOrEmpty(w.Manager)).Select(s => s.Manager).Distinct();
                    if (Employee.CurrentRole.Level == ManagerRequired.LVL)
                    {
                        managers = CH.DB.Projects.Where(w => w.IsActived == true && !string.IsNullOrEmpty(w.Manager) && w.Manager==user).Select(s => s.Manager).Distinct();
                    }
                    //每个成员的call，也就是所有leader和sales的call
                    //var callgroupbymanger = from l in CH.DB.LeadCalls
                    //                        group l by new { l.Project.Manager }
                    //                            into grp
                    //                            select new { };


                    var memberscall = from p in CH.DB.Projects.Where(w => w.IsActived == true && !string.IsNullOrEmpty(w.Manager))
                                      join l in managers on p.Manager equals l
                                      join c in CH.DB.LeadCalls.Where(w => w.LeadCallTypeID != null && w.LeadCallType.Code >= 40 && w.CreatedDate >= md.MonthStartDate && w.CreatedDate <= md.MonthEndDate) on p.ID equals c.ProjectID
                                      select new
                                      {
                                          Manager = p.Manager,
                                          ProjectID = p.ID,
                                          member = c.Member.Name,
                                          salestypeid = c.Member.SalesTypeID
                                      };
                    //var memberscall = from l in CH.DB.LeadCalls.Where(w => w.LeadCallTypeID != null && w.LeadCallType.Code >= 40 && w.CreatedDate >= md.MonthStartDate && w.CreatedDate <= md.MonthEndDate && w.Project.IsActived == true)
                    //                        join m in managers on l.Project.Manager equals m
                    //                        group l by new { l.Project.Manager, l.Project.ID,l.Member.Name,l.Member.SalesTypeID } into grp
                    //                        select new
                    //                        {
                    //                            Manager = grp.Key.Manager,
                    //                            ProjectID = grp.Key.ID,
                    //                            member = grp.Key.Name,
                    //                            salestypeid = grp.Key.SalesTypeID
                                            //};
                    //每个成员的lead，也就是所有leader和sales的表lead中的数据
                    var memberslead = from l in CH.DB.Leads.Where(w => w.CreatedDate >= md.MonthStartDate && w.CreatedDate <= md.MonthEndDate)
                                      join m in CH.DB.Members on l.Creator equals m.Name
                                      join p in CH.DB.Projects.Where(w => w.IsActived == true && !string.IsNullOrEmpty(w.Manager))
                                        on m.ProjectID equals p.ID
                                      select new
                                      {
                                          manager = p.Manager,
                                          name = l.Creator,
                                          salestypeid = m.SalesTypeID
                                      };
                    var leadgroupbymanager = from l in CH.DB.Leads.Where(w => w.CreatedDate >= md.MonthStartDate && w.CreatedDate <= md.MonthEndDate)
                                      join m in CH.DB.Members on l.Creator equals m.Name
                                      join p in CH.DB.Projects.Where(w => w.IsActived == true && !string.IsNullOrEmpty(w.Manager))
                                        on m.ProjectID equals p.ID
                                      select new
                                      {
                                          manager = p.Manager,
                                          name = l.Creator,
                                          salestypeid = m.SalesTypeID
                                      };
                    var deals = CRM_Logical.GetDeals().Where(w => w.ActualPaymentDate.Value != null && w.ActualPaymentDate.Value.Month == month && w.ActualPaymentDate.Value.Year == DateTime.Now.Year);
                    //var deals = CH.DB.Deals.Where(w => w.ActualPaymentDate.Value != null && w.ActualPaymentDate.Value.Month == month && w.ActualPaymentDate.Value.Year == DateTime.Now.Year);
                   
                    var year = DateTime.Now.Year;
                    //获取登录者（考核人）打分的记录
                    var scores = from r in CH.DB.ManagerScores.Where(w => w.Month == month && w.Year == year) select r;
                    managers = from m in managers
                                      where !(new String[] { "Doris", "Michael","Theresa","Dorothy", "Eric", "Olivia"}).Contains(m)
                                      select m;
                    var lps = from l in managers
                              join sc in scores on l equals sc.TargetName into Joinedscores
                              from aa in Joinedscores.DefaultIfEmpty()
                              select new _ManagerScore()
                              {
                                  RoleLevel = rolelvl,
                                  ID = aa != null ? aa.ID : 0,//aa.ID,
                                  TargetName = l,
                                  User = user,
                                  Assigner = aa != null ? aa.Assigner : user,//aa.Assigner,
                                  Responsibility = aa != null ? aa.Responsibility: 5,//aa.Item1Score.HasValue == false ? 5 : aa.Item1Score,//scores.Where(w => w.TargetName == l).Select(s => s.Item1Score).FirstOrDefault().Value,
                                  Discipline = aa != null ? aa.Discipline : 5,//aa.Item2Score.HasValue == false ? 5 : aa.Item2Score,
                                  Excution = aa != null ? aa.Excution : 5,//aa.Item3Score.HasValue == false ? 5 : aa.Item3Score,
                                  Targeting = aa != null ? aa.Targeting : 5,//aa.Item4Score.HasValue == false ? 5 : aa.Item4Score,
                                  Searching = aa != null ? aa.Searching : 5,//aa.Item5Score.HasValue == false ? 5 : aa.Item5Score,
                                  Production = aa != null ? aa.Production : 5,//aa.Item6Score.HasValue == false ? 5 : aa.Item6Score,
                                  PitchPaper = aa != null ? aa.PitchPaper : 5,//aa.Item7Score.HasValue == false ? 5 : aa.Item7Score,
                                  WeeklyMeeting = aa != null ? aa.WeeklyMeeting : 5,//aa.Item8Score.HasValue == false ? 5 : aa.Item8Score,
                                  MonthlyMeeting = aa != null ? aa.MonthlyMeeting : 10,//aa.Item9Score.HasValue == false ? 10 : aa.Item9Score,
                                  leadcallcount = memberscall.Where(c => c.Manager == l).Count(c => c.salestypeid == 2),
                                  salescallcount = memberscall.Where(c => c.Manager == l).Count(c => c.salestypeid == 1),
                                  leadscount = memberscall.Where(c => c.Manager == l && c.salestypeid == 2).Select(s=>s.member).Distinct().Count(),
                                  salescount = memberscall.Where(c => c.Manager == l && c.salestypeid == 1).Select(s => s.member).Distinct().Count(),
                                  leadnewlead = memberslead.Where(c => c.manager == l).Count(c => c.salestypeid == 2),
                                  salesnewlead = memberslead.Where(c => c.manager == l).Count(c => c.salestypeid == 1),
                                  target = CH.DB.TargetOfMonths.Where(t => t.Project.Manager == l && t.Project.IsActived == true && t.StartDate.Month == month).Sum(s => s.CheckIn),
                                  checkinreal = deals.Where(d => d.Project.Manager == l && d.Income!=null).Sum(s => s.Income),
                                  Confirmed = aa != null ? aa.Confirmed == true ? "是" : "否" : "否",
                                  Rate = aa != null ? aa.Rate==null?0:aa.Rate:1,
                                  Month = aa != null ? aa.Month :month,
                                  Graded = aa != null ? "是" : "否",
                                 

                              };
                    
                    return lps;
                }
                return new List<_ManagerScore>();
            }

            public static IEnumerable<_TeamLeadPerformance> GetTeamLeadsPerformances(int month)
            {
                var user = Employee.CurrentUserName;
                var rolelvl = Employee.CurrentRole.Level;

                if (rolelvl == PoliticsInterfaceRequired.LVL)
                {
                    rolelvl = DirectorRequired.LVL;
                }
                if (rolelvl >= LeaderRequired.LVL)
                {
                    var leads = CH.DB.Projects.Where(w => w.IsActived == true && !string.IsNullOrEmpty(w.TeamLeader)).Select(s => s.TeamLeader).Distinct();
                   //leads = leads.Where(w=>w.)

                    if (Utl.Utl.DebugModel() != true)
                    {
                        var debugmembers = CH.DB.Members.Where(w => w.Test == true).Select(s => s.Name).Distinct();
                        leads = leads.Where(w => debugmembers.Any(a => a == w) == false);
                    }
                    if (Employee.EqualToManager())//版块负责人只能看到自己项目所属的lead
                    {
                        var leadinsameprojects = from p in CH.DB.Projects.Where(w => w.Manager == user) select p.TeamLeader;
                        leads = leads.Where(w => leadinsameprojects.Any(a => a == w));
                    }
                    else if (rolelvl == 100)
                    {
                        leads = leads.Where(w => w == user);
                    }


                    var deals = CRM_Logical.GetDeals().Where(w => w.ActualPaymentDate.Value != null && w.ActualPaymentDate.Value.Month == month && w.ActualPaymentDate.Value.Year == DateTime.Now.Year);
                    var calls = from l in CH.DB.LeadCalls.Where(w => w.LeadCallTypeID != null && w.LeadCallType.Code >= 40) select l;
                    // calls =from c in calls group c by c.LeadID into g select g.FirstOrDefault();//分组并选择第一个
                    var leadadds = from l in CH.DB.Leads select l;
                    var year = DateTime.Now.Year;
                    var rates = from r in CH.DB.AssignPerformanceRates.Where(w => w.Month == month && w.Year == year) select r;
                    var scores = from r in CH.DB.AssignPerformanceScores.Where(w => w.Month == month && w.Year == year) select r;
                    var wd = MonthDuration.GetMonthInstance(month).WeekDurations.Select(s => s.StartDate);
                    var lps = from l in leads

                              select new _TeamLeadPerformance()

                              {
                                  RoleLevel = rolelvl,
                                  ID = scores.Where(w => w.TargetName == l).Select(s => s.ID).FirstOrDefault() == null ? 0 : scores.Where(w => w.TargetName == l).Select(s => s.ID).FirstOrDefault(),
                                  Target = CH.DB.TargetOfMonths.Where(t => t.Project.TeamLeader == l && t.Project.IsActived == true && t.StartDate.Month == month).Sum(s => s.CheckIn),
                                  CheckIn = deals.Where(d => d.Project.TeamLeader == l).Sum(s => s.Income),
                                  Name = l,
                                  User = user,
                                  //Assigner = scores.Where(w => w.TargetName == l).Select(s => s.Assigner).FirstOrDefault() == null ? user : scores.Where(w => w.TargetName == l).Select(s => s.Assigner).FirstOrDefault(),
                                  Rate = scores.Where(w => w.TargetName == l).Count() == 0 ? 1 : scores.Where(w => w.TargetName == l).Select(s => s.Rate).FirstOrDefault(), //rates.Where(w => w.TargetName == l).Average(s => s.Rate) == null ? 1 : scores.Where(w => w.TargetName == l).Average(s => s.Score),
                                  AssignedScore = scores.Where(w => w.TargetName == l).Count() == 0 ? 10 : scores.Where(w => w.TargetName == l).Select(s => s.Score).FirstOrDefault(),//scores.Where(w => w.TargetName == l).Average(s => s.Score) == null ? 0 : scores.Where(w => w.TargetName == l).Average(s => s.Score),
                                  TeamLeadPerformanceInWeeks = wd.Select(s => new _TeamLeadPerformanceInWeek
                                  {
                                      //FaxOutCount = calls.Where(c => c.Member != null && c.Member.Name == l).GroupBy(c=>c.LeadID).Select(g=>g.FirstOrDefault()).Count(c=>c.CreatedDate >= s && c.CreatedDate < EntityFunctions.AddDays(s, 7)),
                                      FaxOutCount = calls.Where(w => w.Member != null && w.Member.Name == l && w.CreatedDate >= s && w.CreatedDate < EntityFunctions.AddDays(s, 7)
                                          && calls.Any(a => a.CallDate < s && a.LeadID == w.LeadID && a.Member.Name == l && a.ProjectID==w.ProjectID) == false).GroupBy(g => g.LeadID).Count(),
                                      DealsCount = deals.Count(c => c.SignDate >= s && c.SignDate < EntityFunctions.AddDays(s, 7) && c.Sales==l),
                                      StartDate = s,
                                      EndDate = EntityFunctions.AddDays(s, 7).Value,
                                      LeadsCount = leadadds.Count(c => c.Creator == l && c.CreatedDate >= s && c.CreatedDate < EntityFunctions.AddDays(s, 7))
                                  })

                              };
                    return lps;
                }
                return new List<_TeamLeadPerformance>();
            }

            public static IEnumerable<_SalesPerformance> GetSalesPerformances(int month, string fuzzyInput = "")
            {
                var rolelvl = Employee.CurrentRole.Level;

                if (rolelvl == PoliticsInterfaceRequired.LVL)
                {
                    rolelvl = DirectorRequired.LVL;
                }
                if (rolelvl >= SalesRequired.LVL)
                {
                    IQueryable<string> sales = null;
                    var user = Employee.CurrentUserName;
                    var mems  = CH.DB.Members.Where(w => w.IsActivated == true && w.Project.IsActived == true && !w.SalesType.Name.Contains("其他"));
                    if (Employee.EqualToLeader() || Employee.EqualToManager())//版块或者lead查看
                    {
                        sales =mems.Where(w=>w.Project.Manager == user || w.Project.TeamLeader == user).Select(s => s.Name).Distinct();

                    }
                    else if (Employee.EqualToSales())//销售查看
                    {
                        sales = mems.Where(w => w.Name == user).Select(s => s.Name).Distinct();
                    }
                    else if (rolelvl>=SuperManagerRequired.LVL)
                    {
                        sales = mems.Select(s => s.Name).Distinct();
                    }

                    if (sales != null)
                    {


                        var leads = CH.DB.Projects.Where(w => w.IsActived == true && !string.IsNullOrEmpty(w.TeamLeader)).Select(s => s.TeamLeader).Distinct();

                        if (Utl.Utl.DebugModel() != true)
                        {
                            var debugmembers = CH.DB.Members.Where(w => w.Test == true && w.IsActivated==true).Select(s => s.Name).Distinct();
                            sales = sales.Where(w => !debugmembers.Any(a => a == w));
                        }


                        sales = sales.Where(w => !leads.Any(a => a == w) && w.Contains(fuzzyInput));//排除sales中的lead
                        var year = DateTime.Now.Year;
                        var deals = CRM_Logical.GetDeals();
                        var calls = from l in CH.DB.LeadCalls.Where(w => w.LeadCallTypeID != null && w.LeadCallType.Code >= 40 && w.Project.IsActived==true) select l;
                           // calls =from c in calls group c by c.LeadID into g select g.FirstOrDefault();//分组并选择第一个
                       
                        var leadadds = from l in CH.DB.Leads select l;
                        var targets = from t in CH.DB.TargetOfMonthForMembers.Where(w=>w.Project.IsActived==true) select t;
                        var rates = from r in CH.DB.AssignPerformanceRates.Where(w => w.Month == month && w.Year == year) select r;
                        var scores = from r in CH.DB.AssignPerformanceScores.Where(w => w.Month == month && w.Year == year) select r;
                        var durations = MonthDuration.GetMonthInstance(month).WeekDurations.OrderBy(m=>m.StartDate);
                        var wd = durations.Select(s => s.StartDate);
                        var lps = from l in sales
                                  select new _SalesPerformance()
                                  {
                                      RoleLevel = rolelvl,
                                      ID = scores.Where(w => w.TargetName == l).Count() == 0 ? 0 : scores.Where(w => w.TargetName == l).Select(s => s.ID).FirstOrDefault(),
                                      Rate = scores.Where(w => w.TargetName == l).Count() == 0 ? 1 : scores.Where(w => w.TargetName == l).Select(s => s.Rate).FirstOrDefault(),//scores.Where(w => w.TargetName == l).Select(s => s.Rate).FirstOrDefault()==null?1:scores.Where(w => w.TargetName == l).Select(s => s.Rate).FirstOrDefault(),
                                      AssignedScore = scores.Where(w => w.TargetName == l).Count() == 0 ? 10 : scores.Where(w => w.TargetName == l).Select(s => s.Score).FirstOrDefault(),//scores.Where(w => w.TargetName == l).Select(s => s.Score).FirstOrDefault(),//scores.Where(w => w.TargetName == l).Average(s => s.Score) == null ? 0 : scores.Where(w => w.TargetName == l).Average(s => s.Score),
                                      Target = targets.Where(t => t.Member.Name == l && t.StartDate.Month == month).Sum(s => (decimal?)s.CheckIn),
                                      CheckIn = deals.Where(w => w.Sales == l && w.ActualPaymentDate.Value != null && w.ActualPaymentDate.Value.Month == month && w.ActualPaymentDate.Value.Year == DateTime.Now.Year).Sum(s => (decimal?)s.Income),
                                      Name = l,
                                      User = user,
                                      SalesPerformanceInWeeks = wd.Select(s => new _SalesPerformanceInWeek
                                      {
                                          FaxOutCount = calls.Where(w => w.Member != null && w.Member.Name == l && w.CreatedDate >= s && w.CreatedDate < EntityFunctions.AddDays(s, 7)
                                          && calls.Any(a => a.CallDate < s && a.LeadID == w.LeadID && a.Member.Name == l && a.ProjectID == w.ProjectID) == false).GroupBy(g => g.LeadID).Count(),
                                          DealsCount = deals.Count(c => c.Sales == l && c.SignDate >= s && c.SignDate < EntityFunctions.AddDays(s, 7)),
                                          StartDate = s,
                                          EndDate = EntityFunctions.AddDays(s, 7).Value,
                                          LeadsCount = leadadds.Count(c => c.Creator == l && c.CreatedDate >= s && c.CreatedDate < EntityFunctions.AddDays(s, 7))
                                      })
                                  };
                        return lps;
                    }
                }

                return new List<_SalesPerformance>();

            }
            public static IEnumerable<SelectListItem> GetSalesDDL(int month)
            {
                var rolelvl = Employee.CurrentRole.Level;

                if (rolelvl == PoliticsInterfaceRequired.LVL)
                {
                    rolelvl = DirectorRequired.LVL;
                }
                if (rolelvl >= SalesRequired.LVL)
                {
                    List<SelectListItem> selectList = new List<SelectListItem>();
                    IQueryable<string> sales = null;
                    var user = Employee.CurrentUserName;
                    var mems = CH.DB.Members.Where(w => w.IsActivated == true && w.Project.IsActived == true);
                    if (Employee.EqualToLeader() || Employee.EqualToManager())//版块或者lead查看
                    {
                        sales = mems.Where(w => w.Project.Manager == user || w.Project.TeamLeader == user).Select(s => s.Name).Distinct();

                    }
                    else if (Employee.EqualToSales())//销售查看
                    {
                        sales = mems.Where(w => w.Name == user).Select(s => s.Name).Distinct();
                    }
                    else if (rolelvl >= SuperManagerRequired.LVL)
                    {
                        sales = mems.Select(s => s.Name).Distinct();
                    }

                    if (sales != null)
                    {
                        if (Utl.Utl.DebugModel() != true)
                        {
                            var debugmembers = CH.DB.Members.Where(w => w.Test == true && w.IsActivated == true).Select(s => s.Name).Distinct();
                            sales = sales.Where(w => !debugmembers.Any(a => a == w));
                        }
                        
                        foreach (var sale in sales)
                        {
                            SelectListItem selectListItem = new SelectListItem() { Text = sale, Value = sale };
                            selectList.Add(selectListItem);
                        }
                        
                    }
                    return selectList;
                }

                return new List<SelectListItem>();

            }
            #region manager考核的业务逻辑
            public static int CalcItem10Score(int faxcount)
            {
                int result;
                result = 1;
                return result;
            }
            #endregion

            #region lead考核的业务逻辑
            /// <summary>
            /// 当周出单三次或以上，28个faxout达标，否则35个faxout达标
            /// </summary>
            public static int GetLeadFaxoutStandard(int dealscount)
            {
                if (dealscount >= 3)
                    return 28;
                else
                    return 35;
            }

            /// <summary>
            /// 当周出单三次或以上，60个Lead添加达标，否则70个Lead添加达标
            /// </summary>
            public static int GetLeadAddStandard(int dealscount)
            {
                if (dealscount >= 3)
                    return 60;
                else
                    return 70;
            }
            #endregion

            #region sales考核业务逻辑
            /// <summary>
            /// 当周出单三次或以上，sales 40个faxout达标，否则50个faxout达标
            /// </summary>
            internal static int GetSalesFaxoutStandard(int dealscount)
            {
                if (dealscount >= 3)
                    return 40;
                else
                    return 50;
            }
            /// <summary>
            /// 当周出单三次或以上，sales 80个lead达标，否则105个lead达标
            /// </summary>
            internal static int GetSalesAddStandard(int dealscount)
            {
                if (dealscount >= 3)
                    return 80;
                else
                    return 105;
            }
            #endregion

            public static int GetCheckInScore(double? completePercent)
            {
                if (completePercent >= 1.4) return 70;
                else if (completePercent >= 1.2) return 60;
                else if (completePercent >= 1) return 50;
                else if (completePercent >= 0.8) return 40;
                else if (completePercent >= 0.6) return 30;
                return 0;
            }

            public static int GetLeadAddScore(int leadNotQualifiedWeeksCount)
            {
                if (leadNotQualifiedWeeksCount == 0) return 20;
                else if (leadNotQualifiedWeeksCount == 1) return 15;
                else if (leadNotQualifiedWeeksCount == 2) return 10;
                else if (leadNotQualifiedWeeksCount == 3) return 5;
                return 0;
            }

            public static int GetFaxOutScore(int hoursOrFaxNotQualifiedWeeksCount)
            {
                if (hoursOrFaxNotQualifiedWeeksCount == 0) return 20;
                else if (hoursOrFaxNotQualifiedWeeksCount == 1) return 15;
                else if (hoursOrFaxNotQualifiedWeeksCount == 2) return 10;
                else if (hoursOrFaxNotQualifiedWeeksCount == 3) return 5;
                return 0;
            }

            public static _TeamLeadPerformance GetSingleTemaLeadsPerformance(int month)
            {
                var user = Employee.CurrentUserName;

                return GetTeamLeadsPerformances(month).Where(w => w.Name == user).FirstOrDefault();
                //var rolelvl = Employee.CurrentRole.Level;

                //var leads = CH.DB.Projects.Where(w => w.IsActived == true && !string.IsNullOrEmpty(w.TeamLeader)).Select(s => s.TeamLeader).Distinct();
                //leads = leads.Where(w => w == user);

                //var year = DateTime.Now.Year;
                ////var deals = CRM_Logical.GetDeals().Where(w => w.ActualPaymentDate.Value != null && w.ActualPaymentDate.Value.Month == month );
                //var deals = CRM_Logical.GetDeals().Where(w => w.ActualPaymentDate.Value != null && w.ActualPaymentDate.Value.Month == month && w.ActualPaymentDate.Value.Year == year);
                //var calls = from l in CH.DB.LeadCalls.Where(w => w.LeadCallTypeID != null && w.LeadCallType.Code >= 40) select l;
                //var leadadds = from l in CH.DB.Leads select l;
                //var rates = from r in CH.DB.AssignPerformanceRates.Where(w => w.Month == month && w.Year == year) select r;
                //var scores = from r in CH.DB.AssignPerformanceScores.Where(w => w.Month == month && w.Year == year) select r;
                //var wd = MonthDuration.GetMonthInstance(month).WeekDurations.Select(s => s.StartDate);

                //var pids = new List<int>();
                //var ptids = new List<int>();
                //foreach (var c in CH.DB.Projects.Where(w => w.IsActived && w.TeamLeader == user))
                //{
                //    pids.Add(c.ID);
                //}
                //foreach (var target in CH.DB.TargetOfMonths.Where(t => t.Project.TeamLeader == user && t.Project.IsActived == true && t.StartDate.Month == month))
                //{
                //    ptids.Add(target.Project.ID);
                //}
                //var unptids = String.Join(",", pids.Except<int>(ptids));

                //var lps = from l in leads
                //          select new _TeamLeadPerformance()
                //          {
                //              Target = CH.DB.TargetOfMonths.Where(t => t.Project.TeamLeader == l && t.Project.IsActived == true && t.StartDate.Month == month).Sum(s => s.CheckIn),
                //              TargetUnSetProjects = unptids,
                //              CheckIn = deals.Where(d => d.Project.TeamLeader == l).Sum(s => s.Income),
                //              Name = l,
                //              Rate = rates.Where(w => w.TargetName == l).Average(s => s.Rate) == null ? 1 : scores.Where(w => w.TargetName == l).Average(s => s.Score),
                //              AssignedScore = scores.Where(w => w.TargetName == l).Average(s => s.Score) == null ? 0 : scores.Where(w => w.TargetName == l).Average(s => s.Score),
                //              TeamLeadPerformanceInWeeks = wd.Select(s => new _TeamLeadPerformanceInWeek
                //              {
                //                  //FaxOutCount = calls.Where(c => c.Member != null && c.Member.Name == l).GroupBy(c=>c.LeadID).Select(g=>g.FirstOrDefault()).Count(c=>c.CreatedDate >= s && c.CreatedDate < EntityFunctions.AddDays(s, 7)),
                //                  FaxOutCount = calls.Where(w => w.Member != null && w.Member.Name == l && w.CreatedDate >= s && w.CreatedDate < EntityFunctions.AddDays(s, 7)
                //                          && calls.Any(a => a.CallDate < s && a.LeadID == w.LeadID && a.Member.Name == l && a.ProjectID == w.ProjectID) == false).GroupBy(g => g.LeadID).Count(),
                //                  DealsCount = deals.Count(c => c.SignDate >= s && c.SignDate < EntityFunctions.AddDays(s, 7)),
                //                  StartDate = s,
                //                  EndDate = EntityFunctions.AddDays(s, 7).Value,
                //                  LeadsCount = leadadds.Count(c => c.Creator == l && c.CreatedDate >= s && c.CreatedDate < EntityFunctions.AddDays(s, 7))
                //              })

                //          };
                //return lps.FirstOrDefault();

            }

            public static _SalesPerformance GetSingleSalesPerformance(int month)
            {
                return GetSalesPerformances(month, Employee.CurrentUserName).FirstOrDefault();
                //var user = Employee.CurrentUserName;
                //var rolelvl = Employee.CurrentRole.Level;
                //IQueryable<string> sales = null;
                //sales = CH.DB.Members.Where(w => w.IsActivated == true && w.Project.IsActived == true && w.Name == user).Select(s => s.Name).Distinct();

                ////var deals = CRM_Logical.GetDeals();
                //var deals = CRM_Logical.GetDeals(false, null, user, null);
                //var calls = from l in CH.DB.LeadCalls.Where(w => w.LeadCallTypeID != null && w.LeadCallType.Code >= 40) select l;
                //var leadadds = from l in CH.DB.Leads select l;
                //var rates = from r in CH.DB.AssignPerformanceRates.Where(w => w.Month == month && w.Year == DateTime.Now.Year) select r;
                //var scores = from r in CH.DB.AssignPerformanceScores.Where(w => w.Month == month && w.Year == DateTime.Now.Year) select r;
                //var wd = MonthDuration.GetMonthInstance(month).WeekDurations.Select(s => s.StartDate);
                //var lps = from l in sales
                //          select new _SalesPerformance()
                //          {
                //              Target = CH.DB.TargetOfMonthForMembers.Where(t => t.Member.Name == l && t.StartDate.Month == month && t.Project.IsActived == true).Sum(s => s.CheckIn),
                //              CheckIn = deals.Where(w => w.ActualPaymentDate.Value != null && w.ActualPaymentDate.Value.Month == month && w.ActualPaymentDate.Value.Year == DateTime.Now.Year).Sum(s => s.Income),
                //              Name = l,
                //              Rate = rates.Where(w => w.TargetName == l).Average(s => s.Rate) == null ? 1 : scores.Where(w => w.TargetName == l).Average(s => s.Score),
                //              AssignedScore = scores.Where(w => w.TargetName == l).Average(s => s.Score) == null ? 0 : scores.Where(w => w.TargetName == l).Average(s => s.Score),
                //              SalesPerformanceInWeeks = wd.Select(s => new _SalesPerformanceInWeek
                //              {
                //                  FaxOutCount = calls.Where(w => w.Member != null && w.Member.Name == l && w.CreatedDate >= s && w.CreatedDate < EntityFunctions.AddDays(s, 7)
                //                          && calls.Any(a => a.CallDate < s && a.LeadID == w.LeadID && a.Member.Name == l && a.ProjectID == w.ProjectID) == false).GroupBy(g => g.LeadID).Count(),
                //                  DealsCount = deals.Count(c => c.SignDate >= s && c.SignDate < EntityFunctions.AddDays(s, 7)),
                //                  StartDate = s,
                //                  EndDate = EntityFunctions.AddDays(s, 7).Value,
                //                  LeadsCount = leadadds.Count(c => c.Creator == l && c.CreatedDate >= s && c.CreatedDate < EntityFunctions.AddDays(s, 7))
                //              })

                //          };
                //return lps.FirstOrDefault();
            }
        }

        public static class _TargetOfMonth
        {
            public static IQueryable<_TargetOfMonthStatus> GetCurrentMonthProjectTagetStatus()
            {
                var month = DateTime.Now.Month;
                var year = DateTime.Now.Year;
                var ps = from p in CH.DB.Projects.Where(w=>w.IsActived==true && w.Test==null && DateTime.Now < w.ConferenceStartDate) select p;
                var tagets = from t in CH.DB.TargetOfMonths.Where(w=>w.Project.IsActived==true) select t;
                var data = from p in ps
                           select new _TargetOfMonthStatus()
                           {
                                  ProjectName = p.Name_CH,
                                   ProjectCode = p.ProjectCode,
                                   Mangager = p.Manager,
                                  HasTargetOfMonth = tagets.Count(w => w.StartDate.Month == month && w.StartDate.Year == year && w.ProjectID == p.ID)>0,
                                  HasTargetOfWeek = tagets.Count(w => w.StartDate.Month == month && w.StartDate.Year == year && w.ProjectID == p.ID &&
                                      (w.TargetOf1stWeek > 0 || w.TargetOf2ndWeek > 0 || w.TargetOf3rdWeek > 0 
                                      || w.TargetOf4thWeek > 0 || w.TargetOf5thWeek > 0)) > 0

                           };
                return data;
            }
        }

        public static class _Reports
        {

            public static List<ProjectWeekPerformance> GetProjectSingleWeekCheckIn(DateTime? day)
            {
                if (day == null)
                    day = DateTime.Now.AddDays(1); ;
                //day = day.Value.AddDays(25);
                //day = day.Value.AddDays(-7);
                var weekend = day.Value;
                if (day.Value.DayOfWeek == DayOfWeek.Sunday)
                    weekend = day.Value.AddDays(-3);
                else if (day.Value.DayOfWeek == DayOfWeek.Saturday)
                    weekend = day.Value.AddDays(-2);
                else if (day.Value.DayOfWeek == DayOfWeek.Friday)
                    weekend = day.Value.AddDays(-1);
                else if (day.Value.DayOfWeek == DayOfWeek.Thursday)
                    weekend = day.Value.AddDays(-7);
                else if (day.Value.DayOfWeek == DayOfWeek.Wednesday)
                    weekend = day.Value.AddDays(-6);
                else if (day.Value.DayOfWeek == DayOfWeek.Tuesday)
                    weekend = day.Value.AddDays(-5);
                else if (day.Value.DayOfWeek == DayOfWeek.Monday)
                    weekend = day.Value.AddDays(-4);
                //var weekend = day.Value.AddDays(0 - (day.Value.DayOfWeek + 3));//3天表示周五周六周日三天。
                var weekstart = weekend.AddDays(-6);
                var dealins = from d in GetDeals().Where(w => w.SignDate >= weekstart && w.SignDate < weekend) select d;
                var checkins = from d in GetDeals().Where(w => w.ActualPaymentDate >= weekstart && w.ActualPaymentDate < weekend) select d;
                var alldeals = GetDeals();
                //和周目标相关的月
                var targets = from t in CH.DB.TargetOfMonths
                              select t;
                var cs =from p in CH.DB.Projects
                         where p.IsActived == true && (p.Test == null)
                         select new ProjectWeekPerformance()
                         {
                             StartDate = weekstart,
                             EndDate = weekend,
                             Income = checkins.Where(w => w.ProjectID == p.ID).Sum(s => s.Income),
                             Payment = dealins.Where(w => w.ProjectID == p.ID).Sum(s => s.Payment),
                             RMBPayment = dealins.Where(w => w.ProjectID == p.ID && w.Currencytype.Name == "RMB").Sum(s => s.Payment),
                             USDPayment = dealins.Where(w => w.ProjectID == p.ID && w.Currencytype.Name == "USD").Sum(s => s.Payment),
                             ProjectName = p.Name_CH,
                             ProjectID = p.ID,
                             Manager = p.Manager,
                             Leader = p.TeamLeader,
                             LeftDay = EntityFunctions.DiffDays(DateTime.Now, p.ConferenceStartDate).Value,
                             TotalTarget = p.Target,
                             TotalCheckIn = alldeals.Where(w => w.ProjectID == p.ID).Sum(s => s.Income)
                         };
                var rate = CH.DB.CurrencyTypes.Where(r => r.Name == "RMB").Select(r => r.Rate).FirstOrDefault();
                var list = cs.ToList();
                foreach (var l in list)
                {
                    l.rate = rate;
                    targets = targets.Where(w => w.ProjectID == l.ProjectID);
                    decimal? target;//入账目标
                    decimal? dealtarget;//出单目标
                    if (weekend.Month != weekstart.Month)//跨月
                    {
                        DateTime matchdate = DateTime.Parse(weekstart.Year.ToString() + "-" + weekstart.Month.ToString() + "-" + "1");
                        //上月最后一周

                        dealtarget = targets.Where(t => t.StartDate == matchdate).Select(t => t.TargetOf5thWeek).FirstOrDefault();
                        target = targets.Where(t => t.StartDate == matchdate).Select(t => t.CheckInOf5thWeek).FirstOrDefault();
                        matchdate = DateTime.Parse(weekend.Year.ToString() + "-" + weekend.Month.ToString() + "-" + "1");
                        //本月第一周
                        dealtarget = dealtarget + targets.Where(t => t.StartDate == matchdate).Select(t => t.TargetOf1stWeek).FirstOrDefault();
                        target = target + targets.Where(t => t.StartDate == matchdate).Select(t => t.CheckInOf1stWeek).FirstOrDefault();
                        l.DealTarget = dealtarget;
                        l.Target = target;
                    }
                    else
                    {
                        int i = weekend.Day;
                        int weekindex = 0;
                        while (i > 0)
                        {
                            if (DateTime.Parse(weekend.Year.ToString() + "-" + weekend.Month.ToString() + "-" + i.ToString()).DayOfWeek == DayOfWeek.Thursday)
                                weekindex++;
                            i--;
                        }
                        DateTime matchdate = DateTime.Parse(weekend.Year.ToString() + "-" + weekend.Month.ToString() + "-" + "1");
                        if (weekindex == 1)
                        {
                            l.DealTarget = targets.Where(t => t.StartDate == matchdate).Select(t => t.TargetOf1stWeek).FirstOrDefault();
                            l.Target = targets.Where(t => t.StartDate == matchdate).Select(t => t.CheckInOf1stWeek).FirstOrDefault();
                        }
                        else if (weekindex == 2)
                        {
                            l.DealTarget = targets.Where(t => t.StartDate == matchdate).Select(t => t.TargetOf2ndWeek).FirstOrDefault();
                            l.Target = targets.Where(t => t.StartDate == matchdate).Select(t => t.CheckInOf2ndWeek).FirstOrDefault();
                        }
                        else if (weekindex == 3)
                        {
                            l.DealTarget = targets.Where(t => t.StartDate == matchdate).Select(t => t.TargetOf3rdWeek).FirstOrDefault();
                            l.Target = targets.Where(t => t.StartDate == matchdate).Select(t => t.CheckInOf3rdWeek).FirstOrDefault();
                        }
                        else if (weekindex == 4)
                        {
                            l.DealTarget = targets.Where(t => t.StartDate == matchdate).Select(t => t.TargetOf4thWeek).FirstOrDefault();
                            l.Target = targets.Where(t => t.StartDate == matchdate).Select(t => t.CheckInOf4thWeek).FirstOrDefault();
                        }
                        else if (weekindex == 5)
                        {
                            l.DealTarget = targets.Where(t => t.StartDate == matchdate).Select(t => t.TargetOf5thWeek).FirstOrDefault();
                            l.Target = targets.Where(t => t.StartDate == matchdate).Select(t => t.CheckInOf5thWeek).FirstOrDefault();
                        }

                    }
                }
                return list;
                //if (day == null)
                //    day = DateTime.Now; ;
                //day = day.Value.AddDays(-7);
                //var weekstart = day.Value.StartOfWeek();
                //var weekend = day.Value.EndOfWeek().AddDays(1);
                //var dealins = from d in GetDeals().Where(w => w.SignDate >= weekstart && w.SignDate < weekend) select d;
                //var checkins = from d in GetDeals().Where(w => w.ActualPaymentDate >= weekstart && w.ActualPaymentDate < weekend) select d;
                //var alldeals = GetDeals();
                ////和周目标相关的月
                //var targets = from t in GetProjectMonthTargets().Where(w => (w.EndDate >= weekstart && w.EndDate < weekend)
                //                  || (w.StartDate >= weekstart && w.StartDate < weekend) || (w.StartDate >= weekstart && w.EndDate > weekend))
                //              select t;
                ////var ds = from d in dealins
                ////           group d by new { d.ProjectID, d.Project.Name_CH } into grp
                ////           select new ProjectWeekPerformance()
                ////           {
                ////               StartDate = weekstart,
                ////               EndDate = weekend,
                ////               Income = dealins.Where(w => w.ProjectID == grp.Key.ProjectID).Sum(s => s.Payment),
                ////               ProjectName = grp.Key.Name_CH,
                ////               ProjectID = grp.Key.ProjectID
                ////           };
                //var cs =
                //    //from d in checkins
                //    //group d by new { d.ProjectID } into grp
                //         from p in CH.DB.Projects
                //         where p.IsActived == true && (p.Test == null)
                //         select new ProjectWeekPerformance()
                //         {
                //             StartDate = weekstart,
                //             EndDate = weekend,
                //             Income = checkins.Where(w => w.ProjectID == p.ID).Sum(s => s.Income),
                //             Payment = dealins.Where(w => w.ProjectID == p.ID).Sum(s => s.Payment),
                //             ProjectName = p.Name_CH,
                //             ProjectID = p.ID,
                //             Manager = p.Manager,
                //             Leader = p.TeamLeader,
                //             LeftDay = EntityFunctions.DiffDays(DateTime.Now, p.ConferenceStartDate).Value,
                //             TotalTarget = p.Target,
                //             TotalCheckIn = alldeals.Where(w => w.ProjectID == p.ID).Sum(s => s.Income)
                //         };

                //var list = cs.ToList();
                //foreach (var l in list)
                //{
                //    targets = targets.Where(w => w.ProjectID == l.ProjectID);
                //    decimal t = 0;
                //    foreach (var f in targets)
                //    {
                //        t = t + GetWeekTarget(f, weekstart, weekend).Value;
                //    }
                //}

                //return list;
            }

            static private decimal? GetWeekTarget(TargetOfMonth f, DateTime weekstart, DateTime weekend)
            {
                int week = (weekstart - f.StartDate).Days;
                if (week == 0) return f.TargetOf1stWeek;
                if (week == 1) return f.TargetOf2ndWeek;
                if (week == 2) return f.TargetOf3rdWeek;
                if (week == 3) return f.TargetOf4thWeek;
                else return f.TargetOf5thWeek;
            }

            public static List<ProjectWeekPerformance> GetProjectsReportLastweek(DateTime? day)
            {
                if (day == null)
                    day = DateTime.Now.AddDays(1); ;
                //day = day.Value.AddDays(25);
                day = day.Value.AddDays(-17);
                var weekend = day.Value;
                if (day.Value.DayOfWeek == DayOfWeek.Sunday)
                    weekend = day.Value.AddDays(-3);
                else if (day.Value.DayOfWeek == DayOfWeek.Saturday)
                    weekend = day.Value.AddDays(-2);
                else if (day.Value.DayOfWeek == DayOfWeek.Friday)
                    weekend = day.Value.AddDays(-1);
                else if (day.Value.DayOfWeek == DayOfWeek.Thursday)
                    weekend = day.Value.AddDays(-7);
                else if (day.Value.DayOfWeek == DayOfWeek.Wednesday)
                    weekend = day.Value.AddDays(-6);
                else if (day.Value.DayOfWeek == DayOfWeek.Tuesday)
                    weekend = day.Value.AddDays(-5);
                else if (day.Value.DayOfWeek == DayOfWeek.Monday)
                    weekend = day.Value.AddDays(-4);
                //var weekend = day.Value.AddDays(0 - (day.Value.DayOfWeek + 3));//3天表示周五周六周日三天。
                var weekstart = weekend.AddDays(-6);
                var dealins = from d in GetDeals().Where(w => w.SignDate >= weekstart && w.SignDate < weekend) select d;
                var checkins = from d in GetDeals().Where(w => w.ActualPaymentDate >= weekstart && w.ActualPaymentDate < weekend) select d;
                var alldeals = GetDeals();
                //月目标
                var targets = from t in CH.DB.TargetOfMonths
                              select t;
                var cs =
                        from p in CH.DB.Projects
                        where p.IsActived == true && (p.Test == null)
                        select new ProjectWeekPerformance()
                        {
                            StartDate = weekstart,
                            EndDate = weekend,
                            Target = 0,
                            Income = checkins.Where(w => w.ProjectID == p.ID).Sum(s => s.Income),
                            Payment = dealins.Where(w => w.ProjectID == p.ID).Sum(s => s.Payment),
                            RMBPayment = dealins.Where(w => w.ProjectID == p.ID && w.Currencytype.Name == "RMB").Sum(s => s.Payment),
                            USDPayment = dealins.Where(w => w.ProjectID == p.ID && w.Currencytype.Name == "USD").Sum(s => s.Payment),
                            ProjectName = p.Name_CH,
                            ProjectID = p.ID,
                            Manager = p.Manager,
                            Leader = p.TeamLeader,
                            MemberCount = p.Members.Count
                        };

                var rate = CH.DB.CurrencyTypes.Where(r => r.Name == "RMB").Select(r => r.Rate).FirstOrDefault();
                var list = cs.ToList();
                foreach (var l in list)
                {
                    l.rate = rate;
                    targets = targets.Where(w => w.ProjectID == l.ProjectID);
                    decimal? target;//入账目标
                    decimal? dealtarget;//出单目标
                    if (weekend.Month != weekstart.Month)//跨月
                    {
                        DateTime matchdate = DateTime.Parse(weekstart.Year.ToString() + "-" + weekstart.Month.ToString() + "-" + "1");
                        //上月最后一周

                        dealtarget = targets.Where(t => t.StartDate == matchdate).Select(t => t.TargetOf5thWeek).FirstOrDefault();
                        target = targets.Where(t => t.StartDate == matchdate).Select(t => t.CheckInOf5thWeek).FirstOrDefault();
                        matchdate = DateTime.Parse(weekend.Year.ToString() + "-" + weekend.Month.ToString() + "-" + "1");
                        //本月第一周
                        dealtarget = dealtarget + targets.Where(t => t.StartDate == matchdate).Select(t => t.TargetOf1stWeek).FirstOrDefault();
                        target = target + targets.Where(t => t.StartDate == matchdate).Select(t => t.CheckInOf1stWeek).FirstOrDefault();
                        l.DealTarget = dealtarget;
                        l.Target = target;
                    }
                    else
                    {
                        int i = weekend.Day;
                        int weekindex = 0;
                        while (i > 0)
                        {
                            if (DateTime.Parse(weekend.Year.ToString() + "-" + weekend.Month.ToString() + "-" + i.ToString()).DayOfWeek == DayOfWeek.Thursday)
                                weekindex++;
                            i--;
                        }
                        DateTime matchdate = DateTime.Parse(weekend.Year.ToString() + "-" + weekend.Month.ToString() + "-" + "1");
                        if (weekindex == 1)
                        {
                            l.DealTarget = targets.Where(t => t.StartDate == matchdate).Select(t => t.TargetOf1stWeek).FirstOrDefault();
                            l.Target = targets.Where(t => t.StartDate == matchdate).Select(t => t.CheckInOf1stWeek).FirstOrDefault();
                        }
                        else if (weekindex == 2)
                        {
                            l.DealTarget = targets.Where(t => t.StartDate == matchdate).Select(t => t.TargetOf2ndWeek).FirstOrDefault();
                            l.Target = targets.Where(t => t.StartDate == matchdate).Select(t => t.CheckInOf2ndWeek).FirstOrDefault();
                        }
                        else if (weekindex == 3)
                        {
                            l.DealTarget = targets.Where(t => t.StartDate == matchdate).Select(t => t.TargetOf3rdWeek).FirstOrDefault();
                            l.Target = targets.Where(t => t.StartDate == matchdate).Select(t => t.CheckInOf3rdWeek).FirstOrDefault();
                        }
                        else if (weekindex == 4)
                        {
                            l.DealTarget = targets.Where(t => t.StartDate == matchdate).Select(t => t.TargetOf4thWeek).FirstOrDefault();
                            l.Target = targets.Where(t => t.StartDate == matchdate).Select(t => t.CheckInOf4thWeek).FirstOrDefault();
                        }
                        else if (weekindex == 5)
                        {
                            l.DealTarget = targets.Where(t => t.StartDate == matchdate).Select(t => t.TargetOf5thWeek).FirstOrDefault();
                            l.Target = targets.Where(t => t.StartDate == matchdate).Select(t => t.CheckInOf5thWeek).FirstOrDefault();
                        }

                    }
                }
                return list;
            }

            public static List<AjaxProjectsProgressByWeek> GetProjectsProgressByWeek(DateTime? day, int? typeid, string manager = null)
            {
                var ps = CRM_Logical.GetUserInvolveProject();
                if (typeid != null)
                    ps = ps.Where(p => p.ProjectTypeID == typeid).ToList();
                if (!string.IsNullOrEmpty(manager))
                    ps = ps.Where(p => p.Manager == manager).ToList();
                var plist = from p in ps
                           group p by new {p.ID, p.ProjectUnitCode, p.ProjectUnitName, p.ConferenceStartDate } into grp
                           select new
                           {
                               ID=grp.Key.ID,
                               Name = grp.Key.ProjectUnitName,
                               Code = grp.Key.ProjectUnitCode,
                               StartDate = grp.Key.ConferenceStartDate
                           };
                if (day == null)
                    day = DateTime.Now.AddDays(1); ;
                //day = day.Value.AddDays(25);
                var weekend = day.Value;
                if (day.Value.DayOfWeek == DayOfWeek.Sunday)
                    weekend = day.Value.AddDays(-3);
                else if (day.Value.DayOfWeek == DayOfWeek.Saturday)
                    weekend = day.Value.AddDays(-2);
                else if (day.Value.DayOfWeek == DayOfWeek.Friday)
                    weekend = day.Value.AddDays(-1);
                else if (day.Value.DayOfWeek == DayOfWeek.Thursday)
                    weekend = day.Value.AddDays(-7);
                else if (day.Value.DayOfWeek == DayOfWeek.Wednesday)
                    weekend = day.Value.AddDays(-6);
                else if (day.Value.DayOfWeek == DayOfWeek.Tuesday)
                    weekend = day.Value.AddDays(-5);
                else if (day.Value.DayOfWeek == DayOfWeek.Monday)
                    weekend = day.Value.AddDays(-4);
                //var weekend = day.Value.AddDays(0 - (day.Value.DayOfWeek + 3));//3天表示周五周六周日三天。
                var weekstart = weekend.AddDays(-6);
                var lastweekstart = weekstart.AddDays(-6);
                var lastweekend = weekend.AddDays(-6);
                ////本周业绩
                //var dealins = from d in GetDeals().Where(w => w.SignDate >= weekstart && w.SignDate < weekend) select d;
                ////本周入账
                //var checkins = from d in GetDeals().Where(w => w.ActualPaymentDate >= weekstart && w.ActualPaymentDate < weekend) select d;
                var alldeals = GetDeals();
                //月目标
                var targets = from t in CH.DB.TargetOfMonths
                              select t;
                var cs =from l in plist
                        select new AjaxProjectsProgressByWeek()
                        {
                            ProjectUnitName = l.Name,
                            ProjectUnitCode = l.Code,
                            ProjectID=l.ID,
                            LastWeekRMBTotalDealIn = alldeals.Where(w => w.Project.ProjectUnitCode == l.Code && w.Currencytype.Name == "RMB" && w.SignDate >= lastweekstart && w.SignDate < lastweekend).Sum(s => (decimal?)s.Payment),
                            LastWeekUSDTotalDealIn = alldeals.Where(w => w.Project.ProjectUnitCode == l.Code && w.Currencytype.Name == "USD" && w.SignDate >= lastweekstart && w.SignDate < lastweekend).Sum(s => (decimal?)s.Payment),
                            LastWeekCheckIn = alldeals.Where(w => w.Project.ProjectUnitCode == l.Code && w.ActualPaymentDate >= lastweekstart && w.ActualPaymentDate < lastweekend).Sum(s => (decimal?)s.Income),

                            CurrentWeekRMBTotalDealIn = alldeals.Where(w => w.Project.ProjectUnitCode == l.Code && w.Currencytype.Name == "RMB" && w.SignDate >= weekstart && w.SignDate < weekend).Sum(s => (decimal?)s.Payment),
                            CurrentWeekUSDTotalDealIn = alldeals.Where(w => w.Project.ProjectUnitCode == l.Code && w.Currencytype.Name == "USD" && w.SignDate >= weekstart && w.SignDate < weekend).Sum(s => (decimal?)s.Payment),
                            CurrentWeekCheckIn = alldeals.Where(w => w.Project.ProjectUnitCode == l.Code && w.ActualPaymentDate >= weekstart && w.ActualPaymentDate < weekend).Sum(s => (decimal?)s.Income),
                        };

                var rate = CH.DB.CurrencyTypes.Where(r => r.Name == "USD").Select(r => r.Rate).FirstOrDefault();
                var list = cs.ToList();
                foreach (var l in list)
                {
                    l.rate = rate;
                    targets = targets.Where(w => w.ProjectID == l.ProjectID);
                    decimal? checkintarget;//入账目标
                    decimal? dealintarget;//业绩目标
                    if (weekend.Month != weekstart.Month)//跨月
                    {
                        //处理本周目标
                        DateTime matchdate = DateTime.Parse(weekstart.Year.ToString() + "-" + weekstart.Month.ToString() + "-" + "1");
                        //上月最后一周
                        dealintarget = targets.Where(t => t.StartDate == matchdate).Select(t => t.TargetOf5thWeek).FirstOrDefault();
                        checkintarget = targets.Where(t => t.StartDate == matchdate).Select(t => t.CheckInOf5thWeek).FirstOrDefault();
                        matchdate = DateTime.Parse(weekend.Year.ToString() + "-" + weekend.Month.ToString() + "-" + "1");
                        //本月第一周
                        dealintarget = dealintarget + targets.Where(t => t.StartDate == matchdate).Select(t => t.TargetOf1stWeek).FirstOrDefault();
                        checkintarget = checkintarget + targets.Where(t => t.StartDate == matchdate).Select(t => t.CheckInOf1stWeek).FirstOrDefault();
                        l.CurrentWeekDealInTarget = dealintarget;
                        l.CurrentWeekCheckInTarget = checkintarget;
                        //处理上周目标
                        matchdate = DateTime.Parse(lastweekstart.Year.ToString() + "-" + lastweekstart.Month.ToString() + "-" + "1");
                        //上月最后一周
                        dealintarget = targets.Where(t => t.StartDate == matchdate).Select(t => t.TargetOf5thWeek).FirstOrDefault();
                        checkintarget = targets.Where(t => t.StartDate == matchdate).Select(t => t.CheckInOf5thWeek).FirstOrDefault();
                        matchdate = DateTime.Parse(lastweekend.Year.ToString() + "-" + lastweekend.Month.ToString() + "-" + "1");
                        //本月第一周
                        dealintarget = dealintarget + targets.Where(t => t.StartDate == matchdate).Select(t => t.TargetOf1stWeek).FirstOrDefault();
                        checkintarget = checkintarget + targets.Where(t => t.StartDate == matchdate).Select(t => t.CheckInOf1stWeek).FirstOrDefault();
                        l.LastWeekDealInTarget= dealintarget;
                        l.LastWeekCheckInTarget = checkintarget;
                    }
                    else
                    {
                        int i = weekend.Day;
                        int weekindex = 0;
                        while (i > 0)
                        {
                            if (DateTime.Parse(weekend.Year.ToString() + "-" + weekend.Month.ToString() + "-" + i.ToString()).DayOfWeek == DayOfWeek.Thursday)
                                weekindex++;
                            i--;
                        }
                        DateTime matchdate = DateTime.Parse(weekend.Year.ToString() + "-" + weekend.Month.ToString() + "-" + "1");
                        if (weekindex == 1)
                        {
                            l.CurrentWeekDealInTarget = targets.Where(t => t.StartDate == matchdate).Select(t => t.TargetOf1stWeek).FirstOrDefault();
                            l.CurrentWeekCheckInTarget = targets.Where(t => t.StartDate == matchdate).Select(t => t.CheckInOf1stWeek).FirstOrDefault();
                        }
                        else if (weekindex == 2)
                        {
                            l.CurrentWeekDealInTarget = targets.Where(t => t.StartDate == matchdate).Select(t => t.TargetOf2ndWeek).FirstOrDefault();
                            l.CurrentWeekCheckInTarget = targets.Where(t => t.StartDate == matchdate).Select(t => t.CheckInOf2ndWeek).FirstOrDefault();
                        }
                        else if (weekindex == 3)
                        {
                            l.CurrentWeekDealInTarget = targets.Where(t => t.StartDate == matchdate).Select(t => t.TargetOf3rdWeek).FirstOrDefault();
                            l.CurrentWeekCheckInTarget = targets.Where(t => t.StartDate == matchdate).Select(t => t.CheckInOf3rdWeek).FirstOrDefault();
                        }
                        else if (weekindex == 4)
                        {
                            l.CurrentWeekDealInTarget = targets.Where(t => t.StartDate == matchdate).Select(t => t.TargetOf4thWeek).FirstOrDefault();
                            l.CurrentWeekCheckInTarget = targets.Where(t => t.StartDate == matchdate).Select(t => t.CheckInOf4thWeek).FirstOrDefault();
                        }
                        else if (weekindex == 5)
                        {
                            l.CurrentWeekDealInTarget = targets.Where(t => t.StartDate == matchdate).Select(t => t.TargetOf5thWeek).FirstOrDefault();
                            l.CurrentWeekCheckInTarget = targets.Where(t => t.StartDate == matchdate).Select(t => t.CheckInOf5thWeek).FirstOrDefault();
                        }
                        //处理上周目标
                        i = lastweekend.Day;
                        weekindex = 0;
                        while (i > 0)
                        {
                            if (DateTime.Parse(lastweekend.Year.ToString() + "-" + lastweekend.Month.ToString() + "-" + i.ToString()).DayOfWeek == DayOfWeek.Thursday)
                                weekindex++;
                            i--;
                        }
                        matchdate = DateTime.Parse(lastweekend.Year.ToString() + "-" + lastweekend.Month.ToString() + "-" + "1");
                        if (weekindex == 1)
                        {
                            l.LastWeekDealInTarget = targets.Where(t => t.StartDate == matchdate).Select(t => t.TargetOf1stWeek).FirstOrDefault();
                            l.LastWeekCheckInTarget = targets.Where(t => t.StartDate == matchdate).Select(t => t.CheckInOf1stWeek).FirstOrDefault();
                        }
                        else if (weekindex == 2)
                        {
                            l.LastWeekDealInTarget = targets.Where(t => t.StartDate == matchdate).Select(t => t.TargetOf2ndWeek).FirstOrDefault();
                            l.LastWeekCheckInTarget = targets.Where(t => t.StartDate == matchdate).Select(t => t.CheckInOf2ndWeek).FirstOrDefault();
                        }
                        else if (weekindex == 3)
                        {
                            l.LastWeekDealInTarget = targets.Where(t => t.StartDate == matchdate).Select(t => t.TargetOf3rdWeek).FirstOrDefault();
                            l.LastWeekCheckInTarget = targets.Where(t => t.StartDate == matchdate).Select(t => t.CheckInOf3rdWeek).FirstOrDefault();
                        }
                        else if (weekindex == 4)
                        {
                            l.LastWeekDealInTarget = targets.Where(t => t.StartDate == matchdate).Select(t => t.TargetOf4thWeek).FirstOrDefault();
                            l.LastWeekCheckInTarget = targets.Where(t => t.StartDate == matchdate).Select(t => t.CheckInOf4thWeek).FirstOrDefault();
                        }
                        else if (weekindex == 5)
                        {
                            l.LastWeekDealInTarget = targets.Where(t => t.StartDate == matchdate).Select(t => t.TargetOf5thWeek).FirstOrDefault();
                            l.LastWeekCheckInTarget = targets.Where(t => t.StartDate == matchdate).Select(t => t.CheckInOf5thWeek).FirstOrDefault();
                        }
                    }
                }
                return list;
            }

            public static List<AjaxMemberProjectsProgressByWeek> GetMemberProjectsProgressByWeek(DateTime? day, int? typeid, string manager = null)
            {
                var ps = CRM_Logical.GetUserInvolveProject();
                if (typeid != null)
                    ps = ps.Where(p => p.ProjectTypeID == typeid).ToList();
                if (!string.IsNullOrEmpty(manager))
                    ps = ps.Where(p => p.Manager == manager).ToList();
                //IEnumerable<int> idList = ps.Select(o => o.ID);
                //var plist = (from p in idList
                //               join m in CH.DB.Members on p equals m.ProjectID
                //               select new
                //               {
                //                   Sales = m.Name,
                //                   ProjectID = p,
                //                   ProjectUnitName = m.Project.ProjectUnitName,
                //                   ConferenceStartDate = m.Project.ConferenceStartDate
                //               }).ToList();
                IEnumerable<int> idList = ps.Select(o => o.ID);
                var deals = from d in CH.DB.Deals where idList.Contains((int)d.ProjectID) select d;
                
                if (day == null)
                    day = DateTime.Now.AddDays(1); ;
                //day = day.Value.AddDays(25);
                var weekend = day.Value;
                if (day.Value.DayOfWeek == DayOfWeek.Sunday)
                    weekend = day.Value.AddDays(-3);
                else if (day.Value.DayOfWeek == DayOfWeek.Saturday)
                    weekend = day.Value.AddDays(-2);
                else if (day.Value.DayOfWeek == DayOfWeek.Friday)
                    weekend = day.Value.AddDays(-1);
                else if (day.Value.DayOfWeek == DayOfWeek.Thursday)
                    weekend = day.Value.AddDays(-7);
                else if (day.Value.DayOfWeek == DayOfWeek.Wednesday)
                    weekend = day.Value.AddDays(-6);
                else if (day.Value.DayOfWeek == DayOfWeek.Tuesday)
                    weekend = day.Value.AddDays(-5);
                else if (day.Value.DayOfWeek == DayOfWeek.Monday)
                    weekend = day.Value.AddDays(-4);
                //var weekend = day.Value.AddDays(0 - (day.Value.DayOfWeek + 3));//3天表示周五周六周日三天。
                var weekstart = weekend.AddDays(-6);
                var lastweekstart = weekstart.AddDays(-6);
                var lastweekend = weekend.AddDays(-6);
                ////本周业绩
                //var dealins = from d in GetDeals().Where(w => w.SignDate >= weekstart && w.SignDate < weekend) select d;
                ////本周入账
                //var checkins = from d in GetDeals().Where(w => w.ActualPaymentDate >= weekstart && w.ActualPaymentDate < weekend) select d;
                //var alldeals = GetDeals();
                //月目标
                var targets = from t in CH.DB.TargetOfMonthForMembers
                              select t;
                //var list = from d in deals
                //           group d by new { d.Sales,d.ProjectID,d.Project.ProjectUnitName,d.Project.ConferenceStartDate } into grp
                var cs = from d in deals
                         group d by new { d.Sales,d.ProjectID,d.Project.ProjectUnitName,d.Project.ConferenceStartDate } into grp
                         select new AjaxMemberProjectsProgressByWeek()
                         {
                             Member = grp.Key.Sales,
                             ProjectUnitName = grp.Key.ProjectUnitName,
                             LastWeekRMBTotalDealIn = deals.Where(w => w.Project.ID == grp.Key.ProjectID && w.Sales == grp.Key.Sales && w.Currencytype.Name == "RMB" && w.SignDate >= lastweekstart && w.SignDate < lastweekend).Sum(s => (decimal?)s.Payment),
                             LastWeekUSDTotalDealIn = deals.Where(w => w.Project.ID == grp.Key.ProjectID && w.Sales == grp.Key.Sales && w.Currencytype.Name == "USD" && w.SignDate >= lastweekstart && w.SignDate < lastweekend).Sum(s => (decimal?)s.Payment),
                             LastWeekCheckIn = deals.Where(w => w.Project.ID == grp.Key.ProjectID && w.Sales == grp.Key.Sales && w.ActualPaymentDate >= lastweekstart && w.ActualPaymentDate < lastweekend).Sum(s => (decimal?)s.Income),

                             CurrentWeekRMBTotalDealIn = deals.Where(w => w.Project.ID == grp.Key.ProjectID && w.Sales == grp.Key.Sales && w.Currencytype.Name == "RMB" && w.SignDate >= weekstart && w.SignDate < weekend).Sum(s => (decimal?)s.Payment),
                             CurrentWeekUSDTotalDealIn = deals.Where(w => w.Project.ID == grp.Key.ProjectID && w.Sales == grp.Key.Sales && w.Currencytype.Name == "USD" && w.SignDate >= weekstart && w.SignDate < weekend).Sum(s => (decimal?)s.Payment),
                             CurrentWeekCheckIn = deals.Where(w => w.Project.ID == grp.Key.ProjectID && w.Sales == grp.Key.Sales && w.ActualPaymentDate >= weekstart && w.ActualPaymentDate < weekend).Sum(s => (decimal?)s.Income),
                         };
                //var cs = from l in plist
                //         select new AjaxMemberProjectsProgressByWeek()
                //         {
                //             Member = l.Sales,
                //             ProjectUnitName = l.ProjectUnitName,
                //             LastWeekRMBTotalDealIn = alldeals.Where(w => w.Project.ID == l.ProjectID && w.Sales==l.Sales && w.Currencytype.Name == "RMB" && w.SignDate >= lastweekstart && w.SignDate < lastweekend).Count()==0?0: alldeals.Where(w => w.Project.ID == l.ProjectID && w.Sales==l.Sales && w.Currencytype.Name == "RMB" && w.SignDate >= lastweekstart && w.SignDate < lastweekend).Sum(s => (decimal?)s.Payment),
                //             LastWeekUSDTotalDealIn = alldeals.Where(w => w.Project.ID == l.ProjectID && w.Sales == l.Sales && w.Currencytype.Name == "USD" && w.SignDate >= lastweekstart && w.SignDate < lastweekend).Count()==0?0: alldeals.Where(w => w.Project.ID == l.ProjectID && w.Sales == l.Sales && w.Currencytype.Name == "USD" && w.SignDate >= lastweekstart && w.SignDate < lastweekend).Sum(s => (decimal?)s.Payment),
                //             LastWeekCheckIn = alldeals.Where(w => w.Project.ID == l.ProjectID && w.Sales == l.Sales && w.ActualPaymentDate >= lastweekstart && w.ActualPaymentDate < lastweekend).Count()==0?0: alldeals.Where(w => w.Project.ID == l.ProjectID && w.Sales == l.Sales && w.ActualPaymentDate >= lastweekstart && w.ActualPaymentDate < lastweekend).Sum(s => (decimal?)s.Income),

                //             CurrentWeekRMBTotalDealIn = alldeals.Where(w => w.Project.ID == l.ProjectID && w.Sales == l.Sales && w.Currencytype.Name == "RMB" && w.SignDate >= weekstart && w.SignDate < weekend).Count()==0?0: alldeals.Where(w => w.Project.ID == l.ProjectID && w.Sales == l.Sales && w.Currencytype.Name == "RMB" && w.SignDate >= weekstart && w.SignDate < weekend).Sum(s => (decimal?)s.Payment),
                //             CurrentWeekUSDTotalDealIn = alldeals.Where(w => w.Project.ID == l.ProjectID && w.Sales == l.Sales && w.Currencytype.Name == "USD" && w.SignDate >= weekstart && w.SignDate < weekend).Count()==0?0: alldeals.Where(w => w.Project.ID == l.ProjectID && w.Sales == l.Sales && w.Currencytype.Name == "USD" && w.SignDate >= weekstart && w.SignDate < weekend).Sum(s => (decimal?)s.Payment),
                //             CurrentWeekCheckIn = alldeals.Where(w => w.Project.ID == l.ProjectID && w.Sales == l.Sales && w.ActualPaymentDate >= weekstart && w.ActualPaymentDate < weekend).Count()==0?0: alldeals.Where(w => w.Project.ID == l.ProjectID && w.Sales == l.Sales && w.ActualPaymentDate >= weekstart && w.ActualPaymentDate < weekend).Sum(s => (decimal?)s.Income),
                //         };
                var rate = CH.DB.CurrencyTypes.Where(r => r.Name == "USD").Select(r => r.Rate).FirstOrDefault();
                cs = cs.OrderBy(d => d.Member);
                var list = cs.ToList();
                foreach (var l in list)
                {
                    l.rate = rate;
                    targets = targets.Where(w => w.ProjectID == l.ProjectID);
                    decimal? checkintarget;//入账目标
                    decimal? dealintarget;//业绩目标
                    if (weekend.Month != weekstart.Month)//跨月
                    {
                        //处理本周目标
                        DateTime matchdate = DateTime.Parse(weekstart.Year.ToString() + "-" + weekstart.Month.ToString() + "-" + "1");
                        //上月最后一周
                        dealintarget = targets.Where(t => t.StartDate == matchdate && t.Member.Name==l.Member).Count()==0?0:targets.Where(t => t.StartDate == matchdate && t.Member.Name==l.Member).Select(t => t.TargetOf5thWeek).FirstOrDefault();
                        checkintarget = targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Count()==0?0:targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Select(t => t.CheckInOf5thWeek).FirstOrDefault();
                        matchdate = DateTime.Parse(weekend.Year.ToString() + "-" + weekend.Month.ToString() + "-" + "1");
                        //本月第一周
                        dealintarget = dealintarget + targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Count()==0?0:targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Select(t => t.TargetOf1stWeek).FirstOrDefault();
                        checkintarget = checkintarget + targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Count()==0?0:targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Select(t => t.CheckInOf1stWeek).FirstOrDefault();
                        l.CurrentWeekDealInTarget = dealintarget;
                        l.CurrentWeekCheckInTarget = checkintarget;
                        //处理上周目标
                        matchdate = DateTime.Parse(lastweekstart.Year.ToString() + "-" + lastweekstart.Month.ToString() + "-" + "1");
                        //上月最后一周
                        dealintarget = targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Count()==0?0:targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Select(t => t.TargetOf5thWeek).FirstOrDefault();
                        checkintarget = targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Count()==0?0:targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Select(t => t.CheckInOf5thWeek).FirstOrDefault();
                        matchdate = DateTime.Parse(lastweekend.Year.ToString() + "-" + lastweekend.Month.ToString() + "-" + "1");
                        //本月第一周
                        dealintarget = dealintarget + targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Count()==0?0:targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Select(t => t.TargetOf1stWeek).FirstOrDefault();
                        checkintarget = checkintarget + targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Count()==0?0:targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Select(t => t.CheckInOf1stWeek).FirstOrDefault();
                        l.LastWeekDealInTarget = dealintarget;
                        l.LastWeekCheckInTarget = checkintarget;
                    }
                    else
                    {
                        int i = weekend.Day;
                        int weekindex = 0;
                        while (i > 0)
                        {
                            if (DateTime.Parse(weekend.Year.ToString() + "-" + weekend.Month.ToString() + "-" + i.ToString()).DayOfWeek == DayOfWeek.Thursday)
                                weekindex++;
                            i--;
                        }
                        DateTime matchdate = DateTime.Parse(weekend.Year.ToString() + "-" + weekend.Month.ToString() + "-" + "1");
                        if (weekindex == 1)
                        {
                            l.CurrentWeekDealInTarget = targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Count()==0?0:targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Select(t => t.TargetOf1stWeek).FirstOrDefault();
                            l.CurrentWeekCheckInTarget = targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Count() == 0 ? 0 : targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Select(t => t.CheckInOf1stWeek).FirstOrDefault();
                        }
                        else if (weekindex == 2)
                        {
                            l.CurrentWeekDealInTarget = targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Count() == 0 ? 0 : targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Select(t => t.TargetOf2ndWeek).FirstOrDefault();
                            l.CurrentWeekCheckInTarget = targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Count() == 0 ? 0 : targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Select(t => t.CheckInOf2ndWeek).FirstOrDefault();
                        }
                        else if (weekindex == 3)
                        {
                            l.CurrentWeekDealInTarget = targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Count() == 0 ? 0 : targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Select(t => t.TargetOf3rdWeek).FirstOrDefault();
                            l.CurrentWeekCheckInTarget = targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Count() == 0 ? 0 : targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Select(t => t.CheckInOf3rdWeek).FirstOrDefault();
                        }
                        else if (weekindex == 4)
                        {
                            l.CurrentWeekDealInTarget = targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Count() == 0 ? 0 : targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Select(t => t.TargetOf4thWeek).FirstOrDefault();
                            l.CurrentWeekCheckInTarget = targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Count() == 0 ? 0 : targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Select(t => t.CheckInOf4thWeek).FirstOrDefault();
                        }
                        else if (weekindex == 5)
                        {
                            l.CurrentWeekDealInTarget = targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Count() == 0 ? 0 : targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Select(t => t.TargetOf5thWeek).FirstOrDefault();
                            l.CurrentWeekCheckInTarget = targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Count() == 0 ? 0 : targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Select(t => t.CheckInOf5thWeek).FirstOrDefault();
                        }
                        //处理上周目标
                        i = lastweekend.Day;
                        weekindex = 0;
                        while (i > 0)
                        {
                            if (DateTime.Parse(lastweekend.Year.ToString() + "-" + lastweekend.Month.ToString() + "-" + i.ToString()).DayOfWeek == DayOfWeek.Thursday)
                                weekindex++;
                            i--;
                        }
                        matchdate = DateTime.Parse(lastweekend.Year.ToString() + "-" + lastweekend.Month.ToString() + "-" + "1");
                        if (weekindex == 1)
                        {
                            l.LastWeekDealInTarget = targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Count() == 0 ? 0 : targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Select(t => t.TargetOf1stWeek).FirstOrDefault();
                            l.LastWeekCheckInTarget = targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Count() == 0 ? 0 : targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Select(t => t.CheckInOf1stWeek).FirstOrDefault();
                        }
                        else if (weekindex == 2)
                        {
                            l.LastWeekDealInTarget = targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Count() == 0 ? 0 : targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Select(t => t.TargetOf2ndWeek).FirstOrDefault();
                            l.LastWeekCheckInTarget = targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Count() == 0 ? 0 : targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Select(t => t.CheckInOf2ndWeek).FirstOrDefault();
                        }
                        else if (weekindex == 3)
                        {
                            l.LastWeekDealInTarget = targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Count() == 0 ? 0 : targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Select(t => t.TargetOf3rdWeek).FirstOrDefault();
                            l.LastWeekCheckInTarget = targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Count() == 0 ? 0 : targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Select(t => t.CheckInOf3rdWeek).FirstOrDefault();
                        }
                        else if (weekindex == 4)
                        {
                            l.LastWeekDealInTarget = targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Count() == 0 ? 0 : targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Select(t => t.TargetOf4thWeek).FirstOrDefault();
                            l.LastWeekCheckInTarget = targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Count() == 0 ? 0 : targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Select(t => t.CheckInOf4thWeek).FirstOrDefault();
                        }
                        else if (weekindex == 5)
                        {
                            l.LastWeekDealInTarget = targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Count() == 0 ? 0 : targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Select(t => t.TargetOf5thWeek).FirstOrDefault();
                            l.LastWeekCheckInTarget = targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Count() == 0 ? 0 : targets.Where(t => t.StartDate == matchdate && t.Member.Name == l.Member).Select(t => t.CheckInOf5thWeek).FirstOrDefault();
                        }
                    }
                }
                return list;
            }

            public static IQueryable<AjaxProjectCheckInByMonth> GetProjectsCheckInByMonth()
            {
                var yeaerstart = new DateTime(DateTime.Now.Year, 1, 1);

                var ps = CRM_Logical.GetUserInvolveProject();
                var pid = ps.Select(s => s.ID);
                var targets = CH.DB.TargetOfMonths.Where(w => w.Project.IsActived == true);
                var currentmonthstart = DateTime.Now.StartOfMonth();
                var currentmonthend = DateTime.Now.EndOfMonth().AddDays(1);

                var onemonthbeforeend = currentmonthstart.AddDays(-1);
                var onemonthbeforestart = onemonthbeforeend.StartOfMonth();

                var twomonthbeforeend = onemonthbeforestart.AddDays(-1);
                var twomonthbeforestart = twomonthbeforeend.StartOfMonth();

                var threemonthbeforeend = twomonthbeforestart.AddDays(-1);
                var threemonthbeforestart = threemonthbeforeend.StartOfMonth();

                var fourmonthbeforeend = threemonthbeforestart.AddDays(-1);
                var fourmonthbeforestart = fourmonthbeforeend.StartOfMonth();

                var fifthmonthbeforeend = fourmonthbeforestart.AddDays(-1);
                var fifthmonthbeforestart = fifthmonthbeforeend.StartOfMonth();

                var deals = CRM_Logical.GetDeals(true);
                var list = from d in deals
                           group d by new { d.Project.ProjectUnitCode, d.Project.ProjectUnitName } into grp
                           select new AjaxProjectCheckInByMonth
                           {

                               ProjectName = grp.Key.ProjectUnitName,
                               ProjectCode = grp.Key.ProjectUnitCode,
                               CurrentMonthChickIn = deals.Where(w => w.Project.ProjectUnitCode == grp.Key.ProjectUnitCode && w.ActualPaymentDate >= currentmonthstart && w.ActualPaymentDate < currentmonthend).Sum(s => (decimal?)s.Income),
                               OneMonthBeforeChickIn = deals.Where(w => w.Project.ProjectUnitCode == grp.Key.ProjectUnitCode && w.ActualPaymentDate >= onemonthbeforestart && w.ActualPaymentDate < currentmonthstart).Sum(s => s.Income),
                               TwoMonthBeforeChickIn = deals.Where(w => w.Project.ProjectUnitCode == grp.Key.ProjectUnitCode && w.ActualPaymentDate >= twomonthbeforestart && w.ActualPaymentDate < onemonthbeforestart).Sum(s => s.Income),
                               ThreeMonthBeforeChickIn = deals.Where(w => w.Project.ProjectUnitCode == grp.Key.ProjectUnitCode && w.ActualPaymentDate >= threemonthbeforestart && w.ActualPaymentDate < twomonthbeforestart).Sum(s => s.Income),
                               FourthMonthBeforeChickIn = deals.Where(w => w.Project.ProjectUnitCode == grp.Key.ProjectUnitCode && w.ActualPaymentDate >= fourmonthbeforestart && w.ActualPaymentDate < threemonthbeforestart).Sum(s => s.Income),
                               FifthMonthBeforeChickIn = deals.Where(w => w.Project.ProjectUnitCode == grp.Key.ProjectUnitCode && w.ActualPaymentDate >= fifthmonthbeforestart && w.ActualPaymentDate < fourmonthbeforestart).Sum(s => s.Income),
                               CurrentMonthTarget = targets.Where(w => w.Project.ProjectUnitCode == grp.Key.ProjectUnitCode && w.StartDate.Month == currentmonthstart.Month).Sum(s => (decimal?)s.CheckIn),
                               OneMonthBeforeTarget = targets.Where(w => w.Project.ProjectUnitCode == grp.Key.ProjectUnitCode && w.StartDate.Month == onemonthbeforestart.Month).Sum(s => s.CheckIn),
                               TwoMonthBeforeTarget = targets.Where(w => w.Project.ProjectUnitCode == grp.Key.ProjectUnitCode && w.StartDate.Month == twomonthbeforestart.Month).Sum(s => s.CheckIn),
                               ThreeMonthBeforeTarget = targets.Where(w => w.Project.ProjectUnitCode == grp.Key.ProjectUnitCode && w.StartDate.Month == threemonthbeforestart.Month).Sum(s => s.CheckIn),
                               FourthMonthBeforeTarget = targets.Where(w => w.Project.ProjectUnitCode == grp.Key.ProjectUnitCode && w.StartDate.Month == fourmonthbeforestart.Month).Sum(s => s.CheckIn),
                               FifthMonthBeforeTarget = targets.Where(w => w.Project.ProjectUnitCode == grp.Key.ProjectUnitCode && w.StartDate.Month == fifthmonthbeforestart.Month).Sum(s => s.CheckIn),
                               ProjectLines = (from ds in deals.Where(w => w.Project.ProjectUnitCode == grp.Key.ProjectUnitCode && w.ActualPaymentDate != null && w.ActualPaymentDate > yeaerstart)
                                               group ds by new { ds.ActualPaymentDate.Value.Month } into grps
                                               select new AjaxProjectPerformanceInMonth()
                                               {
                                                   Month = grps.Key.Month,
                                                   CheckIn = grps.Sum(s => (decimal?)s.Income),
                                                   CheckinTarget = targets.Where(w => w.StartDate.Month == grps.Key.Month && w.Project.ProjectUnitCode == grp.Key.ProjectUnitCode).Sum(s => (decimal?)s.CheckIn)
                                               })

                           };

                return list.Where(w => !string.IsNullOrEmpty(w.ProjectName) || !string.IsNullOrEmpty(w.ProjectCode));
            }

            public static IQueryable<AjaxProjectCheckInMonthByProjectType> GetProjectsPerformanceInProjectType()
            {
                var pid = CRM_Logical.GetUserInvolveProject().Select(s => s.ID);
                var targets = CH.DB.TargetOfMonths.Where(w => w.Project.IsActived == true);
                var deals = CRM_Logical.GetDeals(true);
                var list = from d in deals
                           group d by new { d.ActualPaymentDate.Value.Month, d.ActualPaymentDate.Value.Year } into grp
                           select new AjaxProjectCheckInMonthByProjectType
                           {
                               Year = grp.Key.Year,
                               Month = grp.Key.Month,
                               EventsCheckIn = deals.Where(w => w.Project.ProjectTypeID == 1).Sum(s => s.Income),//1为events
                               KaoChaCheckIn = deals.Where(w => w.Project.ProjectTypeID == 2).Sum(s => s.Income),//2为考察
                               MagazineCheckIn = deals.Where(w => w.Project.ProjectTypeID == 3).Sum(s => s.Income),//3为杂志
                               QingDaoSubComanyCheckIn = deals.Where(w => w.Project.ProjectTypeID == 3).Sum(s => s.Income),//4为青岛分公司
                               EventsTarget = targets.Where(w => w.Project.ProjectTypeID == 1).Sum(s => s.CheckIn),
                               KaoChaTarget = targets.Where(w => w.Project.ProjectTypeID == 2).Sum(s => s.CheckIn),
                               MagazineTarget = targets.Where(w => w.Project.ProjectTypeID == 3).Sum(s => s.CheckIn),
                               QingDaoSubComanyTarget = targets.Where(w => w.Project.ProjectTypeID == 4).Sum(s => s.CheckIn),
                           };
                return list;
            }

            public static IQueryable<AjaxEmployeeCheckInByMonth> GetEmployeesCheckInByMonth()
            {
                var yeaerstart = new DateTime(DateTime.Now.Year, 1, 1);

                var ps = CRM_Logical.GetUserInvolveProject();
                var pid = ps.Select(s => s.ID);
                var targets = CH.DB.TargetOfMonthForMembers.Where(w => w.Project.IsActived == true);
                var currentmonthstart = DateTime.Now.StartOfMonth();
                var currentmonthend = DateTime.Now.EndOfMonth().AddDays(1);

                var onemonthbeforeend = currentmonthstart.AddDays(-1);
                var onemonthbeforestart = onemonthbeforeend.StartOfMonth();

                var twomonthbeforeend = onemonthbeforestart.AddDays(-1);
                var twomonthbeforestart = twomonthbeforeend.StartOfMonth();

                var threemonthbeforeend = twomonthbeforestart.AddDays(-1);
                var threemonthbeforestart = threemonthbeforeend.StartOfMonth();

                var fourmonthbeforeend = threemonthbeforestart.AddDays(-1);
                var fourmonthbeforestart = fourmonthbeforeend.StartOfMonth();

                var fifthmonthbeforeend = fourmonthbeforestart.AddDays(-1);
                var fifthmonthbeforestart = fifthmonthbeforeend.StartOfMonth();

                var alivemembers = CH.DB.Members.Where(w => w.IsActivated == true).Select(s => s.Name).Distinct();
                var deals = CRM_Logical.GetDeals(true);
                var list = from d in deals
                           group d by new { d.Sales } into grp
                           select new AjaxEmployeeCheckInByMonth
                           {
                               Name = grp.Key.Sales,
                               CurrentMonthChickIn = deals.Where(w => w.Sales == grp.Key.Sales && w.ActualPaymentDate >= currentmonthstart && w.ActualPaymentDate < currentmonthend).Sum(s => (decimal?)s.Income),
                               OneMonthBeforeChickIn = deals.Where(w => w.Sales == grp.Key.Sales && w.ActualPaymentDate >= onemonthbeforestart && w.ActualPaymentDate < currentmonthstart).Sum(s => s.Income),
                               TwoMonthBeforeChickIn = deals.Where(w => w.Sales == grp.Key.Sales && w.ActualPaymentDate >= twomonthbeforestart && w.ActualPaymentDate < onemonthbeforestart).Sum(s => s.Income),
                               ThreeMonthBeforeChickIn = deals.Where(w => w.Sales == grp.Key.Sales && w.ActualPaymentDate >= threemonthbeforestart && w.ActualPaymentDate < twomonthbeforestart).Sum(s => s.Income),
                               FourthMonthBeforeChickIn = deals.Where(w => w.Sales == grp.Key.Sales && w.ActualPaymentDate >= fourmonthbeforestart && w.ActualPaymentDate < threemonthbeforestart).Sum(s => s.Income),
                               FifthMonthBeforeChickIn = deals.Where(w => w.Sales == grp.Key.Sales && w.ActualPaymentDate >= fifthmonthbeforestart && w.ActualPaymentDate < fourmonthbeforestart).Sum(s => s.Income),
                               CurrentMonthTarget = targets.Where(w => w.Member.Name == grp.Key.Sales && w.StartDate.Month == currentmonthstart.Month).Sum(s => (decimal?)s.CheckIn),
                               OneMonthBeforeTarget = targets.Where(w => w.Member.Name == grp.Key.Sales && w.StartDate.Month == onemonthbeforestart.Month).Sum(s => s.CheckIn),
                               TwoMonthBeforeTarget = targets.Where(w => w.Member.Name == grp.Key.Sales && w.StartDate.Month == twomonthbeforestart.Month).Sum(s => s.CheckIn),
                               ThreeMonthBeforeTarget = targets.Where(w => w.Member.Name == grp.Key.Sales && w.StartDate.Month == threemonthbeforestart.Month).Sum(s => s.CheckIn),
                               FourthMonthBeforeTarget = targets.Where(w => w.Member.Name == grp.Key.Sales && w.StartDate.Month == fourmonthbeforestart.Month).Sum(s => s.CheckIn),
                               FifthMonthBeforeTarget = targets.Where(w => w.Member.Name == grp.Key.Sales && w.StartDate.Month == fifthmonthbeforestart.Month).Sum(s => s.CheckIn),
                           };

                return list.Where(w => alivemembers.Any(a => a == w.Name));
            }

            public static IQueryable<AjaxProjectCheckInByWeek> GetProjectsCheckInByWeek()
            {
                var ps = CRM_Logical.GetUserInvolveProject();
                //var monthstart = DateTime.Now.StartOfMonth();
                var monthstart = new DateTime(2013, 5, 2).StartOfMonth();
                var monthend = DateTime.Now.EndOfMonth();
                var firstweekstart = monthstart;
                while (firstweekstart.DayOfWeek == DayOfWeek.Thursday && firstweekstart.DayOfWeek == DayOfWeek.Sunday)
                {
                    firstweekstart = firstweekstart.AddDays(1);
                }
                var firstweekend = firstweekstart;
                while (firstweekend.DayOfWeek != DayOfWeek.Sunday)
                {
                    firstweekend = firstweekend.AddDays(1);
                }
                var secondweekstart = firstweekend;
                var secondweekend = secondweekstart.AddDays(7);
                var thirdweekstart = secondweekend;
                var thirdweekend = thirdweekstart.AddDays(7);
                var fourthweekstart = thirdweekend;
                var fourthweekend = fourthweekstart.AddDays(7);
                var fifthweekstart = fourthweekend;
                var fifthweekend = fifthweekstart.AddDays(7);
                while (fifthweekend > monthend)
                {
                    firstweekend.AddDays(-1);
                }
                IQueryable<Deal> deals = from deal in CH.DB.Deals.Where(d => d.Abandoned == false).Where(w => w.ActualPaymentDate >= monthstart && w.ActualPaymentDate < monthend) select deal;
                var list = from d in deals
                           group d by new { d.Project.ProjectUnitName, d.Project.ProjectUnitCode } into grp
                           select new AjaxProjectCheckInByWeek()
                           {
                               FirstWeekCheckIn = deals.Where(w => w.Project.ProjectUnitCode == grp.Key.ProjectUnitCode && w.ActualPaymentDate >= firstweekstart && w.ActualPaymentDate < firstweekend).Sum(s => s.Income),
                               SencondWeekCheckIn = deals.Where(w => w.Project.ProjectUnitCode == grp.Key.ProjectUnitCode && w.ActualPaymentDate >= secondweekstart && w.ActualPaymentDate < secondweekend).Sum(s => s.Income),
                               ThirdWeekCheckIn = deals.Where(w => w.Project.ProjectUnitCode == grp.Key.ProjectUnitCode && w.ActualPaymentDate >= thirdweekstart && w.ActualPaymentDate < thirdweekend).Sum(s => s.Income),
                               FourWeekCheckIn = deals.Where(w => w.Project.ProjectUnitCode == grp.Key.ProjectUnitCode && w.ActualPaymentDate >= fourthweekstart && w.ActualPaymentDate < fourthweekend).Sum(s => s.Income),
                               FifthWeekCheckIn = deals.Where(w => w.Project.ProjectUnitCode == grp.Key.ProjectUnitCode && w.ActualPaymentDate >= fifthweekstart && w.ActualPaymentDate < fifthweekend).Sum(s => s.Income),
                               TotalCheckIn = deals.Where(w => w.Project.ProjectUnitCode == grp.Key.ProjectUnitCode).Sum(s => s.Income),
                               ProjectName = grp.Key.ProjectUnitName,
                               ProjectCode = grp.Key.ProjectUnitCode
                           };

                return list.Where(w => !string.IsNullOrEmpty(w.ProjectName) || !string.IsNullOrEmpty(w.ProjectCode));
            }

            public static IEnumerable<AjaxProjectProcess> GetProjectsProgress()
            {
                var ps = CRM_Logical.GetUserInvolveProject();
                var targets = CH.DB.TargetOfMonths.Where(w => w.Project.IsActived == true);
                var deals = CRM_Logical.GetDeals(true);
                //var date = new DateTime(2013, 5, 1);
                var date =  DateTime.Now;
                var currentmonthstart = date.StartOfMonth();
                var currentmonthend = date.EndOfMonth();
                var currentweekstart = date.StartOfWeek();
                var currentweekend = date.EndOfWeek();
                var currentdaystart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                var currentdayend = currentdaystart.AddDays(1);
                var list = from p in ps
                           group p by new { p.ProjectUnitCode, p.ProjectUnitName, p.ConferenceStartDate } into grp
                           select new
                           {
                               Name = grp.Key.ProjectUnitName,
                               Code = grp.Key.ProjectUnitCode,
                               StartDate = grp.Key.ConferenceStartDate,
                               LeftDay = (grp.Key.ConferenceStartDate - DateTime.Now).Days,
                               Target = grp.Sum(s => s.Target),
                               MemberCount = grp.Sum(s => s.Members.Where(w => w.IsActivated == true).Count())
                           };

                var data = from l in list
                           select new AjaxProjectProcess
                           {
                               ProjectUnitName = l.Name,
                               ProjectUnitCode = l.Code,
                               LeftedDay = l.LeftDay,
                               CurrentMonthCheckInTarget = targets.Where(w => w.Project.ProjectUnitCode == l.Code && w.StartDate.Month == currentmonthstart.Month).Sum(s => (decimal?)s.CheckIn),
                               CurrentMonthCheckIn = deals.Where(w => w.Project.ProjectUnitCode == l.Code && w.ActualPaymentDate >= currentmonthstart && w.ActualPaymentDate < currentmonthend).Sum(s => (decimal?)s.Income),
                               CurrentWeekCheckIn = deals.Where(w => w.Project.ProjectUnitCode == l.Code && w.ActualPaymentDate >= currentweekstart && w.ActualPaymentDate < currentweekend).Sum(s => (decimal?)s.Income),
                               CurrentDayDealIn = deals.Where(w => w.Project.ProjectUnitCode == l.Code && w.SignDate >= currentdaystart && w.SignDate < currentdayend).Sum(s => (decimal?)s.Payment),
                               TotalCheckIn = deals.Where(w => w.Project.ProjectUnitCode == l.Code).Sum(s => (decimal?)s.Income),
                               TotalCheckInTarget = (decimal?)l.Target,
                               CurrentSales = l.MemberCount,
                               TotalDealInTarget = targets.Where(w => w.Project.ProjectUnitCode == l.Code ).Sum(s => (decimal?)s.Deal),
                               RMBTotalDealIn = deals.Where(w => w.Project.ProjectUnitCode == l.Code && w.Currencytype.Name=="RMB").Sum(s => (decimal?)s.Payment),
                               USDTotalDealIn = deals.Where(w => w.Project.ProjectUnitCode == l.Code && w.Currencytype.Name == "USD").Sum(s => (decimal?)s.Payment)
                           };

                return data;
            }

            public static IEnumerable<AjaxProjectProcessByMonth> GetProjectsProgressByMonth(int? typeid,string manager=null)
            {
                var ps = CRM_Logical.GetUserInvolveProject();
                if (typeid != null)
                    ps = ps.Where(p => p.ProjectTypeID == typeid).ToList();
                if (!string.IsNullOrEmpty(manager))
                    ps = ps.Where(p => p.Manager == manager).ToList();
                var targets = CH.DB.TargetOfMonths.Where(w => w.Project.IsActived == true);
                var deals = CRM_Logical.GetDeals(true);
                var date = new DateTime(2013, 5, 1);
                //var date = DateTime.Now;
                var currentmonthstart = date.StartOfMonth();
                var currentmonthend = date.EndOfMonth();
                var currentweekstart = date.StartOfWeek();
                var currentweekend = date.EndOfWeek();
                var currentdaystart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                var currentdayend = currentdaystart.AddDays(1);
                var list = from p in ps
                           group p by new { p.ProjectUnitCode, p.ProjectUnitName, p.ConferenceStartDate } into grp
                           select new
                           {
                               Name = grp.Key.ProjectUnitName,
                               Code = grp.Key.ProjectUnitCode,
                               StartDate = grp.Key.ConferenceStartDate,
                               LeftDay = (grp.Key.ConferenceStartDate - DateTime.Now).Days,
                               Target = grp.Sum(s => s.Target),
                               MemberCount = grp.Sum(s => s.Members.Where(w => w.IsActivated == true).Count())
                           };

                var data = from l in list
                           select new AjaxProjectProcessByMonth
                           {
                               ProjectUnitName = l.Name,
                               ProjectUnitCode = l.Code,
                               ConferenceStartDate = l.StartDate,
                               LeftedDay = l.LeftDay,
                               TotalDealInTarget = targets.Where(w => w.Project.ProjectUnitCode == l.Code).Sum(s => (decimal?)s.Deal),
                               RMBTotalDealIn = deals.Where(w => w.Project.ProjectUnitCode == l.Code && w.Currencytype.Name == "RMB").Sum(s => (decimal?)s.Payment),
                               USDTotalDealIn = deals.Where(w => w.Project.ProjectUnitCode == l.Code && w.Currencytype.Name == "USD").Sum(s => (decimal?)s.Payment),
                               TotalCheckInTarget = (decimal?)l.Target,
                               TotalCheckIn = deals.Where(w => w.Project.ProjectUnitCode == l.Code).Sum(s => (decimal?)s.Income),
                               CurrentMonthDealInTarget = targets.Where(w => w.Project.ProjectUnitCode == l.Code  && w.StartDate.Month == currentmonthstart.Month).Sum(s => (decimal?)s.Deal),
                               CurrentMonthRMBTotalDealIn = deals.Where(w => w.Project.ProjectUnitCode == l.Code && w.Currencytype.Name == "RMB" && w.SignDate >= currentmonthstart && w.SignDate < currentmonthend).Sum(s => (decimal?)s.Payment),
                               CurrentMonthUSDTotalDealIn = deals.Where(w => w.Project.ProjectUnitCode == l.Code && w.Currencytype.Name == "USD" && w.SignDate >= currentmonthstart && w.SignDate < currentmonthend).Sum(s => (decimal?)s.Payment),

                               CurrentMonthCheckInTarget = targets.Where(w => w.Project.ProjectUnitCode == l.Code && w.StartDate.Month == currentmonthstart.Month).Sum(s => (decimal?)s.CheckIn),
                               CurrentMonthCheckIn = deals.Where(w => w.Project.ProjectUnitCode == l.Code && w.ActualPaymentDate >= currentmonthstart && w.ActualPaymentDate < currentmonthend).Sum(s => (decimal?)s.Income),
                               
                           };

                return data;
            }

            public static IEnumerable<AjaxProjectsCheckInSummary> GetProjectsCheckInSummary(int?year,int? typeid, string manager = null)
            {
                var ps = CRM_Logical.GetUserInvolveProject();
                if (typeid != null)
                    ps = ps.Where(p => p.ProjectTypeID == typeid).ToList();
                if (!string.IsNullOrEmpty(manager))
                    ps = ps.Where(p => p.Manager == manager).ToList();
                var targets = CH.DB.TargetOfMonths.Where(w => w.Project.IsActived == true);
                var deals = CRM_Logical.GetDeals(true);
                if (year == null)
                    year = DateTime.Now.Year;
                //var date = DateTime.Now;
                var list = from p in ps
                           group p by new { p.ProjectUnitCode, p.ProjectUnitName, p.ConferenceStartDate } into grp
                           select new
                           {
                               Code=grp.Key.ProjectUnitCode,
                               Name = grp.Key.ProjectUnitName,
                               StartDate = grp.Key.ConferenceStartDate,
                               Target = grp.Sum(s => s.Target),
                           };

                var data = from l in list
                           select new AjaxProjectsCheckInSummary
                           {
                               ProjectUnitName = l.Name,
                               ConferenceStartDate = l.StartDate,
                               TotalCheckInTarget = (decimal?)l.Target,

                               CheckInJanuary = deals.Where(w => w.Project.ProjectUnitCode == l.Code && w.ActualPaymentDate.Value.Year == year && w.ActualPaymentDate.Value.Month ==1).Sum(s => (decimal?)s.Income),
                                CheckInFebruary  = deals.Where(w => w.Project.ProjectUnitCode == l.Code && w.ActualPaymentDate.Value.Year == year && w.ActualPaymentDate.Value.Month ==2).Sum(s => (decimal?)s.Income),
                                CheckInMarch = deals.Where(w => w.Project.ProjectUnitCode == l.Code && w.ActualPaymentDate.Value.Year == year && w.ActualPaymentDate.Value.Month ==3).Sum(s => (decimal?)s.Income),
                                CheckInApril = deals.Where(w => w.Project.ProjectUnitCode == l.Code && w.ActualPaymentDate.Value.Year == year && w.ActualPaymentDate.Value.Month ==4).Sum(s => (decimal?)s.Income),
                                CheckInMay  = deals.Where(w => w.Project.ProjectUnitCode == l.Code && w.ActualPaymentDate.Value.Year == year && w.ActualPaymentDate.Value.Month ==5).Sum(s => (decimal?)s.Income),
                                CheckInJune = deals.Where(w => w.Project.ProjectUnitCode == l.Code && w.ActualPaymentDate.Value.Year == year && w.ActualPaymentDate.Value.Month ==5).Sum(s => (decimal?)s.Income),
                                CheckInJuly = deals.Where(w => w.Project.ProjectUnitCode == l.Code && w.ActualPaymentDate.Value.Year == year && w.ActualPaymentDate.Value.Month ==6).Sum(s => (decimal?)s.Income),
                                CheckInAugust = deals.Where(w => w.Project.ProjectUnitCode == l.Code && w.ActualPaymentDate.Value.Year == year && w.ActualPaymentDate.Value.Month ==8).Sum(s => (decimal?)s.Income),
                                CheckInSeptember = deals.Where(w => w.Project.ProjectUnitCode == l.Code && w.ActualPaymentDate.Value.Year == year && w.ActualPaymentDate.Value.Month ==9).Sum(s => (decimal?)s.Income),
                                CheckInOctober  = deals.Where(w => w.Project.ProjectUnitCode == l.Code && w.ActualPaymentDate.Value.Year == year && w.ActualPaymentDate.Value.Month ==10).Sum(s => (decimal?)s.Income),
                                CheckInNovember = deals.Where(w => w.Project.ProjectUnitCode == l.Code && w.ActualPaymentDate.Value.Year == year && w.ActualPaymentDate.Value.Month ==11).Sum(s => (decimal?)s.Income),
                               CheckInDecember = deals.Where(w => w.Project.ProjectUnitCode == l.Code && w.ActualPaymentDate.Value.Year == year && w.ActualPaymentDate.Value.Month == 12).Sum(s => (decimal?)s.Income),

                           };

                return data;
            }


            public static IEnumerable<AjaxMemberProjectProcessByMonth> GetMemberProjectsProgressByMonth(int? typeid, string manager = null)
            {
                var ps = CRM_Logical.GetUserInvolveProject();
                if (typeid != null)
                    ps = ps.Where(p => p.ProjectTypeID == typeid).ToList();
                if (!string.IsNullOrEmpty(manager))
                    ps = ps.Where(p => p.Manager == manager).ToList();
                var targets = CH.DB.TargetOfMonthForMembers.Where(w => w.Project.IsActived == true);
                //var deals = CRM_Logical.GetDeals(true);
                var date = new DateTime(2013, 5, 1);
                //var date = DateTime.Now;
                var currentmonthstart = date.StartOfMonth();
                var currentmonthend = date.EndOfMonth();
                IEnumerable<int> idList = ps.Select(o => o.ID);
                var deals = from d in CH.DB.Deals where idList.Contains((int)d.ProjectID) select d;
                
                var list = from d in deals
                           group d by new { d.Sales,d.ProjectID,d.Project.ProjectUnitName,d.Project.ConferenceStartDate } into grp
                           select new AjaxMemberProjectProcessByMonth
                           {
                               Member = grp.Key.Sales,
                               //ConferenceStartDate = CH.GetDataById<Project>(grp.Key.ProjectID).ConferenceStartDate,
                               TotalCheckInTarget = targets.Where(w => w.Member.Name == grp.Key.Sales).Sum(w => (decimal?)w.CheckIn),
                               TotalDealInRMB = deals.Where(w => w.Project.ID == grp.Key.ProjectID && w.Currencytype.Name == "RMB" && w.Sales == grp.Key.Sales).Sum(s => (decimal?)s.Payment),
                               TotalDealInUSD = deals.Where(w => w.Project.ID == grp.Key.ProjectID && w.Currencytype.Name == "USD" && w.Sales == grp.Key.Sales).Sum(s => (decimal?)s.Payment),
                               TotalCheckIn = deals.Where(w => w.Project.ID == grp.Key.ProjectID && w.Sales == grp.Key.Sales).Sum(s => (decimal?)s.Income),
                               CurrentMonthDealInTarget = targets.Where(w => w.Project.ID == grp.Key.ProjectID && w.Member.Name == grp.Key.Sales && w.StartDate.Month == currentmonthstart.Month).Sum(s => (decimal?)s.Deal),
                               CurrentMonthRMBTotalDealIn = deals.Where(w => w.Project.ID == grp.Key.ProjectID && w.Sales == grp.Key.Sales && w.Currencytype.Name == "RMB" && w.SignDate >= currentmonthstart && w.SignDate < currentmonthend).Sum(s => (decimal?)s.Payment),
                               CurrentMonthUSDTotalDealIn = deals.Where(w => w.Project.ID == grp.Key.ProjectID && w.Sales == grp.Key.Sales && w.Currencytype.Name == "USD" && w.SignDate >= currentmonthstart && w.SignDate < currentmonthend).Sum(s => (decimal?)s.Payment),

                               CurrentMonthCheckInTarget = targets.Where(w => w.Project.ID == grp.Key.ProjectID && w.Member.Name == grp.Key.Sales && w.StartDate.Month == currentmonthstart.Month).Sum(s => (decimal?)s.CheckIn),
                               CurrentMonthCheckIn = deals.Where(w => w.Project.ID == grp.Key.ProjectID && w.Sales == grp.Key.Sales && w.ActualPaymentDate >= currentmonthstart && w.ActualPaymentDate < currentmonthend).Sum(s => (decimal?)s.Income),
                               ProjectUnitName = grp.Key.ProjectUnitName,
                               ConferenceStartDate = grp.Key.ConferenceStartDate
                           };
               
                list = list.OrderBy(l => l.Member);

                //list = list.Where(w => salesList.Any(a => a == w.Member));
                

                return list;
            }

            public static IEnumerable<AjaxSalesCheckInSummary> GetAjaxSalesCheckInSummary(int?year ,int? typeid, string manager = null)
            {
                var ps = CRM_Logical.GetUserInvolveProject();
                if (typeid != null)
                    ps = ps.Where(p => p.ProjectTypeID == typeid).ToList();
                if (!string.IsNullOrEmpty(manager))
                    ps = ps.Where(p => p.Manager == manager).ToList();
                if (year == null)
                    year = DateTime.Now.Year;
                IEnumerable<int> idList = ps.Select(o => o.ID);
                var deals = from d in CH.DB.Deals where idList.Contains((int)d.ProjectID) select d;
                var employroles = from e in CH.DB.EmployeeRoles select e;
                var list = from d in deals
                           group d by new { d.Sales} into grp
                           select new AjaxSalesCheckInSummary
                           {
                               Sales = grp.Key.Sales,
                               SalesStartDate = employroles.Where(w=>w.AccountName==grp.Key.Sales).FirstOrDefault().StartDate,
                               CheckInJanuary = deals.Where(w => w.Sales == grp.Key.Sales && w.ActualPaymentDate.Value.Year == year && w.ActualPaymentDate.Value.Month == 1).Sum(s => (decimal?)s.Income),
                               CheckInFebruary = deals.Where(w => w.Sales == grp.Key.Sales && w.ActualPaymentDate.Value.Year == year && w.ActualPaymentDate.Value.Month == 2).Sum(s => (decimal?)s.Income),
                               CheckInMarch = deals.Where(w => w.Sales == grp.Key.Sales && w.ActualPaymentDate.Value.Year == year && w.ActualPaymentDate.Value.Month == 3).Sum(s => (decimal?)s.Income),
                               CheckInApril = deals.Where(w => w.Sales == grp.Key.Sales && w.ActualPaymentDate.Value.Year == year && w.ActualPaymentDate.Value.Month == 4).Sum(s => (decimal?)s.Income),
                               CheckInMay = deals.Where(w => w.Sales == grp.Key.Sales && w.ActualPaymentDate.Value.Year == year && w.ActualPaymentDate.Value.Month == 5).Sum(s => (decimal?)s.Income),
                               CheckInJune = deals.Where(w => w.Sales == grp.Key.Sales && w.ActualPaymentDate.Value.Year == year && w.ActualPaymentDate.Value.Month == 5).Sum(s => (decimal?)s.Income),
                               CheckInJuly = deals.Where(w => w.Sales == grp.Key.Sales && w.ActualPaymentDate.Value.Year == year && w.ActualPaymentDate.Value.Month == 6).Sum(s => (decimal?)s.Income),
                               CheckInAugust = deals.Where(w => w.Sales == grp.Key.Sales && w.ActualPaymentDate.Value.Year == year && w.ActualPaymentDate.Value.Month == 8).Sum(s => (decimal?)s.Income),
                               CheckInSeptember = deals.Where(w => w.Sales == grp.Key.Sales && w.ActualPaymentDate.Value.Year == year && w.ActualPaymentDate.Value.Month == 9).Sum(s => (decimal?)s.Income),
                               CheckInOctober = deals.Where(w => w.Sales == grp.Key.Sales && w.ActualPaymentDate.Value.Year == year && w.ActualPaymentDate.Value.Month == 10).Sum(s => (decimal?)s.Income),
                               CheckInNovember = deals.Where(w => w.Sales == grp.Key.Sales && w.ActualPaymentDate.Value.Year == year && w.ActualPaymentDate.Value.Month == 11).Sum(s => (decimal?)s.Income),
                               CheckInDecember = deals.Where(w => w.Sales == grp.Key.Sales && w.ActualPaymentDate.Value.Year == year && w.ActualPaymentDate.Value.Month == 12).Sum(s => (decimal?)s.Income),

                           };

                list = list.OrderBy(l => l.Sales);

                //list = list.Where(w => salesList.Any(a => a == w.Member));


                return list;
            }

            public static IEnumerable<AjaxCompanyDailyReceivedPayment> GetCompanyDailyReceivedPayment(DateTime? currentdate, int? abstractid, string sales, int? projectid, string companyname = null)
            {
                var ps = CRM_Logical.GetUserInvolveProject();
                if (projectid != null)
                    ps = ps.Where(p => p.ID == projectid).ToList();
                var deals = CRM_Logical.GetDeals(true);
                if (currentdate != null)
                    deals = deals.Where(w => w.ActualPaymentDate == currentdate);
                if (!string.IsNullOrEmpty(companyname))
                {
                    deals = deals.Where(w => w.CompanyRelationship.Company.Name_CH.Contains(companyname) || w.CompanyRelationship.Company.Name_EN.Contains(companyname));
                }
                var date = new DateTime(2013, 5, 1);
                //var date = DateTime.Now;
                IEnumerable<int> idList = ps.Select(o => o.ID);
                var members = (from p in idList
                               join d in deals on p equals d.ProjectID
                               join m in CH.DB.EmployeeRoles on d.Sales equals m.AccountName
                               select new
                               {
                                   Sales = d.Sales,
                                   ProjectID = p,
                                   RoleName = m.Role.Name
                               });
                if (abstractid != null)
                {
                    if (abstractid == 1 || abstractid == 3)
                    {
                        //members = from m in members
                        //          join d in CH.DB.EmployeeRoles.Where(w=>w.Role.Name=="国外销售") on m.Sales equals d.AccountName 
                        //          select new
                        //          {
                        //              Sales = m.Sales,
                        //              ProjectID = m.ProjectID
                        //          };
                        members = members.Where(m => m.RoleName == "海外销售");
                    }
                    else
                    {
                        //members = from m in members
                        //          join d in CH.DB.EmployeeRoles.Where(w => w.Role.Name == "国内销售") on m.Sales equals d.AccountName 
                        //          select new
                        //          {
                        //              Sales = m.Sales,
                        //              ProjectID = m.ProjectID
                        //          };
                        members = members.Where(m => m.RoleName == "国内销售");
                    }
                }
                if (!string.IsNullOrEmpty(sales))
                {
                    //members = from m in members.Where(w => w.Sales == sales)
                    //          select new
                    //          {
                    //              Sales = m.Sales,
                    //              ProjectID = m.ProjectID
                    //          };
                    members = members.Where(m => m.Sales == sales);
                }
                IEnumerable<string> salesList = members.Select(o => o.Sales).Distinct();
                var list = from p in idList
                           join d in deals on p equals d.ProjectID
                           join m in salesList on d.Sales equals m
                           select new AjaxCompanyDailyReceivedPayment
                           {
                               Sales = d.Sales,
                               CheckInDate=d.ActualPaymentDate,
                               CheckInRMB = d.Currencytype.Name == "RMB" ? (decimal?)d.Payment : 0,
                               CheckInUSD = d.Currencytype.Name == "USD" ? (decimal?)d.Payment : 0,
                               CompanyName = d.CompanyRelationship.Company.Name_CH == null ? d.CompanyRelationship.Company.Name_EN : d.CompanyRelationship.Company.Name_CH + "|" + d.CompanyRelationship.Company.Name_EN,
                               ProjectName = d.Project.ProjectUnitName,
                               Abstract = CH.DB.EmployeeRoles.Where(e => e.AccountName == d.Sales).FirstOrDefault() == null ? "" : CH.DB.EmployeeRoles.Where(e => e.AccountName == d.Sales).FirstOrDefault().Role.Name=="国内销售"?"国内销售收入":"国外销售收入",
                               Remark = d.PaymentDetail
                               
                           };
                list = list.OrderBy(w=>w.Sales);
                return list;
            }
            public static IEnumerable<SelectListItem> GetProjectsManager()
            {
                var ps = CRM_Logical.GetUserInvolveProject();
                var pros=ps.Select(p => p.Manager).Distinct();
                List<SelectListItem> selectList = new List<SelectListItem>();
                foreach (var pro in pros)
                {
                    SelectListItem selectListItem = new SelectListItem { Text = pro, Value = pro };
                    selectList.Add(selectListItem);
                }
                return selectList;
            }
            public static IEnumerable<SelectListItem> GetProjectsSales()
            {
                var ps = CRM_Logical.GetUserInvolveProject();
                IEnumerable<int> idList = ps.Select(o => o.ID);
                var members = (from p in idList
                               join m in CH.DB.Members on p equals m.ProjectID
                               select new
                               {
                                   Sales = m.Name,
                               }).ToList();
                var pros = members.Select(p => p.Sales).Distinct();
                List<SelectListItem> selectList = new List<SelectListItem>();
                foreach (var pro in pros)
                {
                    SelectListItem selectListItem = new SelectListItem { Text = pro, Value = pro };
                    selectList.Add(selectListItem);
                }
                return selectList;
            }
        }

        public static class _Project
        {

            public static List<AjaxProjectPerformance> GetAllProjectPerformance()
            {
                var pid = CRM_Logical.GetUserInvolveProject().Select(s => s.ID);
                var list = new List<AjaxProjectPerformance>();
                foreach (var id in pid)
                {
                    var performance = GetAjaxProjectPerformance(id);
                    list.Add(performance);
                }
                return list;
            }

            public static AjaxProjectPerformance GetAjaxProjectPerformance(int? projectid)
            {

                var p = CH.GetDataById<Project>(projectid);
                var pdeals = CH.DB.Deals.Where(w => w.ProjectID == projectid && w.Abandoned == false && w.Income > 0 && w.ActualPaymentDate != null);
                var pt = CH.DB.TargetOfMonths.Where(w => w.ProjectID == projectid);
                var now = DateTime.Now;
                var data = new AjaxProjectPerformance()
                {
                    ProjectCode = p.ProjectCode,
                    LeftDays = (p.ConferenceStartDate - now).TotalDays,
                    Duration = (p.EndDate - p.StartDate).TotalDays,
                    ProjectID = p.ID,
                    Name_CH = p.Name_CH,
                    Target = p.Target,
                    Memebers = (from m in p.Members.Where(a => a.IsActivated == true) select new AjaxMember { Name = m.Name }),
                    AjaxProjectPerformanceInMonths = (from d in pdeals
                                                      group d by new { d.ActualPaymentDate.Value.Month, d.ActualPaymentDate.Value.Year } into grp
                                                      select new AjaxProjectPerformanceInMonth
                                                      {
                                                          Year = grp.Key.Year,
                                                          Month = grp.Key.Month,
                                                          CheckIn = pdeals.Sum(s => s.Income),
                                                          CheckinTarget = pt.Where(w => w.StartDate.Month == grp.Key.Month && w.StartDate.Year == grp.Key.Year).Sum(s => s.CheckIn)
                                                      })

                };

                return data;
            }

            public static AjaxProject GetAjaxProject(int? projectid)
            {
                var p = CH.GetDataById<Project>(projectid);
                var data = new AjaxProject()
                {
                    ProjectCode = p.ProjectCode,
                    ProjectID = p.ID,
                    ProjectName = p.Name,
                    CRMs = _CRM.GetCrmByProjectId(projectid)
                };
                return data;
            }
        }

        public static class _LeadCall
        {
            public static IQueryable<LeadCall> GetUserCallBackSince7DayBefore()
            {
                //取得该员工的所有成员实例

                var sevendaybefore = DateTime.Now.AddDays(-7);
                var user = Employee.CurrentUserName;
                var calls = from c in CH.DB.LeadCalls
                            where c.Member.Name == user && c.CallBackDate >= sevendaybefore
                            select c;

                return calls;
            }
        }

        public static class _Deal
        {
            public static IQueryable<Deal> GetUserUnClosedDeal()
            {
                //取得该员工的所有成员实例


                var user = Employee.CurrentUserName;
                var deals = from d in CH.DB.Deals
                            where d.Sales == user && d.Income == 0 && d.Abandoned==false
                            select d;

                return deals;
            }
        }

        public static class _CRM
        {
            //导入公司
            public static void ImportCompany(int[] sourceprojectids, int targetprojectid, string user, out List<Company> conflictCompany)
            {
                //导入的目标
                var p = CH.GetDataById<Project>(targetprojectid);
                user = "系统导入:" + user;

                //取得要导入的公司
                var comapnytoexport = CH.GetAllData<CompanyRelationship>(c => c.ProjectID != null && sourceprojectids.Contains(c.ProjectID.Value)).Select(s => s.Company);

                var crms = from c in comapnytoexport
                           select new CompanyRelationship()
                           {
                               CompanyID = c.ID,
                               Company = c,
                               ProjectID = p.ID,
                               Project = p,
                               Creator = user,
                               CreatedDate = DateTime.Now,
                               MarkForDelete = false 
                           };
                // Company.IsCompanyExist();

                //公司名在这个项目中是否存在
                var existscompanynames = from c in p.CompanyRelationships.Select(s => s.Company)
                                         select new { namech = c.Name_CH, nameen = c.Name_EN, id = c.ID };
                //获取导入公司
                var importCrms = crms.Where(c => (string.IsNullOrEmpty(c.Company.Name_CH) || existscompanynames.Any(a => a.namech == c.Company.Name_CH) == false)
                     && (string.IsNullOrEmpty(c.Company.Name_EN) || existscompanynames.Any(a => a.nameen == c.Company.Name_EN) == false));


                //获取导入公司的Lead Calls
                var crtoexport = CH.GetAllData<CompanyRelationship>(c => c.ProjectID != null && sourceprojectids.Contains(c.ProjectID.Value)).Select(s => s.LeadCalls);
                var importLeadCalls = from tc in crtoexport select tc;
                
                //获取冲突公司
                //var conflictCrms = crms.Except(importCrms);
                var conflictCrms = CH.GetAllData<CompanyRelationship>().Where(c => c.ProjectID == targetprojectid && comapnytoexport.Any(i => i.ID == c.CompanyID));

                if (conflictCrms.Count() > 0)
                {
                    conflictCompany = CH.GetAllData<Company>().Where(c => conflictCrms.Any(s => s.CompanyID == c.ID)).ToList();
                }
                else
                {
                    conflictCompany = null;
                }

                if (importCrms.Count() > 0)
                {
                    ImportCompanyTrace newdata = new ImportCompanyTrace();
                    newdata.ImportDate = DateTime.Now;
                    newdata.ImportUserName = user;
                    newdata.ImportCompanyCount = importCrms.Count();
                    newdata.ImportLeadCount = CH.GetAllData<Lead>().Where(s => importCrms.Any(i => i.CompanyID == s.CompanyID)).Count();
                    newdata.ImportTargetProject = CH.GetDataById<Project>(targetprojectid).Name;
                    var data = CH.GetAllData<Project>().Where(s => sourceprojectids.Any(a => a == s.ID)).Select(s => s.Name).ToArray();

                    newdata.ImportSourceProject = string.Join(",", data);

                    CH.Create<ImportCompanyTrace>(newdata);

                    p.CompanyRelationships.AddRange(importCrms);
                    CH.Edit<Project>(p);
                    //if (importLeadCalls != null && importLeadCalls.Count() > 0)
                    //{
                    //    for (int i = 0; i < importLeadCalls.Count();i++ )
                    //    {
                    //        if (importLeadCalls.ElementAt(i) != null && importLeadCalls.ElementAt(i).Count() > 0)
                    //        {
                    //            for (int j = 0; j < importLeadCalls.ElementAt(i).Count(); j++)
                    //            {
                    //                var leadcall = importLeadCalls.ElementAt(i).ElementAt(j);
                    //                var crmid = p.CompanyRelationships.Where(crs =>
                    //                            crs.CompanyID == leadcall.CompanyID).FirstOrDefault().ID;
                    //                leadcall.CompanyRelationshipID = crmid;
                    //                leadcall.IsImport = true;
                    //                CH.Create<LeadCall>(leadcall);
                    //            }
                    //        }
                    //    }
                    //}
                }
            }

            public static IEnumerable<AjaxCRM> GetCrmByProjectId(int? projectid)
            {
                if (projectid != null)
                {
                    var crms = CH.DB.CompanyRelationships.Where(c => c.ProjectID == projectid);
                    var ajaxcrms = from c in crms
                                   select new AjaxCRM()
                                   {
                                       CrmCreateDate = c.CreatedDate,
                                       CRMID = c.ID,
                                       CompanyCategories = c.Categorys,
                                       Members = c.Members,
                                       CompanyNameEN = string.IsNullOrEmpty(c.Company.Name_EN) ? "" : c.Company.Name_EN,
                                       CompanyNameCH = string.IsNullOrEmpty(c.Company.Name_CH) ? "" : c.Company.Name_CH,
                                       CompanyContact = string.IsNullOrEmpty(c.Company.Contact) ? "" : c.Company.Contact,
                                       CrmCreator = c.Creator,
                                       CompanyType = c.Company.CompanyType.Name,
                                       AjaxLeads = (from l in c.Company.Leads
                                                    select new AjaxLead
                                                    {
                                                        LeadNameCH = string.IsNullOrEmpty(l.Name_CH) ? "" : l.Name_CH,
                                                        LeadNameEN = string.IsNullOrEmpty(l.Name_EN) ? "" : l.Name_EN,
                                                        LeadEmail = string.IsNullOrEmpty(l.EMail) ? "" : l.EMail,
                                                        LeadPersonalEmail = string.IsNullOrEmpty(l.PersonalEmailAddress) ? "" : l.PersonalEmailAddress,
                                                        LeadTitle = l.Title,
                                                        CRMID = c.ID,
                                                        LeadID = l.ID,
                                                        AjaxCalls = (from call in c.LeadCalls.Where(w => w.LeadID == l.ID)
                                                                     select new AjaxCall
                                                                     {
                                                                         CallDate = call.CallDate,
                                                                         CallBackDate = call.CallBackDate,
                                                                         CallType = call.LeadCallType.Name,
                                                                         Caller = call.Member.Name,
                                                                         LeadCallTypeCode = call.LeadCallType.Code
                                                                     })
                                                    })
                                   };
                    return ajaxcrms;
                }

                return null;
            }

        }
        public static IQueryable<TargetOfMonth> GetProjectMonthTargets()
        {
            return from t in CH.DB.TargetOfMonths.Where(w => w.Project.IsActived == true && w.Project.Test != null && w.Project.Test != true) select t;
        }
        public static IQueryable<Deal> GetDeals(bool? acitivatedprojectonly = false, int? projectid = null, string sales = null, string filter = null)
        {
            IQueryable<Deal> deals = from deal in CH.DB.Deals.Where(d => d.Abandoned == false) select deal;

            if (Utl.Utl.DebugModel() != true)
            {
                deals = deals.Where(w => w.Project.Test == null || w.Project.Test != true);
            }

            if (acitivatedprojectonly == true)//只取激活的项目的deals
            {
                deals = deals.Where(w => w.Project.IsActived == true);
            }

            if (projectid != null)//只取对应的项目的deals
            {
                deals = deals.Where(w => w.ProjectID == projectid);
            }

            if (!string.IsNullOrEmpty(sales))//只取对应的sales
            {
                deals = deals.Where(w => w.Sales == sales);
            }

            if (!string.IsNullOrEmpty(filter))
            {
                switch (filter)
                {
                    case "1":
                        DateTime oneday = DateTime.Now.AddDays(-1);
                        deals = deals.Where(w => w.SignDate > oneday);
                        break;
                    case "7":
                        DateTime oneweek = DateTime.Now.AddDays(-7);
                        deals = deals.Where(w => w.SignDate > oneweek);
                        break;
                    case "14":
                        DateTime twoweek = DateTime.Now.AddDays(-14);
                        deals = deals.Where(w => w.SignDate > twoweek);
                        break;
                    case "30":
                        DateTime onemonth = DateTime.Now.AddMonths(-1);
                        deals = deals.Where(w => w.SignDate > onemonth);
                        break;
                    case "2":
                        deals = deals.Where(w => w.IsConfirm == null || w.IsConfirm == false);
                        break;
                    case "3":
                        deals = deals.Where(w => w.IsConfirm == true && (w.Income == 0 || w.ActualPaymentDate == null));
                        break;
                    case "4":
                        deals = deals.Where(w => w.IsConfirm == true && w.Income != 0 && w.ActualPaymentDate != null);
                        break;
                }
            }
            return deals;

        }

        public static decimal? GetTotalPayment(bool? acitivatedprojectonly = false, int? crId = null)
        {
            var total = (from deal in CH.DB.Deals
                         where deal.Abandoned == false && deal.Project.IsActived == acitivatedprojectonly && deal.CompanyRelationshipID == crId
                         select (decimal?)deal.Payment).Sum() ?? 0;
            return total;
        }

        #region Project Management
        public List<Project> GetProjectByCurrentRole()
        {
            var currentuser = Employee.CurrentUserName;
            List<Project> projects = null;
            if (Employee.AsDirector())
            {
                projects = CH.GetAllData<Project>();
            }
            else
            {
                if (Employee.EqualToManager())
                {
                    projects = CH.GetAllData<Project>(p => Employee.IsEqualToCurrentUserName(p.Manager));
                }
                else if (Employee.EqualToLeader())
                {
                    projects = CH.GetAllData<Project>(p => Employee.IsEqualToCurrentUserName(p.TeamLeader));
                }
                else
                {
                    projects = CH.GetAllData<Project>();
                    projects = projects.FindAll(p => p.Members.Any(m => Employee.IsEqualToCurrentUserName(m.Name)));
                }
            }
            return projects;
        }

        public static TargetOfWeek GetTargetOfWeek(int projectid, int targetofmonth, string member, DateTime startdate, DateTime enddate)
        {
            var data = CH.GetAllData<TargetOfWeek>(tw => tw.ProjectID == projectid && tw.TargetOfMonthID == targetofmonth && tw.Member == member);
            var result = data.FirstOrDefault(t => t.StartDate == startdate && t.EndDate == enddate);
            return result;
        }

        public static decimal GetTargetOfWeekValue(int projectid, int targetofmonth, string member, DateTime startdate, DateTime enddate)
        {
            var data = GetTargetOfWeek(projectid, targetofmonth, member, startdate, enddate);
            if (data != null) return data.Deal;
            return 0;
        }

        public static List<string> GetDefaultCharatracter()
        {
            return new List<string>() { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", 
                "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        }

        public static bool IsCompanySelectedForProject(Company c, int projectid)
        {
            var p = CH.GetAllData<Project>(i => i.ID == projectid).FirstOrDefault();

            if (p != null && p.CompanyRelationships.FirstOrDefault(child => child.CompanyID == c.ID) != null)
            {
                return true;
            }

            return false;
        }

        public static bool IsProjectReferedbyCurrentProject(Project c, int projectid)
        {
            var p = CH.GetAllData<Project>(i => i.ID == projectid).FirstOrDefault();
            if (p.References != null)
            {
                var references = p.References.Split('|');
                if (p != null && references.Contains(c.ProjectCode))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        public static IQueryable<CompanyRelationship> GetUserCallingCRM(int? projectid = null)
        {
            var user = Employee.CurrentUserName;
            var data = CH.DB.CompanyRelationships.Where(c => c.Members.Any(s => s.Name == user));
            if (projectid != null)
            {
                data = data.Where(w => w.ProjectID == projectid);
            }
            return data;
        }


        public static List<LeadCall> GetProjectFaxoutList(DateTime? startdate, DateTime? enddate, List<int> projectids)
        {
            if (!startdate.HasValue)
            {
                startdate = new DateTime(1, 1, 1);
            }
            if (!enddate.HasValue)
            {
                enddate = new DateTime(9999, 1, 1);
            }

            var calls = from l in CH.DB.LeadCalls where l.LeadCallType.Code >= 40 select l;
            if (projectids.Count > 0)
            {
                calls = calls.Where(l => projectids.Contains(l.ProjectID.Value));
            }
            var list = calls.OrderBy(o => o.CallDate).ToList();
            list = list.Distinct(new LeadCallLeadDistinct()).Where(w => w.CallDate >= startdate && w.CallDate < enddate).ToList();

            return list;
        }

        public static List<Project> GetUserProjectRight(string funtionname)
        {
            var ps = from p in CH.DB.ProjectRights where p.Project.IsActived == true && p.AccessRights.Select(s => s.Name).Contains(funtionname) select p;

            var list = ps.ToList();

            var plist = list.FindAll(f => IsContainLoginUser(f.Name)).Select(s => s.Project).ToList();

            var plistorigin = GetUserInvolveProject();

            plistorigin.AddRange(plist);
            plistorigin = plistorigin.Distinct().ToList();
            return plistorigin;
        }

        static bool IsContainLoginUser(string namelist)
        {
            string name = Employee.CurrentUserName;
            var names = namelist.Split('|');
            foreach (var n in names)
            {
                if (n.Trim().ToLower() == name.Trim().ToLower())
                {
                    return true;
                }
            }
            return false;
        }


        public static List<Project> GetUserInvolveProject()
        {
            List<Project> ps = new List<Project>(); ;
            var lvl = Employee.CurrentRole.Level;
            if (lvl == 5)
            {
                ps = GetProductInvolveProject();
            }

            if (lvl == 1)
            {
                ps = GetProductInvolveProject();
            }

            if (lvl == 3)
            {
                ps = GetConferenceInvolveProject();
            }
            if (lvl == 4)
            {
                ps = GetFinanceInvolveProject();
            }

            if (lvl >= 10 && lvl <= 100)
            {
                ps = GetSalesInvolveProject();
            }
            if (lvl >= 1000 && lvl < 99999)
            {
                ps = GetDirectorInvolveProject();
            }
            if (lvl >= 500 && lvl < 1000)
            {
                ps = GetManagerInvolveProject();
            }
            if (lvl == 99999)
            {
                ps = GetAdminInvolveProject();
            }
            ps = ps.Where(w => w.Test == null || w.Test == false).ToList();//不反回测试项目
            return ps;
        }
        /// <summary>
        /// 取得sales正在参与的，已经激活，并且当前时间在项目周期内的项目
        /// </summary>
        /// <returns></returns>
        public static List<Project> GetSalesInvolveProject()
        {
            var name = Employee.CurrentUserName;
            var now = DateTime.Now;
            var projects = CH.GetAllData<Project>();

            var data = projects.FindAll(p => (p.Members.Any(m => m.Name == name && m.IsActivated==true) || p.TeamLeader == name ) && p.IsActived == true );
            return data;
        }

        public static List<Project> GetManagerInvolveProject()
        {
            var name = Employee.CurrentUserName;
            var now = DateTime.Now;
            var projects = CH.GetAllData<Project>(p => p.Manager == name);

            var data = projects.FindAll(p => p.IsActived == true);
            return data;
        }

        public static List<Project> GetDirectorInvolveProject()
        {
            var name = Employee.CurrentUserName;
            var now = DateTime.Now;
            var projects = CH.GetAllData<Project>().OrderByDescending(p => p.ConferenceEndDate).ToList();

            var data = projects.FindAll(p => p.IsActived == true);
            return data;
        }

        public static List<Project> GetProductInvolveProject()
        {
            var name = Employee.CurrentUserName;
            var now = DateTime.Now;
            var projects = CH.GetAllData<Project>(p => p.Product == name);
            var data = projects.FindAll(p => p.IsActived == true);
            return data;
        }

        public static List<Project> GetMarketInvolveProject()
        {
            var name = Employee.CurrentUserName;
            var now = DateTime.Now;
            var projects = CH.GetAllData<Project>();

            var data = projects.FindAll(p => p.Market == name && p.IsActived == true);
            return data;
        }

        public static List<Project> GetConferenceInvolveProject()
        {
            var name = Employee.CurrentUserName;
            var projects = CH.GetAllData<Project>();

            List<Project> data = new List<Project>();
            foreach (var c in projects.FindAll(p => p.IsActived == true))
            {
                if (!string.IsNullOrEmpty(c.Conference))
                {
                    var names = c.Conference.Trim().Split(new string[] { ";", "；" }, StringSplitOptions.RemoveEmptyEntries);
                    if (names.Contains(name))
                    {
                        data.Add(c);
                    }
                }
            }
            return data;
        }

        public static List<Project> GetFinanceInvolveProject()
        {
            var name = Employee.CurrentUserName;
            var now = DateTime.Now;
            var projects = CH.GetAllData<Project>();

            var data = projects.FindAll(p => p.IsActived == true);
            return data;
        }

        public static List<Project> GetAdminInvolveProject()
        {
            var data = CH.GetAllData<Project>();
            return data;
        }

        /// <summary>
        /// 版块负责人月度考核项目：责任心与积极性（带队加班情况）
        /// 参照《销售部月度考核（2013准事业部负责人版）2月新版.xls》
        /// </summary>
        /// <returns></returns>
        public static List<_Item> GetResponsibilityItems()
        {
            List<_Item> itemList = new List<_Item>();
            _Item item = new _Item();
            //item.ID = 0;
            //item.Name = "请选择";
            //itemList.Add(item);

            //item = new _Item();
            item.ID = 1;
            item.Name = "1%-对工作基本上没有热情，消极被动，只安于现状，缺乏工作责任心，经常推卸责任，几乎没有加班";
            itemList.Add(item);
            item = new _Item();
            item.ID = 3;
            item.Name = "3%-对工作有一定责任心和积极性，但专注度尚不够，其程度有时受个人偏好影响，偶尔会带队加班加点，产生的效果、作用一般";
            itemList.Add(item);

            item = new _Item();
            item.ID = 5;
            item.Name = "5%-有很强的工作责任心和积极性，对待工作认真扎实，精益求精，经常主动地带队加班，并且产生明显的效果、作用";
            itemList.Add(item);
            return itemList;
        }
        /// <summary>
        /// 版块负责人月度考核项目：纪律性（请假，迟到情况）
        /// 参照《销售部月度考核（2013准事业部负责人版）2月新版.xls》
        /// </summary>
        /// <returns></returns>
        public static List<_Item> GetDisciplineItems()
        {
            List<_Item> itemList = new List<_Item>();
            _Item item = new _Item();
            //item.ID = 0;
            //item.Name = "请选择";
            //itemList.Add(item);

            //item = new _Item();
            item.ID = 1;
            item.Name = "1%-基本不能遵守工作规定、制度和考勤要求，迟到或早退超过五次，或有缺勤，或工作中有其他违规情况发生";
            itemList.Add(item);
            item = new _Item();
            item.ID = 3;
            item.Name = "3%-行为规范上对自己有一定的要求，能够基本遵守各项规章制度，偶尔有请假，迟到或早退现象发生";
            itemList.Add(item);

            item = new _Item();
            item.ID = 5;
            item.Name = "5%-行为规范上严于律己，能起到表率作用,没有请假，迟到或早退现象发生";
            itemList.Add(item);
            return itemList;
        }
        /// <summary>
        /// 版块负责人月度考核项目：执行能力
        /// 参照《销售部月度考核（2013准事业部负责人版）2月新版.xls》
        /// </summary>
        /// <returns></returns>
        public static List<_Item> GetExcutionItems()
        {
            List<_Item> itemList = new List<_Item>();
            _Item item = new _Item();
            //item.ID = 0;
            //item.Name = "请选择";
            //itemList.Add(item);

            //item = new _Item();
            item.ID = 1;
            item.Name = "1%-对上级下达的招聘，培训等任务比较消极，工作结果不尽人意";
            itemList.Add(item);
            item = new _Item();
            item.ID = 3;
            item.Name = "3%-尚能按时完成上级下达的招聘，培训等任务，基本达到上级要求";
            itemList.Add(item);

            item = new _Item();
            item.ID = 5;
            item.Name = "5%-以积极认真的态度，及时认真完成上级下达的招聘，培训等任务且成效令人满意";
            itemList.Add(item);
            return itemList;
        }
        /// <summary>
        /// 版块负责人月度考核项目：目标意识
        /// 参照《销售部月度考核（2013准事业部负责人版）2月新版.xls》
        /// </summary>
        /// <returns></returns>
        public static List<_Item> GetTargetingItems()
        {
            List<_Item> itemList = new List<_Item>();
            _Item item = new _Item();
            //item.ID = 0;
            //item.Name = "请选择";
            //itemList.Add(item);

            //item = new _Item();
            item.ID = 1;
            item.Name = "1%-计划缺失，对目标的认识不够充分，团队目标不明确，不能够充分利用目标进行团队激励";
            itemList.Add(item);
            item = new _Item();
            item.ID = 3;
            item.Name = "3%-制定一般可操作的工作计划，明确团队目标，并且使用目标进行激励";
            itemList.Add(item);

            item = new _Item();
            item.ID = 5;
            item.Name = "5%-能够设定科学合理的工作计划与目标，逐层分解目标，并利用目标有效激励成员，结合工作计划阶段性分解实现";
            itemList.Add(item);
            return itemList;
        }
        /// <summary>
        /// 版块负责人月度考核项目：每天检查团队成员research,   call list,on phone时间
        /// 参照《销售部月度考核（2013准事业部负责人版）2月新版.xls》
        /// </summary>
        /// <returns></returns>
        public static List<_Item> GetSearchingItems()
        {
            List<_Item> itemList = new List<_Item>();
            _Item item = new _Item();
            //item.ID = 0;
            //item.Name = "请选择";
            //itemList.Add(item);

            //item = new _Item();
            item.ID = 1;
            item.Name = "1%-随机检查，提醒就做，不提醒就不做";
            itemList.Add(item);
            item = new _Item();
            item.ID = 3;
            item.Name = "3%-有计划的不定时检查";
            itemList.Add(item);

            item = new _Item();
            item.ID = 5;
            item.Name = "5%-按时检查无遗漏";
            itemList.Add(item);
            return itemList;
        }
        /// <summary>
        /// 版块负责人月度考核项目：每周与研发人员的项目进度协调
        /// 参照《销售部月度考核（2013准事业部负责人版）2月新版.xls》
        /// </summary>
        /// <returns></returns>
        public static List<_Item> GetProductionItems()
        {
            List<_Item> itemList = new List<_Item>();
            _Item item = new _Item();
            //item.ID = 0;
            //item.Name = "请选择";
            //itemList.Add(item);

            //item = new _Item();
            item.ID = 1;
            item.Name = "1%-协调缺失，项目进度受影响";
            itemList.Add(item);
            item = new _Item();
            item.ID = 3;
            item.Name = "3%-协调滞后，效果一般";
            itemList.Add(item);

            item = new _Item();
            item.ID = 5;
            item.Name = "5%-协调及时，效果较好";
            itemList.Add(item);
            return itemList;
        }
        /// <summary>
        /// 版块负责人月度考核项目：每周更新Pitch paper/Email cover/ EB内容，帮助组员找到针对不同客户的Pitch点 
        /// 参照《销售部月度考核（2013准事业部负责人版）2月新版.xls》
        /// </summary>
        /// <returns></returns>
        public static List<_Item> GetPitchPaperItems()
        {
            List<_Item> itemList = new List<_Item>();
            _Item item = new _Item();
            //item.ID = 0;
            //item.Name = "请选择";
            //itemList.Add(item);

            //item = new _Item();
            item.ID = 1;
            item.Name = "1%-内容几乎没有更新，效果较差";
            itemList.Add(item);
            item = new _Item();
            item.ID = 3;
            item.Name = "3%-内容更新滞后，效果一般";
            itemList.Add(item);

            item = new _Item();
            item.ID = 5;
            item.Name = "5%-内容更新及时，效果较好";
            itemList.Add(item);
            return itemList;
        }
        /// <summary>
        /// 版块负责人月度考核项目：每周销售例会
        /// 参照《销售部月度考核（2013准事业部负责人版）2月新版.xls》
        /// </summary>
        /// <returns></returns>
        public static List<_Item> GetWeeklyMeetingItems()
        {
            List<_Item> itemList = new List<_Item>();
            _Item item = new _Item();
            //item.ID = 0;
            //item.Name = "请选择";
            //itemList.Add(item);

            //item = new _Item();
            item.ID = 1;
            item.Name = "1%-每周销售例会准备不成分，组织随意，效果较差";
            itemList.Add(item);
            item = new _Item();
            item.ID = 3;
            item.Name = "3%-每周销售例会有准备，组织尚可，内容实用";
            itemList.Add(item);

            item = new _Item();
            item.ID = 5;
            item.Name = "5%-每周销售例会准备充分，组织高效，作用明显";
            itemList.Add(item);
            return itemList;
        }
        /// <summary>
        /// 版块负责人月度考核项目：每月通话时间
        /// 参照《销售部月度考核（2013准事业部负责人版）2月新版.xls》
        /// </summary>
        /// <returns></returns>
        public static List<_Item> GetMonthlyMeetingItems()
        {
            List<_Item> itemList = new List<_Item>();
            _Item item = new _Item();
            //item.ID = 0;
            //item.Name = "请选择";
            //itemList.Add(item);

            //item = new _Item();
            item.ID = 2;
            item.Name = "2%-每月通话到达4小时以下";
            itemList.Add(item);
            item = new _Item();
            item.ID = 6;
            item.Name = "6%-每月通话到达4-6小时";
            itemList.Add(item);

            item = new _Item();
            item.ID = 10;
            item.Name = "10%-每月通话到达6小时以上";
            itemList.Add(item);
            return itemList;
        }


    }
}