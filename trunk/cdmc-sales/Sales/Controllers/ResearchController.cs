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
using Telerik.Web.Mvc;
using BLL;
using Sales.Model;

namespace Sales.Controllers
{
    public class ResearchController : Controller
    {
        public ActionResult ResearchIndex(int? month)
        {
            var md = MonthDuration.GetMonthInstance(month);
            return View(md);
        }


        

        [GridAction]
        public ActionResult _UserResearchIndex(int? month)
        {
            var md = MonthDuration.GetMonthInstance(month);

            var users = Query.AliveMemberNames();
            var prs = //from c in CH.DB.CompanyRelationships group c by new {c.Creator} into cg
                      from l in CH.DB.Leads group l by new { l.Creator } into lg
                      from u in users
                      where lg.Key.Creator == u 
                  
                      select new _UserResearch()
                      {
                          UserName = u,
                          FirstWeekLeadCount = lg.Count(w => w.CreatedDate >= md.StartDate2 && w.CreatedDate < md.EndDate2),
                          SecondWeekLeadCount = lg.Count(w => w.CreatedDate >= md.StartDate2 && w.CreatedDate < md.EndDate2),
                          ThirdWeekLeadCount = lg.Count(w => w.CreatedDate >= md.StartDate3 && w.CreatedDate < md.EndDate3),
                          FourthWeekLeadCount = lg.Count(w => w.CreatedDate >= md.StartDate4 && w.CreatedDate < md.EndDate4),
                          FivethWeekLeadCount = lg.Count(w => w.CreatedDate >= md.StartDate5 && w.CreatedDate < md.EndDate5)
                      };

            //var list = prs.ToList();
            //var lg = CH.DB.Leads.Where(w=>w.Creator==Employee.CurrentUserName);
            //foreach (var v in list)
            //{
            //        v.FirstWeekLeadCount = lg.Count(w => w.CreatedDate >= md.StartDate1 && w.CreatedDate < md.EndDate1);
            //        v.SecondWeekLeadCount = lg.Count(w => w.CreatedDate >= md.StartDate2 && w.CreatedDate < md.EndDate2);
            //        v.ThirdWeekLeadCount = lg.Count(w => w.CreatedDate >= md.StartDate3 && w.CreatedDate < md.EndDate3);
            //        v.FourthWeekLeadCount = lg.Count(w => w.CreatedDate >= md.StartDate4 && w.CreatedDate < md.EndDate4);
            //        v.FivethWeekLeadCount = lg.Count(w => w.CreatedDate >= md.StartDate5 && w.CreatedDate < md.EndDate5);
            //}

            //var nprs = from  l in CH.DB.Leads group l by new { l.Creator } into lg
            //           from p in list
            //           where p.UserName == lg.Key.Creator
            //           select new _UserResearch()
            //           {
            //               UserName = p.UserName,
            //               FirstWeekCompanyCount = p.FirstWeekCompanyCount,
            //               SecondWeekCompanyCount = p.SecondWeekCompanyCount,
            //               ThirdWeekCompanyCount = p.ThirdWeekCompanyCount,
            //               FourthWeekCompanyCount = p.FourthWeekCompanyCount,
            //               FivethWeekCompanyCount = p.FivethWeekCompanyCount,
                       

            //           };

            return View(new GridModel(prs));
        }
        public JsonResult _ResearchUserName(string username)        
        {
            return Json(new { username = username });    
        }

        [GridAction]
        public ActionResult _UserResearchList(string name, string type) 
        {
            if (type == "project")
            {
                int id = int.Parse(name);
                var details = from l in CH.DB.CompanyRelationships.Where(w => w.Project.ID == id).Select(s => s.Company).SelectMany(s => s.Leads)
                              select new _UserResearchDetail()
                              {
                                  LeadNameEN = l.Name_EN,
                                  LeadNameCH = l.Name_CH,
                                  CompanyNameCH = l.Company.Name_EN,
                                  CompanyNameEN = l.Company.Name_CH,
                                  CompanyContact = l.Company.Contact,
                                  CompanyDesicription = l.Company.Description,
                                  Email = l.EMail,
                                  LeadContact = l.Contact,
                                  LeadMobile = l.Mobile,
                                  LeadTitle = l.Title
                              };

                return View(new GridModel(details.ToList()));
 
            }
            if (type == "user")
            {
                var details = from l in CH.DB.Leads.Where(w => w.Creator == name)
                              select new _UserResearchDetail()
                              {
                                  LeadNameEN = l.Name_EN,
                                  LeadNameCH = l.Name_CH,
                                  CompanyNameCH = l.Company.Name_EN,
                                  CompanyNameEN = l.Company.Name_CH,
                                  CompanyContact = l.Company.Contact,
                                  CompanyDesicription = l.Company.Description,
                                  Email = l.EMail,
                                  LeadContact = l.Contact,
                                  LeadMobile = l.Mobile,
                                  LeadTitle = l.Title,
                                  Creator = l.Creator,
                              };

                return View(new GridModel(details.ToList()));
            }

            return View(new GridModel(new List<_UserResearchDetail>()));
        }
        

        [GridAction]
        public ActionResult _ProjectResearchIndex(int? month)
        {
            var md = MonthDuration.GetMonthInstance(month);
         
            var projects = Query.AliveProjects();
            var prs = from p in projects select new _ProjectResearch() { 
                  ProjectName = p.Name_CH,
                  MemberCount = p.Members.Count,
                  ProjectCode = p.ProjectCode,
                  ProjectID = p.ID,
                  FirstWeekCompanyCount = p.CompanyRelationships.Count(w=>w.CreatedDate >= md.StartDate1 && w.CreatedDate<md.EndDate1),
                  SecondWeekCompanyCount = p.CompanyRelationships.Count(w=>w.CreatedDate >= md.StartDate2 && w.CreatedDate<md.EndDate2),
                  ThirdWeekCompanyCount = p.CompanyRelationships.Count(w=>w.CreatedDate >= md.StartDate3 && w.CreatedDate<md.EndDate3),
                  FourthWeekCompanyCount = p.CompanyRelationships.Count(w => w.CreatedDate >= md.StartDate4 && w.CreatedDate < md.EndDate4),
                  FivethWeekCompanyCount = p.CompanyRelationships.Count(w => w.CreatedDate >= md.StartDate5 && w.CreatedDate < md.EndDate5),
                 // CompanyAverage = p.CompanyRelationships.Count(w =>w.CreatedDate>=md.StartDate1 && w.CreatedDate <md.EndDate5)/p.Members.Count,

                  FirstWeekLeadCount = p.CompanyRelationships.Select(s => s.Company).SelectMany(s => s.Leads).Count(w => w.CreatedDate >= md.StartDate1 && w.CreatedDate < md.EndDate1),
                  SecondWeekLeadCount = p.CompanyRelationships.Select(s => s.Company).SelectMany(s => s.Leads).Count(w => w.CreatedDate >= md.StartDate2 && w.CreatedDate < md.EndDate2),
                  ThirdWeekLeadCount = p.CompanyRelationships.Select(s => s.Company).SelectMany(s => s.Leads).Count(w => w.CreatedDate >= md.StartDate3 && w.CreatedDate < md.EndDate3),
                  FourthWeekLeadCount = p.CompanyRelationships.Select(s => s.Company).SelectMany(s => s.Leads).Count(w => w.CreatedDate >= md.StartDate4 && w.CreatedDate < md.EndDate4),
                  FivethWeekLeadCount = p.CompanyRelationships.Select(s => s.Company).SelectMany(s => s.Leads).Count(w => w.CreatedDate >= md.StartDate5 && w.CreatedDate < md.EndDate5),
                 // LeadAverage = p.CompanyRelationships.Select(s => s.Company).SelectMany(s => s.Leads).Count(w => w.CreatedDate >= md.StartDate1 && w.CreatedDate < md.EndDate5) / p.Members.Count
            };
            //foreach (var p in prs)
            //{
            //    p.CompanyAverage = Math.Round(p.CompanyAverage, 1);
            //    p.LeadAverage = Math.Round(p.LeadAverage, 1);
            //}
            return View(new GridModel(prs));
        }

        protected override void Dispose(bool disposing)
        {
            CH.DB.Dispose();
            base.Dispose(disposing);
        }

        public ViewResult MyIndex(int? projectid )
        {
            projectid = this.TrySetProjectIDForUser(projectid);
            return View(CH.GetAllData<Research>(r => r.Creator == Employee.CurrentUserName || r.AddPerson == Employee.CurrentUserName).OrderByDescending(o => o.CreatedDate).ToList());
        }

        [LeaderRequired]
        public ViewResult Index(List<int> selectedprojects, bool? isActivated, DateTime? startdate, DateTime? enddate)
        {
            startdate = startdate == null ? new DateTime(1, 1, 1) : startdate;
            enddate = enddate == null ? new DateTime(9999, 1, 1) : startdate;

            if (selectedprojects != null)
            {
                var rs = CH.GetAllData<Research>(r => selectedprojects.Any(sp => sp == r.ProjectID) && r.CreatedDate >= startdate && r.CreatedDate <= enddate);
                return View(rs);
            }
            else 
            {
                var ps = CRM_Logical.GetUserInvolveProject();
                var rs = CH.GetAllData<Research>(r => ps.Any(sp => sp.ID == r.ProjectID) && r.CreatedDate >= startdate && r.CreatedDate <= enddate);
                return View(rs);
            }
        }

        //public ViewResult Index()
        //{
        //    if(Employee.AsDirector())
        //        return View(CH.GetAllData<Research>().OrderByDescending(o => o.CreatedDate).ToList());
        //    else if (Employee.EqualToManager())
        //    {
        //        var ps = CH.GetAllData<Project>(p => p.Manager == Employee.CurrentUserName);
        //        var list = new List<Member>();
        //        ps.ForEach(p => {
        //           list.AddRange(p.Members);
        //        });
        //        var rs = CH.GetAllData<Research>(r => list.Any(m=>m.Name==r.Creator));
        //        return View(rs.OrderByDescending(o => o.CreatedDate).ToList());
        //    }
        //      return View();
        //}

        public ViewResult Details(int id)
        {
            var data = CH.GetDataById<Research>(id);
            data.Contents = HttpUtility.HtmlDecode(data.Contents);
            return View(data);
        }

        public ActionResult Create()
        {
            //if (projectid == null)
            //    projectid = this.TrySetProjectIDForUser(projectid);

            //ViewBag.ProjectID = projectid;
            return View();
        }

        [HttpPost]
        public ActionResult Create(Research item)
        {
            if (ModelState.IsValid)
            {
                // item.Contents = HttpUtility.HtmlEncode(item.Contents);
                item.AddPerson = Employee.CurrentUserName;
                Image image = ImageController.UploadImg(Request, item.Image);
                if (image != null)
                    item.ImageID = image.ID;
                CH.Create<Research>(item);
                return RedirectToAction("MyIndex");
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            var data = CH.GetDataById<Research>(id);
            data.Contents = HttpUtility.HtmlDecode(data.Contents);
            return View(data);
        }

        [HttpPost]
        public ActionResult Edit(Research item)
        {

            if (ModelState.IsValid)
            {
                //item.Contents = HttpUtility.HtmlDecode(item.Contents);
                item.AddPerson = Employee.CurrentUserName;
                Image image = ImageController.UploadImg(Request, item.Image);
                if (image != null)
                    item.ImageID = image.ID;
                CH.Edit<Research>(item);
                return RedirectToAction("MyIndex");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            var data = CH.GetDataById<Research>(id);
            data.Contents = HttpUtility.HtmlDecode(data.Contents);
            return View(data);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<Research>(id);
            return RedirectToAction("MyIndex");
        }
    }
}