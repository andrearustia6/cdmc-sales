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

        public ViewResult MyDealIndex()
        {
            return View(CH.GetAllData<Deal>(d=>d.Sales == User.Identity.Name));
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<Deal>(id));
        }

        public ActionResult MakeDeal(int projectid, int packageid, int companyrelationshipid)
        {
            ViewBag.ProjectID = projectid;
            ViewBag.PackageID = packageid;
            ViewBag.CompanyRelationshipID = companyrelationshipid;
            return View();
        }

        [HttpPost]
        public ActionResult MakeDeal(Deal item)
        {
            if (ModelState.IsValid)
            {
                item.Sales = User.Identity.Name;
                CH.Create<Deal>(item);
                return RedirectToAction("Management", "Lead", new { leadid=item.CompanyRelationshipID,projectid=item.ProjectID});
            }
            return View(item);
        }

        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<Deal>(id));
        }

        [HttpPost]
        public ActionResult Edit(Deal item)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<Deal>(item);
                return RedirectToAction("Index");
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
            CH.Delete<Deal>(id);
            return RedirectToAction("Index");
        }
    }
}