using System;
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
    public class CompanyController : Controller
    {
        [ManagerRequired]
        public ActionResult Index()
        {
           return View(CH.GetAllData<Company>("Leads"));
        }

        [LeaderRequired]
        public ViewResult SalesIndex()
        {
          
            var members = CH.GetAllData<Member>(m => m.Name == User.Identity.Name);

            var member = members.FirstOrDefault();
            if (member != null&&member.CharactersSet!=null)
            {
                var data = CH.GetAllData<Company>("Leads");
                 data = data.FindAll(c => member.CharactersSet.Any(ca=>(c.Name_EN.ToUpper().StartsWith(ca))));
                 return View(data);
            }
            return View();
        }

        public ViewResult MarketIndex()
        {
            return View(CH.GetAllData<Lead>());
        }

        public ViewResult CompanyMaintainIndex()
        {
            return View(CH.GetAllData<Company>(c => c.Cerator == User.Identity.Name,"Leads"));
        }

        public ViewResult ProductIndex()
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
        [SalesRequired]
        public ActionResult Create(Company item)
        {

            if (EntityUtl.CheckPropertyAllNull(item, "Name_EN", "Name_CH"))
                ModelState.AddModelError("", "名字不完整,中文名和英文名不能同时为空");

            if (ModelState.IsValid)
            {
                Image image = ImageController.UploadImg(Request, item.Image);
                if (image != null)
                    item.ImageID = image.ID;
                item.Cerator = User.Identity.Name;
                item.From = Employee.GetCurrentProfile("Department").ToString();
                CH.Create<Company>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        [SalesRequired]
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
        [SalesRequired]
        public ActionResult Edit(Company item)
        {
            if (EntityUtl.CheckPropertyAllNull(item, "Name_EN", "Name_CH"))
                ModelState.AddModelError("", "名字不完整,中文名和英文名不能同时为空");

            if (ModelState.IsValid)
            {
                Image image = ImageController.UploadImg(Request, item.Image);
                if (image != null)
                    item.ImageID = image.ID;

                if (Employee.AsManager()||Employee.IsEqualToCurrentUserName(item.Cerator))
                {
                    ImageController.UploadImg(Request, item.Image);
                    CH.Edit<Company>(item);
                }

                return RedirectToAction("Index");
            }
            return View(item);
        }
        [SalesRequired]
        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<Company>(id));
        }
        [SalesRequired]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<Company>(id);
            return RedirectToAction("Index");
        }
    }
}