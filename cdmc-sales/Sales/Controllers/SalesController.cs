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
using Telerik.Web.Mvc;
using Sales.Model;

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
                data = CH.GetDataById<CompanyRelationship>(crid).LeadCalls;
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
                data = CH.GetDataById<CompanyRelationship>(crid).LeadCalls.FindAll(l => l.LeadID == leadid);
            }
            return View("SalesLeadCallsIndex", data);
        }

        public ActionResult AddLeadCall(int? crid, int? leadid)
        {
            //传拨打人到页面
            var m = CH.GetDataById<Project>(CH.GetDataById<CompanyRelationship>(crid).ProjectID).GetProjectMemberByName();
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

        #endregion participant
        public ActionResult ModifyParticipant(int? dealid, int? projectid)
        {
            ViewBag.ProjectID = projectid;
            ViewBag.DealID = dealid;
            return View(CH.GetDataById<Deal>(dealid));
        }

        [HttpPost]
        public PartialViewResult JsonModifyParticipant(Participant p)
        {
            if (string.IsNullOrEmpty(p.Name))
            {
                return PartialView(@"~\views\shared\PageMessage.cshtml", "名字不能为空");
            }
            if (!string.IsNullOrEmpty(p.Contact) && !SelectHelper.IsTelephone(p.Contact))
            {
                return PartialView(@"~\views\shared\PageMessage.cshtml", "电话号码只能输入数字,空格或中划线.");
            }
            if (!string.IsNullOrEmpty(p.Mobile) && !SelectHelper.IsTelephone(p.Mobile))
            {
                return PartialView(@"~\views\shared\PageMessage.cshtml", "电话号码只能输入数字,空格或中划线.");
            }
            if (p.ParticipantTypeID == null)
            {
                return PartialView(@"~\views\shared\PageMessage.cshtml", "参会类型不能为空");
            }

            if (p.ID == 0)
                CH.Create<Participant>(p);
            else
                CH.Edit<Participant>(p);
            return PartialView(@"~\views\shared\ParticipantContainer.cshtml", CH.GetDataById<Deal>(p.DealID));

        }

        [HttpPost]
        public PartialViewResult JsonDeleteParticipant(int? participantid, int? dealid)
        {
            if (participantid != null)
            {
                var deal = CH.GetDataById<Deal>(dealid);
                CH.Delete<Participant>(participantid);
                return PartialView(@"~\views\shared\ParticipantContainer.cshtml", deal);
            }
            return PartialView(@"~\views\shared\PageMessage.cshtml", "删除不成功");
        }


        #region
        #endregion

        #region deal
        [HttpPost]
        public ActionResult AddDeal(Deal item, int? projectid)
        {
            ViewBag.ProjectID = projectid;
            ViewBag.CompanyRelationshipID = item.CompanyRelationshipID;

            if (ModelState.IsValid)
            {
                item.Sales = Employee.CurrentUserName;
                string prefix = CH.GetDataById<Project>(projectid).ProjectCode + DateTime.Now.Year.ToString();
                var records = CH.GetAllData<Deal>().Where(s => s.DealCode != null && s.DealCode.StartsWith(prefix));
                if (records != null && records.Count() > 0)
                {
                    item.DealCode = prefix + string.Format("{0:D3}", Convert.ToInt32(records.OrderByDescending(s => s.DealCode).First().DealCode.Substring(prefix.Length)) + 1);
                }
                else
                {
                    item.DealCode = prefix + "001";
                }

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
                //Deal old = CH.DB.Deals.AsNoTracking().Where(s => s.ID == item.ID).Single();
                //item.IsConfirm = old.IsConfirm;
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
            return View(data.FindAll(d => d.Sales == Employee.CurrentUserName && d.ProjectID == projectid).OrderByDescending(m => m.CreatedDate).ToList());
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
                    var mem = CH.GetAllData<Member>(m => m.ProjectID == projectid && m.Name == Employee.CurrentUserName).FirstOrDefault();

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
        public ActionResult AddCompany(Company item, int? projectid, int[] checkedCategorys, int? progressid)
        {
            projectid = this.TrySetProjectIDForUser(projectid);
            ViewBag.ProjectID = projectid;
            ViewBag.ProgressID = progressid;
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

                var p = CH.GetDataById<Project>(projectid);
                CH.Create<Company>(item);
                var ms = new List<Member>();
                ms.Add(p.GetMemberInProjectByName(Employee.CurrentUserName));

                var cr = new CompanyRelationship() { CompanyID = item.ID, ProjectID = projectid, Importancy = 1, Members = ms, Categorys = lc, CategoryString = categorystring, ProgressID = progressid };
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
        public ActionResult EditCompany(Company item, int? crid, int? projectid, int[] checkedCategorys, int? progressid)
        {
            this.AddErrorIfAllNamesEmpty(item);


            if (ModelState.IsValid)
            {
                var cr = CH.GetDataById<CompanyRelationship>(crid);
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

                }
                cr.ProgressID = progressid;
                CH.Edit<CompanyRelationship>(cr);

                CH.Edit<Company>(item);
                return RedirectToAction("CompanyRelationshipIndex", "Sales", new { projectid = projectid });
            }
            ViewBag.ProgressID = progressid;
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
                var project = CH.GetDataById<Project>(projectid);
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
                item.Member = Employee.CurrentUserName;
                var p = CH.GetDataById<Project>(item.ProjectID);
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

        #region SubCompany
        public ViewResult AddSubCompany(int? companyid, int? projectid)
        {
            ViewBag.companyID = companyid;
            ViewBag.ProjectID = projectid;
            return View();
        }

        [HttpPost]
        public ActionResult AddSubCompany(SubCompany item, int? projectid)
        {
            this.AddErrorStateIfSalesNoAccessRightToTheProject(projectid);
            this.AddAddErrorStateIfOneOfNameExist<Company>(item.Name_EN, item.Name_CH);
            this.AddErrorIfAllNamesEmpty(item);
            if (ModelState.IsValid)
            {
                CH.Create<SubCompany>(item);
                return RedirectToAction("CompanyRelationshipIndex", "Sales", new { projectid = projectid });
            }
            else
                return View(item);
        }

        public ViewResult EditSubCompany(int? id, int? projectid)
        {
            ViewBag.ProjectID = projectid;
            var item = CH.GetDataById<SubCompany>(id);
            return View(item);
        }

        [HttpPost]
        public ActionResult EditSubCompany(SubCompany item, int? projectid)
        {
            this.AddErrorStateIfSalesNoAccessRightToTheProject(projectid);
            this.AddAddErrorStateIfOneOfNameExist<Company>(item.Name_EN, item.Name_CH);
            this.AddErrorIfAllNamesEmpty(item);

            if (ModelState.IsValid)
            {
                CH.Create<SubCompany>(item);
                return RedirectToAction("CompanyRelationshipIndex", "Sales", new { projectid = projectid });
            }
            else
                return View(item);
        }

        //public ViewResult DisplayCompany(int? id,int? projectid)
        //{
        //    this.AddErrorStateIfSalesNoAccessRightToTheProject(projectid);
        //    var sc = CH.GetDataById<SubCompany>(id);
        //    if (ModelState.IsValid)
        //        return View(sc);
        //    else
        //        return View();
        //}

        public ActionResult DeleteSubCompany(int? projectid, int? id)
        {
            var lc = CH.GetDataById<SubCompany>(id);
            this.AddErrorStateIfSalesNoAccessRightToTheProject(projectid);

            var count = from c in CH.DB.Leads
                        where c.SubCompanyID == id
                        select c;

            if (count.Count() > 0)
                return View(@"~\views\shared\Error.cshtml", null, SR.CannotDelete);

            if (ModelState.IsValid)
                return View(lc);
            else
                return View();
        }

        [HttpPost, ActionName("DeleteSubCompany")]
        public ActionResult DeleteSubCompanyConfirmed(int? id, int? projectid)
        {
            CH.Delete<SubCompany>(id);
            return RedirectToAction("CompanyRelationshipIndex", "Sales", new { projectid = projectid });

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

            var cr = CH.GetDataById<CompanyRelationship>(crmid);
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

        public PartialViewResult JsonGetCategorys(int? projectid, int? crmid)
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
            string user = Employee.CurrentUserName;
            var leadcalls = from lcs in CH.DB.LeadCalls
                            where lcs.LeadID == leadid && lcs.ProjectID == projectid
                            select lcs;
            var data = leadcalls.ToList();
            return PartialView(@"~\views\shared\LeadCalls.cshtml", data);
        }

        public PartialViewResult JsonCompanyInfo(int? companyid, int? crmid, int? projectid)
        {
            string user = Employee.CurrentUserName;
            ViewBag.ProjectID = projectid;
            ViewBag.CRMID = crmid;

            var c = from company in CH.DB.Companys
                    where company.ID == companyid
                    select company;

            var cr = from crm in CH.DB.CompanyRelationships
                     where crm.ID == crmid
                     select crm;
            var tc = cr.FirstOrDefault();
            if (tc != null)
            {
                ViewBag.Categorys = tc.Categorys.Select(x => x.ID).ToList();
                ViewBag.ProgressID = tc.ProgressID;
            }

            var data = c.ToList().FirstOrDefault();
            return PartialView(@"~\views\shared\CompanyInfo.cshtml", data);
        }

        public PartialViewResult JsonGetCompanys(int? projectid, string condition, string sort)
        {
            if (condition == null) condition = string.Empty;
            var lc = condition.Trim().ToLower();

            string user = Employee.CurrentUserName;
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
            string username = Employee.CurrentUserName;
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

                string username = Employee.CurrentUserName;

                //检查是否是可打公司
                var dbcrm = from crm in CH.DB.CompanyRelationships
                            where crm.ProjectID == data.ProjectID && (crm.Company.Name_CH == data.Company.Name_CH || crm.Company.Name_EN == data.Company.Name_EN)
                            select crm;

                if (dbcrm.Count() > 0)
                {
                    data.Satisfied = false;
                    if (!dbcrm.Any(c => c.Members.Any(m => m.Name == username)))
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
        public ViewResult ContectedLeads(int? projectid, DateTime? startdate, DateTime? enddate, int filterId = 0, int? leadCallTypeId = null)
        {
            ViewBag.FilterID = filterId;
            ViewBag.StartDate = startdate;
            ViewBag.EndDate = enddate;
            ViewBag.ProjectID = projectid == null ? this.TrySetProjectIDForUser(projectid) : projectid;
            ViewBag.LeadCallTypeId = leadCallTypeId;
            return View();
        }

        [GridAction]
        public ActionResult _ContectedLeads(int? projectid, DateTime? startdate, DateTime? enddate, int filterId = 0, int? leadCallTypeId = null)
        {
            List<int> projectIds = new List<int>();
            if (projectid.HasValue)
            {
                projectIds.Add(projectid.Value);
            }

            IQueryable<LeadCall> leadCallQuery = CH.DB.LeadCalls.Where(c => c.Member.Name == Employee.CurrentUserName);
            if (projectid.HasValue)
            {
                leadCallQuery = leadCallQuery.Where(c => c.ProjectID == projectid);
            }
            if (leadCallTypeId.HasValue)
            {
                leadCallQuery = leadCallQuery.Where(c => c.LeadCallTypeID == leadCallTypeId);
            }
            if (startdate.HasValue && enddate.HasValue)
            {
                leadCallQuery = leadCallQuery.Where(c => c.CallDate > startdate.Value && c.CallDate < enddate.Value);
            }

            if (filterId == 1)
            {
                leadCallQuery = CRM_Logical.GetProjectFaxoutList(startdate, enddate, projectIds).AsQueryable();
                leadCallTypeId = null;
            }

            if (filterId == 2)
            {
                List<int> lastLeadCallIds = leadCallQuery.GroupBy(c => c.LeadID).Select(c => c.Max(l => l.ID)).ToList();
                leadCallQuery = CH.DB.LeadCalls.Where(c => lastLeadCallIds.Contains(c.ID));
            }
            leadCallQuery = leadCallQuery.Where(w => w.Member.Name == Employee.CurrentUserName);
            List<AjaxViewSaleCallListData> ajaxViewCallListDatas = new List<AjaxViewSaleCallListData>();
            foreach (LeadCall leadCall in leadCallQuery)
            {
                AjaxViewSaleCallListData ajaxViewSalCallListData = new AjaxViewSaleCallListData
                {
                    CallDate = leadCall.CallDate,
                    CompanyContact = leadCall.Lead.SubCompanyID == null ? leadCall.Lead.Company.Contact : leadCall.Lead.SubCompany.Contact,
                    CompanyName = leadCall.Lead.Company.Name,
                    CompanyRelationShipId = leadCall.CompanyRelationshipID,
                    Id = leadCall.ID,
                    CallTypeName = leadCall.LeadCallType.Name,
                    LeaderContact = leadCall.Lead.Contact,
                    LeaderEmail = leadCall.Lead.EMail,
                    LeaderFax = leadCall.Lead.Fax,
                    LeaderGender = leadCall.Lead.Gender,
                    LeaderMobile = leadCall.Lead.Mobile,
                    LeaderName = leadCall.Lead.Name,
                    LeadTitle = leadCall.Lead.Title,
                    Result = leadCall.Result
                };
                if (leadCall.Lead.SubCompanyID == null)
                {
                    if (leadCall.Lead.Company.DistrictNumber != null)
                    {
                        ajaxViewSalCallListData.TimeDifference = leadCall.Lead.Company.DistrictNumber.TimeDifference;
                    }
                }
                else
                {
                    if (leadCall.Lead.SubCompany.DistrictNumber != null)
                    {
                        ajaxViewSalCallListData.TimeDifference = leadCall.Lead.SubCompany.DistrictNumber.TimeDifference;
                    }
                }
                ajaxViewCallListDatas.Add(ajaxViewSalCallListData);
            }

            return View(new GridModel<AjaxViewSaleCallListData> { Data = ajaxViewCallListDatas });
        }



        public List<ViewContactedLead> GetContedtedLeadData(int? projectid)
        {
            var account = Employee.CurrentUserName;
            var calls = CH.GetAllData<LeadCall>(l => l.ProjectID == projectid && account == l.Member.Name);
            var contectleads = calls.Distinct(new LeadCallLeadDistinct());
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
            //IEnumerable leads = GetContedtedLeadData(projectid).AsQueryable().ToGridModel(1, Int32.MaxValue, orderBy, string.Empty, filter).Data;
            
            string user  = Employee.CurrentUserName;
             IQueryable<int?> cs= from c in CH.DB.CompanyRelationships
                                  where c.Members.Select(s => s.Name).Any(a => a == user) && c.ProjectID == projectid
                                  select c.CompanyID;
             IQueryable<Lead> leads = from l in CH.DB.Leads where cs.Any(a=>a==l.CompanyID) select l;
          
            MemoryStream output = new MemoryStream();
            StreamWriter writer = new StreamWriter(output, Encoding.UTF8);
            var projectcode = CH.GetDataById<Project>(projectid).ProjectCode;
            foreach (var vl in leads)
            {

                //writer.Write(vl.Name);
                //writer.Write(","); writer.Write("\"");
                //writer.Write(vl.Title);
                //writer.Write("\"");
                //writer.Write(",");
                //writer.Write("\"");
                writer.Write(vl.EMail);
                //writer.Write("\"");
                //writer.Write(",");
                //writer.Write(vl.Gender);
                writer.WriteLine();
            } writer.Flush(); output.Position = 0; return File(output, "text/comma-separated-values", user + "_" + projectcode + ".csv");
        }

        #endregion

        public ViewResult MyPage(int? month)
        {
            ViewBag.SelectedMonth = month;

            var ps = CRM_Logical.GetSalesInvolveProject();
            return View(ps);
        }

        public ActionResult Service_File_Donwload(string fileurl, string filename)
        {
            string filePath = Request.MapPath(fileurl);
            if (System.IO.File.Exists(filePath))
            {
                return new DownloadResult { VirtualPath = fileurl, FileDownloadName = filename };
            }

            return View(SR.ErrorView, null, SR.CannotDownload);
        }

        public ActionResult CallableCompanies(int? project_filter)
        {
            ViewBag.ProjectID = project_filter == null ? this.TrySetProjectIDForUser(project_filter) : project_filter;
            return View();
        }

        [GridAction]
        public ActionResult _CallableCompanies(int? projectid)
        {
            string user = Employee.CurrentUserName;
            List<AjaxViewSaleCompany> AjaxViewSaleCompanies = new List<AjaxViewSaleCompany>();
            foreach (CompanyRelationship companyRelationship in CH.GetAllData<CompanyRelationship>(c => c.MarkForDelete == false && c.Members.Any(m => m.Name == user) && c.ProjectID == projectid))
            {
                AjaxViewSaleCompany ajaxViewSaleCompany = new AjaxViewSaleCompany()
                {
                    CompanRelationshipId = companyRelationship.ID,
                    CompanyId = companyRelationship.CompanyID,
                    Address = companyRelationship.Company.Address,
                    Business = companyRelationship.Company.Business,
                    Desc = companyRelationship.Company.Description,
                    DistrictNumberId = companyRelationship.Company.DistrictNumberID,
                    Fax = companyRelationship.Company.Fax,
                    Name_CN = companyRelationship.Company.Name_CH,
                    Name_EN = companyRelationship.Company.Name_EN,
                    Phone = companyRelationship.Company.Contact
                };

                if (companyRelationship.Company.Area != null)
                {
                    ajaxViewSaleCompany.IndustryString = companyRelationship.Company.Area.Name_CH;
                }
                if (companyRelationship.Company.CompanyType != null)
                {
                    ajaxViewSaleCompany.TypeString = companyRelationship.Company.CompanyType.Name;
                }
                if (companyRelationship.Progress != null)
                {
                    ajaxViewSaleCompany.ProgressString = companyRelationship.Progress.Description;
                }
                AjaxViewSaleCompanies.Add(ajaxViewSaleCompany);
            }
            return View(new GridModel<AjaxViewSaleCompany> { Data = AjaxViewSaleCompanies });
        }

        [GridAction]
        public ActionResult _LeadInCompany(int? companyId)
        {
            List<AjaxViewLead> AjaxViewLeads = new List<AjaxViewLead>();
            foreach (Lead lead in CH.GetAllData<Lead>(c => c.CompanyID == companyId && c.MarkForDelete == false))
            {
                AjaxViewLead ajaxViewLead = new AjaxViewLead()
                {
                    Address = lead.Address,
                    Birthday = lead.Birthday,
                    CellPhone = lead.Contact,
                    Department = lead.Department,
                    Desc = lead.Description,
                    Fax = lead.Fax,
                    Gender = lead.Gender,
                    LeadId = lead.ID,
                    Name_CN = lead.Name_CH,
                    Name_EN = lead.Name_EN,
                    PersonelEmail = lead.PersonalEmailAddress,
                    Title = lead.Title,
                    Telephone = lead.Contact,
                    WorkingEmail = lead.EMail,
                    Zip = lead.ZIP
                };
                if (lead.SubCompany != null)
                {
                    ajaxViewLead.SubCompany = lead.SubCompany.Name_CH;
                }
                AjaxViewLeads.Add(ajaxViewLead);
            }
            return View(new GridModel<AjaxViewLead> { Data = AjaxViewLeads });
        }

        [GridAction]
        public ActionResult _CallInLead(int? leadId)
        {
            string userName = Employee.CurrentUserName;
            List<AjaxViewLeadCall> ajaxViewCalls = new List<AjaxViewLeadCall>();
            foreach (LeadCall leadCall in CH.GetAllData<LeadCall>(c => c.LeadID == leadId && c.Member.Name == userName && c.MarkForDelete == false))
            {
                AjaxViewLeadCall ajaxViewLeadCall = new AjaxViewLeadCall()
                {
                    LeadId = leadCall.LeadID.Value,
                    CallId = leadCall.ID,
                    CallDate = leadCall.CallDate,
                    CallBackDate = leadCall.CallBackDate,
                    Result = leadCall.Result
                };

                if (leadCall.LeadCallType != null)
                {
                    ajaxViewLeadCall.CallTypeId = leadCall.LeadCallTypeID.Value;
                    ajaxViewLeadCall.CallTypeString = leadCall.LeadCallType.Name;
                }
                ajaxViewCalls.Add(ajaxViewLeadCall);
            }

            return View(new GridModel<AjaxViewLeadCall> { Data = ajaxViewCalls });
        }

        public ActionResult AddSaleCompany(AjaxViewSaleCompany ajaxViewSaleCompany)
        {
            CompanyRelationship companyRelationship = new CompanyRelationship();
            companyRelationship.Company = new Company();
            companyRelationship.Company.Address = ajaxViewSaleCompany.Address;
            companyRelationship.Company.AreaID = ajaxViewSaleCompany.IndustryId;
            companyRelationship.Company.Business = ajaxViewSaleCompany.Business;
            companyRelationship.Company.CompanyTypeID = ajaxViewSaleCompany.TypeId;
            companyRelationship.Company.Contact = ajaxViewSaleCompany.Phone;
            companyRelationship.Company.Description = ajaxViewSaleCompany.Desc;
            companyRelationship.Company.DistrictNumberID = ajaxViewSaleCompany.DistrictNumberId;
            companyRelationship.Company.Fax = ajaxViewSaleCompany.Fax;
            companyRelationship.Company.Name_CH = ajaxViewSaleCompany.Name_CN;
            companyRelationship.Company.Name_EN = ajaxViewSaleCompany.Name_EN;
            companyRelationship.Company.WebSite = ajaxViewSaleCompany.WebSite;
            companyRelationship.Company.ZIP = ajaxViewSaleCompany.ZipCode;
            companyRelationship.Description = ajaxViewSaleCompany.Desc;
            companyRelationship.ProgressID = ajaxViewSaleCompany.ProgressId;
            companyRelationship.Members = new List<Member>() { };
            companyRelationship.Members.Add(CH.GetAllData<Member>(c => c.Name == Employee.CurrentUserName).First());
            companyRelationship.ProjectID = ajaxViewSaleCompany.ProjectId;
            if (ajaxViewSaleCompany.Categories != null)
            {
                companyRelationship.Categorys = CH.GetAllData<Category>(c => ajaxViewSaleCompany.Categories.Contains(c.ID)).ToList();
            }
            CH.Create<CompanyRelationship>(companyRelationship);

            return RedirectToAction("CallableCompanies");
        }

        public ActionResult GetEditSaleCompany(int companyId)
        {
            CompanyRelationship companyRelationship = CH.GetAllData<CompanyRelationship>(c => c.CompanyID == companyId).First();
            AjaxViewSaleCompany ajaxViewSaleCompany = new AjaxViewSaleCompany()
            {
                ProjectId = companyRelationship.ProjectID,
                CompanyId = companyRelationship.CompanyID,
                Address = companyRelationship.Company.Address,
                Business = companyRelationship.Company.Business,
                Desc = companyRelationship.Company.Description,
                DistrictNumberId = companyRelationship.Company.DistrictNumberID,
                Fax = companyRelationship.Company.Fax,
                Name_CN = companyRelationship.Company.Name_CH,
                Name_EN = companyRelationship.Company.Name_EN,
                Phone = companyRelationship.Company.Contact,
                Categories = companyRelationship.Categorys.Select(c => c.ID).ToList()
            };

            if (companyRelationship.Company.Area != null)
            {
                ajaxViewSaleCompany.IndustryId = companyRelationship.Company.AreaID.Value;
                ajaxViewSaleCompany.IndustryString = companyRelationship.Company.Area.Name_CH;
            }
            if (companyRelationship.Company.CompanyType != null)
            {
                ajaxViewSaleCompany.TypeId = companyRelationship.Company.CompanyTypeID.Value;
                ajaxViewSaleCompany.TypeString = companyRelationship.Company.CompanyType.Name;
            }
            if (companyRelationship.Progress != null)
            {
                ajaxViewSaleCompany.ProgressId = companyRelationship.ProgressID.Value;
                ajaxViewSaleCompany.ProgressString = companyRelationship.Progress.Description;
            }

            return PartialView("CompanyEdit", ajaxViewSaleCompany);
        }

        public ActionResult GetAddSaleCompany(int projectId)
        {
            AjaxViewSaleCompany ajaxViewSaleCompany = new AjaxViewSaleCompany() { ProjectId = projectId, Categories = new List<int>() { } };
            return PartialView("CompanyAdd", ajaxViewSaleCompany);
        }

        public ActionResult GetAddSaleCompanyAll(int projectId)
        {
            AjaxViewSaleCompanyAll ajaxViewSaleCompanyAll = new AjaxViewSaleCompanyAll();
            ajaxViewSaleCompanyAll.AjaxViewSaleCompany = new AjaxViewSaleCompany() { ProjectId = projectId, Categories = new List<int>() { } };
            ajaxViewSaleCompanyAll.AjaxViewLead = new AjaxViewLead();
            ajaxViewSaleCompanyAll.AjaxViewLeadCall = new AjaxViewLeadCall();
            return PartialView("CompanyAddAll", ajaxViewSaleCompanyAll);
        }

        public ActionResult AddSaleCompanyAll(AjaxViewSaleCompanyAll ajaxViewSaleCompanyAll)
        {
            CompanyRelationship companyRelationship = new CompanyRelationship();
            companyRelationship.Company = new Company();
            companyRelationship.Company.Address = ajaxViewSaleCompanyAll.AjaxViewSaleCompany.Address;
            companyRelationship.Company.AreaID = ajaxViewSaleCompanyAll.AjaxViewSaleCompany.IndustryId;
            companyRelationship.Company.Business = ajaxViewSaleCompanyAll.AjaxViewSaleCompany.Business;
            companyRelationship.Company.CompanyTypeID = ajaxViewSaleCompanyAll.AjaxViewSaleCompany.TypeId;
            companyRelationship.Company.Contact = ajaxViewSaleCompanyAll.AjaxViewSaleCompany.Phone;
            companyRelationship.Company.Description = ajaxViewSaleCompanyAll.AjaxViewSaleCompany.Desc;
            companyRelationship.Company.DistrictNumberID = ajaxViewSaleCompanyAll.AjaxViewSaleCompany.DistrictNumberId;
            companyRelationship.Company.Fax = ajaxViewSaleCompanyAll.AjaxViewSaleCompany.Fax;
            companyRelationship.Company.Name_CH = ajaxViewSaleCompanyAll.AjaxViewSaleCompany.Name_CN;
            companyRelationship.Company.Name_EN = ajaxViewSaleCompanyAll.AjaxViewSaleCompany.Name_EN;
            companyRelationship.Company.WebSite = ajaxViewSaleCompanyAll.AjaxViewSaleCompany.WebSite;
            companyRelationship.Company.ZIP = ajaxViewSaleCompanyAll.AjaxViewSaleCompany.ZipCode;
            companyRelationship.Description = ajaxViewSaleCompanyAll.AjaxViewSaleCompany.Desc;
            companyRelationship.ProgressID = ajaxViewSaleCompanyAll.AjaxViewSaleCompany.ProgressId;
            companyRelationship.Members = new List<Member>() { };
            companyRelationship.Members.Add(CH.GetAllData<Member>(c => c.Name == Employee.CurrentUserName).First());
            companyRelationship.ProjectID = ajaxViewSaleCompanyAll.AjaxViewSaleCompany.ProjectId;
            if (ajaxViewSaleCompanyAll.AjaxViewSaleCompany.Categories != null)
            {
                companyRelationship.Categorys = CH.GetAllData<Category>(c => ajaxViewSaleCompanyAll.AjaxViewSaleCompany.Categories.Contains(c.ID)).ToList();
            }
            CH.Create<CompanyRelationship>(companyRelationship);

            Lead lead = new Lead()
            {
                Name_CH = ajaxViewSaleCompanyAll.AjaxViewLead.Name_CN,
                Name_EN = ajaxViewSaleCompanyAll.AjaxViewLead.Name_EN,
                CompanyID = companyRelationship.Company.ID,
                Address = ajaxViewSaleCompanyAll.AjaxViewLead.Address,
                Birthday = ajaxViewSaleCompanyAll.AjaxViewLead.Birthday,
                Contact = ajaxViewSaleCompanyAll.AjaxViewLead.Telephone,
                Department = ajaxViewSaleCompanyAll.AjaxViewLead.Department,
                Description = ajaxViewSaleCompanyAll.AjaxViewLead.Desc,
                EMail = ajaxViewSaleCompanyAll.AjaxViewLead.WorkingEmail,
                Fax = ajaxViewSaleCompanyAll.AjaxViewLead.Fax,
                Gender = ajaxViewSaleCompanyAll.AjaxViewLead.Gender,
                Mobile = ajaxViewSaleCompanyAll.AjaxViewLead.CellPhone
            };
            CH.Create<Lead>(lead);

            LeadCall leadCall = new LeadCall();
            leadCall.CallBackDate = ajaxViewSaleCompanyAll.AjaxViewLeadCall.CallBackDate;
            leadCall.CallDate = ajaxViewSaleCompanyAll.AjaxViewLeadCall.CallDate;
            leadCall.CompanyRelationshipID = companyRelationship.ID;
            leadCall.LeadCallTypeID = ajaxViewSaleCompanyAll.AjaxViewLeadCall.CallTypeId;
            leadCall.LeadID = lead.ID;
            leadCall.Member = CH.GetAllData<Member>(c => c.Name == Employee.CurrentUserName).First();
            leadCall.ProjectID = ajaxViewSaleCompanyAll.AjaxViewSaleCompany.ProjectId;
            leadCall.Result = ajaxViewSaleCompanyAll.AjaxViewLeadCall.Result;
            CH.Create<LeadCall>(leadCall);

            return RedirectToAction("CallableCompanies");
        }

        [HttpPost]
        public ActionResult EditCompany(AjaxViewSaleCompany ajaxViewSaleCompany)
        {
            CompanyRelationship companyRelationship = CH.GetAllData<CompanyRelationship>(c => c.CompanyID == ajaxViewSaleCompany.CompanyId).First();
            companyRelationship.Company.Address = ajaxViewSaleCompany.Address;
            companyRelationship.Company.AreaID = ajaxViewSaleCompany.IndustryId;
            companyRelationship.Company.Business = ajaxViewSaleCompany.Business;
            companyRelationship.Company.CompanyTypeID = ajaxViewSaleCompany.TypeId;
            companyRelationship.Company.Contact = ajaxViewSaleCompany.Phone;
            companyRelationship.Company.Description = ajaxViewSaleCompany.Desc;
            companyRelationship.Company.DistrictNumberID = ajaxViewSaleCompany.DistrictNumberId;
            companyRelationship.Company.Fax = ajaxViewSaleCompany.Fax;
            companyRelationship.Company.Name_CH = ajaxViewSaleCompany.Name_CN;
            companyRelationship.Company.Name_EN = ajaxViewSaleCompany.Name_EN;
            companyRelationship.Company.WebSite = ajaxViewSaleCompany.WebSite;
            companyRelationship.Company.ZIP = ajaxViewSaleCompany.ZipCode;
            companyRelationship.Description = ajaxViewSaleCompany.Desc;
            companyRelationship.ProgressID = ajaxViewSaleCompany.ProgressId;
            companyRelationship.Categorys.Clear();
            companyRelationship.Categorys = CH.GetAllData<Category>(c => ajaxViewSaleCompany.Categories.Contains(c.ID)).ToList();
            CH.Edit<CompanyRelationship>(companyRelationship);
            return RedirectToAction("CallableCompanies");
        }

        [HttpPost]
        public ActionResult DeleteSaleCompany(int companyId)
        {
            CompanyRelationship companyRelationship = CH.GetAllData<CompanyRelationship>(c => c.CompanyID == companyId).First();
            companyRelationship.MarkForDelete = true;
            CH.Edit<CompanyRelationship>(companyRelationship);
            return Content("公司删除成功！");
        }

        public ActionResult GetAddSaleLead(int companyId)
        {
            AjaxViewLead ajaxViewLead = new AjaxViewLead() { CompanyId = companyId };
            return PartialView("LeadAdd", ajaxViewLead);
        }

        public ActionResult AddSaleLead(AjaxViewLead ajaxViewLead)
        {
            Lead lead = new Lead()
            {
                Name_CH = ajaxViewLead.Name_CN,
                Name_EN = ajaxViewLead.Name_EN,
                CompanyID = ajaxViewLead.CompanyId,
                Address = ajaxViewLead.Address,
                Birthday = ajaxViewLead.Birthday,
                Contact = ajaxViewLead.Telephone,
                Department = ajaxViewLead.Department,
                Description = ajaxViewLead.Desc,
                EMail = ajaxViewLead.WorkingEmail,
                Fax = ajaxViewLead.Fax,
                Gender = ajaxViewLead.Gender,
                Mobile = ajaxViewLead.CellPhone
            };

            CH.Create<Lead>(lead);
            return RedirectToAction("CallableCompanies");
        }

        public ActionResult GetEditSaleLead(int leadId)
        {
            Lead lead = CH.GetAllData<Lead>(c => c.ID == leadId).First();
            AjaxViewLead ajaxViewLead = new AjaxViewLead()
            {
                CompanyId = lead.CompanyID.Value,
                Address = lead.Address,
                Birthday = lead.Birthday,
                CellPhone = lead.Contact,
                Department = lead.Department,
                Desc = lead.Description,
                Fax = lead.Fax,
                Gender = lead.Gender,
                LeadId = lead.ID,
                Name_CN = lead.Name_CH,
                Name_EN = lead.Name_EN,
                PersonelEmail = lead.PersonalEmailAddress,
                Title = lead.Title,
                Telephone = lead.Contact,
                WorkingEmail = lead.EMail,
                Zip = lead.ZIP,
                WeiBo = lead.WeiBo,
                WeiXin = lead.WeiXin,
                LinkIn = lead.LinkIn,
                FaceBook = lead.FaceBook,
                Blog = lead.Blog
            };
            return PartialView("LeadEdit", ajaxViewLead);
        }

        [HttpPost]
        public ActionResult EditSaleLead(AjaxViewLead ajaxViewLead)
        {
            Lead lead = CH.GetAllData<Lead>(c => c.ID == ajaxViewLead.LeadId).First();
            lead.Name_CH = ajaxViewLead.Name_CN;
            lead.Name_EN = ajaxViewLead.Name_EN;
            lead.CompanyID = ajaxViewLead.CompanyId;
            lead.Address = ajaxViewLead.Address;
            lead.Birthday = ajaxViewLead.Birthday;
            lead.Contact = ajaxViewLead.Telephone;
            lead.Department = ajaxViewLead.Department;
            lead.Description = ajaxViewLead.Desc;
            lead.EMail = ajaxViewLead.WorkingEmail;
            lead.Fax = ajaxViewLead.Fax;
            lead.Gender = ajaxViewLead.Gender;
            lead.Mobile = ajaxViewLead.CellPhone;
            lead.WeiBo = ajaxViewLead.WeiBo;
            lead.WeiXin = ajaxViewLead.WeiXin;
            lead.LinkIn = ajaxViewLead.LinkIn;
            lead.FaceBook = ajaxViewLead.FaceBook;
            lead.Blog = ajaxViewLead.Blog;
            CH.Edit<Lead>(lead);
            return RedirectToAction("CallableCompanies");
        }

        public ActionResult GetAddSaleCall(int leadId, int companyRelationId, int projectId)
        {
            AjaxViewLeadCall ajaxViewLeadCall = new AjaxViewLeadCall() { LeadId = leadId, CompanyRelationshipId = companyRelationId, ProjectId = projectId };
            return PartialView("CallAdd", ajaxViewLeadCall);
        }

        public ActionResult AddSaleCall(AjaxViewLeadCall ajaxViewLeadCall)
        {
            LeadCall leadCall = new LeadCall();
            leadCall.CallBackDate = ajaxViewLeadCall.CallBackDate;
            leadCall.CallDate = ajaxViewLeadCall.CallDate;
            leadCall.CompanyRelationshipID = ajaxViewLeadCall.CompanyRelationshipId;
            leadCall.LeadCallTypeID = ajaxViewLeadCall.CallTypeId;
            leadCall.LeadID = ajaxViewLeadCall.LeadId;
            leadCall.Member = CH.GetAllData<Member>(c => c.Name == Employee.CurrentUserName).First();
            leadCall.ProjectID = ajaxViewLeadCall.ProjectId;
            leadCall.Result = ajaxViewLeadCall.Result;
            CH.Create<LeadCall>(leadCall);
            return RedirectToAction("CallableCompanies");
        }

        public ActionResult GetEditSaleCall(int leadCallId)
        {
            LeadCall leadCall = CH.GetAllData<LeadCall>(c => c.ID == leadCallId).First();
            AjaxViewLeadCall ajaxViewLeadCall = new AjaxViewLeadCall();
            ajaxViewLeadCall.CallId = leadCallId;
            ajaxViewLeadCall.CallBackDate = leadCall.CallBackDate;
            ajaxViewLeadCall.CallDate = leadCall.CallDate;
            ajaxViewLeadCall.CompanyRelationshipId = leadCall.CompanyRelationshipID.Value;
            ajaxViewLeadCall.CallTypeId = leadCall.LeadCallTypeID.Value;
            ajaxViewLeadCall.LeadId = leadCall.LeadID.Value;
            ajaxViewLeadCall.ProjectId = leadCall.ProjectID.Value;
            ajaxViewLeadCall.Result = leadCall.Result;

            return PartialView("CallEdit", ajaxViewLeadCall);
        }

        public ActionResult EditSaleCall(AjaxViewLeadCall ajaxViewLeadCall)
        {
            LeadCall leadCall = CH.GetAllData<LeadCall>(c => c.ID == ajaxViewLeadCall.CallId).First();
            leadCall.CallBackDate = ajaxViewLeadCall.CallBackDate;
            leadCall.CallDate = ajaxViewLeadCall.CallDate;
            leadCall.LeadCallTypeID = ajaxViewLeadCall.CallTypeId;
            leadCall.Member = CH.GetAllData<Member>(c => c.Name == Employee.CurrentUserName).First();
            leadCall.Result = ajaxViewLeadCall.Result;
            CH.Edit<LeadCall>(leadCall);
            return RedirectToAction("CallableCompanies");
        }

        public ActionResult DeleteSaleLead(int leadId)
        {
            Lead lead = CH.GetAllData<Lead>(c => c.ID == leadId).First();
            lead.MarkForDelete = true;
            CH.Edit<Lead>(lead);
            return Content("Lead删除成功！");
        }

        public ActionResult DeleteSaleCall(int callId)
        {
            LeadCall leadCall = CH.GetAllData<LeadCall>(c => c.ID == callId).First();
            leadCall.MarkForDelete = true;
            CH.Edit<LeadCall>(leadCall);
            return Content("Call删除成功！");
        }

        public ActionResult CheckCompanyExist(string beforeUpdateCN, string afterUpdateCN)
        {
            if (CH.GetAllData<CompanyRelationship>(c => c.MarkForDelete == false && c.Company.Name_CH == afterUpdateCN && c.Company.Name_CH != beforeUpdateCN).Count > 0)
            {
                return Content("同名公司名字已存在！");
            }

            return Content("");
        }
        
        public ActionResult SalesDashboard(int? month)
        {
            if (month == null) month = DateTime.Now.Month;
            var single = CRM_Logical._EmployeePerformance.GetSingleSalesPerformance(month.Value);

            return PartialView(single);
        }

        public ActionResult TeamLeaderDashboard(int? month)
        {
            if (month == null) month = DateTime.Now.Month;
            var single = CRM_Logical._EmployeePerformance.GetSingleTemaLeadsPerformance(month.Value);

            return PartialView(single);
        }

        //public JsonResult GetUserCallBackSince7DayBefore(int? projectid)
        //{
        //    var sevendaybefore = DateTime.Now.AddDays(-7);
        //    var user = Employee.CurrentUserName;
        //    var calls = from c in CH.DB.LeadCalls
        //                where c.Member.Name == user && c.CallBackDate >= sevendaybefore
        //                select c;
        //    if (projectid != null)
        //        calls = calls.Where(c => c.ProjectID == projectid);
        //    var rets= from cs in calls
        //              select new
        //                {
        //                    Name=cs.Lead.Name,
        //                    Title=cs.Lead.Title,

        //            };
        //    return Json(calls);
        //}

        [GridAction]
        public ActionResult GetUserCallBackSince7DayBefore(int? projectid)
        {
            var sevendaybefore = DateTime.Now.AddDays(-7);
            var user = Employee.CurrentUserName;
            var calls = from c in CH.DB.LeadCalls
                        where c.Member.Name == user && c.CallBackDate >= sevendaybefore
                        select c;
            if (projectid != null)
                calls = calls.Where(c => c.ProjectID == projectid);
            var rets= from cs in calls
                      select new
                        {
                            Name = cs.Lead.Name,
                            Title = cs.Lead.Title

                        };
            return View(new GridModel(rets));
        }
    }
}
