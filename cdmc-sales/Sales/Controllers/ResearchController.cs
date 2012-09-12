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
    public class ResearchController : Controller
    {
        public ViewResult Index()
        {
            return View(CH.GetAllData<Research>());
        }

        public ViewResult Details(int id)
        {
            var data = CH.GetDataById<Research>(id);
            data.Contents = HttpUtility.HtmlDecode(data.Contents);
            return View(data);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Research item)
        {
            if (ModelState.IsValid)
            {
               // item.Contents = HttpUtility.HtmlEncode(item.Contents);
                CH.Create<Research>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            var data = CH.GetDataById<Research>(id);
            data.Contents = HttpUtility.HtmlDecode(data.Contents);
            return View(data);
        }

        [HttpPost]
        public ActionResult Edit(Research item)
        {
            
            if (ModelState.IsValid)
            {
                //item.Contents = HttpUtility.HtmlDecode(item.Contents);
              
                CH.Edit<Research>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            var data = CH.GetDataById<Research>(id);
            data.Contents = HttpUtility.HtmlDecode(data.Contents);
            return View(data);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<Research>(id);
            return RedirectToAction("Index");
        }
    }
}