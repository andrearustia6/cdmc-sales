﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Entity;
using Sales;
using Utl;
using BLL;
using System.IO;

namespace Sales.Controllers
{

    [ManagerRequired]
    public class ProjectController : Controller
    {
        protected override void Dispose(bool disposing)
        {
            CH.DB.Dispose();
            base.Dispose(disposing);
        }
        public ViewResult Index()
        {
            var data = this.GetProjectByRole();
            return View(data);
        }

        public ViewResult GotoReports()
        {
            return View();
        }

        //public ViewResult Reports(DateTime? setdate)
        //{
        //    if (setdate == null)
        //        return View();
        //    var projects = CH.GetAllData<Project>("Members", "TargetOfWeeks", "Leads", "Deals", "Companys", "Leads");
        //    var weeklyreports = CRM_Logical.GenerateWeeklyReports(projects, setdate);
        //    return View(weeklyreports);
        //}

        //[ManagerRequired]
        //public ViewResult Compare(DateTime? setdate, int? projectid)
        //{
        //    if (setdate == null)
        //        return View();
        //    var projects = CH.GetAllData<Project>("Members", "TargetOfWeeks", "Leads", "Deals", "Companys", "Leads");
        //    var weeklyreports = CRM_Logical.GenerateWeeklyReports(projects, setdate);
        //    var rp = weeklyreports.FirstOrDefault(w => w.Project.ID == projectid);
        //    return View(rp.MemberItems);
        //}

        #region 添加公司
        public ViewResult SelectCompanyByProjectCode(int projectid)
        {
            ViewBag.ProjectID = projectid;
            var p = CH.GetDataById<Project>(projectid);
            var data = CH.GetAllData<Project>(i => i.ID != projectid);
            if (!string.IsNullOrEmpty(p.References))
            {
                var refers = p.References.Split('|');
                data = data.FindAll(d => refers.Contains(d.ProjectCode) == false && d.ProjectCode != p.ProjectCode);
            }
            else
            {
                data = data.FindAll(d => d.ProjectCode != p.ProjectCode);
            }

            return View(data);
        }


        //public ViewResult AddToCompanyRelationship(int projectid)
        //{
        //    ViewBag.ProjectID = projectid;

        //    return View(CH.GetAllData<CompanyRelationship>(c => c.ProjectID == projectid));
        //}




        [HttpPost]
        public ActionResult SelectCompanyByProjectCode(int[] checkedRecords, int projectid)
        {
            if (checkedRecords != null)
            {
                var allselectedprojects = CH.GetAllData<Project>(item => checkedRecords.Any(cr => cr == item.ID), "CompanyRelationships");
                var p = CH.GetDataById<Project>(projectid);
                if (p != null)
                {
                    allselectedprojects.ForEach(nr =>
                    {
                        nr.CompanyRelationships.ForEach(cr =>
                        {
                            p.AddExistCompanyToNewCompanyRelationship(cr.CompanyID);

                        });
                    });
                    string refer = string.Empty;
                    allselectedprojects.ForEach(select =>
                    {
                        if (string.IsNullOrEmpty(refer))
                        {
                            refer = select.ProjectCode;
                        }
                        else
                        {
                            refer += "|" + select.ProjectCode;
                        }

                    });
                    p.References = refer;
                    // 没有删除逻辑
                    CH.Edit<Project>(p);
                }
                return RedirectToAction("Management", "Project", new { id = projectid, tabindex = 3 });
            }
            else
            {
                ViewBag.ProjectID = projectid;
                var p = CH.GetDataById<Project>(projectid);
                var data = CH.GetAllData<Project>(i => i.ID != projectid);
                if (!string.IsNullOrEmpty(p.References))
                {
                    var refers = p.References.Split('|');
                    data = data.FindAll(d => refers.Contains(d.ProjectCode) == false && d.ProjectCode != p.ProjectCode);
                }
                else
                {
                    data = data.FindAll(d => d.ProjectCode != p.ProjectCode);
                }

                return View(data);
            }

        }

        #endregion

        #region 指定成员到公司
        public ViewResult AppointSales(int? projectid, int? companyrelationshipid)
        {
            var ms = CH.GetAllData<Member>(m => m.ProjectID == projectid, "CompanyRelationships");
            ViewBag.CompanyRelationshipID = companyrelationshipid;
            return View(ms);
        }

        [HttpPost]
        public ActionResult AppointSales(int[] checkedRecords, int companyrelationshipid)
        {
            var c = CH.GetAllData<CompanyRelationship>(i => i.ID == companyrelationshipid, "Members").FirstOrDefault();

            if (c != null)
            {
                c.Members.Clear();
                if (checkedRecords != null)
                {
                    foreach (int i in checkedRecords)
                    {
                        if (!c.Members.Any(m => m.ID == i))
                        {
                            var mem = CH.GetDataById<Member>(i);
                            c.Members.Add(mem);

                        }
                    }
                }
                CH.Edit<CompanyRelationship>(c);
            }

            return RedirectToAction("Management", "Project", new { id = c.ProjectID, tabindex = 3 });
        }
        #endregion

        #region 分配字头
        public ActionResult Distribution(int? projectid)
        {
            var member = CH.GetAllData<Member>(i => i.ProjectID == projectid);
            var dc = CRM_Logical.GetDefaultCharatracter();
            member.ForEach(m =>
            {
                dc.RemoveAll(i => m.CharactersSet.Contains(i.ToUpper()));
            });
            ViewBag.ProjectID = projectid;
            ViewBag.DC = dc;

            return View(member);
        }

        [HttpPost]
        public ActionResult Distribution(List<string> mc, int? projectid)
        {
            //清空原有的字头分配
            List<Member> temp = CH.GetAllData<Member>(m => m.ProjectID == projectid);
            temp.ForEach(m =>
            {
                m.Characters = string.Empty;
            });

            if (mc != null)
            {
                mc.ForEach(child =>
                {
                    //解析提交的数据
                    var data = child.Split('|');
                    var id = Int32.Parse(data[0]);
                    var value = data[1];


                    Member member = temp.FirstOrDefault(m => m.ID == id);

                    if (string.IsNullOrEmpty(member.Characters))
                        member.Characters = value;
                    else
                        member.Characters += "|" + value;
                });

            }

            //把更新的charatacter写到数据库
            temp.ForEach(tm =>
            {
                CH.Edit<Member>(tm);
            });

            return RedirectToAction("Management", "Project", new { id = projectid, tabindex = 1 });

        }
        #endregion

        #region 出单
        //public ViewResult Deals(int? projectid)
        //{
        //    return View(CH.GetAllData<Deal>(d => d.ProjectID == projectid));
        //}

        [HttpPost]
        public ActionResult Deals(DateTime? startdate, DateTime? enddate, int? projectid)
        {

            return RedirectToAction("Management", new { id = projectid, tabindex = 4, dealstartdate = startdate, dealenddate = enddate });
        }
        #endregion

        public ViewResult Details(int id)
        {
            var data = CH.GetDataById<Project>(id);
            data.SaleBrief = HttpUtility.HtmlDecode(data.SaleBrief);
            return View(data);
        }

        public ActionResult Create()
        {
            return View();
        }


        public ActionResult Management(int? id, int? tabindex, DateTime? dealstartdate, DateTime? dealenddate, int? memberFilterForCompany)
        {
            var Data = CH.GetAllData<Project>(i => i.ID == id).FirstOrDefault();
            ViewBag.DealStartDate = dealstartdate;
            ViewBag.DealEndDate = dealenddate;
            ViewBag.TabIndex = tabindex;
            if (memberFilterForCompany.HasValue)
            {
                int memberId = memberFilterForCompany.Value;
                ViewBag.MemberFilterForCompany = memberFilterForCompany.ToString();
                if (memberId == -1)
                {
                    Data.CompanyRelationships = Data.CompanyRelationships.Where(c => c.Members.Count == 0).ToList();
                }
                else if (memberId == -2)
                {
                    Data.CompanyRelationships = Data.CompanyRelationships.Where(c => c.Members.Count != 0).ToList();
                }
                else
                {
                    Data.CompanyRelationships = Data.CompanyRelationships.Where(c => c.Members.Any(m => m.ID == memberId)).ToList();
                }
            }
            else
            {
                ViewBag.MemberId = "";
            }

            return View(Data);
        }

        [HttpPost]
        public ActionResult AssignCompanies(string selectedCompanies, string selectedMembers)
        {
            if (string.IsNullOrWhiteSpace(selectedCompanies))
            {
                return Content("请选择公司!");
            }
            if (string.IsNullOrWhiteSpace(selectedMembers))
            {
                return Content("请选择销售人员!");
            }
            selectedCompanies = selectedCompanies.Replace("on,", "");
            IEnumerable<int> companyIDs = selectedCompanies.Split(',').Select(c => int.Parse(c));
            IEnumerable<int> memberIDs = selectedMembers.Split(',').Select(c => int.Parse(c));
            foreach (int companyID in companyIDs)
            {
                var company = CH.GetAllData<CompanyRelationship>(i => i.ID == companyID, "Members").FirstOrDefault();
                if (company != null)
                {
                    foreach (int memberID in memberIDs)
                    {
                        if (company.Members.All(m => m.ID != memberID))
                        {
                            var mem = CH.GetDataById<Member>(memberID);
                            company.Members.Add(mem);
                        }
                    }
                }
                CH.Edit<CompanyRelationship>(company);
            }

            return Content("分配成功!");
        }

        [HttpPost]
        public ActionResult UnsignCompanies(string selectedCompanyFilter, string selectedCompanies)
        {
            if (string.IsNullOrWhiteSpace(selectedCompanies))
            {
                return Content("请选择公司!");
            }
            if (string.IsNullOrWhiteSpace(selectedCompanyFilter))
            {
                return Content("请选择销售人员!");
            }

            int memberID = int.Parse(selectedCompanyFilter);
            selectedCompanies = selectedCompanies.Replace("on,", "");
            IEnumerable<int> companyIDs;
                if(selectedCompanies.Contains("all"))
                {
                    var pid = Int32.Parse(selectedCompanies.Replace("all",""));
                    var coms = CH.GetAllData<CompanyRelationship>(c=>c.ProjectID ==pid );
                    companyIDs = coms.Where(c=>c.Members!=null).Select(s => s.ID);
                }
                else
               companyIDs = selectedCompanies.Split(',').Select(c => int.Parse(c));
            foreach (int companyID in companyIDs)
            {
                var company = CH.GetAllData<CompanyRelationship>(i => i.ID == companyID, "Members").FirstOrDefault();
                if (company != null)
                {
                    if (memberID == -2)
                    {
                        company.Members.Clear();
                        CH.Edit<CompanyRelationship>(company);
                    }
                    else
                    {
                        var mem = CH.GetDataById<Member>(memberID);
                        if (mem != null)
                        {
                            company.Members.Remove(mem);
                            CH.Edit<CompanyRelationship>(company);
                        }
                    }
                    
                }
                
            }
            return Content("取消分配成功!");
        }
        [HttpPost]
        public ActionResult Create(Project item, IEnumerable<HttpPostedFileBase> attachments)
        {
            this.AddErrorIfAllNamesEmpty(item);
            this.AddErrorStateIfStartDateLaterThanEndDate(item.StartDate, item.EndDate);
            if (ModelState.IsValid)
            {
                if (attachments != null)
                {
                    if (string.IsNullOrEmpty(item.SalesBriefName))
                        item.SalesBriefName = "销售简介";
                    foreach (var file in attachments)
                    {
                        var fileName = Path.GetFileName(file.FileName).Replace(" ", ""); ;
                        string serverpath = "/Uploads/Projects/" + item.ProjectCode + "/Salesbrief";
                        string path = Server.MapPath(serverpath);
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        var physicalPath = Path.Combine(path, fileName);
                        file.SaveAs(physicalPath);
                        item.SalesBriefUrl = serverpath + "/" + fileName;
                    }
                }

                CH.Create<Project>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            var data = CH.GetDataById<Project>(id);
            data.SaleBrief = HttpUtility.HtmlDecode(data.SaleBrief);
            return View(data);
        }

        [HttpPost]
        public ActionResult Edit(Project item, IEnumerable<HttpPostedFileBase> attachments)
        {
            this.AddErrorIfAllNamesEmpty(item);
            this.AddErrorStateIfStartDateLaterThanEndDate(item.StartDate, item.EndDate);
            if (ModelState.IsValid)
            {
                if (attachments != null)
                {
                    if (string.IsNullOrEmpty(item.SalesBriefName))
                        item.SalesBriefName = "销售简介";

                    foreach (var file in attachments)
                    {
                        var fileName = Path.GetFileName(file.FileName).Replace(" ", ""); ;
                        string serverpath = "/Uploads/Projects/" + item.ProjectCode + "/Salesbrief";
                        string path = Server.MapPath(serverpath);
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        var physicalPath = Path.Combine(path, fileName);
                        file.SaveAs(physicalPath);
                        item.SalesBriefUrl = serverpath + "/" + fileName;
                    }
                }

                CH.Edit<Project>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            var data = CH.GetDataById<Project>(id);
            data.SaleBrief = HttpUtility.HtmlDecode(data.SaleBrief);
            return View(data);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var p = CH.GetAllData<Project>(i => i.ID == id).FirstOrDefault();
            p.Templates.ForEach(t =>
            {
                CH.Delete<Template>(t.ID);
            });

            var lcs = CH.GetAllData<LeadCall>(l => l.ProjectID == id);
            lcs.ForEach(l =>
            {
                CH.Delete<LeadCall>(l.ID);
            });

            p.Messages.ForEach(m =>
            {
                CH.Delete<Message>(m.ID);
            });
            p.News.ForEach(m =>
            {
                CH.Delete<News>(m.ID);
            });
            p.TargetOfWeeks.ForEach(t =>
            {
                CH.Delete<TargetOfWeek>(t.ID);
            });
            p.TargetOfMonths.ForEach(t =>
            {
                CH.Delete<TargetOfMonth>(t.ID);
            });

            p.CompanyRelationships.ForEach(t =>
            {
                CH.Delete<CompanyRelationship>(t.ID);
            });
            p.Members.ForEach(t =>
            {
                CH.Delete<Member>(t.ID);
            });
            p.Categorys.ForEach(t =>
            {
                CH.Delete<Category>(t.ID);
            });
            var deals = CH.GetAllData<Deal>(d => d.ProjectID == p.ID);
            deals.ForEach(d =>
            {
                CH.Delete<Deal>(d.ID);
            });
            p.PhoneSaleSupports.ForEach(t =>
            {
                CH.Delete<PhoneSaleSupport>(t.ID);
            });

            CH.Delete<Project>(id);
            return RedirectToAction("Index");
        }

        public ActionResult Service_File_Donwload(string fileurl, string filename)
        {
            string filePath = Request.MapPath(fileurl);
            if (System.IO.File.Exists(filePath))
            {
                return new DownloadResult { VirtualPath = fileurl, FileDownloadName = filename };
            }

            return View(SR.ErrorView, null, SR.CannotDownload);
        }

        public ActionResult LeadDelete(int id, int projectid)
        {
            var calls = CH.GetAllData<LeadCall>(l => l.LeadID == id && l.ProjectID == projectid);
            calls.ForEach(c =>
            {
                CH.Delete<LeadCall>(c.ID);
            });
            ViewBag.ProjectID = projectid;
            return View(CH.GetDataById<Lead>(id));
        }

        [HttpPost, ActionName("LeadDelete")]
        public ActionResult DeleteConfirmed(int id, int projectid)
        {
            CH.Delete<Lead>(id);
            return RedirectToAction("Management", "Project", new { id = projectid, tabindex = 3 });
        }

        [HttpGet]
        public ActionResult AssignCompany(int? projectId, int? memberFilterForCompany)
        {
            Project project = null;
            if (projectId.HasValue)
            {
                project = CH.GetAllData<Project>(i => i.ID == projectId.Value).FirstOrDefault();
            }
            else
            {
                if (Employee.CurrentRole.Level == 1000)
                {
                    project = CH.DB.Projects.FirstOrDefault();
                }
                else
                {
                    project = CH.DB.Projects.Where(w => w.Members.Select(s => s.Name).Contains(Employee.CurrentUserName) == true).FirstOrDefault();
                }
            }
            if (memberFilterForCompany.HasValue)
            {
                int memberId = memberFilterForCompany.Value;
                ViewBag.MemberFilterForCompany = memberFilterForCompany.ToString();
                if (memberId == -1)
                {
                    project.CompanyRelationships = project.CompanyRelationships.Where(c => c.Members.Count == 0).ToList();
                }
                else if (memberId == -2)
                {
                    project.CompanyRelationships = project.CompanyRelationships.Where(c => c.Members.Count != 0).ToList();
                }
                else
                {
                    project.CompanyRelationships = project.CompanyRelationships.Where(c => c.Members.Any(m => m.ID == memberId)).ToList();
                }
            }
            else
            {
                ViewBag.MemberId = "";
            }

            return View(project);
        }
    }
}