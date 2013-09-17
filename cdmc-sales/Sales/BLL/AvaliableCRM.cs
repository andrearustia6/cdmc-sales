﻿using System;
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
            data.PublicCompanies = GetPublicCRM(false, projectid);

            return data;
        }

        static IQueryable<_CoreLVL> GetGroupedCRM(bool memberonly, int? projectid)
        {
            if (projectid == null) projectid = 26;
            var crms = from c in CH.DB.CompanyRelationships where c.ProjectID == projectid select c;
            if (memberonly)
            {
                crms = crms.Where(w => w.Members.Count > 0).OrderBy(w=>w.ID);
            }
            else
            {
                crms = crms.Where(w => w.Members.Count == 0).OrderBy(w => w.ID);
            }
            var data = from c in CH.DB.CoreLVLs
                       select new _CoreLVL()
                           {
                               CoreName = c.CoreLVLName,
                               ID = c.ID,
                               CrmCount=crms.Where(cr=>cr.CoreLVLID==c.CoreLVLCode).Count(),
                               _Maturitys = from m in crms.Where(cr=>cr.CoreLVLID==c.CoreLVLCode) group m by new{m.ProgressID,m.Progress.Name} into grp
                                            select new _Maturity() 
                                            { 
                                                 Name= grp.Key.Name,
                                                  ID = grp.Key.ProgressID.Value,
                                                  Count = crms.Where(co=>co.ProgressID==grp.Key.ProgressID && co.CoreLVLID==c.CoreLVLCode).Count(),
                                                 _CRMs = from crm in grp.Select(s=>s)
                                                         select new _CRM
                                                         {
                                                               ID= crm.ID,
                                                               CompanyNameCH = crm.Company.Name_CH,
                                                               CompanyNameEN = crm.Company.Name_EN,
                                                               CoreCompany = c.CoreLVLName=="核心公司"?true:false,
                                                               ContectedLeadCount = crm.LeadCalls.GroupBy(call=>call.LeadID).Count(),
                                                               LeadCount = CH.DB.Leads.Where(l=>l.CompanyID==crm.CompanyID).Count(),
                                                               _Comments=(from co in crm.Comments.OrderByDescending(m=>m.CommentDate)
                                                                          select new _Comment()
                                                                          {
                                                                              Submitter=co.Submitter,
                                                                              CommentDate=co.CommentDate,
                                                                              CRMID=co.CompanyRelationshipID,
                                                                              Content=co.Contents
                                                                          })
                                                         }
                                            }
                           };
          
           return data;
        }
        static IQueryable<_CoreLVL> GetPublicCRM(bool memberonly, int? projectid)
        {
            if (projectid == null) projectid = 26;
            var crms = from c in CH.DB.CompanyRelationships where c.ProjectID == projectid select c;
            if (memberonly)
            {
                crms = crms.Where(w => w.Members.Count > 0).OrderBy(w => w.ID);
            }
            else
            {
                crms = crms.Where(w => w.Members.Count == 0).OrderBy(w => w.ID);
            }

            var data = from c in CH.DB.CoreLVLs
                       select new _CoreLVL()
                       {
                           CoreName = c.CoreLVLName,
                           ID = c.ID,
                           CrmCount = crms.Where(cr => cr.CoreLVLID == c.CoreLVLCode).Count(),
                           _CRMs = from crm in crms.Where(cr => cr.CoreLVLID == c.CoreLVLCode)
                                    select new _CRM
                                    {
                                        ID = crm.ID,
                                        CompanyNameCH = crm.Company.Name_CH,
                                        CompanyNameEN = crm.Company.Name_EN,
                                        CoreCompany = c.CoreLVLName == "核心公司" ? true : false,
                                        _Comments = (from co in crm.Comments.OrderByDescending(m => m.CommentDate)
                                                        select new _Comment()
                                                        {
                                                            Submitter = co.Submitter,
                                                            CommentDate = co.CommentDate,
                                                            CRMID = co.CompanyRelationshipID,
                                                            Content = co.Contents
                                                        })
                                    }
                       };

            return data;
        }
        public static _CRM _CRMGetAvaliableCrmDetail(int? crmid)
        {
            var data = CH.GetDataById<CompanyRelationship>(crmid);
            var callsgrp = data.LeadCalls.OrderByDescending(c => c.CallDate).GroupBy(c => c.LeadID)
                                .Where(g => g.Count() >= 1)
                                .Select(g => g.ElementAt(0));
            var crm = new _CRM()
            {
                ID=data.ID,
                CompanyID=data.CompanyID,
                CompanyNameEN = data.Company.Name_EN,
                CompanyNameCH=data.Company.Name_CH,
                Contact=data.Company.Contact,
                Fax=data.Company.Fax,
                Email="没找到字段",
                BlowedCount = callsgrp.Where(c => c.LeadCallTypeID == 2).Count(),
                NoPitchCount = callsgrp.Where(c => c.LeadCallTypeID == 3).Count(),
                PitchCount = callsgrp.Where(c => c.LeadCallTypeID == 4).Count(),
                FullPitchCount = callsgrp.Where(c => c.LeadCallTypeID == 5).Count(),
                CallBackedCount = callsgrp.Where(c => c.LeadCallTypeID == 6).Count(),
                QualifiedDecisionCount = callsgrp.Where(c => c.LeadCallTypeID == 8).Count(),
                WaitForApproveCount = callsgrp.Where(c => c.LeadCallTypeID == 7).Count(),
                CloseDealCount = callsgrp.Where(c => c.LeadCallTypeID == 9).Count(),
                NoCallCount = data.Company.Leads.Where(l => !data.LeadCalls.Where(c => c.LeadID == l.ID).Any()).Count(),
                CategoryString=data.CategoryString,
                CoreCompany = data.CoreLVL==null?false:data.CoreLVL.CoreLVLName == "核心公司" ? true : false,
                CoreLVLID=data.CoreLVLID,
                _Comments = (from co in data.Comments.OrderByDescending(m => m.CommentDate)
                            select new _Comment()
                            {
                                Submitter=co.Submitter,
                                CommentDate=co.CommentDate,
                                CRMID=co.CompanyRelationshipID,
                                Content=co.Contents
                            }),
                _Categorys = (from c in data.Categorys
                              select new _Category()
                              {
                                  Name = c.Name,
                                  Details = c.Details,
                                  Description = c.Description
                              }
                                 ),
                Description=data.Description,
                Competitor=data.Company.Competitor,
                PitchPoint=data.PitchedPoint,
                _Leads= (from leads in data.Company.Leads
                         select new _Lead()
                         {
                             ID=leads.ID,
                             CompanyID=leads.CompanyID,
                             Name = leads.Name_CH + " " + leads.Name_EN,
                             Title=leads.Title,
                             Contact=leads.Contact,
                             Fax=leads.Fax,
                             Email=leads.EMail
                         }),
                _LeadCalls = (from leadcalls in data.LeadCalls.OrderByDescending(m=>m.CallDate)
                              select new _LeadCall()
                              {
                                  LeadID=leadcalls.LeadID,
                                  LeadName = leadcalls.Lead.Name_EN + " " + leadcalls.Lead.Name_CH,
                                  LeadTitle=leadcalls.Lead.Title,
                                  CallResult=leadcalls.Result,
                                  CallType=leadcalls.LeadCallType.Name,
                                  CallDate=leadcalls.CallDate,
                                  Creator=leadcalls.Creator,
                                  LeadCallTypeID=leadcalls.LeadCallTypeID
                              })

            };

            return crm;
        }
        public static _CRM _CRMGetAvaliableCrmDetailByCrmIDLeadID(int crmid,int leadid)
        {
            var data = CH.GetDataById<CompanyRelationship>(crmid);
            var callsgrp = data.LeadCalls.OrderByDescending(c => c.CallDate).GroupBy(c => c.LeadID)
                                .Where(g => g.Count() >= 1)
                                .Select(g => g.ElementAt(0));
            var crm = new _CRM()
            {
                ID = data.ID,
                CompanyID = data.CompanyID,
                CompanyNameEN = data.Company.Name_EN,
                CompanyNameCH = data.Company.Name_CH,
                Contact = data.Company.Contact,
                Fax = data.Company.Fax,
                Email = "没找到字段",
                BlowedCount = callsgrp.Where(c => c.LeadCallTypeID == 2).Count(),
                NoPitchCount = callsgrp.Where(c => c.LeadCallTypeID == 3).Count(),
                PitchCount = callsgrp.Where(c => c.LeadCallTypeID == 4).Count(),
                FullPitchCount = callsgrp.Where(c => c.LeadCallTypeID == 5).Count(),
                CallBackedCount = callsgrp.Where(c => c.LeadCallTypeID == 6).Count(),
                QualifiedDecisionCount = callsgrp.Where(c => c.LeadCallTypeID == 8).Count(),
                WaitForApproveCount = callsgrp.Where(c => c.LeadCallTypeID == 7).Count(),
                CloseDealCount = callsgrp.Where(c => c.LeadCallTypeID == 9).Count(),
                NoCallCount = data.Company.Leads.Where(l => !data.LeadCalls.Where(c => c.LeadID == l.ID).Any()).Count(),
                CoreLVLID = data.CoreLVLID,
                _Categorys= (from c in data.Categorys
                                 select new _Category()
                                 {
                                     Name=c.Name,
                                     Details=c.Details,
                                     Description=c.Description
                                 }
                                 ),
                CategoryString = data.CategoryString,
                _Comments = (from co in data.Comments.OrderByDescending(m => m.CommentDate)
                             select new _Comment()
                             {
                                 Submitter = co.Submitter,
                                 CommentDate = co.CommentDate,
                                 CRMID = co.CompanyRelationshipID,
                                 Content = co.Contents
                             }),
                Description = data.Description,
                Competitor = data.Company.Competitor,
                PitchPoint = data.PitchedPoint,
                _Leads = (from leads in data.Company.Leads
                          select new _Lead()
                          {
                              ID = leads.ID,
                              Name = leads.Name_CH + " " + leads.Name_EN,
                              Title = leads.Title,
                              Contact = leads.Contact,
                              Fax = leads.Fax,
                              Email = leads.EMail
                          }),
                _LeadCalls = (from leadcalls in data.LeadCalls.OrderByDescending(m => m.CallDate)
                              select new _LeadCall()
                              {
                                  LeadID=leadcalls.LeadID,
                                  LeadName = leadcalls.Lead.Name_EN + " " + leadcalls.Lead.Name_CH,
                                  LeadTitle = leadcalls.Lead.Title,
                                  CallResult = leadcalls.Result,
                                  CallType = leadcalls.LeadCallType.Name,
                                  CallDate = leadcalls.CallDate,
                                  Creator = leadcalls.Creator,
                                  LeadCallTypeID = leadcalls.LeadCallTypeID
                              })

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