//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Data.Entity;
//using System.Data;
//using System.Collections;
//using System.Reflection;
//using System.Data.Entity.Infrastructure;
//using Entity;

//namespace Utilities
//{
//    /// <summary>
//    /// •Detach the first instance, then attach the second
// ///•Copy the fields from the second instance to the first

//    /// </summary>
//     public interface IDBProvider
//    {
//         DB GetDB();
//    }

//    public class CH
//    {
//        public void Detach(object entity)
//        {
//            var objectContext = ((IObjectContextAdapter)this).ObjectContext;
//            objectContext.Detach(entity);
//        }

//        public static IDBProvider DBProvider { get; set; }

//        static DB _DB;
//        public static DB DB
//        {
//            get
//            {
//                if (_DB == null)
//                    return new DB();
//                else
//                    return _DB;
//            }
//            set { _DB = value; }
            
//        }

//        static DbSet<T> GetAllRowData<T>()
//            where T : class
//        {
//            var re = DB.Set<T>();
//            return re;
//        }

//        public static List<T> GetAllData<T>(params string[] property)
//           where T : class
//        {
//            return GetAllData<T>(null, property);
//        }


//        public static List<T> GetAllDataNoTrack<T>(params string[] property)
//          where T : class
//        {
//            return GetAllDataNoTrack<T>(null, property);

//        }

//        public static List<T> GetAllDataNoTrack<T>(Func<T, bool> filters, params string[] property) where T : class
//        {

//            var db = GetAllRowData<T>().AsNoTracking();
//            if (property.Count() == 0)
//            {
//                if (filters != null)
//                    return db.Where(filters).ToList();
//                else
//                    return db.ToList();
//            }

//            int i = 0;
//            var tt = db.Include(property[0]);

//            while (property.Count() - 1 > i)
//            {
//                i++;
//                tt = tt.Include(property[i]);
//            }
//            if (filters != null)
//                return tt.Where(filters).ToList();
//            else
//                return tt.ToList();
//        }

//        public static List<T> GetAllData<T>(Func<T, bool> filters, params string[] property)
//            where T : class
//        {
//            DbSet<T> db = GetAllRowData<T>();

//            if (property.Count() == 0)
//            {
//                if (filters != null)
//                    return db.Where(filters).ToList();
//                else
//                    return db.ToList();
//            }

//            int i = 0;
//            var tt = db.Include(property[0]);

//            while (property.Count() - 1 > i)
//            {
//                i++;
//                tt = tt.Include(property[i]);
//            }
//            if (filters != null)
//                return tt.Where(filters).ToList();
//            else
//                return tt.ToList();
//        }

//        public static T GetDataById<T>(int? id)
//            where T : class
//        {
//            return GetAllRowData<T>().Find(id);
//        }

//        public static int Create<T>(T entity)
//            where T : EntityBase
//        {
//            SetTimeAndUser(entity);
//            DB.Set<T>().Add(entity);
//            var re = DB.SaveChanges();
//            return re;
//        }

//        public static int GetID(object o)
//        {
//            return (int)o.GetType().GetProperty("ID").GetValue(o, null);
//        }

//        public static int EditCollection<T>(List<T> origins, List<T> currents)
//             where T : EntityBase
//        {
//            //make sure not to throw null exception
//            if (currents == null) currents = new List<T>();
//            if (origins == null) origins = new List<T>();

//            //edit & add
//            foreach (T current in currents)
//            {

//                if (current.ID > 0)
//                {
//                    var origin = origins.Find(i => i.ID == current.ID);
//                    DB.Entry(origin).CurrentValues.SetValues(current);
//                }
//                else
//                {
//                    DB.Entry(current).State = EntityState.Added;
//                }
//            }

//            //delete
//            var toDeletes = origins.FindAll(o => currents.FirstOrDefault(c => c.ID == o.ID) == null);

//            foreach (var d in toDeletes)
//            {
//                var delete = DB.Set<T>().Find(d.ID);
//                DB.Set<T>().Remove(delete);
//            }

//            var re = DB.SaveChanges();
//            return re;

//        }
       
//        public static int Edit<T>(T entity)
//            where T : EntityBase
//        {
//            DB.Entry(entity).State = EntityState.Modified;
//            SetTimeAndUser(entity);
//            var re = DB.SaveChanges();
//            return re;
//        }

//         public static void SetTimeAndUser<T>(T entity)
//            where T : EntityBase
//        {
//            entity.ModifiedTime = DateTime.Now;
//            //entity.User = HttpContext.Current.User.Identity.Name;
//        }

//        public static int Delete<T>(int? id)
//        where T : class
//        {
//            T record = DB.Set<T>().Find(id);
//            DB.Set<T>().Remove(record);
//            return DB.SaveChanges();
//        }

//        internal static int DeleteRange<T>(List<T> list) where T : class
//        {
//            foreach (var e in list)
//            {
//                DB.Set<T>().Remove(e);
//            }
//            return DB.SaveChanges();
//        }
//    }
//}