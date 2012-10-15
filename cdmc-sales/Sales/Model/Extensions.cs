using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Utl;

namespace Entity
{
    public static class EntityBaseExtensions
    {
        public static bool SameFieldExist<T>(this EntityBase item,string fieldname) where T: EntityBase
        {
            var value = item.GetType().GetProperty(fieldname).GetValue(item,null);
            var data = CH.GetAllData<T>(child=>child.GetType().GetProperty(fieldname).GetValue(child,null).ToString()==value.ToString());
            if (data.Count > 0) return true;
            return false;
        }
    }

    
    public static class CompanyRelationshipExtensions
    {
        /// <summary>
        /// 查看当前谁在给这家公司打电话 
        /// </summary>
        /// <param name="companyid"></param>
        /// <param name="projectid"></param>
        /// <returns></returns>
        public static List<Member> WhoCallTheCompanyMember(this CompanyRelationship item)
        { 
             var ms = CH.GetAllData<Member>(c => c.ID == item.ID);
            List<Member> result = new List<Member>();
            result.AddRange(ms);


            //如果公司上没有直接指定，按字头分配查找
            if (result.Count == 0)
            {
                
                var project = CH.GetAllData<Project>(p => p.ID == item.ProjectID, "Members").FirstOrDefault();

                project.Members.ForEach(m=>{
                    if (!string.IsNullOrEmpty(m.Characters))
                    {
                        var chars = m.Characters.Split('|').ToList();
                        chars.ForEach(ch =>
                        {
                            if ((!string.IsNullOrEmpty(item.Company.Name_CH)&&item.Company.Name_CH.StartsWith(ch)) ||
                                (!string.IsNullOrEmpty(item.Company.Name_EN) && item.Company.Name_EN.StartsWith(ch.ToUpper())))
                            {
                                result.Add(m);
                            }
                        });
                    }
                   
                });
            }

            return result;
        }

        public static string WhoCallTheCompanyMemberName(this CompanyRelationship item)
        {
            var ml = item.WhoCallTheCompanyMember();

            string ms = string.Empty;

            ml.ForEach(m =>
            {
                if (ms == string.Empty)
                    ms += m.Name;
                else
                    ms += "|" + m.Name;
            });

            return ms;
        }

        public static string CategoryString(this CompanyRelationship item)
        {
            //重读，确保读到refernced的category
            var cs = CH.GetDataById<CompanyRelationship>(item.ID,"Categorys");
            string result = string.Empty;
            cs.Categorys.ForEach(c => {
                if (string.IsNullOrEmpty(result))
                    result = c.Name;
                else
                    result += "|" + c.Name;
            });

            return result;
        }
    }

     

     
 
}