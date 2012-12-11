using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utl;
using Entity;
using BLL;
using Model;

namespace Sales.Controllers
{
    [SalesRequired]
    public class SalesController : Controller
    {
        #region Leadcall
        /// <summary>
        /// 查看客户公司的 销售记录
        /// </summary>
        /// <param name="crid"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 查看个人的 销售记录
        /// </summary>
        /// <param name="crid"></param>
        /// <param name="leadid"></param>
        /// <returns></returns>
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
            //传拨打人到页面
            var m = CH.GetDataById<Project>(CH.GetDataById<CompanyRelationship>(crid).ProjectID, "Members").GetProjectMemberByName();
            if (m != null)
                ViewBag.MemberID = m.ID;

            ViewBag.CompanyRelationshipID = crid;
            return View();
        }

        [HttpPost]
        public ActionResult AddLeadCall(LeadCall leadcall, int? crid)
        {
            var cr = CH.GetDataById<CompanyRelationship>(leadcall.CompanyRelationshipID);
            ViewBag.CompanyRelationshipID = crid;
            ViewBag.ProjectID = cr.ProjectID;
            if (ModelState.IsValid)
            {
                leadcall.ProjectID = cr.ProjectID;
                CH.Create<LeadCall>(leadcall);
                return RedirectToAction("CompanyRelationshipLeadCallsIndex", new { crid = crid });
            }
            else
            {
                ViewBag.CompanyRelationshipID = crid;
                return View(leadcall);
            }
        }

        public ActionResult DisplayLeadCall(int? crid, int? leadcallid)
        {
            this.AddErrorStateIfSalesNoAccessRightToTheCRM(crid);

            ViewBag.CompanyRelationshipID = crid;
            var lc = CH.GetDataById<LeadCall>(leadcallid);
            return View(lc);
        }

        public ActionResult DeleteLeadCall(int? crid, int? leadcallid)
        {
            var lc = CH.GetDataById<LeadCall>(leadcallid);
            ViewBag.CompanyRelationshipID = crid;
            this.AddErrorStateIfCallerIsNotTheLoginUser(lc);
            if (ModelState.IsValid)
                return View(lc);
            else
                return RedirectToAction("CompanyRelationshipLeadCallsIndex", new { crid = crid });
        }

        [HttpPost, ActionName("DeleteLeadCall")]
        public ActionResult DeleteLeadCallConfirmed( int? leadcallid,int? crid)
        {
            CH.Delete<LeadCall>(leadcallid);
            return RedirectToAction("CompanyRelationshipLeadCallsIndex", new { crid = crid });
           
        }

        public ActionResult EditLeadCall(int? crid, int? leadcallid)
        {
            this.AddErrorStateIfSalesNoAccessRightToTheCRM(crid);

            ViewBag.CompanyRelationshipID = crid;
            var lc = CH.GetDataById<LeadCall>(leadcallid);
            return View(lc);
        }

        [HttpPost]
        public ActionResult EditLeadCall(LeadCall leadcall, int? crid)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<LeadCall>(leadcall);
                return RedirectToAction("CompanyRelationshipLeadCallsIndex", new { crid = crid });

            }
            else
            {
                ViewBag.CompanyRelationshipID = crid;
                return View(leadcall);
            }
        }

        #endregion

        #region deal
        [HttpPost]
        public ActionResult AddDeal(Deal item, int? projectid)
        {
            ViewBag.ProjectID = projectid;
            ViewBag.CompanyRelationshipID = item.CompanyRelationshipID;

            if (ModelState.IsValid)
            {
                item.Sales = Employee.GetCurrentUserName();
                CH.Create<Deal>(item);
                return RedirectToAction("MyDealIndex", "Sales", new { projectid = projectid });
            }
            return View(item);
        }

        public ActionResult AddDeal(int? projectid)
        {
            projectid = this.TrySetProjectIDForUser(projectid);
            ViewBag.ProjectID = projectid;


            this.AddErrorStateIfSalesNoAccessRightToTheProject(projectid);

           
            if(ModelState.IsValid)
               return View();
            else
                return RedirectToAction("MyDealIndex");
        }

        public ActionResult EditDeal(int? id)
        {
            
            var item = CH.GetDataById<Deal>(id);
            this.AddErrorStateIfSalesNoAccessRightToTheCRM(item.CompanyRelationshipID);
            ViewBag.ProjectID = item.CompanyRelationship.ProjectID;
            if (ModelState.IsValid)
                return View(item);
            else
                return RedirectToAction("MyDealIndex");
        }

        [HttpPost]
        public ActionResult EditDeal(Deal item,int? projectid)
        {
            ViewBag.ProjectID = projectid;
            ViewBag.CompanyRelationshipID = item.CompanyRelationshipID;
            if (ModelState.IsValid)
            {
                CH.Edit<Deal>(item);
                return RedirectToAction("MyDealIndex", "Sales", new { projectid = projectid });
            }
            return View(item);
        }

        public ViewResult MyDealIndex(int? projectid)
        {
            projectid = this.TrySetProjectIDForUser(projectid);
            ViewBag.ProjectID = projectid;
            var data = CH.GetAllData<Deal>();
            ViewBag.ProjectID = projectid;
            return View(data.FindAll(d => d.Sales == Employee.GetCurrentUserName() && d.CompanyRelationship.ProjectID == projectid).OrderByDescending(m => m.CreatedDate).ToList());
        }

        public ViewResult DisplayDeal(int? id, int? projectid)
        {
            var deal =  CH.GetDataById<Deal>(id);
            this.AddErrorStateIfSalesNoAccessRightToTheCRM(deal.CompanyRelationshipID);
            ViewBag.CompanyRelationshipID = deal.CompanyRelationshipID;
            ViewBag.ProjectID = projectid;
            if (ModelState.IsValid)
                return View(CH.GetDataById<Deal>(id));
            else
                return View();
        }

        #endregion

        #region support
        public ViewResult PhoneSaleSupport()
        {
            var ps = CRM_Logical.GetSalesInvolveProject();
            var data = CH.GetAllData<PhoneSaleSupport>(s => ps.Any(p => p.ID == s.ProjectID) || s.ProjectID == null);
            return View(data);
        }

        [HttpPost]
        public ViewResult PhoneSaleSupport(int? onphonesupportid,string condition=null)
        {
            var ps = CRM_Logical.GetSalesInvolveProject();

            var data = CH.GetAllData<PhoneSaleSupport>(s => ps.Any(p=>p.ID == s.ProjectID)|| s.ProjectID==null);

            if (!string.IsNullOrEmpty(condition))
            {
                string c = (string)condition.Trim();
                data = data.FindAll(d => d.Name.Contains(c) 
                    || d.Answer.Contains(c) 
                    || d.Answer.Contains(c)
                    || d.Block.Contains(c)
                    || d.OnPhoneBlockType.Name.Contains(c)
                    || d.OnPhoneBlockType.Code.ToString() == c
                    );
                ViewBag.Condition = condition;
            }
            
            return View(data);
        }
        #endregion

        #region Lead
        /// <summary>
        /// id = leadid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ViewResult DisplayLead(int? leadid, int? crid,int? projectid)
        {
            this.AddErrorStateIfSalesNoAccessRightToTheCRM(crid);
            ViewBag.CompanyRelationshipID = crid;
            ViewBag.ProjectID = projectid;
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
        public ViewResult EditLead(int? id, int? crid,int? projectid)
        {
            this.AddErrorStateIfSalesNoAccessRightToTheCRM(crid);
            ViewBag.ProjectID = projectid;
            ViewBag.CompanyRelationshipID = crid;
            if (ModelState.IsValid)
            {
                var lead = CH.GetDataById<Lead>(id);
                return View(lead);
            }
            else
                return View();
        }

        [HttpPost]
        public ActionResult EditLead(Lead lead, int? projectid)
        {
       
            if (ModelState.IsValid)
            {
                CH.Edit<Lead>(lead);
                return RedirectToAction("CompanyRelationshipIndex", new { projectid = projectid });
            }
            else
                return View(lead);
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
        #endregion

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
            projectid = this.TrySetProjectIDForUser(projectid);
            ViewBag.ProjectID = projectid;

            this.AddErrorStateIfSalesNoAccessRightToTheProject(projectid);
            this.AddAddErrorStateIfOneOfNameExist<Company>(item.Name_EN,item.Name_CH);
            this.AddErrorIfAllNamesEmpty(item);
            if (ModelState.IsValid)
            {
                List<Category> lc = new List<Category>();
                string categorystring=string.Empty;
                if (checkedCategorys != null)
                {
                   
                    lc = CH.GetAllData<Category>(i => checkedCategorys.Contains(i.ID));
                      lc.ForEach(l=>{
                          if(string.IsNullOrEmpty(categorystring))
                              categorystring=l.Name;
                          else
                              categorystring+= "|"+l.Name;
                      });
                }

                var p = CH.GetDataById<Project>(projectid,"Members");
                CH.Create<Company>(item);
                var ms = new List<Member>();
                ms.Add(p.GetMemberInProjectByName(Employee.GetCurrentUserName()));

                var cr = new CompanyRelationship() { CompanyID = item.ID, ProjectID = projectid, Importancy = 1, Members = ms, Categorys = lc, CategoryString = categorystring };
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
            var cr = CH.GetDataById<CompanyRelationship>(crid);
            ViewBag.COmpanyRelationshipID = cr.ID;
            if (ModelState.IsValid)
                return View(cr.Company);
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

                    string categorystring = string.Empty;
                    lc.ForEach(l =>
                    {
                        if (string.IsNullOrEmpty(categorystring))
                            categorystring = l.Name;
                        else
                            categorystring += "|" + l.Name;
                    });
                    cr.CategoryString = categorystring;
                    CH.Edit<CompanyRelationship>(cr);
                }

                CH.Edit<Company>(item);
                return RedirectToAction("CompanyRelationshipIndex", "Sales", new { projectid = projectid });
            }

            ViewBag.ProjectID = projectid;
            return View();
        }

        /// <summary>
        /// 根据分配显示sales需要拨打的公司
        /// </summary>
        /// <returns></return
        public ViewResult CompanyRelationshipIndex(int? projectid)
        {
          
            projectid = this.TrySetProjectIDForUser(projectid);
            ViewBag.ProjectID = projectid;
            
            if (projectid != null)
            {
                var project = CH.GetDataById<Project>(projectid, "Members");
                var data = project.GetCRM();
                return View(data);
            }
            else
            {
                return View();
            }
        }
        #endregion  
        
        #region
        public ViewResult OpenProjectSalesBrief(int? projectid)
        {
            var data = CH.GetDataById<Project>(projectid);
            data.SaleBrief = HttpUtility.HtmlDecode(data.SaleBrief);
            return View(data);
        }
        #endregion

        #region message
        public ViewResult MyMessageIndex(int? projectid)
        {

            projectid = this.TrySetProjectIDForUser(projectid);
            ViewBag.ProjectID = projectid;
            if (projectid != null)
            {
                var ms = CH.GetAllData<Message>(m => m.ProjectID == projectid);
                return View(ms);
            }
            else
            {
                return View();
            }

           
        }

        public ViewResult AddMessage(int? projectid)
        {
            ViewBag.ProjectID = projectid;
            return View();
        }


        [HttpPost]
        public ActionResult AddMessage(Message item)
        {
            var project = CH.GetDataById<Project>(item.ProjectID);
            if (ModelState.IsValid)
            {
                item = item.SetFlowNumber(project);
                item.Member = Employee.GetCurrentUserName();
                var p = CH.GetDataById<Project>(item.ProjectID, "Members");
                var member = p.GetMemberInProjectByName(item.Member);
                item.SalesTypeID = member.SalesTypeID;
                CH.Create<Message>(item);
                return RedirectToAction("MyMessageIndex");
            }
            return View(item);
        }

        public ViewResult EditMessage(int? id)
        {
            return View(CH.GetDataById<Message>(id));
        }

        [HttpPost]
        public ActionResult EditMessage(Message item)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<Message>(item);
                return RedirectToAction("MyMessageIndex");
            }
            return View(item);
        }

        public ViewResult DisplayMessage(int? id,int?projectid)
        {
            return View(CH.GetDataById<Message>(id));
        }

        #endregion

        #region contancted leads
        public ViewResult ContectedLeads(int? projectid)
        {
            projectid = this.TrySetProjectIDForUser(projectid);
            ViewBag.ProjectID = projectid;

            var account = Employee.GetCurrentUserName();
            var calls = CH.GetAllData<LeadCall>(l => l.ProjectID == projectid && Employee.GetCurrentUserName() ==l.Member.Name,"CompanyRelationship","Lead");
            var contectleads = calls.Distinct(new LeadCallDistinct());
            var ls = new List<ViewContactedLead>();
            foreach (var cl in contectleads)
            {
                ViewContactedLead v = new ViewContactedLead();
                v.Lead = cl.Lead;
                v.ID = cl.Lead.ID;
                v.CompanyRelationshipID = cl.CompanyRelationshipID;

                v.LastCall = calls.FindAll(c=>c.Lead.ID == cl.Lead.ID).OrderByDescending(o => o.CallDate).ToList().FirstOrDefault();
                ls.Add(v);
            }
            return View(ls.OrderByDescending(o=>o.LastCall.CallDate).ToList());
        }

        #endregion

        public ViewResult MyPage()
        {
            var ps = CRM_Logical.GetSalesInvolveProject();

            return View(ps);
        }

        public ActionResult Service_File_Donwload(string fileurl, string filename)
        {
            return new DownloadResult { VirtualPath = fileurl, FileDownloadName = filename };
        }
    }


    public class LeadCallDistinct : IEqualityComparer<LeadCall>
{
    public bool Equals(LeadCall x, LeadCall y)
    {
        if (x.CallDate > y.CallDate)
        { return true; }
        else
        { return false; }
    }

    public int GetHashCode(LeadCall obj) { return 0; }
}

}
