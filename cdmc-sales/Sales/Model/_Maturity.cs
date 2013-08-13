using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sales.Model
{
    public class _Category
    {
        public string Name{get;set;}
        public string PitchPoint {get;set;}
    }


    public class _Lead 
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Department { get; set; }
    }
     public class _CRM
    {
         public string CompanyName{get;set;}
         public int LeadCount {get;set;}
         public int ContectedLeadCount{get;set;}
         public string DisplayName {get{return CompanyName + "("+ContectedLeadCount+"/"+LeadCount+")";}}
         public string Contacts { get; set; }
         public string Email { get; set; }
         public int BlowedCount { get; set; }
         public int PitchCount { get; set; }
         public int FullPitchCount { get; set; }
         public int CloseDealCount { get; set; }
         public int WaitForApprove{ get; set; }
         public int NoCallCount { get; set; }
         public IQueryable<_Category> _Categorys { get; set; }
         public string Description { get; set; }
         public string PitchPoint { get; set; }
         public IQueryable<AjaxLead> _Leads { get; set; }
     }
    public class _Maturity
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public string DisplayName { get { return Name + "("+Count+")";} }
        IQueryable<_CRM> _CRMs { get; set; }
    }
}