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
        public ViewResult MarketIndex(int? projectid)
        {

            var pj =  CH.GetDataById<Project>(projectid);
            if (pj == null)
            {
                pj = CH.GetAllData<Project>().FirstOrDefault();
                if(pj==null)
                    return View();
            }
            this.AddErrorStateIfCreatorIsTheLoginUserIsNotTheMarketInterface(pj);
            ViewBag.ProjectID = pj.ID;
            if (ModelState.IsValid)
            {
                var ls = pj.ProjectLeads();
                return View(ls);
            }
            else
                return View();
            
        }

        public ActionResult EmailExportCsv(int? projectid)
        {
            var pj = CH.GetDataById<Project>(projectid);
            if (pj == null)
            {
                return RedirectToAction("MarketIndex", new { projectid = projectid });
            }
            var ls = pj.ProjectLeads();

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
