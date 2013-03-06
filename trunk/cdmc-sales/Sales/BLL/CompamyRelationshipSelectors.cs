using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entity;
using System.Web.Mvc;
using Utl;
using BLL;

namespace Sales.BLL
{
     public class  SelectListHelper
     {
         public static SelectList GetLocation()
         {
             return new SelectList(new List<string>() { "国外", "国内" }, "-请选择-");
         }

         public static SelectList GetProject(string accessright)
         {
             var ps = CRM_Logical.GetUserProjectRight(accessright);
             return new SelectList(ps, "ID", "Name_CH", "-请选择-");
         }

         /// <summary>
         /// 打过的lead数
         /// </summary>
         /// <param name="accessright"></param>
         /// <returns></returns>
         public static SelectList GetCalledLead(string accessright)
         {
              return new SelectList(new List<int>() { 1,2,3,4,5,6,7,8,9,10,11,12 }, "-请选择-");
         }

         /// <summary>
         /// 是否出单
         /// </summary>
         /// <param name="accessright"></param>
         /// <returns></returns>
         public static SelectList GetDeal(string accessright)
         {
             return new SelectList(new List<string>() { "已出单","未出单" }, "-请选择-");
         }

         /// <summary>
         /// 是否保护
         /// </summary>
         /// <param name="accessright"></param>
         /// <returns></returns>
         public static SelectList GetProtection(string accessright)
         {
             return new SelectList(new List<string>() { "保护", "未保护" }, "-请选择-");
         }


         /// <summary>
         /// 几个sales打过这个公司
         /// </summary>
         /// <param name="accessright"></param>
         /// <returns></returns>
         public static SelectList GetSales(string accessright)
         {
             return new SelectList(new List<int>() { 1, 2, 3, 4, 5, 6 }, "-请选择-");
         }

         /// <summary>
         /// 重要程度
         /// </summary>
         /// <param name="accessright"></param>
         /// <returns></returns>
         public static SelectList GetImportancy(string accessright)
         {
             return new SelectList(new List<int>() { 1, 2, 3, 4, 5, 6 }, "-请选择-");
         }

         /// <summary>
         /// 成熟度
         /// </summary>
         /// <param name="accessright"></param>
         /// <returns></returns>
         public static SelectList GetProgress(string accessright)
         {
             return new SelectList(CH.GetAllData<Progress>(), "Code", "Name", "-请选择-");
         }

         /// <summary>
         /// 天数选择
         /// </summary>
         /// <param name="accessright"></param>
         /// <returns></returns>
         public static SelectList GetDuration(string accessright)
         {
             var d1 = new SelectListItem() { Text = "1天以内", Value = "1" };
             var d3 = new SelectListItem() { Text = "3天以内", Value = "3" };
             var d7 = new SelectListItem() { Text = "7天以内", Value = "7" };
             var d15 = new SelectListItem() { Text = "15天以内", Value = "15" };
             var d30 = new SelectListItem() { Text = "30天以内", Value = "30" };
             var l = new List<SelectListItem>();
             l.Add(d1);
             l.Add(d3);
             l.Add(d7);
             l.Add(d15);
             l.Add(d30);
             return new SelectList(l, "-请选择-");
         }

         /// <summary>
         /// 来源
         /// </summary>
         /// <param name="accessright"></param>
         /// <returns></returns>
         public static SelectList GetFrom(int? projectid)
         {
             List<Member> members;
             if(projectid!=null)
             members =  CH.GetAllData<Member>(m=>m.ProjectID == projectid);
             else
                 members =  CH.GetAllData<Member>();
             var names = members.Select(s=>s.Name).Distinct().ToList();
             var l = new List<SelectListItem>();
              var d = new SelectListItem() { Text ="系统导入" , Value = "系统导入" };
             l.Add(d);
             foreach(var n in names)
             {
                  var a = new SelectListItem() { Text =n , Value = n };
                 l.Add(a);
             }

             return new SelectList(l, "-请选择-");
         }
     }

   
    public class CompamyRelationshipSelectors
    {
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coms">公司</param>
        /// <param name="location">国内还是国外</param>
        /// <param name="projectid">项目</param>
        /// <param name="calledlead">打过的Lead数、以上</param>
        /// <param name="hasdeal">是否已出单</param>
        /// <param name="protect">是否保护</param>
        /// <param name="salescall">几个sales打过这个公司、以上</param>
        /// <param name="importancy">重要程度</param>
        /// <param name="progress">成熟度</param>
        /// <param name="beforedayadd">添加的天数/以内</param>
        /// <param name="from">来源</param>
        /// <param name="dealinsum">出单总额以上</param>
        /// <param name="sales">指派销售</param>
        /// <returns></returns>
        public static List<CompanyRelationship> Selector(List<CompanyRelationship> coms,
            string location,
            int? projectid,
            int?calledlead,
            bool? hasdeal,
            bool? protect,
            int? salescall,
            int?importancy,
            int? progress,
            int? beforedayadd,
            string from,
            decimal? dealinsum,
            string sales
            )
        {
            if (!string.IsNullOrEmpty(location))
            {
                if (location == "国内")
                coms = coms.FindAll(f => f.Company.DistrictNumberID == null);
                else
                    coms = coms.FindAll(f => f.Company.DistrictNumberID == null);
            }
            if (projectid != null)
            {
                coms = coms.FindAll(f=>f.ProjectID == projectid);
            }
            if(calledlead != null)
            {
                coms = coms.FindAll(f => f.LeadCalls.Select(s => s.Lead.ID).Distinct().Count() >= calledlead);
            }
            if (hasdeal == null)
            {
                coms = coms.FindAll(f => f.Deals!=null&& f.Deals.Count>0);
            }
            if (importancy != null)
            {
                coms = coms.FindAll(f => f.Importancy >= importancy);
            }
            if (progress != null)
            {
                coms = coms.FindAll(f => f.Progress.Code >= progress);
            }
            if (beforedayadd != null)
            {
                coms = coms.FindAll(f => f.CreatedDate !=null && (DateTime.Now - f.CreatedDate.Value).Days <= beforedayadd.Value);
            }
            if (string.IsNullOrEmpty(from))
            {
                coms = coms.FindAll(f => f.CreatedDate != null && (DateTime.Now - f.CreatedDate.Value).Days <= beforedayadd.Value);
            }
            if (dealinsum==null)
            {
                coms = coms.FindAll(f => f.Deals.Sum(s=>s.Payment) >= dealinsum);
            }
            if (!string.IsNullOrEmpty(sales))
            {
                coms = coms.FindAll(f => f.Members.Any(m => m.Name == sales));
            }
            return coms;
        }
    }
}