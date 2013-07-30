using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utl;
namespace Sales.Controllers
{
    public class ProtectedCompanysController : Controller
    {
        /// <summary>
        /// 已经出单的公司
        /// </summary>
        /// <param name="projectid"></param>
        /// <returns></returns>
        public ActionResult CompanyIndex(int? projectid)
        {
            ViewBag.Right = ReviewRight.ProtectedCompanyReview.ToString();
             projectid = this.TrySetProjectIDForUser(projectid);
            ViewBag.ProjectID = projectid;
            if (projectid != null)
            {
                var cs = from c in CH.DB.CompanyRelationships where (c.Deals.Count > 0 || c.Progress.Code >= 40) && c.ProjectID == projectid select c;
                var data = cs.ToList();
                //var cs = from c in CH.DB.CompanyRelationships where c.ProjectID == projectid select c;

                return View(cs);
            }
            return View();
        }

    }
}
