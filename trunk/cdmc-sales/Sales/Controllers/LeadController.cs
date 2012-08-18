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
    public class LeadController : Controller
    {
        public ViewResult Index()
        {
            return View();
        }
        //[GridAction]
        //public ActionResult AjaxClientIndex()
        //{
        //    var data = CH.GetAllData<Client>();
        //    return View(new GridModel(data));
        //}
        [GridAction()]
        public ActionResult AjaxClientIndex()
        {
            var data = CH.GetAllData<Lead>();
            return new DataJsonResult<Lead>() { Data = data };
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<Lead>(id));
        }

        public ActionResult Create(int? companyid)
        {
            ViewBag.CompanyID = companyid;
            return View();
        }

        [HttpPost]
        public ActionResult Create(Lead item)
        {
            if (ModelState.IsValid)
            {
                Image image = ImageController.UploadImg(Request, item.Image);
                item.ImageID = image.ID;
                CH.Create<Lead>(item);
                return RedirectToAction("Index");
            }
            ViewBag.CompanyID = item.CompanyID;
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<Lead>(id));
        }

        [HttpPost]
        public ActionResult Edit(Lead item)
        {
            if (ModelState.IsValid)
            {
                ImageController.UploadImg(Request, item.Image);
                CH.Edit<Lead>(item);
                return RedirectToAction("Index");
            }
            ViewBag.CompanyID = item.CompanyID;
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<Lead>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<Lead>(id);
            return RedirectToAction("Index");
        }
    }
}