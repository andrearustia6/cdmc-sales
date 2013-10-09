using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Entity;
using Sales;
using Utl;
using BLL;
using Telerik.Web.Mvc;
using System.IO;
using System.Data.Entity.Infrastructure;
using Telerik.Web.Mvc.UI;
using Sales.Model;
using Model;

namespace Sales.Controllers
{
    public class ProjectReviewController : Controller
    {
        protected override void Dispose(bool disposing)
        {
            CH.DB.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult Index()
        {
            List<SelectListItem> list = new List<SelectListItem> { };
            foreach (var x in CH.DB.Projects.ToList())
            {
                list.Add(new SelectListItem { Text = x.Name, Value = x.ID.ToString() });
            }
            ViewBag.projectlist = list;

            //List<ProjectReview> result = CH.DB.ProjectReviews.OrderByDescending(x => x.ModifiedDate).ToList();
            return View();
        }

        [GridAction]
        public ActionResult _SelectAjaxEditing()
        {
            return View(new GridModel(GetData()));
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(FormCollection c)
        {
            try
            {
                int id = int.Parse(c["id"]);
                string summary = Server.UrlDecode(c["summary"]);
                ProjectReview pr = new ProjectReview();
                pr.ProjectID = id;
                pr.Summary = summary;
                CH.Create<ProjectReview>(pr);
            }
            catch (Exception e)
            { }
            return Json("");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _DeleteAjaxEditing(int id)
        {
            CH.Delete<ProjectReview>(id);
            return View(new GridModel(GetData()));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _UpdateAjaxEditing(int id)
        {
            ProjectReview pr = CH.DB.ProjectReviews.Find(id);
            if (TryUpdateModel(pr))
            {
                CH.Edit(pr);
            }

            return View(new GridModel(GetData()));
        }

        private List<_ProjectReview> GetData()
        {
            var result = (from x in CH.DB.ProjectReviews
                          select new _ProjectReview
                          {
                              ID = x.ID,
                              ProjectID = x.ProjectID,
                              Summary = x.Summary,
                              ModifiedUser = x.ModifiedUser == null ? x.Creator : x.ModifiedUser,
                              ModifiedDate = x.ModifiedDate == null ? x.CreatedDate : x.ModifiedDate
                          }).OrderByDescending(x => x.ID).ToList();

            result.ForEach(x =>
            {
                if (x.ProjectID != null)
                {
                    var item = CH.DB.Projects.Find(x.ProjectID);
                    if (item != null)
                    {
                        x.ProjectName = item.Name;
                        x.ProjectType = item.ProjectType != null ? item.ProjectType.Name : "";

                        List<Deal> ds = (from d in CH.DB.Deals where d.ProjectID == item.ID select d).ToList();
                        List<LeadCall> lcs = (from d in CH.DB.LeadCalls where d.MemberID == item.ID select d).ToList();
                        //List<CompanyRelationship> crs = (from d in CH.DB.CompanyRelationships where d.ProjectID == item.ID select d).ToList();

                        x.CallCount = lcs.Count();
                        x.FaxOutCount = lcs.Select(l => l.LeadID).Distinct().Count();

                        x.ConCount = (from d in CH.DB.CompanyRelationships where d.ProjectID == item.ID select d).Count();
                        //x.ConCount = crs.Count();
                        x.delegatecount = (from d in CH.DB.Deals
                                           join a in CH.DB.Packages on d.PackageID equals a.ID
                                           join b in CH.DB.PackageTypes on a.PackageTypeID equals b.ID
                                           where b.Name_EN.ToLower() == "delegate"
                                           select d).Count();
                        //x.delegatecount = ds.Where(l => l.Package.ParticipantType.Name_EN.ToLower() == "delegate").Count();  <<<<速度太慢
                        x.sponsorcount = (from d in CH.DB.Deals
                                          join a in CH.DB.Packages on d.PackageID equals a.ID
                                          join b in CH.DB.PackageTypes on a.PackageTypeID equals b.ID
                                          where b.Name_EN.ToLower() == "sponsor"
                                          select d).Count();

                        x.LeadCount = (from d in CH.DB.CompanyRelationships
                                       join b in CH.DB.Leads on d.CompanyID equals b.CompanyID
                                       where d.ProjectID==item.ID
                                       select d).Count();

                        //x.LeadCount = crs.Select(l => l.Company).SelectMany(l => l.Leads).Count();   //<<<<<速度太慢

                        //x.sponsorcount = ds.Where(l => l.Package.ParticipantType.Name_EN.ToLower() == "sponsor").Count();  <<<<速度太慢
                        x.出单数量 = ds.Count();
                    }

                   
                }
            });

            return result;
        }
    }
}
