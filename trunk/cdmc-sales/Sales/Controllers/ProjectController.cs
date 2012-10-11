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

namespace Sales.Controllers
{

    public class ProjectController : Controller
    {
        [DirectorRequired]
        public ViewResult Index()
        {
            return View(CH.GetAllData<Project>());
        }

        public ViewResult GotoReports()
        {
            return View();
        }

        [ManagerRequired]
        public ViewResult Reports(DateTime? setdate)
        {
            if (setdate == null)
                return View();
            var projects = CH.GetAllData<Project>("Members", "TargetOfWeeks", "Leads", "Deals", "Companys", "Leads");
            var weeklyreports = CRM_Logical.GenerateWeeklyReports(projects, setdate);
            return View(weeklyreports);
        }
        [ManagerRequired]
        public ViewResult Compare(DateTime? setdate, int? projectid)
        {
            if (setdate == null)
                return View();
            var projects = CH.GetAllData<Project>("Members", "TargetOfWeeks", "Leads", "Deals", "Companys", "Leads");
            var weeklyreports = CRM_Logical.GenerateWeeklyReports(projects, setdate);
            var rp = weeklyreports.FirstOrDefault(w => w.Project.ID == projectid);
            return View(rp.MemberItems);
        }

        #region 添加公司
        [ProjectInformationAccess]
        public ViewResult SelectCompanyByProjectCode(int projectid)
        {
            ViewBag.ProjectID = projectid;
            return View(CH.GetAllData<Project>());
        }

        [ProjectInformationAccess]
        public ViewResult AddToCompanyRelationship(int projectid)
        {
            ViewBag.ProjectID = projectid;
            return View(CH.GetAllData<CompanyRelationship>(c => c.ProjectID == projectid));
        }

        [ProjectInformationAccess]
        [HttpPost]
        public ActionResult AddToCompanyRelationship(int projectid, string enname, string description, int importancy)
        {

            ViewBag.ProjectID = projectid;
            var project = CH.GetDataById<Project>(projectid, "CompanyRelationships");
            Company company = CH.GetAllData<Company>(co => co.Name_EN == enname).FirstOrDefault();
            if (company == null)
            {
                company = new Company() { Name_EN = enname, Creator = User.Identity.Name, Description = description };
                CRM_Logical.TryAddCompany(company);
            }

            CompanyRelationship cr1 = new CompanyRelationship() { CompanyID = company.ID, ProjectID = projectid, Importancy = importancy };
            project.CompanyRelationships.Add(cr1);
            CH.Edit<Project>(project);
            //return RedirectToAction("Management", new { tabindex = 3, id = projectid });

            return View(CH.GetAllData<CompanyRelationship>(c => c.ProjectID == projectid));
        }

        [ProjectInformationAccess]
        [HttpPost]
        public ActionResult SelectCompanyByProjectCode(int[] checkedRecords, int projectid)
        {
            var ps = CH.GetAllData<Project>("CompanyRelationships");
            var allselectedprojects = ps.FindAll(item => checkedRecords.Any(cr => cr == item.ID));
            var p = ps.FirstOrDefault(i => i.ID == projectid);
            if (p != null)
            {

                //var alreadyrefers = p.References==null?new string[]{}:p.References.Split('|');
                //var alreadyreferprojects = allselectedprojects.FindAll(item => alreadyrefers.Any(already => already == item.ProjectCode));
                //var notreferprojects = allselectedprojects.FindAll(item => alreadyrefers.Any(already => already != item.ProjectCode));

                allselectedprojects.ForEach(nr =>
                {
                    nr.CompanyRelationships.ForEach(cr =>
                    {
                        CRM_Logical.TryAddCompanyRelationship(new CompanyRelationship() { CompanyID = cr.CompanyID, ProjectID = p.ID }, p.ID);
                    });
                });
                string refer = string.Empty;
                allselectedprojects.ForEach(select => {
                    if (string.IsNullOrEmpty(refer))
                    {
                        refer = select.ProjectCode;
                    }
                    else
                    {
                        refer += "|"+ select.ProjectCode;
                    }

                });
                p.References = refer;
                // 没有删除逻辑
                CH.Edit<Project>(p);
            }

            return RedirectToAction("Management", "Project", new { id = projectid });
        }


        //public ViewResult SelectCompany(int? projectid)
        //{
        //    ViewBag.ProjectID = projectid;
        //    return View(CH.GetAllData<Company>());
        //}

        //[HttpPost]
        //public ActionResult SelectCompany(int[] checkedRecords, int projectid)
        //{
        //    var p = CH.GetAllData<Project>(i => i.ID == projectid, "Companys","Leads").FirstOrDefault();
        //    p.Companys.Clear();
        //    //p.Leads.Clear();
        //    if (p != null)
        //    {
        //        foreach (int i in checkedRecords)
        //        {
        //            if (!p.Companys.Any(c => c.ID == i))
        //            {
        //                var company = CH.GetAllData<Company>(c => c.ID == i, "Leads").FirstOrDefault();
        //                p.Companys.Add(company);
        //                //p.Leads.AddRange(company.Leads);

        //            }
        //        }
        //    }
        //    CH.Edit<Project>(p);
        //    return RedirectToAction("Management", "Project", new { id = projectid });
        //}
        #endregion

         #region 指定成员到公司
        public ViewResult AppointSales(int? projectid, int? companyrelationshipid)
        {
            var p = CH.GetDataById<Project>(projectid, "Members");
            ViewBag.CompanyRelationshipID = companyrelationshipid;
            return View(p.Members);
        }

        [HttpPost]
        public ActionResult AppointSales(int[] checkedRecords, int companyrelationshipid)
        {
            var c = CH.GetAllData<CompanyRelationship>(i => i.ID == companyrelationshipid, "Members").FirstOrDefault();
            
            if (c != null)
            {
                c.Members.Clear();
                foreach (int i in checkedRecords)
                {
                    if (!c.Members.Any(m => m.ID == i))
                    {
                        var mem = CH.GetDataById<Member>(i);
                        c.Members.Add(mem);

                    }
                }
                CH.Edit<CompanyRelationship>(c);
            }
           
            return RedirectToAction("Management", "Project", new { id = c.ProjectID, tabindex=2 });
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

            return RedirectToAction("Management", "Project", new { id = projectid });

        }
        #endregion

        #region 目标划分
        public ActionResult BreakdownIndex(int? projectid)
        {
            ViewBag.ProjectID = projectid;

            return View(CH.GetAllData<TargetOfMonth>(m => m.ProjectID == projectid));
        }
        public ActionResult Breakdown(int? projectid, int? targetofmonthid, string startdate)
        {
            ViewBag.ProjectID = projectid;
            ViewBag.TargetOfMonthID = targetofmonthid;
            ViewBag.OldStartDate = startdate;
            if (startdate != null)
            {

                var targets = CH.GetAllData<TargetOfWeek>(i => i.StartDate.ToShortDateString() == startdate).OrderBy(i => i.StartDate).ToList();
                ViewBag.Targets = targets;
            }

            return View(CH.GetAllData<Member>(m => m.ProjectID == projectid));
        }

        [HttpPost]
        public ActionResult Breakdown(List<string> mtw, int projectid, int TargetOfMonthid, DateTime startdate, DateTime enddate, string OldDate)
        {
            if (startdate.DayOfWeek != DayOfWeek.Monday || enddate.DayOfWeek != DayOfWeek.Friday)
            {
                ViewBag.ProjectID = projectid;
                ViewBag.TargetOfMonthID = TargetOfMonthid;
                if (startdate != null)
                {
                    var targets = CH.GetAllData<TargetOfWeek>(i => i.StartDate.ToShortDateString() == OldDate).OrderBy(i => i.StartDate).ToList(); ;
                    ViewBag.Targets = targets;
                }
                ModelState.AddModelError("", "开始时间不是周一或者结束时间不是周五");
                return View(CH.GetAllData<Member>(m => m.ProjectID == projectid));
            }
            if (mtw != null)
            {
                foreach (var v in mtw)
                {
                    var s = v.Split('|');
                    var name = s[0];
                    var value = s[1];
                    var ts = CH.GetAllData<TargetOfWeek>(t => t.Member == name && t.ProjectID == projectid && t.TargetOfMonthID == TargetOfMonthid && startdate == t.StartDate);
                    if (ts.Count == 0)
                    {
                        CH.Create<TargetOfWeek>(new TargetOfWeek() { Deal = Decimal.Parse(value), EndDate = enddate, Member = name, StartDate = startdate, ProjectID = projectid, TargetOfMonthID = TargetOfMonthid });
                    }
                    else
                    {
                        var item = ts.FirstOrDefault();
                        item.Deal = Decimal.Parse(value);
                        CH.Edit<TargetOfWeek>(item);
                    }
                }
            }

            return RedirectToAction("management", "project", new { id = projectid,tabindex=2 });
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
            return View(CH.GetDataById<Project>(id));
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
            var Data = CH.GetAllData<Project>(i => i.ID == id, "CompanyRelationships", "Members", "Templates", "Messages", "TargetOfMonths").FirstOrDefault();
            return View(Data);
        }

        [HttpPost]
        public ActionResult Create(Project item)
        {
            if (ModelState.IsValid)
            {
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
        public ActionResult Edit(Project item)
        {
            if (ModelState.IsValid)
            {
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
            CH.Delete<Project>(id);
            return RedirectToAction("Index");
        }
    }
}