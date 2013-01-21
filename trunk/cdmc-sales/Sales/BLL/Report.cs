using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model;
using Utl;
using Entity;

namespace BLL
{
    public class Report
    {
        private List<int>  SetProjectSelectedList(List<int> selectedprojects)
        {
            if(selectedprojects == null)
            {
                 selectedprojects = BLL.CRM_Logical.GetUserInvolveProject().Select(p => p.ID).ToList();
            }
            return selectedprojects;
        }
        public static List<ViewProjectProgressAmount> GetProjectProgressList(List<int> selectedprojects, bool? isActivated, DateTime? startdate, DateTime? enddate)
        {
            startdate = startdate == null ? new DateTime(1, 1, 1) : startdate;
            enddate = enddate == null ? new DateTime(9999, 1, 1) : startdate;
            var list = new List<ViewProjectProgressAmount>();
            if (selectedprojects == null)
            {
                selectedprojects = CRM_Logical.GetUserInvolveProject().Select(p => p.ID).ToList();
            }
            if (selectedprojects != null)
            {
                var totaldeals = from d in CH.DB.Deals
                                 where selectedprojects.Any(sp => sp == d.ProjectID)
                                 select d;

                var deals = from d in totaldeals where d.SignDate >= startdate && d.SignDate <= enddate select d;

                var totaltargetofmonths = from t in CH.DB.TargetOfMonths
                                          where selectedprojects.Any(sp => sp == t.ProjectID)
                                          select t;

                var targetofmonths = from t in totaltargetofmonths
                                     where (t.StartDate >= startdate && t.StartDate <= enddate) || (t.StartDate >= startdate && t.StartDate <= enddate)
                                     select t;

                var projects = from p in CH.DB.Projects
                               where selectedprojects.Any(sp => sp == p.ID) && p.EndDate >= startdate && p.EndDate <= enddate
                               select p;



                foreach (var p in projects.ToList())
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

                        v.CheckInPercentage = (int)((v.CheckIn / v.CheckInTarget) * 100);
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
            }

            return list;
        }

        public static List<ViewProjectMemberProgressAmount> GetMemberProgressList(List<int> selectedprojects, bool? isActivated, DateTime? startdate, DateTime? enddate)
        {
            startdate = startdate == null ? new DateTime(1, 1, 1) : startdate;
            enddate = enddate == null ? new DateTime(9999, 1, 1) : startdate;

            if (selectedprojects == null)
            {
                selectedprojects = BLL.CRM_Logical.GetUserInvolveProject().Select(p => p.ID).ToList();
            }

            var list = new List<ViewProjectMemberProgressAmount>();
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



            foreach (var p in projects.ToList())
            {
                ViewProjectMemberProgressAmount pm = new ViewProjectMemberProgressAmount();
                pm.Project = p;
                var tms = from m in p.Members where p.IsActived == true select m;
                var memlist = new List<ViewMemberProgressAmount>();
                foreach (var m in tms)
                {
                    ViewMemberProgressAmount v = new ViewMemberProgressAmount();
                    v.Member = m;
                    var projectdeals = from d in deals where d.Sales == m.Name && d.ProjectID == p.ID select d;
                    var projecttargets = from t in targetofmonths where t.MemberID == m.ID && t.ProjectID == p.ID select t;

                    var projecttotaldeals = from d in totaldeals where d.Sales == m.Name && d.ProjectID == p.ID select d;
                    var projecttotaltargets = from t in totaltargetofmonths where t.MemberID == m.ID && t.ProjectID == p.ID select t;

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
                        v.TotalDealinTarget = projecttotaltargets.Sum(t => t.Deal);
                    }
                    memlist.Add(v);
                }
                pm.ViewMemberProgressAmounts = memlist;
                list.Add(pm);
            }

            return list;

        }

        public void GetMemberPerformanceIndex(List<int> selectedproject,  int? month )
        {
            if (month == null) month = DateTime.Now.Month;

            var ps = CH.GetAllData<Project>(p => selectedproject.Any(sp=>sp==p.ID));

            DateTime startdate = new DateTime(DateTime.Now.Year, month.Value, 1);
            DateTime enddate = startdate;
            while(enddate.Month==startdate.Month)
            {
                var tempdate = enddate.AddDays(7);
                if (tempdate.Month == startdate.Month)
                    enddate = tempdate;
            }

            while (startdate.DayOfWeek== DayOfWeek.Monday)
            {
                startdate = startdate.AddDays(-1);
            }
            while (startdate.DayOfWeek == DayOfWeek.Friday)
            {
                startdate = startdate.AddDays(-1);
            }

            Utl.Utl.GetCallsInfo(ps, startdate, enddate);

        }
      
    }
}