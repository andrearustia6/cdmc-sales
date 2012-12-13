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

namespace Sales.Controllers
{
    public class MemberController : Controller
    {
        protected override void Dispose(bool disposing)
        {
            CH.DB.Dispose();
            base.Dispose(disposing);
        }

        public ViewResult Index()
        {
            return View(CH.GetAllData<Member>());
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<Member>(id));
        }



        public ActionResult Create(int? projectid)
        {
            ViewBag.ProjectID = projectid;
            return View();
        }

        [HttpPost]
        public ActionResult Create(Member item)
        {
            //this.AddErrorStateIfMemberExist(item.ProjectID,item.Name);


            if (ModelState.IsValid)
            {
                CH.Create<Member>(item);
                return RedirectToAction("Management", "Project", new { id = item.ProjectID,tabindex=1 });
            }
            ViewBag.ProjectID = item.ProjectID;
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<Member>(id));
        }

        [HttpPost]
        public ActionResult Edit(Member item)
        {
            //this.AddErrorStateIfMemberExist(item.ProjectID, item.Name);
            if (ModelState.IsValid)
            {
                CH.Edit<Member>(item);
                return RedirectToAction("Management", "Project", new { id = item.ProjectID,tabindex=1 });
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<Member>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var item = CH.GetDataById<Member>(id);
            var pid = item.ProjectID;
            CH.Delete<Member>(id);
            return RedirectToAction("Management", "Project", new { id = pid, tabindex = 1 });
        }
    }
}