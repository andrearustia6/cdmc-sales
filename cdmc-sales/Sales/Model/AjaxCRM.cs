using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entity;
using System.ComponentModel.DataAnnotations;
using Utl;

namespace Model
{
    public class AjaxCrmTypedList
    {
        public IQueryable<AjaxCRM> AllCRMs { get; set; }
        public IQueryable<AjaxGroupedCRM> CustomCrmGroups { get; set; }
    }

    public class AjaxGroupedCRM
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public IQueryable<AjaxCRM> GroupedCRMs { get; set; }
    }

    public class AjaxCRM
    {
        #region projecy
        public string ProjectName { get; set; }
        public int? ProjectID { get; set; }
        #endregion

        #region crm
        public Progress Progress { private get; set; }
        public string ProgressString
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

        // public string Status
        //{
        //    get
        //    {
        //        var status = string.Empty;
        //        var contactedlead = AjaxLeads.Where(w=>w.AjaxCalls.)
        //    }
        public string DisplayText
        {
            get
            {
                List<string> v = new List<string>();
                v.Add(CompanyName);
                if(!string.IsNullOrEmpty(ProgressString))
                {
                    v.Add(ProgressString);
                };
                var nocontactleadcount =  AjaxLeads.Count(c => c.AjaxCalls.Count() == 0);
                if (nocontactleadcount > 0)
                {
                    v.Add(nocontactleadcount.ToString() + "Leads未打");
                }
                return string.Join(",", v);
            }
        }
        public IEnumerable<AjaxLead> AjaxLeads { get; set; }
    }

    public class AjaxLead
    {
        #region lead
        public string HasBlowed { get; set; }
        public string LeadName { get { return Utl.Utl.GetFullName(LeadNameCH, LeadNameEN); } }
        public string LeadNameCH { get; set; }
        public string LeadNameEN { get; set; }
        public string LeadFax { get; set; }
        public int CRMID { get; set; }
        public int LeadID { get; set; }
        public string LeadTitle { get; set; }
        public DateTime? LeadCreateDate { get; set; }
        public int CallsCount { get; set; }
        public string DisplayText
        {
            get
            {
                var lastcall = "未打";
                if (AjaxCalls != null && AjaxCalls.Count() > 0)
                {
                    lastcall = AjaxCalls.OrderByDescending(o => o.CallDate).FirstOrDefault().CallType;
                }

                List<string> v = new List<string> { LeadName };
                if(!string.IsNullOrEmpty(LeadTitle))
                {
                   v.Add(LeadTitle);
                }
                v.Add(lastcall);
                return string.Join(",", v);
            }
        }
        public string Status
        {
            get
            {
             
                 string status = string.Empty;
                var ajaxcallcount = AjaxCalls.Count();
                if (ajaxcallcount > 0)
                {
                    status += "此人已打" + ajaxcallcount + "次";
                    var lastcall = AjaxCalls.OrderByDescending(o=>o.CallDate).FirstOrDefault();
                    status += "，最后通话状态为" + lastcall.CallType;
                    var haspitch = AjaxCalls.Where(w => w.LeadCallTypeCode > 30);
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

           
            
                return status;
            }
        }



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
        public string LeadEmail { get; set; }

        #region calls
        //public int AjaxCallsCount { get{return AjaxCalls.Count();}}
        public IEnumerable<AjaxCall> AjaxCalls { get; set; }


        #endregion

        #endregion
    }

    public class AjaxCall 
    {
       
        public int LeadCallTypeCode { get; set; }
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