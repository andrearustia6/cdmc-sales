using System;
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
    public enum ReviewRight { 
        ProjectInfoReview, 
        CallsReview,
        CallsSumReview, 
        DealsReview,
        CallAnalysisReview, 
        ProgressReview, 
        EvaluationsReview,
        ProtectedCompanyReview,
        TargetsView,
        AvailableCompaniesReview }

    public enum EditRight { ProjectInfoEdit, MembersEdit, DealsEdit,  EvaluationsEdit, TargetsEdit, AvailableCompaniesEdit }
   
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
       
            var mode = ConfigurationManager.AppSettings["DebugModel"].ToString(); 
            var user = HttpContext.Current.User.Identity.Name;
            
            if (mode == "true" && Employee.GetRole(user).Name == "系统管理员")
            {
                var name = ConfigurationManager.AppSettings["DebugAccount"].ToString();
                return name;
            }
            else
            return HttpContext.Current.User.Identity.Name;
        }

        public static UserInfoModel GetUserByName(string username)
        {

            ProfileBase objProfile = ProfileBase.Create(username);
            var email = Membership.GetUser(username).Email;
            var roleid = (int)objProfile.GetPropertyValue("RoleLevelID");
            var gender = objProfile.GetPropertyValue("Gender") as string;
            var displayName = objProfile.GetPropertyValue("DisplayName") as string;
            var mobile = objProfile.GetPropertyValue("Mobile") as string;
            int con = 0;
            Int32.TryParse(objProfile.GetPropertyValue("Contact").ToString(),out con);
            var contact = con;
            var departmentid = objProfile.GetPropertyValue("DepartmentID") as int?;
            var startDate = objProfile.GetPropertyValue("StartDate") as string;
            DateTime date;
            DateTime.TryParse(startDate,out date);
            var userinfo = new UserInfoModel()
            {
                RoleID = roleid,
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

        public static List<MembershipUser> GetAllEmployees()
        {
            var list = Membership.GetAllUsers().Cast<MembershipUser>().ToList<MembershipUser>();

            return list;
        }

        /// <summary>
        /// 取得所有 基本的对象
        /// </summary>
        /// <returns></returns>
        public static List<MembershipUser> GetEmplyeeByLVL(params int[] lvl)
        {
            var list = Membership.GetAllUsers().Cast<MembershipUser>().ToList<MembershipUser>();
            var data = new List<MembershipUser>();
            list.ForEach(l => {
                if (lvl.Contains(GetRoleLevel(l.UserName)))
                    data.Add(l);
            });
            return data;
        }

        /// <summary>
        /// 添加成员
        /// </summary>
        /// <returns></returns>
        public static List<MembershipUser> GetAvailbleSales(int? projectid,params string[] ms)
        {
            var list = GetEmplyeeByLVL(SalesRequired.LVL, LeaderRequired.LVL);
            var p = CH.GetDataById<Project>(projectid,"Members");
            list = list.FindAll(l => p.Members.Exists(m => m.Name == l.UserName) == false);

            if (ms != null)
            {
                ms.ToList().ForEach(m => {
                    if (!string.IsNullOrEmpty(m))
                    list.Add(Membership.GetUser(m));
                });
            }
           
            return list;
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

            if (!string.IsNullOrEmpty(Employee.CurrentUserName))
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
            var roleid = GetProfile("RoleLevelID", name).ToString();
            int id;
            int.TryParse(roleid, out id);
            var role = CH.GetDataById<Role>(id);
            return role;
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

        

        public static void GetMonthActualStartdateAndEnddate(int? month, out DateTime startdate, out DateTime enddate)
        {
            if (month == null) month = DateTime.Now.Month;

            startdate = DateTime.Now.StartOfMonth();
            enddate = DateTime.Now.EndOfMonth();

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
        public static IEnumerable<DataRow> GetCallsInfoForPerformanceDataRows( DateTime startdate, DateTime enddate, List<string> members)
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
                    var phones = rows.Where(p=>p["duration"].ToString()!= "00:00:00").GroupBy(r => r["phone"]);
                    foreach (var p in phones)
                    {

                         var total =  p.Sum(s => (TimeSpan.Parse(s["duration"].ToString())).TotalMinutes);
                        var t = new ViewPhoneInfo() { Phone = p.Key.ToString(), Duration = TimeSpan.FromMinutes(total) };
                        phonelist.Add(t);
                    }
                    con.Close();
                }
            }
            return phonelist;
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
            if (str!=null && str.Length > length)
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


    }

    public static class DateTimeExtensions
    {
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
            var re= dt.AddDays(1 * diff).AddHours(23).Date;
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
