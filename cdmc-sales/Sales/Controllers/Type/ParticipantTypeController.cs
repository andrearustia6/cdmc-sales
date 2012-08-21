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
    public class ParticipantTypeController : Controller
    {
        public ViewResult Index()
        {
            return View(CH.GetAllData<ParticipantType>());
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<ParticipantType>(id));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(ParticipantType item)
        {
            if (ModelState.IsValid)
            {
                CH.Create<ParticipantType>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<ParticipantType>(id));
        }

        [HttpPost]
        public ActionResult Edit(ParticipantType item)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<ParticipantType>(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<ParticipantType>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CH.Delete<ParticipantType>(id);
            return RedirectToAction("Index");
        }
    }
}