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
                    projects = CH.GetAllData<Project>(p => Employee.IsEqualToCurrentUserName(p.Leader));
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

        public static bool IsSameMemberExistInProject(string name, int? projectid)
        {
            var p = CH.GetAllData<Project>(i => i.ID == projectid, "Members").FirstOrDefault();
            if (p != null && p.Members.FirstOrDefault(c => c.Name == name)!=null)
                return true;
            else
                return false;
        }

        public static bool IsMemberExist(string name)
        {
            var user = Membership.GetUser(name);
            return user == null ? false : true;
        }

        /// <summary>
        /// 查看当前谁在给这家公司打电话 
        /// </summary>
        /// <param name="companyid"></param>
        /// <param name="projectid"></param>
        /// <returns></returns>
        public static List<Member> GetMemberWhoCallTheCompany(int companyid, int projectid)
        {
            var com = CH.GetAllData<CompanyRelationship>(c => c.ID == companyid, "Members").FirstOrDefault();
            var project = CH.GetAllData<Project>(p => p.ID == projectid, "Members").FirstOrDefault();
            List<Member> result = new List<Member>();
            
            com.Members.ForEach(m => {
                if (project.Members.Any(pm => pm.ID == m.ID))
                    result.Add(m);
            });

            //如果公司上没有直接指定，按字头分配查找
            if (result.Count == 0)
            {
                project.Members.ForEach(m=>{
                    if (!string.IsNullOrEmpty(m.Characters))
                    {
                        var chars = m.Characters.Split('|').ToList();
                        chars.ForEach(ch =>
                        {
                            if ((!string.IsNullOrEmpty(com.Company.Name_CH)&&com.Company.Name_CH.StartsWith(ch)) ||
                                (!string.IsNullOrEmpty(com.Company.Name_EN) && com.Company.Name_EN.StartsWith(ch.ToLower())))
                            {
                                result.Add(m);
                            }
                        });
                    }
                   
                });
            }

            return result;
        }

        public static string GetMemberNameWhoCallTheCompany(int companyid, int projectid)
        {
            var ml = GetMemberWhoCallTheCompany(companyid, projectid);
            string ms = string.Empty;

            ml.ForEach(m => {
                if(ms==string.Empty)
                    ms += m.Name;
                else
                    ms += "|"+ m.Name;
            });

            return ms;
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
            return call.IsFirstPitch;
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

        public static string GetLeadStatus(int porjectid, Lead lead)
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

        #region Add.....
        public static bool TryAddCompany(Company c)
        {
            bool exist = CH.GetAllData<Company>().Any(ex => ex.Name_EN == c.Name_EN || (!string.IsNullOrEmpty(ex.Name_CH) && ex.Name_CH == c.Name_CH));
            if (!exist)
            {
                CH.Create<Company>(c);
                return true;
            }
            return false;
        }

        public static bool TryAddCompanyRelationship(CompanyRelationship c, int projectid)
        {
            var p = CH.GetDataById<Project>(projectid, "CompanyRelationships");
            bool exist = p.CompanyRelationships.Any(ex => ex.CompanyID == c.CompanyID);
            if (!exist)
            {
                CH.Create<CompanyRelationship>(c);
                return true;
            }
            return false;
        }
        #endregion
    }
}