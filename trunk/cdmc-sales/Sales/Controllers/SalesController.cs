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
   
        /// <summary>
        /// id = companyrelationshipid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ViewResult DisplayCompany(int? id)
        {
            var cr = CH.GetDataById<CompanyRelationship>(id,"Members");
            if (cr.Members.Any(m => m.Name == User.Identity.Name))
                return View(cr.Company);
            else
                return View();
        }

        /// <summary>
        /// id = leadid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ViewResult DisplayLead(int? id,int? companyrelationshipid)
        {
            var cr = CH.GetDataById<CompanyRelationship>(companyrelationshipid, "Members");
            if (cr.Members.Any(m => m.Name == User.Identity.Name))
            {
                var lead = CH.GetDataById<Lead>(id);
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
            var project = CH.GetDataById<Project>(projectid,"Members");
            if (project != null)
            {
                var cs = CH.GetAllData<CompanyRelationship>(c => c.ProjectID == projectid);

                var member = project.Members.FirstOrDefault(m => m.Name == User.Identity.Name);
                if (member != null && member.CharactersSet != null)
                {

                    var data = new List<CompanyRelationship>();
                    cs.ForEach(i =>
                    {
                        var members = CRM_Logical.GetMemberWhoCallTheCompany((int)i.CompanyID, (int)projectid);
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
