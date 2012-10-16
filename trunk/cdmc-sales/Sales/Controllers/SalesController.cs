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
        public ViewResult CompanyRelationshipLeadCallsIndex(int? companyrelationshipid)
        {
           var data = CH.GetDataById<CompanyRelationship>(companyrelationshipid,"LeadCalls");
           return View("SalesLeadCallsIndex",data.LeadCalls);
        }
        public ViewResult LeadCallsIndex(int? companyrelationshipid, int? leadid)
        {
            var data = CH.GetDataById<CompanyRelationship>(companyrelationshipid, "LeadCalls");
            return View("SalesLeadCallsIndex", data.LeadCalls.FindAll(l=>l.LeadID==leadid));
        }
        #endregion
        /// <summary>
        /// id = companyrelationshipid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ViewResult DisplayCompany(int? id)
        {
            if (CRM_Logical.IsTheSalesAbleToAccessTheCompanyRelationship(id))
                return View(CH.GetDataById<CompanyRelationship>(id).Company);
            else
                return View();
        }

        /// <summary>
        /// id = leadid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ViewResult DisplayLead(int? leadid, int? companyrelationshipid)
        {
            if (CRM_Logical.IsTheSalesAbleToAccessTheCompanyRelationship(companyrelationshipid))
            {
                var lead = CH.GetDataById<Lead>(leadid);
                return View(lead);
            }
            else
                return View();
        }

        /// <summary>
        /// id = leadid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ViewResult EditLead(int? leadid, int? companyrelationshipid)
        {
            if (CRM_Logical.IsTheSalesAbleToAccessTheCompanyRelationship(companyrelationshipid))
            {
                var lead = CH.GetDataById<Lead>(leadid);
                return View(lead);
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
                        var members =i.WhoCallTheCompanyMember();
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

        public ViewResult LeadCall(int? companyrelationshipid, int? leadid)
        {
            ViewBag.CompanyRelationshipID = companyrelationshipid;
            ViewBag.LeadID = leadid;
            return View();
        }

        [HttpPost]
        public ViewResult LeadCall(LeadCall lc)
        {
            ViewBag.CompanyRelationshipID = lc.CompanyRelationship;
            ViewBag.LeadID = lc.LeadID;

            var c = CH.Create<LeadCall>(lc);

            return View();
        }
    }
}
