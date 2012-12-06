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
        public ViewResult MyIndex()
        {
            return View(CH.GetAllData<Research>(r => r.Creator == Employee.GetCurrentUserName()).OrderByDescending(o => o.CreatedDate).ToList());
        }


        public ViewResult Index()
        {
            if(Employee.AsDirector())
                return View(CH.GetAllData<Research>().OrderByDescending(o => o.CreatedDate).ToList());
            else if (Employee.EqualToManager())
            {
                var ps = CH.GetAllData<Project>(p => p.Manager == Employee.GetCurrentUserName(), "Members");
                var list = new List<Member>();
                ps.ForEach(p => {
                   list.AddRange(p.Members);
                });
                var rs = CH.GetAllData<Research>(r => list.Any(m=>m.Name==r.Creator));
                return View(rs.OrderByDescending(o => o.CreatedDate).ToList());
            }
              return View();
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
                return RedirectToAction("MyIndex");
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
                return RedirectToAction("MyIndex");
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
            return RedirectToAction("MyIndex");
        }
    }
}