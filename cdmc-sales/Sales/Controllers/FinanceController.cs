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

            if (btnExport == "1")
            {
                MemoryStream output = new MemoryStream();
                StreamWriter writer = new StreamWriter(output, System.Text.Encoding.Default);

                if (month == null) month = DateTime.Now.Month;

                writer.Write(month + "月");
                writer.WriteLine();

                var PreCommissionList = Finance_Logical._PreCommissionBLL.GetPreCommission(month.Value).ToList();

                if (PreCommissionList.Count() > 0)
                {
                    writer.Write("销售提成预发表,");
                    writer.WriteLine();

                    writer.Write("提成单号,");
                    writer.Write("国内外,");
                    writer.Write("英文名,");
                    writer.Write("中文名,");
                    writer.Write("开始时间,");
                    writer.Write("结束时间,");
                    writer.Write("项目,");
                    writer.Write("入账总额,");
                    writer.Write("提成总额,");
                    writer.Write("扣税,");
                    writer.Write("扣奖金,");
                    writer.Write("预发总额,");
                    writer.Write("冲销金额,");
                    writer.Write("冲销后总额");
                    writer.WriteLine();

                    foreach (var item in PreCommissionList)
                    {
                        writer.Write(item.CommID);
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(item.InOut);
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(item.TargetNameEN);
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(item.TargetNameCN);
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:yyyy-MM-dd}", item.StartDate));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:yyyy-MM-dd}", item.EndDate));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(item.ProjectName);
                        writer.Write("\"");
                        writer.Write(",");

                        writer.Write("\"");
                        writer.Write(string.Format("{0:c0}", Convert.ToDouble(item.Income==null?0:item.Income)));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:c0}", Convert.ToDouble(item.Commission==null?0:item.Commission)));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:c0}", Convert.ToDouble(item.Tax==null?0:item.Tax)));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:c0}", Convert.ToDouble(item.Bonus==null?0:item.Bonus)));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:c0}", Convert.ToDouble(item.TotalCommission==null?0:item.TotalCommission)));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:c0}", Convert.ToDouble(item.ReturnIncome==null?0:item.ReturnIncome)));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:c0}", Convert.ToDouble(item.ActualCommission==null?0:item.ActualCommission)));
                        writer.Write("\"");
                        writer.WriteLine();

                    }

                    writer.WriteLine();
                    writer.WriteLine();
                }
                writer.Flush();
                output.Position = 0;
                return File(output, "text/comma-separated-values", "PreCommission.csv");

            }

            return View();
            //if (month == null) month = DateTime.Now.Month;
            //// if (month == null) month = 5;
            //var list = Finance_Logical._PreCommissionBLL.GetPreCommission(month.Value);
            //var data = list.OrderBy(p => p.TargetNameEN).ToList();
            //return View(data);
        }
        public ActionResult CommissionIndex(int? projectid,string sale, string btnExport)
        {
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

            if (btnExport == "1")
            {
                MemoryStream output = new MemoryStream();
                StreamWriter writer = new StreamWriter(output, System.Text.Encoding.Default);



                var PreCommissionList = Finance_Logical._PreCommissionBLL.GetFinalCommission(projectid, sale).ToList();
                PreCommissionList = PreCommissionList.OrderBy(p => p.TargetNameEN).ToList();

                if (PreCommissionList.Count() > 0)
                {
                    writer.Write("销售提成结算表,");
                    writer.WriteLine();

                    writer.Write("提成单号,");
                    writer.Write("国内外,");
                    writer.Write("英文名,");
                    writer.Write("中文名,");
                    writer.Write("项目,");
                    writer.Write("入账总额,");
                    writer.Write("提成总额,");
                    writer.Write("扣税,");
                    writer.Write("扣奖金,");
                    writer.Write("结算金额,");
                    writer.Write("实际结算额");
                    writer.WriteLine();

                    foreach (var item in PreCommissionList)
                    {
                        writer.Write(item.CommID);
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(item.InOut);
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(item.TargetNameEN);
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(item.TargetNameCN);
                        writer.Write("\"");
                        writer.Write(",");
                        
                        writer.Write("\"");
                        writer.Write(item.ProjectName);
                        writer.Write("\"");
                        writer.Write(",");

                        writer.Write("\"");
                        writer.Write(string.Format("{0:c0}", Convert.ToDouble(item.Income == null ? 0 : item.Income)));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:c0}", Convert.ToDouble(item.Commission == null ? 0 : item.Commission)));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:c0}", Convert.ToDouble(item.Tax == null ? 0 : item.Tax)));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:c0}", Convert.ToDouble(item.Bonus == null ? 0 : item.Bonus)));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:c0}", Convert.ToDouble(item.TotalCommission == null ? 0 : item.TotalCommission)));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:c0}", Convert.ToDouble(item.ActualCommission == null ? 0 : item.ActualCommission)));
                        writer.Write("\"");
                        writer.WriteLine();

                    }

                    writer.WriteLine();
                    writer.WriteLine();
                }
                writer.Flush();
                output.Position = 0;
                return File(output, "text/comma-separated-values", "Commission.csv");

            }
           
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
            list.InOut = "海外";
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
