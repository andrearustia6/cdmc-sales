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
    public class LoginPageImageController : Controller
    {

        public ViewResult Index()
        {
            var data = CH.GetAllData<Image>(i => i.ImageArea == ImageArea.LoginPage.ToString());
            if (data.Count > 0)
                return View(data.OrderByDescending(o => o.Sequence).ToList());
            else
                return View();
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<Image>(id));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Image item)
        {
            if (ModelState.IsValid)
            {
                item.ImageArea = ImageArea.LoginPage.ToString();
                Image image = ImageController.UploadImg(Request, item); 
               
                return RedirectToAction("Index");
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<Image>(id));
        }

        [HttpPost]
        public ActionResult Edit(Image item)
        {
            if (ModelState.IsValid)
            {
                item.ImageArea = ImageArea.LoginPage.ToString();
                Image image = ImageController.UploadImg(Request, item); 
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<Image>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<Image>(id);
            return RedirectToAction("Index");
        }
    }
}