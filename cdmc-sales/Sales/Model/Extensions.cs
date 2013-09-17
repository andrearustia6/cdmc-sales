using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Utl;
using Model;
using System.Web.Profile;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Web.Security;
using System.Collections;

namespace Entity
{
    public static class PackageExtensions
    {
        public static decimal GetYuan(this Package item,double? rate)
        {
            var yuan = item.Prize * (decimal)rate;
            return yuan;
        }
    }

    public static class LeadExtensions
    {
        public static string GetLeadStatus(this Lead item, int? porjectid)
        {
            var call = CH.GetAllData<LeadCall>(lc => lc.CompanyRelationship.ProjectID == porjectid && lc.LeadID == item.ID).OrderByDescending(o => o.CreatedDate).FirstOrDefault();
            if (call == null)
                return "未打";
            else
                return call.LeadCallType.Name;

        }
    }
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
            if (item.Creator != Employee.CurrentUserName)
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
        public static DateTime? GetAvailabeTime(this LeadCall item)
        {
            if (item.CallBackDate==null) return null;
            var date = item.CallBackDate.Value;
            var districtNumber = item.Lead.SubCompanyID == null ? item.Lead.Company.DistrictNumber : item.Lead.SubCompany.DistrictNumber;
            if (districtNumber != null)
            {
                var differs = districtNumber.TimeDifference;
                date = date.AddHours(-differs);
            }
            return date;
        }
          

        public static bool CallerIsTheLoginUser(this LeadCall item)
        {
            if (item.Member.Name ==Employee.CurrentUserName)
                return true;
            else
                return false;
        }

        public static bool IsFirstPitch(this LeadCall item)
        {
            var ls = item.CompanyRelationship.LeadCalls.FindAll(lc => lc.LeadCallType.Code >= 40 && lc.Lead.Name == item.Lead.Name).OrderBy(o => o.CallDate).ToList().FirstOrDefault();
            if (ls!=null &&ls.CallDate == item.CallDate)
                return true;
            else
                return false;
        }

        #region Call Management



        public static bool IsFullPitched(this LeadCall item,LeadCall call)
        {
            return call.LeadCallType.Name == "Full Pitched" ? true : false;
        }
        public static bool IsPitched(this LeadCall item)
        {
            return item.LeadCallType.Name == "Pitched" ? true : false;
        }

        public static bool IsDMs(this LeadCall item,string leadCallType)
        {
            if (leadCallType == "Others" || leadCallType == "Blowed" || leadCallType == "Not Pitched")
                return false;
            else
                return true;
        }

        public static bool IsNewDMs(this LeadCall item)
        {
            return item.IsFirstPitch();
        }

        public static bool IsDMs(this LeadCall item)
        {
            var type = item.LeadCallType.Code;
            if (type > 3)
                return true;
            else
                return false;
        }

        public static bool IsDealClosed(this LeadCall item)
        {
            return item.LeadCallType.Name == "Closed" ? true : false;
        }

        public static bool IsQualifiedDecision(this LeadCall item)
        {
            return item.LeadCallType.Name == "Qualified Decision" ? true : false;
        }
        #endregion
    }

    public enum RoleInProject
    {
        Administrator,Director, Manager, Leader, Member, MarketInterface, ProductInterface, NotIn
    }

    public static class MemberExtensions
    {
        /// <summary>
        /// 是否有权限到crm
        /// </summary>
        /// <param name="item"></param>
        /// <param name="cr"></param>
        /// <returns></returns>
        //public static bool IsAbleToAccessTheCompanyRelationship(this Member item, CompanyRelationship cr)
        //{
          
        //    var d = cr.WhoCallTheCompanyMember();
        //    if (d.Any(i => i.Name == Employee.CurrentUser))
        //        return true;
        //    else
        //        return false;
        //}

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

        public static double? EmployeeDuration(this Member item)
        {
            ProfileBase objProfile = ProfileBase.Create(item.Name);
            object StartDate;
            DateTime startdate;
            StartDate = objProfile.GetPropertyValue("StartDate");
            DateTime.TryParse(StartDate.ToString(), out startdate);
            if (startdate.Year != 1)
            {
                var months = (DateTime.Now - startdate).Days / 30;
                return months;
            }

            return null;

        }
        public static List<LeadCall> GetMemberToCallList(this Member item, DateTime? startdate = null, DateTime? enddate = null)
        {
            startdate = startdate == null ? new DateTime(1, 1, 1) : startdate;
            enddate = enddate == null ? new DateTime(9999, 1, 1) : enddate;

            var leadcalls = from l in CH.DB.LeadCalls
                            where l.CallBackDate != null && l.MemberID == item.ID && l.CallBackDate > startdate && l.CallBackDate < enddate 
                            && CH.DB.LeadCalls.FirstOrDefault(f=>f.CallDate>l.CallBackDate && f.LeadID==l.LeadID&&f.MemberID == item.ID)==null
                            select l;
            
            return leadcalls.OrderByDescending(o => o.CallBackDate).ToList().Distinct(new LeadCallLeadDistinct()).ToList();
        }
        /// <summary>
        /// 取得带拨打电话列表
        /// </summary>
        /// <returns></returns>
        //public static List<ViewMemberLeadToCall> GetMemberToCallList(this Member item, DateTime? startdate = null, DateTime? enddate = null)
        //{
        //    var lcs = CH.GetAllData<LeadCall>(lc =>lc.Member.Name==item.Name && lc.CompanyRelationship.ProjectID == item.ProjectID && lc.CallBackDate != null);

        //    startdate = startdate == null ? new DateTime(1, 1, 1) : startdate;
        //    enddate = enddate == null ? new DateTime(9999, 1, 1) : enddate.Value.AddDays(+1);

        //    lcs = lcs.FindAll(l => l.CallBackDate >= startdate && l.CallBackDate <= enddate);

        //    var list = new List<ViewMemberLeadToCall>();
        //    lcs.ForEach(l =>
        //    {
        //        var nd = new ViewMemberLeadToCall() { LeadCall = l };
        //        list.Add(nd);
        //    });
        //    return list;
        //}
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

        //public static bool IsAbleToAccessTheCompanyRelationship(this Member item, int? crid)
        //{
        //    var cr = CH.GetDataById<CompanyRelationship>(crid);
        //    return item.IsAbleToAccessTheCompanyRelationship(cr);
        //}

        public static ViewLeadCallAmount CallAmount(this Member item, List<ViewPhoneInfo> cs, DateTime? startdate, DateTime? enddate)
        {
            startdate = startdate == null ? new DateTime(1, 1, 1) : startdate;
            enddate = enddate == null ? new DateTime(9999, 1, 1) : enddate;
            var property = Employee.GetProfile("Contact",item.Name);
            string phone = property == null ? string.Empty : property.ToString();
            
            var result = new ViewLeadCallAmount() { Member = item,Phone = phone};
            var phonerecord = cs.FirstOrDefault(c => c.Phone == phone);

            if (phonerecord!=null)
            {
                result.Duration = phonerecord.Duration;
                result.Cold_Calls = phonerecord.CallSum;
            }
            
            var lcs = CH.GetAllData<LeadCall>(l => l.MemberID == item.ID);

            var dealins = CH.GetAllData<Deal>(d => d.Abandoned==false && d.Sales == item.Name && d.SignDate >= startdate && d.SignDate <= enddate && d.ProjectID == item.ProjectID);
            var checkins = CH.GetAllData<Deal>(d => d.Abandoned == false &&  d.Sales == item.Name && d.ActualPaymentDate >= startdate && d.ActualPaymentDate <= enddate && d.ProjectID == item.ProjectID);
            decimal checkinamount=0;
            decimal dealinamount=0;

            dealins.ForEach(da => {
                dealinamount += da.Payment;
            });
            checkins.ForEach(da =>
            {
                checkinamount += da.Income;
            });

            result.DealInAmount = dealinamount;
            result.CheckInAmount = checkinamount;

            lcs.FindAll(lc => lc.CallDate > startdate && lc.CallDate < enddate).ForEach(l =>
            {
                result.CallListAmount++;

                if (l.LeadCallType.Code > 30)
                    result.DMs++;
                if (l.IsFirstPitch())
                    result.New_DMs++;

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
        public static ViewProjectProgressAmount GetProjectMemberProgress(this Project item, string member, DateTime? startdate = null, DateTime? enddate = null)
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
            decimal rmbdeal = 0;
            decimal usddeal = 0;
            var crs = CH.GetAllData<CompanyRelationship>(c => c.ProjectID == item.ID, "Deals");

            item.CompanyRelationships.ForEach(c =>
            {

                totalcheckin += c.Deals.FindAll(d => d.Sales == member).Sum(d => d.Income);
                totaldeal += c.Deals.FindAll(d => d.Sales == member).Sum(d => d.Payment);
                rmbdeal += c.Deals.FindAll(d => d.Sales == member && d.Currencytype.Name == "RMB").Sum(d => d.Payment);
                usddeal += c.Deals.FindAll(d => d.Sales == member && d.Currencytype.Name == "USD").Sum(d => d.Payment);
            });

            item.CompanyRelationships.ForEach(c =>
            {
                
                deal += c.Deals.FindAll(d => d.Sales == member).FindAll(ds => ds.SignDate < enddate && ds.SignDate > startdate).Sum(d => d.Income);
                checkin += c.Deals.FindAll(d => d.Sales == member).FindAll(ds => ds.ActualPaymentDate < enddate && ds.ActualPaymentDate > startdate).Sum(d => d.Payment);
            });

           
            var result = new ViewProjectProgressAmount()
            {
                TotalCheckIn = totalcheckin,
                TotalDealIn = totaldeal,
                CheckIn = checkin,
                DealIn = deal,
                RMBDealIn=rmbdeal,
                USDDealIn=usddeal,
                Project = item
            };
            return result;

        }

        public static List<Deal> GetProjectDeals(this Project item, DateTime? startdate=null, DateTime? enddate=null)
        {
            if (startdate == null) startdate = new DateTime(1000, 1, 1);
            if (enddate == null) enddate = new DateTime(9000, 1, 1);

            var deals = CH.GetAllData<Deal>(d => d.ProjectID == item.ID);
            deals = deals.FindAll(d => d.ExpectedPaymentDate >= startdate.Value && d.ExpectedPaymentDate <= enddate.Value.AddDays(1));
            return deals;
        }

        public static List<CompanyRelationship> GetCRM(this Project item)
        {
            return item.GetCRMbyUserName(Employee.CurrentUserName);
        }

        public static List<CompanyRelationship> GetCRMbyUserName(this Project item, string user)
        {
            var data = new List<CompanyRelationship>();
            if (item == null) return data;
            var cs = CH.GetAllData<CompanyRelationship>(c => c.ProjectID == item.ID&& c.Members.Any(s=>s.Name==user),"Company","Members");

            return cs;

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
            if (username == null) username = Employee.CurrentUserName;
            return item.Members.FirstOrDefault(m => m.Name == username);
        }

        public static List<ViewLeadCallAmount> GetProjectMemberLeadCalls(this Project item,  List<ViewPhoneInfo> cs, DateTime? startdate=null, DateTime? enddate=null)
        {
            var list = new List<ViewLeadCallAmount>();
            if (item.Members != null)
            {
                item.Members.ForEach(m =>
                {
                    if (m.IsActivated==true)
                      list.Add(m.CallAmount(cs,startdate, enddate));
                });
            }
            return list;
        }

        public static Member GetProjectMemberByName(this Project item, string username = null)
        {
            if (username == null) username = Employee.CurrentUserName;
            return item.Members.FirstOrDefault(m => m.Name == username);
        }

        public static  IEnumerable<Lead> ProjectLeads(this Project item, int? dealcondition, int? distinctnumber, string categories = "")
        {
            var list = new List<Lead>();
            //item.CompanyRelationships.ForEach(c =>
            //{
            //    list.AddRange(CH.GetAllData<Lead>(l => l.CompanyID == c.CompanyID && l.EMail != null));
            //});   var crms = item.CompanyRelationships;
            IEnumerable<CompanyRelationship> query = item.CompanyRelationships;
            if (dealcondition != null)
            {
                if (dealcondition == 0)
                {
                    query = query.Where(q => (q.Deals.Count() == 0));
                }
                else
                {
                    query = query.Where(q => (q.Deals.Count() > 0));
                }
            }
            if (distinctnumber != null && distinctnumber != 0)
            {
                var lists =  CH.GetAllData<DistrictNumber>();
                int max = lists.Max(d => d.ID);
                if (distinctnumber == (max + 1))
                {
                    query = query.Where(q => q.Company.DistrictNumberID != null);
                }
                else
                {
                    query = query.Where(q => q.Company.DistrictNumberID == distinctnumber);
                }
            }
            else if (distinctnumber == null)
            {
                query = query.Where(q => q.Company.DistrictNumberID == null);
            }

            if (!String.IsNullOrEmpty(categories))
            {
                List<int> catId = new List<int>();
                List<string> catarr = categories.Split(',').ToList();
                if ((catarr != null) && (catarr.Count > 0))
                {
                    foreach (string cat in catarr)
                    {
                        if (!string.IsNullOrEmpty(cat))
                        {
                            catId.Add(Convert.ToInt32(cat));
                        }
                    }
                }
                query = query.Where(q => q.Categorys.Any(c => catId.Any(a => a == c.ID)));
            }

            var leads = query.Select(s => s.Company).SelectMany(s => s.Leads);

            return leads;
        }

        public static RoleInProject RoleInProject(this Project item)
        {
            var p = CH.GetDataById<Project>(item.ID);
            var name = Employee.CurrentUserName;

            if (Employee.CurrentRole.Level == 99999)
                return Entity.RoleInProject.Administrator;
            if (Employee.CurrentRole.Level == 1000)
                return Entity.RoleInProject.Director;
            else if (p.Manager == name)
                return Entity.RoleInProject.Manager;
            else if (p.TeamLeader == name)
                return Entity.RoleInProject.Leader;
            else if (p.Product == name)
                return Entity.RoleInProject.ProductInterface;
            else if (p.Market == name)
                return Entity.RoleInProject.MarketInterface;
            else if (p.Members!=null && p.Members.Any(m => m.Name == name))
                return Entity.RoleInProject.Member;
            else
                return Entity.RoleInProject.NotIn;
        }
    }
    public static class DealExtensions
    {
        public static bool SalesIsTheLoginUser(this Deal item)
        {
            if (item.Sales == Employee.CurrentUserName)
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
                return CH.GetDataById<CompanyRelationshipChildItem>(item.CompanyRelationshipID).CompanyRelationship.ProjectID;
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
        public static List<Member> WhoCallTheCompanyMember(this CompanyRelationship item, List<Member> members)
        {
            if (members == null)
            {
                members = CH.GetAllData<Member>(m => m.ProjectID == item.ProjectID);
            }
            var ms = members.FindAll(m => m.CompanyRelationships.Any(c => c.ID == item.ID));
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

        public static string WhoCallTheCompanyMemberName(this CompanyRelationship item,List<Member> members=null)
        {
            var ml = item.WhoCallTheCompanyMember(members);

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
            var cs = CH.GetDataById<CompanyRelationship>(item.ID);
            string result = string.Empty;
            cs.Categorys.ForEach(c =>
            {
                if (string.IsNullOrEmpty(result))
                    result = c.Name;
                else
                    result += "|" + c.Name;
            });
            item.CategoryString = result;
            return result;
        }
    }





}