﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using System.Configuration;
using System.Web.Script.Serialization;
using System.Web.Mvc;
using System.Web;
using Entity;
using Telerik.Web.Mvc;
using Attributes;
using System.Web.Security;
using System.Web.Profile;
using System.Xml;
using System.Data.OleDb;
using System.Data;
using Model;
namespace Utl
{
    /// <summary>
    /// 权限标记
    /// </summary>
    public enum ReviewRight
    {
        ProjectInfoReview,
        CallsReview,
        CallsSumReview,
        DealsReview,
        CallAnalysisReview,
        ProgressReview,
        EvaluationsReview,
        ProtectedCompanyReview,
        TargetsView,
        AvailableCompaniesReview
    }

    public enum EditRight { ProjectInfoEdit, MembersEdit, DealsEdit, EvaluationsEdit, TargetsEdit, AvailableCompaniesEdit }

    public class LeadCallLeadDistinct : IEqualityComparer<LeadCall>
    {
        public bool Equals(LeadCall x, LeadCall y)
        {
            if ((x.LeadID == y.LeadID))
            { return true; }
            else
            { return false; }
        }

        public int GetHashCode(LeadCall obj) { return 0; }
    }

    public class LeadCallCompanyDistinct : IEqualityComparer<LeadCall>
    {
        public bool Equals(LeadCall x, LeadCall y)
        {
            if ((x.CompanyRelationshipID == y.CompanyRelationshipID))
            { return true; }
            else
            { return false; }
        }

        public int GetHashCode(LeadCall obj) { return 0; }
    }

    public class MemberDistinct : IEqualityComparer<Member>
    {
        public bool Equals(Member x, Member y)
        {
            if ((x.Name == y.Name))
            { return true; }
            else
            { return false; }
        }

        public int GetHashCode(Member obj) { return 0; }
    }

    public static class AppConfig
    {
        public static string ConnectionStringSetting
        {
            get { return ConfigurationManager.AppSettings["ConnectionStringSetting"]; }
        }
    }


    public class Employee
    {
        public static UserInfoModel CurrentUser
        {
            get
            {
                if (HttpContext.Current != null && HttpContext.Current.Items["CurrentUser"] == null)
                {
                    HttpContext.Current.Items["CurrentUser"] = GetCurrentUser();
                }
                return HttpContext.Current.Items["CurrentUser"] as UserInfoModel;
            }
            set
            {
                if (HttpContext.Current != null)
                    HttpContext.Current.Items["CurrentUser"] = value;
            }
        }

        public static Role CurrentRole
        {
            get
            {
                if (HttpContext.Current != null && HttpContext.Current.Items["CurrentRole"] == null)
                {
                    HttpContext.Current.Items["CurrentRole"] = GetCurrentRole();
                }
                return HttpContext.Current.Items["CurrentRole"] as Role;
            }
            set
            {
                if (HttpContext.Current != null)
                    HttpContext.Current.Items["CurrentRole"] = value;
            }
        }

        public static string CurrentUserName
        {
            get
            {
                if (HttpContext.Current != null && HttpContext.Current.Items["CurrentUserName"] == null)
                {
                    HttpContext.Current.Items["CurrentUserName"] = GetCurrentUserName().ToLower();
                }
                if (HttpContext.Current == null)
                {
                    return "";
                }
                return HttpContext.Current.Items["CurrentUserName"] as string;
            }
            set
            {
                if (HttpContext.Current != null)
                    HttpContext.Current.Items["CurrentUserName"] = value;
            }
        }


        static UserInfoModel GetCurrentUser()
        {
            return GetUserByName(Employee.CurrentUserName);
        }

        static string GetCurrentUserName()
        {
            var username = GetLoginUserName();

            if (LoginUserIsAdmin())
            {
                // Access system with simulator authority.
                SimulatorConfig simulatorConfig =
                    CH.GetAllData<SimulatorConfig>(s => s.AdminName == username).SingleOrDefault();
                if (simulatorConfig == null)
                {
                    return username;
                }
                return simulatorConfig.SimulatorName;
            }
            else
                return username;
        }

        public static UserInfoModel GetUserByName(string username)
        {

            ProfileBase objProfile = ProfileBase.Create(username);

            var email = Membership.GetUser(username).Email;
            var roleid = CH.GetAllData<EmployeeRole>(w => w.AccountName == username).Select(s => s.RoleID).FirstOrDefault();
            var gender = objProfile.GetPropertyValue("Gender") as string;
            var displayName = objProfile.GetPropertyValue("DisplayName") as string;
            var mobile = objProfile.GetPropertyValue("Mobile") as string;
            int con = 0;
            Int32.TryParse(objProfile.GetPropertyValue("Contact").ToString(), out con);
            var contact = con;
            var departmentid = objProfile.GetPropertyValue("DepartmentID") as int?;
            var startDate = objProfile.GetPropertyValue("StartDate") as string;
            DateTime date;
            DateTime.TryParse(startDate, out date);
            var userinfo = new UserInfoModel()
            {
                RoleID = roleid.Value,
                Email = email,
                Contact = contact,
                StartDate = date,
                Mobile = mobile,
                Gender = gender,
                DisplayName = displayName,
                DepartmentID = departmentid,
                UserName = username
            };
            return userinfo;
        }

        /// <summary>
        /// Gets the name of current login user.
        /// </summary>
        /// <returns>User name of current logon user.</returns>
        public static string GetLoginUserName()
        {
            return HttpContext.Current.User.Identity.Name;
        }

        /// <summary>
        /// Gets the role of current login user.
        /// Note: This method only be used when admin login system with other role.
        /// In such case, Logon user is different from current user.
        /// </summary>
        /// <returns>User name of current logon user.</returns>
        public static bool LoginUserIsAdmin()
        {
            return GetRoleLevel(GetLoginUserName()) == AdministratorRequired.LVL;
        }

        public static List<MembershipUser> GetAllEmployees()
        {
            var list = Membership.GetAllUsers().Cast<MembershipUser>().ToList<MembershipUser>();

            return list;
        }

        /// <summary>
        /// 取得所有 基本的对象
        /// </summary>
        /// <returns></returns>
        public static IQueryable<string> GetEmplyeeByLVL(params int[] lvl)
        {
           // var names = Membership.GetAllUsers().Cast<MembershipUser>().Where(s => s.IsLockedOut == false).Select(s => s.UserName).ToList();
            var emps = from e in CH.DB.EmployeeRoles.Where(w => w.IsActivated==true) select e;

            var data = from r in CH.DB.Roles
                       from e in emps
                       where lvl.Any(a => a == r.Level && r.ID == e.RoleID)
                       select e.AccountName;

            //var data = new List<MembershipUser>();
            //list.ForEach(l =>
            //{
            //    if (lvl.Contains(GetRoleLevel(l.UserName)))
            //        data.Add(l);
            //});
            return data.OrderBy(o => o);
        }

        ///// <summary>
        ///// 取得所有 基本的对象 for autocomplete multiple
        ///// </summary>
        ///// <returns></returns>
        //public static List<string> GetEmplyeeNameByLVL(params int[] lvl)
        //{
        //    var list = Membership.GetAllUsers().Cast<MembershipUser>().ToList<MembershipUser>();
        //    var data = new List<string>();
        //    list.ForEach(l =>
        //    {
        //        if (lvl.Contains(GetRoleLevel(l.UserName)))
        //            data.Add(l.UserName);
        //    });
        //    return data;
        //}

        /// <summary>
        /// 添加成员
        /// </summary>
        /// <returns></returns>
        public static IQueryable<string> GetAvailbleSales(int? projectid, params string[] ms)
        {
            var list = GetEmplyeeByLVL(SalesRequired.LVL, LeaderRequired.LVL);
            var currentmembers = from m in CH.DB.Members where m.ProjectID == projectid select m.Name;
            var sales = from s in list.Where(w => currentmembers.Any(a => a == w) == false) select s;

            //if (ms != null)
            //{
            //    ms.ToList().ForEach(m =>
            //    {
            //        if (!string.IsNullOrEmpty(m))
            //            list.Add(Membership.GetUser(m));
            //    });
            //}

            return list.OrderBy(o => o);
        }

        public static IQueryable<string> GetAvailbleMembers(int? projectid, params string[] ms)
        {
            var list = GetEmplyeeByLVL(SalesRequired.LVL, LeaderRequired.LVL);
            var manager = CH.DB.Projects.Where(p=>p.ID==projectid).Select(p=>p.Manager);
            var market= CH.DB.Projects.Where(p=>p.ID==projectid).Select(p=>p.Market);
            var product= CH.DB.Projects.Where(p=>p.ID==projectid).Select(p=>p.Product);
            list=list.Union(manager).Union(market).Union(product);
            //if (ms != null)
            //{
            //    ms.ToList().ForEach(m =>
            //    {
            //        if (!string.IsNullOrEmpty(m))
            //            list.Add(Membership.GetUser(m));
            //    });
            //}

            return list.OrderBy(o => o);
        }
        public static object GetCurrentProfile(string propertyName)
        {
            var user = Employee.CurrentUserName;
            ProfileBase objProfile = ProfileBase.Create(user);

            return objProfile.GetPropertyValue(propertyName);
        }

        public static object GetProfile(string propertyName, string username)
        {
            ProfileBase objProfile = ProfileBase.Create(username);

            return objProfile.GetPropertyValue(propertyName);
        }

        public static string GetRoleName(string name)
        {
            var role = GetRole(name);
            if (role != null)
                return role.Name;
            else
                return "未配置";
        }

        public static int GetCurrentRoleLevel()
        {

            if (Employee.CurrentRole != null && !string.IsNullOrEmpty(Employee.CurrentUserName))
                return Employee.CurrentRole.Level;
            else
                return -100;
        }

        public static bool EqualToDirector()
        {
            return GetCurrentRoleLevel() == DirectorRequired.LVL;
        }

        public static bool EqualToSales()
        {
            return GetCurrentRoleLevel() == SalesRequired.LVL;
        }

        public static bool EqualToLeader()
        {
            return GetCurrentRoleLevel() == LeaderRequired.LVL;
        }

        public static bool EqualToManager()
        {
            return GetCurrentRoleLevel() == ManagerRequired.LVL;
        }

        public static bool EqualToAdministrator()
        {
            return GetCurrentRoleLevel() == 99999;
        }

        public static bool EqualToProductInterface()
        {
            return GetCurrentRoleLevel() == ProductInterfaceRequired.LVL;
        }

        public static bool EqualToMarketInterface()
        {
            return GetCurrentRoleLevel() == MarketInterfaceRequired.LVL;
        }
        #region

        public static bool AsDirector()
        {
            return GetCurrentRoleLevel() >= DirectorRequired.LVL;
        }

        public static bool AsLeader()
        {
            return GetCurrentRoleLevel() >= LeaderRequired.LVL;
        }



        public static bool AsManager()
        {
            return GetCurrentRoleLevel() >= ManagerRequired.LVL;
        }
        public static bool AsProductInterface()
        {
            return GetCurrentRoleLevel() >= ProductInterfaceRequired.LVL;
        }
        public static bool AsMarketInterface()
        {
            return GetCurrentRoleLevel() >= MarketInterfaceRequired.LVL;
        }
        public static bool AsSales()
        {
            return GetCurrentRoleLevel() >= SalesRequired.LVL;
        }

        #endregion









        public static EmployeeRole GetPerson(string name)
        {
            var user = CH.DB.EmployeeRoles.Where(s => s.AccountName == name).FirstOrDefault();
            return user;
        }

        public static int GetRoleLevel(string name)
        {
            var role = GetRole(name);
            if (role != null)
                return GetRole(name).Level;
            else
                return -1;
        }

        public static Role GetRole(string name)
        {
            var roles = from e in CH.DB.EmployeeRoles
                        from r in CH.DB.Roles
                        where e.AccountName == name && e.RoleID == r.ID

                        select r;
            if (roles.FirstOrDefault() != null)
                return roles.FirstOrDefault();
            else
            {

                var roleid = GetProfile("RoleLevelID", name).ToString();
                int id;
                int.TryParse(roleid, out id);
                var role = CH.GetDataById<Role>(id);
                if (role != null)
                {
                    var emp = new EmployeeRole() { RoleID = role.ID, AccountName = name, CreatedDate = DateTime.Now };
                    CH.DB.Set<EmployeeRole>().Add(emp);
                    CH.DB.SaveChanges();
                }
                return role;
            }
        }

        static Role GetCurrentRole()
        {

            return GetRole(Employee.CurrentUserName);
        }



        public static bool IsEqualToCurrentUserName(string name)
        {
            var user = Employee.CurrentUserName;
            return user.ToLower() == name.ToLower() ? true : false;
        }
        //public static User GetUser(string nameoremail)
        //{
        //    var data = Membership.GetUser(nameoremail);
        //    if (data == null)
        //    {
        //        string name = Membership.GetUserNameByEmail(nameoremail);
        //        if (!string.IsNullOrEmpty(name))
        //            data = Membership.GetUser(name);
        //    }

        //    if (data == null)
        //        throw new Exception("输入的名字");
        //    else
        //    {
        //        var user = new User() { UserName = data.UserName, Email = data.Email, Contact = GetContact(data.UserName) };
        //        return user;
        //    }
        //}

        /// <summary>
        /// 通过Outlook取得
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static string GetContact(string Name)
        {
            return "";
        }
    }


    public class DataToJson<T> : JavaScriptConverter where T : class
    {
        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            T theSource = obj as T;
            var ps = theSource.GetType().GetProperties();
            var c = typeof(T).GetCustomAttributes(typeof(JsonIgnoreAttribute), false).FirstOrDefault();
            var oo = theSource.GetType().Attributes;
            Dictionary<string, object> OutputJson = new Dictionary<string, object>();

            foreach (var p in ps)
            {
                if (c != null)
                {
                    JsonIgnoreAttribute j = c as JsonIgnoreAttribute;
                    if (j != null && j.IgnoreProperties.Contains(p.Name))
                    {
                        continue;
                    }
                }
                OutputJson.Add(p.Name, p.GetValue(theSource, null));
            }
            return OutputJson;
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get { return new Type[] { typeof(T) }; }
        }

    }

    public class DataJsonResult<T> : JsonResult where T : class
    {
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            HttpResponseBase response = context.HttpContext.Response;
            if (!string.IsNullOrEmpty(this.ContentType))
            {
                response.ContentType = this.ContentType;
            }
            else
            {
                response.ContentType = "application/json";
            }
            if (this.ContentEncoding != null)
            {
                response.ContentEncoding = this.ContentEncoding;
            }
            if (this.Data != null)
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                serializer.RegisterConverters(new JavaScriptConverter[] 
                {   new DataToJson<Company>(),
                    new DataToJson<CompanyType>(),
                    new DataToJson<Lead>(),
                    new DataToJson<Area>(),
                    new DataToJson<Research>(),
                    new DataToJson<LeadCall>(),
                    new DataToJson<Project>(),
                    new DataToJson<LeadCallType>(),
                    new DataToJson<JosonCompany>(),
                    new DataToJson<CompanyRelationship>(),
                    new DataToJson<Member>()
                });
                string sJSON = serializer.Serialize(Data);
                response.Write(sJSON);
            }
        }
    }

    public class DownloadResult : ActionResult
    {

        public DownloadResult()
        {
        }

        public DownloadResult(string virtualPath)
        {
            this.VirtualPath = virtualPath;
        }

        public string VirtualPath
        {
            get;
            set;
        }

        public string FileDownloadName
        {
            get;
            set;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            string filePath = context.HttpContext.Server.MapPath(this.VirtualPath);
            var position = filePath.LastIndexOf(".");
            if (position >= 0)
            {
                string extession = filePath.Remove(0, position);
                if (!String.IsNullOrEmpty(FileDownloadName))
                {
                    context.HttpContext.Response.AddHeader("content-disposition", "attachment; filename=" + HttpUtility.UrlEncode(this.FileDownloadName, System.Text.Encoding.UTF8));
                    if (extession == ".doc")
                        context.HttpContext.Response.ContentType = "application/msword";
                    if (extession == ".xls")
                        context.HttpContext.Response.ContentType = "application/vnd.xls";
                    if (extession == ".pdf")
                        context.HttpContext.Response.ContentType = "application/pdf";
                    if (extession == ".jpg")
                        context.HttpContext.Response.ContentType = "image/jpeg";

                }
                context.HttpContext.Response.TransmitFile(filePath);
            }
        }
    }

    public class SR
    {

        public static string GobackToList { get { return "回到列表"; } }
        public static string Save { get { return "保存"; } }
        public static string Delete { get { return "删除"; } }
        public static string Details { get { return "详细"; } }
        public static string Edit { get { return "编辑"; } }
        public static string Create { get { return "创建"; } }
        public static string Required { get { return "此栏位不能为空"; } }
        public static string Form { get { return "表单"; } }
        public static string CannotDelete { get { return "此项已经被引用，无法直接删除，需要取消引用后再进行删除"; } }
        public static string CannotDownload { get { return "此文件不存在，不能继续下载"; } }
        public static string ErrorView { get { return @"~\views\shared\Error.cshtml"; } }
        public static string Confirm { get { return "确认"; } }

    }
    public static class EnumerableExtensions
    {
        public static string ToStringList(this IEnumerable<string> item)
        {
            var temp = string.Empty;
            foreach (var i in item)
            {
                if (string.IsNullOrEmpty(temp))
                {
                    temp = i;
                }
                else
                {
                    temp += ", " + i;
                }
            }
            return temp;
        }
    }

    public class Utl
    {
        public static bool DebugModel()
        {
            if (HttpContext.Current != null && HttpContext.Current.Items["DebugModel"] == null)
            {
                var settings = ConfigurationManager.AppSettings.GetValues("DebugModel");
                var debug = bool.Parse(settings.FirstOrDefault());
                HttpContext.Current.Items["DebugModel"] = debug;
            }

            return (bool)HttpContext.Current.Items["DebugModel"];

        }

        public static string DebugAccount()
        {
            if (HttpContext.Current != null && HttpContext.Current.Items["DebugAccount"] == null)
            {
                var settings = ConfigurationManager.AppSettings.GetValues("DebugAccount");

                HttpContext.Current.Items["DebugAccount"] = settings[0] as String;
            }

            return (string)HttpContext.Current.Items["DebugAccount"];

        }

        public static string GetSpecialSuperManager()
        {
            if (HttpContext.Current != null && HttpContext.Current.Items["SpecialSuperManager"] == null)
            {
                var settings = ConfigurationManager.AppSettings.GetValues("SpecialSuperManager");

                HttpContext.Current.Items["SpecialSuperManager"] = settings[0] as String;
            }
            return (string)HttpContext.Current.Items["SpecialSuperManager"];
        }


        public static string[] GetSpecialManagers()
        {
            if (HttpContext.Current != null && HttpContext.Current.Items["SpecialManagers"] == null)
            {
                var settings = ConfigurationManager.AppSettings.GetValues("SpecialManagers");

                HttpContext.Current.Items["SpecialManagers"] = settings[0] as String;
            }

            return HttpContext.Current.Items["SpecialManagers"].ToString().Split(',');
        }

        public static string[] GetAvaliableCompaniesRole()
        {
            if (HttpContext.Current != null && HttpContext.Current.Items["AvaliableCompaniesRole"] == null)
            {
                var settings = ConfigurationManager.AppSettings.GetValues("AvaliableCompaniesRole");

                HttpContext.Current.Items["AvaliableCompaniesRole"] = settings[0] as String;
            }

            return HttpContext.Current.Items["AvaliableCompaniesRole"].ToString().Split(',');
        }
        public static string GetTeam1Leader()
        {
            if (HttpContext.Current != null && HttpContext.Current.Items["team1leader"] == null)
            {
                var settings = ConfigurationManager.AppSettings.GetValues("team1leader");

                HttpContext.Current.Items["team1leader"] = settings[0] as String;
            }
            return (string)HttpContext.Current.Items["team1leader"];
        }
        public static string GetTeam2Leader()
        {
            if (HttpContext.Current != null && HttpContext.Current.Items["team2leader"] == null)
            {
                var settings = ConfigurationManager.AppSettings.GetValues("team2leader");

                HttpContext.Current.Items["team2leader"] = settings[0] as String;
            }
            return (string)HttpContext.Current.Items["team2leader"];
        }
        public static string GetTeam3Leader()
        {
            if (HttpContext.Current != null && HttpContext.Current.Items["team3leader"] == null)
            {
                var settings = ConfigurationManager.AppSettings.GetValues("team3leader");

                HttpContext.Current.Items["team3leader"] = settings[0] as String;
            }
            return (string)HttpContext.Current.Items["team3leader"];
        }
        public static string[] GetTeam1ReportManagers()
        {
            if (HttpContext.Current != null && HttpContext.Current.Items["team1Managers"] == null)
            {
                var settings = ConfigurationManager.AppSettings.GetValues("team1Managers");

                HttpContext.Current.Items["team1Managers"] = settings[0] as String;
            }
            return HttpContext.Current.Items["team1Managers"].ToString().Split(',');
        }
        public static string[] GetTeam2ReportManagers()
        {
            if (HttpContext.Current != null && HttpContext.Current.Items["team2Managers"] == null)
            {
                var settings = ConfigurationManager.AppSettings.GetValues("team2Managers");

                HttpContext.Current.Items["team2Managers"] = settings[0] as String;
            }
            return HttpContext.Current.Items["team2Managers"].ToString().Split(',');
        }
        public static string[] GetTeam3ReportManagers()
        {
            if (HttpContext.Current != null && HttpContext.Current.Items["team3Managers"] == null)
            {
                var settings = ConfigurationManager.AppSettings.GetValues("team3Managers");

                HttpContext.Current.Items["team3Managers"] = settings[0] as String;
            }
            return HttpContext.Current.Items["team3Managers"].ToString().Split(',');
        }
        public static decimal GetAverage(decimal? child, int father)
        {
            if (father == 0) return 0;
            return Math.Round(((decimal)child / (decimal)father), 2);
        }

        public static double GetPercent(double child, double father)
        {
            if (father == 0) return 0;
            return Math.Round((child / father), 2);
        }

        public static double GetPercent(int child, int father)
        {
            if (father == 0 || child == 0) return 0;
            return Math.Round(((double)child / (double)father), 2);
        }

        public static double GetPercent(decimal? child, decimal? father)
        {
            if (father == 0 || child == 0 || father == null || child == null) return 0;
            return (double)Math.Round((child.Value / father.Value), 2);
        }

        public static void GetMonthActualStartdateAndEnddate(int? month, out DateTime startdate, out DateTime enddate)
        {
            if (month == null) month = DateTime.Now.Month;

            var selectedmonth = new DateTime(DateTime.Now.Year, month.Value, 1);
            startdate = selectedmonth.StartOfMonth();
            enddate = selectedmonth.EndOfMonth();

            //如果是周末。往后加1天 循环到周1为止
            while (startdate.DayOfWeek == DayOfWeek.Saturday || startdate.DayOfWeek == DayOfWeek.Sunday)
            {
                startdate = startdate.AddDays(1);
            }
            //工作日跨月的周，计入本月
            while (startdate.DayOfWeek != DayOfWeek.Monday)
            {
                startdate = startdate.AddDays(-1);
            }

            while (enddate.DayOfWeek == DayOfWeek.Saturday || enddate.DayOfWeek == DayOfWeek.Friday)
            {
                enddate = enddate.AddDays(1);
            }

            while (enddate.DayOfWeek != DayOfWeek.Sunday)
            {
                enddate = enddate.AddDays(-1);
            }

            // enddate = startdate.AddDays(28);
        }

        public static string ConvertSelectProjectIDtoString(List<int> selectedprojects)
        {
            var s = string.Empty;
            if (selectedprojects != null)
            {
                foreach (var p in selectedprojects)
                {
                    s = s + p.ToString() + "|";
                }
            }
            return s;
        }
        public static List<int> ConvertStringToSelectProjectID(string s)
        {
            List<int> pids = new List<int>();
            if (s != null)
            {
                var ps = s.Split('|');
                foreach (var p in ps)
                {
                    if (!string.IsNullOrEmpty(p))
                    {
                        pids.Add(Int32.Parse(p));
                    }
                }
            }
            return pids;
        }

        string plsstring = string.Empty;
        public static IEnumerable<DataRow> GetCallsInfoForPerformanceDataRows(DateTime startdate, DateTime enddate, List<string> members)
        {

            IEnumerable<DataRow> phones;


            string cstr = ConfigurationManager.ConnectionStrings["BillDB"].ToString();
            using (var con = new OleDbConnection(cstr))
            {
                string sql = "SELECT * FROM bill ";

                //string sql = "SELECT * FROM bill";
                OleDbDataAdapter da = new OleDbDataAdapter(sql, con);
                OleDbCommand c = new OleDbCommand(sql, con);
                da.SelectCommand = c;
                DataSet ds = new DataSet();//创建数据集
                da.Fill(ds, "bill");//填充数据集
                DataTable tb = ds.Tables["bill"];//创建表
                var rows = ds.Tables["bill"].Select();
                phones = rows.Where(p => p["duration"].ToString() != "00:00:00");

                con.Close();
            }
            return phones;
        }

        public static List<ViewPhoneInfo> GetCallsInfo(List<Project> ps, DateTime? startdate, DateTime? enddate)
        {
            if (startdate == null)
            {
                startdate = new DateTime(1, 1, 1);
            }
            if (enddate == null)
            {
                enddate = new DateTime(9999, 1, 1);
            }

            var phonelist = new List<ViewPhoneInfo>();
            if (ps.Count > 0)
            {

                string cstr = ConfigurationManager.ConnectionStrings["BillDB"].ToString();
                using (var con = new OleDbConnection(cstr))
                {
                    //string sql = "SELECT * FROM bill " + contacts;
                    string sql = "SELECT * FROM bill ";
                    //string sql = "SELECT * FROM bill";
                    OleDbDataAdapter da = new OleDbDataAdapter(sql, con);
                    OleDbCommand c = new OleDbCommand(sql, con);
                    da.SelectCommand = c;
                    DataSet ds = new DataSet();//创建数据集
                    da.Fill(ds, "bill");//填充数据集
                    DataTable tb = ds.Tables["bill"];//创建表
                    var rows = ds.Tables["bill"].Select();
                    var phones = rows.Where(p => p["duration"].ToString() != "00:00:00").GroupBy(r => r["phone"]);
                    foreach (var p in phones)
                    {

                        var total = p.Sum(s => (TimeSpan.Parse(s["duration"].ToString())).TotalMinutes);
                        var t = new ViewPhoneInfo() { Phone = p.Key.ToString(), Duration = TimeSpan.FromMinutes(total) };
                        phonelist.Add(t);
                    }
                    con.Close();
                }
            }
            return phonelist;
        }

        public static string HidePhoneNumber(string origin)
        {
            var m = origin;
            if (string.IsNullOrWhiteSpace(m)) return string.Empty;
            string start = string.Empty;
            if (m.Length > 3)
            {
                var hide = m.Substring(3, m.Length - 3);
                var hidecount = hide.Count();

                for (int i = 0; i < hidecount; i++)
                {
                    start += "*";
                }

                return m.Substring(0, 3) + start;
            }
            return m;
        }


        public static string FilterStr(string source)
        {
            source = source.Replace("& ", "& ");
            source = source.Replace(" < ", "< ");
            source = source.Replace("> ", "> ");
            source = source.Replace(" ' ", " ' ' "); source = source.Replace("\n ", " <br/> ");
            source = source.Replace("\r\n ", " <br/> ");
            return source;
        }

        public static string GetFullString(string seperator, params string[] datas)
        {
            var s = new List<string>();
            foreach (var d in datas)
            {
                if (!string.IsNullOrEmpty(d))
                {
                    s.Add(d);
                }
            }
            return string.Join(seperator, s);
        }


        public static string GetFullName(string ch, string en)
        {
            if (string.IsNullOrEmpty(ch) && !string.IsNullOrEmpty(en))
            {
                return en;
            }

            if (!string.IsNullOrEmpty(ch) && string.IsNullOrEmpty(en))
            {
                return ch;
            }

            if (!string.IsNullOrEmpty(ch) && !string.IsNullOrEmpty(en))
            {
                return en + " | " + ch;
            }

            return string.Empty;
        }


        public static string ShortText(string str, int length)
        {
            if (str != null && str.Length > length)
                return str.Remove(length, str.Length - length) + "...";
            else
                return str;
        }

        public static bool IsToday(DateTime dt)
        {
            DateTime today = DateTime.Today;
            DateTime tempToday = new DateTime(dt.Year, dt.Month, dt.Day);
            if (today == tempToday)
                return true;
            else
                return false;
        }

        public static void SyncUser()
        {
            foreach (MembershipUser item in Membership.GetAllUsers())
            {
                if (CH.DB.EmployeeRoles.Where(s => s.AccountName == item.UserName).Any())
                {
                    ProfileBase objProfile = ProfileBase.Create(item.UserName);
                    EmployeeRole selUser = CH.DB.EmployeeRoles.Where(s => s.AccountName == item.UserName).First();

                    selUser.IsActivated = (bool)objProfile.GetPropertyValue("IsActivated");
                    if (string.IsNullOrEmpty(selUser.Gender))
                    {
                        selUser.Gender = objProfile.GetPropertyValue("Gender") as string;
                    }
                    if (string.IsNullOrEmpty(selUser.AccountNameCN) || selUser.AccountNameCN=="-")
                    {
                        var v = objProfile.GetPropertyValue("DisplayName") as string;
                        if (!string.IsNullOrEmpty(v))
                        selUser.AccountNameCN = objProfile.GetPropertyValue("DisplayName") as string;
                    }
                    if (string.IsNullOrEmpty(selUser.Mobile))
                    {
                        selUser.Mobile = objProfile.GetPropertyValue("Mobile") as string;
                    }
                    if (selUser.AgentNum ==null)
                    {
                        selUser.AgentNum = objProfile.GetPropertyValue("Contact") as int?;
                    }
                    if (selUser.DepartmentID == null)
                    {
                        selUser.DepartmentID = objProfile.GetPropertyValue("DepartmentID") as int?;
                    }
                    var sdstring = objProfile.GetPropertyValue("StartDate") as string;
                  
                    selUser.BirthDay = objProfile.GetPropertyValue("BirthDay") as DateTime?;
                    DateTime sddate;
                    DateTime.TryParse(sdstring, out sddate);
                    if (sddate != DateTime.MinValue && sddate!=null)
                    {
                        selUser.StartDate = sddate;
                    }
                    if (selUser.BirthDay == DateTime.MinValue)
                    {
                        selUser.BirthDay = null;
                    }
                    if (string.IsNullOrWhiteSpace(selUser.AccountNameCN))
                    {
                        selUser.AccountNameCN = "-";
                    }
                    CH.Edit<EmployeeRole>(selUser);
                }
            }
        }


    }

    public static class DateTimeExtensions
    {
        public static string ToShortMonthString(this DateTime dt)
        {

            return dt.Month.ToString() + "-" + dt.Day.ToString(); ;
        }

        public static DateTime StartOfWeek(this DateTime dt)
        {
            int diff = dt.DayOfWeek - DayOfWeek.Monday; if (diff < 0) { diff += 7; }
            return dt.AddDays(-1 * diff).Date;
        }

        public static DateTime EndOfWeek(this DateTime dt)
        {
            int diff = DayOfWeek.Saturday - dt.DayOfWeek;
            if (diff < 0)
            { diff += 7; }
            var re = dt.AddDays(1 * diff).AddHours(23).Date;
            return re;
        }

        public static DateTime StartOfMonth(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1);
        }

        public static bool IsWorkingday(this DateTime dt)
        {
            if (dt.DayOfWeek != DayOfWeek.Sunday && dt.DayOfWeek != DayOfWeek.Saturday)
                return true;
            else
                return false;
        }

        public static DateTime EndOfMonth(this DateTime dt)
        {
            DateTime firstDayOfTheMonth = new DateTime(dt.Year, dt.Month, 1);
            return firstDayOfTheMonth.AddMonths(1).AddDays(-1);
        }


    }
}
