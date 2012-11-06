using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utl;
using Entity;

namespace Sales.Controllers
{
    [ProductInterfaceRequired(AccessType = AccessType.Equal)]
    public class ProductInterfaceController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></return
        public ViewResult CompanyRelationshipIndex(int? projectid)
        {
           
            var project = CH.GetDataById<Project>(projectid, "CompanyRelationships");
            if (project != null)
            {
                return View(project);
            }
            else
            {
                var pp = CH.GetAllData<Project>(p => p.Product == User.Identity.Name, "CompanyRelationships").FirstOrDefault();
                if (pp == null)
                    return View();
                else
                {
                    return View(pp);
                }
            }
            
        }


    }
}
