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
                CH.Create<LeadCall>(leadcall);

                if (leadcall.LeadCallTypeID != 9)
                    return RedirectToAction("CompanyRelationshipLeadCallsIndex", new { crid = crid });
                else
                    return RedirectToAction("AddDeal", new { projectid = cr.ProjectID, companyrelationshipid = cr.ID });
            }
            else
            {
                ViewBag.CompanyRelationshipID = crid;
                return View(leadcall);
            }
        }
        #endregion

        #region
        [HttpPost]
        public ActionResult AddDeal(Deal item, int? projectid)
        {
            ViewBag.ProjectID = projectid;
            ViewBag.CompanyRelationshipID = item.CompanyRelationshipID;
            if (ModelState.IsValid)
            {
                item.Sales = User.Identity.Name;
                CH.Create<Deal>(item);
                return RedirectToAction("MyDealIndex", "Sales", new { projectid = projectid });
            }
            return View(item);
        }

        public ActionResult AddDeal(int projectid, int companyrelationshipid)
        {
            ViewBag.ProjectID = projectid;
            ViewBag.CompanyRelationshipID = companyrelationshipid;

            return View();
        }

        public ActionResult EditDeal(int? id)
        {
            var item = CH.GetDataById<Deal>(id);
            ViewBag.ProjectID = item.CompanyRelationship.ProjectID;
            ViewBag.CompanyRelationshipID = item.CompanyRelationshipID;
            return View(item);
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
            var data = CH.GetAllData<Deal>();
            return View(data.FindAll(d => d.Sales == User.Identity.Name && d.CompanyRelationship.ProjectID == projectid).OrderByDescending(m=>m.CreatedDate).ToList());
        }

        public ViewResult DisplayDeal(int? id)
        {
            var deal =  CH.GetDataById<Deal>(id);
            this.AddErrorStateIfSalesNoAccessRightToTheCRM(deal.CompanyRelationshipID);

            if (ModelState.IsValid)
                return View(CH.GetDataById<Deal>(id));
            else
                return View();
        }

        #endregion


        #region Lead
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
