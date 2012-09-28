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

namespace Utl
{
    [TestAttribute]
    public class test
    {
    }
    public static class AppConfig
    {
        public static string ConnectionStringSetting
        {
            get { return ConfigurationManager.AppSettings["ConnectionStringSetting"]; }
        }
    }
    public class TestAttribute : Attribute
    {
        public List<string> IgnoreProperties { get; set; }
        public TestAttribute(params string[] properties)
        {
            IgnoreProperties = new List<string>();
            IgnoreProperties.AddRange(properties);
        }
    }

    public class Employee
    {
        public static List<MembershipUser> GetAllEmployees()
        {
            var list = Membership.GetAllUsers().Cast<MembershipUser>().ToList<MembershipUser>();

            return list;
        }
        public static object GetCurrentProfile(string propertyName)
        {
            var user = HttpContext.Current.User.Identity.Name;
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
            var name = HttpContext.Current.User.Identity.Name;
            if (!string.IsNullOrEmpty(name))
                return GetRole(HttpContext.Current.User.Identity.Name).Level;
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
            return GetCurrentRoleLevel() >= ManagerRequired.LVL;
        }

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

        public static bool EqualToProductInterface()
        {
            return GetCurrentRoleLevel() == ProductInterfaceRequired.LVL;
        }

        public static bool AsMarketInterface()
        {
            return GetCurrentRoleLevel() >= MarketInterfaceRequired.LVL;
        }

        public static bool EqualToMarketInterface()
        {
            return GetCurrentRoleLevel() == MarketInterfaceRequired.LVL;
        }

        public static bool AsSales()
        {
            return GetCurrentRoleLevel() >= SalesRequired.LVL;
        }

        

        public static int GetRoleLevel(string name)
        {
            return GetRole(name).Level;
        }

        public static Role GetRole(string name)
        {
            var roleid = GetProfile("RoleLevelID", name).ToString();
            int id;
            int.TryParse(roleid, out id);
            var role = CH.GetDataById<Role>(id);
            return role;
        }

        public static Role GetCurrentRole()
        {
            return GetRole(HttpContext.Current.User.Identity.Name);
        }



        public static bool IsEqualToCurrentUserName(string name)
        {
            var user = HttpContext.Current.User.Identity.Name;
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
                    new DataToJson<Category>(),
                    new DataToJson<Research>(),
                    new DataToJson<LeadCall>(),
                    new DataToJson<Project>(),
                    new DataToJson<LeadCallType>()
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
            string extession = filePath.Remove(0, filePath.LastIndexOf("."));
            if (!String.IsNullOrEmpty(FileDownloadName))
            {
                context.HttpContext.Response.AddHeader("content-disposition", "attachment; filename=" + HttpUtility.UrlEncode(this.FileDownloadName));
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
    }

    public class Utl
    {
        public static string FilterStr(string source)
        {
            source = source.Replace("& ", "& ");
            source = source.Replace(" < ", "< ");
            source = source.Replace("> ", "> ");
            source = source.Replace(" ' ", " ' ' "); source = source.Replace("\n ", " <br/> ");
            source = source.Replace("\r\n ", " <br/> ");
            return source;
        }

        public static TimeSpan GetAvailableTimeSpan(int district)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("~/TimeSpan.xml");


            return new TimeSpan();
        }


        //public static string GetImageUrls(List<Image> imgs)
        //{
        //    ArrayList al = new ArrayList();
        //    imgs.ForEach(i =>
        //    {
        //        al.Add(new string[] { i.ImageUrl, "", "", i.Description });
        //    });
        //    JavaScriptSerializer serializer = new JavaScriptSerializer();
        //    string result = serializer.Serialize(al);
        //    return result;
        //}

        public static string ShortTime(DateTime time)
        {
            return time.ToShortDateString();
        }


        public static string ShortText(string str, int length)
        {
            if (str.Length > length)
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

    public class DisplayCurrency
    {
        public static DisplayCurrency Instance(string name, decimal value, string symbole)
        {
            return new DisplayCurrency() { Name = name, Symbol = symbole, Value = value };
        }
        public static DisplayCurrency Dollars(EntityBase e, string Property)
        {
            var name = e.GetType().GetProperty("ID").GetValue(e, null).ToString();
            var value = (decimal)e.GetType().GetProperty(Property).GetValue(e, null);
            return new DisplayCurrency() { Name = name + Property, Symbol = "$", Value = value };
        }
        public string Symbol { get; set; }
        public decimal Value { get; set; }
        public string Name { get; set; }
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

        public static DateTime EndOfMonth(this DateTime dt)
        {
            DateTime firstDayOfTheMonth = new DateTime(dt.Year, dt.Month, 1);
            return firstDayOfTheMonth.AddMonths(1).AddDays(-1);
        }
    }
}
