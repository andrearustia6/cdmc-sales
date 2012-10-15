using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entity;

namespace System.Web.Mvc
{
    public static class ControllerExtension
    {
        public static void AddErrorStateIfFieldExist(this Controller item,EntityBase target,string fieldname)
        { 
             if (target.SameFieldExist<Category>("Name"))
            {
               item.ModelState.AddModelError("","已经存在相同的字段，字段名为："+fieldname);
            }
        }
    }
}