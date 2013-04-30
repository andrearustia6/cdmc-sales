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
            return View(GetNavigationBar());
        }



        /// <summary>
        /// 导航选中的公司或者lead
        /// </summary>
        /// <param name="indexs"></param>
        /// <returns></returns>
        public PartialViewResult _SelectedList(string indexs, CompanyFilters filters = null)
        {
            var ids = indexs.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var crmid = int.Parse(ids[0]);
            var leadid = 0;
            if (ids.Count() > 1)
            {
                leadid = int.Parse(ids[1]);
            }
            var d = GetNavigationBar(filters);
            var data = d.AllCRMs.FirstOrDefault(f => f.CRMID == crmid);
            if (data == null)
                data = d.CustomCrmGroups.SelectMany(s => s.GroupedCRMs).FirstOrDefault(f => f.CRMID == crmid);

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
                Blog = lead.Blog,
                DistrictNumberId = lead.DistrictNumberID
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
            CH.Edit<Lead>(lead);
            return null;
        }

        public ActionResult GetAddLead(int companyId)
        {
            AjaxViewLead ajaxViewLead = new AjaxViewLead() { CompanyId = companyId };
            return PartialView("AddLead", ajaxViewLead);
        }

        public ActionResult AddLead(AjaxViewLead ajaxViewLead)
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
                PersonalEmailAddress = ajaxViewLead.PersonelEmail
            };

            CH.Create<Lead>(lead);
            return Content(lead.ID.ToString());
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

        public ActionResult GetAddCall(int leadId, int companyRelationId, int? projectId)
        {
            AjaxViewLeadCall ajaxViewLeadCall = new AjaxViewLeadCall() { LeadId = leadId, CompanyRelationshipId = companyRelationId, ProjectId = projectId, CallBackDate = DateTime.Now, CallDate = DateTime.Now };
            return PartialView("AddCall", ajaxViewLeadCall);
        }

        public ActionResult AddCall(AjaxViewLeadCall ajaxViewLeadCall)
        {

            LeadCall leadCall = new LeadCall();
            leadCall.CallBackDate = ajaxViewLeadCall.CallBackDate;
            leadCall.CallDate = ajaxViewLeadCall.CallDate;
            leadCall.CompanyRelationshipID = ajaxViewLeadCall.CompanyRelationshipId;
            leadCall.LeadCallTypeID = ajaxViewLeadCall.CallTypeId;
            leadCall.LeadID = ajaxViewLeadCall.LeadId;
            leadCall.Member = CH.GetAllData<Member>(c => c.Name == Employee.CurrentUserName).First();
            leadCall.ProjectID = ajaxViewLeadCall.ProjectId;
            leadCall.Result = ajaxViewLeadCall.Result;
            leadCall.MarkForDelete = false;
            CH.Create<LeadCall>(leadCall);

            return null;
        }

        public ActionResult GetAddCompany(int? projectId)
        {
            AjaxViewSaleCompany ajaxViewSaleCompany = new AjaxViewSaleCompany() { ProjectId = projectId, Categories = new List<int>() { }, ProgressId = 11 };
            return PartialView("AddCompany", ajaxViewSaleCompany);
        }

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
            companyRelationship.Description = ajaxViewSaleCompany.Desc;
            companyRelationship.ProgressID = ajaxViewSaleCompany.ProgressId;
            companyRelationship.Members = new List<Member>() { };
            companyRelationship.Members.Add(CH.GetAllData<Member>(c => c.Name == Employee.CurrentUserName).First());
            companyRelationship.ProjectID = ajaxViewSaleCompany.ProjectId;
            companyRelationship.MarkForDelete = false;
            if (ajaxViewSaleCompany.Categories != null)
            {
                companyRelationship.Categorys = CH.GetAllData<Category>(c => ajaxViewSaleCompany.Categories.Contains(c.ID)).ToList();
            }
            CH.Create<CompanyRelationship>(companyRelationship);

            return Json(new { companyRelationshipId = companyRelationship.ID, companyId = companyRelationship.CompanyID, projectId = companyRelationship.ProjectID });
        }

        public ActionResult GetEditCompany(int companyId)
        {
            CompanyRelationship companyRelationship = CH.GetAllData<CompanyRelationship>(c => c.CompanyID == companyId).First();
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
                Categories = companyRelationship.Categorys.Select(c => c.ID).ToList()
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

        [HttpPost]
        public ActionResult EditCompany(AjaxViewSaleCompany ajaxViewSaleCompany)
        {
            CompanyRelationship companyRelationship = CH.GetAllData<CompanyRelationship>(c => c.CompanyID == ajaxViewSaleCompany.CompanyId).First();
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
            companyRelationship.Description = ajaxViewSaleCompany.Desc;
            companyRelationship.ProgressID = ajaxViewSaleCompany.ProgressId;
            companyRelationship.Categorys.Clear();
            if (ajaxViewSaleCompany.Categories != null)
            {
                companyRelationship.Categorys = CH.GetAllData<Category>(c => ajaxViewSaleCompany.Categories.Contains(c.ID)).ToList();
            }
            CH.Edit<CompanyRelationship>(companyRelationship);
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