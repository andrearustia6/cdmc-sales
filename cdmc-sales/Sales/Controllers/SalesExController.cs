using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utl;
using Model;
using Telerik.Web.Mvc;

namespace Sales.Controllers
{
    public class SalesExController : Controller
    {
        //
        // GET: /SalesEx/
        public ActionResult Index()
        {
            return View();
        }

        [GridAction]
        public ActionResult _Index()
        {
            var user = Employee.CurrentUserName;
            var query = from l in CH.DB.LeadCalls where l.Member.Name == Employee.CurrentUserName
                        group l by new { l.LeadID, l.Lead.CompanyID,l.CompanyRelationshipID,l.CompanyRelationship.ProjectID } into grp
                        from c in CH.DB.Companys where c.ID == grp.Key.CompanyID
                        from p in CH.DB.Projects where p.ID == grp.Key.ProjectID
                        from crm in CH.DB.CompanyRelationships where crm.ID == grp.Key.CompanyRelationshipID
                        from l in CH.DB.Leads where l.ID == grp.Key.LeadID
                        select new AjaxCRM 
                        {
                            ProjectID = grp.Key.ProjectID.Value,
                            LeadID =  grp.Key.LeadID.Value,
                            CompanyID = grp.Key.LeadID.Value,
                            CRMID = grp.Key.CompanyRelationshipID.Value,

                            ProjectName = p.Name_CH,

                            Progress = crm.Progress,
                            HasBlowed = crm.HasBlowed,

                            CompanyNameCH = c.Name_CH,
                            CompanyNameEN = c.Name_EN,
                            CompanyContact = c.Contact,
                            CompanyFax = c.Fax,
                            CompanyDistinct = c.DistrictNumber,
                            
                            LeadNameCH = l.Name_CH,
                            LeadNameEN = l.Name_EN,
                            LeadContact = l.Contact,
                            LeadDistinct = l.DistrictNumber,
                            LeadEmail = l.EMail,
                            LeadMobile = l.Mobile,
                            LeadTitle = l.Title,
                            LeadFax = l.Fax,

                            
                            AjaxCalls = (from call in grp select new AjaxCall{
                                 LeadID = grp.Key.LeadID.Value,
                                  CompanyID = grp.Key.LeadID.Value,
                                  LeadCallID = call.ID,
                                  CallType = call.LeadCallType.Name,
                                  Result = call.Result,
                                  CallDate = call.CallDate,
                                  Caller = call.Member.Name,
                                  CallBackDate = call.CallBackDate
                                     
                            })
                        };

            return View(new GridModel<AjaxCRM> { Data = query });
        }

    }
}
