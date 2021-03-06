﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sales.Model;
using Utl;
using Entity;
using BLL;
using System.Data.Common;
using System.Text.RegularExpressions;
using System.Collections;
using System.Text;
namespace Sales.BLL
{

    public static class AvaliableCRM
    {
        public static _AvaliableCompanies GetAvaliableCompanies(CRMFilters filters = null)
        {
            var data = new _AvaliableCompanies();
            if (filters.ProjectId == null)
            {
                filters.ProjectId = CRM_Logical.GetUserInvolveProject().FirstOrDefault() == null ? 0 : CRM_Logical.GetUserInvolveProject().FirstOrDefault().ID;
            }
            if (filters.ProjectId > 0)
            {
                data.MemberCompanies = GetGroupedCRM(true, filters);
                //data.PublicCompanies = GetPublicCRM(false, filters);
            }


            return data;
        }

        static IQueryable<_CoreLVL> GetGroupedCRM(bool memberonly, CRMFilters filters = null)
        {
            if (filters == null) filters.ProjectId = 26;
            var crms = from c in CH.DB.CompanyRelationships where c.ProjectID == filters.ProjectId && c.Deleted==false select c;
            if (memberonly)
            {
                if (Employee.CurrentRole.Level == MarketInterfaceRequired.LVL || Employee.CurrentRole.Level == SalesRequired.LVL)
                    crms = crms.Where(w => w.Members.Count > 0 && w.Members.Select(s => s.Name).Contains(Employee.CurrentUserName)).OrderBy(w => w.ID);
                else if (Employee.CurrentRole.Level == ChinaTLRequired.LVL )
                    crms = crms.Where(w => w.Members.Count > 0 ).OrderBy(w => w.ID);
                    //crms = crms.Where(w => w.Members.Count > 0 && w.Members.Any(m => m.Name == Employee.CurrentUserName)).OrderBy(w => w.ID);
                else if (Employee.CurrentRole.Level == ManagerRequired.LVL || Employee.CurrentRole.Level == LeaderRequired.LVL || Employee.CurrentRole.Level == ProductInterfaceRequired.LVL || Employee.CurrentRole.Level == SalesManagerRequired.LVL)
                    crms = crms.Where(w => w.Members.Count > 0).OrderBy(w => w.ID);
            }
            else
            {
                crms = crms.Where(w => w.Members.Count == 0).OrderBy(w => w.ID);
            }
            //模糊搜索
            if (filters != null && !string.IsNullOrWhiteSpace(filters.FuzzyQuery))
            {
                crms = crms.Where(q => q.Company.Leads.Any(l =>  
                    l.Deleted==false &&
                    (
                        l.Name_CH.Contains(filters.FuzzyQuery) || 
                        l.Name_EN.Contains(filters.FuzzyQuery) || 
                        l.EMail.Contains(filters.FuzzyQuery) || 
                        l.PersonalEmailAddress.Contains(filters.FuzzyQuery)
                       )) || 
                       (q.Company.Deleted==false &&
                        (q.Company.Name_CH.Contains(filters.FuzzyQuery) || 
                        q.Company.Name_EN.Contains(filters.FuzzyQuery) || 
                        q.Company.Contact.Contains(filters.FuzzyQuery)))
                    
                    );
            }
            //行业
            if (filters != null && filters.CategoryId.HasValue)
            {
                crms = crms.Where(q => q.Categorys.Any(c => c.ID == filters.CategoryId.Value));
            }
            //时区
            if (filters != null && filters.DistinctNumber.HasValue)
            {
                crms = crms.Where(q => q.Company.DistrictNumberID == filters.DistinctNumber);
            }
            //点评
            if (filters != null && filters.IfComment == 1)
            {
                crms = crms.Where(q => q.Comments.Count > 0);
            }
            if (filters != null && filters.IfComment == 0)
            {
                crms = crms.Where(q => q.Comments.Count == 0);
            }

            if (filters != null && !string.IsNullOrWhiteSpace(filters.selSales))
            {
                crms = crms.Where(s => s.Members.Any(q => q.Name == filters.selSales));
            }

            var data = from c in CH.DB.CoreLVLs
                       select new _CoreLVL()
                           {
                               CoreName = c.CoreLVLName,
                               ID = c.ID,
                               CrmCount = crms.Where(cr => cr.CoreLVLID == c.CoreLVLCode).Count(),
                               _Maturitys = (from m in crms.Where(cr => cr.CoreLVLID == c.CoreLVLCode)
                                             group m by new { m.ProgressID, m.Progress.Name } into grp
                                             select new _Maturity()
                                             {
                                                 Name = grp.Key.Name,
                                                 ID = grp.Key.ProgressID.Value,
                                                 Count = crms.Where(co => co.ProgressID == grp.Key.ProgressID && co.CoreLVLID == c.CoreLVLCode).Count(),
                                                 _CRMs = (from crm in grp.Select(s => s)
                                                          select new _CRM
                                                          {
                                                              ID = crm.ID,
                                                              CompanyNameCH = crm.Company.Name_CH,
                                                              CompanyNameEN = crm.Company.Name_EN,
                                                              CoreCompany = c.CoreLVLName == "核心公司" ? true : false,
                                                              //ContectedLeadCount = CH.DB.Leads.Where(l => l.CompanyID == crm.CompanyID && l.Deleted == false && crm.LeadCalls.Any(w => w.LeadID == l.ID)).Count(),
                                                              //LeadCount = CH.DB.Leads.Where(l => l.CompanyID == crm.CompanyID && l.Deleted==false).Count(),
                                                              CrmCommentStateID = crm.CrmCommentStateID,
                                                              CrmCommentStateIDOrder = (crm.CrmCommentStateID == 1 || crm.CrmCommentStateID == 2 || crm.CrmCommentStateID == 3) ? "a" : "b",
                                                              _Comments = (from co in crm.Comments.OrderByDescending(m => m.CommentDate)
                                                                           select new _Comment()
                                                                           {
                                                                               Submitter = co.Submitter,
                                                                               CommentDate = co.CommentDate,
                                                                               CRMID = co.CompanyRelationshipID,
                                                                               Contents = co.Contents
                                                                           })
                                                          }).OrderBy(cr => cr.CrmCommentStateIDOrder).ThenBy(cr => cr.CompanyNameEN).ThenBy(cr => cr.CompanyNameCH)
                                             })
                           };

            data = data.Take(2);//修复数据异常

            return data;
        }
        //static IQueryable<_CoreLVL> GetPublicCRM(bool memberonly, CRMFilters filters = null)
        //{
        //    if (filters.ProjectId == null) filters.ProjectId = 26;
        //    var crms = from c in CH.DB.CompanyRelationships where c.ProjectID == filters.ProjectId && c.Deleted==false select c;

        //    if (memberonly)
        //    {
        //        crms = crms.Where(w => w.Members.Count > 0);
        //    }
        //    else
        //    {
        //        //if (Employee.CurrentRole.Level == SalesRequired.LVL || Employee.CurrentRole.Level == LeaderRequired.LVL)
        //        //    crms = crms.Where(w => w.Members.Any(m => m.Name == Employee.CurrentUserName)==false).OrderBy(w => w.ID);
        //        //else if (Employee.CurrentRole.Level == ManagerRequired.LVL)
        //        crms = crms.Where(w => w.Members.Count == 0);

        //    }
        //    //模糊搜索
        //    if (filters != null && !string.IsNullOrWhiteSpace(filters.FuzzyQuery))
        //    {
        //        crms = crms.Where(q => q.Company.Leads.Any(l =>  l.Deleted==false &&( l.Name_CH.Contains(filters.FuzzyQuery) || l.Name_EN.Contains(filters.FuzzyQuery) || l.EMail.Contains(filters.FuzzyQuery) || l.PersonalEmailAddress.Contains(filters.FuzzyQuery)) || q.Company.Name_CH.Contains(filters.FuzzyQuery) || q.Company.Name_EN.Contains(filters.FuzzyQuery) || q.Company.Contact.Contains(filters.FuzzyQuery)));
        //    }
        //    //行业
        //    if (filters != null && filters.CategoryId.HasValue)
        //    {
        //        crms = crms.Where(q => q.Categorys.Any(c => c.ID == filters.CategoryId.Value));
        //    }
        //    //时区
        //    if (filters != null && filters.DistinctNumber.HasValue)
        //    {
        //        crms = crms.Where(q => q.Company.DistrictNumberID == filters.DistinctNumber);
        //    }
        //    //点评
        //    if (filters != null && filters.IfComment == 1)
        //    {
        //        crms = crms.Where(q => q.Comments.Count > 0);
        //    }
        //    if (filters != null && filters.IfComment == 0)
        //    {
        //        crms = crms.Where(q => q.Comments.Count == 0);
        //    }

        //    //if (filters != null && !string.IsNullOrWhiteSpace(filters.selSales))
        //    //{
        //    //    crms = crms.Where(s => s.Members.Any(q => q.Name == filters.selSales));
        //    //}

        //    var data = from c in CH.DB.CoreLVLs
        //               select new _CoreLVL()
        //               {
        //                   CoreName = c.CoreLVLName,
        //                   ID = c.ID,
        //                   CrmCount = crms.Where(cr => cr.CoreLVLID == c.CoreLVLCode).Count(),
        //                   //_CRMs = (from crm in crms.Where(cr => cr.CoreLVLID == c.CoreLVLCode)
        //                   //         select new _CRM
        //                   //         {
        //                   //             ID = crm.ID,
        //                   //             CompanyNameCH = crm.Company.Name_CH,
        //                   //             CompanyNameEN = crm.Company.Name_EN,
        //                   //             CoreCompany = c.CoreLVLName == "核心公司" ? true : false,
        //                   //             //ContectedLeadCount = crm.LeadCalls.GroupBy(call => call.LeadID).Count(),
        //                   //            // ContectedLeadCount = CH.DB.Leads.Where(l => l.CompanyID == crm.CompanyID && crm.LeadCalls.Any(w=>w.LeadID==l.ID)).Count(),
        //                   //             LeadCount = CH.DB.Leads.Where(l => l.CompanyID == crm.CompanyID).Count(),
        //                   //             //CrmCommentStateID = crm.CrmCommentStateID,
        //                   //             //CrmCommentStateIDOrder = (crm.CrmCommentStateID == 1 || crm.CrmCommentStateID == 2 || crm.CrmCommentStateID == 3) ? "a" : "b",
        //                   //             //_Comments = (from co in crm.Comments.OrderByDescending(m => m.CommentDate)
        //                   //             //             select new _Comment()
        //                   //             //             {
        //                   //             //                 Submitter = co.Submitter,
        //                   //             //                 CommentDate = co.CommentDate,
        //                   //             //                 CRMID = co.CompanyRelationshipID,
        //                   //             //                 Contents = co.Contents
        //                   //             //             })
        //                   //         })
        //                            //.OrderBy(cr => cr.CrmCommentStateIDOrder).ThenBy(cr => cr.CompanyNameEN).ThenBy(cr => cr.CompanyNameCH)
        //                   //.OrderBy(cr => cr.CompanyNameCH).OrderBy(cr => cr.CompanyNameEN).OrderBy(cr => cr.CrmCommentStateIDOrder)
        //               };

        //    return data;
        //}
        public static _CRM _CRMGetAvaliableCrmDetail(int? crmid)
        {
            var data = CH.GetDataById<CompanyRelationship>(crmid);
            var projectenddata = data.Project.EndDate == null ? DateTime.Now : data.Project.EndDate;
            var projectcreatedata = data.Project.CreatedDate == null ? DateTime.Now : data.Project.CreatedDate;
            var othersaleslead = data.Company.Leads.Where(x => x.Creator != Employee.CurrentUserName && x.CreatedDate >= projectcreatedata && x.CreatedDate <= projectenddata);
            bool hasvalue = false;
            var callsgrp = data.LeadCalls.Where(w=>w.Deleted==false).OrderByDescending(c => c.CallDate).GroupBy(c => c.LeadID).Where(g => g.Count() >= 1).Select(g => g.ElementAt(0));
            //覆盖率 = 以打lead数/总数 *１００％
            // 不同时差数，是ｌｅａｄ所在ｌｅａｄｄｉｓｃｔｉｎｃｔｉｄ有几个不同
            //  个人类型数量是个ｌｉｓｔ
            // 各个ｃａｌｌｔｙｐｅ的数量
            
            //var leadids = data.Company.Leads.Where(w=>w.Deleted==false ).Except(othersaleslead).Select(s=>s.ID).ToList();
            var leadids = data.Company.Leads.Where(w => w.Deleted == false).Select(s => s.ID).ToList();

            var hiscall = from c in CH.DB.LeadCalls where leadids.Contains((int)c.LeadID) && c.ProjectID != data.ProjectID && c.Deleted==false
                          orderby c.CallDate descending
                          select c;

            var crm = new _CRM()
            {
                CategoryEdit = (Employee.AsManager() || Employee.AsProductInterface()),
                ID = data.ID,
                CompanyID = data.CompanyID,
                ProgressID = data.ProgressID,
                CompanyNameEN = data.Company.Name_EN,
                CompanyNameCH = data.Company.Name_CH,
                Contact = data.Company.Contact,
                Fax = data.Company.Fax,
                Email = "没找到字段",
                BlowedCount = callsgrp != null ? callsgrp.Where(c => c.LeadCallTypeID == 2).Count() : 0,
                NoPitchCount = callsgrp != null ? callsgrp.Where(c => c.LeadCallTypeID == 3).Count() : 0,
                PitchCount = callsgrp != null ? callsgrp.Where(c => c.LeadCallTypeID == 4).Count() : 0,
                FullPitchCount = callsgrp != null ? callsgrp.Where(c => c.LeadCallTypeID == 5).Count() : 0,
                CallBackedCount = callsgrp != null ? callsgrp.Where(c => c.LeadCallTypeID == 6).Count() : 0,
                QualifiedDecisionCount = callsgrp != null ? callsgrp.Where(c => c.LeadCallTypeID == 8).Count() : 0,
                WaitForApproveCount = callsgrp != null ? callsgrp.Where(c => c.LeadCallTypeID == 7).Count() : 0,
                CloseDealCount = callsgrp != null ? callsgrp.Where(c => c.LeadCallTypeID == 9).Count() : 0,
                NoCallCount = data.Company.Leads.Where(l => !data.LeadCalls.Where(c => c.LeadID == l.ID).Any()).Count(),
                CoreCompany = data.CoreLVL == null ? false : data.CoreLVL.CoreLVLName == "核心公司" ? true : false,
                CrmCommentStateID = data.CrmCommentStateID,
                CoreLVLID = data.CoreLVLID,
                CrmStatisitcs = new _CrmStatisitcs()
                                {
                                    LeadCount = data.Company.Leads.Count(f=>f.Deleted==false),
                                    CallCount = data.LeadCalls.Count(f => f.Deleted == false),
                                    LeadMaxCallCount = data.LeadCalls.Where(f=>f.Deleted==false).GroupBy(lc => lc.LeadID).Select(k => new { Name = k.Key, Count = k.Count() }).OrderByDescending(m => m.Count).FirstOrDefault() != null ? data.LeadCalls.Where(f=>f.Deleted==false).GroupBy(lc => lc.LeadID).Select(k => new { Name = k.Key, Count = k.Count() }).OrderByDescending(m => m.Count).FirstOrDefault().Count : 0,
                                    LeadAvgCallCount = data.Company.Leads.Where(f=>f.Deleted==false).Count() != 0 ? (double)data.LeadCalls.Count(f => f.Deleted == false) / (double)data.Company.Leads.Count(f => f.Deleted == false) : 0,
                                    CoverageRate = data.Company.Leads.Where(f => f.Deleted == false).Count() != 0 ? ((double)callsgrp.Count() / (double)data.Company.Leads.Count(f => f.Deleted == false)) * 100 : 0,
                                    TimeDiffer = data.Company.Leads.Where(f => f.Deleted == false).GroupBy(l => l.DistrictNumberID).Count(),
                                    LeadCalledCount = data.LeadCalls.Where(f => f.Deleted == false).GroupBy(lc => lc.LeadID).Count(),

                                    CallTypeCounts = from grp in data.LeadCalls.Where(f => f.Deleted == false).GroupBy(lc => lc.LeadCallTypeID)
                                                     select new CallTypeCount()
                                                     {
                                                         TypeName = CH.GetDataById<LeadCallType>(grp.Key).Name,
                                                         Count = grp.Count()
                                                     }
                                },
                _members = data.Members,
                _Comments = (from co in data.Comments.OrderByDescending(m => m.CommentDate)
                             select new _Comment()
                             {
                                 Submitter = co.Submitter,
                                 CommentDate = co.CommentDate,
                                 CRMID = co.CompanyRelationshipID,
                                 Contents = co.Contents
                             }),
                _Categorys = (from c in data.Categorys
                              select new _Category()
                              {
                                  ID = c.ID,
                                  Name = c.Name,
                                  Details = c.Details,
                                  Description = c.Description
                              }
                                 ),
                Description = data.Company.Description,
                Competitor = data.Company.Competitor,
                PitchPoint = data.PitchedPoint,
                _Leads = (from leads in data.Company.Leads.Where(f => f.Deleted == false && leadids.Contains(f.ID)).OrderByDescending(x => x.Sequence)
                          select new _Lead()
                          {
                              ID = leads.ID,
                              CompanyID = leads.CompanyID,
                              Name = leads.Name_CH + " " + leads.Name_EN,
                              SName = !string.IsNullOrEmpty(leads.Name_EN) ?  leads.Name_EN :leads.Name_CH,
                              Title = leads.Title,
                              Contact = leads.Mobile,
                              Fax = leads.Fax,
                              TelePhone = leads.Contact,
                              Email = leads.EMail,
                              Gender = !string.IsNullOrEmpty(leads.Gender) ? leads.Gender : "",
                              
                              LastCallTypeID = data.LeadCalls.Where(c => c.LeadID == leads.ID && c.Deleted==false).OrderByDescending(c => c.CallDate).FirstOrDefault() == null ? 0 : data.LeadCalls.Where(c => c.LeadID == leads.ID && c.Deleted==false).OrderByDescending(c => c.CallDate).FirstOrDefault().LeadCallTypeID,
                              OwnLeader = data.Members.Where(w=>w.Name==Employee.CurrentUserName).Any(),
                              Sequence = leads.Sequence
                          }),
                _LeadCalls = (from leadcalls in data.LeadCalls.Where(f => f.Deleted == false && f.ProjectID == data.ProjectID && leadids.Contains((int)f.LeadID)).OrderByDescending(m => m.CallDate)
                              select new _LeadCall()
                              {
                                  LeadID = leadcalls.LeadID,
                                  LeadName = leadcalls.Lead.Name_EN + " " + leadcalls.Lead.Name_CH,
                                  LeadTitle = leadcalls.Lead.Title,
                                  CallResult = leadcalls.Result,
                                  CallType = leadcalls.LeadCallType.Name,
                                  CallDate = leadcalls.CallDate,
                                  Creator = leadcalls.Creator,
                                  LeadCallTypeID = leadcalls.LeadCallTypeID,
                                  MemberName = leadcalls.Member.Name
                              }),
                              _HistoryCalls = (from histcalls in  hiscall
                                select new _LeadCall()
                              {
                                  LeadID = histcalls.LeadID,
                                  LeadName = histcalls.Lead.Name_EN + " " + histcalls.Lead.Name_CH,
                                  LeadTitle = histcalls.Lead.Title,
                                  CallResult = histcalls.Result,
                                  CallType = histcalls.LeadCallType.Name,
                                  CallDate = histcalls.CallDate,
                                  Creator = histcalls.Creator,
                                  LeadCallTypeID = histcalls.LeadCallTypeID,
                                  MemberName = histcalls.Member.Name
                              }),
                _ProgressTrack = CH.DB.ProgressTrack.Where(pt => pt.CompanyRelationshipID == crmid).OrderByDescending(pt => pt.ChangeDate)
            };
            crm._Leads = crm._Leads.OrderByDescending(l => l.Sequence).ThenBy(l => l.Name);
            //Regex regex = new Regex(@"<[^>]+>|</[^>]+>");
            //crm.Description = regex.Replace(crm.Description, "");
            //crm.Description = crm.Description.Replace("&nbsp;", "");
            if (!string.IsNullOrEmpty(crm.Description))
            {
                if (crm.Description.Length > 200)
                    crm.Description = SubstringToHTML(crm.Description, 200, "...");
            }
            return crm;
        }
        public static _CRM _CRMGetAvaliableCrmDetailByCrmIDLeadID(int crmid, int leadid)
        {
            var data = CH.GetDataById<CompanyRelationship>(crmid);
            var projectenddata = data.Project.EndDate == null ? DateTime.Now : data.Project.EndDate;
            var projectcreatedata = data.Project.CreatedDate == null ? DateTime.Now : data.Project.CreatedDate;
            var othersaleslead = data.Company.Leads.Where(x => x.Creator != Employee.CurrentUserName && x.CreatedDate >= projectcreatedata && x.CreatedDate <= projectenddata);
            //var leadids = data.Company.Leads.Where(w => w.Deleted == false).Except(othersaleslead).Select(s => s.ID).ToList();
            var leadids = data.Company.Leads.Where(w => w.Deleted == false).Select(s => s.ID).ToList();
            var callsgrp = data.LeadCalls.Where(f => f.Deleted == false).OrderByDescending(c => c.CallDate).GroupBy(c => c.LeadID)
                                .Where(g => g.Count() >= 1)
                                .Select(g => g.ElementAt(0));
            var crm = new _CRM()
            {
                ID = data.ID,
                CompanyID = data.CompanyID,
                ProgressID = data.ProgressID,
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
                CrmCommentStateID = data.CrmCommentStateID,
                CoreLVLID = data.CoreLVLID,
                _members = data.Members,
                CrmStatisitcs = new _CrmStatisitcs()
                {
                    LeadCount = data.Company.Leads.Where(f => f.Deleted == false).Count(),
                    CallCount = data.LeadCalls.Where(f => f.Deleted == false).Count(),
                    LeadMaxCallCount = data.LeadCalls.Where(f => f.Deleted == false).GroupBy(lc => lc.LeadID).Select(k => new { Name = k.Key, Count = k.Count() }).OrderByDescending(m => m.Count).FirstOrDefault() != null ? data.LeadCalls.Where(f=>f.Deleted==false).GroupBy(lc => lc.LeadID).Select(k => new { Name = k.Key, Count = k.Count() }).OrderByDescending(m => m.Count).FirstOrDefault().Count : 0,
                    LeadAvgCallCount = data.Company.Leads.Where(f => f.Deleted == false).Count() != 0 ? (double)data.LeadCalls.Count(f => f.Deleted == false) / (double)data.Company.Leads.Count(f => f.Deleted == false) : 0,
                    CoverageRate = data.Company.Leads.Count(f => f.Deleted == false) != 0 ? ((double)callsgrp.Count() / (double)data.Company.Leads.Count(f => f.Deleted == false)) * 100 : 0,
                    TimeDiffer = data.Company.Leads.Where(f => f.Deleted == false).GroupBy(l => l.DistrictNumberID).Count(),
                    LeadCalledCount = data.LeadCalls.Where(f => f.Deleted == false).GroupBy(lc => lc.LeadID).Count(),

                    CallTypeCounts = from grp in data.LeadCalls.Where(f => f.Deleted == false).GroupBy(lc => lc.LeadCallTypeID)
                                     select new CallTypeCount()
                                     {
                                         TypeName = CH.GetDataById<LeadCallType>(grp.Key).Name,
                                         Count = grp.Count()
                                     }
                },
                _Categorys = (from c in data.Categorys
                              select new _Category()
                              {
                                  Name = c.Name,
                                  Details = c.Details,
                                  Description = c.Description
                              }
                                 ),
                _Comments = (from co in data.Comments.OrderByDescending(m => m.CommentDate)
                             select new _Comment()
                             {
                                 Submitter = co.Submitter,
                                 CommentDate = co.CommentDate,
                                 CRMID = co.CompanyRelationshipID,
                                 Contents = co.Contents
                             }),
                Description = data.Company.Description,
                Competitor = data.Company.Competitor,
                PitchPoint = data.PitchedPoint,
                _Leads = (from leads in data.Company.Leads.Where(f => f.Deleted == false && leadids.Contains(f.ID))
                          select new _Lead()
                          {
                              ID = leads.ID,
                              Name = leads.Name_CH + " " + leads.Name_EN,
                              SName = !string.IsNullOrEmpty(leads.Name_EN) ? leads.Name_EN : leads.Name_CH,
                              Title = leads.Title,
                              Contact = leads.Mobile,
                              Fax = leads.Fax,
                              TelePhone = leads.Contact,
                              Email = leads.EMail,
                              Gender = !string.IsNullOrEmpty(leads.Gender) ? leads.Gender : "",
                              LastCallTypeID = data.LeadCalls.Where(c => c.LeadID == leads.ID && c.Deleted==false).OrderByDescending(c => c.CallDate).FirstOrDefault() == null ? 0 : data.LeadCalls.Where(c => c.LeadID == leads.ID && c.Deleted==false).OrderByDescending(c => c.CallDate).FirstOrDefault().LeadCallTypeID,
                              OwnLeader = data.Members.Where(w => w.Name == Employee.CurrentUserName).Any(),
                              Sequence = leads.Sequence
                          }),
                _LeadCalls = (from leadcalls in data.LeadCalls.Where(f => f.Deleted == false && leadids.Contains((int)f.LeadID)).OrderByDescending(m => m.CallDate)
                              select new _LeadCall()
                              {
                                  LeadID = leadcalls.LeadID,
                                  LeadName = leadcalls.Lead.Name_EN + " " + leadcalls.Lead.Name_CH,
                                  LeadTitle = leadcalls.Lead.Title,
                                  CallResult = leadcalls.Result,
                                  CallType = leadcalls.LeadCallType.Name,
                                  CallDate = leadcalls.CallDate,
                                  Creator = leadcalls.Creator,
                                  LeadCallTypeID = leadcalls.LeadCallTypeID,
                                  MemberName = leadcalls.Member.Name
                              }),
                _ProgressTrack = CH.DB.ProgressTrack.Where(pt => pt.CompanyRelationshipID == crmid).OrderByDescending(pt => pt.ChangeDate)

            };
            crm._Leads = crm._Leads.OrderByDescending(l => l.Sequence);
            if (!string.IsNullOrEmpty(crm.Description))
            {
                if (crm.Description.Length > 200)
                    crm.Description = SubstringToHTML(crm.Description, 200, "...");
            }
            return crm;
        }
        public static _CRM _CRMGetAvaliableCrmDetailByCrmIDMember(int crmid, string membername)
        {
            var data = CH.GetDataById<CompanyRelationship>(crmid);
            var projectenddata = data.Project.EndDate == null ? DateTime.Now : data.Project.EndDate;
            var projectcreatedata = data.Project.CreatedDate == null ? DateTime.Now : data.Project.CreatedDate;
            var othersaleslead = data.Company.Leads.Where(x => x.Creator != Employee.CurrentUserName && x.CreatedDate >= projectcreatedata && x.CreatedDate <= projectenddata);
            //var leadids = data.Company.Leads.Where(w => w.Deleted == false).Except(othersaleslead).Select(s => s.ID).ToList();
            var leadids = data.Company.Leads.Where(w => w.Deleted == false).Select(s => s.ID).ToList();
            var callsgrp = data.LeadCalls.Where(f => f.Deleted == false).OrderByDescending(c => c.CallDate).GroupBy(c => c.LeadID)
                                .Where(g => g.Count() >= 1)
                                .Select(g => g.ElementAt(0));
            var crm = new _CRM()
            {
                ID = data.ID,
                CompanyID = data.CompanyID,
                ProgressID = data.ProgressID,
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
                CrmCommentStateID = data.CrmCommentStateID,
                CoreLVLID = data.CoreLVLID,
                _members = data.Members,
                CrmStatisitcs = new _CrmStatisitcs()
                {
                    LeadCount = data.Company.Leads.Where(f => f.Deleted == false).Count(),
                    CallCount = data.LeadCalls.Where(cc => cc.Member.Name == membername && cc.Deleted==false).Count(),
                    LeadMaxCallCount = data.LeadCalls.Where(cc => cc.Member.Name == membername && cc.Deleted==false).GroupBy(lc => lc.LeadID).Select(k => new { Name = k.Key, Count = k.Count() }).OrderByDescending(m => m.Count).FirstOrDefault() != null ? data.LeadCalls.Where(cc => cc.Member.Name == membername).GroupBy(lc => lc.LeadID).Select(k => new { Name = k.Key, Count = k.Count() }).OrderByDescending(m => m.Count).FirstOrDefault().Count : 0,
                    LeadAvgCallCount = data.Company.Leads.Count(f => f.Deleted == false) != 0 ? (double)data.LeadCalls.Where(cc => cc.Member.Name == membername).Count(f => f.Deleted == false) / (double)data.Company.Leads.Count(f => f.Deleted == false) : 0,
                    CoverageRate = data.Company.Leads.Count(f => f.Deleted == false) != 0 ? (double)callsgrp.Count() / (double)data.Company.Leads.Count(f => f.Deleted == false) * 100 : 0,
                    TimeDiffer = data.Company.Leads.Where(f => f.Deleted == false).GroupBy(l => l.DistrictNumberID).Count(),
                    LeadCalledCount = data.LeadCalls.Where(cc => cc.Member.Name == membername && cc.Deleted==false).GroupBy(lc => lc.LeadID).Count(),
                    CallTypeCounts = from grp in data.LeadCalls.Where(cc => cc.Member.Name == membername && cc.Deleted==false).GroupBy(lc => lc.LeadCallTypeID)
                                     select new CallTypeCount()
                                     {
                                         TypeName = CH.GetDataById<LeadCallType>(grp.Key).Name,
                                         Count = grp.Count()
                                     },
                    crmtracks = CH.DB.CrmTracks.Where(tr => tr.CompanyRelationshipID == crmid && tr.Owner == membername).OrderByDescending(tr => tr.GetDate)
                },
                _Categorys = (from c in data.Categorys
                              select new _Category()
                              {
                                  Name = c.Name,
                                  Details = c.Details,
                                  Description = c.Description
                              }
                                 ),
                _Comments = (from co in data.Comments.OrderByDescending(m => m.CommentDate)
                             select new _Comment()
                             {
                                 Submitter = co.Submitter,
                                 CommentDate = co.CommentDate,
                                 CRMID = co.CompanyRelationshipID,
                                 Contents = co.Contents
                             }),
                Description = data.Company.Description,
                Competitor = data.Company.Competitor,
                PitchPoint = data.PitchedPoint,
                _Leads = (from leads in data.Company.Leads.Where(f => f.Deleted == false && leadids.Contains(f.ID))
                          select new _Lead()
                          {
                              ID = leads.ID,
                              Name = leads.Name_CH + " " + leads.Name_EN,
                              SName = !string.IsNullOrEmpty(leads.Name_EN) ? leads.Name_EN : leads.Name_CH,
                              Title = leads.Title,
                              Contact = leads.Mobile,
                              Fax = leads.Fax,
                              TelePhone = leads.Contact,
                              Email = leads.EMail,
                              Gender = !string.IsNullOrEmpty(leads.Gender) ? leads.Gender : "",
                              LastCallTypeID = data.LeadCalls.Where(c => c.LeadID == leads.ID && c.Deleted==false).OrderByDescending(c => c.CallDate).FirstOrDefault() == null ? 0 : data.LeadCalls.Where(c => c.LeadID == leads.ID && c.Deleted==false).OrderByDescending(c => c.CallDate).FirstOrDefault().LeadCallTypeID,
                              OwnLeader = data.Members.Where(w => w.Name == Employee.CurrentUserName).Any(),
                              Sequence = leads.Sequence
                          }),
                _LeadCalls = (from leadcalls in data.LeadCalls.Where(f => f.Deleted == false && leadids.Contains((int)f.LeadID)).OrderByDescending(m => m.CallDate)
                              select new _LeadCall()
                              {
                                  LeadID = leadcalls.LeadID,
                                  LeadName = leadcalls.Lead.Name_EN + " " + leadcalls.Lead.Name_CH,
                                  LeadTitle = leadcalls.Lead.Title,
                                  CallResult = leadcalls.Result,
                                  CallType = leadcalls.LeadCallType.Name,
                                  CallDate = leadcalls.CallDate,
                                  Creator = leadcalls.Creator,
                                  LeadCallTypeID = leadcalls.LeadCallTypeID,
                                  MemberName = leadcalls.Member.Name
                              }),
                _ProgressTrack = CH.DB.ProgressTrack.Where(pt => pt.CompanyRelationshipID == crmid).OrderByDescending(pt => pt.ChangeDate)

            };
            crm._Leads = crm._Leads.OrderByDescending(l => l.Sequence);
            if (!string.IsNullOrEmpty(crm.Description))
            {
                if (crm.Description.Length > 200)
                    crm.Description = SubstringToHTML(crm.Description, 200, "...");
            }
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
                 CrmCount = CH.DB.CompanyRelationships.Where(f => f.Deleted == false).Count(w => w.ProjectID == projectid && w.CoreLVLID == core.ID),
                 _Maturitys = (from m in CH.DB.Progresss
                               select new _Maturity()
                               {
                                   Name = m.Name,
                                   _CRMs = from crm in CH.DB.CompanyRelationships.Where(w => w.ProjectID == projectid && w.CoreLVLID == core.ID && w.Deleted==false)
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
        /// <summary>
        /// 按字节长度截取字符串(支持截取带HTML代码样式的字符串)
        /// </summary>
        /// <param name="param">将要截取的字符串参数</param>
        /// <param name="length">截取的字节长度</param>
        /// <param name="end">字符串末尾补上的字符串</param>
        /// <returns>返回截取后的字符串</returns>
        public static string SubstringToHTML(string param, int length, string end)
        {
            string Pattern = null;
            MatchCollection m = null;
            StringBuilder result = new StringBuilder();
            int n = 0;
            char temp;
            bool isCode = false; //是不是HTML代码
            bool isHTML = false; //是不是HTML特殊字符,如&nbsp;
            char[] pchar = param.ToCharArray();
            for (int i = 0; i < pchar.Length; i++)
            {
                temp = pchar[i];
                if (temp == '<')
                {
                    isCode = true;
                }
                else if (temp == '&')
                {
                    isHTML = true;
                }
                else if (temp == '>' && isCode)
                {
                    n = n - 1;
                    isCode = false;
                }
                else if (temp == ';' && isHTML)
                {
                    isHTML = false;
                }
                if (!isCode && !isHTML)
                {
                    n = n + 1;
                    //UNICODE码字符占两个字节
                    if (System.Text.Encoding.Default.GetBytes(temp + "").Length > 1)
                    {
                        n = n + 1;
                    }
                }
                result.Append(temp);
                if (n >= length)
                {
                    break;
                }
            }
            result.Append(end);
            //取出截取字符串中的HTML标记
            string temp_result = result.ToString().Replace("(>)[^<>]*(<?)", "$1$2");
            //去掉不需要结素标记的HTML标记
            temp_result = temp_result.Replace(@"</?(AREA|BASE|BASEFONT|BODY|BR|COL|COLGROUP|DD|DT|FRAME|HEAD|HR|HTML|IMG|INPUT|ISINDEX|LI|LINK|META|OPTION|P|PARAM|TBODY|TD|TFOOT|TH|THEAD|TR|area|base|basefont|body|br|col|colgroup|dd|dt|frame|head|hr|html|img|input|isindex|li|link|meta|option|p|param|tbody|td|tfoot|th|thead|tr)[^<>]*/?>",
             "");
            //去掉成对的HTML标记
            temp_result = temp_result.Replace(@"<([a-zA-Z]+)[^<>]*>(.*?)</\1>", "$2");
            //用正则表达式取出标记
            Pattern = ("<([a-zA-Z]+)[^<>]*>");
            m = Regex.Matches(temp_result, Pattern);
            ArrayList endHTML = new ArrayList();
            foreach (Match mt in m)
            {
                endHTML.Add(mt.Result("$1"));
            }
            //补全不成对的HTML标记
            for (int i = endHTML.Count - 1; i >= 0; i--)
            {
                result.Append("</");
                result.Append(endHTML[i]);
                result.Append(">");
            }
            return result.ToString();
        }
    }
}