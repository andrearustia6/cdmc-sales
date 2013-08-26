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
        public static _AvaliableCompanies GetAvaliableCompanies(int? projectid)
        {
            var data = new _AvaliableCompanies();

            data.MemberCompanies = GetGroupedCRM(true,projectid);
            data.PublicCompanies = GetGroupedCRM(false, projectid);

            return data;
        }

        static IQueryable<_CoreLVL> GetGroupedCRM(bool memberonly, int? projectid)
        {
            if (projectid == null) projectid = 26;
            var crms = from c in CH.DB.CompanyRelationships where c.ProjectID == projectid select c;
            if (memberonly)
            {
                crms = crms.Where(w => w.Members.Count > 0);
            }
            else
            {
                crms = crms.Where(w => w.Members.Count == 0);
            }

            var data = from c in CH.DB.CoreLVLs
                       select new _CoreLVL()
                           {
                               CoreName = c.CoreLVLName,
                               ID = c.ID,
                               _Maturitys = from m in crms group m by new{m.ProgressID,m.Progress.Name} into grp
                                            select new _Maturity() 
                                            { 
                                                 Name= grp.Key.Name,
                                                  ID = grp.Key.ProgressID.Value,
                                                 _CRMs = from crm in grp.Select(s=>s)
                                                         select new _CRM
                                                         {
                                                               ID= crm.ID,
                                                               CompanyNameCH = crm.Company.Name_CH,
                                                               CompanyNameEN = crm.Company.Name_EN,
                                                                 
                                                         }
                                            }
                           };
          
           return data;
        }

        public static _CRM _CRMGetAvaliableCrmDetail(int? crmid)
        {
            var data = CH.GetDataById<CompanyRelationship>(crmid);
            var crm = new _CRM()
            {
               //  CompanyName = data.CompanyName
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
                 CoreName = core.CoreLVLName,
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
                                               //CompanyName = crm.Company.Name,
                                               CrmCommentState = crm.CrmCommentState == null ? "" : crm.CrmCommentState.StateName
                                           },



                               }
                              )

             };

            return corecrms;
        }
    }
}