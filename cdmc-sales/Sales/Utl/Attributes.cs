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

public class RoleRequired : AuthorizeAttribute
{
    public virtual int Level { get; set; }
    public override void OnAuthorization(AuthorizationContext filterContext)
    {
        bool skipAuthorization = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
        || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true);
        var level = Employee.GetCurrentRoleLevel();

        if (level >= Level) skipAuthorization = true;


        if (!skipAuthorization)
        {
            filterContext.Result = new HttpUnauthorizedResult();

            base.OnAuthorization(filterContext);
        }
    }
}
/// <summary>
/// 1000
/// </summary>
public sealed class SupervisorRequired : RoleRequired
{
    public static const int LVL = 1000;
    public override int Level
    {
        get { return LVL; }
    }
}

/// <summary>
/// 500
/// </summary>
public sealed class ManagerRequired : RoleRequired
{
    public static const int LVL = 500;
    public override int Level
    {
        get { return LVL; }
    }
}

/// <summary>
/// 10
/// </summary>
public sealed class SalesRequired : RoleRequired
{
    public static const int LVL = 10;
    public override int Level
    {
        get { return LVL; }
    }
}

/// <summary>
/// 1
/// </summary>
public sealed class MarketInterfaceRequired : RoleRequired
{
    public static const int LVL = 1;
    public override int Level
    {
        get { return LVL; }
    }
}

/// <summary>
/// 5
/// </summary>
public sealed class ProductInterfaceRequired : RoleRequired
{
    public static const int LVL = 5;
    public override int Level
    {
        get { return LVL; }
    }
}