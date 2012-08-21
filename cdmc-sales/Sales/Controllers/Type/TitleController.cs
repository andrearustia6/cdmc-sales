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

namespace Sales.Controllers.Type
{
    public class TitleController : Controller
    {
        public ViewResult Index()
        {
            return View(CH.GetAllData<Title>());
        }



        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<Title>(id));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Title item)
        {
            if (ModelState.IsValid)
            {
                CH.Create<Title>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<Title>(id));
        }

        [HttpPost]
        public ActionResult Edit(Title item)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<Title>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<Title>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<Title>(id);
            return RedirectToAction("Index");
        }
    }
}