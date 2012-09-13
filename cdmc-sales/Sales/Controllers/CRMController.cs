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
    public class CRMController : Controller
    {
        public ViewResult Index()
        {
            return View(CH.GetAllData<CRM>());
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<CRM>(id));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(CRM item)
        {
            if (ModelState.IsValid)
            {
                CH.Create<CRM>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<CRM>(id));
        }

        public ActionResult Management(int leadid,int? ProjectID)
        {
            ViewBag.ProjectID = ProjectID;
            return View(CH.GetAllData<Lead>(i => i.ID == leadid, "Target_Packages").FirstOrDefault());
        }

        public ActionResult LeadCallIndex(int leadid)
        {
            var data = CH.GetAllData<LeadCall>(i=>i.LeadID == leadid);
            return View(data);
        }

        public ActionResult Save_LeadPackage(int leadid,int projectid, int packageid)
        {
            CH.Create<Target_Package>(new Target_Package() { PackageID = packageid, LeadID = leadid, ProjectID = projectid });
            return new JsonResult();
        }
        [HttpPost]
        public ActionResult Save_LeadCall(LeadCall callresult)
        {
            callresult.CallingTime = DateTime.Now;
            CH.Create<LeadCall>(callresult);

            callresult = CH.GetAllData<LeadCall>(lc => lc.LeadID == callresult.LeadID, "LeadCallType").FirstOrDefault();
           
            //return new DataJsonResult<Lead>() { Data = data };
            return new DataJsonResult<LeadCall>() { Data = callresult };
        }

        [HttpPost]
        public ActionResult Edit(CRM item)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<CRM>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<CRM>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<CRM>(id);
            return RedirectToAction("Index");
        }
    }
}