using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entity;

namespace Model
{
    public class AjaxCRM
    {
        #region projecy
        public string ProjectName { get; set; }
        public int? ProjectID { get; set; }
        #endregion
        #region crm
        public Progress Progress { private get; set; }
        public string Progressstring
        {
            get
            {
                return Progress == null ? string.Empty : Progress.Name;
            }
        }
        public string CompanyCategories { get; set; }
        public int CRMID { get; set; }
        #endregion

        #region company
        public string CompanyNameCH { get; set; }
        public string CompanyNameEN { get; set; }
        public string CompanyName { get { return Utl.Utl.GetFullName(CompanyNameCH,CompanyNameEN); } }
        public int CompanyID { get; set; }
        public string CompanyContact {get;set;}
        public string CompanyFax{get;set;}

        public string CompanyDistrictNumberString
        {
            get
            {
                return CompanyDistinct == null ? string.Empty : CompanyDistinct.Name;
            }
        }

        public DistrictNumber CompanyDistinct { set; private get; }
        #endregion

        #region lead
        public string HasBlowed {get;set;}
        public string LeadName { get {return Utl.Utl.GetFullName(LeadNameCH,LeadNameEN);} }
        public string LeadNameCH { get; set; }
        public string LeadNameEN { get; set; }
        public string LeadFax { get; set; }
        public int LeadID { get; set; }
        public string LeadTitle { get; set; }

        public string LeadDistinctNumberString
        {
            get
            {
                return LeadDistinct == null ? string.Empty : LeadDistinct.Name;
            }
        }
        public DistrictNumber LeadDistinct { set; private get; }
        public string LeadContact { get; set; }
        public string LeadMobile { get; set; }
        public string LeadEmail {get;set;}
        #endregion

        #region calls
        public IEnumerable<AjaxCall> AjaxCalls { get; set; }
        #endregion
    }

    public class AjaxCall 
    {
        public int LeadCallID { get; set; }
        public int LeadID { get; set; }
        public int CompanyID { get; set; }
        public string Result { get; set; }
        public string CallType { get; set; }
        public DateTime? CallDate { get; set; }
        public DateTime? CallBackDate { get; set; }
        public string Caller{ get; set; }

    }
}