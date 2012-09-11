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

        public ActionResult Management(int leadid)
        {
            var crm = CH.GetAllData<CRM>(i => i.LeadID == leadid,"LeadCalls").FirstOrDefault();

            if (crm == null)
            {
                CH.Create<CRM>(crm = new CRM() { LeadID = leadid});
                crm.Lead = CH.GetDataById<Lead>(leadid);
                if (crm.Lead == null)
                    throw new Exception("客户ID在数据库中不存在");
            }

            return View(crm);
        }

        public ActionResult LeadCallIndex(int leadid)
        {
            var data = CH.GetAllData<LeadCall>(i=>i.LeadID == leadid);
            return View(data);
        }


        [HttpPost]
        public ActionResult Save_LeadCall(LeadCall callresult)
        {
           
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