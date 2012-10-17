using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entity;
using Utl;

namespace System.Web.Mvc
{
    public static class ControllerExtension
    {
        public static void AddErrorStateIfFieldExist<T>(this Controller item,EntityBase target,string fieldname) where T:EntityBase
        {
            if (target.SameFieldValueExist<T>("Name"))
            {
               item.ModelState.AddModelError("","已经存在相同的字段，字段名为："+fieldname);
            }
        }

        public static void AddErrorIfOneOfNamesEmpty(this Controller item, FullNameEntity target)
        {
            if (target.IsAllNamesEmpty())
            {
                item.ModelState.AddModelError("", "中文名和英文名不允许同时为空");
            }
        }

        public static void AddErrorStateIfFieldExist<T>(this Controller item, object targetvalue, string fieldname) where T : EntityBase
        {
            var value = targetvalue;
            var data = CH.GetAllData<T>(child => child.GetType().GetProperty(fieldname).GetValue(child, null).ToString() == value.ToString());
            if (data.Count > 0)
            {
                item.ModelState.AddModelError("", "已经存在相同的字段，字段名为：" + fieldname);
            }
        }

        public static void AddErrorStateIfSalesNoAccessRight(this Controller item, int? crid) 
        {
            var cr = CH.GetDataById<CompanyRelationship>(crid);
            var role = cr.Project.RoleInProject();
            if (role == RoleInProject.NotIn || role == RoleInProject.NotIn || role== RoleInProject.MarketInterface || role== RoleInProject.ProductInterface)
            {
                item.ModelState.AddModelError("", "对不起，您在此项目中的权限是：" + role.ToString() + ", 无法访问此页面。");
            }
        }
    }
}