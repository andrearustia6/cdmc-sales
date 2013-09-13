using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sales.BLL;
using Sales.Model;
using Model;
using Entity;
using Utl;
namespace Sales.Controllers
{
    public class AvaliableCompaniesController : Controller
    {
        public ActionResult Index(int? projectid)
        {
            var data = AvaliableCRM.GetAvaliableCompanies(projectid);
            return View(data);
        }
        /// <summary>
        /// 导航选中的公司或者lead
        /// </summary>
        /// <param name="indexs"></param>
        /// <returns></returns>
        public PartialViewResult _SelectedCRMNode(string crmid)
        {
            ViewBag.leadid = 0;
            var data = AvaliableCRM._CRMGetAvaliableCrmDetail(int.Parse(crmid));
            return PartialView(@"~\views\AvaliableCompanies\DetailContainer.cshtml", data);
        }
        public PartialViewResult GetCRMByCrmIDLeadID(int crmid,int leadid)
        {
            ViewBag.leadid = leadid;
            var data = AvaliableCRM._CRMGetAvaliableCrmDetailByCrmIDLeadID(crmid,leadid);
            return PartialView(@"~\views\AvaliableCompanies\DetailContainer.cshtml", data);
        }
        public PartialViewResult GetDescription(string crmid)
        {
            var data = AvaliableCRM._CRMGetAvaliableCrmDetail(int.Parse(crmid));
            return PartialView("Description", data);
        }
        public PartialViewResult GetCompetitor(string crmid)
        {
            var data = AvaliableCRM._CRMGetAvaliableCrmDetail(int.Parse(crmid));
            return PartialView("Competitor", data);
        }
        public PartialViewResult GetPitchPoint(string crmid)
        {
            var data = AvaliableCRM._CRMGetAvaliableCrmDetail(int.Parse(crmid));
            return PartialView("PitchPoint", data);
        }
        public PartialViewResult GetCategories(string crmid)
        {
            var data = AvaliableCRM._CRMGetAvaliableCrmDetail(int.Parse(crmid));
            return PartialView("Category", data);
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
                Name_CH = string.IsNullOrEmpty(ajaxViewLead.Name_CN) ? "" : ajaxViewLead.Name_CN.Trim(),
                Name_EN = string.IsNullOrEmpty(ajaxViewLead.Name_EN) ? "" : ajaxViewLead.Name_EN.Trim(),
                Title = string.IsNullOrEmpty(ajaxViewLead.Title) ? "" : ajaxViewLead.Title.Trim(),
                CompanyID = ajaxViewLead.CompanyId,
                Address = string.IsNullOrEmpty(ajaxViewLead.Address) ? "" : ajaxViewLead.Address.Trim(),
                Birthday = ajaxViewLead.Birthday,
                Contact = string.IsNullOrEmpty(ajaxViewLead.Telephone) ? "" : ajaxViewLead.Telephone.Trim(),
                Department = string.IsNullOrEmpty(ajaxViewLead.Department) ? "" : ajaxViewLead.Department.Trim(),
                Description = string.IsNullOrEmpty(ajaxViewLead.Desc) ? "" : ajaxViewLead.Desc.Trim(),
                EMail = string.IsNullOrEmpty(ajaxViewLead.WorkingEmail) ? "" : ajaxViewLead.WorkingEmail.Trim(),
                Fax = string.IsNullOrEmpty(ajaxViewLead.Fax) ? "" : ajaxViewLead.Fax.Trim(),
                Gender = string.IsNullOrEmpty(ajaxViewLead.Gender) ? "" : ajaxViewLead.Gender.Trim(),
                Mobile = string.IsNullOrEmpty(ajaxViewLead.CellPhone) ? "" : ajaxViewLead.CellPhone.Trim(),
                WeiBo = string.IsNullOrEmpty(ajaxViewLead.WeiBo) ? "" : ajaxViewLead.WeiBo.Trim(),
                WeiXin = string.IsNullOrEmpty(ajaxViewLead.WeiXin) ? "" : ajaxViewLead.WeiXin.Trim(),
                LinkIn = string.IsNullOrEmpty(ajaxViewLead.LinkIn) ? "" : ajaxViewLead.LinkIn.Trim(),
                FaceBook = string.IsNullOrEmpty(ajaxViewLead.FaceBook) ? "" : ajaxViewLead.FaceBook.Trim(),
                Blog = string.IsNullOrEmpty(ajaxViewLead.Blog) ? "" : ajaxViewLead.Blog.Trim(),
                MarkForDelete = false,
                DistrictNumberID = ajaxViewLead.DistrictNumberId,
                PersonalEmailAddress = string.IsNullOrEmpty(ajaxViewLead.PersonelEmail) ? "" : ajaxViewLead.PersonelEmail.Trim(),
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                PersonalPhone = string.IsNullOrEmpty(ajaxViewLead.PersonalPhone) ? "" : ajaxViewLead.PersonalPhone.Trim(),
                PersonalCellPhone = string.IsNullOrEmpty(ajaxViewLead.PersonalCellPhone) ? "" : ajaxViewLead.PersonalCellPhone.Trim(),
                PersonalFax = string.IsNullOrEmpty(ajaxViewLead.PersonalFax) ? "" : ajaxViewLead.PersonalFax.Trim(),
                Comment = string.IsNullOrEmpty(ajaxViewLead.Comment) ? "" : ajaxViewLead.Comment.Trim(),
                QQ = string.IsNullOrEmpty(ajaxViewLead.QQ) ? "" : ajaxViewLead.QQ.Trim(),
                Twitter = string.IsNullOrEmpty(ajaxViewLead.Twitter) ? "" : ajaxViewLead.Twitter.Trim(),
                Branch = string.IsNullOrEmpty(ajaxViewLead.Branch) ? "" : ajaxViewLead.Branch.Trim(),
                ZIP = string.IsNullOrEmpty(ajaxViewLead.Zip) ? "" : ajaxViewLead.Zip.Trim()
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
                LeadRoles = lead.LeadRoles == null ? string.Empty : lead.LeadRoles,
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
                ModifiedUser = companyRelationship.Company.ModifiedUser,
                Customers = companyRelationship.Company.Customers,
                Competitor = companyRelationship.Company.Competitor,
                PitchedPoint = companyRelationship.PitchedPoint
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
            if (companyRelationship.CoreLVL != null)
            {
                ajaxViewSaleCompany.CoreLVLID = companyRelationship.CoreLVLID.Value;
            }
            return PartialView("EditCompany", ajaxViewSaleCompany);
        }

        [ValidateInput(false)]
        public ActionResult CheckCompanyExist(int? projectid, string beforeUpdateCN, string afterUpdateCN, string beforeUpdateEN, string afterUpdateEN)
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

            return Content("");
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
            companyRelationship.Company.Customers = ajaxViewSaleCompany.Customers;
            companyRelationship.Company.Competitor = ajaxViewSaleCompany.Competitor;
            companyRelationship.Description = ajaxViewSaleCompany.Desc;
            companyRelationship.ProgressID = ajaxViewSaleCompany.ProgressId;
            companyRelationship.CoreLVLID = ajaxViewSaleCompany.CoreLVLID;
            companyRelationship.ModifiedDate = DateTime.Now;
            companyRelationship.PitchedPoint = ajaxViewSaleCompany.PitchedPoint;
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
            return Json(new { companyRelationshipId = companyRelationship.ID, companyId = companyRelationship.CompanyID, projectId = companyRelationship.ProjectID, corelvlid = companyRelationship.CoreLVLID, processid = companyRelationship.ProgressID });
        }

        public ActionResult GetAddCompany(int? projectId)
        {
            var progress = CH.GetAllData<Progress>().Where(p => p.Code == 10).FirstOrDefault();

            AjaxViewSaleCompany ajaxViewSaleCompany = new AjaxViewSaleCompany() { ProjectId = projectId, Categories = new List<int>() { }, ProgressId = progress.ID };
            return PartialView("AddCompany", ajaxViewSaleCompany);
        }
        [ValidateInput(false)]
        public ActionResult AddCompany(AjaxViewSaleCompany ajaxViewSaleCompany)
        {
            CompanyRelationship companyRelationship = new CompanyRelationship();
            companyRelationship.Company = new Company();
            companyRelationship.Company.Address = string.IsNullOrEmpty(ajaxViewSaleCompany.Address) ? "" : ajaxViewSaleCompany.Address.Trim();
            companyRelationship.Company.AreaID = ajaxViewSaleCompany.IndustryId;
            companyRelationship.Company.Business = string.IsNullOrEmpty(ajaxViewSaleCompany.Business) ? "" : ajaxViewSaleCompany.Business.Trim();
            companyRelationship.Company.CompanyTypeID = ajaxViewSaleCompany.TypeId;
            companyRelationship.Company.Contact = string.IsNullOrEmpty(ajaxViewSaleCompany.Phone) ? "" : ajaxViewSaleCompany.Phone.Trim();
            companyRelationship.Company.Description = string.IsNullOrEmpty(ajaxViewSaleCompany.Desc) ? "" : ajaxViewSaleCompany.Desc.Trim();
            companyRelationship.Company.DistrictNumberID = ajaxViewSaleCompany.DistrictNumberId;
            companyRelationship.Company.Fax = string.IsNullOrEmpty(ajaxViewSaleCompany.Fax) ? "" : ajaxViewSaleCompany.Fax.Trim();
            companyRelationship.Company.Name_CH = string.IsNullOrEmpty(ajaxViewSaleCompany.Name_CN) ? "" : ajaxViewSaleCompany.Name_CN.Trim();
            companyRelationship.Company.Name_EN = string.IsNullOrEmpty(ajaxViewSaleCompany.Name_EN) ? "" : ajaxViewSaleCompany.Name_EN.Trim();
            companyRelationship.Company.WebSite = string.IsNullOrEmpty(ajaxViewSaleCompany.WebSite) ? "" : ajaxViewSaleCompany.WebSite.Trim();
            companyRelationship.Company.ZIP = string.IsNullOrEmpty(ajaxViewSaleCompany.ZipCode) ? "" : ajaxViewSaleCompany.ZipCode.Trim();
            companyRelationship.Company.CreatedDate = DateTime.Now;
            companyRelationship.Company.Creator = Employee.CurrentUserName;
            companyRelationship.Company.ModifiedDate = DateTime.Now;
            companyRelationship.Company.ModifiedUser = Employee.CurrentUserName;
            companyRelationship.Company.Address_EN = string.IsNullOrEmpty(ajaxViewSaleCompany.Address_EN) ? "" : ajaxViewSaleCompany.Address_EN.Trim();
            companyRelationship.Company.Province = string.IsNullOrEmpty(ajaxViewSaleCompany.Province) ? "" : ajaxViewSaleCompany.Province.Trim();
            companyRelationship.Company.City = string.IsNullOrEmpty(ajaxViewSaleCompany.City) ? "" : ajaxViewSaleCompany.City.Trim();
            companyRelationship.Company.Scale = string.IsNullOrEmpty(ajaxViewSaleCompany.Scale) ? "" : ajaxViewSaleCompany.Scale.Trim();
            companyRelationship.Company.AnnualSales = string.IsNullOrEmpty(ajaxViewSaleCompany.AnnualSales) ? "" : ajaxViewSaleCompany.AnnualSales.Trim();
            companyRelationship.Company.MainProduct = string.IsNullOrEmpty(ajaxViewSaleCompany.MainProduct) ? "" : ajaxViewSaleCompany.MainProduct.Trim();
            companyRelationship.Company.MainClient = string.IsNullOrEmpty(ajaxViewSaleCompany.MainClient) ? "" : ajaxViewSaleCompany.MainClient.Trim();
            companyRelationship.Company.Customers = string.IsNullOrEmpty(ajaxViewSaleCompany.Customers) ? "" : ajaxViewSaleCompany.Customers.Trim();
            companyRelationship.Company.Competitor = string.IsNullOrEmpty(ajaxViewSaleCompany.Competitor) ? "" : ajaxViewSaleCompany.Competitor.Trim();

            companyRelationship.Description = string.IsNullOrEmpty(ajaxViewSaleCompany.Desc) ? "" : ajaxViewSaleCompany.Desc.Trim();
            companyRelationship.ProgressID = ajaxViewSaleCompany.ProgressId;
            companyRelationship.CoreLVLID = ajaxViewSaleCompany.CoreLVLID;
            companyRelationship.Members = new List<Member>() { };
            companyRelationship.Members.Add(CH.DB.Members.FirstOrDefault(c => c.Name == Employee.CurrentUserName && ajaxViewSaleCompany.ProjectId == c.ProjectID));
            companyRelationship.ProjectID = ajaxViewSaleCompany.ProjectId;
            companyRelationship.PitchedPoint = string.IsNullOrEmpty(ajaxViewSaleCompany.PitchedPoint) ? "" : ajaxViewSaleCompany.PitchedPoint.Trim();
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

            return Json(new { companyRelationshipId = companyRelationship.ID, companyId = companyRelationship.CompanyID, projectId = companyRelationship.ProjectID, corelvlid = companyRelationship.CoreLVLID, processid = companyRelationship.ProgressID });
        }
        public ActionResult GetAddCall(int leadId, int companyRelationId, int? projectId)
        {
            AjaxViewLeadCall ajaxViewLeadCall = new AjaxViewLeadCall() { LeadId = leadId, CompanyRelationshipId = companyRelationId, ProjectId = projectId, CallDate = DateTime.Now,ProgressId=(int)CH.GetDataById<CompanyRelationship>(companyRelationId).ProgressID };
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
            var name = Employee.CurrentUserName;
            var memid = CH.DB.Members.Where(w => w.Name == name && w.ProjectID == c.ProjectID).Select(s => s.ID).FirstOrDefault();
            leadCall.MemberID = memid;
            //leadCall.Member = CH.DB.Members.FirstOrDefault(c => c.Name == Employee.CurrentUserName);
            leadCall.ProjectID = ajaxViewLeadCall.ProjectId;
            leadCall.Result = ajaxViewLeadCall.Result;
            leadCall.MarkForDelete = false;
            CH.Create<LeadCall>(leadCall);

            CompanyRelationship ship = CH.GetDataById<CompanyRelationship>(ajaxViewLeadCall.CompanyRelationshipId);
            ship.ProgressID = ajaxViewLeadCall.ProgressId;
            CH.Edit<CompanyRelationship>(ship);
            return Json(new { companyRelationshipId = ship.ID, companyId = ship.CompanyID, projectId = ship.ProjectID, corelvlid = ship.CoreLVLID, processid = ship.ProgressID, leadid = leadCall.LeadID });
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
            //leadCall.Member = CH.DB.Members.FirstOrDefault(c => c.Name == Employee.CurrentUserName);
            leadCall.Result = ajaxViewLeadCall.Result;
            CH.Edit<LeadCall>(leadCall);
            return null;
        }

        public ActionResult GetAddComment(int? crmid)
        {
            Comment comment = new Comment() { CompanyRelationshipID = crmid };
            return PartialView("Comment", comment);
        }
        [ValidateInput(false)]
        public ActionResult AddComment(Comment comment)
        {
            comment.CommentDate = DateTime.Now;
            comment.Submitter = Employee.CurrentUserName;
            CH.Create<Comment>(comment);
            return null;
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
            companyRelationship.Company.Contact = string.IsNullOrEmpty(quickEntry.Phone) ? "" : quickEntry.Phone.Trim();
            companyRelationship.Company.Description = string.IsNullOrEmpty(quickEntry.Desc) ? "" : quickEntry.Desc.Trim();
            companyRelationship.Company.DistrictNumberID = quickEntry.DistrictNumberId;
            companyRelationship.Company.Name_CH = string.IsNullOrEmpty(quickEntry.Name_CN) ? "" : quickEntry.Name_CN.Trim();
            companyRelationship.Company.Name_EN = string.IsNullOrEmpty(quickEntry.Name_EN) ? "" : quickEntry.Name_EN.Trim();
            companyRelationship.Company.CreatedDate = DateTime.Now;
            companyRelationship.Company.Creator = Employee.CurrentUserName;
            companyRelationship.Company.ModifiedDate = DateTime.Now;
            companyRelationship.Company.ModifiedUser = Employee.CurrentUserName;
            companyRelationship.Company.Customers = string.IsNullOrEmpty(quickEntry.Customers) ? "" : quickEntry.Customers.Trim();
            companyRelationship.Company.Competitor = string.IsNullOrEmpty(quickEntry.Competitor) ? "" : quickEntry.Competitor.Trim();
            companyRelationship.ProgressID = quickEntry.ProgressId;
            companyRelationship.Members = new List<Member>() { };
            companyRelationship.Members.Add(CH.DB.Members.FirstOrDefault(c => c.Name == Employee.CurrentUserName && c.ProjectID == quickEntry.ProjectId));
            companyRelationship.ProjectID = quickEntry.ProjectId;
            companyRelationship.CoreLVLID = quickEntry.CoreLVLID;
            companyRelationship.MarkForDelete = false;
            companyRelationship.CreatedDate = DateTime.Now;
            companyRelationship.ModifiedDate = DateTime.Now;
            companyRelationship.PitchedPoint = string.IsNullOrEmpty(quickEntry.PitchedPoint) ? "" : quickEntry.PitchedPoint.Trim();
            companyRelationship.Description = string.IsNullOrEmpty(quickEntry.Desc) ? "" : quickEntry.Desc.Trim();
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
                    Name_CH = string.IsNullOrEmpty(quickEntry.LeadName_CN) ? "" : quickEntry.LeadName_CN.Trim(),
                    Name_EN = string.IsNullOrEmpty(quickEntry.LeadName_EN) ? "" : quickEntry.LeadName_EN.Trim(),
                    Title = string.IsNullOrEmpty(quickEntry.Title) ? "" : quickEntry.Title.Trim(),
                    CompanyID = companyRelationship.CompanyID,
                    Contact = string.IsNullOrEmpty(quickEntry.Telephone) ? "" : quickEntry.Telephone.Trim(),
                    Department = string.IsNullOrEmpty(quickEntry.Department) ? "" : quickEntry.Department.Trim(),
                    Gender = string.IsNullOrEmpty(quickEntry.Gender) ? "" : quickEntry.Gender.Trim(),
                    Mobile = string.IsNullOrEmpty(quickEntry.CellPhone) ? "" : quickEntry.CellPhone.Trim(),
                    EMail = string.IsNullOrEmpty(quickEntry.WorkingEmail) ? "" : quickEntry.WorkingEmail.Trim(),
                    MarkForDelete = false,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                };

                CH.Create<Lead>(lead);

                if (lead.ID > 0)
                {

                    var c = CH.GetDataById<CompanyRelationship>(companyRelationship.ID);
                    var mem = c.Members.FirstOrDefault(m => m.Name == Employee.CurrentUserName && m.ProjectID == c.ProjectID);
                    leadCall = new LeadCall()
                    {
                        CallBackDate = quickEntry.CallBackDate,
                        CallDate = quickEntry.CallDate,
                        CompanyRelationshipID = companyRelationship.ID,
                        LeadCallTypeID = quickEntry.CallTypeId,
                        LeadID = lead.ID,
                        MemberID = mem.ID,
                        ProjectID = quickEntry.ProjectId,
                        Result = string.IsNullOrEmpty(quickEntry.Result) ? "" : quickEntry.Result.Trim(),
                        MarkForDelete = false
                    };
                    CH.Create<LeadCall>(leadCall);
                }
            }
            return Json(new { companyRelationshipId = companyRelationship.ID, companyId = companyRelationship.CompanyID, leadId = lead.ID, leadCallId = leadCall.ID, projectId = companyRelationship.ProjectID, processid = companyRelationship.ProgressID, corelvlid = companyRelationship.CoreLVLID });
        }

        public ActionResult GetBulkEntry(int? projectId)
        {
            var progress = CH.GetAllData<Progress>().Where(p => p.Code == 10).FirstOrDefault();

            BulkEntry bulkEntry = new BulkEntry() { ProjectId = projectId, Categories = new List<int>() { }, ProgressId = progress.ID };
            LeadBulk lead = new LeadBulk();
            for (int i = 0; i < 10; i++)
            {
                bulkEntry.Leads.Add(lead);
            }
            return PartialView("BulkEntry", bulkEntry);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult BulkEntry(BulkEntry bulkEntry, FormCollection collection)
        {
            CompanyRelationship companyRelationship = new CompanyRelationship();
            companyRelationship.Company = new Company();
            companyRelationship.Company.AreaID = bulkEntry.IndustryId;
            companyRelationship.Company.CompanyTypeID = bulkEntry.TypeId;
            companyRelationship.Company.Contact = string.IsNullOrEmpty(bulkEntry.Phone) ? "" : bulkEntry.Phone.Trim();
            companyRelationship.Company.DistrictNumberID = bulkEntry.DistrictNumberId;
            companyRelationship.Company.Name_CH = string.IsNullOrEmpty(bulkEntry.Name_CN) ? "" : bulkEntry.Name_CN.Trim();
            companyRelationship.Company.Name_EN = string.IsNullOrEmpty(bulkEntry.Name_EN) ? "" : bulkEntry.Name_EN.Trim();
            companyRelationship.Company.Description = bulkEntry.Description;
            companyRelationship.Company.CreatedDate = DateTime.Now;
            companyRelationship.Company.Creator = Employee.CurrentUserName;
            companyRelationship.Company.ModifiedDate = DateTime.Now;
            companyRelationship.Company.ModifiedUser = Employee.CurrentUserName;
            companyRelationship.Company.Customers = string.IsNullOrEmpty(bulkEntry.Customers) ? "" : bulkEntry.Customers.Trim();
            companyRelationship.Company.Competitor = string.IsNullOrEmpty(bulkEntry.Competitor) ? "" : bulkEntry.Competitor.Trim();
            companyRelationship.ProgressID = bulkEntry.ProgressId;
            companyRelationship.CoreLVLID = bulkEntry.CoreLVLID;
            companyRelationship.Members = new List<Member>() { };
            companyRelationship.Members.Add(CH.DB.Members.FirstOrDefault(c => c.Name == Employee.CurrentUserName && bulkEntry.ProjectId == c.ProjectID));
            companyRelationship.ProjectID = bulkEntry.ProjectId;
            companyRelationship.MarkForDelete = false;
            companyRelationship.CreatedDate = DateTime.Now;
            companyRelationship.ModifiedDate = DateTime.Now;
            companyRelationship.PitchedPoint = string.IsNullOrEmpty(bulkEntry.PitchedPoint) ? "" : bulkEntry.PitchedPoint.Trim();
            if (bulkEntry.Categories != null)
            {
                companyRelationship.Categorys = CH.GetAllData<Category>(c => bulkEntry.Categories.Contains(c.ID)).ToList();
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

            if (companyRelationship.ID > 0)
            {
                string namecnstr = (collection["LeadName_CN"] != null) ? collection["LeadName_CN"].Trim() : "";
                string nameenstr = (collection["LeadName_EN"] != null) ? collection["LeadName_EN"].Trim() : "";
                string genderstr = (collection["Gender"] != null) ? collection["Gender"].Trim() : "";
                string titlestr = (collection["Title"] != null) ? collection["Title"].Trim() : "";
                string telstr = (collection["Telephone"] != null) ? collection["Telephone"].Trim() : "";
                string cellstr = (collection["CellPhone"] != null) ? collection["CellPhone"].Trim() : "";
                string emailstr = (collection["WorkingEmail"] != null) ? collection["WorkingEmail"].Trim() : "";
                if (!(string.IsNullOrEmpty(namecnstr) && string.IsNullOrEmpty(nameenstr)) && !string.IsNullOrEmpty(genderstr)
                        && !string.IsNullOrEmpty(titlestr) && !(string.IsNullOrEmpty(telstr) && string.IsNullOrEmpty(cellstr)))
                {
                    List<string> arrNameCN = namecnstr.Split(',').ToList();
                    List<string> arrNameEN = nameenstr.Split(',').ToList();
                    List<string> arrGender = genderstr.Split(',').ToList();
                    List<string> arrTitle = titlestr.Split(',').ToList();
                    List<string> arrTel = telstr.Split(',').ToList();
                    List<string> arrCell = cellstr.Split(',').ToList();
                    List<string> arrEmail = emailstr.Split(',').ToList();
                    string namecn = ""; string nameen = ""; string gender = "";
                    string title = ""; string tel = ""; string cell = ""; string email = "";

                    for (int i = 0; i < 10; i++)
                    {
                        namecn = arrNameCN[i];
                        nameen = arrNameEN[i];
                        gender = arrGender[i];
                        title = arrTitle[i];
                        tel = arrTel[i];
                        cell = arrCell[i];
                        email = arrEmail[i];
                        if (!(string.IsNullOrEmpty(namecn) && string.IsNullOrEmpty(nameen)) && !string.IsNullOrEmpty(gender)
                            && !string.IsNullOrEmpty(title) && !(string.IsNullOrEmpty(tel) && string.IsNullOrEmpty(cell)))
                        {
                            lead = new Lead()
                            {
                                Name_CH = string.IsNullOrEmpty(arrNameCN[i]) ? "" : arrNameCN[i].Trim(),
                                Name_EN = string.IsNullOrEmpty(arrNameEN[i]) ? "" : arrNameEN[i].Trim(),
                                Title = string.IsNullOrEmpty(arrTitle[i]) ? "" : arrTitle[i].Trim(),
                                CompanyID = companyRelationship.CompanyID,
                                Contact = string.IsNullOrEmpty(arrTel[i]) ? "" : arrTel[i].Trim(),
                                Gender = string.IsNullOrEmpty(arrGender[i]) ? "" : arrGender[i].Trim(),
                                Mobile = string.IsNullOrEmpty(arrCell[i]) ? "" : arrCell[i].Trim(),
                                EMail = string.IsNullOrEmpty(arrEmail[i]) ? "" : arrEmail[i].Trim(),
                                MarkForDelete = false,
                                CreatedDate = DateTime.Now,
                                ModifiedDate = DateTime.Now,
                            };
                            CH.Create<Lead>(lead);
                        }
                    }
                }
            }
            return Json(new { companyRelationshipId = companyRelationship.ID, companyId = companyRelationship.CompanyID, projectId = companyRelationship.ProjectID, processid = companyRelationship.ProgressID, corelvlid = companyRelationship.CoreLVLID });
        }
        /// <summary>
        /// 刷新导航栏
        /// </summary>
        /// <returns></returns>
        public PartialViewResult _RefreshCrmTree(int? projectid)
        {
            var data = AvaliableCRM.GetAvaliableCompanies(projectid);
            return PartialView(@"~\views\AvaliableCompanies\MainNavigationContainer.cshtml", data);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult PickUp(int crmid)
        {
            CompanyRelationship companyRelationship = CH.GetDataById<CompanyRelationship>(crmid);
            companyRelationship.ProgressID = 3;
            Member member = CH.DB.Members.Where(m => m.Name == Employee.CurrentUserName).FirstOrDefault();
            companyRelationship.Members.Add(member);
            CH.Edit(companyRelationship);
            return Json(new { companyRelationshipId = companyRelationship.ID, companyId = companyRelationship.CompanyID, projectId = companyRelationship.ProjectID, processid = companyRelationship.ProgressID, corelvlid = companyRelationship.CoreLVLID });
        }
    }
}
