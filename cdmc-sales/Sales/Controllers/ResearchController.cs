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
    public class ResearchController : Controller
    {
        protected override void Dispose(bool disposing)
        {
            CH.DB.Dispose();
            base.Dispose(disposing);
        }

        public ViewResult MyIndex()
        {
            return View(CH.GetAllData<Research>(r => r.Creator == Employee.CurrentUserName || r.AddPerson == Employee.CurrentUserName).OrderByDescending(o => o.CreatedDate).ToList());
        }

        [LeaderRequired]
        public ViewResult Index(List<int> selectedprojects, bool? isActivated, DateTime? startdate, DateTime? enddate)
        {
            startdate = startdate == null ? new DateTime(1, 1, 1) : startdate;
            enddate = enddate == null ? new DateTime(9999, 1, 1) : startdate;

            if (selectedprojects != null)
            {
               var rs = CH.GetAllData<Research>(r=>selectedprojects.Any(sp=>sp == r.ProjectID ) && r.CreatedDate>= startdate && r.CreatedDate<= enddate);
               return View(rs);
            }

            return View();
        }

        //public ViewResult Index()
        //{
        //    if(Employee.AsDirector())
        //        return View(CH.GetAllData<Research>().OrderByDescending(o => o.CreatedDate).ToList());
        //    else if (Employee.EqualToManager())
        //    {
        //        var ps = CH.GetAllData<Project>(p => p.Manager == Employee.CurrentUserName);
        //        var list = new List<Member>();
        //        ps.ForEach(p => {
        //           list.AddRange(p.Members);
        //        });
        //        var rs = CH.GetAllData<Research>(r => list.Any(m=>m.Name==r.Creator));
        //        return View(rs.OrderByDescending(o => o.CreatedDate).ToList());
        //    }
        //      return View();
        //}

        public ViewResult Details(int id)
        {
            var data = CH.GetDataById<Research>(id);
            data.Contents = HttpUtility.HtmlDecode(data.Contents);
            return View(data);
        }

        public ActionResult Create(int? projectid)
        {
            if (projectid == null)
                projectid = this.TrySetProjectIDForUser(projectid);

            ViewBag.ProjectID = projectid;
            return View();
        }

        [HttpPost]
        public ActionResult Create(Research item)
        {
            if (ModelState.IsValid)
            {
                // item.Contents = HttpUtility.HtmlEncode(item.Contents);
                item.AddPerson = Employee.CurrentUserName;
                Image image = ImageController.UploadImg(Request, item.Image);
                if (image != null)
                    item.ImageID = image.ID;
                CH.Create<Research>(item);
                return RedirectToAction("MyIndex");
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            var data = CH.GetDataById<Research>(id);
            data.Contents = HttpUtility.HtmlDecode(data.Contents);
            return View(data);
        }

        [HttpPost]
        public ActionResult Edit(Research item)
        {

            if (ModelState.IsValid)
            {
                //item.Contents = HttpUtility.HtmlDecode(item.Contents);
                item.AddPerson = Employee.CurrentUserName;
                Image image = ImageController.UploadImg(Request, item.Image);
                if (image != null)
                    item.ImageID = image.ID;
                CH.Edit<Research>(item);
                return RedirectToAction("MyIndex");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            var data = CH.GetDataById<Research>(id);
            data.Contents = HttpUtility.HtmlDecode(data.Contents);
            return View(data);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<Research>(id);
            return RedirectToAction("MyIndex");
        }
    }
}