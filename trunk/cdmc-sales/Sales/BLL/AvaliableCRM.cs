using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sales.Model;
using Utl;
using Entity;
namespace Sales.BLL
{

    public static class AvaliableCRM
    {

        public static _CRM _CRMGetAvaliableCrmDetail(int? crmid)
        {
            var data = CH.GetDataById<CompanyRelationship>(crmid);
            var crm = new _CRM()
            {
                 CompanyName = data.CompanyName
            };

            return crm;
        }
        /// <summary>
        /// 读取可打公司左边的公司列表
        /// </summary>
        /// <param name="projectid"></param>
        /// <returns></returns>
        public static IEnumerable<_CoreLVL> GetAvaliableCrmIndex(int? projectid)
        {

            var corecrms = from core in CH.DB.CoreLVLs
                           select
             new _CoreLVL
             {
                 CoreName = core.CoreLVL,
                 CrmCount = CH.DB.CompanyRelationships.Count(w => w.ProjectID == projectid && w.CoreLVLID == core.ID),
                 _Maturitys = (from m in CH.DB.Progresss
                               select new _Maturity()
                               {
                                   Name = m.Name,
                                   _CRMs = from crm in CH.DB.CompanyRelationships.Where(w => w.ProjectID == projectid && w.CoreLVLID == core.ID)
                                           select new _CRM()
                                           {
                                               _Categorys = (from cate in crm.Categorys
                                                             select new _Category()
                                                             {
                                                                 CRMID = crm.ID,
                                                                 Name = cate.Name,
                                                                 PitchPoint = cate.Details
                                                             }) as IQueryable<_Category>,
                                               CompanyName = crm.Company.Name,
                                               CrmCommentState = crm.CrmCommentState == null ? "" : crm.CrmCommentState.StateName
                                           },



                               }
                              )

             };

            return corecrms;
        }
    }
}