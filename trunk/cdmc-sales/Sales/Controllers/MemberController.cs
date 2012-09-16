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

namespace Sales.Controllers
{
    public class MemberController : Controller
    {

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
            if (ModelState.IsValid)
            {
                CH.Create<Member>(item);
                return RedirectToAction("Management", "Project", new { projectid = item.ProjectID});
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
            if (ModelState.IsValid)
            {
                CH.Edit<Member>(item);
                return RedirectToAction("Management", "Project", new { projectid = item.ProjectID });
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
            CH.Delete<Member>(id);
            return RedirectToAction("Management", "Project", new { projectid = item.ProjectID });
        }
    }
}