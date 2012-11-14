using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entity;
using Utl;
using System.Web.Security;
using Sales.Model;

namespace BLL
{
    public class CRM_Logical
    {
        #region Project Management
        public List<Project> GetProjectByCurrentRole()
        {
            var currentuser = HttpContext.Current.User.Identity.Name;
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
                    projects = CH.GetAllData<Project>("Members");
                    projects = projects.FindAll(p=>p.Members.Any(m=>Employee.IsEqualToCurrentUserName(m.Name)));
                }
            }
            return projects;
        }

        public static TargetOfWeek GetTargetOfWeek(int projectid,int targetofmonth,string member,DateTime startdate,DateTime enddate)
        {
            var data = CH.GetAllData<TargetOfWeek>(tw => tw.ProjectID == projectid && tw.TargetOfMonthID == targetofmonth&&tw.Member==member);
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
            return new List<string>() { "A", "B", "C", "D", "E", "F", "G", "H", "I", "Z", "K", "L", "M", "N", 
                "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        }
       
        public static bool IsCompanySelectedForProject(Company c, int projectid)
        {
            var p = CH.GetAllData<Project>(i=>i.ID == projectid,"Companys").FirstOrDefault();

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

        #region Call Management

        

        public static bool IsFullPitched(LeadCall call)
        {
            return call.LeadCallType.Name=="Full Pitched"?true:false;
        }
         public static bool IsPitched(LeadCall call)
        {
            return call.LeadCallType.Name == "Pitched" ? true : false;
        }
        
        
        //public static TargetOfPackage GetTargetOfPackage(Lead lead,object projectid)
        //{
        //    if (projectid == null)
        //        return new TargetOfPackage() { LeadID = lead.ID };

        //    var pid = (int)projectid;

        //    if (lead.TargetOfPackages != null)
        //    {
        //        var tp = lead.TargetOfPackages.FirstOrDefault(i => i.ProjectID == pid && i.LeadID == lead.ID);
        //        if (tp == null) tp = new TargetOfPackage() { LeadID = lead.ID, ProjectID = pid };
        //        return tp;
        //    }
        //    return new  TargetOfPackage() { LeadID = lead.ID, ProjectID = pid };
        //}
        public static bool IsDMS(string leadCallType)
        {
            if (leadCallType == "Others" || leadCallType == "Blowed" || leadCallType == "Not Pitched")
                return false;
            else
                return true;
        }

        public static bool IsNewDMS(LeadCall call)
        {
            return call.IsFirstPitch();
        }

        public static bool IsDMS(LeadCall call)
        {
            var type = call.LeadCallType.Code;
            if (type > 3)
                return true;
            else
                return false;
        }

        public static bool IsDealClosed(LeadCall call)
        {
            return call.LeadCallType.Name == "Closed" ? true : false;
        }

        public static bool IsQualifiedDecision(LeadCall call)
        {
            return call.LeadCallType.Name == "Qualified Decision" ? true : false;
        }

        public static string GetLeadStatus(int? porjectid, Lead lead)
        {
            var call = CH.GetAllData<LeadCall>(lc => lc.CompanyRelationship.ProjectID == porjectid && lc.LeadID == lead.ID).OrderByDescending(o=>o.CreatedDate).FirstOrDefault();
            if (call == null)
                return "未打";
            else
                return call.LeadCallType.Name;

        }
        #endregion

        public static List<Deal> GetProjectDeals(Project p, DateTime? startdate, DateTime? enddate)
        {
            if (startdate == null) startdate = new DateTime(1000, 1, 1);
            if (enddate == null) enddate = new DateTime(9000, 1, 1);
            var deals = GetProjectDeals(p);
            deals = deals.FindAll(d => d.ExpectedPaymentDate >= startdate.Value && d.ExpectedPaymentDate <= enddate.Value.AddDays(1));
            return deals;
        }

        public static List<Deal> GetProjectDeals(Project p)
        {
            List<Deal> deals = new List<Deal>();
            if (p.CompanyRelationships != null)
            {
                p.CompanyRelationships.ForEach(cr =>
                {
                    var ds = CH.GetAllData<Deal>(d => d.CompanyRelationshipID == cr.ID);
                    deals.AddRange(ds);
                });
            }
            return deals;
        }

        #region Reports
        public static WeeklyReport GenerateSingleWeeklyReport(Project project,DateTime? settime)
        {
            var result = new WeeklyReport(project, (DateTime)settime);
            return result;
        }

        public static List<WeeklyReport> GenerateWeeklyReports(List<Project> projects, DateTime? settime)
        {
            var result = new List<WeeklyReport>();
            foreach (var p in projects)
            {
                result.Add(GenerateSingleWeeklyReport(p, settime));
            }
            return result;
        }
        #endregion

        public static List<Project> GetUserInvolveProject()
        {
            var lvl = Employee.GetCurrentRoleLevel();
            if (lvl == 5)
            {
                return GetProductInvolveProject();
            }

            if (lvl == 1)
            {
                return GetProductInvolveProject();
            }

            if (lvl >= 10 && lvl <= 100)
            {
                return GetSalesInvolveProject();
            }

            return new List<Project>();
        }
        /// <summary>
        /// 取得sales正在参与的，已经激活，并且当前时间在项目周期内的项目
        /// </summary>
        /// <returns></returns>
        public static List<Project> GetSalesInvolveProject()
        {
            var name = HttpContext.Current.User.Identity.Name;
            var now = DateTime.Now;
            var projects = CH.GetAllData<Project>("Members");

            var data = projects.FindAll(p=>p.Members.Any(m=>m.Name == name)&& p.IsActived==true && now>p.StartDate && now<p.EndDate);
            return data;
        }

        public static List<Project> GetProductInvolveProject()
        {
            var name = HttpContext.Current.User.Identity.Name;
            var now = DateTime.Now;
            var projects = CH.GetAllData<Project>("Members");

            var data = projects.FindAll(p => p.Product == name && p.IsActived == true && now > p.StartDate && now < p.EndDate);
            return data;
        }

        public static List<Project> GetMarketInvolveProject()
        {
            var name = HttpContext.Current.User.Identity.Name;
            var now = DateTime.Now;
            var projects = CH.GetAllData<Project>("Members");

            var data = projects.FindAll(p => p.Market == name && p.IsActived == true && now > p.StartDate && now < p.EndDate);
            return data;
        }
    }
}