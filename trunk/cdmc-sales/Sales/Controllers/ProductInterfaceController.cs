using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utl;
using Entity;
using BLL;

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

            projectid = this.TrySetProjectIDForProduct(projectid);
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

            projectid = this.TrySetProjectIDForProduct(projectid);
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


        #region 项目公司
        public ViewResult MyProjectIndex()
        {
            var ps = CRM_Logical.GetProductInvolveProject();
            return View(ps);
        }
        public ViewResult UpdateSalesBrief(int? id)
        {
            ViewBag.ProjectID = id;
            var data = CH.GetDataById<Project>(id).SaleBrief;
            return View("UpdateSalesBrief","",HttpUtility.HtmlDecode(data));
        }
        [HttpPost]
        public ActionResult UpdateSalesBrief(int? id,string salesbrief)
        {
            var data = CH.GetDataById<Project>(id);
            data.SaleBrief = salesbrief;
            CH.Edit<Project>(data);
            return RedirectToAction("MyProjectIndex"); 
        }
        #endregion
    }
}
