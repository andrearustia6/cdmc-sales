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
using System.Data.Objects;

namespace Sales.Controllers
{
    public class CompanyRelationshipController : Controller
    {
        public ViewResult Index()
        {
            return View(CH.GetAllData<CompanyRelationship>());
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<CompanyRelationship>(id));
        }

        public ActionResult Create(int? projectid)
        {
            ViewBag.ProjectID = projectid;
            return View();
        }

        [HttpPost]
        public ActionResult Create(string enname,string chname,CompanyRelationship item, int[] checkedCategorys)
        {

            this.AddErrorStateIfFieldExist<Company>(enname,"Name_EN");
            this.AddErrorStateIfFieldExist<Company>(enname, "Name_CH");

            if (ModelState.IsValid)
            {

                var company = new Company() { Name_EN = enname, Name_CH = chname, Creator = User.Identity.Name, From = Employee.GetCurrentProfile("Department").ToString() };
                    CH.Create<Company>(company);
           
     
                if (ModelState.IsValid)
                {
                    if (checkedCategorys != null)
                    {

                     
                        var ck = CH.GetAllData<Category>(i => checkedCategorys.Contains(i.ID));
                        item.Categorys = ck;
                    }
                    item.CompanyID = company.ID;
                    CH.Create<CompanyRelationship>(item);
                    return RedirectToAction("management", "project", new { id = item.ProjectID, tabindex = 3 });
                }
            }
            ViewBag.ProjectID = item.ProjectID;
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            var data = CH.GetDataById<CompanyRelationship>(id);
            ViewBag.ProjectID = data.ProjectID;
            return View(data);
        }

        [HttpPost]
        public ActionResult Edit(CompanyRelationship item, int[] checkedCategorys)
        {
           
            if (ModelState.IsValid)
            {
                if (checkedCategorys != null)
                {
                   // var old = CH.GetDataById<CompanyRelationship>(item.ID, "Categorys");

                    var ck = CH.GetAllData<Category>(c => checkedCategorys.Any(cc=>cc == c.ID));
                  
                    CH.DB.Entry(item).Collection("Categorys").CurrentValue = ck;
                    CH.DB.SaveChanges();
               
                    CH.Edit<CompanyRelationship>(item);
                 
           
                    
                
                
       
                }
                else
                CH.Edit<CompanyRelationship>(item);
               
                return RedirectToAction("management", "project", new { id = item.ProjectID, tabindex = 3 });
            }
            ViewBag.ProjectID = item.ProjectID;
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<CompanyRelationship>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var item = CH.GetDataById<CompanyRelationship>(id);
            CH.Delete<CompanyRelationship>(id);
            return RedirectToAction("management", "project", new { id = item.ProjectID, tabindex = 3 });
        }
    }
}