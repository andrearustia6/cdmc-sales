using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using Utl;
using Entity;

namespace Sales.Controllers
{
    public class MasterSheetController : Controller
    {
     

        public ActionResult Index()
        {
            return View();
        }

        #region Participants

         [GridAction]
        public ActionResult _SelectParticipantAjaxIndex(int? conferenceid)
        {
            var data = GetParticipant();
            return View(new GridModel(data));
        }

        IQueryable<AjaxParticipant> GetParticipant()
        {
            var data = from p in CH.DB.Participants
                       select new AjaxParticipant()
                       {
                           Address = p.Address,
                           Name = p.Name,
                           Contact = p.Contact,
                           Title = p.Title,
                           Company = p.Deal.CompanyRelationship.Company.Name_CH !=null? p.Deal.CompanyRelationship.Company.Name_CH:p.Deal.CompanyRelationship.Company.Name_EN,
                           Gender = p.Gender,
                           ID = p.ID,
                           Email = p.Email,
                           ProjectCode = p.Project.ProjectCode,
                           DealCode = p.Deal.DealCode,
                       };

            return data;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveParticipantAjaxEditing(int participantid)
        {


            var item = CH.GetDataById<Participant>(participantid);
            if (TryUpdateModel(item))
            {
                CH.Edit<Participant>(item);
            }
            return View(new GridModel(GetParticipant()));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _InsertParticipantAjaxEditing(int participantid)
        {
            var item = new Participant();
            
            if (TryUpdateModel(item))
            {
                CH.Create<Participant>(item);
            }
            return View(new GridModel(GetParticipant()));
        }

       
        #endregion

    }
}
