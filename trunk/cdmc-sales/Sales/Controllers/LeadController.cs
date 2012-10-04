using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Entity;
using EntityUtl;
using Sales;
using Utl;
using Telerik.Web.Mvc;
using System.Collections;
using System.IO;
using System.Text;

namespace Sales.Controllers
{
    
    public class LeadController : Controller
    {
        [MarketInterfaceRequired]
        public ViewResult MarketIndex()
        {
            return View(CH.GetAllData<Lead>());
        }

        [SalesRequired]
        public ViewResult DistributionIndex(int? projectid)
        {
            ViewBag.ProjectID = projectid;
            var members = CH.GetAllData<Member>(m => m.Name == User.Identity.Name&&m.ProjectID == projectid);

            var member = members.FirstOrDefault();
            if (member != null && member.CharactersSet != null)
            {
                var data = CH.GetAllData<Company>("Leads");
                data = data.FindAll(c => member.CharactersSet.Any(ca => (c.Name_EN.ToUpper().StartsWith(ca))));
                var leads = new List<Lead>();
                data.ForEach(c => {
                    leads.AddRange(c.Leads);
                });
                return View(leads);
            }
            return View();
        }
        [MarketInterfaceRequired]
        public ActionResult EmailExportCsv(int page, string orderBy, string filter)
        {
             var leads = CH.GetAllData<Lead>();           
             MemoryStream output = new MemoryStream();            
             StreamWriter writer = new StreamWriter(output, Encoding.UTF8);  
             foreach (var lead in leads)            
             {
                 writer.Write(lead.EMail); 
                 writer.WriteLine();            
             }           
             writer.Flush();           
             output.Position = 0;                       
             return File(output, "text/comma-separated-values", "Emails.csv");        }
        

        public ViewResult Index()
        {
            return View(CH.GetAllData<Lead>());
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<Lead>(id));
        }

        public ActionResult Create(int? companyid, string from)
        {
            ViewBag.CompanyID = companyid;
            if (from == "company")
            {
                ViewBag.From = "company";
            }
            return View();
        }

        [HttpPost]
        public ActionResult Create(Lead item)
        {
            if (EntityUtl.Utl.CheckPropertyAllNull(item, "Name_EN", "Name_CH"))
                ModelState.AddModelError("", "名字不完整,中文名和英文名不能同时为空");

            if (EntityUtl.Utl.CheckPropertyAllNull(item, "EMail", "Fax"))
                ModelState.AddModelError("", "联系方式不完整,座机和传真号不能同时为空");

            if (ModelState.IsValid)
            {
                Image image = ImageController.UploadImg(Request, item.Image);
                if(image!=null)
                  item.ImageID = image.ID;
                CH.Create<Lead>(item);
                return RedirectToAction("Index", "Company");
            }
            ViewBag.CompanyID = item.CompanyID;
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<Lead>(id));
        }

        [HttpPost]
        public ActionResult Edit(Lead item)
        {
            if (EntityUtl.Utl.CheckPropertyAllNull(item, "Name_EN", "Name_CH"))
                ModelState.AddModelError("", "名字不完整,中文名和英文名不能同时为空");

            if (EntityUtl.Utl.CheckPropertyAllNull(item, "EMail", "Fax"))
                ModelState.AddModelError("", "联系方式不完整,座机和传真号不能同时为空");

            if (ModelState.IsValid)
            {
                Image image = ImageController.UploadImg(Request, item.Image);
                if (image != null)
                    item.ImageID = image.ID;
                CH.Edit<Lead>(item);
                return RedirectToAction("Index","Company");
            }
            ViewBag.CompanyID = item.CompanyID;
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<Lead>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<Lead>(id);
            return RedirectToAction("Index", "Company");
        }

        public ActionResult Management(int leadid, int? ProjectID)
        {
            if (ProjectID != null)
            {
                ViewBag.ProjectID = ProjectID;
             
                var leads = CH.GetAllData<Lead>(i => i.ID == leadid, "TargetOfPackages","Projects");
                var data = leads.FirstOrDefault(l=>l.Projects.Any(p=>p.ID==ProjectID));
                return View(data);
            }
            else
               return View(CH.GetAllData<Lead>(i => i.ID == leadid, "TargetOfPackages").FirstOrDefault());
        }

        public ActionResult LeadCallIndex(int leadid)
        {
            var data = CH.GetAllData<LeadCall>(i => i.LeadID == leadid);
            return View(data);
        }

        public ActionResult Save_LeadPackage(int leadid, int projectid, int packageid)
        {
            var old = CH.GetAllData<TargetOfPackage>(i => i.LeadID == leadid).FirstOrDefault();
            if (old != null && old.PackageID != packageid)
                CH.Delete<TargetOfPackage>(old.ID);

            CH.Create<TargetOfPackage>(new TargetOfPackage() { PackageID = packageid, LeadID = leadid, ProjectID = projectid });
            return new JsonResult();
        }
        [HttpPost]
        public ActionResult Save_LeadCall(LeadCall callresult)
        {
            callresult.CallingTime = DateTime.Now;
            callresult.Caller = User.Identity.Name;
            CH.Create<LeadCall>(callresult);

            callresult = CH.GetAllData<LeadCall>(lc => lc.LeadID == callresult.LeadID, "LeadCallType").FirstOrDefault();

            //return new DataJsonResult<Lead>() { Data = data };
            return new DataJsonResult<LeadCall>() { Data = callresult };
        }
    }
}