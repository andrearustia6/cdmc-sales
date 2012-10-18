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
            this.AddErrorStateIfSalesNoAccessRight(crid);

            if (ModelState.IsValid)
            {
                data = CH.GetDataById<CompanyRelationship>(crid, "LeadCalls").LeadCalls;
            }
            return View("SalesLeadCallsIndex", data);
        }

        public ViewResult LeadCallsIndex(int? crid, int? leadid)
        {
            var data = new List<LeadCall>();
            this.AddErrorStateIfSalesNoAccessRight(crid);

            if (ModelState.IsValid)
            {
                data = CH.GetDataById<CompanyRelationship>(crid, "LeadCalls").LeadCalls.FindAll(l => l.LeadID == leadid);
            }
            return View("SalesLeadCallsIndex", data);
        }

        public ActionResult AddLeadCall(int? crid,int? leadid)
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
        /// id = companyrelationshipid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ViewResult DisplayCompany(int? crid)
        {
            this.AddErrorStateIfSalesNoAccessRight(crid);

            if (ModelState.IsValid)
                return View(CH.GetDataById<CompanyRelationship>(crid).Company);
            else
                return View();
        }

        /// <summary>
        /// id = leadid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ViewResult DisplayLead(int? leadid, int? crid)
        {
            this.AddErrorStateIfSalesNoAccessRight(crid);

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
            this.AddErrorStateIfSalesNoAccessRight(crid);
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
        public ViewResult AddLead(int? crid)
        {
            this.AddErrorStateIfSalesNoAccessRight(crid);
            if (ModelState.IsValid)
            {
                var cr = CH.GetDataById<CompanyRelationship>(crid);
                if (cr != null)
                {
                    ViewBag.CompanyID = cr.CompanyID;
                    ViewBag.ProjectID = cr.ProjectID;
                }
                return View();
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
            this.AddErrorIfOneOfNamesEmpty(lead);
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
        public ViewResult EditCompany(int? crid)
        {
            this.AddErrorStateIfSalesNoAccessRight(crid);
            if (ModelState.IsValid)
            {
                var cr = CH.GetDataById<CompanyRelationship>(crid);
                ViewBag.ProjectID = cr.ProjectID;
                return View(cr.Company);
            }
            else
                return View();
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
