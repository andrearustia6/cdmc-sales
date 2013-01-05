using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utl;
using Entity;
using BLL;
using Model;
using System.Collections;
using System.IO;
using Telerik.Web.Mvc.Extensions;
using System.Text;

namespace Sales.Controllers
{
    [SalesRequired]
    public class SalesController : Controller
    {
        protected override void Dispose(bool disposing)
        {
            CH.DB.Dispose();
            base.Dispose(disposing);
        }

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
            return View("SalesLeadCallsIndex", data.OrderByDescending(o => o.CallDate).ToList());
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
        public ActionResult DeleteLeadCallConfirmed(int? leadcallid, int? crid)
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


            if (ModelState.IsValid)
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
        public ActionResult EditDeal(Deal item, int? projectid)
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
            var deal = CH.GetDataById<Deal>(id);
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
        public ViewResult PhoneSaleSupport(int? onphonesupportid, string condition = null)
        {
            var ps = CRM_Logical.GetSalesInvolveProject();

            var data = CH.GetAllData<PhoneSaleSupport>(s => ps.Any(p => p.ID == s.ProjectID) || s.ProjectID == null);

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
        public ViewResult DisplayLead(int? leadid, int? crid, int? projectid)
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
        public ViewResult EditLead(int? id, int? crid, int? projectid)
        {
            this.AddErrorStateIfSalesNoAccessRightToTheCRM(crid);
            ViewBag.ProjectID = projectid;
            ViewBag.CompanyRelationshipID = crid;
            if (ModelState.IsValid)
            {
                //Image image = ImageController.UploadImg(Request, lead.Image);
                //if (image != null)
                //    item.ImageID = image.ID;
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
        public ActionResult AddLead(Lead lead, int? projectid, int? LeadCallTypeID, string Result, DateTime? CallBackDate, DateTime? CallDate, int? crid)
        {
            this.AddErrorIfAllNamesEmpty(lead);

            if (LeadCallTypeID != null)
            {
                if (CallDate == null)
                {
                    ModelState.AddModelError("", "已选择Call 类型，但是拨打时间为空");
                }
            }
            var exist = CH.GetAllData<Lead>(l => l.CompanyID == lead.CompanyID && lead.Name_CH == l.Name_CH && l.Name_EN == lead.Name_EN).FirstOrDefault();
            if (exist != null)
            {
                ModelState.AddModelError("", "此公司下已经存在相同名字的Lead，无法添加");
            }

            if (ModelState.IsValid)
            {
                //Image image = ImageController.UploadImg(Request, lead.Image);
                //if (image != null)
                //    item.ImageID = image.ID;
                CH.Create<Lead>(lead);

                if (LeadCallTypeID != null)
                {
                    var mem = CH.GetAllData<Member>(m => m.ProjectID == projectid && m.Name == Employee.GetCurrentUserName()).FirstOrDefault();

                    var leadcall = new LeadCall()
                    {
                        LeadID = lead.ID,
                        LeadCallTypeID = LeadCallTypeID,
                        MemberID = mem.ID,
                        Result = Result,
                        CallBackDate = CallBackDate,
                        CompanyRelationshipID = crid,
                        CallDate = CallDate.Value,
                        ProjectID = projectid
                    };
                    CH.Create<LeadCall>(leadcall);

                }
                return RedirectToAction("CompanyRelationshipIndex", new { projectid = projectid });
            }
            else
            {
                ViewBag.CompanyID = lead.CompanyID;
                ViewBag.ProjectID = projectid;
                ViewBag.CRID = crid;
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
                ViewBag.CRID = crid;
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
        public ActionResult AddCompany(Company item, int? projectid, int[] checkedCategorys)
        {
            projectid = this.TrySetProjectIDForUser(projectid);
            ViewBag.ProjectID = projectid;

            this.AddErrorStateIfSalesNoAccessRightToTheProject(projectid);
            this.AddAddErrorStateIfOneOfNameExist<Company>(item.Name_EN, item.Name_CH);
            this.AddErrorIfAllNamesEmpty(item);
            if (ModelState.IsValid)
            {
                List<Category> lc = new List<Category>();
                string categorystring = string.Empty;
                if (checkedCategorys != null)
                {

                    lc = CH.GetAllData<Category>(i => checkedCategorys.Contains(i.ID));
                    lc.ForEach(l =>
                    {
                        if (string.IsNullOrEmpty(categorystring))
                            categorystring = l.Name;
                        else
                            categorystring += "|" + l.Name;
                    });
                }

                var p = CH.GetDataById<Project>(projectid, "Members");
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
                return View(data.OrderByDescending(o => o.CreatedDate).ToList());
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

        public ViewResult DisplayMessage(int? id, int? projectid)
        {
            return View(CH.GetDataById<Message>(id));
        }

        #endregion


        public ViewResult AvailableCompanys(int? projectid)
        {
            ViewBag.ProjectID = projectid;
            return View();
        }

        #region News
        public ViewResult ProjectNewsIndex(int? projectid)
        {
            projectid = this.TrySetProjectIDForUser(projectid);
            ViewBag.ProjectID = projectid;
            if (projectid != null)
            {
                var data = CH.GetAllData<News>(n => n.ProjectID == projectid);
                return View(data);
            }
            else
            {
                return View();
            }

           
        }

        public ViewResult DisplayNews(int? id, int? projectid)
        {
            return View(CH.GetDataById<News>(id));
        }

        #endregion
        #region Json

        [HttpPost]
        public PartialViewResult JsonSaveCompany(Company company, int? crmid, List<int> categorys, int? projectid, int? progressid)
        {

            CH.Edit<Company>(company);
            ViewBag.CRMID = crmid;
            ViewBag.ProjectID = projectid;
            ViewBag.Categorys = categorys;
            var leads = from l in CH.DB.Leads where l.CompanyID == company.ID select l;
            company.Leads = leads.ToList();

            var cr = CH.GetDataById<CompanyRelationship>(crmid, "Categorys");
                List<Category> lc = new List<Category>();
                if (categorys != null)
                {
                    cr.Categorys.Clear();
                    lc = CH.GetAllData<Category>(i => categorys.Contains(i.ID));
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
                  
                }
                cr.ProgressID = progressid;
                CH.Edit<CompanyRelationship>(cr);
            return PartialView(@"~\views\shared\CompanyInfo.cshtml", company);

        }

        //[HttpPost]
        //public PartialViewResult JsonSaveCompany(Company company)
        //{

        //    CH.Edit<Company>(company);
          
        //    var leads = from l in CH.DB.Leads where l.CompanyID == company.ID select l;
        //    company.Leads = leads.ToList();
        //    return PartialView(@"~\views\shared\CompanyInfo.cshtml", company);

        //}

        [HttpPost]
        public PartialViewResult JsonSaveLead(Lead l)
        {
            CH.Edit<Lead>(l);
            return PartialView(@"~\views\shared\Leadinfo.cshtml", l);

        }

        public PartialViewResult JsonSaveLeadCall(LeadCall l)
        {
            CH.Edit<LeadCall>(l);
            return PartialView(@"~\views\shared\singleLeadcall.cshtml", l);

        }
        public PartialViewResult JsonCancelInput()
        {
            return PartialView(@"~\views\shared\SalesInputWindow.cshtml");
        }

        public PartialViewResult JsonRefreshLeads(int? companyid)
        {
            var leads = from l in CH.DB.Leads where l.CompanyID == companyid select l;
            return PartialView(@"~\views\shared\leads.cshtml", leads.ToList());
        }

        public PartialViewResult JsonGetCategorys(int? projectid,int? crmid)
        {
            ViewBag.ProjectID = projectid;
            var categorys = from c in CH.DB.Categorys where c.CompanyRelationships.Any(r => r.ID == crmid) select c.ID;
            return PartialView(@"~\views\shared\Categorys.cshtml", categorys.ToList());
        }


        public PartialViewResult JsonRefreshLeadcalls(int? leadid, int? crmid)
        {
             var calls = from l in CH.DB.LeadCalls where l.LeadID == leadid && l.CompanyRelationshipID == crmid select l;
             return PartialView(@"~\views\shared\leadcalls.cshtml", calls);
        }

  
        public PartialViewResult JsonLeadCalls(int? leadid, int? projectid)
        {
            string user = Employee.GetCurrentUserName();
            var leadcalls = from lcs in CH.DB.LeadCalls
                            where lcs.LeadID == leadid && lcs.ProjectID == projectid
                            select lcs;
            var data = leadcalls.ToList();
            return PartialView(@"~\views\shared\LeadCalls.cshtml", data);
        }

        public PartialViewResult JsonCompanyInfo(int? companyid,int?crmid,int?projectid)
        {
            string user = Employee.GetCurrentUserName();
            ViewBag.ProjectID = projectid;
            ViewBag.CRMID = crmid;
         
            var c = from company in CH.DB.Companys.Include("Leads")
                    where company.ID == companyid
                    select company;

            var cr = from crm in CH.DB.CompanyRelationships.Include("Categorys")
                     where crm.ID == crmid
                     select crm;
            var tc = cr.FirstOrDefault();
            if(tc!=null)
            {
                ViewBag.Categorys = tc.Categorys.Select(x=>x.ID).ToList();
                ViewBag.ProgressID = tc.ProgressID;
            }

            var data = c.ToList().FirstOrDefault();
            return PartialView(@"~\views\shared\CompanyInfo.cshtml", data);
        }

        public PartialViewResult JsonGetCompanys(int? projectid, string condition,string sort )
        {
            if (condition == null) condition = string.Empty;
            var lc = condition.ToLower();

            string user = Employee.GetCurrentUserName();
            var crms = from c in CH.DB.Companys
                       from cr in CH.DB.CompanyRelationships
                       where (c.Name_CH.ToLower().Contains(lc) || c.Name_EN.ToLower().Contains(lc)) && cr.Members.Any(m => m.Name == user) && cr.ProjectID == projectid && c.ID == cr.CompanyID
                       select cr;

            if (sort == "名称")
            {
                return PartialView(@"~\views\shared\CRMList.cshtml", crms.OrderBy(o => o.Company.Name_EN).AsQueryable());
            }
            return PartialView(@"~\views\shared\CRMList.cshtml", crms.OrderByDescending(o => o.CreatedDate).AsQueryable());
        
            
        }



        [HttpPost]
        public PartialViewResult JsonAddSalesInputData(JosonSalesInputData data)
        {
            string username = Employee.GetCurrentUserName();
            JsonValidateInput(data);

            if (data.Satisfied == true)
            {
                var types = data.SubmitType.Split('&');
                //保存公司
                if (data.Company != null && types.Any(t => t == "company"))
                {
                    
                    CH.Create<Company>(data.Company);
                    data.Lead.CompanyID = data.Company.ID;//如果不是添加情况，不需要设companyid
                    //add crm
                    var sales = new List<Member>();
                    var member = from m in CH.DB.Members
                                 where m.Name == username && data.ProjectID == m.ProjectID
                                 select m;
                    sales.AddRange(member);

                    var crm = new CompanyRelationship() { CompanyID = data.Company.ID, Importancy = 1, ProjectID = data.ProjectID, Members = sales };
             
                    CH.Create<CompanyRelationship>(crm);
                    if (data.LeadCall != null)
                    {
                        data.LeadCall.CompanyRelationshipID = crm.ID;
                    }
                }


                //保存lead
                if (data.Lead != null && types.Any(t => t == "lead"))
                {
                    CH.Create<Lead>(data.Lead);

                    data.LeadCall.LeadID = data.Lead.ID;
                }

                if (data.LeadCall != null && types.Any(t => t == "leadcall"))
                {
                    //设置call销售
                    var ms = from m in CH.DB.Members
                             where m.Name == username && m.ProjectID == data.ProjectID
                             select m.ID;

                    if (ms.Count() > 0)
                    {
                        data.LeadCall.MemberID = ms.FirstOrDefault();
                    }
                    //创建新的数据
                    CH.Create<LeadCall>(data.LeadCall);
                }
               
              
                return PartialView(@"~\views\shared\SalesInputWindow.cshtml", data);
            }
            else
                return PartialView(@"~\views\shared\PageMessage.cshtml", data.Message);

        }

        private void JsonValidateInput(JosonSalesInputData data)
        {
            data.Satisfied = true;


            var types = data.SubmitType.Split('&');

            if (types.Any(t => t == "company"))
            {
                //检查公司名
               
                if (string.IsNullOrEmpty(data.Company.Name_CH) && string.IsNullOrEmpty(data.Company.Name_EN))
                {
                    data.Satisfied = false;
                    data.Message = "公司中文名和英文名不可以同时为空, 请填写数据";
                    return;
                }

                string username = Employee.GetCurrentUserName();

                //检查是否是可打公司
                var dbcrm = from crm in CH.DB.CompanyRelationships.Include("Members")
                            where crm.ProjectID == data.ProjectID && (crm.Company.Name_CH == data.Company.Name_CH || crm.Company.Name_EN == data.Company.Name_EN )
                            select crm;

                if (dbcrm.Count() > 0)
                {
                    data.Satisfied = false;
                    if (!dbcrm.Any(c=>c.Members.Any(m=>m.Name==username)))
                        data.Message = "此公司在公司数据库中存在同名公司, 但并不是您的可打公司，请联系项目管理人员把该公司加到您的可打公司";
                    else
                        data.Message = "此公司已经在你的可打列表中，不可以重复添加此公司";
                    return;
                }

            }

            if (types.Any(t => t == "lead"))
            {
                if (string.IsNullOrEmpty(data.Lead.Name_CH) && string.IsNullOrEmpty(data.Lead.Name_EN))
                {
                    data.Satisfied = false;
                    data.Message = "Lead中文名和英文名不可以同时为空, 请填写数据";
                    return;
                }

                //检查lead是否已经存在
                var lead = from l in CH.DB.Leads
                           where l.CompanyID == data.Lead.CompanyID && (l.Name_EN == data.Lead.Name_EN && !string.IsNullOrEmpty(l.Name_EN))
                           select l;
                if (lead.Count() > 0)
                {
                    data.Satisfied = false;
                    data.Message = "在此公司下,Lead已经存在,不能重复添加";
                }
            }

            if (types.Any(t => t == "leadcall"))
            {
                if (data.LeadCall.LeadCallTypeID == null)
                {
                    data.Satisfied = false;
                    data.Message = "致电类型不能为空, 请填写数据";
                    return;
                }
            }
        }
        #endregion

        #region contancted leads
        public ViewResult ContectedLeads(int? projectid)
        {
            projectid = this.TrySetProjectIDForUser(projectid);
            ViewBag.ProjectID = projectid;
            return View(GetContedtedLeadData(projectid));
            //var account = Employee.GetCurrentUserName();
            //var calls = CH.GetAllData<LeadCall>(l => l.ProjectID == projectid && Employee.GetCurrentUserName() ==l.Member.Name,"CompanyRelationship","Lead");
            //var contectleads = calls.Distinct(new LeadCallDistinct());
            //var ls = new List<ViewContactedLead>();
            //foreach (var cl in contectleads)
            //{
            //    ViewContactedLead v = new ViewContactedLead();
            //    v.Lead = cl.Lead;
            //    v.ID = cl.Lead.ID;
            //    v.CompanyRelationshipID = cl.CompanyRelationshipID;

            //    v.LastCall = calls.FindAll(c=>c.Lead.ID == cl.Lead.ID).OrderByDescending(o => o.CallDate).ToList().FirstOrDefault();
            //    ls.Add(v);
            //}
            //return View(ls.OrderByDescending(o=>o.LastCall.CallDate).ToList());
        }

        public List<ViewContactedLead> GetContedtedLeadData(int? projectid)
        {
            var account = Employee.GetCurrentUserName();
            var calls = CH.GetAllData<LeadCall>(l => l.ProjectID == projectid && account == l.Member.Name);
            var contectleads = calls.Distinct(new LeadCallDistinct());
            var ls = new List<ViewContactedLead>();
            foreach (var cl in contectleads)
            {
                ViewContactedLead v = new ViewContactedLead();
                v.Lead = cl.Lead;
                v.ID = cl.Lead.ID;
                v.CompanyRelationshipID = cl.CompanyRelationshipID;

                v.LastCall = calls.FindAll(c => c.Lead.ID == cl.Lead.ID).OrderByDescending(o => o.CallDate).ToList().FirstOrDefault();
                ls.Add(v);
            }
            return ls.OrderByDescending(o => o.LastCall.CallDate).ToList();
        }

        public ActionResult ExportCsv(int? projectid, string orderBy, string filter)
        {
            IEnumerable leads = GetContedtedLeadData(projectid).AsQueryable().ToGridModel(1, Int32.MaxValue, orderBy, string.Empty, filter).Data;
            MemoryStream output = new MemoryStream();
            StreamWriter writer = new StreamWriter(output, Encoding.UTF8);
            writer.Write("姓名,"); writer.Write("职位,");
            writer.Write("邮件,"); writer.Write("性别");
            writer.WriteLine();
            foreach (ViewContactedLead vl in leads)
            {

                writer.Write(vl.Lead.Name);
                writer.Write(","); writer.Write("\"");
                writer.Write(vl.Lead.Title);
                writer.Write("\"");
                writer.Write(",");
                writer.Write("\"");
                writer.Write(vl.Lead.EMail);
                writer.Write("\"");
                writer.Write(",");
                writer.Write(vl.Lead.Gender);
                writer.WriteLine();
            } writer.Flush(); output.Position = 0; return File(output, "text/comma-separated-values", "contected.csv");
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
