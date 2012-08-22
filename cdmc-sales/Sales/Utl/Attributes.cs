using System;
using System.Web.Mvc;
using System.Web.Security;
using System.Linq;
using Utl;
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class AllowAnonymousAttribute : Attribute { }

public sealed class LogonRequired : AuthorizeAttribute
{
    public override void OnAuthorization(AuthorizationContext filterContext)
    {
        bool skipAuthorization = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
        || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true);
        if (!skipAuthorization)
        {
            base.OnAuthorization(filterContext);
        }
    }
}

public sealed class ManagerRequired : AuthorizeAttribute
{
    public override void OnAuthorization(AuthorizationContext filterContext)
    {
        bool skipAuthorization = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
        || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true);
         var level = Employee.GetCurrentRoleLevel();

         if (level > 500) skipAuthorization = true;
      

        if (!skipAuthorization)
        {
            filterContext.Result = new HttpUnauthorizedResult(); 
        
            base.OnAuthorization(filterContext);
        }
    }
}