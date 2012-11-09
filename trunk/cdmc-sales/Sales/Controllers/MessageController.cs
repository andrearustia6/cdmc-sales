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
    public class MessageController : Controller
    {
        public ViewResult Index()
        {
            return View(CH.GetAllData<Message>());
        }


        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<Message>(id));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Message item)
        {
            if (ModelState.IsValid)
            {
                item.Member = User.Identity.Name;
                CH.Create<Message>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<Message>(id));
        }

        [HttpPost]
        public ActionResult Edit(Message item)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<Message>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<Message>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<Message>(id);
            return RedirectToAction("Index");
        }
    }
}