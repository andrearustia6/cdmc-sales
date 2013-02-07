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

namespace Sales.Controllers.Type
{
    public class ProjectRightController : Controller
    {
        protected override void Dispose(bool disposing)
        {
            CH.DB.Dispose();
            base.Dispose(disposing);
        }

        public ViewResult Index()
        {
            return View(CH.GetAllData<ProjectRight>());
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<ProjectRight>(id));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(ProjectRight item, List<int> rights)
        {
            if (ModelState.IsValid)
            {
                var ars = CH.GetAllData<AccessRight>(a => rights.Contains(a.ID));
                item.AccessRights = ars;
                CH.Create<ProjectRight>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            var data = CH.GetDataById<ProjectRight>(id);
            return View(data);
        }

        [HttpPost]
        public ActionResult Edit(ProjectRight item, List<int> rights)
        {
            if (ModelState.IsValid)
            {
              
                if (rights == null) rights = new List<int>();

                //清空
                var i = CH.GetDataById<ProjectRight>(item.ID);
                if (i.AccessRights != null)
                {
                    i.AccessRights.Clear();
                    CH.Edit<ProjectRight>(i);
                    CH.DB.Detach(i);
                }


                item.AccessRights = new List<AccessRight>();

                var ars = CH.GetAllData<AccessRight>(a => rights.Contains(a.ID));
                CH.Edit<ProjectRight>(item);
                item.AccessRights.AddRange(ars);
                CH.Edit<ProjectRight>(item);

                 //item.AccessRights = new List<AccessRight>();
                 //var ars = CH.GetAllData<AccessRight>(a => rights.Contains(a.ID));
                 //item.AccessRights.AddRange(ars);

                 //CH.DB.ProjectRights.Attach(item);
                 //CH.Edit<ProjectRight>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {

            return View(CH.GetDataById<ProjectRight>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<ProjectRight>(id);
            return RedirectToAction("Index");
        }
    }
}