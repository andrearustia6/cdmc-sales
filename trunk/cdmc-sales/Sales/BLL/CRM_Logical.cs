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
        public static class _Project
        {
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
        public static class _CRM
        {
            //导入公司
            public static void ImportCompany(int[] sourceprojectids, int targetprojectid, string user)
            {
                //导入的目标
                var p = CH.GetDataById<Project>(targetprojectid);
                user = "系统导入:" + user;
                //取得要导入的公司
                var comapnytoexport = CH.GetAllData<CompanyRelationship>(c =>c.ProjectID!=null && sourceprojectids.Contains(c.ProjectID.Value)).Select(s => s.Company);
                var crms = from c in comapnytoexport
                           select new CompanyRelationship()
                           {
                               CompanyID = c.ID,
                               Company = c,
                               ProjectID = p.ID,
                               Project = p,
                               Creator = user,
                               CreatedDate = DateTime.Now
                           };
               // Company.IsCompanyExist();

                //公司名在这个项目中是否存在
                var existscompanynames = from c in p.CompanyRelationships.Select(s => s.Company)
                                         select new { namech = c.Name_CH, nameen = c.Name_EN, id = c.ID };

               crms = crms.Where(c=> (string.IsNullOrEmpty(c.Company.Name_CH) || existscompanynames.Any(a=>a.namech== c.Company.Name_CH)==false)
                   && (string.IsNullOrEmpty(c.Company.Name_EN) || existscompanynames.Any(a => a.nameen == c.Company.Name_EN) == false));
               if (crms.Count() > 0)
               {
                   p.CompanyRelationships.AddRange(crms);
                   CH.Edit<Project>(p);
               }
                //CH.GetAllData<>
            }

           

            public static IEnumerable<AjaxCRM> GetCrmByProjectId(int? projectid)
            {
                if (projectid != null)
                { 
                    var crms = CH.DB.CompanyRelationships.Where(c=>c.ProjectID == projectid);
                    var ajaxcrms = from c in crms
                                   select new AjaxCRM()
                                   {
                                       CrmCreateDate = c.CreatedDate,
                                       CRMID = c.ID,
                                       CompanyCategories = c.Categorys,
                                       Members = c.Members,
                                       CompanyNameEN = c.Company.Name_EN,
                                       CompanyNameCH = c.Company.Name_CH,
                                       CrmCreator = c.Creator,
                                       CompanyType = c.Company.CompanyType.Name,
                                   };
                    return ajaxcrms;
                }

                return null;
            }
 
        }

       // public  class Company
        //{
            //public static bool IsCompanyExist(string namech, string nameen, int? projectid = null, int? comid = null)
            //{
            //    IQueryable<Company> companys;
            //    if (projectid != null)
            //    {
            //        companys = CH.DB.CompanyRelationships.Select(s => s.Company);
            //    }
            //    else
            //    {
            //        companys = CH.DB.Companys;
            //    }
            //    companys = companys.Where(w => w.Name_EN == nameen || w.Name_CH == namech);
            //    if (comid != null)
            //    {

            //    }


            //}
       // }

         //公司是否存在
     

        public static IQueryable<Deal> GetDeals(bool? acitivatedprojectonly = false, int? projectid = null, string sales = null, string filter = null)
        {
            IQueryable<Deal> deals;
            if (Utl.Utl.DebugModel() == false)
            {
                var debugmembers = from m in CH.DB.Members.Where(w => w.Test == true) select m;
                var names = debugmembers.Select(s => s.Name);
                deals = from deal in CH.DB.Deals.Where(d => names.Any(a => a == d.Sales) == false && d.Abandoned == false) select deal;
            }
            else
            {
                deals = from deal in CH.DB.Deals.Where(d => d.Abandoned == false) select deal;
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
                }
            }
            return deals;

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

            var data = projects.FindAll(p => (p.Members.Any(m => m.Name == name) || p.TeamLeader == name) && p.IsActived == true);
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
    }
}