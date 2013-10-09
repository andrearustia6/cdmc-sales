﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entity;

namespace Sales.Model
{


    public class _CrmStatistics
    {
        /// <summary>
        /// member筛选
        /// </summary>
        public string MemberFilter { get; set; }

        /// <summary>
        ///  打过的职位
        /// </summary>
        public string ContactTitles { get; set; }
        /// <summary>
        /// 所有时间段
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Lead总数
        /// </summary>
        public int LeadTotalCount { get; set; }

        /// <summary>
        /// 已覆盖lead数
        /// </summary>
        public int CoveredLeadCount { get; set; }

        /// <summary>
        /// lead所在不同时差数
        /// </summary>
        public int DistinctLeadCount { get; set; }


        /// <summary>
        /// 单个lead最大联系次数
        /// </summary>
        public int SingleLeadContactCount { get; set; }


        /// <summary>
        /// 人均lead联系次数
        /// </summary>
        public int AverageLeadContactCount { get; set; }

        /// <summary>
        /// Call的总数
        /// </summary>
        public int CallCount { get; set; }

        /// <summary>
        /// Call类型数量统计表
        /// </summary>
        public IQueryable<CallTypeCount> CallTypeCounts { get; set; }
    }

    public class CallTypeCount
    {
        public string TypeName { get; set; }
        public int Count { get; set; }
    }

    /// <summary>
    /// 细分行业的分类
    /// </summary>
    public class _Category : EntityBase
    {
        public string Name{get;set;}
        public string PitchPoint {get;set;}
        public string Details { get; set; }
        public int? CRMID { get; set; }
    }

    /// <summary>
    /// 拨打的结果
    /// </summary>
    public class _LeadCall : EntityBase
    {
        public string LeadName { get; set; }
        public string LeadTitle { get; set; }
        public string CallResult { get; set; }
        public string CallType { get; set; }
        public DateTime CallDate { get; set; }
        public string Creator { get; set; }
        public int? LeadID { get; set; }
        public int? LeadCallTypeID { get; set; }
        public string MemberName { get; set; }
    }

    /// <summary>
    /// 决策人
    /// </summary>
    public class _Lead : EntityBase
    {
        public int? CompanyID { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Department { get; set; }
        public string LastCallType { get; set; }
        public string Contact { get; set; }
        public string Fax { get; set; }
        public string TelePhone { get; set; }
        public string Email { get; set; }
        public int? LastCallTypeID { get; set; }
    }

    /// <summary>
    /// 点评
    /// </summary>
    public class _Comment : EntityBase
    {
        public string Submitter { get; set; }
        public string Contents { get; set; }
        public DateTime CommentDate { get; set; }
        public int? CRMID { get; set; }
        public int? CrmCommentStateID { get; set; }
    }

    public class _CrmStatisitcs
    {
        /// <summary>
        /// Lead总数
        /// </summary>
        public int LeadCount { get; set; }
        /// <summary>
        /// 覆盖率
        /// </summary>
        public double CoverageRate { get; set; }
        /// <summary>
        /// 不同时差数
        /// </summary>
        public int TimeDiffer { get; set; }
        /// <summary>
        /// 单个Lead最大联系次数
        /// </summary>
        public int LeadMaxCallCount { get; set; }
        /// <summary>
        /// 人均Lead联系次数
        /// </summary>
        public double LeadAvgCallCount { get; set; }
        /// <summary>
        /// Call总数
        /// </summary>
        public int CallCount { get; set; }
      
        /// <summary>
        /// Call类型数量统计表
        /// </summary>
        public IEnumerable<CallTypeCount> CallTypeCounts { get; set; }
    }
  
    /// <summary>
    /// 公司的客户关系
    /// </summary>
    public class _CRM : EntityBase
    {
        public _CrmStatisitcs CrmStatisitcs { get; set; }
         public string Contact { get; set; }
         public string Fax { get; set; }
         public bool CoreCompany { get; set; }
         //点评，回应，通过
         public string CrmCommentState { get; set; }
         public bool? _CoreCompany { get; set; }
         public int? CompanyID { get; set; }
         public string CompanyName { get { return string.Join(",",CompanyNameEN, CompanyNameCH).Trim(','); } }
         public string CompanyNameCH { get; set; }
         public string CompanyNameEN { get; set; }
         public int LeadCount {get;set;}
         public int ContectedLeadCount{get;set;}
         public string DisplayName {get{return CompanyName + "("+ContectedLeadCount+"/"+LeadCount+")";}}
         public string Contacts { get; set; }
         public string Email { get; set; }
         public int? CrmCommentStateID { get; set; }
         public int CrmCommentStateIDOrder
         {
             get;
             set;
         }
         public int? CoreLVLID { get; set; }
         public int BlowedCount
         {
             get;
             set;
         }
         public int NoPitchCount
         {
             get;
             set;
         }
         public int NoPitchBlowedCount
         {
             get
             {
                 return NoPitchCount + BlowedCount;
             }
         }

         public int PitchCount
         {
             get;
             set;
         }
         public int FullPitchCount
         {
             get;
             set;
         }
         public int CallBackedCount
         {
             get;
             set;
         }
         public int TotalPitchCount
         {
             get
             {
                 return PitchCount + FullPitchCount + CallBackedCount;
             }
         }
         public int QualifiedDecisionCount
         {
             get;
             set;

         }
         public int WaitForApproveCount
         {
             get;
             set;
             
         }
         public int QualifiedWaitForCount
         {
             get
             {
                 return WaitForApproveCount + QualifiedDecisionCount;
             }

         }
         public int CloseDealCount {
             get;
             set;
             
         }
         public int NoCallCount
         {
             get;
             set;
             
         }
         public IEnumerable<_Category> _Categorys { get; set; }
         public bool CategoryEdit { get; set; }
         public string CategoryString{ get; set; }
         public string Description { get; set; }
         public string PitchPoint { get; set; }
        /// <summary>
        /// 竞争对手
        /// </summary>
         public string Competitor { get; set; }
         public IEnumerable<_Lead> _Leads { get; set; }
         public IEnumerable<_Comment> _Comments { get; set; }
         public IEnumerable<_LeadCall> _LeadCalls { get; set; }
         public IEnumerable<Member> _members { get; set; }
     }

    /// <summary>
    /// 可打公司左边的导航列表
    /// </summary>
    public class _AvaliableCompanies : EntityBase
    {
        //领用公司
        public IQueryable<_CoreLVL> MemberCompanies { get; set; }
        //公海公司
        public IQueryable<_CoreLVL> PublicCompanies { get; set; }
    }

    /// <summary>
    /// 可打公司的核心级别
    /// </summary>
    public class _CoreLVL : EntityBase
    {
        public string CoreNameDisplayText
        {
            get
            {
                return CoreName + "(" + CrmCount.ToString() + ")";
            }
        }
         public string CoreName { get; set; }
         public IQueryable<_Maturity> _Maturitys { get; set; }
         public int CrmCount { get; set; }

         int? _NoContactCount;
         int? _ContactCount;
         public int NoContactCount
         {
             get
             {
                 if (_NoContactCount == null)
                 { 
                    // _Maturitys.SelectMany(s=>s._CRMs).Count(c=>c.)
                 }
                 return _NoContactCount.Value;
             }
         }
         public int ContactCount
         {
             get
             {
                 if (_ContactCount == null)
                 { }
                 return _ContactCount.Value;
             }
         }
         public IEnumerable<_CRM> _CRMs { get; set; }
    }

    /// <summary>
    /// 成熟度
    /// </summary>
    public class _Maturity : EntityBase
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public string DisplayName { get { return Name + "("+Count+")";} }
        public IEnumerable<_CRM> _CRMs { get; set; }
    }
}