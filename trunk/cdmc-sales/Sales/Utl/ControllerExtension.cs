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

        public static void AddErrorStateIfFieldExist<T>(this Controller item, object targetvalue, string fieldname) where T : EntityBase
        {
            var value = targetvalue;
            var data = CH.GetAllData<T>(child => child.GetType().GetProperty(fieldname).GetValue(child, null).ToString() == value.ToString());
            if (data.Count > 0)
            {
                item.ModelState.AddModelError("", "已经存在相同的字段，字段名为：" + fieldname);
            }
        }
    }
}