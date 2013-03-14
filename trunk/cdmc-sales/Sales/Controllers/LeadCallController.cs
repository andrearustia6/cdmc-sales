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
    public class LeadCallController : Controller
    {
        protected override void Dispose(bool disposing)
        {
            CH.DB.Dispose();
            base.Dispose(disposing);
        }

        //[ManagerRequired]
        [ProductInterfaceRequired]
        public ViewResult Index()
        {
            return View(CH.GetAllData<LeadCall>().OrderByDescending(o=>o.CreatedDate).ToList());
        }

        public ViewResult CompanyRelationshipIndex(int projectid, int companyrelationshipid)
        {

            return View("Index", CH.GetAllData<LeadCall>().FindAll(lc => lc.CompanyRelationship.ProjectID == projectid && lc.CompanyRelationship.ID == companyrelationshipid).OrderByDescending(o => o.CreatedDate).ToList());
        }

        public ViewResult LeadIndex(int projectid, int companyrelationshipid, int leadid)
        {
            return View("Index", CH.GetAllData<LeadCall>().FindAll(lc => lc.CompanyRelationship.ProjectID == projectid && lc.CompanyRelationship.ID == companyrelationshipid && lc.LeadID == leadid).OrderByDescending(o => o.CreatedDate).ToList());
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<LeadCall>(id));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(LeadCall item)
        {
            if (ModelState.IsValid)
            {
                CH.Create<LeadCall>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<LeadCall>(id));
        }

        [HttpPost]
        public ActionResult Edit(LeadCall item)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<LeadCall>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<LeadCall>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<LeadCall>(id);
            return RedirectToAction("Index");
        }
    }
}