using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data;
using System.Collections;
using System.Reflection;
using Entity;
using Utilities;
using System.Data.SqlClient;
using System.Data.EntityClient;
using System.Configuration;

namespace Utl
{
    public class MappingRecords
    {
        public Type DbContextType { get; set; }
        public string primaryKeyName { get; set; }
    }
    public class CH
    {
        public static DB TestDB { get; set; }
        public static DB DB
        {
            get
            {
                if (HttpContext.Current != null && HttpContext.Current.Items["DB"] == null)
                {
                    HttpContext.Current.Items["DB"] = new DB();
                }
                if (HttpContext.Current == null)//为测试情况
                {
                    var entityConnectionStringBuilder = new EntityConnectionStringBuilder();

                    //entityConnectionStringBuilder.Provider = "System.Data.SqlClient";
                    //entityConnectionStringBuilder.ConnectionString = "Server=192.168.93.143;Initial Catalog=processfix;User ID=process;password=cdncsqld3j7;Max Pool Size=512;MultipleActiveResultSets=true;";
                    //entityConnectionStringBuilder.Metadata = @"res://*/processfix.csdl|res://*/processfix.ssdl|res://*/processfix.msl";
                    if (TestDB == null)
                    {
                        var db = new DB("Server=192.168.93.143;Initial Catalog=processfix;User ID=process;password=cdncsqld3j7;Max Pool Size=512;MultipleActiveResultSets=true;");
                        TestDB = db;
                        return TestDB;
                    }
                    return TestDB;
                  
                }
                return HttpContext.Current.Items["DB"] as DB;
            }
            set
            {
                if (HttpContext.Current != null)
                    HttpContext.Current.Items["DB"] = value;
            }
        }

        static DbSet<T> GetAllRowData<T>()
            where T : class
        {
            var re = DB.Set<T>();
            return re;
        }

        public static List<T> GetAllData<T>(params string[] property)
           where T : class
        {
            return GetAllData<T>(null, property);
        }

        public static List<T> GetAllData<T>(Func<T, bool> filters, params string[] property)
            where T : class
        {

            DbSet<T> db = GetAllRowData<T>();

            if (property.Count() == 0)
            {
                if (filters != null)
                    return db.Where(filters).ToList();
                else
                    return db.ToList();
            }

            int i = 0;
            var tt = db.Include(property[0]);

            while (property.Count() - 1 > i)
            {
                i++;
                tt = tt.Include(property[i]);
            }
            if (filters != null)
                return tt.Where(filters).ToList();
            else
                return tt.ToList();
        }

        public static T GetDataById<T>(int? id)
            where T : class
        {
            return CH.DB.Set<T>().Find(id);
            //return GetAllRowData<T>().Find(id);
        }

        public static T GetDataById<T>(int? id, params string[] property)
            where T : class
        {
            return GetAllData<T>(item => (item as EntityBase).ID == id, property).FirstOrDefault();
            //return GetAllRowData<T>().Find(id);
        }

        static void AddCreatedStamp(object entity)
        {
            if (entity is EntityBase)
            {
                var eb = entity as EntityBase;
                eb.CreatedDate = DateTime.Now;
                eb.Creator = Employee.CurrentUserName;
            }
        }

         static void AddModifiedStamp(object entity)
        {
            if (entity is EntityBase)
            {
                var eb = entity as EntityBase;
                eb.ModifiedDate = DateTime.Now;
                eb.ModifiedUser = Employee.CurrentUserName;
            }
        }

        public static int Create<T>(T entity)
            where T : class
        {
            AddCreatedStamp(entity);
            DB.Set<T>().Add(entity);
            var re = DB.SaveChanges();
            return re;
        }

        public static int GetID(object o)
        {
            return (int)o.GetType().GetProperty("ID").GetValue(o, null);
        }

        public static int Edit<T>(T entity, params string[] properties)
             where T : class
        {
            AddModifiedStamp(entity);
            foreach (var p in properties)
            {
                string propertyname = string.Empty;
                List<string> childpropertynames = new List<string>();
                PropertyInfo property = null;
                if (!p.Contains('.'))
                    property = entity.GetType().GetProperty(p);
                else
                {
                    childpropertynames.AddRange(p.Split('.'));
                    childpropertynames.RemoveAt(0);
                }
                var pv = property.GetValue(entity, null);
                List<int> IDS = new List<int>();// 保存新数据的id
                var pvs = pv as IEnumerable;
                if (pvs != null)
                {
                    DbSet origindata = null;
                    foreach (var item in pvs)
                    {
                        if (origindata == null)
                            origindata = DB.Set(item.GetType());

                        var id = GetID(item);
                        if (id == 0)//新数据
                        {
                           
                            var result = DB.Set(item.GetType()).Add(item);

                            IDS.Add(GetID(item));
                        }
                        else//老数据更新
                        {
                            DB.Entry(item).State = EntityState.Modified;
                            IDS.Add(GetID(item));
                        }
                    }

                    var oq = origindata.AsQueryable();
                    foreach (var item in oq)
                    {
                        var id = GetID(item);
                        var result = IDS.Find(i => i == id);
                        if (result == 0)
                        {
                            origindata.Remove(item);
                        }
                    }

                    DB.SaveChanges();
                }
                else
                {
                    DB.Entry(pv).State = EntityState.Modified;
                }
                DB.SaveChanges();//保存以产生id
            }
            var c = DB.Entry(entity);
            c.State = EntityState.Modified;

            var re = DB.SaveChanges();
            return re;
        }

        public static int EditRange<T>(List<T> origins, List<T> changes) where T : class
        {
            foreach (var e in changes)
            {
                var id = GetID(e);
                if (id > 0)
                    DB.Entry<T>(e).State = EntityState.Modified;
                else
                    DB.Set<T>().Add(e);
            }

            if (origins != null)
            {
                var toremove = origins.FindAll(o => changes.FirstOrDefault(c => GetID(c) == GetID(o)) == null);

                foreach (var r in toremove)
                {
                    DB.Set<T>().Remove(DB.Set<T>().Find(GetID(r)));
                }
            }
            var re = DB.SaveChanges();
            return re;
        }

        public static int Delete<T>(int? id)
        where T : class
        {
            T record = DB.Set<T>().Find(id);
            DB.Set<T>().Remove(record);
            return DB.SaveChanges();
        }

        internal static int DeleteRange<T>(List<T> list) where T : class
        {
            foreach (var e in list)
            {
                DB.Set<T>().Remove(e);
            }
            return DB.SaveChanges();
        }
    }
}