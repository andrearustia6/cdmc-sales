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
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Configuration;
using Telerik.Web.Mvc;
using BLL;
using Telerik.Web.Mvc.UI;
using System.Collections;
namespace Sales.Controllers
{
    public class AvaliableCompaniesController : Controller
    {
        public ActionResult Index(int? projectid)
        {
            CRMFilters filters = new CRMFilters();
            if (projectid == null)
            {
                filters.ProjectId = CRM_Logical.GetUserInvolveProject().First().ID;
            }
            var data = AvaliableCRM.GetAvaliableCompanies(filters);
            if (projectid == null)
                ViewBag.projectid = CRM_Logical.GetUserInvolveProject().First().ID;
            else
                ViewBag.projectid = projectid;
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
        public PartialViewResult GetCRMByCrmIDLeadID(int crmid, int leadid)
        {
            ViewBag.leadid = leadid;
            var data = AvaliableCRM._CRMGetAvaliableCrmDetailByCrmIDLeadID(crmid, leadid);
            return PartialView(@"~\views\AvaliableCompanies\DetailContainer.cshtml", data);
        }
        public PartialViewResult GetCRMByCrmIDMember(string crmid, string membername)
        {
            ViewBag.leadid = 0;
            var data = AvaliableCRM._CRMGetAvaliableCrmDetailByCrmIDMember(int.Parse(crmid), membername);
            return PartialView(@"~\views\AvaliableCompanies\DetailContainer.cshtml", data);
        }
        public PartialViewResult GetDescription(int crmid)
        {
            var cr = CH.DB.CompanyRelationships.Find(crmid);
            return PartialView("Description", cr);
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
            lead.Deleted = false;
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

        public ActionResult GetEditCompany(int crmid)
        {
            var companyRelationship = CH.GetDataById<CompanyRelationship>(crmid);

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
                PitchedPoint = companyRelationship.PitchedPoint,
                IsVIP = companyRelationship.Company.IsVIP == null ? false : (bool)companyRelationship.Company.IsVIP,
                Info = companyRelationship.Company.Info,
                InfoRemark = companyRelationship.Company.InfoRemark,

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
            var chcount = exist.Count((c => !string.IsNullOrEmpty(afterUpdateCN) & c.Company.Name_CH == afterUpdateCN && c.Company.Name_CH != beforeUpdateCN && c.Company.Deleted==false));
            if (chcount > 0)
            {
                return Content("同名中文公司名字已存在！");
            }

            var encount = exist.Count(c => !string.IsNullOrEmpty(afterUpdateEN) & c.Company.Name_EN == afterUpdateEN && c.Company.Name_EN != beforeUpdateEN && c.Company.Deleted==false);
            if (encount > 0)
            {
                return Content("同名英文公司名字已存在！");
            }

            return Content("");
        }
        public ActionResult CheckCompanyNameCNExist(string name,int projectid)
        {
            var exist = from c in CH.DB.CompanyRelationships select c;
            var chcount = exist.Count(c => c.Company.Name_CH == name && c.ProjectID==projectid && c.Deleted==false );
            if (chcount > 0)
            {
                return Json(new { flag = 0, Content = "本项目中已经存在同名中文公司，不能重复录入！"});
            }
            var ex = exist.Any(c=> c.Company.Name_CH == name && c.ProjectID!=projectid && c.Company.Deleted==false);
            if (ex)
            {
                return Json(new { flag = 1, crmid = exist.Where(c => c.Company.Name_CH == name && c.ProjectID != projectid && c.Company.Deleted==false).FirstOrDefault().ID, Content = "已经存在同名中文公司，是否领用?" });
            }

            return Json(new { flag = 2, Content = "" });
        }
        public ActionResult CheckCompanyNameENExist(string name, int projectid)
        {
            var exist = from c in CH.DB.CompanyRelationships select c;
            var chcount = exist.Count(c => c.Company.Name_EN == name && c.ProjectID == projectid && c.Deleted == false);
            if (chcount > 0)
            {
                return Json(new { flag = 0, Content = "本项目中已经存在同名英文公司，不能重复录入！" });
            }
            var ex = exist.Any(c => c.Company.Name_EN == name && c.ProjectID != projectid && c.Company.Deleted == false);
            if (ex)
            {
                return Json(new { flag = 1, crmid = exist.Where(c => c.Company.Name_EN == name && c.ProjectID != projectid && c.Company.Deleted == false).FirstOrDefault().ID, Content = "已经存在同名英文公司，是否领用?" });
            }

            return Json(new { flag = 2, Content = "" });
        }
        [ValidateInput(false)]
        public ActionResult CheckMemberShip(int? projectid)
        {
            var exist = from c in CH.DB.Members.Where(w => w.ProjectID == projectid && w.Name == Employee.CurrentUserName) select c;
            if (exist.Count() == 0)
            {
                return Content("抱歉，您不是member，要成为member才能操作！");
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
            companyRelationship.Company.IsVIP = ajaxViewSaleCompany.IsVIP;
            companyRelationship.Company.Info = ajaxViewSaleCompany.Info;
            companyRelationship.Company.InfoRemark = ajaxViewSaleCompany.InfoRemark;
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
            var core = CH.GetAllData<CoreLVL>().Where(c => c.CoreLVLCode == 2).FirstOrDefault();
            AjaxViewSaleCompany ajaxViewSaleCompany = new AjaxViewSaleCompany() { ProjectId = projectId, Categories = new List<int>() { }, ProgressId = progress.ID, CoreLVLID = core.ID };
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
            companyRelationship.Company.IsVIP = ajaxViewSaleCompany.IsVIP;
            companyRelationship.Company.Info = ajaxViewSaleCompany.Info;
            companyRelationship.Company.InfoRemark = ajaxViewSaleCompany.InfoRemark;
            companyRelationship.Company.Deleted = false;
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
            companyRelationship.Deleted = false;
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
            CrmTrack ct = new CrmTrack();
            ct.Owner = Employee.CurrentUserName;
            ct.Type = "自加";
            ct.CompanyRelationshipID = companyRelationship.ID;
            ct.GetDate = DateTime.Now;
            CH.Create<CrmTrack>(ct);
            return Json(new { companyRelationshipId = companyRelationship.ID, companyId = companyRelationship.CompanyID, projectId = companyRelationship.ProjectID, corelvlid = companyRelationship.CoreLVLID, processid = companyRelationship.ProgressID });
        }
        public ActionResult GetAddCall(int leadId, int companyRelationId, int? projectId)
        {
            AjaxViewLeadCall ajaxViewLeadCall = new AjaxViewLeadCall() { LeadId = leadId, CompanyRelationshipId = companyRelationId, ProjectId = projectId, CallDate = DateTime.Now, ProgressId = (int)CH.GetDataById<CompanyRelationship>(companyRelationId).ProgressID };
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
            //添加人不是member
            if (memid == 0)
            {
                Member m = new Member() { Name = name, ProjectID = ajaxViewLeadCall.ProjectId, SalesTypeID = CH.GetAllData<SalesType>().FirstOrDefault(w => w.Name == "其他").ID };
                CH.Create<Member>(m);
                memid = m.ID;
            }
            leadCall.MemberID = memid;

            leadCall.ProjectID = ajaxViewLeadCall.ProjectId;
            leadCall.Result = ajaxViewLeadCall.Result;
            leadCall.MarkForDelete = false;
            leadCall.Deleted = false;
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
            CompanyRelationship crm = CH.GetDataById<CompanyRelationship>(crmid);
            _Comment comment = new _Comment() { CRMID = crmid, CrmCommentStateID = crm.CrmCommentStateID };
            return PartialView("Comment", comment);
        }
        [ValidateInput(false)]
        public ActionResult AddComment(_Comment comment)
        {
            Comment comm = new Comment();
            comm.CommentDate = DateTime.Now;
            comm.Submitter = Employee.CurrentUserName;
            comm.Contents = comment.Contents;
            comm.CompanyRelationshipID = comment.CRMID;
            comm.Deleted = false;
            CH.Create<Comment>(comm);

            CompanyRelationship crm = CH.GetDataById<CompanyRelationship>(comment.CRMID);
            crm.CrmCommentStateID = comment.CrmCommentStateID;
            CH.Edit<CompanyRelationship>(crm);
            return Json(new { companyRelationshipId = crm.ID, companyId = crm.CompanyID, projectId = crm.ProjectID, corelvlid = crm.CoreLVLID, processid = crm.ProgressID });
        }

        public ActionResult GetQuickEntry(int? projectId)
        {
            var progress = CH.GetAllData<Progress>().Where(p => p.Code == 10).FirstOrDefault();
            var core = CH.GetAllData<CoreLVL>().Where(c => c.CoreLVLCode == 2).FirstOrDefault();
            QuickEntry quickEntry = new QuickEntry() { ProjectId = projectId, Categories = new List<int>() { }, ProgressId = progress.ID, CallDate = DateTime.Now, CoreLVLID = core.ID };
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
            companyRelationship.Company.Deleted = false;
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
            companyRelationship.Deleted = false;
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
                    Deleted=false,
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
                        MarkForDelete = false,
                        Deleted=false,
                    };
                    CH.Create<LeadCall>(leadCall);
                }
            }
            return Json(new { companyRelationshipId = companyRelationship.ID, companyId = companyRelationship.CompanyID, leadId = lead.ID, leadCallId = leadCall.ID, projectId = companyRelationship.ProjectID, processid = companyRelationship.ProgressID, corelvlid = companyRelationship.CoreLVLID });
        }

        public ActionResult GetBulkEntry(int? projectId)
        {
            var progress = CH.GetAllData<Progress>().Where(p => p.Code == 10).FirstOrDefault();
            var core = CH.GetAllData<CoreLVL>().Where(c => c.CoreLVLCode == 2).FirstOrDefault();
            BulkEntry bulkEntry = new BulkEntry() { ProjectId = projectId, Categories = new List<int>() { }, ProgressId = progress.ID, CoreLVLID = core.ID };
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
            companyRelationship.Company.Deleted = false;
            companyRelationship.ProgressID = bulkEntry.ProgressId;
            companyRelationship.CoreLVLID = bulkEntry.CoreLVLID;
            companyRelationship.Members = new List<Member>() { };
            companyRelationship.Members.Add(CH.DB.Members.FirstOrDefault(c => c.Name == Employee.CurrentUserName && bulkEntry.ProjectId == c.ProjectID));
            companyRelationship.ProjectID = bulkEntry.ProjectId;
            companyRelationship.MarkForDelete = false;
            companyRelationship.CreatedDate = DateTime.Now;
            companyRelationship.ModifiedDate = DateTime.Now;
            companyRelationship.Deleted = false;
            companyRelationship.Description =  string.IsNullOrEmpty(bulkEntry.Description) ? string.Empty : bulkEntry.Description.Trim();
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
                                Deleted=false,
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
        public PartialViewResult _RefreshCrmTree(CRMFilters f)
        {
            var data = AvaliableCRM.GetAvaliableCompanies(f);
            
            return PartialView(@"~\views\AvaliableCompanies\MainNavigationContainer.cshtml", data);
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult _PickUpByOtherSales(CRMFilters filters)
        {
            if (string.IsNullOrWhiteSpace(filters.FuzzyQuery))
                return Json(new { msg = "" });
            var owncrms = from c in CH.DB.CompanyRelationships where c.ProjectID == filters.ProjectId && c.Deleted==false select c;
            if (Employee.CurrentRole.Level == SalesRequired.LVL || Employee.CurrentRole.Level == MarketInterfaceRequired.LVL )
                owncrms = owncrms.Where(w => w.Members.Count > 0 && w.Members.Any(m => m.Name == Employee.CurrentUserName)).OrderBy(w => w.ID);
            else if (Employee.CurrentRole.Level == ManagerRequired.LVL || Employee.CurrentRole.Level == LeaderRequired.LVL || Employee.CurrentRole.Level == ProductInterfaceRequired.LVL)
                owncrms = owncrms.Where(w => w.Members.Count > 0).OrderBy(w => w.ID);
            
            //模糊搜索
            if (filters != null && !string.IsNullOrWhiteSpace(filters.FuzzyQuery))
            {
                owncrms = owncrms.Where(q => q.Company.Leads.Any(l =>  
                    l.Deleted==false &&
                    (
                        l.Name_CH.Contains(filters.FuzzyQuery) || 
                        l.Name_EN.Contains(filters.FuzzyQuery) || 
                        l.EMail.Contains(filters.FuzzyQuery) || 
                        l.PersonalEmailAddress.Contains(filters.FuzzyQuery)
                       )) || 
                       (q.Company.Deleted==false &&
                        (q.Company.Name_CH.Contains(filters.FuzzyQuery) || 
                        q.Company.Name_EN.Contains(filters.FuzzyQuery) || 
                        q.Company.Contact.Contains(filters.FuzzyQuery)))
                    
                    );
            }
            //行业
            if (filters != null && filters.CategoryId.HasValue)
            {
                owncrms = owncrms.Where(q => q.Categorys.Any(c => c.ID == filters.CategoryId.Value));
            }
            //时区
            if (filters != null && filters.DistinctNumber.HasValue)
            {
                owncrms = owncrms.Where(q => q.Company.DistrictNumberID == filters.DistinctNumber);
            }
            //点评
            if (filters != null && filters.IfComment == 1)
            {
                owncrms = owncrms.Where(q => q.Comments.Count > 0);
            }
            if (filters != null && filters.IfComment == 0)
            {
                owncrms = owncrms.Where(q => q.Comments.Count == 0);
            }

            if (filters != null && !string.IsNullOrWhiteSpace(filters.selSales))
            {
                owncrms = owncrms.Where(s => s.Members.Any(q => q.Name == filters.selSales));
            }


            var publiccrms = from c in CH.DB.CompanyRelationships where c.ProjectID == filters.ProjectId && c.Deleted == false select c;

            publiccrms = publiccrms.Where(w => w.Members.Count == 0);

            //模糊搜索
            if (filters != null && !string.IsNullOrWhiteSpace(filters.FuzzyQuery))
            {
                publiccrms = publiccrms.Where(q => q.Company.Leads.Any(l => l.Deleted == false && (l.Name_CH.Contains(filters.FuzzyQuery) || l.Name_EN.Contains(filters.FuzzyQuery) || l.EMail.Contains(filters.FuzzyQuery) || l.PersonalEmailAddress.Contains(filters.FuzzyQuery)) || q.Company.Name_CH.Contains(filters.FuzzyQuery) || q.Company.Name_EN.Contains(filters.FuzzyQuery) || q.Company.Contact.Contains(filters.FuzzyQuery)));
            }
            //行业
            if (filters != null && filters.CategoryId.HasValue)
            {
                publiccrms = publiccrms.Where(q => q.Categorys.Any(c => c.ID == filters.CategoryId.Value));
            }
            //时区
            if (filters != null && filters.DistinctNumber.HasValue)
            {
                publiccrms = publiccrms.Where(q => q.Company.DistrictNumberID == filters.DistinctNumber);
            }
            //点评
            if (filters != null && filters.IfComment == 1)
            {
                publiccrms = publiccrms.Where(q => q.Comments.Count > 0);
            }
            if (filters != null && filters.IfComment == 0)
            {
                publiccrms = publiccrms.Where(q => q.Comments.Count == 0);
            }
            if(owncrms.Count()>0 || publiccrms.Count()>0)
                return Json(new { msg = "" });

            var crms = from c in CH.DB.CompanyRelationships where c.ProjectID == filters.ProjectId && c.Deleted == false select c;
            crms = crms.Where(w => w.Members.Count > 0 && w.Members.Any(m => m.Name == Employee.CurrentUserName)==false).OrderBy(w => w.ID);
            //模糊搜索
            if (filters != null && !string.IsNullOrWhiteSpace(filters.FuzzyQuery))
            {
                crms = crms.Where(q => q.Company.Leads.Any(l =>
                    l.Deleted == false &&
                    (
                        l.Name_CH.Contains(filters.FuzzyQuery) ||
                        l.Name_EN.Contains(filters.FuzzyQuery) ||
                        l.EMail.Contains(filters.FuzzyQuery) ||
                        l.PersonalEmailAddress.Contains(filters.FuzzyQuery)
                       )) ||
                       (q.Company.Deleted == false &&
                        (q.Company.Name_CH.Contains(filters.FuzzyQuery) ||
                        q.Company.Name_EN.Contains(filters.FuzzyQuery) ||
                        q.Company.Contact.Contains(filters.FuzzyQuery)))

                    );
            }
            //行业
            if (filters != null && filters.CategoryId.HasValue)
            {
                crms = crms.Where(q => q.Categorys.Any(c => c.ID == filters.CategoryId.Value));
            }
            //时区
            if (filters != null && filters.DistinctNumber.HasValue)
            {
                crms = crms.Where(q => q.Company.DistrictNumberID == filters.DistinctNumber);
            }
            //点评
            if (filters != null && filters.IfComment == 1)
            {
                crms = crms.Where(q => q.Comments.Count > 0);
            }
            if (filters != null && filters.IfComment == 0)
            {
                crms = crms.Where(q => q.Comments.Count == 0);
            }

            
            string ret = "符合条件的公司有:";
            bool hascompany = false;

            foreach (var crm in crms)
            {
                //var track = CH.DB.CrmTracks.Where(w=>w.CompanyRelationshipID==crm.ID ).OrderByDescending(w=>w.CreatedDate).FirstOrDefault();
                var members = crm.Members;
                if (crm.Members != null)
                {
                    ret += crm.CompanyName + "(领用人:" + string.Join(",",crm.Members.Select(w=>w.Name).ToArray()) + ");";
                    hascompany = true;
                }
                //if (track != null)
                //{
                //    if (track.Type != "放回")
                //    {
                //        ret += crm.CompanyName + "(领用人:" + track.Owner + ");";
                //        hascompany = true;
                //    }
                //}
               
            }
            if(hascompany)
                return Json(new { msg = ret });
            else
                return Json(new { msg = "" });
        }
        [HttpPost]
        public ActionResult _AjaxTreeViewLoadingCore(CRMFilters filters)
        {
            if (filters.ProjectId == null) filters.ProjectId = 26;
            var crms = from c in CH.DB.CompanyRelationships where c.ProjectID == filters.ProjectId && c.Deleted==false select c;

            crms = crms.Where(w => w.Members.Count == 0);

            //模糊搜索
            if (filters != null && !string.IsNullOrWhiteSpace(filters.FuzzyQuery))
            {
                crms = crms.Where(q => q.Company.Leads.Any(l =>l.Deleted==false &&( l.Name_CH.Contains(filters.FuzzyQuery) || l.Name_EN.Contains(filters.FuzzyQuery) || l.EMail.Contains(filters.FuzzyQuery) || l.PersonalEmailAddress.Contains(filters.FuzzyQuery)) || q.Company.Name_CH.Contains(filters.FuzzyQuery) || q.Company.Name_EN.Contains(filters.FuzzyQuery) || q.Company.Contact.Contains(filters.FuzzyQuery)));
            }
            //行业
            if (filters != null && filters.CategoryId.HasValue)
            {
                crms = crms.Where(q => q.Categorys.Any(c => c.ID == filters.CategoryId.Value));
            }
            //时区
            if (filters != null && filters.DistinctNumber.HasValue)
            {
                crms = crms.Where(q => q.Company.DistrictNumberID == filters.DistinctNumber);
            }
            //点评
            if (filters != null && filters.IfComment == 1)
            {
                crms = crms.Where(q => q.Comments.Count > 0);
            }
            if (filters != null && filters.IfComment == 0)
            {
                crms = crms.Where(q => q.Comments.Count == 0);
            }
            var data = from c in CH.DB.CoreLVLs
                      select new Sales.Model.TreeViewItemModel
                       {
                           Value = c.ID,
                           NameCH = c.CoreLVLName,
                           NameEN="",
                           LoadOnDemand=true,
                           Enabled=true,
                           CrmCount = crms.Where(cr => cr.CoreLVLID == c.CoreLVLCode).Count()
                       };
            return Json ( data );
        }
        [HttpPost]
        public ActionResult _AjaxTreeViewLoading(CRMFilters filters)
        {
            var crms = from c in CH.DB.CompanyRelationships where c.ProjectID == filters.ProjectId && c.Deleted==false select c;

            crms = crms.Where(w => w.Members.Count == 0);
            //模糊搜索
            if (filters != null && !string.IsNullOrWhiteSpace(filters.FuzzyQuery))
            {
                crms = crms.Where(q => q.Company.Leads.Any(l =>l.Deleted==false &&( l.Name_CH.Contains(filters.FuzzyQuery) || l.Name_EN.Contains(filters.FuzzyQuery) || l.EMail.Contains(filters.FuzzyQuery) || l.PersonalEmailAddress.Contains(filters.FuzzyQuery)) || q.Company.Name_CH.Contains(filters.FuzzyQuery) || q.Company.Name_EN.Contains(filters.FuzzyQuery) || q.Company.Contact.Contains(filters.FuzzyQuery)));
            }
            //行业
            if (filters != null && filters.CategoryId.HasValue)
            {
                crms = crms.Where(q => q.Categorys.Any(c => c.ID == filters.CategoryId.Value));
            }
            //时区
            if (filters != null && filters.DistinctNumber.HasValue)
            {
                crms = crms.Where(q => q.Company.DistrictNumberID == filters.DistinctNumber);
            }
            //点评
            if (filters != null && filters.IfComment == 1)
            {
                crms = crms.Where(q => q.Comments.Count > 0);
            }
            if (filters != null && filters.IfComment == 0)
            {
                crms = crms.Where(q => q.Comments.Count == 0);
            }
            IEnumerable nodes = from crm in crms.OrderBy(w=>w.Company.Name_EN).ThenBy(w=>w.Company.Name_CH)
                                where crm.CoreLVLID == filters.CoreId
                       select new Sales.Model.TreeViewItemModel
                       {
                           Checkable=true,
                           Value = crm.ID,
                           NameEN = crm.Company.Name_EN,
                           NameCH = crm.Company.Name_CH,
                           LoadOnDemand=false,
                           Enabled=true,
                           CrmCount=-1,
                       };
            
            return new JsonResult { Data = nodes };
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult PickUp(int crmid)
        {
            CompanyRelationship companyRelationship = CH.GetDataById<CompanyRelationship>(crmid);
            //var p = companyRelationship.Project.CompanyRelationships.Count(w => w.Members.Where(m => m.Name == Employee.CurrentUserName).Any() == true);
            //var d = CH.DB.Members.Where(w => w.Name == Employee.CurrentUserName && w.CompanyRelationships.Where(c=>c.ProjectID == companyRelationship.ProjectID).Any() == true).Count();
            var d = CH.DB.CompanyRelationships.Where(w =>w.Deleted==false && w.Members.Where(m => m.Name == Employee.CurrentUserName).Any() == true && w.ProjectID == companyRelationship.ProjectID).Count();
            if (d >= 150)
                return Content("从公海领用的，公司数超过150的不能领用！");

            Member member = CH.DB.Members.Where(m => m.Name == Employee.CurrentUserName && m.ProjectID == companyRelationship.ProjectID).FirstOrDefault();
            companyRelationship.Members.Add(member);
            CH.Edit(companyRelationship);

            doCrmTrack(crmid, true);

            return Json(new { companyRelationshipId = companyRelationship.ID, companyId = companyRelationship.CompanyID, projectId = companyRelationship.ProjectID, processid = companyRelationship.ProgressID, corelvlid = companyRelationship.CoreLVLID });
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult PickUpDirect(int? crmid,int projectid)
        {
            CompanyRelationship companyRelationship = CH.GetDataById<CompanyRelationship>(crmid);
            var d = CH.DB.CompanyRelationships.Where(w => w.Deleted == false && w.Members.Where(m => m.Name == Employee.CurrentUserName).Count() > 0 && w.ProjectID == projectid).Count();
            if (d >= 150)
                return Content("从公海领用的，公司数超过150的不能领用！");

            Member member = CH.DB.Members.Where(m => m.Name == Employee.CurrentUserName && m.ProjectID==projectid).FirstOrDefault();
            companyRelationship.ProjectID = projectid;
            companyRelationship.Members.Add(member);

            int id =CH.Create<CompanyRelationship>(companyRelationship);

            doCrmTrack(companyRelationship.ID, true);

            return Json(new { companyRelationshipId = companyRelationship.ID, companyId = companyRelationship.CompanyID, projectId = companyRelationship.ProjectID, processid = companyRelationship.ProgressID, corelvlid = companyRelationship.CoreLVLID });
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult BulkPickUp(List<int> crmid)
        {
            //List<int> crmid = ids.ToList();
            int id = crmid[0];
            CompanyRelationship companyRelationship1 = CH.GetDataById<CompanyRelationship>(id);
            //var p = companyRelationship1.Project.CompanyRelationships.Count(w => w.Members.Where(m => m.Name == Employee.CurrentUserName).Any() == true);
            var d = CH.DB.CompanyRelationships.Where(w => w.Deleted == false && w.Members.Where(m => m.Name == Employee.CurrentUserName).Any() == true && w.ProjectID == companyRelationship1.ProjectID).Count();
            if (d + crmid.Count>= 150)
                return Content("从公海领用的，公司数超过150的不能领用！");
            int companyRelationshipId=0;
            int companyId = 0;
            int projectId = 0;
            int processid = 0;
            int corelvlid = 0; 
            for (int i = 0; i < crmid.Count; i++)
            {
                CompanyRelationship companyRelationship = CH.GetDataById<CompanyRelationship>(crmid[i]);
                Member member = CH.DB.Members.Where(m => m.Name == Employee.CurrentUserName && m.ProjectID == companyRelationship.ProjectID).FirstOrDefault();
                companyRelationship.Members.Add(member);
                CH.Edit(companyRelationship);
                doCrmTrack(crmid[i], true);
                companyRelationshipId =(int) companyRelationship.ID;
                companyId = (int)companyRelationship.CompanyID;
                projectId = (int)companyRelationship.ProjectID;
                processid = (int)companyRelationship.ProgressID;
                corelvlid = (int)companyRelationship.CoreLVLID;
            }
            return Json(new { companyRelationshipId = companyRelationshipId, companyId = companyId, projectId = projectId, processid = processid, corelvlid = corelvlid });
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult UnPickUp(int crmid)
        {

            CompanyRelationship companyRelationship = CH.GetDataById<CompanyRelationship>(crmid);

            Member member = companyRelationship.Members.Where(m => m.Name == Employee.CurrentUserName && m.ProjectID == companyRelationship.ProjectID).FirstOrDefault();

            companyRelationship.Members.Remove(member);
            CH.Edit(companyRelationship);

            doCrmTrack(crmid, false);



            return Json(new { companyRelationshipId = companyRelationship.ID, companyId = companyRelationship.CompanyID, projectId = companyRelationship.ProjectID, processid = companyRelationship.ProgressID, corelvlid = companyRelationship.CoreLVLID });
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult ChangeCore(int crmid, int corelvlid)
        {
            CompanyRelationship companyRelationship = CH.GetDataById<CompanyRelationship>(crmid);
            companyRelationship.CoreLVLID = corelvlid;
            CH.Edit(companyRelationship);
            return Json(new { companyRelationshipId = companyRelationship.ID, companyId = companyRelationship.CompanyID, projectId = companyRelationship.ProjectID, processid = companyRelationship.ProgressID, corelvlid = corelvlid });
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult ChangeProgress(int crmid, int ProgressID)
        {
            CompanyRelationship companyRelationship = CH.GetDataById<CompanyRelationship>(crmid);
            companyRelationship.ProgressID = ProgressID;
            CH.Edit(companyRelationship);

            ProgressTrack pt = new ProgressTrack();
            pt.ProgressID = ProgressID;
            pt.ChangeDate = DateTime.Now;
            pt.CompanyRelationshipID = crmid;
            pt.Owner = Employee.CurrentUserName;
            CH.Create(pt);

            return Json(new { companyRelationshipId = companyRelationship.ID, companyId = companyRelationship.CompanyID, projectId = companyRelationship.ProjectID, processid = companyRelationship.ProgressID, corelvlid = companyRelationship.CoreLVLID });
        }

        public ActionResult GetEmailPage(string callType, int leadid, string templatename)
        {
            Lead lead = CH.GetDataById<Lead>(leadid);
            string template = "";
            string filename = templatename + ".html";

            #region Read template from file.
            string file = AppDomain.CurrentDomain.BaseDirectory + "/App_Data/" + filename;
            if (System.IO.File.Exists(file))
            {
                FileInfo fi1einfo = new FileInfo(file);
                using (StreamReader sr = fi1einfo.OpenText())
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        template += s;
                    }
                }
            }
            if (lead.DistrictNumberID == null)
                template = template.Replace("{CLIENTNAME}", string.IsNullOrEmpty(lead.Name_CH) ? "客户" : lead.Name_CH);
            else
                template = template.Replace("{CLIENTNAME}", string.IsNullOrEmpty(lead.Name_EN) ? "Client" : lead.Name_EN);
            #endregion

            EmailModel model = new EmailModel();
            model.Content = template;
            model.ToEmail = lead.EMail;
            model.ToName = string.Join(",", lead.Name_EN, lead.Name_CH).Trim(',');
            return PartialView("EmailPage", model);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult EmailPage(EmailModel model)
        {
            if (SendEmail(model.ToEmail, model.ToName, model.Subject, model.Content))
                return Json(new { sentEmail = true });
            else
                return Json(new { sentEmail = false });
        }

        /// <summary>
        /// Methods to send email.
        /// </summary>
        /// <param name="toEmail"></param>
        /// <param name="toName"></param>
        /// <param name="subject"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public bool SendEmail(string toEmail, string toName, string subject, string content)
        {
            //UserInfoModel currUser =  Employee.GetUserByName(Employee.CurrentUserName);
            var  currUsers = from c in CH.DB.EmployeeRoles where c.AccountName == Employee.CurrentUserName select c;
            EmployeeRole currUser = currUsers.FirstOrDefault();
            if ((currUser == null) || (String.IsNullOrEmpty(currUser.Email)) || string.IsNullOrEmpty(currUser.EmailPassword))
                return false;

            // Send email
            SmtpClient smtp = new SmtpClient();
            smtp.Host = ConfigurationManager.AppSettings["SMTP_Server"].ToString();
            smtp.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SMTP_Port"].ToString());
            //NetworkCredential SMTPUserInfo = new NetworkCredential(ConfigurationManager.AppSettings["SMTP_Username"].ToString(), ConfigurationManager.AppSettings["SMTP_Password"].ToString());
            NetworkCredential SMTPUserInfo = new NetworkCredential(currUser.Email, currUser.EmailPassword);
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = SMTPUserInfo;

            //MailAddress from = new MailAddress("system@cdmc.org.cn", "system@cdmc.org.cn", System.Text.Encoding.Unicode);
            MailAddress from = new MailAddress(currUser.Email, currUser.EmailSignatures, System.Text.Encoding.Unicode);
            MailAddress to = new MailAddress(toEmail, toName, System.Text.Encoding.UTF8);
            MailMessage message = new MailMessage(from, to);
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            message.Subject = subject;
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.Body = content;
            message.IsBodyHtml = true;
            try
            {
                smtp.Send(message);
                return true;
            }
            catch
            {
                return false;
            }
            finally { message.Dispose(); smtp.Dispose(); }
        }

        [HttpPost]
        public bool IsAddressAvailable(int CRMId)
        {
            //var res = new JsonResult();
            bool result = false;

            var CRM = CH.GetDataById<CompanyRelationship>(CRMId);
            var CRMCompany = CH.GetDataById<Company>(CRM.CompanyID);
            if ((CRMCompany != null) && (CRMCompany.DistrictNumberID == null))
            {
                result = true;
            }
            //res.Data = result;
            return result;
        }

        [HttpGet]
        public ActionResult GetCatagories(int currentProjectId)
        {
            List<Category> categoriylist = CH.GetAllData<Category>(o => o.ProjectID == currentProjectId).ToList();
            Category ca = new Category();
            ca.ID = -1;
            ca.Name = "不指定";
            categoriylist.Insert(0, ca);
            return Json(categoriylist.Select(c => new { Value = c.ID, Text = c.Name }), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public PartialViewResult GetPitchPoint1(int crmid)
        {
            var cr = CH.DB.CompanyRelationships.Find(crmid);
            return PartialView("GetPitchPoint", cr);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditCategories(FormCollection c)
        {
            string[] ids = Server.UrlDecode(c["ids"]).Split('|');
            string[] cas = Server.UrlDecode(c["cas"]).Split('|');
            for (int i = 0; i < ids.Length; i++)
            {
                try
                {
                    if (ids[i].Trim() != "")
                    {
                        int id = int.Parse(ids[i].Trim());
                        Category category = CH.DB.Categorys.Find(id);
                        category.Details = Server.UrlDecode(cas[i]);
                        CH.Edit(category);
                    }
                }
                catch (Exception ex) { }
            }
            return Json("");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditPitchPoint(FormCollection c)
        {
            int id = int.Parse(c["crmid"]);
            string pp = c["pp"];
            var cr = CH.DB.CompanyRelationships.Find(id);
            cr.PitchedPoint = pp;
            CH.Edit<CompanyRelationship>(cr);

            return Json("");
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditDescription(FormCollection c)
        {
            int id = int.Parse(c["crmid"]);
            string pp = c["pp"];
            var cr = CH.DB.CompanyRelationships.Find(id);
            cr.Company.Description = pp;
            CH.Edit<CompanyRelationship>(cr);

            return Json("");
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditCompetitor(FormCollection c)
        {
            int id = int.Parse(c["crmid"]);
            string pp = c["pp"];
            var cr = CH.DB.CompanyRelationships.Find(id);
            cr.Company.Competitor = pp;
            CH.Edit<CompanyRelationship>(cr);

            return Json("");
        }
        [HttpPost]
        public PartialViewResult GetAssignCompany(int crmid, int projectid)
        {
            var result = CH.DB.CompanyRelationships.Find(crmid).Members.Where(s => s.IsActivated && s.ProjectID == projectid).Select(s=>s.ID);
            ViewBag.project = CH.GetDataById<Project>(projectid);
            return PartialView("AssignCompany", result);
        }

        [HttpPost]
        public ActionResult SaveAssignCompany(int id, string ids)
        {
            try
            {
                var cr = CH.DB.CompanyRelationships.Find(id);
                List<int> idlist = ids.Split(',').Where(x => x.Trim() != "").Select(x => int.Parse(x)).ToList();
                List<int> crids = cr.Members.Select(x => x.ID).ToList();
                //新增
                foreach (var x in idlist)
                {
                    if (crids.Contains(x) == false)
                    {
                        Member member = CH.DB.Members.Find(x);
                        cr.Members.Add(member);
                        doCrmTrack(id, true);
                    }
                }
                //减少
                foreach (var x in crids)
                {
                    if (idlist.Contains(x) == false)
                    {
                        Member member = CH.DB.Members.Find(x);
                        cr.Members.Remove(member);
                        doCrmTrack(id, false);
                    }
                }

                //存入
                CH.Edit<CompanyRelationship>(cr);
            }
            catch (Exception ex)
            { }
            return Json("");
        }

        /// <summary>
        /// 处理CrmTrack
        /// </summary>
        /// <param name="crmid"></param>
        /// <param name="picked">true:领用；false：放回</param>
        private void doCrmTrack(int crmid, bool picked)
        {
            if (picked)
            {
                CrmTrack ct = new CrmTrack();
                ct.Owner = Employee.CurrentUserName;
                ct.Type = "领用";
                ct.CompanyRelationshipID = crmid;
                ct.GetDate = DateTime.Now;
                CH.Create<CrmTrack>(ct);
            }
            else
            {
                CrmTrack ct = CH.DB.CrmTracks.Where(ct1 => ct1.CompanyRelationshipID == crmid && ct1.Owner == Employee.CurrentUserName).OrderByDescending(ct1 => ct1.GetDate).FirstOrDefault();
                if (ct != null)
                {
                    ct.Owner = Employee.CurrentUserName;
                    ct.Type = "放回";
                    ct.ReleaseDate = DateTime.Now;
                    CH.Edit<CrmTrack>(ct);
                }
                else
                {
                    ct = new CrmTrack();
                    ct.Owner = Employee.CurrentUserName;
                    ct.Type = "放回";
                    ct.CompanyRelationshipID = crmid;
                    ct.ReleaseDate = DateTime.Now;
                    CH.Create<CrmTrack>(ct);
                }
            }
        }
        public PartialViewResult GetCoreCoverage(int projectid, int coreid)
        {
            //var crms = CH.DB.CompanyRelationships.Where(cr => cr.ProjectID == projectid && cr.CoreLVLID == coreid && cr.Members.Count > 0);
            //if (Employee.CurrentRole.Level == SalesRequired.LVL || Employee.CurrentRole.Level == LeaderRequired.LVL)
            //{
            //    crms = crms.Where(cr => cr.Members.Any(m => m.Name == Employee.CurrentUserName));
            //}
            //var ccs = from l in crms
            //          select new _CoreCoverage()
            //          {
            //              CompanyName = l.Company.Name_CH.Length > 0 ? l.Company.Name_CH + "|" + l.Company.Name_EN : l.Company.Name_EN,
            //              Members = l.Members,
            //              PickUpTime = l.CrmTracks.OrderByDescending(ct => ct.GetDate).FirstOrDefault() != null ? l.CrmTracks.OrderByDescending(ct => ct.GetDate).FirstOrDefault().GetDate : null,
            //              LeadCalledCount = l.LeadCalls.GroupBy(lc => lc.LeadID).Count(),
            //              Calls = l.LeadCalls,
            //              ProcessName = l.Progress.Name
            //          };
            ViewBag.projectid = projectid;
            ViewBag.coreid = coreid;
            //return PartialView(@"~\views\AvaliableCompanies\CoreCoverage.cshtml", ccs);
            return PartialView(@"~\views\AvaliableCompanies\CoreCoverage.cshtml");
        }
        [GridAction]
        public ActionResult _CoreCoverage(int projectid, int coreid,int? typeid)
        {
            var crms = CH.DB.CompanyRelationships.Where(cr => cr.ProjectID == projectid && cr.CoreLVLID == coreid && cr.Members.Count > 0 && cr.Deleted==false);
            if (Employee.CurrentRole.Level == SalesRequired.LVL || Employee.CurrentRole.Level == LeaderRequired.LVL)
            {
                crms = crms.Where(cr => cr.Members.Any(m => m.Name == Employee.CurrentUserName));
            }

            var ccs = from l in crms
                      select new _CoreCoverage()
                      {
                          CompanyName = l.Company.Name_EN.Length > 0 ? l.Company.Name_EN : l.Company.Name_CH,
                          
                          PickUpTime = l.CrmTracks.OrderByDescending(ct => ct.GetDate).FirstOrDefault() != null ? l.CrmTracks.OrderByDescending(ct => ct.GetDate).FirstOrDefault().GetDate : null,
                          LeadCalledCount = l.LeadCalls.Where(lc => lc.Deleted == false && lc.CreatedDate.Value.Year==DateTime.Now.Year ).GroupBy(lc => lc.LeadID).Count(),
                          Calls = from c in l.LeadCalls.Where(lc => lc.Deleted == false && lc.CallDate >= l.CreatedDate && lc.CreatedDate.Value.Year == DateTime.Now.Year)
                                  select new _LeadCall()
                                  {
                                      MemberName = c.Member.Name,
                                      LeadID = c.LeadID
                                  },
                          ProcessName = l.Progress.Name,
                          DealCount = l.Deals.Count(),
                          Members = l.Members
                      };
            CH.DB.Configuration.ProxyCreationEnabled = false;
            if (typeid == 0)
                ccs = ccs.Where(w => w.LeadCalledCount == 0);
            else if (typeid == 1)
                ccs = ccs.Where(w => w.LeadCalledCount == 1);
            else if (typeid == 2)
                ccs = ccs.Where(w => w.LeadCalledCount == 2);
            else if (typeid == 3)
                ccs = ccs.Where(w => w.LeadCalledCount >= 3);
            ccs = ccs.OrderByDescending(w => w.DealCount).ThenBy(w=>w.CompanyName);

            return View(new GridModel(ccs.ToList()));
        }
        [GridAction]
        public ActionResult _PickUpList(int projectid, int coreid)
        {
            var saleslist = CH.DB.Members.Where(w => w.ProjectID == projectid && w.IsActivated == true);
            var date = DateTime.Now;
            var firstweekstart = date.AddDays(-7);
            var firstweekend = date;
            var secondweekstart = firstweekstart.AddDays(-7);
            var secondweekend = firstweekstart;
            var thirdweekstart = secondweekstart.AddDays(-7);
            var thirdweekend = secondweekstart;
            var fourthweekend = thirdweekstart;
            var crmtrack = CH.DB.CrmTracks.Where(w => w.CompanyRelationship.ProjectID == projectid && w.Type == "领用" && w.CompanyRelationship.CoreLVLID == coreid);

            var pickuplist = from s in saleslist
                             select new _PickUpList()
                             {
                                 Sales = s.Name,
                                 FirstWeekCount = crmtrack.Where(w=> w.Owner == s.Name && w.CreatedDate>=firstweekstart && w.CreatedDate<=firstweekend).Count(),
                                 SecondWeekCount = crmtrack.Where(w => w.Owner == s.Name && w.CreatedDate >= secondweekstart && w.CreatedDate < secondweekend).Count(),
                                 ThirdWeekCount = crmtrack.Where(w => w.Owner == s.Name && w.CreatedDate >= thirdweekstart && w.CreatedDate < thirdweekend).Count(),
                                 FourthWeekCount = crmtrack.Where(w => w.Owner == s.Name && w.CreatedDate < fourthweekend).Count(),
                             };
            return View(new GridModel(pickuplist.ToList()));
        }
        public ActionResult GetTemplate(int calltypeid)
        {
            var calltype = CH.GetDataById<LeadCallType>(calltypeid);
            if (string.IsNullOrEmpty(calltype.TemplateName))
                return Json("");
            else
            {
                string[] temp = calltype.TemplateName.Split(';');
                return Json(temp);
            }
        }

        public ActionResult _SalesFilter(int ProjectId)
        {
            var teamleaders = CH.GetDataById<Project>(ProjectId).TeamLeader!=null?CH.GetDataById<Project>(ProjectId).TeamLeader.Trim().Split(new string[] { ";", "；" }, StringSplitOptions.RemoveEmptyEntries):new string[]{""};
            if(CH.DB.Projects.Any(w=>w.ID==ProjectId && (w.Manager.Contains(Employee.CurrentUserName) 
                || w.TeamLeader.Contains(Employee.CurrentUserName)
                || w.ProjectManager.Contains(Employee.CurrentUserName)
                || w.SalesManager.Contains(Employee.CurrentUserName)
                || w.ChinaTL.Contains(Employee.CurrentUserName)
                || w.Market.Contains(Employee.CurrentUserName)
                || w.Product.Contains(Employee.CurrentUserName)
                //|| w.Conference.Contains(Employee.CurrentUserName) 
                ))==false)
            {
                return Json(new List<Member>());
            }
            if (Employee.CurrentRole.Level==100 && teamleaders.Contains(Employee.CurrentUserName))
            {
                var selSales = CH.DB.Members.Where(s => s.ProjectID == ProjectId && s.IsActivated == true).Select(s => s.Name);
                return Json(selSales);
            }
            else if (Employee.CurrentRole.Level > 100 || Employee.CurrentRole.Level == 5 )
            {
                var selSales = CH.DB.Members.Where(s => s.ProjectID == ProjectId && s.IsActivated == true).Select(s => s.Name);
                return Json(selSales);
            }
            else if (Employee.CurrentRole.Level == 80)
            {
                var selSales = CH.DB.Members.Where(s => s.ProjectID == ProjectId && s.IsActivated == true).Select(s => s.Name);
                //只能看到国内销售
                var sales = CH.DB.EmployeeRoles.Where(w => selSales.Contains(w.AccountName) &&( w.Role.Name == "国内销售" || w.Role.Level==80)  ).Select(w => w.AccountName);
                
                return Json(sales);
            }
            else
                return Json(new List<Member>());
        }

        public ActionResult GetQuickAddDeal(int? projectId, int? CRMId)
        {
            projectId = this.TrySetProjectIDForUser(projectId);
            ViewBag.ProjectID = projectId;
            ViewBag.CompanyRelationshipID = CRMId;
            if (CRMId == null)
                ViewBag.DistrictNumberID = 0;
            else if (CH.GetDataById<CompanyRelationship>(CRMId).Company.DistrictNumberID == null)
                ViewBag.DistrictNumberID = 0;
            else
                ViewBag.DistrictNumberID = 1;
            List<AjaxParticipant> pList = new List<AjaxParticipant>();
            Session["pList"] = pList;

            ViewBag.pList = pList;

            this.AddErrorStateIfSalesNoAccessRightToTheProject(projectId);
            return PartialView("QuickAddDeal");
        }

        [HttpPost]
        public ActionResult QuickAddDeal(Deal item, int? projectid)
        {
            ViewBag.ProjectID = projectid;
            ViewBag.CompanyRelationshipID = item.CompanyRelationshipID;
            Participant p = null;
            if (ModelState.IsValid)
            {
                //var CRM = CH.GetDataById<CompanyRelationship>(item.CompanyRelationshipID);
                //var CRMCompany = CH.GetDataById<Company>(CRM.CompanyID);
                //CRMCompany.Address = String.IsNullOrEmpty(Request["Address_CH"]) ? null : Request["Address_CH"].Trim(); 
                //CRMCompany.Address_EN = String.IsNullOrEmpty(Request["Address_EN"]) ? null : Request["Address_EN"].Trim(); 
                //CH.Edit<Company>(CRMCompany);

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
                item.Committer = string.IsNullOrEmpty(item.Committer) ? "" : item.Committer.Trim();
                item.CommitterContect = string.IsNullOrEmpty(item.CommitterContect) ? "" : item.CommitterContect.Trim();
                item.CommitterEmail = string.IsNullOrEmpty(item.CommitterEmail) ? "" : item.CommitterEmail.Trim();
                item.TicketDescription = string.IsNullOrEmpty(item.TicketDescription) ? "" : item.TicketDescription.Trim();
                //item.AbandonReason = string.IsNullOrEmpty(item.AbandonReason) ? "" : item.AbandonReason.Trim();
                item.PaymentDetail = string.IsNullOrEmpty(item.PaymentDetail) ? "" : item.PaymentDetail.Trim();
                item.Sales = item.Sales.Trim();
                item.Deleted = false;
                item.DealType = item.DealType;
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
                            var partType = CH.GetAllData<ParticipantType>().Where(pt => pt.Name == ajaxp.ParticipantTypeName).FirstOrDefault();
                            if ((ajaxp.ID != null) && (ajaxp.ID != 0))
                            {
                                var lead = CH.GetDataById<Lead>(ajaxp.ID);
                                if (lead != null)
                                {
                                    lead.ZIP = ajaxp.ZIP;
                                    lead.Address = ajaxp.Address;
                                    CH.Edit<Lead>(lead);
                                }
                            }
                            p = new Participant();
                            p.Name = string.IsNullOrEmpty(ajaxp.Name) ? "" : ajaxp.Name.Trim();
                            p.Title = string.IsNullOrEmpty(ajaxp.Title) ? "" : ajaxp.Title.Trim();
                            p.Gender = string.IsNullOrEmpty(ajaxp.Gender) ? "" : ajaxp.Gender.Trim();
                            p.Mobile = string.IsNullOrEmpty(ajaxp.Mobile) ? "" : ajaxp.Mobile.Trim();
                            p.Contact = string.IsNullOrEmpty(ajaxp.Contact) ? "" : ajaxp.Contact.Trim();
                            p.Email = string.IsNullOrEmpty(ajaxp.Email) ? "" : ajaxp.Email.Trim();
                            p.ZIP = string.IsNullOrEmpty(ajaxp.ZIP) ? "" : ajaxp.ZIP.Trim();
                            p.Address = string.IsNullOrEmpty(ajaxp.Address) ? "" : ajaxp.Address.Trim();
                            p.ParticipantTypeID = partType.ID;
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
            return Json(new { dealId = item.ID, dealCode = item.DealCode, companyRelationshipId = item.CompanyRelationshipID });
        }
    }
}
