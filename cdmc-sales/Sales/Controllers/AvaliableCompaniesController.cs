using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sales.BLL;
using Sales.Model;
namespace Sales.Controllers
{
    public class AvaliableCompaniesController : Controller
    {
        public ActionResult Index(int? projectid)
        {
            var data = AvaliableCRM.GetAvaliableCompanies(projectid);
            return View(data);
        }
        /// <summary>
        /// 导航选中的公司或者lead
        /// </summary>
        /// <param name="indexs"></param>
        /// <returns></returns>
        public PartialViewResult _SelectedCRMNode(string crmid)
        {
            var data = AvaliableCRM._CRMGetAvaliableCrmDetail(int.Parse(crmid));
            return PartialView(@"~\views\AvaliableCompanies\DetailContainer.cshtml", data);
        }

        public PartialViewResult GetCompetitor(string crmid)
        {
            var data = AvaliableCRM._CRMGetAvaliableCrmDetail(int.Parse(crmid));
            return PartialView("Competitor", data);
        }
        public PartialViewResult GetPitchPoint(string crmid)
        {
            var data = AvaliableCRM._CRMGetAvaliableCrmDetail(int.Parse(crmid));
            return PartialView("PitchPoint", data);
        }
    }
}
