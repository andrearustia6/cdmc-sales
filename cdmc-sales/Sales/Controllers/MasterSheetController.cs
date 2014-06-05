﻿using System;
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


        public ActionResult Index(int? projectid)
        {
            ViewBag.ProjectID = projectid;
            return View();
        }

        #region Participants

         [GridAction]
        public ActionResult _SelectParticipantAjaxIndex(int? projectid,int? dealid)
        {
            var data = GetParticipant(projectid, dealid);
            return View(new GridModel(data));
        }

         IQueryable<AjaxParticipant> GetParticipant(int? projectid, int? dealid)
        {

            var data = CH.DB.Participants.Where(x=>x.ProjectID==projectid);
            if(dealid!=null)
                data = data.Where(x=>x.DealID==dealid) ;
            var ret=from p in data
                       
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
                           Mobile=p.Mobile,
                           ParticipantTypeName = p.ParticipantType.Name_CH + " | " + p.ParticipantType.Name_EN,
                       };

            return ret;
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
            return View(new GridModel(GetParticipant(item.ProjectID, item.DealID)));
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
            return View(new GridModel(GetParticipant(item.ProjectID,item.DealID)));
        }

       
        #endregion
        public ActionResult GetDealId(int id)
        {
            List<Deal> deals = CH.DB.Deals.Where(x=>x.ProjectID==id && x.DealCode!=null).ToList();
            var ret = from p in deals
                      select new Deal
                      {
                          ID = p.ID,
                          DealCode = p.DealCode
                      };
            return Json(ret);
        }
        public PartialViewResult GetEditParticipant(int? id)
        {
            var p = CH.GetDataById<Participant>(id);
            var ret = new AjaxParticipantForDeal();
            ret.ID = p.ID;
            ret.Name = p.Name;
            ret.Email = p.Email;
            ret.Contact = p.Contact;
            ret.Mobile = p.Mobile;
            ret.Title = p.Title;
            ret.ParticipantTypeID = p.ParticipantTypeID;
            ret.ZIP = p.ZIP;
            ret.Address = p.Address;
            ret.Gender = p.Gender;
            ret.CancelAttended = p.CancelAttended;
            return PartialView("EditParticipant", ret);
        }
        [HttpPost]
        public ActionResult SaveParticipant(AjaxParticipantForDeal item)
        {
            //int dealid = 1;
            Participant p = CH.GetDataById<Participant>(item.ID);
            p.Name = item.Name;
            p.Title = item.Title;
            p.Gender = item.Gender;
            p.Mobile = item.Mobile;
            p.Contact = item.Contact;
            p.Email = item.Email;
            p.ZIP = item.ZIP;
            p.Address = item.Address;
            p.ParticipantTypeID = item.ParticipantTypeID;
            CH.Edit<Participant>(p);
            return Json(new { id = p.ID });
        }
    }
}
