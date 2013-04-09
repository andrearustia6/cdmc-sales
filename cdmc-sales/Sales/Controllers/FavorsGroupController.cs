using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Entity;
using Utl;

namespace Sales.Controllers
{
    public class FavorsGroupController : Controller
    {
        //
        // GET: /FavorsGroup/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(UserFavorsCrmGroup item)
        {
            item.UserName = Employee.CurrentUserName;
            CH.Create<UserFavorsCrmGroup>(item);
            return RedirectToAction("index", "salesex");
        }

        //[HttpPost]
        //public ActionResult AddGroupPopup(string groupname, string groupdescription)
        //{
        //    var item = new UserFavorsCrmGroup() { UserName = Employee.CurrentUserName, DisplayName = groupname, Description = groupdescription };
        //    CH.Create<UserFavorsCrmGroup>(item);
        //    return View();
        //}



    }
}
