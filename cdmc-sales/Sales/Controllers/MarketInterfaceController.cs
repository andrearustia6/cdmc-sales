using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utl;
using System.IO;
using System.Text;
using Entity;
using BLL;

namespace Sales.Controllers
{
    [MarketInterfaceRequired( AccessType=AccessType.Equal)]
    public class MarketInterfaceController : Controller
    {
        protected override void Dispose(bool disposing)
        {
            CH.DB.Dispose();
            base.Dispose(disposing);
        }

        public ViewResult MarketIndex(int? projectid, int? dealcondition, int? distinctnumber)
        {
            var pjs = CH.GetAllData<Project>();
            var pj = CH.GetDataById<Project>(projectid);
            if (projectid != null)
            {
                pj = pjs.Where(p => p.ID == projectid).FirstOrDefault();
            }
            string categories = String.IsNullOrEmpty(Request["Categories"]) ? null : Request["Categories"].Trim();
            if (pj == null)
            {
                pj = CH.GetAllData<Project>().FirstOrDefault();
                if (pj == null)
                    return View();
            }
            this.AddErrorStateIfCreatorIsTheLoginUserIsNotTheMarketInterface(pj);
            ViewBag.ProjectID = pj.ID;
            ViewBag.Categories = categories;
            ViewBag.DealCondition = dealcondition;
            ViewBag.DistinctNumber = distinctnumber;
            if (ModelState.IsValid)
            {
                var ls = pj.ProjectLeads(dealcondition, distinctnumber, categories);
                return View(ls);
            }
            else
                return View();
            
        }

        public ActionResult EmailExportCsv(int? projectid, int? dealcondition, int? distinctnumber, string categories = "")
        {
            var pjs = CH.GetAllData<Project>();
            var pj = CH.GetDataById<Project>(projectid);
            if (projectid != null)
            {
                pj = pjs.Where(p => p.ID == projectid).FirstOrDefault();
            }
            if (pj == null)
            {
                return RedirectToAction("MarketIndex", new { projectid = projectid });
            }
            var ls = pj.ProjectLeads(dealcondition, distinctnumber, categories);

            this.AddErrorStateIfCreatorIsTheLoginUserIsNotTheMarketInterface(pj);
            if (ModelState.IsValid)
            {
                MemoryStream output = new MemoryStream();
                StreamWriter writer = new StreamWriter(output, Encoding.UTF8);
                foreach (var lead in ls)
                {
                    writer.Write(lead.EMail);
                    writer.WriteLine();
                }
                writer.Flush();
                output.Position = 0;
                return File(output, "text/comma-separated-values", "Emails.csv");
            }
            else
                return RedirectToAction("MarketIndex", new { projectid = projectid });

          
           
        }

    }
}
