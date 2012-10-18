using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Utl;
using Model;

namespace Entity
{
    public static class EntityBaseExtensions
    {
        public static bool SameFieldValueExist<T>(this EntityBase item, string fieldname) where T : EntityBase
        {
            var value = item.GetType().GetProperty(fieldname).GetValue(item, null);
            var data = CH.GetAllData<T>(child => child.GetType().GetProperty(fieldname).GetValue(child, null).ToString() == value.ToString());
            if (data.Count > 0) return true;
            return false;
        }
    }


    public static class FullNameEntityExtensions
    {
        public static bool IsAllNamesEmpty(this FullNameEntity item)
        {
            if (string.IsNullOrEmpty(item.Name_EN) && string.IsNullOrEmpty(item.Name_CH))
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
        public static bool IsAbleToAccessTheCompanyRelationship(this Member item, CompanyRelationship cr)
        {
            var d = cr.WhoCallTheCompanyMember();
            if (d.Any(i => i.Name == HttpContext.Current.User.Identity.Name))
                return true;
            else
                return false;
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
                if (l.IsFirstPitch)
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
        public static ViewProjectProgressAmount GetProjectProgress(this Project item,DateTime? startdate, DateTime? enddate)
        {
            startdate= startdate==null? new DateTime(1,1,1):startdate;
            enddate= enddate==null? new DateTime(9999,1,1):enddate.Value.AddDays(+1);
          
            decimal totaldeal = 0;
            decimal totalcheckin = 0;
            decimal deal = 0;
            decimal checkin = 0;
            decimal dealtarget = 0;
            decimal checkintarget = 0;
            decimal nextdealtarget = 0;
            decimal nextcheckintarget = 0;
            var crs = CH.GetAllData<CompanyRelationship>(c=>c.ProjectID == item.ID,"Deals");
            item.CompanyRelationships.ForEach(c => { 
                 totalcheckin += c.Deals.Sum(d => d.Income);
                 totaldeal += c.Deals.Sum(d => d.Payment);
            });

            item.CompanyRelationships.ForEach(c =>
            {
                 deal += c.Deals.FindAll(ds=>ds.SignDate<enddate && ds.SignDate>startdate).Sum(d => d.Income);
                 checkin+= c.Deals.FindAll(ds=>ds.ActualPaymentDate<enddate && ds.ActualPaymentDate>startdate).Sum(d => d.Payment);
            });

             item.Members.ForEach(m=>{
                 var ts = CH.GetAllData<TargetOfWeek>(t=>t.Member== m.Name).FindAll(t => (t.StartDate > startdate && t.StartDate < enddate) | (t.EndDate > startdate && t.EndDate < enddate));
                 dealtarget+=ts.Sum(t => t.Deal);
                 checkintarget+=ts.Sum(t => t.CheckIn);
             });

            var  nextweekend = enddate.Value.AddDays(7);
             item.Members.ForEach(m =>
             {
                 var ts = CH.GetAllData<TargetOfWeek>(t => t.Member == m.Name).FindAll(t => (t.StartDate > enddate && t.EndDate < nextweekend));
                 nextdealtarget += ts.Sum(t => t.Deal);
                 nextcheckintarget += ts.Sum(t => t.CheckIn);
             });
             var result = new ViewProjectProgressAmount() { 
                 TotalCheckIn= totalcheckin,
                 TotalDealIn = totaldeal,
                 CheckIn = checkin,
                 DealIn = deal,
                 LeftDay =(item.EndDate-DateTime.Now).Days,
                 CheckInTarget = checkintarget,
                 Project = item,
                 DealInTarget = dealtarget,
                 NextCheckInTarget = nextcheckintarget,
                 NextDealInTarget = nextdealtarget,
                 CheckInPercentage = checkintarget==0? 100:(int)(checkin*100/checkintarget),
                 DealInPercentage = dealtarget == 0 ? 100 : (int)(deal * 100 / dealtarget)
             };
             return result;

        }

        public static Member Project(this Project item, string username = null)
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

    public static class CompanyRelationshipExtensions
    {
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