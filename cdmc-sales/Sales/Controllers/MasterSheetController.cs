using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using Utl;
using Entity;
using System.IO;
using Sales.Model;

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

        
        [GridAction]
        public ActionResult _SelectSpeakerIndex(int? conferenceid)
        {
            var data = GetSpeakers(conferenceid);
            return View(new GridModel(data));
        }

        public static IQueryable<_Speaker> GetSpeakers(int? conferenceid = null)
        {
            var data = from c in CH.PDDB.Speakers
                       select new _Speaker
                       {
                           CreatedDate = c.CreatedDate,
                           Creator = c.Creator,
                           Description = c.Description,
                           ID = c.ID,
                           ModifiedDate = c.ModifiedDate,
                           ModifiedUser = c.ModifiedUser,
                           Name = c.Name,
                           Sequence = c.Sequence,
                           Title = c.Title,
                           Company = c.Company,
                           ConferenceID = c.ConferenceID,
                           Content = c.Content,
                           ContentDescription = c.ContentDescription,
                           Profile = c.Profile,
                           ImgPath = c.ImgPath,
                           ClientDurationTypeID = c.ClientDurationTypeID,
                           CategoryID = c.CategoryID,
                           IsVIP = c.IsVIP,
                           ConfirmedAttend = c.ConfirmedAttend,
                           CommunicationRecord = c.CommunicationRecord,
                           DraftCase = c.DraftCase,
                           Email = c.Email,
                           Fax = c.Fax,
                           Importance = c.Importance,
                           InstitutionalNature = c.InstitutionalNature,
                           Mobile = c.Mobile,
                           NewsWebSite = c.NewsWebSite,
                           NoteInformation = c.NoteInformation,
                           Phone = c.Phone,
                           RoyaltiesReference = c.RoyaltiesReference,
                           WebSite = c.WebSite,
                           Address = c.Address,
                           Assistant = c.Assistant
                       };
            if (conferenceid != null)
                data = data.Where(w => w.ConferenceID == conferenceid).OrderByDescending(s => s.Sequence);
            return data;

        }
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _InsertSpeakerAjaxEditing()
        {
            var item = new Speaker();

            if (TryUpdateModel(item))
            {
                //HttpPostedFileBase file = Request.Files["UploadPhoto"];
                //int i = ImageController.SaveImage(file, item.UploadPhotoID);
                //if (i != 0)
                //{
                //    item.UploadPhotoID = i;
                //}

                //HttpPostedFileBase file = Request.Files["UploadPhoto"];
                //if (file != null)
                //{
                //    var fileName = Path.GetFileName(file.FileName);
                //    var code = CH.GetDataById<Conferences>(item.ConferenceID).ProjectCode;
                //    string serverpath = "/Uploads/Conferences/" + code;
                //    string path = Server.MapPath(serverpath);
                //    if (!Directory.Exists(path))
                //    {
                //        Directory.CreateDirectory(path);
                //    }
                //    var physicalPath = Path.Combine(path, fileName);
                //    file.SaveAs(physicalPath);
                //    item.ImgPath = serverpath + "/" + fileName;
                //}
                CH.PDCreate<Speaker>(item);
            }
            return Json(new { ConferenceID = item.ConferenceID});
        }
        [AcceptVerbs(HttpVerbs.Post)]
        
        public ActionResult _SaveSpeakerAjaxEditing(int id)
        {
            var item = CH.GetPDDataById<Speaker>(id);
            if (TryUpdateModel(item))
            {
                //HttpPostedFileBase file = Request.Files["UploadPhoto"];
                //int i = ImageController.SaveImage(file, item.UploadPhotoID);
                //if (i != 0)
                //{
                //    item.UploadPhotoID = i;
                //}

                //HttpPostedFileBase file = Request.Files["UploadPhoto"];
                //if (file != null)
                //{
                //    var fileName = Path.GetFileName(file.FileName);
                //    string serverpath = "/Uploads/Conferences/" + item.Conferences.ProjectCode;
                //    string path = Server.MapPath(serverpath);
                //    if (!Directory.Exists(path))
                //    {
                //        Directory.CreateDirectory(path);
                //    }
                //    var physicalPath = Path.Combine(path, fileName);
                //    file.SaveAs(physicalPath);
                //    item.ImgPath = serverpath + "/" + fileName;
                //}
                CH.PDEdit<Speaker>(item);
                
            }
            return Json(new { ConferenceID = item.ConferenceID });
        }

        [GridAction]
        public ActionResult _SelectOrganizationIndex(int? conferenceid)
        {
            return View(new GridModel(GetOrganizations(conferenceid)));
        }

        public static IQueryable<_Organization> GetOrganizations(int? conferenceid = null)
        {
            var data = from c in CH.PDDB.Organizations
                       select new _Organization
                       {
                           CreatedDate = c.CreatedDate,
                           Creator = c.Creator,
                           Description = c.Description,
                           ID = c.ID,
                           ModifiedDate = c.ModifiedDate,
                           ModifiedUser = c.ModifiedUser,
                           Sequence = c.Sequence,
                           ConferenceID = c.ConferenceID,

                           OrgName = c.OrgName,
                           Contact = c.Contact,
                           ContactPerson = c.ContactPerson,
                           Email = c.Email,
                           OrgType = c.OrgType,
                           Owner = c.Owner,
                           OrgForm = c.OrgForm,
                           ImgPath = c.ImgPath,
                           Mobile = c.Mobile
                       };
            if (conferenceid != null)
                data = data.Where(w => w.ConferenceID == conferenceid).OrderByDescending(s => s.Sequence);
            return data;

        }

        [AcceptVerbs(HttpVerbs.Post)]
        
        public ActionResult _InsertOrganizationAjaxEditing()
        {
            var item = new Organization();

            if (TryUpdateModel(item))
            {
                
                CH.PDCreate<Organization>(item);
            }
            return Json(new { ConferenceID = item.ConferenceID });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult _SaveOrganizationAjaxEditing(int id)
        {
            var item = CH.GetPDDataById<Organization>(id);
            if (TryUpdateModel(item))
            {
                
                CH.PDEdit<Organization>(item);
            }
            return Json(new { ConferenceID = item.ConferenceID });
        }
    }
}
