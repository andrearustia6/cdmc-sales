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
namespace Sales.Controllers
{
    public class CompanyMergeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [GridAction]
        public ActionResult _index(int? projectid,string companyname,bool? deleted)
        {
            var ps = CRM_Logical.GetUserInvolveProject();
            ps = ps.Where(w => w.IsActived == true).ToList();
            IEnumerable<int> idList = ps.Select(o => o.ID);
            var crms =from c in CH.DB.CompanyRelationships.Where(w=>idList.Contains((int)w.ProjectID)) select c;
            if (projectid != null)
                crms = crms.Where(w => w.ProjectID == projectid);
            if(companyname!=null)
                crms = crms.Where(q => q.Company.Name_CH.Contains(companyname) || q.Company.Name_EN.Contains(companyname));
            if (deleted != null)
                crms = crms.Where(w => w.Deleted == deleted);
            var list = from crm in crms
                       select new CompanyInfo
                       {
                           CRMID = crm.ID,
                           CompanyID = crm.CompanyID,
                           CompanyNameCH=crm.Company.Name_CH,
                           CompanyNameEN=crm.Company.Name_EN,
                           Deleted=crm.Deleted,
                       };
            list = list.OrderBy(w => w.CompanyNameEN);
            return View(new GridModel(list.ToList()));
        }
        [HttpPost]
        public ActionResult GetCompanies(List<int> checkedRecords)
        {
            checkedRecords = checkedRecords.OrderBy(w => w).ToList();
            var crms = from c in CH.DB.CompanyRelationships.Where(w => checkedRecords.Contains(w.ID)) select c;
            var companies = from c in crms.OrderBy(w=>w.ID)
                            select new _Company
                            {
                                CRMID = c.ID,
                                Address = c.Company.Address,
                                AreaID = c.Company.AreaID,
                                Business = c.Company.Business,
                                CompanyTypeID = c.Company.CompanyTypeID,
                                Contact = c.Company.Contact,
                                Address_EN = c.Company.Address_EN,
                                Province = c.Company.Province,
                                City = c.Company.City,
                                Scale = c.Company.Scale,
                                AnnualSales = c.Company.AnnualSales,
                                MainProduct = c.Company.MainProduct,
                                MainClient = c.Company.MainClient,
                                Description = c.Company.Description,
                                DistrictNumberID = c.Company.DistrictNumberID,
                                Fax = c.Company.Fax,
                                Name_CH = c.Company.Name_CH,
                                Name_EN = c.Company.Name_EN,
                                WebSite = c.Company.WebSite,
                                ZIP = c.Company.ZIP,
                                Customers = c.Company.Customers,
                                Competitor = c.Company.Competitor,
                            };

            return PartialView("MergeCompany", companies.ToList());
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult MergeCompay(AjaxMergeCompany mergecompany)
        {
            var maincrmid = mergecompany.company.CRMID;
            CompanyRelationship crm = CH.GetDataById<CompanyRelationship>(mergecompany.company.CRMID);
            crm.Company.Address = mergecompany.company.Address;
            crm.Company.AreaID = mergecompany.company.AreaID;
            crm.Company.Business = mergecompany.company.Business;
            crm.Company.CompanyTypeID = mergecompany.company.CompanyTypeID;
            crm.Company.Contact = mergecompany.company.Contact.Trim();
            crm.Company.ModifiedDate = DateTime.Now;
            crm.Company.ModifiedUser = Employee.CurrentUserName;
            crm.Company.Address_EN = mergecompany.company.Address_EN;
            crm.Company.Province = mergecompany.company.Province;
            crm.Company.City = mergecompany.company.City;
            crm.Company.Scale = mergecompany.company.Scale;
            crm.Company.AnnualSales = mergecompany.company.AnnualSales;
            crm.Company.MainProduct = mergecompany.company.MainProduct;
            crm.Company.MainClient = mergecompany.company.MainClient;
            crm.Company.Description = mergecompany.company.Description;
            crm.Company.DistrictNumberID = mergecompany.company.DistrictNumberID;
            crm.Company.Fax = mergecompany.company.Fax;
            crm.Company.Name_CH = mergecompany.company.Name_CH.Trim();
            crm.Company.Name_EN = mergecompany.company.Name_EN.Trim();
            crm.Company.WebSite = mergecompany.company.WebSite;
            crm.Company.ZIP = mergecompany.company.ZIP;
            crm.Company.Customers = mergecompany.company.Customers;
            crm.Company.Competitor = mergecompany.company.Competitor;

            
            List<int> crmids = mergecompany.ids.Where(w => w != maincrmid).ToList();
            List<Category> othercates = new List<Category>();

            //crm.Company.Leads.AddRange(otherleads);
            foreach (int i in crmids)
            {
                CompanyRelationship othercrm = CH.GetDataById<CompanyRelationship>(i);
                othercates.AddRange(othercrm.Categorys);
            }
            crm.Categorys.AddRange(othercates);
            CH.Edit<CompanyRelationship>(crm);

            
            
            
            foreach (int i in crmids)
            {
                CompanyRelationship othercrm = CH.GetDataById<CompanyRelationship>(i);
                othercrm.Deleted = true;
                CH.Edit<CompanyRelationship>(othercrm);
            }
            CompanyMergeTrack _track = new CompanyMergeTrack();
            _track.TableName = "CompanyRelationship";
            _track.OldID = maincrmid.ToString();
            _track.NewID = string.Join(";", crmids.ToArray());
            CH.Create<CompanyMergeTrack>(_track);

            List<int> companyids = CH.DB.CompanyRelationships.Where(w=> crmids.Contains((int)w.ID)).Select(w=>(int)w.CompanyID).ToList();
            List<Lead> otherleads = CH.DB.Leads.Where(w => companyids.Contains((int)w.CompanyID)).ToList();
            List<int> newleadids = new List<int>();
            List<int> oldcallids = new List<int>();
            List<int> newcallids = new List<int>();
            foreach (Lead lead in otherleads)
            {
                lead.Deleted = true;
                CH.Edit<Lead>(lead);

                int leadid = lead.ID;
                lead.CompanyID = crm.Company.ID;
                lead.Deleted = false;
                CH.Create<Lead>(lead);
                newleadids.Add(lead.ID);
                List<LeadCall> othercalls = CH.DB.LeadCalls.Where(w => w.LeadID == leadid ).ToList();
                foreach(LeadCall call in othercalls)
                {
                    oldcallids.Add(call.ID);
                    call.Deleted = true;
                    CH.Edit<LeadCall>(call);
                    call.LeadID=lead.ID;
                    call.CompanyRelationshipID = maincrmid;
                    call.Deleted = false;
                    CH.Create<LeadCall>(call);
                    newcallids.Add(call.ID);
                }
            }
            if (otherleads.Count > 0)
            {
                _track = new CompanyMergeTrack();
                _track.TableName = "Lead";
                _track.OldID = string.Join(";", otherleads.Select(w => w.ID).ToArray());
                _track.NewID = string.Join(";", newleadids.ToArray());
                CH.Create<CompanyMergeTrack>(_track);
            }
            if (oldcallids.Count > 0)
            {
                _track = new CompanyMergeTrack();
                _track.TableName = "LeadCall";
                _track.OldID = string.Join(";", oldcallids.ToArray());
                _track.NewID = string.Join(";", newcallids.ToArray());
                CH.Create<CompanyMergeTrack>(_track);
            }
            List<Deal> otherdeals = CH.DB.Deals.Where(w => crmids.Contains((int)w.CompanyRelationshipID)).ToList();
            List<int> newdealids = new List<int>();
            foreach (Deal deal in otherdeals)
            {
                deal.Deleted = true;
                CH.Edit<Deal>(deal);

                deal.CompanyRelationshipID = maincrmid;
                deal.Deleted = false;
                CH.Create<Deal>(deal);
                newdealids.Add(deal.ID);
            }
            if (otherdeals.Count > 0)
            {
                _track = new CompanyMergeTrack();
                _track.TableName = "Deal";
                _track.OldID = string.Join(";", otherdeals.Select(w => w.ID).ToArray());
                _track.NewID = string.Join(";", newdealids.ToArray());
                CH.Create<CompanyMergeTrack>(_track);
            }
            List<Comment> othercomments = CH.DB.Comments.Where(w => crmids.Contains((int)w.CompanyRelationshipID)).ToList();
            List<int> newcommentids = new List<int>();
            foreach (Comment comment in othercomments)
            {
                comment.Deleted = true;
                CH.Edit<Comment>(comment);

                comment.CompanyRelationshipID = maincrmid;
                comment.Deleted = false;
                CH.Create<Comment>(comment);
                newcommentids.Add(comment.ID);
            }
            if (othercomments.Count > 0)
            {
                _track = new CompanyMergeTrack();
                _track.TableName = "Comment";
                _track.OldID = string.Join(";", othercomments.Select(w => w.ID).ToArray());
                _track.NewID = string.Join(";", newcommentids.ToArray());
                CH.Create<CompanyMergeTrack>(_track);
            }

            return Json(new { crmid = crm.ID });
        }
        [HttpPost]
        public ActionResult GetLeads(int crmid)
        {
            return PartialView("LeadCheck", new List<AjaxLead>());
        }
        [HttpPost]
        public ActionResult GetLeadsJson(int crmid)
        {
            CompanyRelationship crm = CH.GetDataById<CompanyRelationship>(crmid);
            var leads = from c in crm.Company.Leads.Where(w=>w.Deleted==false)
                        select new AjaxLead
                        {
                            LeadID = c.ID,
                            LeadNameCH = c.Name_CH,
                            LeadNameEN = c.Name_EN,
                            LeadTitle = c.Title,
                            LeadMobile = c.Mobile,
                            LeadContact = c.Contact
                        };
            return Json(leads.ToList());
        }

        [HttpPost]
        public ActionResult GetLeadsByids(List<int> checkedRecords)
        {
            checkedRecords = checkedRecords.OrderBy(w => w).ToList();
            var leads = from c in CH.DB.Leads.Where(w => checkedRecords.Contains(w.ID)) select c;
            var ajaxleads = from c in leads.OrderBy(w=>w.ID)
                            select new AjaxViewLead
                            {
                                LeadId = c.ID,
                                Name_CN = c.Name_CH,
                                Name_EN = c.Name_EN,
                                Title = c.Title,
                                Address = c.Address,
                                Birthday = c.Birthday,
                                Telephone = c.Contact,
                                Department = c.Department,
                                Desc = c.Description,
                                WorkingEmail = c.EMail,
                                Fax = c.Fax,
                                Gender = c.Gender,
                                CellPhone = c.Mobile,
                                WeiBo = c.WeiBo,
                                WeiXin = c.WeiXin,
                                LinkIn = c.LinkIn,
                                FaceBook = c.FaceBook,
                                Blog = c.Blog,
                                DistrictNumberId = c.DistrictNumberID,
                                PersonelEmail = c.PersonalEmailAddress,
                                PersonalPhone = c.PersonalPhone,
                                PersonalCellPhone = c.PersonalCellPhone,
                                PersonalFax = c.PersonalFax,
                                Comment = c.Comment,
                                QQ = c.QQ,
                                Twitter = c.Twitter,
                                Branch = c.Branch,
                                Zip = c.ZIP,
                                LeadRoles=c.LeadRoles
                            };

            return PartialView("MergeLead", ajaxleads.ToList());
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult MergeLead(AjaxMergeLead mergelead)
        {
            int mainleadid = mergelead.lead.LeadId;
            Lead lead = CH.GetDataById<Lead>(mainleadid);
            lead.Name_CH = mergelead.lead.Name_CN.Trim();
            lead.Name_EN = mergelead.lead.Name_EN.Trim();
            lead.Gender = mergelead.lead.Gender;
            lead.Title = mergelead.lead.Title.Trim();
            lead.Address = mergelead.lead.Address.Trim();
            lead.Birthday = mergelead.lead.Birthday;
            lead.Contact = mergelead.lead.Telephone.Trim();
            lead.Department = mergelead.lead.Department;
            lead.Description = mergelead.lead.Desc;
            lead.EMail = mergelead.lead.WorkingEmail;
            lead.Fax = mergelead.lead.Fax;
            lead.Gender = mergelead.lead.Gender;
            lead.Mobile = mergelead.lead.CellPhone.Trim();
            lead.WeiBo = mergelead.lead.WeiBo;
            lead.WeiXin = mergelead.lead.WeiXin;
            lead.LinkIn = mergelead.lead.LinkIn;
            lead.FaceBook = mergelead.lead.FaceBook;
            lead.Blog = mergelead.lead.Blog;
            lead.MarkForDelete = false;
            lead.DistrictNumberID = mergelead.lead.DistrictNumberId;
            lead.PersonalEmailAddress = mergelead.lead.PersonelEmail;
            lead.PersonalPhone = mergelead.lead.PersonalPhone;
            lead.PersonalCellPhone = mergelead.lead.PersonalCellPhone;
            lead.PersonalFax = mergelead.lead.PersonalFax;
            lead.Comment = mergelead.lead.Comment;
            lead.QQ = mergelead.lead.QQ;
            lead.Twitter = mergelead.lead.Twitter;
            lead.Branch = mergelead.lead.Branch;
            lead.ZIP = mergelead.lead.Zip;
            if (mergelead.lead.leadRole != null)
            {
                foreach (string leadrole in mergelead.lead.leadRole)
                {
                    lead.LeadRoles += leadrole + ";";
                }
            }


            List<int> otherleads = mergelead.ids.Where(w => w != mainleadid).ToList();
            CH.Edit<Lead>(lead);

            //foreach (int i in leadids)
            //{
            //    Lead otherlead = CH.GetDataById<Lead>(i);
            //    otherlead.Deleted = true;
            //    CH.Edit<Lead>(otherlead);
            //}
            //CompanyMergeTrack _track = new CompanyMergeTrack();
            //_track.TableName = "Lead";
            //_track.OldID = mainleadid.ToString();
            //_track.NewID = string.Join(";", leadids.ToArray());
            //CH.Create<CompanyMergeTrack>(_track);


            List<int> oldcallids = new List<int>();
            List<int> newcallids = new List<int>();
            foreach (int i in otherleads)
            {
                Lead otherlead = CH.GetDataById<Lead>(i);
                otherlead.Deleted = true;
                CH.Edit<Lead>(otherlead);

                List<LeadCall> othercalls = CH.DB.LeadCalls.Where(w => w.LeadID == otherlead.ID).ToList();
                foreach (LeadCall call in othercalls)
                {
                    oldcallids.Add(call.ID);
                    call.Deleted = true;
                    CH.Edit<LeadCall>(call);
                    call.LeadID = mainleadid;
                    call.Deleted = false;
                    CH.Create<LeadCall>(call);
                    newcallids.Add(call.ID);
                }
            }
            CompanyMergeTrack _track = new CompanyMergeTrack();
            if (otherleads.Count > 0)
            {
                _track.TableName = "Lead";
                _track.OldID = mainleadid.ToString();
                _track.NewID = string.Join(";", otherleads.ToArray());
                CH.Create<CompanyMergeTrack>(_track);
            }
            if (oldcallids.Count > 0)
            {
                _track = new CompanyMergeTrack();
                _track.TableName = "LeadCall";
                _track.OldID = string.Join(";", oldcallids.ToArray());
                _track.NewID = string.Join(";", newcallids.ToArray());
                CH.Create<CompanyMergeTrack>(_track);
            }

            return Json(new { leadid = mainleadid });
        }
        public ActionResult DisplayLeads(int crmid)
        {
            CompanyRelationship crm = CH.GetDataById<CompanyRelationship>(crmid);
            return PartialView("Lead", crm.Company.Leads);
        }
        public ActionResult DisplayLeadCalls(int crmid)
        {
            CompanyRelationship crm = CH.GetDataById<CompanyRelationship>(crmid);
            return PartialView("LeadCall", crm.LeadCalls);
        }
        public ActionResult DisplayDeals(int crmid)
        {
            CompanyRelationship crm = CH.GetDataById<CompanyRelationship>(crmid);
            var ds = from d in crm.Deals
                     select new AjaxViewDeal
                     {
                         CompanyNameEN = d.CompanyRelationship.Company.Name_EN,
                         CompanyNameCH = d.CompanyRelationship.Company.Name_CH,
                         DealCode = d.DealCode,
                         Abandoned = d.Abandoned,
                         AbandonReason = d.AbandonReason,
                         ActualPaymentDate = d.ActualPaymentDate,
                         Committer = d.Committer,
                         CommitterContect = d.Committer,
                         CommitterEmail = d.CommitterEmail,
                         ExpectedPaymentDate = d.ExpectedPaymentDate,
                         ID = d.ID,
                         Income = d.Income,
                         IsClosed = d.IsClosed,
                         PackageNameCH = d.Package.Name_CH,
                         PackageNameEN = d.Package.Name_EN,
                         Payment = d.Payment,
                         Currency = d.Currencytype.Name,
                         PaymentDetail = d.PaymentDetail,
                         Sales = d.Sales,
                         ProjectCode = d.Project.ProjectCode,
                         SignDate = d.SignDate,
                         TicketDescription = d.TicketDescription,
                         IsConfirm = (d.IsConfirm == true ? "是" : "否")
                     };
            return PartialView("Deal", ds.ToList());
        }
        public ActionResult DisplayCategories(int crmid)
        {
            CompanyRelationship crm = CH.GetDataById<CompanyRelationship>(crmid);
            return PartialView("Category", crm.Categorys);
        }
        public ActionResult DisplayComments(int crmid)
        {
            CompanyRelationship crm = CH.GetDataById<CompanyRelationship>(crmid);
            return PartialView("Comment", crm.Comments);
        }
        
    }
}
