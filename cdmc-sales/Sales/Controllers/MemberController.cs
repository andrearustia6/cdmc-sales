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
            if (!CRM_Logical.IsMemberExist(item.Name))
            {
                ModelState.AddModelError("", "该员工不存在");
            }
            if (CRM_Logical.IsSameMemberExistInProject(item.Name, item.ProjectID))
            {
                ModelState.AddModelError("", "该员工以加入此项目");
            }

            if (ModelState.IsValid)
            {
                CH.Create<Member>(item);
                return RedirectToAction("Management", "Project", new { id = item.ProjectID });
                //return View(@"~\views\Project\Management.cshtml", CH.GetDataById<Project>(item.ProjectID));
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
            if (!CRM_Logical.IsMemberExist(item.Name))
            {
                ModelState.AddModelError("", "该员工不存在");
            }
            //var memberoldname = CH.DB.Members.AsNoTracking().FirstOrDefault(i => i.ID == item.ID);
            //if (CRM_Logical.IsSameMemberExistInProject(item.Name, item.ProjectID) && memberoldname.Name != item.Name)
            //{
            //    ModelState.AddModelError("", "该员工以加入此项目");
            //}
            //else
            //{
            //    CH.DB.Entry(memberoldname).State = EntityState.Detached; 
            //    CH.DB.Members.Attach(item);
            //}
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
            return RedirectToAction("Management", "Project", new { id = pid });
        }
    }
}