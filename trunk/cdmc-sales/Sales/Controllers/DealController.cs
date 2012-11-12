using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Entity;
using Utl;
using BLL;

namespace Sales.Controllers
{
    [ManagerRequired]
    public class DealController : Controller
    {
      

        public ViewResult Index(int? projectid)
        {
            return View(CH.GetAllData<Deal>());
            //if (projectid == null)
            //    return null;
            
            //var  p = CH.GetDataById<Project>(projectid,"CompanyRelationships");
            //return View(CRM_Logical.GetProjectDeals(p,null,null));
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<Deal>(id));
        }

        public ActionResult Edit(int id)
        {
            var item = CH.GetDataById<Deal>(id);
            ViewBag.ProjectID = item.CompanyRelationship.ProjectID;
            ViewBag.CompanyRelationshipID = item.CompanyRelationshipID;
            return View(item);
        }

        [HttpPost]
        public ActionResult Edit(Deal item)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<Deal>(item);
                var projectid = CH.GetDataById<CompanyRelationship>(item.CompanyRelationshipID).ProjectID;
                return RedirectToAction("Management", "Project", new { id = projectid, tabindex = 4 });
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<Deal>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var item = CH.GetDataById<Deal>(id);
            var projectid = CH.GetDataById<CompanyRelationship>(item.CompanyRelationshipID).ProjectID; 
            CH.Delete<Deal>(id);

            return RedirectToAction("Management", "Project", new { id = projectid, tabindex = 4 });
        }
    }
}