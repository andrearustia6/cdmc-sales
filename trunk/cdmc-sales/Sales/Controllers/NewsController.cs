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
using Telerik.Web.Mvc;

namespace Sales.Controllers
{
    [ProjectInformationAccess]
    public class NewsController : Controller
    {
        protected override void Dispose(bool disposing)
        {
            CH.DB.Dispose();
            base.Dispose(disposing);
        }

        public ViewResult Index(int? projectid)
        {
            projectid = this.TrySetProjectIDForUser(projectid);
            ViewBag.ProjectID = projectid;
            if (projectid != null)
            {
                return View(CH.GetAllData<News>(n => n.ProjectID == projectid).OrderByDescending(o => o.CreatedDate).ToList());
            }
            else
            {
                return View();
            }
          
        }

        public ViewResult Details(int id)
        {
            var data = CH.GetDataById<News>(id);
            data.Content = HttpUtility.HtmlDecode(data.Content);
            return View(data);
        }

        public ActionResult Create(int? projectid)
        {
            ViewBag.ProjectID = projectid;
            return View();
        }

        [HttpPost]
        public ActionResult Create(News item)
        {
            if (ModelState.IsValid)
            {
                CH.Create<News>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            var data = CH.GetDataById<News>(id);
            data.Content = HttpUtility.HtmlDecode(data.Content);
            return View(data);
        }

        [HttpPost]
        public ActionResult Edit(News item)
        {

            if (ModelState.IsValid)
            {
                CH.Edit<News>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            var data = CH.GetDataById<News>(id);
            data.Content = HttpUtility.HtmlDecode(data.Content);
            return View(data);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<News>(id);
            
            return RedirectToAction("Index");
        }
    }
}