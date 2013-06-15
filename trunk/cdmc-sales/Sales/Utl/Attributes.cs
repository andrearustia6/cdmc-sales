using System;
using System.Web.Mvc;
using System.Web.Security;
using System.Linq;
using Utl;
using System.Collections.Generic;
using Entity;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class AllowAnonymousAttribute : Attribute { }

//public sealed class ProjectAccessRightRequired : AuthorizeAttribute
//{
//    public override void OnAuthorization(AuthorizationContext filterContext)
//    {

//        bool skipAuthorization = filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true);

//        var right = from r in  CH.DB.ProjectRights where r.
//        //skipAuthorization = true;

//        if (!skipAuthorization)
//        {
//            base.OnAuthorization(filterContext);
//        }
//    }
//}

public sealed class LogonRequired : AuthorizeAttribute
{
    public override void OnAuthorization(AuthorizationContext filterContext)
    {

        bool skipAuthorization = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true);

        if (Utl.Utl.DebugModel())
        {
            skipAuthorization = true;
            var name = Utl.Utl.DebugAccount();
            FormsAuthentication.SetAuthCookie(name, true);
        }

        //skipAuthorization = true;

        if (!skipAuthorization)
        {
            base.OnAuthorization(filterContext);
        }
    }
}

public enum AccessType
{
    Equal, Upper, Lower, Multiple
}




public class RoleRequired : AuthorizeAttribute
{
    public AccessType AccessType { get; set; }

    public RoleRequired()
    {
        AccessType = AccessType.Upper;
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
        var level = Employee.CurrentRole.Level;
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
/// 提供权限给产品接口人 项目管理人员 
/// </summary>
public sealed class ProjectInformationAccess : AuthorizeAttribute
{
    public int[] Levels { get; set; }
    public ProjectInformationAccess(params int[] levels)
    {
        Levels = new int[] { RoleLevel.ProductInterface, RoleLevel.Director, RoleLevel.Manager };
    }

    public override void OnAuthorization(AuthorizationContext filterContext)
    {
        bool skipAuthorization = false;
        var level = Employee.CurrentRole.Level;
        if (Levels.Any(l => l == level))
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
/// 99999
/// </summary>
public sealed class AdministratorRequired : RoleRequired
{
    public const int LVL = 99999;
    public override int Level
    {
        get { return LVL; }
    }

    /// <summary>
    /// Override authriztion method for admin.
    /// </summary>
    /// <param name="filterContext">Default para.</param>
    public override void OnAuthorization(AuthorizationContext filterContext)
    {
        //Judging against login user rather than current user.
        bool loginUserIsAdmin = Employee.LoginUserIsAdmin();
        if (!loginUserIsAdmin)
        {
            filterContext.Result = new HttpUnauthorizedResult();
            base.OnAuthorization(filterContext);
        }
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



/// <summary>
/// 3
/// </summary>
public sealed class ConferenceInterfaceRequired : RoleRequired
{
    public const int LVL = 3;
    public override int Level
    {
        get { return LVL; }
    }
}

/// <summary>
/// 4
/// </summary>
public sealed class FinancialInterfaceRequired : RoleRequired
{
    public const int LVL = 4;
    public override int Level
    {
        get { return LVL; }
    }
}

/// <summary>
/// 2
/// </summary>
public sealed class PoliticsInterfaceRequired : RoleRequired
{
    public const int LVL = 2;
    public override int Level
    {
        get { return LVL; }
    }
}

/// <summary>
/// 6
/// </summary>
public sealed class ImportingInterfaceRequired : RoleRequired
{
    public const int LVL = 6;
    public override int Level
    {
        get { return LVL; }
    }
}

/// <summary>
/// 1500
/// </summary>
public sealed class SuperManagerRequired : RoleRequired
{
    public const int LVL = 800;
    public override int Level
    {
        get { return LVL; }
    }
}

public class RoleLevel
{
    public static int SuperManager = 1500;
    public static int Director = 1000;
    public static int Manager = 500;
    public static int Leader = 100;
    public static int Sales = 10;
    public static int ImportingInterfaceRequired = 6;
    public static int ProductInterface = 5;
    public static int FinancialInterface = 4;
    public static int ConferenceInterface = 3;
    public static int PoliticsInterface = 2;
    public static int MarketInterface = 1;
}


