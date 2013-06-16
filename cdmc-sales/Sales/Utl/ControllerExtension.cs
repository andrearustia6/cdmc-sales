using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entity;
using Utl;
using BLL;

namespace System.Web.Mvc
{

    public static class TargetOfMonthForMemberControllerExtension
    {
        public static void AddErrorStateIfTargetOfMonthNoValid(this Controller item, TargetOfMonthForMember t)
        {
            item.AddErrorStateIfStartDateLaterThanEndDate(t.StartDate, t.EndDate);

            if (t.StartDate.StartOfMonth() != t.StartDate)
                item.ModelState.AddModelError("", "开始时间必须是每个月的一号");

            if (t.EndDate.EndOfMonth() != t.EndDate)
                item.ModelState.AddModelError("", "结束时间必须是每个月的最后一天");

            //if (t.BaseDeal > t.Deal)
            //    item.ModelState.AddModelError("", "保底目标不能大于Deal");

            if (t.Deal <= 0 || t.CheckIn <= 0)
            {
                item.ModelState.AddModelError("", "销售目标和入账目标必须大于0");
            }

            if (t.StartDate.Month != t.EndDate.Month)
                item.ModelState.AddModelError("", "开始时间和结束时间不在同一个月内");

            var ts = from et in CH.DB.TargetOfMonthForMembers
                     where et.MemberID == t.MemberID && et.StartDate == t.StartDate && t.ProjectID == et.ProjectID
                     select et;

            if (ts.Count() > 0)
                item.ModelState.AddModelError("", "该月的目标已经添加，不能再次添加");

        }

        public static void EditErrorStateIfTargetOfMonthNoValid(this Controller item, TargetOfMonthForMember t)
        {
            item.AddErrorStateIfStartDateLaterThanEndDate(t.StartDate, t.EndDate);

            if (t.StartDate.StartOfMonth() != t.StartDate)
                item.ModelState.AddModelError("", "开始时间必须是每个月的一号");

            if (t.EndDate.EndOfMonth() != t.EndDate)
                item.ModelState.AddModelError("", "结束时间必须是每个月的最后一天");

            //if (t.BaseDeal > t.Deal)
            //    item.ModelState.AddModelError("", "保底目标不能大于Deal");

            if (t.Deal <= 0 || t.CheckIn <= 0)
            {
                item.ModelState.AddModelError("", "销售目标和入账目标必须大于0");
            }

            if (t.IsConfirm == true)
                item.ModelState.AddModelError("", "已确认月目标无法修改");

            if (t.StartDate.Month != t.EndDate.Month)
                item.ModelState.AddModelError("", "开始时间和结束时间不在同一个月内");
        }

    }
    public static class TargetOfMonthControllerExtension
    {
        public static void AddErrorStateIfTargetOfMonthNoValid(this Controller item, TargetOfMonth t)
        {
            item.AddErrorStateIfStartDateLaterThanEndDate(t.StartDate, t.EndDate);

            if (t.StartDate.StartOfMonth() != t.StartDate)
                item.ModelState.AddModelError("", "开始时间必须是每个月的一号");

            if (t.EndDate.EndOfMonth() != t.EndDate)
                item.ModelState.AddModelError("", "结束时间必须是每个月的最后一天");

            //if (t.BaseDeal > t.Deal)
            //    item.ModelState.AddModelError("", "保底目标不能大于Deal");

            if (t.Deal <= 0 || t.CheckIn <= 0)
            {
                item.ModelState.AddModelError("", "销售目标和入账目标必须大于0");
            }

            if (t.StartDate.Month != t.EndDate.Month)
                item.ModelState.AddModelError("", "开始时间和结束时间不在同一个月内");

            var ts = from et in CH.DB.TargetOfMonths
                     where et.StartDate == t.StartDate && t.ProjectID == et.ProjectID && et.ID != t.ID
                     select et;

            if (ts.Count() > 0)
                item.ModelState.AddModelError("", "该月的目标已经添加，不能再次添加");

            if ((t.TargetOf1stWeek + t.TargetOf2ndWeek + t.TargetOf3rdWeek + t.TargetOf4thWeek + t.TargetOf5thWeek) < t.Deal)
                item.ModelState.AddModelError("", "周目标总和必须大于等于月目标");
        }

        public static void AddErrorStateIfNotFromMondayToFriday(this Controller item, DateTime startdate, DateTime enddate)
        {
            if (startdate.DayOfWeek != DayOfWeek.Monday || enddate.DayOfWeek != DayOfWeek.Friday)
            {
                item.ModelState.AddModelError("", "开始时间不是周一，或者结束时间不是周五");
            }
        }

        public static void AddErrorStateIfStartDateAndEndDateEmpty(this Controller item, DateTime? startdate, DateTime? enddate)
        {
            if (startdate == null || enddate == null)
            {
                item.ModelState.AddModelError("", "开始时间或者结束时间为空");
            }
        }

        public static void AddErrorStateIfTargetOfWeekExist(this Controller item, DateTime startdate, int targetofmonthid)
        {
            var ts = CH.GetAllData<TargetOfWeek>(t => t.StartDate.ToShortDateString() == startdate.ToShortDateString() && t.TargetOfMonthID == targetofmonthid);
            if (ts.Count != 0)
                item.ModelState.AddModelError("", "已存在该周目标，不能添加");
        }
    }

    public static class ReportControllerExtension
    {
        public static List<Project> GetProjectByAccount(this Controller item, bool activate = true)
        {
            List<Project> list = new List<Project>();
            Role role = Employee.CurrentRole;

            var username = Employee.CurrentUserName;

            if (role.Level == Role.LVL_Director)
            {
                list = CH.GetAllData<Project>(p => p.IsActived == activate, "Members", "CompanyRelationships");
            }
            else if (role.Level == Role.LVL_TeamLeader)
            {
                list = CH.GetAllData<Project>(p => p.TeamLeader == username && p.IsActived == activate, "Members");
            }
            else if (role.Level == Role.LVL_Manager)
            {
                list = CH.GetAllData<Project>(p => p.Manager == username && p.IsActived == activate, "Members");
            }
            else if (role.Level == Role.LVL_MarketInterface)
            {
                list = CH.GetAllData<Project>(p => p.Market == username && p.IsActived == activate, "Members");
            }
            else if (role.Level == Role.LVL_ProductInterface)
            {
                list = CH.GetAllData<Project>(p => p.Product == username && p.IsActived == activate, "Members");
            }

            return list;
        }
    }

    public static class MemberControllerExtension
    {
        public static void AddErrorStateIfMemberExist(this Controller item, int? projectid, string name = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = Employee.CurrentUserName;
            }

            var p = CH.GetDataById<Project>(projectid);

            if (p.Members.Any(m => m.Name == name))
            {
                item.ModelState.AddModelError("", "项目已经包含该销售，销售名为：" + name);
            }
        }
    }

    public static class ControllerExtension
    {

        public static List<Project> TryGetSelectedProjects(this Controller item, List<int> selectedprejcts)
        {
            if (selectedprejcts == null)
            {
                return BLL.CRM_Logical.GetUserInvolveProject();
            }
            else
            {
                return CH.GetAllData<Project>(p => selectedprejcts.Any(a => a == p.ID));
            }
        }

        public static List<int> TryGetSelectedProjectIDs(this Controller item, List<int> selectedprejcts)
        {
            if (selectedprejcts == null)
            {
                return new List<int>();
            }
            else
            {
                return selectedprejcts;
            }
        }
        /// <summary>
        /// 当所属项目未选定时，自动导入存在的项目id
        /// </summary>
        /// <param name="item"></param>
        /// <param name="projectid"></param>
        /// <returns></returns>
        public static int? TrySetProjectIDForUser(this Controller item, int? projectid)
        {
            if (projectid == null)
            {
                var data = CRM_Logical.GetUserInvolveProject().FirstOrDefault();

                if (data != null)
                {
                    projectid = data.ID;
                }
            }
            return projectid;
        }



        public static void AddErrorStateIfCreatorIsNotTheLoginUser(this Controller item, EntityBase target)
        {
            if (!target.CreatorIsTheLoginUser())
                item.ModelState.AddModelError("", "登陆用户不是此数据的创建人， 不允许进行操作");
        }

        public static void AddErrorStateIfCallerIsNotTheLoginUser(this Controller item, LeadCall target)
        {
            if (!target.CallerIsTheLoginUser())
                item.ModelState.AddModelError("", "登陆用户不是致电人， 不允许进行操作");
        }

        public static void AddErrorStateIfFieldExist<T>(this Controller item, EntityBase target, string fieldname) where T : EntityBase
        {
            if (target.SameFieldValueExist<T>("Name"))
            {
                item.ModelState.AddModelError("", "已经存在相同的字段，字段名为：" + fieldname);
            }
        }

        /// <summary>
        /// 记录中存在相同的中文名或者英文名
        /// </summary>
        public static void AddAddErrorStateIfOneOfNameExist<T>(this Controller item, string enname, string chname) where T : NameEntity
        {
            var exists = CH.GetAllData<T>(i => (i.Name_EN == enname && !string.IsNullOrEmpty(enname)) || (!string.IsNullOrEmpty(chname) && i.Name_CH == chname));
            if (exists.Count > 0)
                item.ModelState.AddModelError("", "系统数据库中已经存在相同的公司英文名或中文名的数据");
        }

        public static void AddErrorStateIfStartDateLaterThanEndDate(this Controller item, DateTime? startdate, DateTime? enddate)
        {
            if (startdate != null && enddate != null && startdate.Value >= enddate.Value)
            {
                item.ModelState.AddModelError("", "开始时间不能大于结束时间");
            }
        }

        public static void AddErrorIfAllNamesEmpty(this Controller item, NameEntity target)
        {
            if (target.IsAllNamesEmpty())
            {
                item.ModelState.AddModelError("", "中文名和英文名不允许同时为空");
            }
        }

        public static void AddErrorIfAllNamesEmpty(this Controller item, string enname, string chname)
        {
            if (string.IsNullOrEmpty(enname) && string.IsNullOrEmpty(chname))
            {
                item.ModelState.AddModelError("", "中文名和英文名不允许同时为空");
            }
        }

        public static void AddErrorStateIfSalesNoAccessRightToTheCRM(this Controller item, int? crid)
        {
            var cr = CH.GetDataById<CompanyRelationship>(crid);
            var role = cr.Project.RoleInProject();
            if (role == RoleInProject.NotIn)
            {
                item.ModelState.AddModelError("", "对不起，您在此项目中的权限是：" + role.ToString() + ", 无法访问此页面");
            }
        }

        public static void AddErrorStateIfSalesNoAccessRightToTheProject(this Controller item, int? projectid)
        {
            if (projectid == null)
            {
                item.ModelState.AddModelError("", "对不起，请先选择需要添加出单的项目");
                return;
            }
            var p = CH.GetDataById<Project>(projectid);
            var role = p.RoleInProject();
            if (role == RoleInProject.NotIn)
            {
                item.ModelState.AddModelError("", "对不起，您在此项目中的权限是：" + role.ToString() + ", 无法访问此页面");
            }
        }
    }

    public static class MessageExtension
    {
        public static Message SetFlowNumber(this Message item, Project project)
        {
            var last = CH.GetAllData<Message>(m => !string.IsNullOrEmpty(m.FlowNumber) && m.FlowNumber.Contains(project.ProjectCode)).OrderByDescending(o => o.CreatedDate).FirstOrDefault();
            string procode;

            if (last == null)
            {

                procode = item.FlowNumber = project.ProjectCode + "_" + "1";

            }
            else
            {
                string number = last.FlowNumber.Replace(last.Project.ProjectCode + "_", "");
                int n = 0;
                Int32.TryParse(number, out n);
                n = n + 1;
                item.FlowNumber = last.Project.ProjectCode + "_" + n.ToString();
            }

            return item;
        }
    }

    public static class ProjectExtension
    {
        public static List<Project> GetProjectByRole(this Controller item)
        {
            var role = Employee.CurrentRole;
            if (role.Level >= 500 && role.Level < 1000)
                return CH.GetAllData<Project>(p => p.RoleInProject() == RoleInProject.Manager && p.IsActived == true);
            else if (role.Level == 1000)
                return CH.GetAllData<Project>(p => p.RoleInProject() == RoleInProject.Director && p.IsActived == true);
            else if (role.Level == 99999)
                return CH.GetAllData<Project>();
            else
                return new List<Project>();
        }
    }

    public static class MarketInterfaceExtension
    {
        public static void AddErrorStateIfCreatorIsTheLoginUserIsNotTheMarketInterface(this Controller item, Project target)
        {
            if (target.Market != Employee.CurrentUserName)
                item.ModelState.AddModelError("", "登陆用户不是项目的市场部接口人， 不允许进行操作");
        }
    }
}