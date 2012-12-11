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
using BLL;
using System.IO;

namespace Sales.Controllers
{

    [ManagerRequired]
    public class ProjectController : Controller
    {
      
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
            var data = CH.GetAllData<Project>(i=>i.ID!=projectid);
            if (!string.IsNullOrEmpty(p.References))
            {
                var refers = p.References.Split('|');
                data = data.FindAll(d => refers.Contains(d.ProjectCode) == false && d.ProjectCode != p.ProjectCode);
            }
            else
            {
                data = data.FindAll(d =>  d.ProjectCode != p.ProjectCode);
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
                var p = CH.GetDataById<Project>(projectid, "CompanyRelationships");
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
            var data = CH.GetDataById<Project>(id, "Categorys");
            data.SaleBrief = HttpUtility.HtmlDecode(data.SaleBrief);
            return View(data);
        }

        public ActionResult Create()
        {
            return View();
        }


        public ActionResult Management(int? id, int? tabindex, DateTime? dealstartdate, DateTime? dealenddate)
        {
            ViewBag.DealStartDate = dealstartdate;
            ViewBag.DealEndDate = dealenddate;
            ViewBag.TabIndex = tabindex;
            var Data = CH.GetAllData<Project>(i => i.ID == id, "CompanyRelationships","Categorys", "Messages","Members", "Templates", "Messages", "TargetOfMonths").FirstOrDefault();
            return View(Data);
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
            var p = CH.GetAllData<Project>(i => i.ID == id, "CompanyRelationships", "Categorys", "Messages", "Members", "Templates", "Messages", "TargetOfMonths", "PhoneSaleSupports").FirstOrDefault();
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
            var deals = CH.GetAllData<Deal>(d=>d.ProjectID==p.ID);
            deals.ForEach(d => {
                CH.Delete<Deal>(d.ID);
            });
            p.PhoneSaleSupports.ForEach(t =>
            {
                CH.Delete<PhoneSaleSupport>(t.ID);
            });
          
            CH.Delete<Project>(id);
            return RedirectToAction("Index");
        }

        public ActionResult Service_File_Donwload(string fileurl,string filename)
        {
            return new DownloadResult { VirtualPath = fileurl, FileDownloadName = filename };
        }

        public ActionResult LeadDelete(int id, int projectid)
        {
            ViewBag.ProjectID = projectid;
            return View(CH.GetDataById<Lead>(id));
        }

        [HttpPost, ActionName("LeadDelete")]
        public ActionResult DeleteConfirmed(int id, int projectid)
        {
            CH.Delete<Lead>(id);
            return RedirectToAction("Management", "Project", new { id = projectid, tabindex = 3 });
        }
    }
}