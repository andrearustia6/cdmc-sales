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
        public ViewResult IndexEdit(List<int> selectedprojects, string selecttype)
        {
            ViewBag.SelectedProjects = selectedprojects;
            selectedprojects = this.TryGetSelectedProjectIDs(selectedprojects);
            ViewBag.Right = EditRight.DealsEdit.ToString();
            ViewBag.SelectType = selecttype;
            return View("index");
        }

        public ViewResult IndexReview(List<int> selectedprojects, string selecttype)
        {
            ViewBag.SelectedProjects = selectedprojects;
            selectedprojects = this.TryGetSelectedProjectIDs(selectedprojects);
            ViewBag.Right = ReviewRight.DealsReview.ToString();
            ViewBag.SelectType = selecttype;
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
        public ActionResult _Index(string sp)
        {
            var selectedprojects = Utl.Utl.ConvertStringToSelectProjectID(sp);
            selectedprojects = this.TryGetSelectedProjectIDs(selectedprojects);
            var ds = from d in CH.DB.Deals
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

        public ViewResult ConfirmList(int? projectid)
        {
            //if (Employee.CurrentRole.Level == 500)
            //{
            //    projectid = this.TrySetProjectIDForUser(projectid);
            //    ViewBag.ProjectID = projectid;
            //    var data = CH.GetAllData<Deal>();
            //    ViewBag.ProjectID = projectid;
            //    return View(data.FindAll(d => d.CompanyRelationship.ProjectID == projectid).OrderByDescending(m => m.CreatedDate).ToList());
            //}
            //else if (Employee.CurrentRole.Level == 4)
            //{
            //    return View(CH.DB.Deals.Where(s => s.IsConfirm == true).OrderByDescending(s => s.CreatedDate).ToList());
            //}
            //else
            //{
            //    return View();
            //}
            return View();
        }

        [GridAction]
        public ActionResult _SelectIndex()
        {
            return View(new GridModel(getData()));
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
                CH.Edit<Deal>(item);
            }
            else
            {
                var item = CH.GetDataById<Deal>(id);
                item.IsConfirm = true;
                CH.Edit<Deal>(item);
            }
            return View(new GridModel(getData()));
        }

        private List<AjaxViewDeal> getData()
        {
            if (Employee.CurrentRole.Level == 4)//财务填写income
            {
                var ds = from d in CH.DB.Deals
                         where d.Abandoned == false && d.IsConfirm == true
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
                             PaymentDetail = d.PaymentDetail,
                             Sales = d.Sales,
                             ProjectCode = d.Project.ProjectCode,
                             SignDate = d.SignDate,
                             TicketDescription = d.TicketDescription,
                             IsConfirm = (d.IsConfirm == true ? "是" : "否")
                         };
                return ds.OrderBy(o => o.SignDate).ToList();
            }
            else//会务确定出单
            {
                var ds = from d in CH.DB.Deals
                         where d.Abandoned == false && d.IsConfirm != true
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
                             PaymentDetail = d.PaymentDetail,
                             Sales = d.Sales,
                             ProjectCode = d.Project.ProjectCode,
                             SignDate = d.SignDate,
                             TicketDescription = d.TicketDescription,
                             IsConfirm = (d.IsConfirm == true ? "是" : "否")
                         };
                return ds.OrderBy(o => o.SignDate).ToList();
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
    }
}