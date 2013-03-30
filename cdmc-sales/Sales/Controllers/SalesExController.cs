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
            var query = from c in CH.DB.CompanyRelationships
                        from com in CH.DB.Companys
                        from l in CH.DB.Leads
                        where c.CompanyID == com.ID && com.ID == l.CompanyID && c.Members.Select(s=>s.Name).Contains(user)
                         select new AjaxCRM 
                        {
                            ProjectID = c.ProjectID.Value,
                            LeadID =  l.ID,
                            CompanyID = com.ID,
                            CRMID = c.ID,

                            ProjectName = c.Project.Name_CH,

                            Progress = c.Progress,
                            HasBlowed = c.HasBlowed,
                            CrmCreateDate = c.CreatedDate,
                            CompanyCategories = c.Categorys,

                            CompanyNameCH = com.Name_CH,
                            CompanyNameEN = com.Name_EN,
                            CompanyContact = com.Contact,
                            CompanyFax = com.Fax,
                            CompanyDistinct = com.DistrictNumber,
                            CompanyCreateDate = com.CreatedDate,
                            LeadsCount = com.Leads.Count,
                            //LeadsName = com.Leads.Select(s=>s.Name),

                            LeadNameCH = l.Name_CH,
                            LeadNameEN = l.Name_EN,
                            LeadContact = l.Contact,
                            LeadDistinct = l.DistrictNumber,
                            LeadEmail = l.EMail,
                            LeadMobile = l.Mobile,
                            LeadTitle = l.Title,
                            LeadFax = l.Fax,
                            LeadCreateDate = l.CreatedDate,
                            CallsCount = c.LeadCalls.Where(f => f.LeadID == l.ID).Count(),
                            Calls= c.LeadCalls
                        };
           

            return View(new GridModel<AjaxCRM> { Data = query.OrderByDescending(o => o.CrmCreateDate) });
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
                            Editable = user==call.Member.Name?true:false
                             
                        };

            return View(new GridModel<AjaxCall> { Data = calls.OrderByDescending(o=>o.CallDate) });
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