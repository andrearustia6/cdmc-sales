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

namespace Sales.Controllers
{
    public class TemplateController : Controller
    {

        public ViewResult Index()
        {
            return View(CH.GetAllData<Template>());
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<Template>(id));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Template item)
        {
            if (ModelState.IsValid)
            {
                item.Content = HttpUtility.HtmlDecode(item.Content);
                CH.Create<Template>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<Template>(id));
        }

        [HttpPost]
        public ActionResult Edit(Template item)
        {
            if (ModelState.IsValid)
            {
                item.Content = HttpUtility.HtmlDecode(item.Content);
                CH.Edit<Template>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<Template>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<Template>(id);
            return RedirectToAction("Index");
        }
    }
}