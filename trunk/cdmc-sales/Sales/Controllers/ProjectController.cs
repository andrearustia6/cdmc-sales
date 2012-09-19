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
    public class ProjectController : Controller
    {
        public ViewResult Index()
        {
            return View(CH.GetAllData<Project>());
        }

        public ViewResult SelectCompany(int? projectid)
        {
            ViewBag.ProjectID = projectid;
            return View(CH.GetAllData<Company>());
        }

        [HttpPost]
        public ActionResult SelectCompany(int[] checkedRecords, int projectid)
        {
            var p = CH.GetAllData<Project>(i => i.ID == projectid, "Companys").FirstOrDefault();
            p.Companys.Clear();
            if (p != null)
            {
                foreach (int i in checkedRecords)
                {
                    if (!p.Companys.Any(c => c.ID == i))
                    {
                        p.Companys.Add(CH.GetDataById<Company>(i));
                    }
                }
            }
            CH.Edit<Project>(p);
            return RedirectToAction("Management", "Project", new { id = projectid });
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<Project>(id));
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Management(int? id)
        {
            var Data = CH.GetAllData<Project>(i => i.ID == id, "Members", "Templates", "Messages", "Target_Ms").FirstOrDefault();
            return View(Data);
        }

        [HttpPost]
        public ActionResult Create(Project item)
        {
            if (ModelState.IsValid)
            {
                CH.Create<Project>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            var data = CH.GetDataById<Project>(id);
            data.SaleBrief = HttpUtility.HtmlDecode(data.SaleBrief);
            return View();
        }

        [HttpPost]
        public ActionResult Edit(Project item)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<Project>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            var data = CH.GetDataById<Project>(id);
            data.SaleBrief = HttpUtility.HtmlDecode(data.SaleBrief);
            return View(data);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<Project>(id);
            return RedirectToAction("Index");
        }
    }
}