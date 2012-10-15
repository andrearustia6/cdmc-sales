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
        public ActionResult Create(string enname,CompanyRelationship item, int[] checkedCategorys)
        {
            Company company = CH.GetAllData<Company>(co => co.Name_EN == enname).FirstOrDefault();
            if (company == null)
            {
                company = new Company() { Name_EN = enname, Name_CH = "" };
                CH.Create<Company>(company);
            }

            if (ModelState.IsValid)
            {
                if (checkedCategorys != null)
                {

                    item.Categorys = new List<Category>();
                    var ck = CH.GetAllData<Category>(i => checkedCategorys.Contains(i.ID));
                    item.Categorys.AddRange(ck);
                }

                CH.Create<CompanyRelationship>(item);
                return RedirectToAction("Index");
            }
            ViewBag.ProjectID = item.ProjectID;
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<CompanyRelationship>(id));
        }

        [HttpPost]
        public ActionResult Edit(CompanyRelationship item, int[] checkedCategorys)
        {
            if (ModelState.IsValid)
            {
                if (checkedCategorys != null)
                {
                    item.Categorys = new List<Category>();
                    var ck = CH.GetAllData<Category>(i => checkedCategorys.Contains(i.ID));
                    item.Categorys.AddRange(ck);
                }

                CH.Edit<CompanyRelationship>(item);
                return RedirectToAction("Index");
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
            CH.Delete<CompanyRelationship>(id);
            return RedirectToAction("Index");
        }
    }
}