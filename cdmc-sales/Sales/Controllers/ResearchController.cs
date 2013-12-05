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
            ViewBag.SelectedMonth = month;
            var md = MonthDuration.GetMonthInstance(month);
            return View(md);
        }

        [GridAction]
        public ActionResult _UserResearchIndex(int? month)
        {
            var md = MonthDuration.GetMonthInstance(month);

            var users = Query.AliveMemberNames();
            //权限控制
            var mems = CRM_Logical.GetUserInvolveProject().Select(s => s.Members.Where(w => w.IsActivated));

            var prs = //from c in CH.DB.CompanyRelationships group c by new {c.Creator} into cg
                //from l in CH.DB.Leads group c by new {c.companyid} into lg
                      from l in CH.DB.Leads
                      group l by new { l.Creator } into lg
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
        public ActionResult _UserResearchList(string name, string type, DateTime? starttime,DateTime? endtime, string sale = null)
        {
            IQueryable<_UserResearchDetail> details = null;
            if (type == "project")
            {
                int id = int.Parse(name);

                ViewBag.ProjectId = id;

              

                var temp = from tc in CH.DB.CompanyRelationships.Where(w => w.Project.ID == id) select tc;

                var leads = temp.SelectMany(s => s.Company.Leads);

                if (!string.IsNullOrEmpty(sale))
                {
                    int saleVal = int.Parse(sale);
                    var sales = CH.GetDataById<Member>(saleVal).Name;
                    leads = leads.Where(c => c.Creator == sales);
                }
             

                
                if (starttime != null)
                    leads = leads.Where(d => d.CreatedDate >= starttime);
                if (endtime != null)
                    leads = leads.Where(d => d.CreatedDate <= endtime);

                details = from l in leads
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
                              CreateDate = l.CreatedDate,
                             // Categoris = c.CategoryString
                          };
            }
            if (type == "user")
            {
                details = from l in CH.DB.Leads.Where(w => w.Creator == name)
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
                              CreateDate = l.CreatedDate

                          };
            }

            List<_UserResearchDetail> Detailslist = new List<_UserResearchDetail>();
            if (details != null)
            {

                //if (duration == null)
                //{
                //    duration = 1;
                //}
                //DateTime startdate = DateTime.Now;
                //startdate = startdate.AddDays(-duration.Value);
                //details = details.Where(d => d.CreateDate >= startdate && d.CreateDate <= DateTime.Now);

                //if (starttime != null)
                //    details = details.Where(d => d.CreateDate >= starttime);
                //if (endtime != null)
                //    details = details.Where(d => d.CreateDate <= endtime);
                ////权限控制

                Detailslist = details.ToList();

            }

            return View("UserResearchListView", new GridModel(Detailslist));
        }


        [GridAction]
        public ActionResult _ProjectResearchIndex(int? month)
        {
            var md = MonthDuration.GetMonthInstance(month);

            //权限控制
            var ps = CRM_Logical.GetUserInvolveProject().Select(s => s.ID);
            var projects = Query.AliveProjects();
            var prs = from p in projects.Where(w => ps.Contains(w.ID))
                      select new _ProjectResearch()
                      {
                          ProjectName = p.Name_CH,
                          MemberCount = p.Members.Count,
                          ProjectCode = p.ProjectCode,
                          ProjectID = p.ID,
                          FirstWeekCompanyCount = p.CompanyRelationships.Count(w => w.CreatedDate >= md.StartDate1 && w.CreatedDate < md.EndDate1),
                          SecondWeekCompanyCount = p.CompanyRelationships.Count(w => w.CreatedDate >= md.StartDate2 && w.CreatedDate < md.EndDate2),
                          ThirdWeekCompanyCount = p.CompanyRelationships.Count(w => w.CreatedDate >= md.StartDate3 && w.CreatedDate < md.EndDate3),
                          FourthWeekCompanyCount = p.CompanyRelationships.Count(w => w.CreatedDate >= md.StartDate4 && w.CreatedDate < md.EndDate4),
                          FivethWeekCompanyCount = p.CompanyRelationships.Count(w => w.CreatedDate >= md.StartDate5 && w.CreatedDate < md.EndDate5),
                      };
            ViewBag.ProjectId = prs.FirstOrDefault().ProjectID;

            return View(new GridModel(prs));
        }


        [GridAction]
        public ActionResult _CompanyIndex(int? projectid, string sales, int? selType, DateTime? startTime, DateTime? endTime)
        {
            //sales只能看到自己的项目Lead
            if (Employee.CurrentRole.Level == Role.LVL_Sales)
            {
                sales = Employee.CurrentUserName;
            }
            var selCompany = from data in CH.DB.Companys select data;
            IQueryable<CompanyRelationship> crms = CH.DB.CompanyRelationships.Where(w => w.ProjectID >0);
            if (projectid != null)
            {
                // selCompany.Where(c => CH.DB.CompanyRelationships.Where(r => r.ProjectID == projectid).Any(r => r.CompanyID == c.ID));
                crms = crms.Where(w => w.ProjectID == projectid);
            }
            else
            {
                //var relationship = CH.DB.CompanyRelationships.Where(r => CRM_Logical.GetUserInvolveProject().Any(c => c.ID == r.ProjectID));
                //selCompany.Where(c => relationship.Any(r => r.CompanyID == c.ID));
                var pids = CRM_Logical.GetUserInvolveProject().Select(s => s.ID).ToList();
                crms = CH.DB.CompanyRelationships.Where(w => pids.Contains((int)w.ProjectID));
            }

            if (sales != string.Empty && sales != "null")
            {
                //var relationship = CH.DB.CompanyRelationships.Where(r => CH.DB.Members.Where(m => m.Name == sales).Any(m => m.ProjectID == r.ProjectID));
                //selCompany.Where(c => relationship.Any(r => r.CompanyID == c.ID));
                crms = crms.Where(w => w.Members.Any(a => a.Name == sales));
            }
            //selCompany = crms.Select(s => s.Company);
            switch (selType)
            {
                case 1:
                    //selCompany = selCompany.Where(s => s.IsValid == false);
                    crms.Where(w => w.Company.IsValid == false);
                    break;
                case 2:
                    crms.Where(w => !string.IsNullOrEmpty(w.Company.CompanyReviews));
                    //selCompany = selCompany.Where(s => !string.IsNullOrEmpty(s.CompanyReviews));
                    break;
                default:
                    break;
            }

            if (startTime != null)
            {
                //selCompany = selCompany.Where(s => s.CreatedDate >= startTime);
                crms.Where(w => w.Company.CreatedDate >= startTime);
            }

            if (endTime != null)
            {
                //selCompany = selCompany.Where(s => s.CreatedDate <= endTime);
                crms.Where(w => w.Company.CreatedDate >= endTime);
            }

            var findata = from c in crms
                          select new _CompanyResearchDetail
                                       {
                                           ID = c.Company.ID,
                                           CompanyNameCH = c.Company.Name_CH,
                                           CompanyNameEN = c.Company.Name_EN,
                                           CompanyContact = c.Company.Contact,
                                           CompanyDesicription = c.Company.Description,
                                           CompanyReviews = c.Company.CompanyReviews,
                                           Creator = c.Company.Creator,
                                           CreateDate = c.Company.CreatedDate,
                                           IsValid = c.Company.IsValid == false ? "否" : "是",
                                           Description = c.Description,

                                       };

            return View(new GridModel(findata));
        }

        [GridAction]
        public ActionResult _LeadIndex(int companyid)
        {
            var selLead = from c in CH.DB.Leads
                          where c.CompanyID == companyid
                          select new _LeadResearchDetail
                          {
                              ID = c.ID,
                              LeadNameCH = c.Name_CH,
                              LeadNameEN = c.Name_EN,
                              LeadContact = c.Contact,
                              Email = c.EMail,
                              LeadMobile = c.Mobile,
                              LeadTitle = c.Title,
                              Creator = c.Creator,
                              CreateDate = c.CreatedDate,
                              IsValid = c.IsValid == false ? "否" : "是"
                          };
            return View(new GridModel(selLead));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _CompanyIndexEdit(int id)
        {
            _CompanyResearchDetail data = new _CompanyResearchDetail();
            TryUpdateModel(data);

            Company updateCompany = CH.GetDataById<Company>(id);
            if (data.IsValid == "否")
            {
                updateCompany.IsValid = false;
            }
            else
            {
                updateCompany.IsValid = true;
            }
            updateCompany.CompanyReviews = data.CompanyReviews;
            CH.Edit<Company>(updateCompany);

            var selCompany = from c in CH.DB.Companys
                             select new _CompanyResearchDetail
                             {
                                 ID = c.ID,
                                 CompanyNameCH = c.Name_CH,
                                 CompanyNameEN = c.Name_EN,
                                 CompanyContact = c.Contact,
                                 CompanyDesicription = c.Description,
                                 CompanyReviews = c.CompanyReviews,
                                 Creator = c.Creator,
                                 CreateDate = c.CreatedDate,
                                 IsValid = c.IsValid == false ? "否" : "是"
                             };

            return View(new GridModel(selCompany));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _LeadIndexEdit(int id, int? companyid)
        {
            _LeadResearchDetail data = new _LeadResearchDetail();
            TryUpdateModel(data);
            Lead updateLead = CH.GetDataById<Lead>(id);

            if (data.IsValid == "否")
            {
                updateLead.IsValid = false;
            }
            else
            {
                updateLead.IsValid = true;
            }
            CH.Edit<Lead>(updateLead);

            var selLead = from c in CH.DB.Leads
                          where c.CompanyID == companyid
                          select new _LeadResearchDetail
                          {
                              ID = c.ID,
                              LeadNameCH = c.Name_CH,
                              LeadNameEN = c.Name_EN,
                              LeadContact = c.Contact,
                              Email = c.EMail,
                              LeadMobile = c.Mobile,
                              LeadTitle = c.Title,
                              Creator = c.Creator,
                              CreateDate = c.CreatedDate,
                              IsValid = c.IsValid == false ? "否" : "是"
                          };
            return View(new GridModel(selLead));
        }


        #region
        protected override void Dispose(bool disposing)
        {
            CH.DB.Dispose();
            base.Dispose(disposing);
        }

        public ViewResult MyIndex(int? projectid)
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

        [HttpPost]
        public JsonResult MemberInPorject(int? projectId)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            SelectListItem selectListItemNone = new SelectListItem() { Text = "请选择", Value = "" };
            selectList.Add(selectListItemNone);
            if (projectId != null)
            {
                foreach (Member m in CH.GetAllData<Member>(c => c.ProjectID == projectId && c.IsActivated == true))
                {
                    SelectListItem selectListItem = new SelectListItem { Text = m.Name, Value = m.ID.ToString() };
                    selectList.Add(selectListItem);
                }
            }
            return this.Json(selectList);
        }


        [HttpPost]
        public JsonResult MemberSelectList(int? projectId)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            SelectListItem selectListItemNone = new SelectListItem() { Text = "所有销售", Value = "" };
            selectList.Add(selectListItemNone);
            if (projectId != null)
            {
                foreach (Member m in CH.GetAllData<Member>(c => c.ProjectID == projectId && c.IsActivated == true).OrderBy(s => s.Name))
                {
                    SelectListItem selectListItem = new SelectListItem { Text = m.Name, Value = m.Name };
                    selectList.Add(selectListItem);
                }
            }
            else
            {
                foreach (var m in SelectHelper.MemberSelectListInOwnProject())
                {
                    SelectListItem selectListItem = new SelectListItem { Text = m, Value = m };
                    selectList.Add(selectListItem);
                }
            }
            return this.Json(selectList);
        }
        #endregion
    }
}