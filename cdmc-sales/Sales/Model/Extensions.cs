using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Utl;
using Model;
using System.Web.Profile;

namespace Entity
{
    public static class EntityBaseExtensions
    {
        public static bool SameFieldValueExist<T>(this EntityBase item, string fieldname) where T : EntityBase
        {
            var value = item.GetType().GetProperty(fieldname).GetValue(item, null);
            var data = CH.GetAllData<T>(child => child.GetType().GetProperty(fieldname).GetValue(child, null).ToString() == value.ToString());
            if (data.Count > 0 && data.Any(d => d.ID != item.ID)) return true;
            return false;
        }

        public static bool CreatorIsTheLoginUser(this EntityBase item)
        {
            if (item.Creator != HttpContext.Current.User.Identity.Name)
                return false;
            else
                return true;
        }
    }


    public static class NameEntityExtensions
    {
        public static bool IsAllNamesEmpty(this NameEntity item)
        {
            if (string.IsNullOrEmpty(item.Name_EN) && string.IsNullOrEmpty(item.Name_CH))
                return true;
            else
                return false;
        }
    }

    public static class LeadCallExtensions
    {
        public static bool CallerIsTheLoginUser(this LeadCall item)
        {
            if (item.Member.Name == HttpContext.Current.User.Identity.Name)
                return true;
            else
                return false;
        }

        public static bool IsFirstPitch(this LeadCall item)
        {
            var ls = item.CompanyRelationship.LeadCalls.FindAll(lc => lc.LeadCallType.Code >= 40);
            if (ls.Count > 0)
                return true;
            else
                return false;
        }
    }

    public enum RoleInProject
    {
        Director, Manager, Leader, Member, MarketInterface, ProductInterface, NotIn
    }

    public static class MemberExtensions
    {
        /// <summary>
        /// 是否有权限到crm
        /// </summary>
        /// <param name="item"></param>
        /// <param name="cr"></param>
        /// <returns></returns>
        public static bool IsAbleToAccessTheCompanyRelationship(this Member item, CompanyRelationship cr)
        {
            var d = cr.WhoCallTheCompanyMember();
            if (d.Any(i => i.Name == HttpContext.Current.User.Identity.Name))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 查看以前参加过的项目
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static List<Project> GetInvolveProjects(this Member item)
        {
            var ps = CH.GetAllData<Project>(p => p.Members.Any(m => m.Name == item.Name), "Members");
            return ps;
        }

        /// <summary>
        /// 查看以前参加过的项目
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string GetInvolveProjectsName(this Member item)
        {
            var ps = item.GetInvolveProjects();
            string name = string.Empty;
            ps.ForEach(p =>
            {
                if (string.IsNullOrEmpty(name))
                    name = p.Name;
                else
                    name += "|" + p.Name;
            });
            return name;
        }

        public static int? EmployeeDuration(this Member item)
        {
            ProfileBase objProfile = ProfileBase.Create(item.Name);
            object StartDate;
            DateTime startdate;
            StartDate = objProfile.GetPropertyValue("StartDate");
            DateTime.TryParse(StartDate.ToString(), out startdate);
            if (startdate.Year != 1)
            {
                var weeks = (DateTime.Now - startdate).Days / 7;
                return weeks;
            }

            return null;

        }

        /// <summary>
        /// 取得带拨打电话列表
        /// </summary>
        /// <returns></returns>
        public static List<ViewMemberLeadToCall> GetMemberToCallList(this Member item, DateTime? startdate = null, DateTime? enddate = null)
        {
            var lcs = CH.GetAllData<LeadCall>(lc => lc.CompanyRelationship.ProjectID == item.ProjectID && lc.CallBackDate != null);

            startdate = startdate == null ? new DateTime(1, 1, 1) : startdate;
            enddate = enddate == null ? new DateTime(9999, 1, 1) : enddate.Value.AddDays(+1);

            lcs = lcs.FindAll(l => l.CallBackDate >= startdate && l.CallBackDate <= enddate);

            var list = new List<ViewMemberLeadToCall>();
            lcs.ForEach(l =>
            {
                var nd = new ViewMemberLeadToCall() { LeadCall = l };
                list.Add(nd);
            });
            return list;
        }
        /// <summary>
        /// 员工业绩统计
        /// </summary>
        /// <returns></returns>
        public static ViewMemberProgressAmount GetMemberProgress(this Member item, DateTime? startdate = null, DateTime? enddate = null)
        {
            startdate = startdate == null ? new DateTime(1, 1, 1) : startdate;
            enddate = enddate == null ? new DateTime(9999, 1, 1) : enddate.Value.AddDays(+1);

            decimal totaldeal = 0;
            decimal totalcheckin = 0;
            decimal dealin = 0;
            decimal checkin = 0;
            decimal dealtarget = 0;
            decimal checkintarget = 0;
            decimal nextdealtarget = 0;
            decimal nextcheckintarget = 0;
            var deals = CH.GetAllData<Deal>(d => d.Sales == item.Name && d.CompanyRelationship.ProjectID == item.ProjectID);

            deals.ForEach(d =>
            {
                totalcheckin += d.Income;
                totaldeal += d.Payment;
                if (d.SignDate >= startdate && d.SignDate <= enddate)
                {

                    checkin += d.Income;
                    dealin += d.Payment;
                }
            });

            var ts = CH.GetAllData<TargetOfWeek>(t => t.Member == item.Name && t.ProjectID == item.ProjectID).FindAll(t => (t.StartDate > startdate && t.StartDate < enddate) | (t.EndDate > startdate && t.EndDate < enddate));
            dealtarget += ts.Sum(t => t.Deal);
            checkintarget += ts.Sum(t => t.CheckIn);


            var nextweekend = enddate.Value.AddDays(7);


            var nextts = CH.GetAllData<TargetOfWeek>(t => t.Member == item.Name && t.ProjectID == item.ProjectID).FindAll(t => (t.StartDate > enddate && t.EndDate < nextweekend));
            nextdealtarget += ts.Sum(t => t.Deal);
            nextcheckintarget += ts.Sum(t => t.CheckIn);

            var result = new ViewMemberProgressAmount()
            {
                TotalCheckIn = totalcheckin,
                TotalDealIn = totaldeal,
                CheckIn = checkin,
                DealIn = dealin,
                CheckInTarget = checkintarget,
                Member = item,
                DealInTarget = dealtarget,
                NextCheckInTarget = nextcheckintarget,
                NextDealInTarget = nextdealtarget,
                CheckInPercentage = checkintarget == 0 ? 100 : (int)(checkin * 100 / checkintarget),
                DealInPercentage = dealtarget == 0 ? 100 : (int)(dealin * 100 / dealtarget)
            };
            return result;

        }

        public static bool IsAbleToAccessTheCompanyRelationship(this Member item, int? crid)
        {
            var cr = CH.GetDataById<CompanyRelationship>(crid);
            return item.IsAbleToAccessTheCompanyRelationship(cr);
        }

        public static ViewLeadCallAmount CallAmount(this Member item, DateTime? startdate, DateTime? enddate)
        {
            startdate = startdate == null ? new DateTime(1, 1, 1) : startdate;
            enddate = enddate == null ? new DateTime(9999, 1, 1) : enddate;
            var result = new ViewLeadCallAmount() { Member = item };
            var lcs = CH.GetAllData<LeadCall>(l => l.MemberID == item.ID);
            lcs.FindAll(lc => lc.CallDate > startdate && lc.CallDate < enddate).ForEach(l =>
            {
                result.Cold_Calls++;

                if (l.LeadCallType.Code > 3)
                    result.DMS++;
                if (l.IsFirstPitch())
                    result.New_DMS++;

                if (l.LeadCallTypeID == 1)
                    result.Others++;
                else if (l.LeadCallTypeID == 2)
                    result.Blowed++;
                else if (l.LeadCallTypeID == 3)
                    result.Not_Pitched++;
                else if (l.LeadCallTypeID == 4)
                    result.Pitched++;
                else if (l.LeadCallTypeID == 5)
                    result.Full_Pitched++;
                else if (l.LeadCallTypeID == 6)
                    result.Call_Backed++;
                else if (l.LeadCallTypeID == 7)
                    result.Waiting_For_Approval++;
                else if (l.LeadCallTypeID == 8)
                    result.Qualified_Decision++;
                else if (l.LeadCallTypeID == 9)
                    result.Closed++;
            });

            return result;
        }

    }

    public static class ProjectExtensions
    {
        public static List<CompanyRelationship> GetCRM(this Project item)
        {
            return item.GetCRMbyUserName(HttpContext.Current.User.Identity.Name);
        }

        public static List<CompanyRelationship> GetCRMbyUserName(this Project item, string user)
        {
            var data = new List<CompanyRelationship>();

            var cs = CH.GetAllData<CompanyRelationship>(c => c.ProjectID == item.ID);

            var member = item.Members.FirstOrDefault(m => m.Name == user);
            if (member != null && member.CharactersSet != null)
            {


                cs.ForEach(i =>
                {
                    var members = i.WhoCallTheCompanyMember();
                    if (members.Any(m => m.Name == user))
                    {
                        data.Add(i);
                    }
                });
            }

            return data;

        }
        /// <summary>
        /// 添加已存在的公司到项目
        /// </summary>
        /// <param name="item"></param>
        public static void AddExistCompanyToNewCompanyRelationship(this Project item, int? companyid)
        {
            bool exist = item.CompanyRelationships.Any(ex => ex.CompanyID == companyid);
            if (!exist)
            {
                CH.Create<CompanyRelationship>(new CompanyRelationship() { CompanyID = companyid, ProjectID = item.ID, Importancy = 6 });
            }
        }

        /// <summary>
        /// 项目进展统计
        /// </summary>
        /// <param name="item"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>
        public static ViewProjectProgressAmount GetProjectProgress(this Project item, DateTime? startdate = null, DateTime? enddate = null)
        {
            startdate = startdate == null ? new DateTime(1, 1, 1) : startdate;
            enddate = enddate == null ? new DateTime(9999, 1, 1) : enddate.Value.AddDays(+1);

            decimal totaldeal = 0;
            decimal totalcheckin = 0;
            decimal deal = 0;
            decimal checkin = 0;
            decimal dealtarget = 0;
            decimal checkintarget = 0;
            decimal nextdealtarget = 0;
            decimal nextcheckintarget = 0;
            var crs = CH.GetAllData<CompanyRelationship>(c => c.ProjectID == item.ID, "Deals");

            item.CompanyRelationships.ForEach(c =>
            {
                totalcheckin += c.Deals.Sum(d => d.Income);
                totaldeal += c.Deals.Sum(d => d.Payment);
            });

            item.CompanyRelationships.ForEach(c =>
            {

                deal += c.Deals.FindAll(ds => ds.SignDate < enddate && ds.SignDate > startdate).Sum(d => d.Income);
                checkin += c.Deals.FindAll(ds => ds.ActualPaymentDate < enddate && ds.ActualPaymentDate > startdate).Sum(d => d.Payment);
            });

            item.Members.ForEach(m =>
            {
                var ts = CH.GetAllData<TargetOfWeek>(t => t.Member == m.Name).FindAll(t => (t.StartDate > startdate && t.StartDate < enddate) | (t.EndDate > startdate && t.EndDate < enddate));
                dealtarget += ts.Sum(t => t.Deal);
                checkintarget += ts.Sum(t => t.CheckIn);
            });

            var nextweekend = enddate.Value.AddDays(7);
            item.Members.ForEach(m =>
            {
                var ts = CH.GetAllData<TargetOfWeek>(t => t.Member == m.Name).FindAll(t => (t.StartDate > enddate && t.EndDate < nextweekend));
                nextdealtarget += ts.Sum(t => t.Deal);
                nextcheckintarget += ts.Sum(t => t.CheckIn);
            });
            var result = new ViewProjectProgressAmount()
            {
                TotalCheckIn = totalcheckin,
                TotalDealIn = totaldeal,
                CheckIn = checkin,
                DealIn = deal,
                LeftDay = (item.EndDate - DateTime.Now).Days,
                CheckInTarget = checkintarget,
                Project = item,
                DealInTarget = dealtarget,
                NextCheckInTarget = nextcheckintarget,
                NextDealInTarget = nextdealtarget,
                CheckInPercentage = checkintarget == 0 ? 100 : (int)(checkin * 100 / checkintarget),
                DealInPercentage = dealtarget == 0 ? 100 : (int)(deal * 100 / dealtarget)
            };
            return result;

        }

        /// <summary>
        /// 销售取得在项目中的项目成员实例
        /// </summary>
        /// <param name="item"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public static Member GetMemberInProjectByName(this Project item, string username = null)
        {
            if (username == null) username = HttpContext.Current.User.Identity.Name;
            return item.Members.FirstOrDefault(m => m.Name == username);
        }

        public static List<ViewLeadCallAmount> GetProjectMemberLeadCalls(this Project item, DateTime? startdate, DateTime? enddate)
        {
            var list = new List<ViewLeadCallAmount>();
            if (item.Members != null)
            {
                item.Members.ForEach(m =>
                {
                    list.Add(m.CallAmount(startdate, enddate));
                });
            }
            return list;
        }

        public static Member GetProjectMemberByName(this Project item, string username = null)
        {
            if (username == null) username = HttpContext.Current.User.Identity.Name;
            return item.Members.FirstOrDefault(m => m.Name == username);
        }

        public static List<Lead> ProjectLeads(this Project item)
        {
            var list = new List<Lead>();
            item.CompanyRelationships.ForEach(c =>
            {
                list.AddRange(CH.GetAllData<Lead>(l => l.CompanyID == c.CompanyID));
            });
            return list;
        }

        public static RoleInProject RoleInProject(this Project item)
        {
            var p = CH.GetDataById<Project>(item.ID, "Members");
            var name = HttpContext.Current.User.Identity.Name;

            if (Employee.GetCurrentRoleLevel() == 1000)
                return Entity.RoleInProject.Director;
            else if (p.Manager == name)
                return Entity.RoleInProject.Manager;
            else if (p.TeamLeader == name)
                return Entity.RoleInProject.Leader;
            else if (p.Product == name)
                return Entity.RoleInProject.ProductInterface;
            else if (p.Market == name)
                return Entity.RoleInProject.MarketInterface;
            else if (p.Members.Any(m => m.Name == name))
                return Entity.RoleInProject.Member;
            else
                return Entity.RoleInProject.NotIn;
        }
    }
    public static class DealExtensions
    {
        public static bool SalesIsTheLoginUser(this Deal item)
        {
            if (item.Sales == HttpContext.Current.User.Identity.Name)
                return true;
            else
                return false;
        }

        public static string CopamyName(this Deal item)
        {
            if (item.CompanyRelationship != null)
                return item.CompanyRelationship.CopamyName();
            else
                return CH.GetDataById<CompanyRelationship>(item.CompanyRelationshipID).CopamyName();
        }

        public static string ProjectName(this Deal item)
        {
            if (item.CompanyRelationship != null)
                return item.CompanyRelationship.ProjectName();
            else
                return CH.GetDataById<CompanyRelationship>(item.CompanyRelationshipID).ProjectName();
        }
    }

    public static class CompanyRelationshipChildItemExtensions
    {
        public static int? ProjectID(this CompanyRelationshipChildItem item)
        {
            if (item.CompanyRelationship != null)
                return item.CompanyRelationship.ProjectID;
            else
                return CH.GetDataById<CompanyRelationshipChildItem>(item.CompanyRelationshipID).ProjectID();
        }
    }

    public static class CompanyRelationshipExtensions
    {
        public static string CopamyName(this CompanyRelationship item)
        {
            if (item.Company != null)
                return item.Company.Name;
            else
                return CH.GetDataById<Company>(item.CompanyID).Name;
        }

        public static string ProjectName(this CompanyRelationship item)
        {
            if (item.Project != null)
                return item.Project.Name;
            else
                return CH.GetDataById<Project>(item.ProjectID).Name;
        }

        /// <summary>
        /// 查看当前谁在给这家公司打电话 
        /// </summary>
        /// <param name="companyid"></param>
        /// <param name="projectid"></param>
        /// <returns></returns>
        public static List<Member> WhoCallTheCompanyMember(this CompanyRelationship item)
        {
            var ms = CH.GetAllData<Member>(m => m.CompanyRelationships.Any(c => c.ID == item.ID), "CompanyRelationships");
            List<Member> result = new List<Member>();
            result.AddRange(ms);


            //如果公司上没有直接指定，按字头分配查找
            if (result.Count == 0)
            {

                var project = CH.GetAllData<Project>(p => p.ID == item.ProjectID, "Members").FirstOrDefault();

                project.Members.ForEach(m =>
                {
                    if (!string.IsNullOrEmpty(m.Characters))
                    {
                        var chars = m.Characters.Split('|').ToList();
                        chars.ForEach(ch =>
                        {
                            if ((!string.IsNullOrEmpty(item.Company.Name_CH) && item.Company.Name_CH.StartsWith(ch)) ||
                                (!string.IsNullOrEmpty(item.Company.Name_EN) && item.Company.Name_EN.StartsWith(ch.ToUpper())))
                            {
                                result.Add(m);
                            }
                        });
                    }

                });
            }

            return result;
        }

        public static string WhoCallTheCompanyMemberName(this CompanyRelationship item)
        {
            var ml = item.WhoCallTheCompanyMember();

            string ms = string.Empty;

            ml.ForEach(m =>
            {
                if (ms == string.Empty)
                    ms += m.Name;
                else
                    ms += "|" + m.Name;
            });

            return ms;
        }

        public static string CategoryString(this CompanyRelationship item)
        {
            //重读，确保读到refernced的category
            var cs = CH.GetDataById<CompanyRelationship>(item.ID, "Categorys");
            string result = string.Empty;
            cs.Categorys.ForEach(c =>
            {
                if (string.IsNullOrEmpty(result))
                    result = c.Name;
                else
                    result += "|" + c.Name;
            });

            return result;
        }
    }





}