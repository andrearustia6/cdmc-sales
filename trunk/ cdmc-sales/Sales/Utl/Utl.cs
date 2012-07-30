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

namespace Utl
{
    public class CT
    {
        //public static GridModel GetViewModel<M, V>(Func<M, bool> filters, params string[] properties)
        //    where M : EntityBase
        //    where V : ViewBase
        //{
        //    var data = CH.GetAllData<M>(filters, properties);
        //    var list = new List<V>();
        //    data.ForEach(d =>
        //    {
        //        V view = (V)Activator.CreateInstance(typeof(V), d);
        //        list.Add(view);
        //    });
        //    return new GridModel(list);
        //}

        //public static GridModel GetViewModel<M, V>(params string[] properties)
        //    where M : EntityBase
        //    where V : ViewBase
        //{
        //    return GetViewModel<M, V>(null, properties);
        //}

        public static GridModel GetViewModel<M>(Func<M, bool> filters, params string[] properties)
            where M : EntityBase
        {
            var data = CH.GetAllData<M>(filters, properties);
            return new GridModel(data);
        }

        public static GridModel GetViewModel<M>(params string[] properties)
            where M : EntityBase
        {
            return GetViewModel<M>(null, properties);
        }
    }
    public static class AppConfig
    {
        public static string ConnectionStringSetting
        {
            get { return ConfigurationManager.AppSettings["ConnectionStringSetting"]; }
        }
    }
    public class JsonIgnoreAttribute : Attribute
    {
        public List<string> IgnoreProperties { get; set; }
        public JsonIgnoreAttribute(params string[] properties)
        {
            IgnoreProperties = new List<string>();
            IgnoreProperties.AddRange(properties);
        }
    }

    public class DataToJson<T> : JavaScriptConverter where T : class
    {
        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            T theSource = obj as T;
            var ps = theSource.GetType().GetProperties();
            var c = theSource.GetType().GetCustomAttributes(typeof(JsonIgnoreAttribute), true).FirstOrDefault();

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
                    new DataToJson<Client>()
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
    }
}
