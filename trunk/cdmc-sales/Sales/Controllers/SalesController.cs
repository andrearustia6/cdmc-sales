using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utl;
using Entity;
using BLL;

namespace Sales.Controllers
{
    [SalesRequired]
    public class SalesController : Controller
    {
        #region Leadcall
        public ViewResult CompanyRelationshipLeadCallsIndex(int? crid)
        {
            var data = new List<LeadCall>();
            this.AddErrorStateIfSalesNoAccessRightToTheCRM(crid);

            if (ModelState.IsValid)
            {
                data = CH.GetDataById<CompanyRelationship>(crid, "LeadCalls").LeadCalls;
            }
            return View("SalesLeadCallsIndex", data);
        }

        public ViewResult LeadCallsIndex(int? crid, int? leadid)
        {
            var data = new List<LeadCall>();
            this.AddErrorStateIfSalesNoAccessRightToTheCRM(crid);

            if (ModelState.IsValid)
            {
                data = CH.GetDataById<CompanyRelationship>(crid, "LeadCalls").LeadCalls.FindAll(l => l.LeadID == leadid);
            }
            return View("SalesLeadCallsIndex", data);
        }

        public ActionResult AddLeadCall(int? crid, int? leadid)
        {
            var m = CH.GetDataById<Project>(CH.GetDataById<CompanyRelationship>(crid).ProjectID, "Members").GetProjectMemberByName();
            if (m != null)
                ViewBag.MemberID = m.ID;

            ViewBag.CompanyRelationshipID = crid;
            return View();
        }

        [HttpPost]
        public ActionResult AddLeadCall(LeadCall leadcall, int? crid)
        {

            if (ModelState.IsValid)
            {
                CH.Create<LeadCall>(leadcall);
                return RedirectToAction("CompanyRelationshipLeadCallsIndex", new { crid = crid });
            }
            else
            {
                ViewBag.CompanyRelationshipID = crid;
                return View(leadcall);
            }
        }
        #endregion



        /// <summary>
        /// id = leadid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ViewResult DisplayLead(int? leadid, int? crid)
        {
            this.AddErrorStateIfSalesNoAccessRightToTheCRM(crid);

            if (ModelState.IsValid)
            {
                var lead = CH.GetDataById<Lead>(leadid);
                return View(lead);
            }
            else
                return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ViewResult EditLead(int? leadid, int? crid)
        {
            this.AddErrorStateIfSalesNoAccessRightToTheCRM(crid);
            if (ModelState.IsValid)
            {
                var lead = CH.GetDataById<Lead>(leadid);
                return View(lead);
            }
            else
                return View();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddLead(Lead lead, int? projectid)
        {
            this.AddErrorIfAllNamesEmpty(lead);
            if (ModelState.IsValid)
            {
                CH.Create<Lead>(lead);
                return RedirectToAction("CompanyRelationshipIndex", new { projectid = projectid });
            }
            else
            {
                ViewBag.CompanyID = lead.CompanyID;
                ViewBag.ProjectID = projectid;
                return View(lead);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AddLead(int? crid)
        {
            this.AddErrorStateIfSalesNoAccessRightToTheCRM(crid);
            var cr = CH.GetDataById<CompanyRelationship>(crid);
            if (cr != null)
            {
                ViewBag.CompanyID = cr.CompanyID;
                ViewBag.ProjectID = cr.ProjectID;
            }
            if (ModelState.IsValid)
                return View();
            else
                return RedirectToAction("CompanyRelationshipIndex", "Sales", new { projectid = cr.ProjectID });
        }

        #region company
        public ActionResult AddCompany(int? projectid = null)
        {
            if (projectid == null) return RedirectToAction("CompanyRelationshipIndex", "Sales");

            this.AddErrorStateIfSalesNoAccessRightToTheProject(projectid);

            if (ModelState.IsValid)
            {
                ViewBag.ProjectID = projectid;
                return View();
            }
            else
                return RedirectToAction("CompanyRelationshipIndex", "Sales", new { projectid = projectid });
        }

        [HttpPost]
        public ActionResult AddCompany(Company item,int? projectid, int[] checkedCategorys)
        {
            if (projectid == null) return RedirectToAction("CompanyRelationshipIndex", "Sales");

            ViewBag.ProjectID = projectid;

            this.AddErrorStateIfSalesNoAccessRightToTheProject(projectid);
            this.AddAddErrorStateIfOneOfNameExist<Company>(item.Name_EN,item.Name_CH);
            this.AddErrorIfAllNamesEmpty(item);
            if (ModelState.IsValid)
            {
                List<Category> lc = new List<Category>();
                if (checkedCategorys != null)
                {
                    lc = CH.GetAllData<Category>(i => checkedCategorys.Contains(i.ID));
                }

                var p = CH.GetDataById<Project>(projectid,"Members");
                CH.Create<Company>(item);
                var ms = new List<Member>();
                ms.Add(p.GetMemberInProjectByName(User.Identity.Name));
                var cr = new CompanyRelationship() { CompanyID = item.ID, ProjectID = projectid, Importancy = 1, Members = ms, Categorys = lc };
                CH.Create<CompanyRelationship>(cr);
                return RedirectToAction("CompanyRelationshipIndex", "Sales", new { projectid = projectid });
                
            }
            else
                return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ViewResult EditCompany(int? crid)
        {
            this.AddErrorStateIfSalesNoAccessRightToTheCRM(crid);
            if (ModelState.IsValid)
            {
                var cr = CH.GetDataById<CompanyRelationship>(crid);
                ViewBag.CompanyRelationshipID = cr.ID;
                ViewBag.ProjectID = cr.ProjectID;
                return View(cr.Company);
            }
            else
                return View();
        }

        /// <summary>
        /// id = companyrelationshipid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ViewResult DisplayCompany(int? crid)
        {
            this.AddErrorStateIfSalesNoAccessRightToTheCRM(crid);

            if (ModelState.IsValid)
                return View(CH.GetDataById<CompanyRelationship>(crid).Company);
            else
                return View();
        }

        [HttpPost]
        public ActionResult EditCompany(Company item, int? crid, int? projectid, int[] checkedCategorys)
        {
            this.AddErrorIfAllNamesEmpty(item);
           
            
            if (ModelState.IsValid)
            {
                var cr = CH.GetDataById<CompanyRelationship>(crid, "Categorys");
                List<Category> lc = new List<Category>();
                if (checkedCategorys != null)
                {
                    cr.Categorys.Clear();
                    lc = CH.GetAllData<Category>(i => checkedCategorys.Contains(i.ID));
                    cr.Categorys.AddRange(lc);
                    CH.Edit<CompanyRelationship>(cr);
                }

                CH.Edit<Company>(item);
                return RedirectToAction("CompanyRelationshipIndex", "Sales", new { projectid = projectid });
            }

            ViewBag.ProjectID = projectid;
            return View();
        }
        #endregion

        public ViewResult MyDealIndex(int? porjectid)
        {
            return View(CH.GetAllData<Deal>(d => d.Sales == User.Identity.Name && d.CompanyRelationship.ProjectID == porjectid));
        }

        /// <summary>
        /// 根据分配显示sales需要拨打的公司
        /// </summary>
        /// <returns></return
        public ViewResult CompanyRelationshipIndex(int? projectid)
        {
            ViewBag.ProjectID = projectid;
            var project = CH.GetDataById<Project>(projectid, "Members");
            if (project != null)
            {
                var cs = CH.GetAllData<CompanyRelationship>(c => c.ProjectID == projectid);

                var member = project.Members.FirstOrDefault(m => m.Name == User.Identity.Name);
                if (member != null && member.CharactersSet != null)
                {

                    var data = new List<CompanyRelationship>();
                    cs.ForEach(i =>
                    {
                        var members = i.WhoCallTheCompanyMember();
                        if (members.Any(m => m.Name == User.Identity.Name))
                        {
                            data.Add(i);
                        }
                    });

                    return View(data);
                }
            }
            return View();
        }
    }
}
