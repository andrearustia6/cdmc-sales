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
            var result = (from x in CH.DB.ProjectReviews
                          select new _ProjectReview
                          {
                              ID = x.ID,
                              ProjectID = x.ProjectID,
                              Summary =x.Summary,
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
                    }
                }
            });


            return View(new GridModel(result));
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
                    }
                }
            });


            return View(new GridModel(result));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _EditAjaxEditing(int id)
        {
            CH.Delete<ProjectReview>(id);
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
                    }
                }
            });


            return View(new GridModel(result));
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
                    }
                }
            });


            return View(new GridModel(result));
        }
    }
}
