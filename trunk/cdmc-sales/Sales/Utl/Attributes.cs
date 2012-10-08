using System;
using System.Web.Mvc;
using System.Web.Security;
using System.Linq;
using Utl;
using System.Collections.Generic;
using Entity;
using System.Reflection;
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

public enum AccessType
{
    Equal, Upper, Lower,Multiple
}

public class RoleRequired : AuthorizeAttribute
{
    public AccessType AccessType { get; set; }

    public RoleRequired()
    {
        AccessType = AccessType.Equal;
    }

    public RoleRequired(AccessType access)
    {
        AccessType = access;
    }

    protected bool IsCreatorCallTheController(string actionname, string controllername, int id, string user)
    {
        var types = Assembly.Load("Entity").GetTypes();
        var type = types.First(t => t.Name == controllername);
        var tar = CH.DB.Set(type).Find(id);
        var exist = tar as EntityBase;
        if (exist != null && exist.Creator == user)
        {
            return true;
        }
        return false;
    }

    public virtual int Level { get; set; }
    public override void OnAuthorization(AuthorizationContext filterContext)
    {
        bool skipAuthorization = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
        || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true);
        var level = Employee.GetCurrentRoleLevel();

        if (AccessType == AccessType.Upper)
        {
            if (level >= Level) skipAuthorization = true;
        }
        else if (AccessType == AccessType.Equal)
        {
            if (level == Level) skipAuthorization = true;
        }
        else
        {
            if (level <= Level) skipAuthorization = true;
        }




        if (!skipAuthorization)
        {
            filterContext.Result = new HttpUnauthorizedResult();

            base.OnAuthorization(filterContext);
        }
    }
}

/// <summary>
/// Multiple
/// </summary>
public sealed class MultipleRoleAccess : AuthorizeAttribute
{
    public RoleRequired[] Roles { get; set; }
    public MultipleRoleAccess(params RoleRequired[] roles)
    {
        Roles = roles;
    }

    public override void OnAuthorization(AuthorizationContext filterContext)
    {
        bool skipAuthorization = false;
        var level = Employee.GetCurrentRoleLevel();
        if (Roles.Any(r => r.Level == level))
        {
            skipAuthorization = true;
        }
        if (!skipAuthorization)
          base.OnAuthorization(filterContext);
    }
}

/// <summary>
/// 1000
/// </summary>
public sealed class DirectorRequired : RoleRequired
{
    public const int LVL = 1000;
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
    public const int LVL = 500;
    public override int Level
    {
        get { return LVL; }
    }
}

/// <summary>
/// 100
/// </summary>
public sealed class LeaderRequired : RoleRequired
{
    public const int LVL = 100;
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

    public override void OnAuthorization(AuthorizationContext filterContext)
    {
        base.OnAuthorization(filterContext);

        var actionname = filterContext.ActionDescriptor.ActionName;
        if (actionname == "Edit" || actionname == "Delete" || actionname == "Display")
        {
            var controllername = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            object oid;
            filterContext.RouteData.Values.TryGetValue("id", out oid);
            int id = Int32.Parse(oid.ToString());
            var user = filterContext.HttpContext.User.Identity.Name;

            if (!IsCreatorCallTheController(actionname, controllername, id, user))
            {

                filterContext.Result = new HttpUnauthorizedResult();
                base.OnAuthorization(filterContext);

            }

        }


    }
    public const int LVL = 10;
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
    public const int LVL = 1;
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
    public const int LVL = 5;
    public override int Level
    {
        get { return LVL; }
    }
}


