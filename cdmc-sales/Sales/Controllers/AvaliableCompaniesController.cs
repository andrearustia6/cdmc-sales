using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sales.BLL;

namespace Sales.Controllers
{
    public class AvaliableCompaniesController : Controller
    {
        public ActionResult Index(int? projectid)
        {
            var data = AvaliableCRM.GetAvaliableCompanies(projectid);
            return View(data);
        }

    }
}
