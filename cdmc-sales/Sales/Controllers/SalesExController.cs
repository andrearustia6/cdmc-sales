using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utl;
using Model;
using Telerik.Web.Mvc;
using Entity;

namespace Sales.Controllers
{
    public class SalesExController : Controller
    {
        //
        // GET: /SalesEx/
        public ActionResult Index()
        {
            var d = new AjaxCrmTypedList();
            d.AllCRMs = GetCrmDataQuery();
            d.CustomCrmGroups = GetcustomCrmGroupDataQuery();
            return View(d);
        }

        private IQueryable<AjaxGroupedCRM> GetcustomCrmGroupDataQuery()
        {
            var user = Employee.CurrentUserName;
            var data = from f in CH.DB.UserFavorsCrmGroups
                       where f.UserName == user
                       select new AjaxGroupedCRM()
                       {
                           ID = f.ID,
                           UserName = f.UserName,
                           DisplayName = f.DisplayName,
                           GroupedCRMs = (from u in CH.DB.UserFavorsCRMs
                                          from c in CH.DB.CompanyRelationships
                                          from m in c.Members
                                          where m.Name == user
                                          && c.ID == u.CompanyRelationshipID.Value
                                          && f.UserFavorsCRMs.Select(s => s.ID).Contains(u.ID)
                                          select new AjaxCRM
                                          {
                                              CompanyNameEN = c.Company.Name_EN,
                                              CompanyNameCH = c.Company.Name_CH,
                                              CompanyContact = c.Company.Contact,
                                              Progress = c.Progress,
                                              CompanyFax = c.Company.Fax,
                                              CompanyCategories = c.Categorys,
                                              CompanyDistinct = c.Company.DistrictNumber,
                                              CompanyCreateDate = c.Company.CreatedDate,
                                              CRMID = c.ID,
                                              AjaxLeads = (from l in c.Company.Leads
                                                           select new AjaxLead
                                                           {
                                                               Blog = l.Blog,
                                                               Gender = l.Gender,
                                                               LeadPersonalEmail = l.PersonalEmailAddress,
                                                               FaceBook = l.FaceBook,
                                                               LinkIn = l.LinkIn,
                                                               WeiBo = l.WeiBo,
                                                               WeiXin = l.WeiXin,
                                                               LeadNameCH = l.Name_CH,
                                                               LeadNameEN = l.Name_EN,
                                                               LeadContact = l.Contact,
                                                               LeadDistinct = l.DistrictNumber,
                                                               LeadEmail = l.EMail,
                                                               LeadMobile = l.Mobile,
                                                               LeadTitle = l.Title,
                                                               LeadFax = l.Fax,
                                                               CRMID = c.ID,
                                                               LeadID = l.ID,
                                                               LeadCreateDate = l.CreatedDate,
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

                                          }
                                          )

                       };
            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alldata">true 为选择所有数据</param>
        /// <returns></returns>
        IQueryable<AjaxCRM> GetCrmDataQuery(bool? alldata = false)
        {

            var user = Employee.CurrentUserName;
            var custom = from cg in CH.DB.UserFavorsCRMs.Where(w => w.UserFavorsCrmGroup.UserName == user) select cg;
            var data = from c in CH.DB.CompanyRelationships
                       from m in c.Members

                       where m.Name == user && m.CompanyRelationships.Select(s => s.ID).Contains(c.ID) && (!custom.Select(s => s.CompanyRelationshipID).Contains(c.ID) || alldata.Value)
                       select new AjaxCRM
                       {
                           CompanyID = c.CompanyID,
                           CompanyNameEN = c.Company.Name_EN,
                           CompanyNameCH = c.Company.Name_CH,
                           CompanyContact = c.Company.Contact,
                           Progress = c.Progress,
                           CompanyFax = c.Company.Fax,
                           CompanyCategories = c.Categorys,
                           CompanyDistinct = c.Company.DistrictNumber,
                           CompanyCreateDate = c.Company.CreatedDate,
                           CRMID = c.ID,
                           DistrictNumberID = c.Company.DistrictNumberID,
                           ProgressID = c.ProgressID,
                           AreaID = c.Company.AreaID,
                           CompanyTypeID = c.Company.CompanyTypeID,
                           ZipCode = c.Company.ZIP,
                           WebSite = c.Company.WebSite,
                           Address = c.Company.Address,
                           Business = c.Company.Business,
                           Desc = c.Company.Description,
                           Categories = c.Categorys.Select(ca => ca.ID),
                           AjaxLeads = (from l in c.Company.Leads
                                        select new AjaxLead
                                        {
                                            Blog = l.Blog,
                                            Gender = l.Gender,
                                            LeadPersonalEmail = l.PersonalEmailAddress,
                                            FaceBook = l.FaceBook,
                                            LinkIn = l.LinkIn,
                                            WeiBo = l.WeiBo,
                                            WeiXin = l.WeiXin,

                                            LeadNameCH = l.Name_CH,
                                            LeadNameEN = l.Name_EN,
                                            LeadContact = l.Contact,
                                            LeadDistinct = l.DistrictNumber,
                                            LeadEmail = l.EMail,
                                            LeadMobile = l.Mobile,
                                            LeadTitle = l.Title,
                                            LeadFax = l.Fax,
                                            CRMID = c.ID,
                                            LeadID = l.ID,
                                            LeadCreateDate = l.CreatedDate,
                                            AjaxCalls = (from call in c.LeadCalls.Where(w => w.LeadID == l.ID)
                                                         select new AjaxCall
                                                         {
                                                             CallDate = call.CallDate,
                                                             CallBackDate = call.CallBackDate,
                                                             CallType = call.LeadCallType.Name,
                                                             Caller = call.Member.Name,
                                                             LeadCallTypeCode = call.LeadCallType.Code,
                                                             LeadCallID=call.ID

                                                         })
                                        })

                       };

            return data;
        }

        /// <summary>
        /// 导航选中的公司或者lead
        /// </summary>
        /// <param name="indexs"></param>
        /// <returns></returns>
        public PartialViewResult _SelectedList(string indexs)
        {
            var ids = indexs.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var crmid = int.Parse(ids[0]);
            var leadid = 0;
            if (ids.Count() > 1)
            {
                leadid = int.Parse(ids[1]);
            }
            var data = GetCrmDataQuery(true).FirstOrDefault(f => f.CRMID == crmid);


            if (leadid != 0 && data != null)
            {
                data.AjaxLeads = data.AjaxLeads.Where(w => w.LeadID == leadid);
            }

            return PartialView(@"~\views\salesex\salesexitem.cshtml", data);
        }

        /// <summary>
        /// 放弃可打公司，只把member从companyrelationship中移除，不删除客户信息
        /// </summary>
        /// <param name="crmid"></param>
        /// <returns></returns>
        public PartialViewResult _CrmBlowed(string crmid)
        {
            var sourcecrmid = Int32.Parse(crmid);
            var user = Employee.CurrentUserName;

            //移除自定义组里面crm记录
            var hasassigntouser = CH.DB.UserFavorsCrmGroups.Count(i => i.UserName == user && i.UserFavorsCRMs.Select(s => s.CompanyRelationshipID).Contains(sourcecrmid)) > 0;
            if (hasassigntouser)
            {
                var favorcrms = CH.DB.UserFavorsCrmGroups.Where(i => i.UserName == user).SelectMany(s => s.UserFavorsCRMs).Where(w => w.CompanyRelationshipID == sourcecrmid);
                foreach (var f in favorcrms)
                {
                    CH.DB.Set<UserFavorsCRM>().Remove(f);
                }
                CH.DB.SaveChanges();
            }
            //移除销售对crm公司的分配
            var crm = CH.GetDataById<CompanyRelationship>(sourcecrmid);
            var member = crm.Members.FirstOrDefault(f => f.Name == user);
            crm.Members.Remove(member);
            CH.Edit<CompanyRelationship>(crm);

            return PartialView(@"~\views\salesex\MainNavigationContainer.cshtml", GetNavigationBar());
        }

        AjaxCrmTypedList GetNavigationBar()
        {
            var d = new AjaxCrmTypedList();
            d.AllCRMs = GetCrmDataQuery();
            d.CustomCrmGroups = GetcustomCrmGroupDataQuery();
            return d;
        }

        /// <summary>
        /// 把选中的公司分到指定的分组
        /// </summary>
        /// <param name="indexs"></param>
        /// <returns></returns>
        public PartialViewResult _CopyCrmToGroup(string crmid, string groupid)
        {
            var sourcecrmid = Int32.Parse(crmid);
            var targetgroupid = Int32.Parse(groupid);

            var target = CH.GetDataById<UserFavorsCrmGroup>(targetgroupid);
            RemoveFromGroup(crmid);

            target.UserFavorsCRMs.Add(new UserFavorsCRM() { CompanyRelationshipID = sourcecrmid, UserFavorsCrmGroupID = targetgroupid });
            CH.Edit<UserFavorsCrmGroup>(target);
            var username = Employee.CurrentUserName;
            return PartialView(@"~\views\salesex\MainNavigationContainer.cshtml", GetNavigationBar());
        }

        public void RemoveFromGroup(string crmid)
        {
            var sourcecrmid = Int32.Parse(crmid);
            var username = Employee.CurrentUserName;
            var targets = CH.GetAllData<UserFavorsCrmGroup>(w => w.UserName == username && w.UserFavorsCRMs.Any(a => a.CompanyRelationshipID == sourcecrmid));


            targets.ForEach(f =>
            {
                var sources = f.UserFavorsCRMs.FindAll(a => a.CompanyRelationshipID == sourcecrmid);
                sources.ForEach(s =>
                {
                    f.UserFavorsCRMs.Remove(s);
                });

                CH.Edit<UserFavorsCrmGroup>(f);
            });
        }
        public PartialViewResult _CrmRemove(string crmid)
        {
            RemoveFromGroup(crmid);
            var data = GetcustomCrmGroupDataQuery();
            return PartialView(@"~\views\salesex\MainNavigationContainer.cshtml", GetNavigationBar());
        }


        /// <summary>
        /// 刷新导航栏
        /// </summary>
        /// <returns></returns>
        public PartialViewResult _RefreshCrmList()
        {
            return PartialView(@"~\views\salesex\MainNavigationContainer.cshtml", GetNavigationBar());
        }

        [GridAction]
        public ActionResult _CallIndex(int? leadid)
        {
            var user = Employee.CurrentUserName;
            var calls = from call in CH.DB.LeadCalls
                        where call.LeadID == leadid
                        select new AjaxCall
                        {
                            LeadCallID = call.ID,
                            CallType = call.LeadCallType.Name,
                            Result = call.Result,
                            CallDate = call.CallDate,
                            Caller = call.Member.Name,
                            CallBackDate = call.CallBackDate,
                            Editable = user == call.Member.Name ? true : false
                        };

            return View(new GridModel<AjaxCall> { Data = calls.OrderByDescending(o => o.CallDate) });
        }

        [HttpPost]
        public ActionResult EditSaleCompany(AjaxCRM ajaxCRM)
        {
            CompanyRelationship companyRelationship = CH.GetAllData<CompanyRelationship>(c => c.CompanyID == ajaxCRM.CompanyID).First();
            companyRelationship.Company.Address = ajaxCRM.Address;
            companyRelationship.Company.AreaID = ajaxCRM.AreaID;
            companyRelationship.Company.Business = ajaxCRM.Business;
            companyRelationship.Company.CompanyTypeID = ajaxCRM.CompanyTypeID;
            companyRelationship.Company.Contact = ajaxCRM.CompanyContact;
            companyRelationship.Company.Description = ajaxCRM.Desc;
            companyRelationship.Company.DistrictNumberID = ajaxCRM.DistrictNumberID;
            companyRelationship.Company.Fax = ajaxCRM.CompanyFax;
            companyRelationship.Company.Name_CH = ajaxCRM.CompanyNameCH;
            companyRelationship.Company.Name_EN = ajaxCRM.CompanyNameEN;
            companyRelationship.Company.WebSite = ajaxCRM.WebSite;
            companyRelationship.Company.ZIP = ajaxCRM.ZipCode;
            companyRelationship.Description = ajaxCRM.Desc;
            companyRelationship.ProgressID = ajaxCRM.ProgressID;
            companyRelationship.Categorys.Clear();
            if (ajaxCRM.Categories == null)
            {
                ajaxCRM.Categories = new List<int>() { };
            }
            companyRelationship.Categorys = CH.GetAllData<Category>(c => ajaxCRM.Categories.Contains(c.ID)).ToList();
            CH.Edit<CompanyRelationship>(companyRelationship);
            return null;
        }

        [ValidateInput(false)]
        public ActionResult CheckCompanyExist(string beforeUpdate, string afterUpdate)
        {
            if (CH.GetAllData<CompanyRelationship>(c => c.MarkForDelete == false && c.Company.Name_CH == afterUpdate && c.Company.Name_CH != beforeUpdate).Count > 0)
            {
                return Content("同名公司名字已存在！");
            }

            return Content("");
        }

        public ActionResult GetEditLead(int leadId)
        {
            Lead lead = CH.GetAllData<Lead>(c => c.ID == leadId).First();
            AjaxViewLead ajaxViewLead = new AjaxViewLead()
            {
                CompanyId = lead.CompanyID.Value,
                Address = lead.Address,
                Birthday = lead.Birthday,
                CellPhone = lead.Contact,
                Department = lead.Department,
                Desc = lead.Description,
                Fax = lead.Fax,
                Gender = lead.Gender,
                LeadId = lead.ID,
                Name_CN = lead.Name_CH,
                Name_EN = lead.Name_EN,
                PersonelEmail = lead.PersonalEmailAddress,
                Title = lead.Title,
                Telephone = lead.Contact,
                WorkingEmail = lead.EMail,
                Zip = lead.ZIP,
                WeiBo = lead.WeiBo,
                WeiXin = lead.WeiXin,
                LinkIn = lead.LinkIn,
                FaceBook = lead.FaceBook,
                Blog = lead.Blog

            };
            return PartialView("EditLead", ajaxViewLead);
        }

        [HttpPost]
        public ActionResult EditLead(AjaxViewLead ajaxViewLead)
        {
            Lead lead = CH.GetAllData<Lead>(c => c.ID == ajaxViewLead.LeadId).First();
            lead.Name_CH = ajaxViewLead.Name_CN;
            lead.Name_EN = ajaxViewLead.Name_EN;
            lead.CompanyID = ajaxViewLead.CompanyId;
            lead.Address = ajaxViewLead.Address;
            lead.Birthday = ajaxViewLead.Birthday;
            lead.Contact = ajaxViewLead.Telephone;
            lead.Department = ajaxViewLead.Department;
            lead.Description = ajaxViewLead.Desc;
            lead.EMail = ajaxViewLead.WorkingEmail;
            lead.Fax = ajaxViewLead.Fax;
            lead.Gender = ajaxViewLead.Gender;
            lead.Mobile = ajaxViewLead.CellPhone;
            lead.WeiBo = ajaxViewLead.WeiBo;
            lead.WeiXin = ajaxViewLead.WeiXin;
            lead.LinkIn = ajaxViewLead.LinkIn;
            lead.FaceBook = ajaxViewLead.FaceBook;
            lead.Blog = ajaxViewLead.Blog;
            CH.Edit<Lead>(lead);
            return null;
        }

        public ActionResult GetEditLeadCall(int leadCallId)
        {
            LeadCall leadCall = CH.GetAllData<LeadCall>(c => c.ID == leadCallId).First();
            AjaxViewLeadCall ajaxViewLeadCall = new AjaxViewLeadCall();
            ajaxViewLeadCall.CallId = leadCallId;
            ajaxViewLeadCall.CallBackDate = leadCall.CallBackDate;
            ajaxViewLeadCall.CallDate = leadCall.CallDate;
            ajaxViewLeadCall.CompanyRelationshipId = leadCall.CompanyRelationshipID.Value;
            ajaxViewLeadCall.CallTypeId = leadCall.LeadCallTypeID.Value;
            ajaxViewLeadCall.LeadId = leadCall.LeadID.Value;
            ajaxViewLeadCall.ProjectId = leadCall.ProjectID.Value;
            ajaxViewLeadCall.Result = leadCall.Result;

            return PartialView("EditLeadCall", ajaxViewLeadCall);
        }

        public ActionResult EditLeadCall(AjaxViewLeadCall ajaxViewLeadCall)
        {
            LeadCall leadCall = CH.GetAllData<LeadCall>(c => c.ID == ajaxViewLeadCall.CallId).First();
            leadCall.CallBackDate = ajaxViewLeadCall.CallBackDate;
            leadCall.CallDate = ajaxViewLeadCall.CallDate;
            leadCall.LeadCallTypeID = ajaxViewLeadCall.CallTypeId;
            leadCall.Member = CH.GetAllData<Member>(c => c.Name == Employee.CurrentUserName).First();
            leadCall.Result = ajaxViewLeadCall.Result;
            CH.Edit<LeadCall>(leadCall);
            return null;
        }
    }
}


#region bk
//var query = from l in CH.DB.LeadCalls where l.Member.Name == Employee.CurrentUserName
//            group l by new { l.LeadID, l.Lead.CompanyID,l.CompanyRelationshipID,l.CompanyRelationship.ProjectID } into grp
//            from c in CH.DB.Companys where c.ID == grp.Key.CompanyID
//            from p in CH.DB.Projects where p.ID == grp.Key.ProjectID
//            from crm in CH.DB.CompanyRelationships where crm.ID == grp.Key.CompanyRelationshipID
//            from l in CH.DB.Leads where l.ID == grp.Key.LeadID
//            select new AjaxCRM 
//            {
//                ProjectID = grp.Key.ProjectID.Value,
//                LeadID =  grp.Key.LeadID.Value,
//                CompanyID = grp.Key.LeadID.Value,
//                CRMID = grp.Key.CompanyRelationshipID.Value,

//                ProjectName = p.Name_CH,

//                Progress = crm.Progress,
//                HasBlowed = crm.HasBlowed,

//                CompanyNameCH = c.Name_CH,
//                CompanyNameEN = c.Name_EN,
//                CompanyContact = c.Contact,
//                CompanyFax = c.Fax,
//                CompanyDistinct = c.DistrictNumber,

//                LeadNameCH = l.Name_CH,
//                LeadNameEN = l.Name_EN,
//                LeadContact = l.Contact,
//                LeadDistinct = l.DistrictNumber,
//                LeadEmail = l.EMail,
//                LeadMobile = l.Mobile,
//                LeadTitle = l.Title,
//                LeadFax = l.Fax,


//                AjaxCalls = (from call in grp select new AjaxCall{
//                      LeadID = grp.Key.LeadID.Value,
//                      CompanyID = grp.Key.LeadID.Value,
//                      LeadCallID = call.ID,
//                      CallType = call.LeadCallType.Name,
//                      Result = call.Result,
//                      CallDate = call.CallDate,
//                      Caller = call.Member.Name,
//                      CallBackDate = call.CallBackDate      
//                })
//};
#endregion