using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entity;

namespace Sales.Model
{
    public class _Category : EntityBase
    {
        public string Name{get;set;}
        public string PitchPoint {get;set;}
        public int? CRMID { get; set; }
    }
    public class _LeadCall : EntityBase
    {
        public string LeadName { get; set; }
        public string LeadTitle { get; set; }
        public string CallResult { get; set; }
        public string CallType { get; set; }
    }

    public class _Lead : EntityBase
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Department { get; set; }
        public string LastCallType { get; set; }
    }

    public class _Comment : EntityBase
    {
        public string Person { get; set; }
        public string Content { get; set; }
        public DateTime SetDate { get; set; }
        public int? CRMID { get; set; }
    }

  
    public class _CRM : EntityBase
    {
         public string Contact { get; set; }
         public string Fax { get; set; }
         public bool CoreCompany { get; set; }
         //点评，回应，通过
         public string CrmCommentState { get; set; }
         public bool? _CoreCompany { get; set; }
         public string CompanyName { get { return string.Join(CompanyNameEN, CompanyNameCH); } }
         public string CompanyNameCH { get; set; }
         public string CompanyNameEN { get; set; }
         public int LeadCount {get;set;}
         public int ContectedLeadCount{get;set;}
         public string DisplayName {get{return CompanyName + "("+ContectedLeadCount+"/"+LeadCount+")";}}
         public string Contacts { get; set; }
         public string Email { get; set; }
         public int BlowedCount { get; set; }
         public int PitchCount { get; set; }
         public int FullPitchCount { get; set; }
         public int CloseDealCount { get; set; }
         public int WaitForApproveCount { get; set; }
         public int NoCallCount { get; set; }
         public IQueryable<_Category> _Categorys { get; set; }
         public string Description { get; set; }
         public string PitchPoint { get; set; }
         public IQueryable<_Lead> _Leads { get; set; }
         public IQueryable<_Comment> _Comments { get; set; }
         public IQueryable<_LeadCall> _LeadCalls { get; set; }
     }


    public class _AvaliableCompanies : EntityBase
    {
        //领用公司
        public IQueryable<_CoreLVL> MemberCompanies { get; set; }
        //公海公司
        public IQueryable<_CoreLVL> PublicCompanies { get; set; }
    }

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
    }
    public class _Maturity : EntityBase
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public string DisplayName { get { return Name + "("+Count+")";} }
        public IEnumerable<_CRM> _CRMs { get; set; }
    }
}