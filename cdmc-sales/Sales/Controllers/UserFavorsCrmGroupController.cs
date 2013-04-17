using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using Entity;
using Utl;
using Sales.Model;
using Model;

namespace Sales.Controllers
{
    public class UserFavorsCrmGroupController : Controller
    {
        //
        // GET: /UserFavorsCrmGroup/

        public ActionResult Index()
        {
            return View();
        }
        IQueryable<AjaxGroupedCRM> DataQuery()
        {
            var username = Employee.CurrentUserName;
            var d = from i in CH.DB.UserFavorsCrmGroups.Where(w => w.UserName == username)
                    select new AjaxGroupedCRM()
            {
                ID = i.ID,
                DisplayName = i.DisplayName,
                UserName = i.UserName,
                Description = i.Description,
                CreatedDate = i.CreatedDate,
                Creator = i.Creator,
                Sequence = i.Sequence,
                ModifiedDate = i.ModifiedDate,
                ModifiedUser = i.ModifiedUser
            };
            return d;

        }
        [GridAction]
        public ActionResult _Select()
        {
            return View(new GridModel(DataQuery()));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _Update(int id)
        {
            var item = CH.GetDataById<UserFavorsCrmGroup>(id);
            if (TryUpdateModel(item))
            {
                CH.Edit<UserFavorsCrmGroup>(item);
            }
          
            return View(new GridModel(DataQuery()));
        }
        [AcceptVerbs(HttpVerbs.Post)]

        [GridAction]
        public ActionResult _Insert()
        {
            var item = new UserFavorsCrmGroup();
           
            if (TryUpdateModel(item))
            {
                item.UserName = Employee.CurrentUserName;
                CH.Create<UserFavorsCrmGroup>(item);
            }
            
            return View(new GridModel(DataQuery()));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _Delete(int id)
        {
            var d = CH.GetDataById<UserFavorsCrmGroup>(id);
            var list = d.UserFavorsCRMs;
            foreach (var i in list)
            {
                CH.DB.Set<UserFavorsCRM>().Remove(i);
            }
            CH.DB.SaveChanges();
            CH.Delete<UserFavorsCrmGroup>(id);
           
            return View(new GridModel(DataQuery()));
        }

    }
}
