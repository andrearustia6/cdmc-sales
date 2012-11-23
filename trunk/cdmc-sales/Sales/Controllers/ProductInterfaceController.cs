using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utl;
using Entity;
using BLL;
using System.IO;

namespace Sales.Controllers
{
    [ProductInterfaceRequired(AccessType = AccessType.Equal)]
    public class ProductInterfaceController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></return
        public ViewResult CompanyRelationshipIndex(int? projectid)
        {

            projectid = this.TrySetProjectIDForUser(projectid);
            var project = CH.GetDataById<Project>(projectid, "CompanyRelationships");
            if (project != null)
            {
                return View(project);
            }
            else
            {
                return View();
            }

        }

        #region message


        public ViewResult MyMessageIndex(int? projectid)
        {

            projectid = this.TrySetProjectIDForUser(projectid);
            ViewBag.ProjectID = projectid;
            if (projectid != null)
            {
                var ms = CH.GetAllData<Message>(m => m.ProjectID == projectid);
                return View(ms);
            }
            else
            {
                return View();
            }


        }

        public ViewResult AddMessage(int? projectid)
        {
            ViewBag.ProjectID = projectid;
            return View();
        }


        [HttpPost]
        public ActionResult AddMessage(Message item)
        {
            var project = CH.GetDataById<Project>(item.ProjectID);
            if (ModelState.IsValid)
            {
                var last = CH.GetAllData<Message>(m => !string.IsNullOrEmpty(m.FlowNumber) && m.FlowNumber.Contains(project.ProjectCode)).OrderByDescending(o => o.CreatedDate).FirstOrDefault();
                string procode;



                if (last == null)
                {

                    procode = item.FlowNumber = project.ProjectCode + "1";

                }
                else
                {
                    string number = item.FlowNumber.Replace(item.Project.ProjectCode, "");
                    int n = 0;
                    Int32.TryParse(number, out n);
                    n = n + 1;
                    item.FlowNumber = item.Project.ProjectCode + n.ToString();
                }

                item.Member = User.Identity.Name;
                var p = CH.GetDataById<Project>(item.ProjectID, "Members");
                var member = p.GetMemberInProjectByName(item.Member);

                item.SalesTypeID = member==null? 1:member.SalesTypeID;
                CH.Create<Message>(item);
                return RedirectToAction("MyMessageIndex");
            }
            return View(item);
        }

        public ViewResult EditMessage(int? id)
        {
            return View(CH.GetDataById<Message>(id));
        }

        [HttpPost]
        public ActionResult EditMessage(Message item)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<Message>(item);
                return RedirectToAction("MyMessageIndex");
            }
            return View(item);
        }

        public ViewResult DisplayMessage(int? id, int? projectid)
        {
            return View(CH.GetDataById<Message>(id));
        }

        #endregion


        #region 项目简介
        public ViewResult MyProjectIndex()
        {
            var ps = CRM_Logical.GetProductInvolveProject();
            return View(ps);
        }
        public ViewResult UpdateSalesBrief(int? id)
        {
            ViewBag.ProjectID = id;
            var data = CH.GetDataById<Project>(id);
            ViewBag.SalesBriefName = data.SalesBriefName;
            ViewBag.SalesBriefUrl = data.SalesBriefUrl;
            return View("UpdateSalesBrief","",HttpUtility.HtmlDecode(data.SaleBrief));
        }
        [HttpPost]
        public ActionResult UpdateSalesBrief(int? id, string salesbrief, string salesBriefName, IEnumerable<HttpPostedFileBase> attachments)
        {
            var data = CH.GetDataById<Project>(id);
            data.SaleBrief = salesbrief;
            data.SalesBriefName = salesBriefName;
            if (attachments != null)
            {
                foreach (var file in attachments)
                {
                    var fileName = Path.GetFileName(file.FileName).Replace(" ", ""); ;
                    string serverpath = "/Uploads/Projects/" + data.ProjectCode + "/Salesbrief";
                    string path = Server.MapPath(serverpath);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    var physicalPath = Path.Combine(path, fileName);
                    file.SaveAs(physicalPath);
                    data.SalesBriefUrl = serverpath + "/" + fileName;
                }
            }

            CH.Edit<Project>(data);
            return RedirectToAction("MyProjectIndex"); 
        }
        #endregion

        #region company
        public ActionResult AddCompany(int? projectid = null)
        {
            if (projectid == null) return RedirectToAction("CompanyRelationshipIndex", "productinterface");

            this.AddErrorStateIfSalesNoAccessRightToTheProject(projectid);

            if (ModelState.IsValid)
            {
                ViewBag.ProjectID = projectid;
                return View();
            }
            else
                return RedirectToAction("CompanyRelationshipIndex", "productinterface", new { projectid = projectid });
        }

        [HttpPost]
        public ActionResult AddCompany(Company item, int? projectid, int[] checkedCategorys)
        {
            projectid = this.TrySetProjectIDForUser(projectid);
            ViewBag.ProjectID = projectid;


            this.AddAddErrorStateIfOneOfNameExist<Company>(item.Name_EN, item.Name_CH);
            this.AddErrorIfAllNamesEmpty(item);
            if (ModelState.IsValid)
            {
                List<Category> lc = new List<Category>();
                if (checkedCategorys != null)
                {
                    lc = CH.GetAllData<Category>(i => checkedCategorys.Contains(i.ID));
                }

                var p = CH.GetDataById<Project>(projectid, "Members");
                CH.Create<Company>(item);
                var ms = new List<Member>();
                ms.Add(p.GetMemberInProjectByName(User.Identity.Name));
                var cr = new CompanyRelationship() { CompanyID = item.ID, ProjectID = projectid, Importancy = 1, Members = ms, Categorys = lc };
                CH.Create<CompanyRelationship>(cr);
                return RedirectToAction("CompanyRelationshipIndex", "productinterface", new { projectid = projectid });

            }
            else
                return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ViewResult EditCompany(int? crid)
        {
            this.AddErrorStateIfSalesNoAccessRightToTheCRM(crid);
            if (ModelState.IsValid)
            {
                var cr = CH.GetDataById<CompanyRelationship>(crid);
                ViewBag.CompanyRelationshipID = cr.ID;
                ViewBag.ProjectID = cr.ProjectID;
                return View(cr.Company);
            }
            else
                return View();
        }

        /// <summary>
        /// id = companyrelationshipid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ViewResult DisplayCompany(int? crid)
        {
            this.AddErrorStateIfSalesNoAccessRightToTheCRM(crid);
            var cr = CH.GetDataById<CompanyRelationship>(crid);
            ViewBag.COmpanyRelationshipID = cr.ID;
            if (ModelState.IsValid)
                return View(cr.Company);
            else
                return View();
        }

        [HttpPost]
        public ActionResult EditCompany(Company item, int? crid, int? projectid, int[] checkedCategorys)
        {
            this.AddErrorIfAllNamesEmpty(item);


            if (ModelState.IsValid)
            {
                var cr = CH.GetDataById<CompanyRelationship>(crid, "Categorys");
                List<Category> lc = new List<Category>();
                if (checkedCategorys != null)
                {
                    cr.Categorys.Clear();
                    lc = CH.GetAllData<Category>(i => checkedCategorys.Contains(i.ID));
                    cr.Categorys.AddRange(lc);
                    CH.Edit<CompanyRelationship>(cr);
                }

                CH.Edit<Company>(item);
                return RedirectToAction("CompanyRelationshipIndex", "productinterface", new { projectid = projectid });
            }

            ViewBag.ProjectID = projectid;
            return View();
        }

       
        #endregion  

        public ActionResult Service_File_Donwload(string fileurl, string filename)
        {
            return new DownloadResult { VirtualPath = fileurl, FileDownloadName = filename };
        }
    }
}
