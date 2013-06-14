using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using Entity;
using Utl;
using System.Data;

namespace Sales.Controllers
{
    public class EmployeeRoleController : Controller
    {

        protected override void Dispose(bool disposing)
        {
            CH.DB.Dispose();
            base.Dispose(disposing);
        }


        public ActionResult index()
        {
            return View();

        }
        [GridAction]
        public ActionResult _SelectIndex()
        {
            var list = from l in CH.DB.EmployeeRoles select l;
            return View(new GridModel(list));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveAjaxEditing(int id)
        {
            var item =  CH.DB.EmployeeRoles.Find(id);
            if (TryUpdateModel(item))
            {
                CH.DB.Entry(item).State = EntityState.Modified;
                CH.DB.SaveChanges();
            }
           
            return View(new GridModel(from l in CH.DB.EmployeeRoles select l));
        }
        [AcceptVerbs(HttpVerbs.Post)]

        [GridAction]
        public ActionResult _InsertAjaxEditing()
        {
            var item = new EmployeeRole();

            if (TryUpdateModel(item))
            {
                CH.DB.Set<EmployeeRole>().Add(item);
                CH.DB.SaveChanges();
            }
            //Rebind the grid       
            return View(new GridModel(from l in CH.DB.EmployeeRoles select l));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _DeleteAjaxEditing(int id)
        {
            EmployeeRole record = CH.DB.Set<EmployeeRole>().Find(id);
            CH.DB.Set<EmployeeRole>().Remove(record);
            CH.DB.SaveChanges();
            return View(new GridModel(CH.GetAllData<AssignPerformanceRate>()));
        }

    }
}
