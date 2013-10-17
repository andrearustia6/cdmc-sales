using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sales.Model;
using Utl;
using Entity;
using BLL;
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
                data.PublicCompanies = GetPublicCRM(false, filters);
            }
            

            return data;
        }

        static IQueryable<_CoreLVL> GetGroupedCRM(bool memberonly, CRMFilters filters = null)
        {
            if (filters == null) filters.ProjectId = 26;
            var crms = from c in CH.DB.CompanyRelationships where c.ProjectID == filters.ProjectId select c;
            if (memberonly)
            {
                if (Employee.CurrentRole.Level == SalesRequired.LVL)
                    crms = crms.Where(w => w.Members.Count > 0 && w.Members.Any(m=>m.Name==Employee.CurrentUserName)).OrderBy(w=>w.ID);
                else if (Employee.CurrentRole.Level == ManagerRequired.LVL || Employee.CurrentRole.Level == LeaderRequired.LVL)
                    crms = crms.Where(w => w.Members.Count > 0).OrderBy(w => w.ID);
            }
            else
            {
                crms = crms.Where(w => w.Members.Count == 0).OrderBy(w => w.ID);
            }
            //模糊搜索
            if (filters != null && !string.IsNullOrWhiteSpace(filters.FuzzyQuery))
            {
                crms = crms.Where(q => q.Company.Leads.Any(l => l.Name_CH.Contains(filters.FuzzyQuery) || l.Name_EN.Contains(filters.FuzzyQuery) || l.EMail.Contains(filters.FuzzyQuery) || l.PersonalEmailAddress.Contains(filters.FuzzyQuery)) || q.Company.Name_CH.Contains(filters.FuzzyQuery) || q.Company.Name_EN.Contains(filters.FuzzyQuery) || q.Company.Contact.Contains(filters.FuzzyQuery));
            }
            //行业
            if (filters != null && filters.CategoryId.HasValue)
            {
                crms = crms.Where(q => q.Categorys.Any(c => c.ID == filters.CategoryId.Value));
            }
            //时区
            if (filters != null && filters.DistinctNumber.HasValue)
            {
                crms = crms.Where(q => q.Company.DistrictNumberID==filters.DistinctNumber);
            }
            //点评
            if (filters != null && filters.IfComment==1)
            {
                crms = crms.Where(q => q.Comments.Count>0);
            }
            if (filters != null && filters.IfComment == 0)
            {
                crms = crms.Where(q => q.Comments.Count == 0);
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
                                                 _CRMs = (from crm in grp.Select(s=>s)
                                                         select new _CRM
                                                         {
                                                               ID= crm.ID,
                                                               CompanyNameCH = crm.Company.Name_CH,
                                                               CompanyNameEN = crm.Company.Name_EN,
                                                               CoreCompany = c.CoreLVLName=="核心公司"?true:false,
                                                               ContectedLeadCount = crm.LeadCalls.GroupBy(call => call.LeadID).Count(),
                                                               LeadCount = CH.DB.Leads.Where(l => l.CompanyID == crm.CompanyID).Count(),
                                                               CrmCommentStateID = crm.CrmCommentStateID,
                                                               CrmCommentStateIDOrder = crm.CrmCommentStateID == 1 || crm.CrmCommentStateID == 2 || crm.CrmCommentStateID == 3 ? 1 : 0,
                                                               _Comments = (from co in crm.Comments.OrderByDescending(m => m.CommentDate)
                                                                            select new _Comment()
                                                                            {
                                                                                Submitter = co.Submitter,
                                                                                CommentDate = co.CommentDate,
                                                                                CRMID = co.CompanyRelationshipID,
                                                                                Contents = co.Contents
                                                                            })
                                                         }).OrderByDescending(cr => cr.CrmCommentStateIDOrder)
                                            }
                           };
           return data;
        }
        static IQueryable<_CoreLVL> GetPublicCRM(bool memberonly, CRMFilters filters = null)
        {
            if (filters.ProjectId == null) filters.ProjectId = 26;
            var crms = from c in CH.DB.CompanyRelationships where c.ProjectID == filters.ProjectId select c;

            if (memberonly)
            {
                crms = crms.Where(w => w.Members.Count > 0);
            }
            else
            {
                //if (Employee.CurrentRole.Level == SalesRequired.LVL || Employee.CurrentRole.Level == LeaderRequired.LVL)
                //    crms = crms.Where(w => w.Members.Any(m => m.Name == Employee.CurrentUserName)==false).OrderBy(w => w.ID);
                //else if (Employee.CurrentRole.Level == ManagerRequired.LVL)
                    crms = crms.Where(w => w.Members.Count == 0);
                
            }
            //模糊搜索
            if (filters != null && !string.IsNullOrWhiteSpace(filters.FuzzyQuery))
            {
                crms = crms.Where(q => q.Company.Leads.Any(l => l.Name_CH.Contains(filters.FuzzyQuery) || l.Name_EN.Contains(filters.FuzzyQuery) || l.EMail.Contains(filters.FuzzyQuery) || l.PersonalEmailAddress.Contains(filters.FuzzyQuery)) || q.Company.Name_CH.Contains(filters.FuzzyQuery) || q.Company.Name_EN.Contains(filters.FuzzyQuery) || q.Company.Contact.Contains(filters.FuzzyQuery));
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
            var data = from c in CH.DB.CoreLVLs
                       select new _CoreLVL()
                       {
                           CoreName = c.CoreLVLName,
                           ID = c.ID,
                           CrmCount = crms.Where(cr => cr.CoreLVLID == c.CoreLVLCode).Count(),
                           _CRMs = (from crm in crms.Where(cr => cr.CoreLVLID == c.CoreLVLCode)
                                    select new _CRM
                                    {
                                        ID = crm.ID,
                                        CompanyNameCH = crm.Company.Name_CH,
                                        CompanyNameEN = crm.Company.Name_EN,
                                        CoreCompany = c.CoreLVLName == "核心公司" ? true : false,
                                        ContectedLeadCount = crm.LeadCalls.GroupBy(call => call.LeadID).Count(),
                                        LeadCount = CH.DB.Leads.Where(l => l.CompanyID == crm.CompanyID).Count(),
                                        CrmCommentStateID = crm.CrmCommentStateID,
                                        CrmCommentStateIDOrder= crm.CrmCommentStateID==1 || crm.CrmCommentStateID==2 || crm.CrmCommentStateID==3?1:0,
                                        _Comments = (from co in crm.Comments.OrderByDescending(m => m.CommentDate)
                                                        select new _Comment()
                                                        {
                                                            Submitter = co.Submitter,
                                                            CommentDate = co.CommentDate,
                                                            CRMID = co.CompanyRelationshipID,
                                                            Contents = co.Contents
                                                        })
                                    }).OrderByDescending(cr=>cr.CrmCommentStateIDOrder)
                       };

            return data;
        }
        public static _CRM _CRMGetAvaliableCrmDetail(int? crmid)
        {
            var data = CH.GetDataById<CompanyRelationship>(crmid);
            bool hasvalue = false;
            var callsgrp = data.LeadCalls.OrderByDescending(c => c.CallDate).GroupBy(c => c.LeadID).Where(g => g.Count() >= 1).Select(g => g.ElementAt(0));
           //覆盖率 = 以打lead数/总数 *１００％
           // 不同时差数，是ｌｅａｄ所在ｌｅａｄｄｉｓｃｔｉｎｃｔｉｄ有几个不同
           //  个人类型数量是个ｌｉｓｔ
           // 各个ｃａｌｌｔｙｐｅ的数量


            var crm = new _CRM()
            {
                CategoryEdit = (Employee.AsManager() || Employee.AsProductInterface()),
                ID = data.ID,
                CompanyID = data.CompanyID,
                ProgressID=data.ProgressID,
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
                CategoryString = data.CategoryString,
                CoreCompany = data.CoreLVL == null ? false : data.CoreLVL.CoreLVLName == "核心公司" ? true : false,
                CrmCommentStateID = data.CrmCommentStateID,
                CoreLVLID = data.CoreLVLID,
                CrmStatisitcs = new _CrmStatisitcs()
                                {
                                    LeadCount = data.Company.Leads.Count(),
                                    CallCount = data.LeadCalls.Count(),
                                    LeadMaxCallCount = data.LeadCalls.GroupBy(lc => lc.LeadID).Select(k => new { Name = k.Key, Count = k.Count() }).OrderByDescending(m => m.Count).FirstOrDefault() != null ? data.LeadCalls.GroupBy(lc => lc.LeadID).Select(k => new { Name = k.Key, Count = k.Count() }).OrderByDescending(m => m.Count).FirstOrDefault().Count : 0,
                                    LeadAvgCallCount = data.Company.Leads.Count != 0 ? (double)data.LeadCalls.Count() / (double)data.Company.Leads.Count() : 0,
                                    CoverageRate = data.Company.Leads.Count != 0 ? ((double)callsgrp.Count() / (double)data.Company.Leads.Count()) * 100 : 0,
                                    TimeDiffer = data.Company.Leads.GroupBy(l => l.DistrictNumberID).Count(),
                                    LeadCalledCount = data.LeadCalls.GroupBy(lc => lc.LeadID).Count(),
                                    
                                    CallTypeCounts = from grp in data.LeadCalls.GroupBy(lc => lc.LeadCallTypeID)
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
                Description = data.Description,
                Competitor = data.Company.Competitor,
                PitchPoint = data.PitchedPoint,
                _Leads = (from leads in data.Company.Leads
                          select new _Lead()
                          {
                              ID = leads.ID,
                              CompanyID = leads.CompanyID,
                              Name = leads.Name_CH + " " + leads.Name_EN,
                              SName = (leads.Name_EN == "" ? leads.Name_CH : leads.Name_EN),
                              Title = leads.Title,
                              Contact = leads.Mobile,
                              Fax = leads.Fax,
                              TelePhone = leads.Contact,
                              Email = leads.EMail,
                              LastCallTypeID = data.LeadCalls.Where(c => c.LeadID == leads.ID).OrderByDescending(c => c.CallDate).FirstOrDefault() == null ? 0 : data.LeadCalls.Where(c => c.LeadID == leads.ID).OrderByDescending(c => c.CallDate).FirstOrDefault().LeadCallTypeID,
                          }),
                _LeadCalls = (from leadcalls in data.LeadCalls.OrderByDescending(m => m.CallDate)
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
            crm._Leads= crm._Leads.OrderByDescending(l => l.LastCallTypeID);
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
                CrmCommentStateID=data.CrmCommentStateID,
                CoreLVLID = data.CoreLVLID,
                _members = data.Members,
                CrmStatisitcs = new _CrmStatisitcs()
                {
                    LeadCount = data.Company.Leads.Count(),
                    CallCount = data.LeadCalls.Count(),
                    LeadMaxCallCount = data.LeadCalls.GroupBy(lc => lc.LeadID).Select(k => new { Name = k.Key, Count = k.Count() }).OrderByDescending(m => m.Count).FirstOrDefault() != null ? data.LeadCalls.GroupBy(lc => lc.LeadID).Select(k => new { Name = k.Key, Count = k.Count() }).OrderByDescending(m => m.Count).FirstOrDefault().Count : 0,
                    LeadAvgCallCount = data.Company.Leads.Count != 0 ? (double)data.LeadCalls.Count() / (double)data.Company.Leads.Count() : 0,
                    CoverageRate = data.Company.Leads.Count != 0 ? ((double)callsgrp.Count() / (double)data.Company.Leads.Count()) * 100 : 0,
                    TimeDiffer = data.Company.Leads.GroupBy(l => l.DistrictNumberID).Count(),
                    LeadCalledCount = data.LeadCalls.GroupBy(lc => lc.LeadID).Count(),

                    CallTypeCounts = from grp in data.LeadCalls.GroupBy(lc => lc.LeadCallTypeID)
                                     select new CallTypeCount()
                                     {
                                         TypeName = CH.GetDataById<LeadCallType>(grp.Key).Name,
                                         Count = grp.Count()
                                     }
                },
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
                                 Contents = co.Contents
                             }),
                Description = data.Description,
                Competitor = data.Company.Competitor,
                PitchPoint = data.PitchedPoint,
                _Leads = (from leads in data.Company.Leads
                          select new _Lead()
                          {
                              ID = leads.ID,
                              Name = leads.Name_CH + " " + leads.Name_EN,
                              SName = (leads.Name_EN == "" ? leads.Name_CH : leads.Name_EN),
                              Title = leads.Title,
                              Contact = leads.Mobile,
                              Fax = leads.Fax,
                              TelePhone=leads.Contact,
                              Email = leads.EMail,
                              LastCallTypeID = data.LeadCalls.Where(c => c.LeadID == leads.ID).OrderByDescending(c => c.CallDate).FirstOrDefault() == null ? 0 : data.LeadCalls.Where(c => c.LeadID == leads.ID).OrderByDescending(c => c.CallDate).FirstOrDefault().LeadCallTypeID
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
                                  LeadCallTypeID = leadcalls.LeadCallTypeID,
                                  MemberName=leadcalls.Member.Name
                              }),
                _ProgressTrack = CH.DB.ProgressTrack.Where(pt => pt.CompanyRelationshipID == crmid).OrderByDescending(pt => pt.ChangeDate)

            };
            crm._Leads = crm._Leads.OrderByDescending(l => l.LastCallTypeID);
            return crm;
        }
        public static _CRM _CRMGetAvaliableCrmDetailByCrmIDMember(int crmid, string membername)
        {
            var data = CH.GetDataById<CompanyRelationship>(crmid);
            var callsgrp = data.LeadCalls.OrderByDescending(c => c.CallDate).GroupBy(c => c.LeadID)
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
                _members=data.Members,
                CrmStatisitcs = new _CrmStatisitcs()
                {
                    LeadCount = data.Company.Leads.Count(),
                    CallCount = data.LeadCalls.Where(cc => cc.Member.Name == membername).Count(),
                    LeadMaxCallCount = data.LeadCalls.Where(cc => cc.Member.Name == membername).GroupBy(lc => lc.LeadID).Select(k => new { Name = k.Key, Count = k.Count() }).OrderByDescending(m => m.Count).FirstOrDefault()!=null?data.LeadCalls.Where(cc => cc.Member.Name == membername).GroupBy(lc => lc.LeadID).Select(k => new { Name = k.Key, Count = k.Count() }).OrderByDescending(m => m.Count).FirstOrDefault().Count:0,
                    LeadAvgCallCount = data.Company.Leads.Count != 0 ? (double)data.LeadCalls.Where(cc => cc.Member.Name == membername).Count() / (double)data.Company.Leads.Count() : 0,
                    CoverageRate = data.Company.Leads.Count != 0 ? (double)callsgrp.Count() / (double)data.Company.Leads.Count()*100 : 0,
                    TimeDiffer = data.Company.Leads.GroupBy(l => l.DistrictNumberID).Count(),
                    LeadCalledCount = data.LeadCalls.Where(cc => cc.Member.Name == membername).GroupBy(lc => lc.LeadID).Count(),
                    CallTypeCounts = from grp in data.LeadCalls.Where(cc => cc.Member.Name == membername).GroupBy(lc => lc.LeadCallTypeID)
                                     select new CallTypeCount()
                                     {
                                         TypeName = CH.GetDataById<LeadCallType>(grp.Key).Name,
                                         Count = grp.Count()
                                     },
                    crmtracks = CH.DB.CrmTracks.Where(tr => tr.CompanyRelationshipID == crmid && tr.Owner == membername).OrderByDescending(tr=>tr.GetDate)
                },
                _Categorys = (from c in data.Categorys
                              select new _Category()
                              {
                                  Name = c.Name,
                                  Details = c.Details,
                                  Description = c.Description
                              }
                                 ),
                CategoryString = data.CategoryString,
                _Comments = (from co in data.Comments.OrderByDescending(m => m.CommentDate)
                             select new _Comment()
                             {
                                 Submitter = co.Submitter,
                                 CommentDate = co.CommentDate,
                                 CRMID = co.CompanyRelationshipID,
                                 Contents = co.Contents
                             }),
                Description = data.Description,
                Competitor = data.Company.Competitor,
                PitchPoint = data.PitchedPoint,
                _Leads = (from leads in data.Company.Leads
                          select new _Lead()
                          {
                              ID = leads.ID,
                              Name = leads.Name_CH + " " + leads.Name_EN,
                              SName = (leads.Name_EN == "" ? leads.Name_CH : leads.Name_EN),
                              Title = leads.Title,
                              Contact = leads.Mobile,
                              Fax = leads.Fax,
                              TelePhone = leads.Contact,
                              Email = leads.EMail,
                              LastCallTypeID = data.LeadCalls.Where(c => c.LeadID == leads.ID).OrderByDescending(c => c.CallDate).FirstOrDefault() == null ? 0 : data.LeadCalls.Where(c => c.LeadID == leads.ID).OrderByDescending(c => c.CallDate).FirstOrDefault().LeadCallTypeID
                          }),
                _LeadCalls = (from leadcalls in data.LeadCalls.OrderByDescending(m => m.CallDate)
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
                                  MemberName= leadcalls.Member.Name
                              }),
                _ProgressTrack = CH.DB.ProgressTrack.Where(pt => pt.CompanyRelationshipID == crmid).OrderByDescending(pt=>pt.ChangeDate)

            };
            crm._Leads = crm._Leads.OrderByDescending(l => l.LastCallTypeID);
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