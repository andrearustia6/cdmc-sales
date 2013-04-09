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
            var d = new AjaxCrmTypedList();
            d.AllCRMs = GetCrmDataQuery();
            d.CustomCrmGroups = GetcustomCrmGroupDataQuery();
            return View(d);
        }

        private IQueryable<AjaxGroupedCRM> GetcustomCrmGroupDataQuery()
        {
            var user = Employee.CurrentUserName;
            var data = from f in CH.DB.UserFavorsCrmGroups
                       where f.UserName == user
                       select new AjaxGroupedCRM()
                       {
                           UserName = f.UserName,
                           DisplayName = f.DisplayName,
                           GroupedCRMs = (from u in CH.DB.UserFavorsCRMs
                                          from c in CH.DB.CompanyRelationships
                                          from m in c.Members
                                          where m.Name == user
                                          && c.ID == u.CompanyRelationshipID.Value
                                          && f.UserFavorsCRMs.Select(s=>s.ID).Contains(u.ID)
                                          select new AjaxCRM
                                          {
                                              CompanyNameEN = c.Company.Name_EN,
                                              CompanyNameCH = c.Company.Name_CH,
                                              CompanyContact = c.Company.Contact,
                                              Progress = c.Progress,
                                              CompanyFax = c.Company.Fax,
                                              CompanyCategories = c.Categorys,
                                              CompanyDistinct = c.Company.DistrictNumber,
                                              CompanyCreateDate = c.Company.CreatedDate,
                                              CRMID = c.ID,
                                              AjaxLeads = (from l in c.Company.Leads
                                                           select new AjaxLead
                                                           {
                                                               LeadNameCH = l.Name_CH,
                                                               LeadNameEN = l.Name_EN,
                                                               LeadContact = l.Contact,
                                                               LeadDistinct = l.DistrictNumber,
                                                               LeadEmail = l.EMail,
                                                               LeadMobile = l.Mobile,
                                                               LeadTitle = l.Title,
                                                               LeadFax = l.Fax,
                                                               CRMID = c.ID,
                                                               LeadID = l.ID,
                                                               LeadCreateDate = l.CreatedDate,
                                                               AjaxCalls = (from call in c.LeadCalls
                                                                            select new AjaxCall
                                                                            {
                                                                                CallDate = call.CallDate,
                                                                                CallBackDate = call.CallBackDate,
                                                                                CallType = call.LeadCallType.Name,
                                                                                Caller = call.Member.Name,
                                                                                LeadCallTypeCode = call.LeadCallType.Code

                                                                            })
                                                           })

                                          }
                                          )

                       };
            return data;
        }

        IQueryable<AjaxCRM> GetCrmDataQuery()
        {
            var user = Employee.CurrentUserName;
            var data = from c in CH.DB.CompanyRelationships
                       from m in c.Members

                       where m.Name == user && m.CompanyRelationships.Select(s => s.ID).Contains(c.ID)
                       select new AjaxCRM
                       {
                           CompanyNameEN = c.Company.Name_EN,
                           CompanyNameCH = c.Company.Name_CH,
                           CompanyContact = c.Company.Contact,
                           Progress = c.Progress,
                           CompanyFax = c.Company.Fax,
                           CompanyCategories = c.Categorys,
                           CompanyDistinct = c.Company.DistrictNumber,
                           CompanyCreateDate = c.Company.CreatedDate,
                           CRMID = c.ID,
                           AjaxLeads = (from l in c.Company.Leads
                                        select new AjaxLead
                                        {
                                            LeadNameCH = l.Name_CH,
                                            LeadNameEN = l.Name_EN,
                                            LeadContact = l.Contact,
                                            LeadDistinct = l.DistrictNumber,
                                            LeadEmail = l.EMail,
                                            LeadMobile = l.Mobile,
                                            LeadTitle = l.Title,
                                            LeadFax = l.Fax,
                                            CRMID = c.ID,
                                            LeadID = l.ID,
                                            LeadCreateDate = l.CreatedDate,
                                            AjaxCalls = (from call in c.LeadCalls
                                                         select new AjaxCall
                                                         {
                                                             CallDate = call.CallDate,
                                                             CallBackDate = call.CallBackDate,
                                                             CallType = call.LeadCallType.Name,
                                                             Caller = call.Member.Name,
                                                             LeadCallTypeCode = call.LeadCallType.Code
                                                              
                                                         })
                                        })

                       };

            return data;
        }
        public PartialViewResult JsonSelectedList(string indexs)
        {
            var ids = indexs.Split(new string[]{","}, StringSplitOptions.RemoveEmptyEntries);
            var crmid = int.Parse(ids[0]);
            var leadid = 0;
            if (ids.Count() > 1)
            {
                leadid = int.Parse(ids[1]);
            }
            var data = GetCrmDataQuery().FirstOrDefault(f=>f.CRMID == crmid);

            if (leadid != 0 && data!=null)
            {
                data.AjaxLeads = data.AjaxLeads.Where(w => w.LeadID == leadid);
            }
          
            return PartialView(@"~\views\shared\salesexitem.cshtml", data);
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