using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using Entity;
using Utl;
using System.Data;
using Model;

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
            return View(new GridModel(getData()));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveAjaxEditing(int id)
        {
            var item = CH.DB.EmployeeRoles.Find(id);
            if (TryUpdateModel(item))
            {
                CH.DB.Entry(item).State = EntityState.Modified;
                CH.DB.SaveChanges();
            }

            return View(new GridModel(getData()));
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
            return View(new GridModel(getData()));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _DeleteAjaxEditing(int id)
        {
            EmployeeRole record = CH.DB.Set<EmployeeRole>().Find(id);
            CH.DB.Set<EmployeeRole>().Remove(record);
            CH.DB.SaveChanges();
            return View(new GridModel(getData()));
        }

        private List<AjaxEmployee> getData()
        {
            var list = from l in CH.DB.EmployeeRoles
                       select new AjaxEmployee
                       {
                           ID = l.ID,
                           AccountName = l.AccountName,
                           DepartmentName = (l.Department != null ? l.Department.Name : string.Empty),
                           RoleName = (l.Role != null ? l.Role.Name : string.Empty),
                           ExpLevelName = (l.ExpLevel != null ? l.ExpLevel.Name : string.Empty),
                           Email = l.Email,
                           StartDate = l.StartDate,
                           DepartmentID = l.DepartmentID,
                           ExpLevelID = l.ExpLevelID,
                           RoleID = l.RoleID
                       };


            var role = Employee.CurrentRole;

            if (role != null)
            {
                if (role.Level < 500)
                {
                    list.Where(s => s.AccountName == Employee.CurrentUserName);
                }
                else
                {
                    //list = Membership.GetAllUsers().Cast<MembershipUser>().ToList<MembershipUser>();
                    if (role.Level == 500)
                    {
                        var mems = CH.GetAllData<Member>(m => m.Project.Manager == Employee.CurrentUserName).Select(s => s.Name).Distinct();
                        list = list.Where(f => mems.Contains(f.AccountName) || f.AccountName == Employee.CurrentUserName);
                    }
                }
            }

            return list.ToList();
        }
    }
}
