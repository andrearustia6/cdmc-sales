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
            return View(CH.GetAllData<Lead>());
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<Lead>(id));
        }

        public ActionResult Create(int? companyid, string from)
        {
            ViewBag.CompanyID = companyid;
            if (from == "company")
            {
                ViewBag.From = "company";
            }
            return View();
        }

        [HttpPost]
        public ActionResult Create(Lead item)
        {
            if (!EntityUtl.CheckPropertyAllNull(item, "Name_EN", "Name_CH"))
                ModelState.AddModelError("", "名字不完整,中文名和英文名不能同时为空");

            if (EntityUtl.CheckPropertyAllNull(item, "Email", "Fax"))
                ModelState.AddModelError("", "联系方式不完整,座机和传真号不能同时为空");

            if (ModelState.IsValid)
            {
                Image image = ImageController.UploadImg(Request, item.Image);
                if(image!=null)
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
            if (EntityUtl.CheckPropertyAllNull(item, "Name_EN", "Name_CH"))
                ModelState.AddModelError("", "名字不完整,中文名和英文名不能同时为空");

            if (EntityUtl.CheckPropertyAllNull(item, "Email", "Fax"))
                ModelState.AddModelError("", "联系方式不完整,座机和传真号不能同时为空");

            if (ModelState.IsValid)
            {
                Image image = ImageController.UploadImg(Request, item.Image);
                if (image != null)
                    item.ImageID = image.ID;
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