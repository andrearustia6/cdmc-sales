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
        public ActionResult CommissionIndex(int? month, string btnExport)
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
        [GridAction]
        public ActionResult _CommissionIndex(int? projectid, string sale="")
        {
            var list = Finance_Logical._PreCommissionBLL.GetFinalCommission(projectid,sale);
            var data = list.OrderBy(p => p.TargetNameEN).ToList();
            return View(new GridModel(data));
        }
        
        public ActionResult GetIncome(int month, string sale,int projectid)
        {
            // if (month == null) month = 5;
            var list = Finance_Logical._PreCommissionBLL.GetIncome(month, sale, projectid);
            //var data = list.Where(p => p.TargetNameEN==sale).ToList();
            return Json(list);
        }
        /// <summary>
        /// 用于显示预提成的提成额
        /// </summary>
        /// <param name="projectid"></param>
        /// <param name="sale"></param>
        /// <returns></returns>
        public ActionResult GetPreCommByProSales(int projectid, string sale)
        {

            var list = Finance_Logical._PreCommissionBLL.GetPreCommByProSales(projectid,sale);
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
        public ActionResult GetSalesByProject(int projectid)
        {
            // if (month == null) month = 5;
            var list = Finance_Logical._PreCommissionBLL.GetSalesByProDDL(projectid);
            return Json(list);
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

            newmodel.DelegateIncome = model.DelegateIncome;
            newmodel.DelegateRate = model.DelegateRate;
            newmodel.DelegateCommission = model.DelegateCommission;

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

            newmodel.DelegateIncome = model.DelegateIncome;
            newmodel.DelegateRate = model.DelegateRate;
            newmodel.DelegateCommission = model.DelegateCommission;

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

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult InsertCommission(_FinalCommission model)
        {
            FinalCommission newmodel = new FinalCommission();
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
            newmodel.Commission = model.CommissionA;
            newmodel.Tax = model.Tax;
            newmodel.Bonus = model.Bonus;
            newmodel.ActualCommission = model.ActualCommission;

            newmodel.DelegateLessRate = model.DelegateLessRate;
            newmodel.DelegateLessCommission = model.DelegateLessCommission;
            newmodel.DelegateMoreRate = model.DelegateMoreRate;
            newmodel.DelegateMoreCommission = model.DelegateMoreCommission;
            newmodel.SponsorRate = model.SponsorRate;
            newmodel.SponsorCommission = model.SponsorCommission;
            newmodel.TotalCommission = model.TotalCommission;

            newmodel.DelegateLessPayed = model.DelegateLessPayed;
            newmodel.DelegateMorePayed = model.DelegateMorePayed;
            newmodel.SponsorPayed = model.SponsorPayed;
            newmodel.CommissionPayed = model.CommissionPayed;

            newmodel.DelegateIncome = model.DelegateIncome;
            newmodel.DelegateRate = model.DelegateRate;
            newmodel.DelegateCommission = model.DelegateCommission;
            newmodel.DelegatePayed = model.DelegatePayed;

            var procode = CH.GetDataById<Project>(model.ProjectID).ProjectCode;
            newmodel.CommID = procode + "-" + model.TargetNameEN + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0');
            CH.Create<FinalCommission>(newmodel);
            var list = Finance_Logical._PreCommissionBLL.GetFinalCommission(model.ProjectID,model.TargetNameEN);
            var data = list.OrderBy(p => p.TargetNameEN).ToList();
            return View(new GridModel(data));

        }
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult SaveCommission(_FinalCommission model)
        {
            FinalCommission newmodel = new FinalCommission();
            newmodel = CH.GetDataById<FinalCommission>(model.ID);
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
            newmodel.Commission = model.CommissionA;
            newmodel.Tax = model.Tax;
            newmodel.Bonus = model.Bonus;
            newmodel.ActualCommission = model.ActualCommission;

            newmodel.DelegateLessRate = model.DelegateLessRate;
            newmodel.DelegateLessCommission = model.DelegateLessCommission;
            newmodel.DelegateMoreRate = model.DelegateMoreRate;
            newmodel.DelegateMoreCommission = model.DelegateMoreCommission;
            newmodel.SponsorRate = model.SponsorRate;
            newmodel.SponsorCommission = model.SponsorCommission;
            newmodel.TotalCommission = model.TotalCommission;

            newmodel.DelegateLessPayed = model.DelegateLessPayed;
            newmodel.DelegateMorePayed = model.DelegateMorePayed;
            newmodel.SponsorPayed = model.SponsorPayed;
            newmodel.CommissionPayed = model.CommissionPayed;

            newmodel.DelegateIncome = model.DelegateIncome;
            newmodel.DelegateRate = model.DelegateRate;
            newmodel.DelegateCommission = model.DelegateCommission;
            newmodel.DelegatePayed = model.DelegatePayed;

            CH.Edit<FinalCommission>(newmodel);
            var list = Finance_Logical._PreCommissionBLL.GetFinalCommission(model.ProjectID, model.TargetNameEN);
            var data = list.OrderBy(p => p.TargetNameEN).ToList();
            return View(new GridModel(data));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult DeleteCommission(_FinalCommission model)
        {
            int? projectid = model.ProjectID;
            string sale = model.TargetNameEN;
            CH.Delete<FinalCommission>(model.ID);
            var list = Finance_Logical._PreCommissionBLL.GetFinalCommission(projectid, sale);
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

        public JsonResult CheckUniqueForFinalComm(int ProjectID, string TargetNameEN, int ID)
        {
            bool isValidate = false;
            int count = CH.DB.FinalCommissions.Count(p => p.TargetNameEN == TargetNameEN && p.ProjectID == ProjectID && p.ID != ID);
            if (count == 0)
                isValidate = true;
            return Json(isValidate, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 用于结算表的预结算grid
        /// </summary>
        /// <param name="sales"></param>
        /// <param name="projectid"></param>
        /// <returns></returns>
        public JsonResult GetPrecommissions(int projectid,string sales)
        {
            var list = CH.DB.PreCommissions.Where(p=>p.TargetNameEN==sales & p.ProjectID==projectid);
            return Json(list);
        }
        /// <summary>
        /// 用于结算表的预结算grid
        /// </summary>
        /// <param name="sales"></param>
        /// <param name="projectid"></param>
        /// <returns></returns>
        public JsonResult GetDeals(int projectid,string sales)
        {
            var list = CH.DB.Deals.Where(p =>p.ProjectID == projectid && p.Sales==sales && p.Income>0);
            var deals = from d in list
                        select new _CommissionDeals
                        {
                            DealCode =d.DealCode,
                            Income =d.Income,
                            SignDate =d.SignDate,
                            ExpectedPaymentDate =d.ExpectedPaymentDate,
                            ActualPaymentDate =d.ActualPaymentDate
                        };
            return Json(deals);
        }
    }
}
