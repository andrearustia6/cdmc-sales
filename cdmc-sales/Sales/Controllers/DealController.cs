using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Entity;
using Utl;
using BLL;
using Model;
using Telerik.Web.Mvc;
using System.Data.Objects;

namespace Sales.Controllers
{
    //[LeaderRequired]
    public class DealController : Controller
    {
        protected override void Dispose(bool disposing)
        {
            CH.DB.Dispose();
            base.Dispose(disposing);
        }
        public ViewResult IndexEdit(List<int> selectedprojects, string selecttype, int? paytype)
        {
            ViewBag.SelectedProjects = selectedprojects;
            selectedprojects = this.TryGetSelectedProjectIDs(selectedprojects);
            ViewBag.Right = EditRight.DealsEdit.ToString();
            ViewBag.SelectType = selecttype;
            ViewBag.paytype = paytype;
            return View("index");
        }

        public ViewResult IndexReview(List<int> selectedprojects, string selecttype, int? paytype)
        {
            ViewBag.SelectedProjects = selectedprojects;
            selectedprojects = this.TryGetSelectedProjectIDs(selectedprojects);
            ViewBag.Right = ReviewRight.DealsReview.ToString();
            ViewBag.SelectType = selecttype;
            if (paytype == null)
                paytype = 0;
            ViewBag.paytype = paytype;
            return View("index");
        }
        [GridAction]
        public ActionResult _ParticipantsInDeal(int? dealid)
        {
            if (dealid != null)
            {
                var ps = from p in CH.DB.Participants
                         where p.DealID == dealid
                         select new AjaxViewParticipant
                         {
                             ID = p.ID,
                             Contact = p.Contact,
                             Name = p.Name,
                             DealID = p.ID,
                             Email = p.Email,
                             Gender = p.Gender,
                             Mobile = p.Mobile,
                             ProjectCode = p.Project.ProjectCode,
                             ParticipantTypeNameCH = p.ParticipantType.Name_CH,
                             ParticipantTypeNameEN = p.ParticipantType.Name_EN,
                             ProjectID = p.ProjectID
                         };
                return View(new GridModel<AjaxViewParticipant> { Data = ps.ToList() });
            }
            return View(new GridModel<AjaxViewParticipant> { });
        }
        [GridAction]
        public ActionResult _Index(string sp, int? paytype)
        {
            var selectedprojects = Utl.Utl.ConvertStringToSelectProjectID(sp);
            selectedprojects = this.TryGetSelectedProjectIDs(selectedprojects);
            var deals = from dd in CH.DB.Deals select dd;
            if (paytype == 1)
                deals = deals.Where(d => d.Income > 0);
            else if (paytype == 2)
                deals = deals.Where(d => d.Income <= 0);
            if (Employee.CurrentRole.Level == ChinaTLRequired.LVL)
            {
                List<string> saleslist = new List<string>();
                foreach (var d in CH.GetAllData<EmployeeRole>())
                {
                    if (d.RoleID == 12)
                        saleslist.Add(d.AccountName);
                }
                var ds = from d in deals.Where(w=> saleslist.Contains(w.Sales))
                         where selectedprojects.Contains(d.ProjectID.Value) && d.Abandoned == false
                         select new AjaxViewDeal
                         {
                             CompanyNameEN = d.CompanyRelationship.Company.Name_EN,
                             CompanyNameCH = d.CompanyRelationship.Company.Name_CH,
                             DealCode = d.DealCode,
                             Abandoned = d.Abandoned,
                             AbandonReason = d.AbandonReason,
                             ActualPaymentDate = d.ActualPaymentDate,
                             Committer = d.Committer,
                             CommitterContect = d.Committer,
                             CommitterEmail = d.CommitterEmail,
                             ExpectedPaymentDate = d.ExpectedPaymentDate,
                             ID = d.ID,
                             Income = d.Income,
                             IsClosed = d.IsClosed,
                             PackageNameCH = d.Package.Name_CH,
                             PackageNameEN = d.Package.Name_EN,
                             Payment = d.Payment,
                             Currency = d.Currencytype.Name,
                             PaymentDetail = d.PaymentDetail,
                             Sales = d.Sales,
                             ProjectCode = d.Project.ProjectCode,
                             SignDate = d.SignDate,
                             TicketDescription = d.TicketDescription,
                             IsConfirm = (d.IsConfirm == true ? "是" : "否")
                         };
                var list = ds.OrderBy(o => o.SignDate).ToList();
                return View(new GridModel<AjaxViewDeal> { Data = list });
            }
            else
            {
                var ds = from d in deals
                         where selectedprojects.Contains(d.ProjectID.Value) && d.Abandoned == false
                         select new AjaxViewDeal
                         {
                             CompanyNameEN = d.CompanyRelationship.Company.Name_EN,
                             CompanyNameCH = d.CompanyRelationship.Company.Name_CH,
                             DealCode = d.DealCode,
                             Abandoned = d.Abandoned,
                             AbandonReason = d.AbandonReason,
                             ActualPaymentDate = d.ActualPaymentDate,
                             Committer = d.Committer,
                             CommitterContect = d.Committer,
                             CommitterEmail = d.CommitterEmail,
                             ExpectedPaymentDate = d.ExpectedPaymentDate,
                             ID = d.ID,
                             Income = d.Income,
                             IsClosed = d.IsClosed,
                             PackageNameCH = d.Package.Name_CH,
                             PackageNameEN = d.Package.Name_EN,
                             Payment = d.Payment,
                             Currency = d.Currencytype.Name,
                             PaymentDetail = d.PaymentDetail,
                             Sales = d.Sales,
                             ProjectCode = d.Project.ProjectCode,
                             SignDate = d.SignDate,
                             TicketDescription = d.TicketDescription,
                             IsConfirm = (d.IsConfirm == true ? "是" : "否")
                         };
                var list = ds.OrderBy(o => o.SignDate).ToList();
                return View(new GridModel<AjaxViewDeal> { Data = list });
            }
        }

        public ViewResult AllIndex()
        {
            return View();
        }
        [GridAction]
        public ActionResult _AllIndex(string dealcode, string companyname)
        {
           
            var deals =  CH.DB.Deals.Where(w=>w.Deleted==false);
            if (dealcode != null)
                deals = deals.Where(d => d.DealCode.Contains(dealcode));
            if(companyname!=null)
                deals = deals.Where(d => d.CompanyRelationship.Company.Name_CH.Contains(companyname) || d.CompanyRelationship.Company.Name_EN.Contains(companyname));

            var ds = from d in deals
                     select new AjaxViewDeal
                     {
                         CompanyNameEN = d.CompanyRelationship.Company.Name_EN,
                         CompanyNameCH = d.CompanyRelationship.Company.Name_CH,
                         DealCode = d.DealCode,
                         Abandoned = d.Abandoned,
                         AbandonReason = d.AbandonReason,
                         ActualPaymentDate = d.ActualPaymentDate,
                         Committer = d.Committer,
                         CommitterContect = d.Committer,
                         CommitterEmail = d.CommitterEmail,
                         ExpectedPaymentDate = d.ExpectedPaymentDate,
                         ID = d.ID,
                         Income = d.Income,
                         IsClosed = d.IsClosed,
                         PackageNameCH = d.Package.Name_CH,
                         PackageNameEN = d.Package.Name_EN,
                         Payment = d.Payment,
                         Currency = d.Currencytype.Name,
                         PaymentDetail = d.PaymentDetail,
                         Sales = d.Sales,
                         ProjectCode = d.Project.ProjectCode,
                         SignDate = d.SignDate,
                         TicketDescription = d.TicketDescription,
                         IsConfirm = (d.IsConfirm == true ? "是" : "否"),
                         ModifiedDate = d.ModifiedDate,
                         ParticipantTypeName = d.Package.ParticipantType.Name_EN,
                         DealType = d.DealType,
                         Poll = d.Poll
                     };
            var list = ds.OrderBy(o => o.SignDate).ToList();
            return View(new GridModel<AjaxViewDeal> { Data = list });
        }
        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<Deal>(id));
        }

        public ActionResult Edit(int id)
        {
            var item = CH.GetDataById<Deal>(id);
            ViewBag.ProjectID = item.CompanyRelationship.ProjectID;
            ViewBag.CompanyRelationshipID = item.CompanyRelationshipID;
            return View(item);
        }

        [HttpPost]
        public ActionResult Edit(Deal item)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<Deal>(item);
                var projectid = CH.GetDataById<CompanyRelationship>(item.CompanyRelationshipID).ProjectID;
                return RedirectToAction("index", new { id = projectid });
            }

            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<Deal>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var item = CH.GetDataById<Deal>(id);
            var projectid = CH.GetDataById<CompanyRelationship>(item.CompanyRelationshipID).ProjectID;
            foreach (var p in item.Participants)
            {
                CH.Delete<Participant>(p.ID);
            }
            CH.Delete<Deal>(id);

            return RedirectToAction("index", new { id = projectid });
        }

        [HttpGet]
        public ViewResult ConfirmList(int? projectId)
        {
            ViewBag.selectVal = projectId;

            return View();
        }

        [GridAction]
        public ActionResult _SelectIndex(string filterId, int? projectId, string CompanyDealCodeLike, int? PaymentID, int? ParticipantsID,int? year,int? month)
        {
            //List<AjaxViewDeal> deals;
           var deals = getData(filterId, projectId, CompanyDealCodeLike, PaymentID, ParticipantsID,year,month).ToList<AjaxViewDeal>();
           return View(new GridModel(deals));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveAjaxEditing(int id)
        {
            if (Employee.CurrentRole.Level == 4)
            {
                AjaxViewDeal newData = new AjaxViewDeal();
                TryUpdateModel<AjaxViewDeal>(newData);
                var item = CH.GetDataById<Deal>(id);
                item.Income = newData.Income;
                item.ActualPaymentDate = newData.ActualPaymentDate;
                item.Abandoned = newData.Abandoned;
                item.AbandonReason = newData.AbandonReason;
                if (item.Income > 0 && item.ActualPaymentDate != null)
                {
                    CH.Edit<Deal>(item);
                }
                else
                {
                    ModelState.AddModelError("Income", "实际入账需大于零.");
                }
            }
            else
            {
                var item = CH.GetDataById<Deal>(id);
                item.IsConfirm = true;
                item.Confirmor = Employee.CurrentUserName;
                CH.Edit<Deal>(item);
            }
            return View(new GridModel(getData()));
        }

        private IOrderedEnumerable<AjaxViewDeal> getData(string filter = "", int? projectId = null, string CompanyDealCodeLike = "", int? PaymentID = null, int? ParticipantsID = null,int? year=null, int? month=null)
        {
            var deals = CRM_Logical.GetDeals(false, projectId, null, filter);

            if (year != null)
                deals = deals.Where(w => w.ActualPaymentDate.Value.Year == year);
            if (month != null)
                deals = deals.Where(w => w.ActualPaymentDate.Value.Month == month);
            if (!string.IsNullOrWhiteSpace(CompanyDealCodeLike))
            {
                deals = deals.Where(w => w.CompanyRelationship.Company.Name_EN.Contains(CompanyDealCodeLike.Trim()) || w.CompanyRelationship.Company.Name_CH.Contains(CompanyDealCodeLike.Trim()) || w.DealCode.Contains(CompanyDealCodeLike.Trim()));
            }

            if (PaymentID != null)
            {
                switch (PaymentID)
                {
                    case 1:
                        deals = deals.Where(s => s.Payment > 0 && s.Payment <= 3000);
                        break;
                    case 2:
                        deals = deals.Where(s => s.Payment > 3000 && s.Payment <= 5000);
                        break;
                    case 3:
                        deals = deals.Where(s => s.Payment > 5000 && s.Payment <= 8000);
                        break;
                    case 4:
                        deals = deals.Where(s => s.Payment > 8000 && s.Payment <= 10000);
                        break;
                    case 5:
                        deals = deals.Where(s => s.Payment > 10000 && s.Payment <= 15000);
                        break;
                    case 6:
                        deals = deals.Where(s => s.Payment > 15000);
                        break;
                    default:
                        break;
                }
            }

            if (ParticipantsID != null)
            {
                if (ParticipantsID == 1)
                {
                    deals = deals.Where(s => s.Participants.Any());
                }
                else
                {
                    deals = deals.Where(s => !s.Participants.Any());
                }
            }
            var emprole = CH.DB.EmployeeRoles.Where(w => 1 == 1);
            List<Deal> dllist;
            if (Employee.CurrentRole.Level == 4)//财务填写income
            {

                dllist = deals.ToList();
                var ds = from d in dllist
                         where d.IsConfirm == true
                         //where d.IsConfirm == true && (d.Income == 0 || d.ActualPaymentDate == null)
                         select new AjaxViewDeal
                         {
                             CompanyNameEN = d.CompanyRelationship.Company.Name_EN,
                             CompanyNameCH = d.CompanyRelationship.Company.Name_CH,
                             DealCode = d.DealCode,
                             Abandoned = d.Abandoned,
                             AbandonReason = d.AbandonReason,
                             ActualPaymentDate = d.ActualPaymentDate,
                             Committer = d.Committer,
                             CommitterContect = d.Committer,
                             CommitterEmail = d.CommitterEmail,
                             ExpectedPaymentDate = d.ExpectedPaymentDate,
                             ID = d.ID,
                             Income = d.Income,
                             IsClosed = d.IsClosed,
                             PackageNameCH = d.Package.Name_CH,
                             PackageNameEN = d.Package.Name_EN,
                             Payment = d.Payment,
                             Currency = d.Currencytype.Name,
                             PaymentDetail = d.PaymentDetail,
                             Sales = d.Sales,
                             DealType = d.DealType,
                             ParticipantTypeName=d.Package.ParticipantType.Name_EN,
                             Poll=d.Poll,
                             ProjectCode = d.Project.ProjectCode,
                             SignDate = d.SignDate,
                             TicketDescription = d.TicketDescription,
                             IsConfirm = (d.IsConfirm == true ? "是" : "否"),
                             ModifiedDate = d.ModifiedDate,
                             Role = emprole.Where(w => w.AccountName == d.Sales).FirstOrDefault() == null ? "" : emprole.Where(w => w.AccountName == d.Sales).FirstOrDefault().Role.Name
                         };
                return ds.OrderByDescending(o => o.SignDate);
            }
            else if (Employee.CurrentRole.Level == 3) //会务确定出单
            {
                var user = Employee.CurrentUserName.Trim();
                var pids = new List<int>();
                foreach (var c in CH.DB.Projects.Where(w => w.IsActived))
                {
                    if (!string.IsNullOrEmpty(c.Conference))
                    {
                        var names = c.Conference.Trim().Split(new string[] { ";", "；" }, StringSplitOptions.RemoveEmptyEntries);
                        if (names.Contains(user))
                        {
                            pids.Add(c.ID);
                        }
                    }
                }
              
                dllist = deals.Where(w => pids.Contains((int)w.ProjectID)).ToList();
                var ds = from d in dllist
                         select new AjaxViewDeal
                         {
                             CompanyNameEN = d.CompanyRelationship.Company.Name_EN,
                             CompanyNameCH = d.CompanyRelationship.Company.Name_CH,
                             DealCode = d.DealCode,
                             Abandoned = d.Abandoned,
                             AbandonReason = d.AbandonReason,
                             ActualPaymentDate = d.ActualPaymentDate,
                             Committer = d.Committer,
                             CommitterContect = d.Committer,
                             CommitterEmail = "",
                             ExpectedPaymentDate = d.ExpectedPaymentDate,
                             ID = d.ID,
                             Income = d.Income,
                             IsClosed = d.IsClosed,
                             PackageNameCH = d.Package.Name_CH,
                             PackageNameEN = d.Package.Name_EN,
                             Payment = d.Payment,
                             Currency = d.Currencytype.Name,
                             PaymentDetail = d.PaymentDetail,
                             Sales = d.Sales,
                             ProjectCode = d.Project.ProjectCode,
                             SignDate = d.SignDate,
                             TicketDescription = d.TicketDescription,
                             IsConfirm = (d.IsConfirm == true ? "是" : "否"),
                             ModifiedDate = d.ModifiedDate,
                         };
                return ds.OrderByDescending(o => o.SignDate);
            }
            else//板块修改deal
            {
                List<int> idlist = new List<int>();
                foreach (var p in CH.DB.Projects.Where(w => w.IsActived == true && w.Manager != null))
                {
                    var names = p.Manager.Trim().Split(new string[] { ";", "；" }, StringSplitOptions.RemoveEmptyEntries);
                    if (names.Contains(Employee.CurrentUserName))
                    {
                        if (!idlist.Contains(p.ID))
                            idlist.Add(p.ID);
                    }
                }
                dllist = deals.Where(d => idlist.Contains((int)d.ProjectID)).ToList();
                var ds = from d in dllist
                         select new AjaxViewDeal
                         {
                             CompanyNameEN = d.CompanyRelationship.Company.Name_EN,
                             CompanyNameCH = d.CompanyRelationship.Company.Name_CH,
                             DealCode = d.DealCode,
                             Abandoned = d.Abandoned,
                             AbandonReason = d.AbandonReason,
                             ActualPaymentDate = d.ActualPaymentDate,
                             Committer = d.Committer,
                             CommitterContect = d.Committer,
                             CommitterEmail = d.CommitterEmail,
                             ExpectedPaymentDate = d.ExpectedPaymentDate,
                             ID = d.ID,
                             Income = d.Income,
                             IsClosed = d.IsClosed,
                             PackageNameCH = d.Package.Name_CH,
                             PackageNameEN = d.Package.Name_EN,
                             Payment = d.Payment,
                             Currency = d.Currencytype.Name,
                             PaymentDetail = d.PaymentDetail,
                             Sales = d.Sales,
                             ProjectCode = d.Project.ProjectCode,
                             SignDate = d.SignDate,
                             TicketDescription = d.TicketDescription,
                             IsConfirm = (d.IsConfirm == true ? "是" : "否"),
                             ModifiedDate = d.ModifiedDate,
                         };
                return ds.OrderByDescending(o => o.SignDate);
            }
        }

        [HttpPost]
        public ActionResult ConfirmTemp(Deal item, int? projectid)
        {
            ViewBag.ProjectID = projectid;
            ViewBag.CompanyRelationshipID = item.CompanyRelationshipID;
            if (ModelState.IsValid)
            {
                CH.Edit<Deal>(item);
                return RedirectToAction("ConfirmList", "Deal", new { projectid = projectid });
            }
            return View(item);
        }

        public ActionResult ConfirmTemp(int id)
        {
            var item = CH.GetDataById<Deal>(id);
            ViewBag.ProjectID = item.CompanyRelationship.ProjectID;
            ViewBag.CompanyRelationshipID = item.CompanyRelationshipID;
            return View(item);
        }

        [GridAction]
        public ActionResult _SelectAjaxParticipant(int? id)
        {
            List<AjaxParticipant> pList = new List<AjaxParticipant>();
            if (id != null)
            {
                pList = (from s in CH.GetAllData<Participant>()
                         where s.DealID == id
                         select new AjaxParticipant
                         {
                             ID = s.ID,
                             Name = s.Name,
                             Title = s.Title,
                             Gender = s.Gender,
                             Mobile = s.Mobile,
                             Contact = s.Contact,
                             Email = s.Email,
                             ParticipantTypeName = s.ParticipantType.Name,
                             ParticipantTypeID = s.ParticipantTypeID,
                             DealID = s.DealID,
                             ProjectID = s.ProjectID,
                             CancelAttended = s.CancelAttended,

                         }).ToList();
            }
            Session["pList"] = pList;
            return View(new GridModel(pList));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _UpdateAjaxParticipant([Bind(Prefix = "inserted")]IEnumerable<AjaxParticipant> insertedP,
            [Bind(Prefix = "updated")]IEnumerable<AjaxParticipant> updatedP,
            [Bind(Prefix = "deleted")]IEnumerable<AjaxParticipant> deletedP)
        {
            List<AjaxParticipant> pList = new List<AjaxParticipant>();
            if (Session["pList"] != null)
            {
                pList = Session["pList"] as List<AjaxParticipant>;
            }
            if (ModelState.IsValid)
            {
                if (insertedP != null)
                {
                    foreach (var p in insertedP)
                    {
                        if (p.Name != "" && p.ParticipantTypeName != null)
                        {
                            var partType = CH.GetAllData<ParticipantType>().Where(pt => pt.Name == p.ParticipantTypeName).FirstOrDefault();

                            pList.Add(p);

                            Participant s = new Participant();
                            s.Name = string.IsNullOrEmpty(p.Name) ? "" : p.Name.Trim();
                            s.Title = string.IsNullOrEmpty(p.Title) ? "" : p.Title.Trim();
                            s.Gender = string.IsNullOrEmpty(p.Gender) ? "" : p.Gender.Trim();
                            s.Mobile = string.IsNullOrEmpty(p.Mobile) ? "" : p.Mobile.Trim();
                            s.Contact = string.IsNullOrEmpty(p.Contact) ? "" : p.Contact.Trim();
                            s.Email = string.IsNullOrEmpty(p.Email) ? "" : p.Email.Trim();
                            s.ParticipantTypeID = partType.ID;
                            s.CancelAttended = p.CancelAttended;
                            s.ProjectID = Convert.ToInt32(Session["ProjectID"].ToString());
                            s.DealID = Convert.ToInt32(Session["DealID"].ToString());


                            //s.ProjectID = p.ProjectID;
                            //s.DealID = p.DealID;
                            if (s.ID == 0)
                            {
                                CH.Create<Participant>(s);
                            }
                        }
                    }
                }

                if (deletedP != null)
                {
                    foreach (var p in deletedP)
                    {
                        int index = pList.FindIndex(ap => ap.Name == p.Name && ap.ParticipantTypeName == p.ParticipantTypeName);
                        if (index != -1)
                        {
                            pList.RemoveAt(index);
                            CH.Delete<Participant>(p.ID);
                        }
                    }
                }

                if (updatedP != null)
                {
                    foreach (var p in updatedP)
                    {
                        var partType = CH.GetAllData<ParticipantType>().Where(pt => pt.Name == p.ParticipantTypeName).FirstOrDefault();
                        AjaxParticipant a = pList.Find(s => s.ID == p.ID);
                        a.Name = string.IsNullOrEmpty(p.Name) ? "" : p.Name.Trim();
                        a.Title = string.IsNullOrEmpty(p.Title) ? "" : p.Title.Trim();
                        a.Gender = string.IsNullOrEmpty(p.Gender) ? "" : p.Gender.Trim();
                        a.Mobile = string.IsNullOrEmpty(p.Mobile) ? "" : p.Mobile.Trim();
                        a.Contact = string.IsNullOrEmpty(p.Contact) ? "" : p.Contact.Trim();
                        a.Email = string.IsNullOrEmpty(p.Email) ? "" : p.Email.Trim();
                        a.ParticipantTypeName = p.ParticipantTypeName;
                        a.ParticipantTypeID = p.ParticipantTypeID;
                        a.ProjectID = p.ProjectID;
                        a.DealID = p.DealID;
                        a.CancelAttended = p.CancelAttended;
                        Participant sp = CH.GetDataById<Participant>(p.ID);
                        sp.Name = string.IsNullOrEmpty(p.Name) ? "" : p.Name.Trim();
                        sp.Title = string.IsNullOrEmpty(p.Title) ? "" : p.Title.Trim();
                        sp.Gender = string.IsNullOrEmpty(p.Gender) ? "" : p.Gender.Trim();
                        sp.Mobile = string.IsNullOrEmpty(p.Mobile) ? "" : p.Mobile.Trim();
                        sp.Contact = string.IsNullOrEmpty(p.Contact) ? "" : p.Contact.Trim();
                        sp.Email = string.IsNullOrEmpty(p.Email) ? "" : p.Email.Trim();
                        sp.ParticipantTypeID = partType.ID;
                        sp.ProjectID = p.ProjectID;
                        sp.DealID = p.DealID;
                        sp.CancelAttended = p.CancelAttended;
                        CH.Edit<Participant>(sp);
                    }
                }
            }
            return View(new GridModel(pList));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _UpdateAjaxParticipantNew([Bind(Prefix = "inserted")]IEnumerable<AjaxParticipant> insertedP,
            [Bind(Prefix = "updated")]IEnumerable<AjaxParticipant> updatedP,
            [Bind(Prefix = "deleted")]IEnumerable<AjaxParticipant> deletedP)
        {
            List<AjaxParticipant> pList = new List<AjaxParticipant>();
            if (Session["pList"] != null)
            {
                pList = Session["pList"] as List<AjaxParticipant>;
            }
            if (ModelState.IsValid)
            {
                if (insertedP != null)
                {
                    foreach (var p in insertedP)
                    {
                        if (p.ParticipantTypeID != null)
                        {
                            //var partType = CH.GetAllData<ParticipantType>().Where(pt => pt.Name == p.ParticipantTypeName).FirstOrDefault();

                            pList.Add(p);

                            Participant s = new Participant();
                            s.Name = string.IsNullOrEmpty(p.Name) ? "" : p.Name.Trim();
                            s.Title = string.IsNullOrEmpty(p.Title) ? "" : p.Title.Trim();
                            s.Gender = string.IsNullOrEmpty(p.Gender) ? "" : p.Gender.Trim();
                            s.Mobile = string.IsNullOrEmpty(p.Mobile) ? "" : p.Mobile.Trim();
                            s.Contact = string.IsNullOrEmpty(p.Contact) ? "" : p.Contact.Trim();
                            s.Email = string.IsNullOrEmpty(p.Email) ? "" : p.Email.Trim();
                            s.ParticipantTypeID = p.ParticipantTypeID;
                            s.CancelAttended = p.CancelAttended;
                            s.ProjectID = Convert.ToInt32(Session["ProjectID"].ToString());
                            s.DealID = Convert.ToInt32(Session["DealID"].ToString());


                            //s.ProjectID = p.ProjectID;
                            //s.DealID = p.DealID;
                            if (s.ID == 0)
                            {
                                CH.Create<Participant>(s);
                            }
                        }
                    }
                }

                if (deletedP != null)
                {
                    foreach (var p in deletedP)
                    {
                        int index = pList.FindIndex(ap => ap.Name == p.Name && ap.ParticipantTypeName == p.ParticipantTypeName);
                        if (index != -1)
                        {
                            pList.RemoveAt(index);
                            CH.Delete<Participant>(p.ID);
                        }
                    }
                }

                if (updatedP != null)
                {
                    foreach (var p in updatedP)
                    {
                        //var partType = CH.GetAllData<ParticipantType>().Where(pt => pt.Name == p.ParticipantTypeName).FirstOrDefault();
                        AjaxParticipant a = pList.Find(s => s.ID == p.ID);
                        a.Name = string.IsNullOrEmpty(p.Name) ? "" : p.Name.Trim();
                        a.Title = string.IsNullOrEmpty(p.Title) ? "" : p.Title.Trim();
                        a.Gender = string.IsNullOrEmpty(p.Gender) ? "" : p.Gender.Trim();
                        a.Mobile = string.IsNullOrEmpty(p.Mobile) ? "" : p.Mobile.Trim();
                        a.Contact = string.IsNullOrEmpty(p.Contact) ? "" : p.Contact.Trim();
                        a.Email = string.IsNullOrEmpty(p.Email) ? "" : p.Email.Trim();
                        //a.ParticipantTypeName = p.ParticipantTypeName;
                        a.ParticipantTypeID = p.ParticipantTypeID;
                        a.ProjectID = p.ProjectID;
                        a.DealID = p.DealID;
                        a.CancelAttended = p.CancelAttended;
                        Participant sp = CH.GetDataById<Participant>(p.ID);
                        sp.Name = string.IsNullOrEmpty(p.Name) ? "" : p.Name.Trim();
                        sp.Title = string.IsNullOrEmpty(p.Title) ? "" : p.Title.Trim();
                        sp.Gender = string.IsNullOrEmpty(p.Gender) ? "" : p.Gender.Trim();
                        sp.Mobile = string.IsNullOrEmpty(p.Mobile) ? "" : p.Mobile.Trim();
                        sp.Contact = string.IsNullOrEmpty(p.Contact) ? "" : p.Contact.Trim();
                        sp.Email = string.IsNullOrEmpty(p.Email) ? "" : p.Email.Trim();
                        sp.ParticipantTypeID = p.ParticipantTypeID;
                        sp.ProjectID = p.ProjectID;
                        sp.DealID = p.DealID;
                        sp.CancelAttended = p.CancelAttended;
                        CH.Edit<Participant>(sp);
                    }
                }
            }
            return View(new GridModel(pList));
        }
        public ActionResult PostID(int? id)
        {
            Deal d = CH.GetDataById<Deal>(id);
            Session["DealID"] = d.ID;
            Session["ProjectID"] = d.ProjectID;
            return PartialView("Confirm", new AjaxViewDeal()
            {
                CompanyNameEN = d.CompanyRelationship.Company.Name_EN,
                CompanyNameCH = d.CompanyRelationship.Company.Name_CH,
                DealCode = d.DealCode,
                Abandoned = d.Abandoned,
                AbandonReason = d.AbandonReason,
                ActualPaymentDate = d.ActualPaymentDate,
                Committer = d.Committer,
                CommitterContect = d.Committer,
                CommitterEmail = d.CommitterEmail,
                ExpectedPaymentDate = d.ExpectedPaymentDate,
                ID = d.ID,
                Income = d.Income,
                IsClosed = d.IsClosed,
                PackageNameCH = d.Package.Name_CH,
                PackageNameEN = d.Package.Name_EN,
                Payment = d.Payment,
                Currency = d.Currencytype.Name,
                PaymentDetail = d.PaymentDetail,
                Sales = d.Sales,
                ProjectCode = d.Project.ProjectCode,
                SignDate = d.SignDate,
                TicketDescription = d.TicketDescription,
                IsConfirm = (d.IsConfirm == true ? "是" : "否"),
                ModifiedDate = d.ModifiedDate,
            });
        }

        [HttpPost]
        public ActionResult SaveAjaxConfirm(AjaxViewDeal newData)
        {
            if (Employee.CurrentRole.Level == 4)
            {
                var item = CH.GetDataById<Deal>(newData.ID);
                item.Income = newData.Income;
                item.ActualPaymentDate = newData.ActualPaymentDate;
                item.Abandoned = newData.Abandoned;
                item.AbandonReason = newData.AbandonReason;
                item.Remark = newData.Remark;
                CH.Edit<Deal>(item);
                PreCommByProSales((DateTime)item.ActualPaymentDate, item.Sales,(int) item.ProjectID);

            }
            else if (Employee.CurrentRole.Level == 3)
            {
                var item = CH.GetDataById<Deal>(newData.ID);
                if (item.IsConfirm != true)
                {
                    item.IsConfirm = true;
                    item.Confirmor = Employee.CurrentUserName;
                    //item.Income = newData.Income;
                    //item.ActualPaymentDate = newData.ActualPaymentDate;

                    item.Payment = newData.Payment;
                    item.TicketDescription = newData.TicketDescription;

                    item.Abandoned = newData.Abandoned;
                    item.AbandonReason = newData.AbandonReason;
                    CH.Edit<Deal>(item);
                }
                else
                {
                    item.Payment = newData.Payment;
                    item.TicketDescription = newData.TicketDescription;
                    CH.Edit<Deal>(item);
                }
            }
            else
            {
                var item = CH.GetDataById<Deal>(newData.ID);
                item.Payment = newData.Payment;
                CH.Edit<Deal>(item);
            }
            return Json(new { dealName = newData.DealCode });
        }
        private void PreCommByProSales(DateTime ActualPaymentDate, string sale, int projectid)
        {
            var year = ActualPaymentDate.Year;
            var month = ActualPaymentDate.Month;
            var startdate = new DateTime(year, month, 1);
            var enddate = startdate.AddMonths(1).AddDays(-1);
            var deals = from d in CH.DB.Deals.Where(o => o.ProjectID == projectid && o.Abandoned == false && o.Income > 0 &&
                o.ActualPaymentDate.Value.Month == ActualPaymentDate.Month && o.ActualPaymentDate.Value.Year == year && o.Sales == sale)
                        select d;
            var username = sale;
            var emps = CH.DB.EmployeeRoles.Where(w => w.AccountName == username);
            var displayname = emps.Select(s => s.AccountNameCN).FirstOrDefault();
            var roleid = emps.Select(s => s.RoleID).FirstOrDefault();
            var projects = from p in deals
                           group p by new { p.Project.ProjectCode } into grp
                           select new { projectcode = grp.Key.ProjectCode };
            var proname = "";
            foreach (var name in projects)
            {
                proname = proname + name.projectcode + ",";
            }
            proname = proname.TrimEnd(',');
            string inout = "海外";
            if (roleid != null)
            {
                var name = CH.GetDataById<Role>(roleid).Name;
                if (name.Contains("国内"))
                    inout = "国内";
            }
            decimal standard = 3000;
            var sponsorrate = 5;
            var delegaterate = 5;
            var lps = new PreCommission()
            {
                ProjectID = projectid,
                Income = deals.Sum(s => (decimal?)s.Income),
                TargetNameEN = sale,
                TargetNameCN = displayname,
                InOut = inout,
                SponsorIncome = deals.Where(w => w.Poll == 0).Sum(s => (decimal?)s.Income),
                DelegateIncome = inout == "海外" ? deals.Where(w => w.Poll > 0).Sum(s => (decimal?)s.Income) : 0,
                SponsorRate=sponsorrate,
                SponsorCommission = deals.Where(w => w.Poll == 0).Sum(s => (decimal?)s.Income) * sponsorrate / 100,
                DelegateRate=delegaterate,
                DelegateCommission = (inout == "海外" ? deals.Where(w => w.Poll > 0).Sum(s => (decimal?)s.Income) : 0) * delegaterate / 100,

            };
            if(lps.SponsorIncome==null)
                lps.SponsorIncome=0;
            else
                lps.SponsorIncome=(decimal)Math.Round((decimal)lps.SponsorIncome);
            if (lps.DelegateIncome == null)
                lps.DelegateIncome = 0;
            else
                lps.DelegateIncome = (decimal)Math.Round((decimal)lps.DelegateIncome);
            if (lps.SponsorCommission == null)
                lps.SponsorCommission = 0;
            else
                lps.SponsorCommission = (decimal)Math.Round((decimal)lps.SponsorCommission);
            if (lps.DelegateCommission == null)
                lps.DelegateCommission = 0;
            else
                lps.DelegateCommission = (decimal)Math.Round((decimal)lps.DelegateCommission);
            lps.Income = lps.DelegateIncome + lps.SponsorIncome;
            lps.Commission = lps.SponsorCommission + lps.DelegateCommission;
            if (lps.Commission > 3500)
            {
                lps.Tax = (decimal)Math.Round((((double)lps.Commission - 3500) * 0.07));
            }
            else
                lps.Tax = 0;
            lps.TotalCommission = lps.ActualCommission = lps.Commission - lps.Tax;
            var procode = CH.GetDataById<Project>(projectid).ProjectCode;
            var CommID = sale + year.ToString() + ActualPaymentDate.Month.ToString().PadLeft(2, '0');
            lps.CommID = CommID;
            lps.StartDate = startdate;
            lps.EndDate = enddate;
            var precommission = CH.DB.PreCommissions.Where(x => x.CommID == CommID && x.ProjectID==projectid).FirstOrDefault();
            if (precommission != null)
            {
                CH.Delete<PreCommission>(precommission.ID);
            }
            lps.Creator = "system";
            lps.CreatedDate = DateTime.Now;
            //CH.Create<PreCommission>(lps);
            CH.DB.Set<PreCommission>().Add(lps);
            CH.DB.SaveChanges();
            //DB.Set<PreCommission>().Add(lps);
            //DB.SaveChanges();
        }
    }
}