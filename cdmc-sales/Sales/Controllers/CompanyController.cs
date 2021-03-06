﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Entity;
using Utilities;
using Utl;
using Telerik.Web.Mvc;

namespace Sales.Controllers
{
    [ManagerRequired]
    public class CompanyController : Controller
    {
        protected override void Dispose(bool disposing)
        {
            CH.DB.Dispose();
            base.Dispose(disposing);
        }
        
        public ActionResult Index()
        {
           return View(CH.GetAllData<Company>("Leads"));
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<Company>(id));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Company item)
        {
            if (EntityUtl.Utl.CheckPropertyAllNull(item, "Name_EN", "Name_CH"))
                ModelState.AddModelError("", "名字不完整,中文名和英文名不能同时为空");

            if (ModelState.IsValid)
            {
                Image image = ImageController.UploadImg(Request, item.Image);
                if (image != null)
                    item.ImageID = image.ID;
                
                item.From = Employee.GetCurrentProfile("Department").ToString();
                CH.Create<Company>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Edit(int id)
        {
             var c = CH.GetDataById<Company>(id);

            if(Employee.AsManager())
            return View(CH.GetDataById<Company>(id));
            else if(Employee.IsEqualToCurrentUserName(c.Cerator))
            {
               
                    return View(CH.GetDataById<Company>(id));
             
            }
            else 
              return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(Company item)
        {
            if (EntityUtl.Utl.CheckPropertyAllNull(item, "Name_EN", "Name_CH"))
                ModelState.AddModelError("", "名字不完整,中文名和英文名不能同时为空");
          
            if (ModelState.IsValid)
            {
                Image image = ImageController.UploadImg(Request, item.Image);
                if (image != null)
                    item.ImageID = image.ID;
             
                ImageController.UploadImg(Request, item.Image);
                CH.Edit<Company>(item);
              

                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<Company>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<Company>(id);
            return RedirectToAction("Index");
        }
    }
}