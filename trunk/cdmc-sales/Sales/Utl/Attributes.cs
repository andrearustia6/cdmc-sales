using System;
using System.Web.Mvc;
using System.Web.Security;
using System.Linq;
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

        string[] roles = System.Web.Security.Roles.GetRolesForUser(Membership.GetUser().UserName);
        string[] expects = Roles.Split(new string[]{",",";"}, StringSplitOptions.RemoveEmptyEntries);
        var matched = roles.FirstOrDefault(i=>expects.Contains(i) == true);

        if (!skipAuthorization && matched==null)
        {
            filterContext.Result = new HttpUnauthorizedResult(); 
        
            base.OnAuthorization(filterContext);
        }
    }
}