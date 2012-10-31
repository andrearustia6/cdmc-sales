﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entity;
using Utl;

namespace System.Web.Mvc
{
    public static class TargetOfMonthControllerExtension
    {
        public static void AddErrorStateIfTargetOfMonthNoValid(this Controller item, TargetOfMonth t) 
        {
            item.AddErrorStateIfStartDateLaterThanEndDate(t.StartDate, t.EndDate);

            if(t.StartDate.StartOfMonth() != t.StartDate)
                item.ModelState.AddModelError("", "开始时间必须是每个月的一号");

            if (t.EndDate.EndOfMonth() != t.EndDate)
                item.ModelState.AddModelError("", "结束时间必须是每个月的最后一天");

            if (t.BaseDeal > t.Deal)
                item.ModelState.AddModelError("", "保底目标不能大于Deal");

            if (t.CheckIn > t.Deal)
                item.ModelState.AddModelError("", "Check In不能大于Deal");

            if (t.StartDate.Month != t.EndDate.Month)
                item.ModelState.AddModelError("", "开始时间和结束时间不在同一个月内");

            if(CH.GetAllData<TargetOfMonth>(i=>i.StartDate.ToShortDateString()== t.StartDate.ToShortDateString()&&t.ProjectID==i.ProjectID).Count()>0)
                 item.ModelState.AddModelError("", "该月的目标已经添加，不能再次添加");
        }

        public static void AddErrorStateIfNotFromMondayToFriday(this Controller item, DateTime startdate, DateTime enddate)
        {
            if (startdate.DayOfWeek != DayOfWeek.Monday || enddate.DayOfWeek != DayOfWeek.Friday)
            {
                item.ModelState.AddModelError("", "开始时间不是周一，或者结束时间不是周五");
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
        public static List<Project> GetProjectByAccount(this Controller item) 
        {
            List<Project> list = new List<Project>();
            Role role = Employee.GetCurrentRole();

            var username = HttpContext.Current.User.Identity.Name;

            if (role.Level == Role.LVL_Director)
            {
                list = CH.GetAllData<Project>("Members","CompanyRelationships");
            }
            else
            {
                list = CH.GetAllData<Project>(p=>p.Manager== username,"Members");
            }

            return list;
        }
    }

    public static class MemberControllerExtension
    {
        public static void AddErrorStateIfMemberExist(this Controller item, int? projectid, string name=null)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = HttpContext.Current.User.Identity.Name;
            }

            var p = CH.GetDataById<Project>(projectid, "Members");

            if (p.Members.Any(m=>m.Name==name))
            {
                item.ModelState.AddModelError("", "项目已经包含该员工，员工名为：" + name);
            }
        }
    }

    public static class ControllerExtension
    {
        public static void AddErrorStateIfFieldExist<T>(this Controller item,EntityBase target,string fieldname) where T:EntityBase
        {
            if (target.SameFieldValueExist<T>("Name"))
            {
               item.ModelState.AddModelError("","已经存在相同的字段，字段名为："+fieldname);
            }
        }

        /// <summary>
        /// 记录中存在相同的中文名或者英文名
        /// </summary>
        public static void AddAddErrorStateIfOneOfNameExist<T>(this Controller item, string enname, string chname) where T:FullNameEntity
        {
            var exists = CH.GetAllData<T>(i => (i.Name_EN == enname &&  !string.IsNullOrEmpty(enname)) || (!string.IsNullOrEmpty(chname) && i.Name_CH == chname));
            if(exists.Count>0)
                item.ModelState.AddModelError("", "系统数据库中已经存在相同的公司英文名或中文名的数据");
        }

        public static void AddErrorStateIfStartDateLaterThanEndDate(this Controller item, DateTime? startdate, DateTime? enddate) 
        {
            if (startdate!=null && enddate!=null && startdate.Value>=enddate.Value)
            {
                item.ModelState.AddModelError("", "开始时间不能大于结束时间");
            }
        }

        public static void AddErrorIfAllNamesEmpty(this Controller item, FullNameEntity target)
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
}