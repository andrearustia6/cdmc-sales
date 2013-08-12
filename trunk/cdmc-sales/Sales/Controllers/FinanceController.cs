using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using Utl;
using BLL;
using Entity;
using Sales.Model;
using System.IO;
namespace Sales.Controllers
{
    public class FinanceController : Controller
    {
        public ActionResult Index(int? month, string btnExport)
        {
            ViewBag.Month = month;
            var rolelvl = Employee.CurrentRole.Level;
            if (rolelvl == PoliticsInterfaceRequired.LVL)
            {
                rolelvl = DirectorRequired.LVL;
            }
            if (rolelvl == ManagerRequired.LVL)
            {
                rolelvl = DirectorRequired.LVL;
            }
            ViewBag.RoleLevel = rolelvl;
            return View();
            //if (month == null) month = DateTime.Now.Month;
            //// if (month == null) month = 5;
            //var list = Finance_Logical._PreCommissionBLL.GetPreCommission(month.Value);
            //var data = list.OrderBy(p => p.TargetNameEN).ToList();
            //return View(data);
        }
        [GridAction]
        public ActionResult PrecommissionIndex(int? month)
        {
            if (month == null) month = DateTime.Now.Month;
            // if (month == null) month = 5;
            var list = Finance_Logical._PreCommissionBLL.GetPreCommission(month.Value);
            var data = list.OrderBy(p => p.TargetNameEN).ToList();
            return View(new GridModel(data));
        }
        //public ActionResult Edit(int id)
        //{
        //    var data = CH.GetDataById<PreCommission>(id);
        //    _PreCommission p=new _PreCommission();
        //    p.CommID = data.CommID;
        //    p.StartDate = data.StartDate;
        //    p.EndDate = data.EndDate;

        //    p.ID = data.ID;
        //    p.ProjectNames = data.ProjectNames;
        //    p.Income = data.Income;
        //    p.TargetNameEN = data.TargetNameEN;
        //    p.TargetNameCN = data.TargetNameCN;
        //    p.InOut = data.InOut;
        //    p.DelegateLessCount = data.DelegateLessCount;
        //    p.DelegateLessIncome = data.DelegateLessIncome;
        //    p.DelegateMoreCount = data.DelegateMoreCount;
        //    p.DelegateMoreIncome = data.DelegateMoreIncome;
        //    p.SponsorIncome = data.SponsorIncome;
        //    p.Commission = data.Commission;
        //    p.CommissionRate = data.CommissionRate;
        //    p.Tax = data.Tax;
        //    p.Bonus = data.Bonus;
        //    p.ReturnIncome = data.ReturnIncome;
        //    p.ReturnReason = data.ReturnReason;
        //    p.ActualCommission = data.ActualCommission;
        //    return View(p);
        //}
        public ActionResult GetIncome(int month, string sale,int projectid)
        {
            // if (month == null) month = 5;
            var list = Finance_Logical._PreCommissionBLL.GetIncome(month, sale, projectid);
            //var data = list.Where(p => p.TargetNameEN==sale).ToList();
            return Json(list);
        }
        public ActionResult GetProjects(int month, string sale)
        {
            // if (month == null) month = 5;
            var list = Finance_Logical._PreCommissionBLL.GetProjects(month, sale);
            //var data = list.Where(p => p.TargetNameEN==sale).ToList();
            return Json(list);
        }

        public ActionResult GetSales(int month)
        {
            // if (month == null) month = 5;
            var list = Finance_Logical._PreCommissionBLL.GetSalesDDL(month);
            list = list.OrderBy(o=>o.sales);
            
            return Json(list);
        }

        [HttpPost]
        public ActionResult _InsertPreCommission(PreCommission item)
        {
            if (ModelState.IsValid)
            {
                CH.Edit<PreCommission>(item);
                return RedirectToAction("index");
            }
            return View(item);
        }

        
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult Insert(_PreCommission model)
        {
            PreCommission newmodel = new PreCommission();
            newmodel.StartDate = model.StartDate;
            newmodel.EndDate = model.EndDate;
            newmodel.TargetNameEN = model.TargetNameEN;
            newmodel.TargetNameCN = model.TargetNameCN;
            newmodel.ProjectID = model.ProjectID;
            newmodel.InOut = model.InOut;
            newmodel.DelegateLessIncome = model.DelegateLessIncome;
            newmodel.DelegateMoreCount = model.DelegateMoreCount;
            newmodel.DelegateMoreIncome = model.DelegateMoreIncome;
            newmodel.SponsorIncome = model.SponsorIncome;
            newmodel.Income = model.Income;
            newmodel.CommissionRate = model.CommissionRate;
            newmodel.Commission = model.Commission;
            newmodel.Tax = model.Tax;
            newmodel.Bonus = model.Bonus;
            newmodel.ReturnIncome = model.ReturnIncome;
            newmodel.ReturnReason = model.ReturnReason;
            newmodel.ActualCommission = model.ActualCommission;

            newmodel.DelegateLessRate = model.DelegateLessRate;
            newmodel.DelegateLessCommission = model.DelegateLessCommission;
            newmodel.DelegateMoreRate = model.DelegateMoreRate;
            newmodel.DelegateMoreCommission = model.DelegateMoreCommission;
            newmodel.SponsorRate = model.SponsorRate;
            newmodel.SponsorCommission = model.SponsorCommission;
            newmodel.TotalCommission = model.TotalCommission;

            newmodel.CommID = model.TargetNameEN + model.StartDate.Year.ToString() + model.StartDate.Month.ToString().PadLeft(2, '0');
            CH.Create<PreCommission>(newmodel);
            var list = Finance_Logical._PreCommissionBLL.GetPreCommission(model.StartDate.Month);
            var data = list.OrderBy(p => p.TargetNameEN).ToList();
            return View(new GridModel(data));

        }
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult Save(_PreCommission model)
        {
            PreCommission newmodel = new PreCommission();
            newmodel = CH.GetDataById<PreCommission>(model.ID);
            newmodel.StartDate = model.StartDate;
            newmodel.EndDate = model.EndDate;
            newmodel.TargetNameEN = model.TargetNameEN;
            newmodel.TargetNameCN = model.TargetNameCN;
            newmodel.ProjectID = model.ProjectID;
            newmodel.InOut = model.InOut;
            newmodel.DelegateLessIncome = model.DelegateLessIncome;
            newmodel.DelegateMoreCount = model.DelegateMoreCount;
            newmodel.DelegateMoreIncome = model.DelegateMoreIncome;
            newmodel.SponsorIncome = model.SponsorIncome;
            newmodel.Income = model.Income;
            newmodel.CommissionRate = model.CommissionRate;
            newmodel.Commission = model.Commission;
            newmodel.Tax = model.Tax;
            newmodel.Bonus = model.Bonus;
            newmodel.ReturnIncome = model.ReturnIncome;
            newmodel.ReturnReason = model.ReturnReason;
            newmodel.ActualCommission = model.ActualCommission;

            newmodel.DelegateLessRate = model.DelegateLessRate;
            newmodel.DelegateLessCommission = model.DelegateLessCommission;
            newmodel.DelegateMoreRate = model.DelegateMoreRate;
            newmodel.DelegateMoreCommission = model.DelegateMoreCommission;
            newmodel.SponsorRate = model.SponsorRate;
            newmodel.SponsorCommission = model.SponsorCommission;
            newmodel.TotalCommission = model.TotalCommission;

            CH.Edit<PreCommission>(newmodel);
            var list = Finance_Logical._PreCommissionBLL.GetPreCommission(model.StartDate.Month);
            var data = list.OrderBy(p => p.TargetNameEN).ToList();
            return View(new GridModel(data));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult Delete(_PreCommission model)
        {
            int month = model.StartDate.Month;
            CH.Delete<PreCommission>(model.ID);
            var list = Finance_Logical._PreCommissionBLL.GetPreCommission(month);
            var data = list.OrderBy(p => p.TargetNameEN).ToList();
            return View(new GridModel(data));

        }
        public JsonResult CheckUnique(int projectid,DateTime StartDate,int ID,string TargetNameEN)
        {
            bool isValidate = false;
            int count = CH.DB.PreCommissions.Count(p => p.TargetNameEN == TargetNameEN && p.StartDate == StartDate && p.ID!=ID && p.ProjectID==projectid);
            if (count == 0)
                isValidate = true;
            return Json(isValidate, JsonRequestBehavior.AllowGet);

        }
        public JsonResult CheckUnique1(DateTime StartDate, string TargetNameEN, int ID)
        {
            bool isValidate = false;
            int count = CH.DB.PreCommissions.Count(p => p.TargetNameEN == TargetNameEN && p.StartDate == StartDate && p.ID != ID);
            if (count == 0)
                isValidate = true;
            return Json(isValidate, JsonRequestBehavior.AllowGet);

        }
    }
}
