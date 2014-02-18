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
    public class PerformanceController : Controller
    {
        public ActionResult Index(int? year,int? month, string btnExport)
        {
            ViewBag.Month = month;
            ViewBag.Year = year;
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

            if (btnExport == "export")
            {
                MemoryStream output = new MemoryStream();
                StreamWriter writer = new StreamWriter(output, System.Text.Encoding.Default);

                if (year == null) year = DateTime.Now.Year;
                if (month == null) month = DateTime.Now.Month;


                writer.Write(year+"年 "+month + "月");
                writer.WriteLine();

                var EmployeeList = CRM_Logical._EmployeePerformance.GetManagerLeadsPerformances(year.Value,month.Value).ToList();

                if (EmployeeList.Count() > 0)
                {
                    writer.Write("板块负责人考核,");
                    writer.WriteLine();

                    writer.Write("员工,");
                    writer.Write("责任心,");
                    writer.Write("纪律性,");
                    writer.Write("执行能力,");
                    writer.Write("目标意识,");
                    writer.Write("检查工作状态,");
                    writer.Write("每周项目协调,");
                    writer.Write("每周PitchPaper,");
                    writer.Write("每周例会,");
                    writer.Write("每月通话时间,");
                    writer.Write("团队CallList,");
                    writer.Write("团队新增Leads,");
                    writer.Write("团队业绩表现,");
                    writer.Write("考核系数,");
                    writer.Write("考核总分,");
                    writer.Write("是否确认");
                    writer.WriteLine();

                    foreach (var item in EmployeeList)
                    {
                        writer.Write(item.TargetName);
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:p0}", Convert.ToDouble(item.ResponsibilityDisp)));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:p0}", Convert.ToDouble(item.DisciplineDisp)));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:p0}", Convert.ToDouble(item.ExcutionDisp)));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:p0}", Convert.ToDouble(item.TargetingDisp)));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:p0}", Convert.ToDouble(item.SearchingDisp)));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:p0}", Convert.ToDouble(item.ProductionDisp)));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:p0}", Convert.ToDouble(item.PitchPaperDisp)));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:p0}", Convert.ToDouble(item.WeeklyMeetingDisp)));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:p0}", Convert.ToDouble(item.MonthlyMeetingDisp)));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:p0}", Convert.ToDouble(item.Calllist)));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:p0}", Convert.ToDouble(item.AddLeads)));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:p0}", Convert.ToDouble(item.CheckIn)));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(item.Rate);
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:p0}", item.Score / 100));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write(item.Confirmed);
                        writer.WriteLine();

                    }

                    writer.WriteLine();
                    writer.WriteLine();
                }

                var TlList = CRM_Logical._EmployeePerformance.GetTeamLeadsPerformances(year.Value,month.Value).ToList(); ;
                if (TlList.Count() > 0)
                {
                    writer.Write("销售经理考核,");
                    writer.WriteLine();

                    writer.Write("员工,");
                    writer.Write("入账目标完成百分比,");
                    writer.Write("入账分数,");
                    writer.Write("调研不达标周数,");
                    writer.Write("调研分数,");
                    writer.Write("通话|FaxOut不达标数,");
                    writer.Write("FaxOut分数,");
                    writer.Write("主管评分,");
                    writer.Write("考核系数,");
                    writer.Write("考核总分数,");
                    writer.Write("FaxOut详细,");
                    writer.Write("调研详细");

                    writer.WriteLine();


                    foreach (var item in TlList)
                    {
                        writer.Write(item.Name);
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:#,##0%}", item.CompletePercent));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:p0}", Convert.ToDouble(item.CheckinScore) / 100));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(item.LeadNotQualifiedWeeksCount);
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:p0}", Convert.ToDouble(item.AddLeadScore) / 100));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(item.HoursOrFaxNotQualifiedWeeksCount);
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:p0}", Convert.ToDouble(item.FaxCallScore) / 100));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:p0}", Convert.ToDouble(item.AssignedScore) / 100));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(item.Rate);
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:p0}", Convert.ToDouble(item.Score) / 100));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(item.FaxOutCountString.Replace(",", ";"));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write(item.LeadAddCountString.Replace(",", ";"));

                        writer.WriteLine();

                    }
                    writer.WriteLine();
                    writer.WriteLine();
                }

                var salesList = CRM_Logical._EmployeePerformance.GetSalesPerformances(year.Value,month.Value, string.Empty).ToList();

                if (salesList.Count() > 0)
                {
                    writer.Write("销售考核,");
                    writer.WriteLine();

                    writer.Write("员工,");
                    writer.Write("入账目标完成百分比,");
                    writer.Write("入账分数,");
                    writer.Write("调研不达标周数,");
                    writer.Write("调研分数,");
                    writer.Write("通话|FaxOut不达标数,");
                    writer.Write("FaxOut分数,");
                    writer.Write("主管评分,");
                    writer.Write("考核系数,");
                    writer.Write("考核总分数,");
                    writer.Write("FaxOut详细,");
                    writer.Write("调研详细,");
                    writer.Write("是否实习");

                    writer.WriteLine();


                    foreach (var item in salesList)
                    {
                        writer.Write(item.Name);
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:#,##0%}", item.CompletePercent));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:p0}", Convert.ToDouble(item.CheckinScore) / 100));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(item.LeadNotQualifiedWeeksCount);
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:p0}", Convert.ToDouble(item.AddLeadScore) / 100));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(item.HoursOrFaxNotQualifiedWeeksCount);
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:p0}", Convert.ToDouble(item.FaxCallScore) / 100));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:p0}", Convert.ToDouble(item.AssignedScore) / 100));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(item.Rate);
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:p0}", Convert.ToDouble(item.Score) / 100));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(item.FaxOutCountString.Replace(",", ";"));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(item.LeadAddCountString.Replace(",", ";"));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write(CH.DB.EmployeeRoles.Where(w=>w.AccountName==item.Name).FirstOrDefault().IsTrainee==true?"是":"否");
                        writer.WriteLine();

                    }
                    writer.WriteLine();
                    writer.WriteLine();
                }


                writer.Flush();
                output.Position = 0;
                return File(output, "text/comma-separated-values", "Performances.csv");

            }

            return View();
        }
        [GridAction]
        public ActionResult _SelectManagerIndex(int? year,int? month)
        {
            if (year == null) year = DateTime.Now.Year;
            if (month == null) month = DateTime.Now.Month;
            // if (month == null) month = 5;
            var list = CRM_Logical._EmployeePerformance.GetManagerLeadsPerformances(year.Value,month.Value);
            var data = list.OrderBy(p => p.TargetName).ToList();
            return View(new GridModel(data));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _UpdateManagerScore(int id, int? year,int? month)
        {
            if (year == null) year = DateTime.Now.Year;
            if (month == null) month = DateTime.Now.Month;
            _ManagerScore model = new _ManagerScore();
            ManagerScore newmodel = new ManagerScore();
            if (id > 0)
            {
                if (TryUpdateModel(model))
                {
                    newmodel = CH.GetDataById<ManagerScore>(id);
                    newmodel.ID = id;
                    newmodel.TargetName = model.TargetName;
                    newmodel.Assigner = model.Assigner;
                    newmodel.Responsibility = (int)model.Responsibility;
                    newmodel.Discipline = model.Discipline;
                    newmodel.Excution = model.Excution;
                    newmodel.Targeting = model.Targeting;
                    newmodel.Searching = model.Searching;
                    newmodel.Production = model.Production;
                    newmodel.PitchPaper = model.PitchPaper;
                    newmodel.WeeklyMeeting = model.WeeklyMeeting;
                    newmodel.MonthlyMeeting = model.MonthlyMeeting;
                    newmodel.Rate = model.Rate;
                    newmodel.Month = month;
                    newmodel.Year = year;

                    CH.Edit<ManagerScore>(newmodel);
                }
                //var model = CH.GetDataById<ManagerScore>(id);
                //model.Item1Score = Item1ScoreString;
                //CH.Edit<ManagerScore>(model);
            }
            else
            {

                if (TryUpdateModel(model))
                {
                    newmodel.TargetName = model.TargetName;
                    newmodel.Assigner = model.Assigner;
                    newmodel.Responsibility = (int)model.Responsibility;
                    newmodel.Discipline = model.Discipline;
                    newmodel.Excution = model.Excution;
                    newmodel.Targeting = model.Targeting;
                    newmodel.Searching = model.Searching;
                    newmodel.Production = model.Production;
                    newmodel.PitchPaper = model.PitchPaper;
                    newmodel.WeeklyMeeting = model.WeeklyMeeting;
                    newmodel.MonthlyMeeting = model.MonthlyMeeting;
                    newmodel.Rate = model.Rate;
                    newmodel.Confirmed = false;
                    newmodel.Month = month;
                    newmodel.Year = year;
                    CH.Create<ManagerScore>(newmodel);
                }
            }

            var list = CRM_Logical._EmployeePerformance.GetManagerLeadsPerformances(year.Value,month.Value);
            var data = list.OrderBy(p => p.TargetName).ToList();
            return View(new GridModel(data));

        }
        //[AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _ConfirmManagerScore(int id,int? year, int? month)
        {

            if (year == null) year = DateTime.Now.Year;
            if (month == null) month = DateTime.Now.Month;
            if (id > 0)
            {
                ManagerScore model = CH.GetDataById<ManagerScore>(id);
                model.Confirmed = true;
                CH.Edit<ManagerScore>(model);
                //return View(model);
            }
            var list = CRM_Logical._EmployeePerformance.GetManagerLeadsPerformances(year.Value,month.Value);
            var data = list.OrderBy(p => p.TargetName).ToList();

            return View(new GridModel(data));

        }
        [GridAction]
        public ActionResult _SelectLeadIndex(int? year,int? month)
        {
            if (year == null) year = DateTime.Now.Year;
            if (month == null) month = DateTime.Now.Month;
            // if (month == null) month = 5;
            var list = CRM_Logical._EmployeePerformance.GetTeamLeadsPerformances(year.Value,month.Value);
            list = list.OrderBy(w => w.Name).ToList();
            //var data = list.OrderBy(p => p.Name).ToList();
            return View(new GridModel(list));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _UpdateTeamLeadPerformance(int id,int? year, int? month)
        {
            if (year == null) year = DateTime.Now.Year;
            if (month == null) month = DateTime.Now.Month;
            _TeamLeadPerformance model = new _TeamLeadPerformance();
            AssignPerformanceScore newmodel = new AssignPerformanceScore();
            if (id > 0)
            {
                if (TryUpdateModel(model))
                {
                    newmodel = CH.GetDataById<AssignPerformanceScore>(id);

                    newmodel.TargetName = model.Name;
                    if (model.RoleLevel == 1000)
                        newmodel.RateAssigner = model.User;
                    else
                        newmodel.Assigner = model.User;
                    newmodel.Rate = model.Rate;
                    newmodel.Score = (int)model.AssignedScore;

                    newmodel.Month = month;
                    newmodel.Year = year;

                    CH.Edit<AssignPerformanceScore>(newmodel);
                }
                //var model = CH.GetDataById<ManagerScore>(id);
                //model.Item1Score = Item1ScoreString;
                //CH.Edit<ManagerScore>(model);
            }
            else
            {

                if (TryUpdateModel(model))
                {

                    newmodel.TargetName = model.Name;
                    if (model.RoleLevel == 1000)
                        newmodel.RateAssigner = model.User;
                    else
                        newmodel.Assigner = model.User;
                    newmodel.Rate = model.Rate;
                    newmodel.Score = (int)model.AssignedScore;
                    newmodel.Month = month;
                    newmodel.Year = year;
                    CH.Create<AssignPerformanceScore>(newmodel);
                }
            }

            var list = CRM_Logical._EmployeePerformance.GetTeamLeadsPerformances(year.Value,month.Value);
            var data = list.OrderBy(p => p.Name).ToList();
            return View(new GridModel(data));

        }
        [GridAction]
        public ActionResult _SelectSalesIndex(int? year,int? month, string fuzzyInput = "")
        {
            if (year == null) year = DateTime.Now.Year;
            if (month == null) month = DateTime.Now.Month;
            ViewBag.fuzzyInput = fuzzyInput;
            // if (month == null) month = 5;
            var list = CRM_Logical._EmployeePerformance.GetSalesPerformances(year.Value,month.Value, fuzzyInput);
            list = list.OrderBy(p => p.Name);
            return View(new GridModel(list));
            //var data = list.ToList();
            //return View(new GridModel(data));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _UpdateSalesPerformance(int id,int? year, int? month)
        {
            if (year == null) year = DateTime.Now.Year;
            if (month == null) month = DateTime.Now.Month;
            _SalesPerformance model = new _SalesPerformance();
            AssignPerformanceScore newmodel = new AssignPerformanceScore();
            if (id > 0)
            {
                if (TryUpdateModel(model))
                {

                    newmodel = CH.GetDataById<AssignPerformanceScore>(id);
                    newmodel.TargetName = model.Name;
                    if (model.RoleLevel == 1000)
                        newmodel.RateAssigner = model.User;
                    else
                        newmodel.Assigner = model.User;
                    newmodel.Rate = model.Rate;

                    newmodel.Score = (int)model.AssignedScore;
                    newmodel.Month = month;
                    newmodel.Year = year;

                    CH.Edit<AssignPerformanceScore>(newmodel);
                }
                //var model = CH.GetDataById<ManagerScore>(id);
                //model.Item1Score = Item1ScoreString;
                //CH.Edit<ManagerScore>(model);
            }
            else
            {

                if (TryUpdateModel(model))
                {
                    newmodel.TargetName = model.Name;
                    newmodel.Rate = model.Rate;
                    if (model.RoleLevel == 1000)
                        newmodel.RateAssigner = model.User;
                    else
                        newmodel.Assigner = model.User;
                    newmodel.Score = (int)model.AssignedScore;
                    newmodel.Month = month;
                    newmodel.Year = year;
                    CH.Create<AssignPerformanceScore>(newmodel);
                }
            }

            var list = CRM_Logical._EmployeePerformance.GetSalesPerformances(year.Value,month.Value, "");
            var data = list.OrderBy(p => p.Name).ToList();
            return View(new GridModel(data));

        }


    }
}
