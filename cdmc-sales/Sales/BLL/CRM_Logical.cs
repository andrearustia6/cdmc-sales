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
            var currentuser = Employee.GetCurrentUserName();
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
            var name = Employee.GetCurrentUserName();
            var now = DateTime.Now;
            var projects = CH.GetAllData<Project>("Members");

            var data = projects.FindAll(p=>p.Members.Any(m=>m.Name == name)&& p.IsActived==true );
            return data;
        }

        public static List<Project> GetManagerInvolveProject()
        {
            var name = Employee.GetCurrentUserName();
            var now = DateTime.Now;
            var projects = CH.GetAllData<Project>(p => p.Manager == name);

            var data = projects.FindAll(p => p.IsActived == true );
            return data;
        }

        public static List<Project> GetDirectorInvolveProject()
        {
            var name = Employee.GetCurrentUserName();
            var now = DateTime.Now;
            var projects = CH.GetAllData<Project>();

            var data = projects.FindAll(p =>  p.IsActived == true );
            return data;
        }

        public static List<Project> GetProductInvolveProject()
        {
            var name = Employee.GetCurrentUserName();
            var now = DateTime.Now;
            var projects = CH.GetAllData<Project>(p => p.Product == name);
            var data = projects.FindAll(p => p.IsActived == true );
            return data;
        }

        public static List<Project> GetMarketInvolveProject()
        {
            var name = Employee.GetCurrentUserName();
            var now = DateTime.Now;
            var projects = CH.GetAllData<Project>("Members");

            var data = projects.FindAll(p => p.Market == name && p.IsActived == true );
            return data;
        }
    }
}