using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utl;
using BLL;
using Model;
using Telerik.Web.Mvc;
using Entity;

namespace Sales.Controllers
{

    public class SalesExController : Controller
    {
        //
        // GET: /SalesEx/
        public ActionResult Index(int? crmId)
        {
            AjaxCrmTypedList ajaxCrmTypedList = GetNavigationBar();
            return View(ajaxCrmTypedList);
        }



        /// <summary>
        /// 导航选中的公司或者lead
        /// </summary>
        /// <param name="indexs"></param>
        /// <returns></returns>
        public PartialViewResult _SelectedList(string indexs, CompanyFilters filters = null)
        {
            if (!string.IsNullOrEmpty(indexs))
            {
                var ids = indexs.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                var crmid = int.Parse(ids[0]);

                var c = CH.GetDataById<CompanyRelationship>(crmid);

                var leadid = 0;
                if (ids.Count() > 1)
                {
                    leadid = int.Parse(ids[1]);
                    c.Company.Leads = c.Company.Leads.Where(l => l.ID == leadid).ToList();
                }
                var deals = CRM_Logical.GetDeals(true);
                if (c != null)
                {
                    var data = new AjaxCRM()
                    {
                        ProjectID = c.ProjectID,
                        CompanyID = c.CompanyID,
                        CompanyNameEN = c.Company.Name_EN,
                        CompanyNameCH = c.Company.Name_CH,
                        CompanyContact = c.Company.Contact,
                        Progress = c.Progress,
                        CompanyFax = c.Company.Fax,
                        CompanyCategories = c.Categorys,
                        CompanyDistinct = c.Company.DistrictNumber,
                        CompanyCreateDate = c.Company.CreatedDate,
                        CompanyPayment = CRM_Logical.GetTotalPayment(true,c.ID),
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
                                         Department = l.Department,
                                         Blog = l.Blog,
                                         Gender = l.Gender,
                                         LeadPersonalEmail = l.PersonalEmailAddress,
                                         FaceBook = l.FaceBook,
                                         LinkIn = l.LinkIn,
                                         WeiBo = l.WeiBo,
                                         WeiXin = l.WeiXin,
                                         LeadAddress = l.Address,
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
                                                          LeadCallID = call.ID,
                                                          Result = call.Result
                                                      })
                                     })
                    };
                    return PartialView(@"~\views\salesex\salesexitem.cshtml", data);
                }
                //var d = GetNavigationBar(filters);

                //var data = d.AllCRMs.Where(f => f.CRMID == crmid).FirstOrDefault();

                //if (data == null)
                //    data = d.CustomCrmGroups.SelectMany(s => s.GroupedCRMs).FirstOrDefault(f => f.CRMID == crmid);
            }

            return PartialView(@"~\views\salesex\salesexitem.cshtml", new AjaxCRM());
        }

        /// <summary>
        /// 放弃可打公司，只把member从companyrelationship中移除，不删除客户信息
        /// </summary>
        /// <param name="crmid"></param>
        /// <returns></returns>
        public PartialViewResult _CrmBlowed(string crmid, CompanyFilters filters = null)
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

            return PartialView(@"~\views\salesex\MainNavigationContainer.cshtml", GetNavigationBar(filters));
        }

        AjaxCrmTypedList GetNavigationBar(CompanyFilters filters = null)
        {
            var d = new AjaxCrmTypedList(filters);
            return d;
        }

        /// <summary>
        /// 把选中的公司分到指定的分组
        /// </summary>
        /// <param name="indexs"></param>
        /// <returns></returns>
        public PartialViewResult _CopyCrmToGroup(string crmid, string groupid, CompanyFilters filters = null)
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

        void RemoveFromGroup(string crmid)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="crmid"></param>
        /// <returns></returns>
        public PartialViewResult _CrmRemove(string crmid, CompanyFilters filters = null)
        {
            RemoveFromGroup(crmid);
            return PartialView(@"~\views\salesex\MainNavigationContainer.cshtml", GetNavigationBar(filters));
        }


        /// <summary>
        /// 刷新导航栏
        /// </summary>
        /// <returns></returns>
        public PartialViewResult _RefreshCrmList(CompanyFilters filters = null)
        {

            var bar = GetNavigationBar(filters);
            return PartialView(@"~\views\salesex\MainNavigationContainer.cshtml", bar);
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


        [ValidateInput(false)]
        public ActionResult CheckCompanyExist(int?projectid ,string beforeUpdateCN, string afterUpdateCN, string beforeUpdateEN, string afterUpdateEN)
        {
            var exist = from c in CH.DB.CompanyRelationships.Where(w => w.ProjectID == projectid) select c;
            var chcount = exist.Count((c => !string.IsNullOrEmpty(afterUpdateCN) & c.Company.Name_CH == afterUpdateCN && c.Company.Name_CH != beforeUpdateCN));
            if (chcount > 0)
            {
                return Content("同名中文公司名字已存在！");
            }

            var encount = exist.Count(c => !string.IsNullOrEmpty(afterUpdateEN) & c.Company.Name_EN == afterUpdateEN && c.Company.Name_EN != beforeUpdateEN);
            if (encount > 0)
            {
                return Content("同名英文公司名字已存在！");
            }


            //if (CH.GetAllData<CompanyRelationship>(c => c.Company.Name_CH == afterUpdateCN && c.Company.Name_CH != beforeUpdateCN).Count > 0)
            //{
            //    return Content("同名中文公司名字已存在！");
            //}
            //if (CH.GetAllData<CompanyRelationship>(c => c.Company.Name_EN == afterUpdateEN && c.Company.Name_EN != beforeUpdateEN).Count > 0)
            //{
            //    return Content("同名英文公司名字已存在！");
            //}

            return Content("");
        }

        public ActionResult GetEditLead(int leadId)
        {           
            var lead = CH.DB.Leads.FirstOrDefault(f => f.ID == leadId);  
            AjaxViewLead ajaxViewLead = new AjaxViewLead()
            {
                CompanyId = lead.CompanyID.Value,
                Address = lead.Address,
                Birthday = lead.Birthday,
                CellPhone = lead.Mobile,
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
                Blog = lead.Blog,
                DistrictNumberId = lead.DistrictNumberID,
                PersonalPhone = lead.PersonalPhone,
                PersonalCellPhone = lead.PersonalCellPhone,
                PersonalFax = lead.PersonalFax,
                Comment = lead.Comment,
                LeadRoles = lead.LeadRoles==null?string.Empty:lead.LeadRoles,
                QQ = lead.QQ,
                Twitter = lead.Twitter,
                Branch = lead.Branch
            };
            return PartialView("EditLead", ajaxViewLead);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult EditLead(AjaxViewLead ajaxViewLead, string[] leadRole)
        {
            var lead = CH.DB.Leads.FirstOrDefault(c => c.ID == ajaxViewLead.LeadId);           
            lead.Name_CH = ajaxViewLead.Name_CN;
            lead.Name_EN = ajaxViewLead.Name_EN;
            lead.CompanyID = ajaxViewLead.CompanyId;
            lead.Address = ajaxViewLead.Address;
            lead.Birthday = ajaxViewLead.Birthday;
            lead.Contact = ajaxViewLead.Telephone;
            lead.Department = ajaxViewLead.Department;
            lead.Description = ajaxViewLead.Desc;
            lead.EMail = ajaxViewLead.WorkingEmail;
            lead.PersonalEmailAddress = ajaxViewLead.PersonelEmail;
            lead.Title = ajaxViewLead.Title;
            lead.Fax = ajaxViewLead.Fax;
            lead.Gender = ajaxViewLead.Gender;
            lead.Mobile = ajaxViewLead.CellPhone;
            lead.WeiBo = ajaxViewLead.WeiBo;
            lead.WeiXin = ajaxViewLead.WeiXin;
            lead.LinkIn = ajaxViewLead.LinkIn;
            lead.FaceBook = ajaxViewLead.FaceBook;
            lead.Blog = ajaxViewLead.Blog;
            lead.DistrictNumberID = ajaxViewLead.DistrictNumberId;
            lead.ModifiedDate = DateTime.Now;
            lead.PersonalPhone = ajaxViewLead.PersonalPhone;
            lead.PersonalCellPhone = ajaxViewLead.PersonalCellPhone;
            lead.PersonalFax = ajaxViewLead.PersonalFax;
            lead.Comment = ajaxViewLead.Comment;           
            lead.QQ = ajaxViewLead.QQ;
            lead.Twitter = ajaxViewLead.Twitter;
            lead.Branch = ajaxViewLead.Branch;
            lead.ZIP = ajaxViewLead.Zip;
            lead.LeadRoles = string.Empty;
            if (leadRole != null)
            {
                foreach (string leadrole in leadRole)
                {
                    lead.LeadRoles += leadrole + ";";
                }
            }
            CH.Edit<Lead>(lead);
            return null;
        }

        public ActionResult GetAddLead(int companyId)
        {
            AjaxViewLead ajaxViewLead = new AjaxViewLead() { CompanyId = companyId };
            return PartialView("AddLead", ajaxViewLead);
        }

        [ValidateInput(false)]
        public ActionResult AddLead(AjaxViewLead ajaxViewLead, string[] leadRole)
        {
            Lead lead = new Lead()
            {
                Name_CH = ajaxViewLead.Name_CN,
                Name_EN = ajaxViewLead.Name_EN,
                Title = ajaxViewLead.Title,
                CompanyID = ajaxViewLead.CompanyId,
                Address = ajaxViewLead.Address,
                Birthday = ajaxViewLead.Birthday,
                Contact = ajaxViewLead.Telephone,
                Department = ajaxViewLead.Department,
                Description = ajaxViewLead.Desc,
                EMail = ajaxViewLead.WorkingEmail,
                Fax = ajaxViewLead.Fax,
                Gender = ajaxViewLead.Gender,
                Mobile = ajaxViewLead.CellPhone,
                WeiBo = ajaxViewLead.WeiBo,
                WeiXin = ajaxViewLead.WeiXin,
                LinkIn = ajaxViewLead.LinkIn,
                FaceBook = ajaxViewLead.FaceBook,
                Blog = ajaxViewLead.Blog,
                MarkForDelete = false,
                DistrictNumberID = ajaxViewLead.DistrictNumberId,
                PersonalEmailAddress = ajaxViewLead.PersonelEmail,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                PersonalPhone = ajaxViewLead.PersonalPhone,
                PersonalCellPhone = ajaxViewLead.PersonalCellPhone,
                PersonalFax = ajaxViewLead.PersonalFax,
                Comment = ajaxViewLead.Comment,              
                QQ = ajaxViewLead.QQ,
                Twitter = ajaxViewLead.Twitter,
                Branch = ajaxViewLead.Branch,
                ZIP = ajaxViewLead.Zip
            };
            if (leadRole != null)
            {
                foreach (string leadrole in leadRole)
                {
                    lead.LeadRoles += leadrole + ";";
                }
            }

            CH.Create<Lead>(lead);
            return Content(lead.ID.ToString());
        }

        public ActionResult GetEditLeadCall(int leadCallId)
        {
            var leadCall = CH.DB.LeadCalls.FirstOrDefault(c => c.ID == leadCallId);           
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

        [ValidateInput(false)]
        public ActionResult EditLeadCall(AjaxViewLeadCall ajaxViewLeadCall)
        {
            var leadCall = CH.DB.LeadCalls.FirstOrDefault(c => c.ID == ajaxViewLeadCall.CallId);             
            leadCall.CallBackDate = ajaxViewLeadCall.CallBackDate;
            leadCall.CallDate = ajaxViewLeadCall.CallDate;
            leadCall.LeadCallTypeID = ajaxViewLeadCall.CallTypeId;
            leadCall.Member = CH.DB.Members.FirstOrDefault(c => c.Name == Employee.CurrentUserName);
            leadCall.Result = ajaxViewLeadCall.Result;
            CH.Edit<LeadCall>(leadCall);
            return null;
        }

        public ActionResult GetAddCall(int leadId, int companyRelationId, int? projectId)
        {
            AjaxViewLeadCall ajaxViewLeadCall = new AjaxViewLeadCall() { LeadId = leadId, CompanyRelationshipId = companyRelationId, ProjectId = projectId, CallDate = DateTime.Now };
            return PartialView("AddCall", ajaxViewLeadCall);
        }

        [ValidateInput(false)]
        public ActionResult AddCall(AjaxViewLeadCall ajaxViewLeadCall)
        {

            LeadCall leadCall = new LeadCall();
            leadCall.CallBackDate = ajaxViewLeadCall.CallBackDate;
            leadCall.CallDate = ajaxViewLeadCall.CallDate;
            leadCall.CompanyRelationshipID = ajaxViewLeadCall.CompanyRelationshipId;
            leadCall.LeadCallTypeID = ajaxViewLeadCall.CallTypeId;
            leadCall.LeadID = ajaxViewLeadCall.LeadId;
            var c = CH.GetDataById<CompanyRelationship>(ajaxViewLeadCall.CompanyRelationshipId);
            var mem = c.Members.FirstOrDefault(m => m.Name == Employee.CurrentUserName);
            leadCall.MemberID = mem.ID; 
            //leadCall.Member = CH.DB.Members.FirstOrDefault(c => c.Name == Employee.CurrentUserName);
            leadCall.ProjectID = ajaxViewLeadCall.ProjectId;
            leadCall.Result = ajaxViewLeadCall.Result;
            leadCall.MarkForDelete = false;
            CH.Create<LeadCall>(leadCall);

            return null;
        }

        public ActionResult GetAddCompany(int? projectId)
        {
            var progress = CH.GetAllData<Progress>().Where(p => p.Code == 10).FirstOrDefault();

            AjaxViewSaleCompany ajaxViewSaleCompany = new AjaxViewSaleCompany() { ProjectId = projectId, Categories = new List<int>() { }, ProgressId = progress.ID };
            return PartialView("AddCompany", ajaxViewSaleCompany);
        }

        public ActionResult GetQuickEntry(int? projectId)
        {
            var progress = CH.GetAllData<Progress>().Where(p => p.Code == 10).FirstOrDefault();

            QuickEntry quickEntry = new QuickEntry() { ProjectId = projectId, Categories = new List<int>() { }, ProgressId = progress.ID, CallDate = DateTime.Now };
            return PartialView("QuickEntry", quickEntry);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult QuickEntry(QuickEntry quickEntry)
        {
            CompanyRelationship companyRelationship = new CompanyRelationship();
            companyRelationship.Company = new Company();
            companyRelationship.Company.AreaID = quickEntry.IndustryId;
            companyRelationship.Company.CompanyTypeID = quickEntry.TypeId;
            companyRelationship.Company.Contact = quickEntry.Phone;
            companyRelationship.Company.DistrictNumberID = quickEntry.DistrictNumberId;
            companyRelationship.Company.Name_CH = quickEntry.Name_CN;
            companyRelationship.Company.Name_EN = quickEntry.Name_EN;
            companyRelationship.Company.CreatedDate = DateTime.Now;
            companyRelationship.Company.Creator = Employee.CurrentUserName;
            companyRelationship.Company.ModifiedDate = DateTime.Now;
            companyRelationship.Company.ModifiedUser = Employee.CurrentUserName;
            companyRelationship.ProgressID = quickEntry.ProgressId;
            companyRelationship.Members = new List<Member>() { };
            companyRelationship.Members.Add(CH.DB.Members.FirstOrDefault(c => c.Name == Employee.CurrentUserName));
            companyRelationship.ProjectID = quickEntry.ProjectId;
            companyRelationship.MarkForDelete = false;
            companyRelationship.CreatedDate = DateTime.Now;
            companyRelationship.ModifiedDate = DateTime.Now;
            if (quickEntry.Categories != null)
            {
                companyRelationship.Categorys = CH.GetAllData<Category>(c => quickEntry.Categories.Contains(c.ID)).ToList();
            }
            string categorystring = string.Empty;
            companyRelationship.Categorys.ForEach(l =>
            {
                if (string.IsNullOrEmpty(categorystring))
                    categorystring = l.Name;
                else
                    categorystring += "," + l.Name;
            });
            companyRelationship.CategoryString = categorystring;
            CH.Create<CompanyRelationship>(companyRelationship);

            Lead lead = null;
            LeadCall leadCall = null;

            if (companyRelationship.ID > 0)
            {
                lead = new Lead()
                {
                    Name_CH = quickEntry.LeadName_CN,
                    Name_EN = quickEntry.LeadName_EN,
                    Title = quickEntry.Title,
                    CompanyID = companyRelationship.CompanyID,
                    Contact = quickEntry.Telephone,
                    Department = quickEntry.Department,
                    Gender = quickEntry.Gender,
                    Mobile = quickEntry.CellPhone,
                    MarkForDelete = false,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                };

                CH.Create<Lead>(lead);

                if (lead.ID > 0)
                {
                    
                    var c = CH.GetDataById<CompanyRelationship>(companyRelationship.ID);
                    var mem = c.Members.FirstOrDefault(m => m.Name == Employee.CurrentUserName);
                    leadCall = new LeadCall()
                    {
                        CallBackDate = quickEntry.CallBackDate,
                        CallDate = quickEntry.CallDate,
                        CompanyRelationshipID = companyRelationship.ID,
                        LeadCallTypeID = quickEntry.CallTypeId,
                        LeadID = lead.ID,
                        MemberID = mem.ID,
                        ProjectID = quickEntry.ProjectId,
                        Result = quickEntry.Result,
                        MarkForDelete = false
                    };
                    CH.Create<LeadCall>(leadCall);
                }
            }
            return Json(new { companyRelationshipId = companyRelationship.ID, companyId = companyRelationship.CompanyID, leadId = lead.ID, leadCallId = leadCall.ID, projectId = companyRelationship.ProjectID });
        }

        [ValidateInput(false)]
        public ActionResult AddCompany(AjaxViewSaleCompany ajaxViewSaleCompany)
        {
            CompanyRelationship companyRelationship = new CompanyRelationship();
            companyRelationship.Company = new Company();
            companyRelationship.Company.Address = ajaxViewSaleCompany.Address;
            companyRelationship.Company.AreaID = ajaxViewSaleCompany.IndustryId;
            companyRelationship.Company.Business = ajaxViewSaleCompany.Business;
            companyRelationship.Company.CompanyTypeID = ajaxViewSaleCompany.TypeId;
            companyRelationship.Company.Contact = ajaxViewSaleCompany.Phone;
            companyRelationship.Company.Description = ajaxViewSaleCompany.Desc;
            companyRelationship.Company.DistrictNumberID = ajaxViewSaleCompany.DistrictNumberId;
            companyRelationship.Company.Fax = ajaxViewSaleCompany.Fax;
            companyRelationship.Company.Name_CH = ajaxViewSaleCompany.Name_CN;
            companyRelationship.Company.Name_EN = ajaxViewSaleCompany.Name_EN;
            companyRelationship.Company.WebSite = ajaxViewSaleCompany.WebSite;
            companyRelationship.Company.ZIP = ajaxViewSaleCompany.ZipCode;
            companyRelationship.Company.CreatedDate = DateTime.Now;
            companyRelationship.Company.Creator = Employee.CurrentUserName;
            companyRelationship.Company.ModifiedDate = DateTime.Now;
            companyRelationship.Company.ModifiedUser = Employee.CurrentUserName;
            companyRelationship.Company.Address_EN = ajaxViewSaleCompany.Address_EN;
            companyRelationship.Company.Province = ajaxViewSaleCompany.Province;
            companyRelationship.Company.City = ajaxViewSaleCompany.City;
            companyRelationship.Company.Scale = ajaxViewSaleCompany.Scale;
            companyRelationship.Company.AnnualSales = ajaxViewSaleCompany.AnnualSales;
            companyRelationship.Company.MainProduct = ajaxViewSaleCompany.MainProduct;
            companyRelationship.Company.MainClient = ajaxViewSaleCompany.MainClient;
            companyRelationship.Description = ajaxViewSaleCompany.Desc;
            companyRelationship.ProgressID = ajaxViewSaleCompany.ProgressId;
            companyRelationship.Members = new List<Member>() { };
            companyRelationship.Members.Add(CH.DB.Members.FirstOrDefault(c => c.Name == Employee.CurrentUserName));
            companyRelationship.ProjectID = ajaxViewSaleCompany.ProjectId;
            companyRelationship.MarkForDelete = false;
            companyRelationship.CreatedDate = DateTime.Now;
            companyRelationship.ModifiedDate = DateTime.Now;
            if (ajaxViewSaleCompany.Categories != null)
            {
                companyRelationship.Categorys = CH.GetAllData<Category>(c => ajaxViewSaleCompany.Categories.Contains(c.ID)).ToList();
            }
            string categorystring = string.Empty;
            companyRelationship.Categorys.ForEach(l =>
            {
                if (string.IsNullOrEmpty(categorystring))
                    categorystring = l.Name;
                else
                    categorystring += "," + l.Name;
            });
            companyRelationship.CategoryString = categorystring;
            CH.Create<CompanyRelationship>(companyRelationship);

            return Json(new { companyRelationshipId = companyRelationship.ID, companyId = companyRelationship.CompanyID, projectId = companyRelationship.ProjectID });
        }

        public ActionResult GetEditCompany(int companyId)
        {           
            var companyRelationship = CH.DB.CompanyRelationships.FirstOrDefault(c => c.CompanyID == companyId);  
            AjaxViewSaleCompany ajaxViewSaleCompany = new AjaxViewSaleCompany()
            {
                CompanRelationshipId = companyRelationship.ID,
                ProjectId = companyRelationship.ProjectID,
                CompanyId = companyRelationship.CompanyID,
                Address = companyRelationship.Company.Address,
                Business = companyRelationship.Company.Business,
                Desc = companyRelationship.Company.Description,
                DistrictNumberId = companyRelationship.Company.DistrictNumberID,
                Fax = companyRelationship.Company.Fax,
                Name_CN = companyRelationship.Company.Name_CH,
                Name_EN = companyRelationship.Company.Name_EN,
                Phone = companyRelationship.Company.Contact,
                ZipCode = companyRelationship.Company.ZIP,
                WebSite = companyRelationship.Company.WebSite,
                Categories = companyRelationship.Categorys.Select(c => c.ID).ToList(),
                Address_EN = companyRelationship.Company.Address_EN,
                Province = companyRelationship.Company.Province,
                City = companyRelationship.Company.City,
                Scale = companyRelationship.Company.Scale,
                AnnualSales = companyRelationship.Company.AnnualSales,
                MainProduct = companyRelationship.Company.MainProduct,
                MainClient = companyRelationship.Company.MainClient,
                CreatedDate = companyRelationship.Company.CreatedDate.ToString(),
                Creator = companyRelationship.Company.Creator,
                ModifiedDate = companyRelationship.Company.ModifiedDate.ToString(),
                ModifiedUser = companyRelationship.Company.ModifiedUser
            };

            if (companyRelationship.Company.Area != null)
            {
                ajaxViewSaleCompany.IndustryId = companyRelationship.Company.AreaID.Value;
                ajaxViewSaleCompany.IndustryString = companyRelationship.Company.Area.Name_CH;
            }
            if (companyRelationship.Company.CompanyType != null)
            {
                ajaxViewSaleCompany.TypeId = companyRelationship.Company.CompanyTypeID.Value;
                ajaxViewSaleCompany.TypeString = companyRelationship.Company.CompanyType.Name;
            }
            if (companyRelationship.Progress != null)
            {
                ajaxViewSaleCompany.ProgressId = companyRelationship.ProgressID.Value;
                ajaxViewSaleCompany.ProgressString = companyRelationship.Progress.Description;
            }

            return PartialView("EditCompany", ajaxViewSaleCompany);
        }

        public ActionResult GetDetailCompany(int companyId)
        {            
            var companyRelationship = CH.DB.CompanyRelationships.FirstOrDefault(c => c.CompanyID == companyId);  
            AjaxViewSaleCompany ajaxViewSaleCompany = new AjaxViewSaleCompany()
            {
                ProjectId = companyRelationship.ProjectID,
                CompanyId = companyRelationship.CompanyID,
                Address = companyRelationship.Company.Address,
                Business = companyRelationship.Company.Business,
                Desc = companyRelationship.Company.Description,
                DistrictNumberId = companyRelationship.Company.DistrictNumberID,
                Fax = companyRelationship.Company.Fax,
                Name_CN = companyRelationship.Company.Name_CH,
                Name_EN = companyRelationship.Company.Name_EN,
                Phone = companyRelationship.Company.Contact,
                ZipCode = companyRelationship.Company.ZIP,
                WebSite = companyRelationship.Company.WebSite,
                Categories = companyRelationship.Categorys.Select(c => c.ID).ToList(),
                Province = companyRelationship.Company.Province,
                City = companyRelationship.Company.City,
                Scale = companyRelationship.Company.Scale,
                AnnualSales = companyRelationship.Company.AnnualSales,
                MainProduct = companyRelationship.Company.MainProduct,
                MainClient = companyRelationship.Company.MainClient,
                CreatedDate = companyRelationship.Company.CreatedDate.ToString(),
                Creator = companyRelationship.Company.Creator,
                ModifiedDate = companyRelationship.Company.ModifiedDate.ToString(),
                ModifiedUser = companyRelationship.Company.ModifiedUser
            };

            if (companyRelationship.Company.Area != null)
            {
                ajaxViewSaleCompany.IndustryId = companyRelationship.Company.AreaID.Value;
                ajaxViewSaleCompany.IndustryString = companyRelationship.Company.Area.Name_CH;
            }
            if (companyRelationship.Company.CompanyType != null)
            {
                ajaxViewSaleCompany.TypeId = companyRelationship.Company.CompanyTypeID.Value;
                ajaxViewSaleCompany.TypeString = companyRelationship.Company.CompanyType.Name;
            }
            if (companyRelationship.Progress != null)
            {
                ajaxViewSaleCompany.ProgressId = companyRelationship.ProgressID.Value;
                ajaxViewSaleCompany.ProgressString = companyRelationship.Progress.Description;
            }

            return PartialView("DetailCompany", ajaxViewSaleCompany);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditCompany(AjaxViewSaleCompany ajaxViewSaleCompany)
        {         
            var companyRelationship = CH.DB.CompanyRelationships.FirstOrDefault(c => c.CompanyID == ajaxViewSaleCompany.CompanyId);  
            companyRelationship.Company.Address = ajaxViewSaleCompany.Address;
            companyRelationship.Company.AreaID = ajaxViewSaleCompany.IndustryId;
            companyRelationship.Company.Business = ajaxViewSaleCompany.Business;
            companyRelationship.Company.CompanyTypeID = ajaxViewSaleCompany.TypeId;
            companyRelationship.Company.Contact = ajaxViewSaleCompany.Phone;          
            companyRelationship.Company.ModifiedDate = DateTime.Now;
            companyRelationship.Company.ModifiedUser = Employee.CurrentUserName;
            companyRelationship.Company.Address_EN = ajaxViewSaleCompany.Address_EN;
            companyRelationship.Company.Province = ajaxViewSaleCompany.Province;
            companyRelationship.Company.City = ajaxViewSaleCompany.City;
            companyRelationship.Company.Scale = ajaxViewSaleCompany.Scale;
            companyRelationship.Company.AnnualSales = ajaxViewSaleCompany.AnnualSales;
            companyRelationship.Company.MainProduct = ajaxViewSaleCompany.MainProduct;
            companyRelationship.Company.MainClient = ajaxViewSaleCompany.MainClient;
            companyRelationship.Company.Description = ajaxViewSaleCompany.Desc;
            companyRelationship.Company.DistrictNumberID = ajaxViewSaleCompany.DistrictNumberId;
            companyRelationship.Company.Fax = ajaxViewSaleCompany.Fax;
            companyRelationship.Company.Name_CH = ajaxViewSaleCompany.Name_CN;
            companyRelationship.Company.Name_EN = ajaxViewSaleCompany.Name_EN;
            companyRelationship.Company.WebSite = ajaxViewSaleCompany.WebSite;
            companyRelationship.Company.ZIP = ajaxViewSaleCompany.ZipCode;
            companyRelationship.Description = ajaxViewSaleCompany.Desc;
            companyRelationship.ProgressID = ajaxViewSaleCompany.ProgressId;
            companyRelationship.ModifiedDate = DateTime.Now;
            companyRelationship.Categorys.Clear();
            if (ajaxViewSaleCompany.Categories != null)
            {
                companyRelationship.Categorys = CH.GetAllData<Category>(c => ajaxViewSaleCompany.Categories.Contains(c.ID)).ToList();
            }
            string categorystring = string.Empty;
            companyRelationship.Categorys.ForEach(l =>
            {
                if (string.IsNullOrEmpty(categorystring))
                    categorystring = l.Name;
                else
                    categorystring += "," + l.Name;
            });
            companyRelationship.CategoryString = categorystring;

            CH.Edit<CompanyRelationship>(companyRelationship);
            return null;
        }

        [HttpGet]
        public ActionResult GetCatagories(int currentProjectId)
        {
            List<Category> categoriylist = CH.GetAllData<Category>(o => o.ProjectID == currentProjectId).ToList();
            return Json(categoriylist.Select(c => new { value = c.ID, text = c.Name }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetQuickAddDeal(int? projectId)
        {
            projectId = this.TrySetProjectIDForUser(projectId);
            ViewBag.ProjectID = projectId;
            List<AjaxParticipant> pList = new List<AjaxParticipant>();
            Session["pList"] = pList;

            ViewBag.pList = pList;

            this.AddErrorStateIfSalesNoAccessRightToTheProject(projectId);
            return PartialView("QuickAddDeal");
        }

        [HttpPost]
        //public ActionResult QuickAddDeal(Deal item, List<Participant> pList, int? projectid)
        public ActionResult QuickAddDeal(Deal item, int? projectid)
        {
            ViewBag.ProjectID = projectid;
            ViewBag.CompanyRelationshipID = item.CompanyRelationshipID;
            Participant p = null;
            if (ModelState.IsValid)
            {
                item.Sales = Employee.CurrentUserName;
                string prefix = CH.GetDataById<Project>(projectid).ProjectCode + DateTime.Now.Year.ToString();
                var records = CH.GetAllData<Deal>().Where(s => s.DealCode != null && s.DealCode.StartsWith(prefix));
                if (records != null && records.Count() > 0)
                {
                    item.DealCode = prefix + string.Format("{0:D3}", Convert.ToInt32(records.OrderByDescending(s => s.DealCode).First().DealCode.Substring(prefix.Length)) + 1);
                }
                else
                {
                    item.DealCode = prefix + "001";
                }

                CH.Create<Deal>(item);
                if (item.ID > 0)
                {
                    List<AjaxParticipant> pList = new List<AjaxParticipant>();
                    if (Session["pList"] != null)
                    {
                        pList = Session["pList"] as List<AjaxParticipant>;
                    }
                    if (pList != null && pList.Count > 0)
                    {
                        foreach (var ajaxp in pList)
                        {
                            p = new Participant();
                            p.Name = ajaxp.Name;
                            p.Title = ajaxp.Title;
                            p.Gender = ajaxp.Gender;
                            p.Mobile = ajaxp.Mobile;
                            p.Contact = ajaxp.Contact;
                            p.Email = ajaxp.Email;
                            p.ParticipantTypeID = ajaxp.ParticipantTypeID;
                            p.ProjectID = item.ProjectID;
                            p.DealID = item.ID;
                            p.Creator = Employee.CurrentUser.UserName;
                            p.CreatedDate = DateTime.Now;
                            p.ModifiedUser = Employee.CurrentUser.UserName;
                            p.ModifiedDate = DateTime.Now;
                            if (p.ID == 0)
                                CH.Create<Participant>(p);
                        }
                    }
                }
            }
            return Json(new { dealId = item.ID, participantId = p.ID });
        }

        [GridAction]
        public ActionResult _SelectAjaxParticipant()
        {
            List<AjaxParticipant> pList = new List<AjaxParticipant>();
            if (Session["pList"] != null)
            {
                 pList = Session["pList"] as List<AjaxParticipant>;
            }
            return View(new GridModel(pList));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _UpdateAjaxParticipant([Bind(Prefix = "inserted")]IEnumerable<AjaxParticipant> insertedP,
            [Bind(Prefix = "updated")]IEnumerable<AjaxParticipant> updatedP,
            [Bind(Prefix = "deleted")]IEnumerable<AjaxParticipant> deletedP)
        {
            List<AjaxParticipant> pList = new List<AjaxParticipant>();
            if (Session["pList"] != null)
            {
                pList = Session["pList"] as List<AjaxParticipant>;
            }

            if (insertedP != null)
            {
                foreach (var p in insertedP)
                {
                    if (p.Name != "" && p.ParticipantTypeID != null)
                    {
                        pList.Add(p);
                    }
                }
            }
            
            if (deletedP != null)
            {
                foreach (var p in deletedP)
                {
                    //int index = pList.IndexOf(p);
                    int index = pList.FindIndex(ap => ap.Name == p.Name && ap.ParticipantTypeID == p.ParticipantTypeID);
                    if (index != -1)
                    {
                        pList.RemoveAt(index);
                    }
                }
            }
            return View(new GridModel(pList));
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