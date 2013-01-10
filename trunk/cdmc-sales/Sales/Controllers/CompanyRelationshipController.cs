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
    [ProjectInformationAccess]
    public class CompanyRelationshipController : Controller
    {
        protected override void Dispose(bool disposing)
        {
            CH.DB.Dispose();
            base.Dispose(disposing);
        }

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
            this.AddErrorIfAllNamesEmpty(enname, chname);
            this.AddAddErrorStateIfOneOfNameExist<Company>(enname, chname);

            if (ModelState.IsValid)
            {
                var company = new Company() { Name_EN = enname, Name_CH = chname, Creator = Employee.CurrentUserName, From = Employee.GetCurrentProfile("Department").ToString() };
                CH.Create<Company>(company);
     
                if (ModelState.IsValid)
                {
                    string categorystring = string.Empty;
                   
                    if (checkedCategorys != null)
                    {
                        var ck = CH.GetAllData<Category>(i => checkedCategorys.Contains(i.ID));
                        item.Categorys = ck;

                        ck.ForEach(l =>
                        {
                            if (string.IsNullOrEmpty(categorystring))
                                categorystring = l.Name;
                            else
                                categorystring += "|" + l.Name;
                        });
                        item.CategoryString = categorystring;
                    }

                    item.CompanyID = company.ID;
                    CH.Create<CompanyRelationship>(item);
                    if(Employee.EqualToProductInterface())
                       return RedirectToAction("companyrelationshipindex", "productinterface", new { id = item.ProjectID });
                    else
                       return RedirectToAction("management", "project", new { id = item.ProjectID, tabindex = 3 });
                }
            }
            ViewBag.ProjectID = item.ProjectID;
            return View();
        }

        public ActionResult Edit(int id)
        {
            var data = CH.GetDataById<CompanyRelationship>(id);
            ViewBag.ProjectID = data.ProjectID;
            return View(data);
        }

        [HttpPost]
        public ActionResult Edit(CompanyRelationship item, int[] checkedCategorys, string enname, string chname)
        {
            //如何保存多对多关系中 引用变化的例子
            if (ModelState.IsValid)
            {
                var com = CH.GetDataById<Company>(item.CompanyID);
                if (com.Name_CH != chname || com.Name_EN != enname)
                {
                    com.Name_CH = chname;
                    com.Name_EN = enname;
                    CH.Edit<Company>(com);
                }
        
                  CH.Edit<CompanyRelationship>(item);
             
                if (checkedCategorys != null)
                {
                    item = CH.GetDataById<CompanyRelationship>(item.ID);
                    item.Categorys.Clear();
                    var ck = CH.GetAllData<Category>(c => checkedCategorys.Any(cc=>cc == c.ID));
                    ck.ForEach(c => {
                        item.Categorys.Add(c);
                    });

                    string categorystring = string.Empty;
                    ck.ForEach(l =>
                    {
                        if (string.IsNullOrEmpty(categorystring))
                            categorystring = l.Name;
                        else
                            categorystring += "|" + l.Name;
                    });
                    item.CategoryString = categorystring;
                    CH.Edit<CompanyRelationship>(item);
                    CH.DB.SaveChanges();
                }
                else
                {
                    item = CH.GetDataById<CompanyRelationship>(item.ID);
                    item.Categorys.Clear();
                    CH.DB.SaveChanges();
                }

                if (Employee.EqualToProductInterface())
                    return RedirectToAction("companyrelationshipindex", "productinterface", new { id = item.ProjectID });
                else
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
            var pid = item.ProjectID;
            item.Deals.ForEach(t =>
            {
                CH.Delete<Deal>(t.ID);
            });
            item.LeadCalls.ForEach(t =>
            {
                CH.Delete<LeadCall>(t.ID);
            });
        
            CH.Delete<CompanyRelationship>(id);
            return RedirectToAction("management", "project", new { id = pid, tabindex = 3 });
        }
    }
}