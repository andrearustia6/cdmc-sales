using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entity;
using Utl;
using System.Web.Security;
using Model;

namespace BLL
{
    public class CRM_Logical
    {
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
            return new List<string>() { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", 
                "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        }
       
        public static bool IsCompanySelectedForProject(Company c, int projectid)
        {
            var p = CH.GetAllData<Project>(i=>i.ID == projectid).FirstOrDefault();

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

        public static IQueryable<CompanyRelationship> GetUserCallingCRM(int? projectid=null)
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

            var calls = from l in CH.DB.LeadCalls where  l.LeadCallType.Code >= 40 select l;
            if (projectids.Count > 0)
            {
                calls = calls.Where(l=>projectids.Contains(l.ProjectID.Value));
            }
            var list = calls.OrderBy(o=>o.CallDate).ToList();
            list = list.Distinct(new LeadCallLeadDistinct()).Where(w=>w.CallDate>= startdate && w.CallDate<enddate).ToList();
      
            return list;
        }

        public static List<Project> GetUserProjectRight(string funtionname)
        {
            var ps = from p in CH.DB.ProjectRights where p.Project.IsActived == true && p.AccessRights.Select(s => s.Name).Contains(funtionname) select p;

            var list = ps.ToList();


             var plist = list.FindAll(f => IsContainLoginUser(f.Name)).Select(s=>s.Project).ToList();

             
               var  plistorigin = GetUserInvolveProject();

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

            var lvl = Employee.CurrentRole.Level;
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
            if (lvl >= 1000)
            {
                return GetDirectorInvolveProject();
            }
            if (lvl >= 500 && lvl <= 500)
            {
                return GetManagerInvolveProject();
            }

            return new List<Project>();
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

            var data = projects.FindAll(p=>(p.Members.Any(m=>m.Name == name) || p.TeamLeader == name) &&  p.IsActived==true );
            return data;
        }

        public static List<Project> GetManagerInvolveProject()
        {
            var name = Employee.CurrentUserName;
            var now = DateTime.Now;
            var projects = CH.GetAllData<Project>(p => p.Manager == name);

            var data = projects.FindAll(p => p.IsActived == true );
            return data;
        }

        public static List<Project> GetDirectorInvolveProject()
        {
            var name = Employee.CurrentUserName;
            var now = DateTime.Now;
            var projects = CH.GetAllData<Project>();

            var data = projects.FindAll(p =>  p.IsActived == true );
            return data;
        }

        public static List<Project> GetProductInvolveProject()
        {
            var name = Employee.CurrentUserName;
            var now = DateTime.Now;
            var projects = CH.GetAllData<Project>(p => p.Product == name);
            var data = projects.FindAll(p => p.IsActived == true );
            return data;
        }

        public static List<Project> GetMarketInvolveProject()
        {
            var name = Employee.CurrentUserName;
            var now = DateTime.Now;
            var projects = CH.GetAllData<Project>();

            var data = projects.FindAll(p => p.Market == name && p.IsActived == true );
            return data;
        }    
    }
}