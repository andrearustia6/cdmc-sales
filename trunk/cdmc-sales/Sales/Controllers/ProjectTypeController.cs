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
using Telerik.Web.Mvc;

namespace Sales.Controllers
{
    [DirectorRequired]
    public class ProjectTypeController : Controller
    {
        protected override void Dispose(bool disposing)
        {
            CH.DB.Dispose();
            base.Dispose(disposing);
        }


        public ActionResult index()
        {
            return View();

        }
        [GridAction]
        public ActionResult _SelectIndex()
        {
            List<ProjectType> list;
            list = CH.GetAllData<ProjectType>();
            return View(new GridModel(list));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveAjaxEditing(int id)
        {
            var item = CH.GetDataById<ProjectType>(id);
            if (TryUpdateModel(item))
            {
                CH.Edit<ProjectType>(item);
            }
            return View(new GridModel(CH.GetAllData<ProjectType>()));
        }
        [AcceptVerbs(HttpVerbs.Post)]

        [GridAction]
        public ActionResult _InsertAjaxEditing()
        {
            var item = new ProjectType();

            if (TryUpdateModel(item))
            {
                CH.Create<ProjectType>(item);
            }
            //Rebind the grid       
            return View(new GridModel(CH.GetAllData<ProjectType>()));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _DeleteAjaxEditing(int id)
        {
            CH.Delete<ProjectType>(id);
            return View(new GridModel(CH.GetAllData<ProjectType>()));
        }

    }
}