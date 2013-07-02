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
        public ActionResult Index(int? month, string btnExport)
        {
            ViewBag.Month = month;
            var rolelvl = Employee.CurrentRole.Level;
            if (rolelvl == PoliticsInterfaceRequired.LVL)
            {
                rolelvl = DirectorRequired.LVL;
            }
            ViewBag.RoleLevel = rolelvl;

            if (btnExport == "export")
            {
                MemoryStream output = new MemoryStream();
                StreamWriter writer = new StreamWriter(output, System.Text.Encoding.UTF8);

                if (month == null) month = DateTime.Now.Month;

                writer.Write(month + "月");
                writer.WriteLine();

                var EmployeeList = CRM_Logical._EmployeePerformance.GetManagerLeadsPerformances(month.Value).ToList();

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
                        writer.Write(string.Format("{0:P}", item.ResponsibilityDisp));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:P}", item.DisciplineDisp));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:P}", item.ExcutionDisp));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:P}", item.TargetingDisp));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:P}", item.SearchingDisp));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:P}", item.ProductionDisp));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:P}", item.PitchPaperDisp));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:P}", item.WeeklyMeetingDisp));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:P}", item.MonthlyMeetingDisp));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:P}", item.Calllist));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:P}", item.AddLeads));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:P}", item.CheckIn));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(item.Rate);
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(item.Score);
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write(item.Confirmed);
                        writer.WriteLine();

                    }

                    writer.WriteLine();
                    writer.WriteLine();
                }

                var TlList = CRM_Logical._EmployeePerformance.GetTeamLeadsPerformances(month.Value).ToList(); ;
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
                        writer.Write(string.Format("{0:P}", item.CompletePercent));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(item.CheckinScore);
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(item.LeadNotQualifiedWeeksCount);
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(item.AddLeadScore);
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(item.HoursOrFaxNotQualifiedWeeksCount);
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(item.FaxCallScore);
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(item.AssignedScore);
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(item.Rate);
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(item.Score);
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

                var salesList = CRM_Logical._EmployeePerformance.GetSalesPerformances(month.Value, string.Empty).ToList();

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
                    writer.Write("调研详细");

                    writer.WriteLine();


                    foreach (var item in salesList)
                    {
                        writer.Write(item.Name);
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(string.Format("{0:P}", item.CompletePercent));
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(item.CheckinScore);
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(item.LeadNotQualifiedWeeksCount);
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(item.AddLeadScore);
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(item.HoursOrFaxNotQualifiedWeeksCount);
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(item.FaxCallScore);
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(item.AssignedScore);
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(item.Rate);
                        writer.Write("\"");
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(item.Score);
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


                writer.Flush();
                output.Position = 0;
                return File(output, "text/comma-separated-values", "Performances.csv");

            }

            return View();
        }
        [GridAction]
        public ActionResult _SelectManagerIndex(int? month)
        {
            if (month == null) month = DateTime.Now.Month;
            // if (month == null) month = 5;
            var list = CRM_Logical._EmployeePerformance.GetManagerLeadsPerformances(month.Value);
            var data = list.ToList();
            return View(new GridModel(data));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _UpdateManagerScore(int id, int? month)
        {
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
                    newmodel.Year = DateTime.Now.Year;

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
                    newmodel.Year = DateTime.Now.Year;
                    CH.Create<ManagerScore>(newmodel);
                }
            }

            var list = CRM_Logical._EmployeePerformance.GetManagerLeadsPerformances(month.Value);
            var data = list.ToList();


            return View(new GridModel(data));

        }
        //[AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _ConfirmManagerScore(int id, int? month)
        {

            if (month == null) month = DateTime.Now.Month;
            if (id > 0)
            {
                ManagerScore model = CH.GetDataById<ManagerScore>(id);
                model.Confirmed = true;
                CH.Edit<ManagerScore>(model);
                //return View(model);
            }
            var list = CRM_Logical._EmployeePerformance.GetManagerLeadsPerformances(month.Value);
            var data = list.ToList();

            return View(new GridModel(data));

        }
        [GridAction]
        public ActionResult _SelectLeadIndex(int? month)
        {
            if (month == null) month = DateTime.Now.Month;
            // if (month == null) month = 5;
            var list = CRM_Logical._EmployeePerformance.GetTeamLeadsPerformances(month.Value);
            var data = list.ToList();
            return View(new GridModel(data));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _UpdateTeamLeadPerformance(int id, int? month)
        {
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
                    newmodel.Year = DateTime.Now.Year;

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
                    newmodel.Year = DateTime.Now.Year;
                    CH.Create<AssignPerformanceScore>(newmodel);
                }
            }

            var list = CRM_Logical._EmployeePerformance.GetTeamLeadsPerformances(month.Value);
            var data = list.ToList();
            return View(new GridModel(data));

        }
        [GridAction]
        public ActionResult _SelectSalesIndex(int? month, string fuzzyInput = "")
        {
            ViewBag.fuzzyInput = fuzzyInput;
            if (month == null) month = DateTime.Now.Month;
            // if (month == null) month = 5;
            var list = CRM_Logical._EmployeePerformance.GetSalesPerformances(month.Value, fuzzyInput);
            var data = list.ToList();
            return View(new GridModel(data));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _UpdateSalesPerformance(int id, int? month)
        {
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
                    newmodel.Year = DateTime.Now.Year;

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
                    newmodel.Year = DateTime.Now.Year;
                    CH.Create<AssignPerformanceScore>(newmodel);
                }
            }

            var list = CRM_Logical._EmployeePerformance.GetSalesPerformances(month.Value, "");
            var data = list.ToList();
            return View(new GridModel(data));

        }


    }
}
