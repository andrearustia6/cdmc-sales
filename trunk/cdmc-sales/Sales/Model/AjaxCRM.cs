using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entity;
using System.ComponentModel.DataAnnotations;
using Utl;

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
        public List<Category> CompanyCategories { private get; set; }
        public string CompanyCategoryStrings
        {
            get
            {
                return String.Join(", ", CompanyCategories.Select(s => s.Name).ToArray());
            }
        }
        public int CRMID { get; set; }
        public DateTime? CrmCreateDate { get; set; }
        #endregion

        #region company
        public string CompanyNameCH { get; set; }
        public string CompanyNameEN { get; set; }
        public int LeadsCount{ get; set; }
        public string CompanyName { get { return Utl.Utl.GetFullName(CompanyNameCH,CompanyNameEN); } }
        public int CompanyID { get; set; }
        public string CompanyContact {get;set;}
        public string CompanyFax{get;set;}
        //public IEnumerable<String> LeadsName { get; set; }
        public DateTime? CompanyCreateDate { get; set; }
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
        public DateTime? LeadCreateDate { get; set; }
        public int CallsCount { get; set;}
        public string Status
        {
            get
            {
                var status = " 公司共有" + LeadsCount + "人";
                var calltimes = Calls.Where(w => w.LeadID == LeadID);
                
                if (calltimes.Count() > 0)
                {
                    status += ", 此人已打" + calltimes.Count() + "次";
                    status += "，最后通话状态为" + Calls.LastOrDefault().LeadCallType.Name;
                    var haspitch = calltimes.Where(w=>w.LeadCallType.Code>30);
                    if (haspitch.Count() > 0)
                    {
                        status += ", 此客户已经Pitched过了";
                    }
                    else
                    {
                        status += ", 此客户还未Pitched";
                    }
                }
                else
                {
                    status += "此客户尚未联系";
                }

                var otherpitchced = Calls.Where(w => w.LeadID != LeadID && w.LeadCallType.Code > 30);

                if (otherpitchced.Count() > 0 && LeadsCount > 1)
                {
                    var pitchedleads = otherpitchced.Select(s => s.Lead.Name).Distinct();
                    var pitchedleadsname = string.Join(",", pitchedleads);

                    status += ", 此外，此公司的客户" + pitchedleadsname + "也Pitched过了";

                    string allleads = string.Join(",",CH.DB.Leads.Where(w=>w.CompanyID==CompanyID).ToList().Select(s=>s.Name).Distinct() );
                    foreach (var pl in pitchedleads)
                    {
                        allleads = allleads.Replace(pl, "");
                    }
                    allleads = allleads.Replace(LeadName, "");
                    allleads = allleads.Replace(",,", ",");//没有pitched过的

                    var checklast = allleads.Replace(",", "");
                    if (!string.IsNullOrEmpty(checklast))
                    {

                        var cleannopitchedlead = string.Join(",", allleads.Split(new string[]{","}, StringSplitOptions.RemoveEmptyEntries));

                        status += "，但是，还没有Pitched过的Lead有" + cleannopitchedlead;
                    }
                }
                return status;
            }
        }
        
        public List<LeadCall> Calls { private get;set; }
       
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
        //public int AjaxCallsCount { get{return AjaxCalls.Count();}}
        public IEnumerable<AjaxCall> AjaxCalls { get; set; }
        #endregion
    }

    public class AjaxCall 
    {
        public int LeadCallID { get; set; }
        public int LeadID { get; set; }
        public int CompanyID { get; set; }
        [Display(Name="致电结果")]
        public string Result { get; set; }
        [Display(Name = "致电类型")]
        public string CallType { get; set; }
        [Display(Name = "致电日期")]
        public DateTime? CallDate { get; set; }
        [Display(Name = "预约CallBack")]
        public DateTime? CallBackDate { get; set; }
        [Display(Name = "致电人")]
        public string Caller{ get; set; }
        public bool Editable { get; set; }

    }
}