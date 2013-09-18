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
    public class ManageContentController : Controller
    {
        protected override void Dispose(bool disposing)
        {
            CH.DB.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult Index()
        {
            return View();

        }
        [GridAction]
        public ActionResult _SelectIndex()
        {
            List<CommentContent> list = CH.GetAllData<CommentContent>();
            return View(new GridModel(list));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveAjaxEditing(int id)
        {
            var item = CH.GetDataById<CommentContent>(id);
            if (TryUpdateModel(item))
            {
                if (ModelState.IsValid)
                {
                    CH.Edit<CommentContent>(item);
                }
            }
            List<CommentContent> list = CH.GetAllData<CommentContent>();

            return View(new GridModel(list));
        }
        [AcceptVerbs(HttpVerbs.Post)]

        [GridAction]
        public ActionResult _InsertAjaxEditing()
        {
            var item = new CommentContent();

            if (TryUpdateModel(item))
            {
                if (ModelState.IsValid)
                {
                    CH.Create<CommentContent>(item);
                }
            }
            //Rebind the grid       
            List<CommentContent> list = CH.GetAllData<CommentContent>();
            return View(new GridModel(list));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _DeleteAjaxEditing(int id)
        {
            CH.Delete<CommentContent>(id);
            List<CommentContent> list = CH.GetAllData<CommentContent>();
            return View(new GridModel(list));
        }


    }
}