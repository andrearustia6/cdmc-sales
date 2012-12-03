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
    public class MessageController : Controller
    {
        public ViewResult Index()
        {
            return View(CH.GetAllData<Message>().OrderByDescending(o=>o.CreatedDate).ToList());
        }


        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<Message>(id));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Message item)
        {
            if (ModelState.IsValid)
            {
               
                item.Member = User.Identity.Name;
                var p = CH.GetDataById<Project>(item.ProjectID,"Members");
                item = item.SetFlowNumber(p);
                var m = p.GetMemberInProjectByName(item.Member);
                item.SalesTypeID = m.SalesTypeID;
                CH.Create<Message>(item);
                return RedirectToAction("Management", "Project", new { id = item.ProjectID, tabindex = 5 });
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<Message>(id));
        }

        [HttpPost]
        public ActionResult Edit(Message item)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(item.Answer))
                    item.IsAnswered = false;
                else
                    item.IsAnswered = true;

                CH.Edit<Message>(item);
                return RedirectToAction("Management", "Project", new { id = item.ProjectID, tabindex = 5 });
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<Message>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var item = CH.GetDataById<Message>(id);
            var projectid = item.ProjectID;
            CH.Delete<Message>(id);
            return RedirectToAction("Management", "Project", new { id = projectid, tabindex = 5 });
        }
    }
}