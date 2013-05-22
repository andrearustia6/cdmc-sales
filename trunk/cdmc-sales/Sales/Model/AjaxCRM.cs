﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entity;
using System.ComponentModel.DataAnnotations;
using Utl;

namespace Model
{
    public class CompanyFilters
    {
        public int? CompanyAssignDate { get; set; }
        public int? CompanyProgress { get; set; }
        public int? ProjectId { get; set; }
        public int? LeadProgress { get; set; }
        public int? DealProgress { get; set; }
        public int? CallProgress { get; set; }
        public int? CategoryId { get; set; }
        public bool? Unfold { get; set; }
        public string FuzzyQuery { get; set; }

        public string selectedVal { get; set; }
    }

    public class AjaxCrmTypedList
    {
        public bool ExpandAll
        {
            get
            {
                bool retVal = false;
                if (this.Filters != null && this.Filters.Unfold.HasValue && this.Filters.Unfold.Value == true)
                {
                    retVal = true;
                }
                if (this.Filters != null && !string.IsNullOrWhiteSpace(Filters.FuzzyQuery))
                {
                    retVal = true;
                }

                return retVal;
            }
        }
        public int? CurrentProjectId { get; set; }
        IQueryable<CompanyRelationship> GetFilteredCRM()
        {
            string user = Employee.CurrentUserName;
            var query = from c in CH.DB.CompanyRelationships.Where(w => w.Members.Select(s=>s.Name).Contains(user)) select c;
            //模糊搜索
            if (Filters != null && !string.IsNullOrWhiteSpace(Filters.FuzzyQuery))
            {
                query = query.Where(q => q.Company.Leads.Any(l => l.Name_CH.Contains(Filters.FuzzyQuery) || l.Name_EN.Contains(Filters.FuzzyQuery) || l.EMail.Contains(Filters.FuzzyQuery) || l.PersonalEmailAddress.Contains(Filters.FuzzyQuery)) || q.Company.Name_CH.Contains(Filters.FuzzyQuery) || q.Company.Name_EN.Contains(Filters.FuzzyQuery) || q.Company.Contact.Contains(Filters.FuzzyQuery));
            }

            //项目
            if (Filters != null && Filters.ProjectId.HasValue)
            {
                query = query.Where(q => q.ProjectID == Filters.ProjectId);
            }
            //分配时间
            if (Filters != null && Filters.CompanyAssignDate.HasValue)
            {
                var date = DateTime.Now.AddDays(-Filters.CompanyAssignDate.Value);
                query = query.Where(q => q.CreatedDate.Value >= date);
            }
            //成熟度
            if (Filters != null && Filters.CompanyProgress.HasValue)
            {
                query = query.Where(q => q.Progress.Code == Filters.CompanyProgress);
            }
            //出单
            if (Filters != null && Filters.DealProgress.HasValue)
            {
                if (Filters.DealProgress == 0)
                    query = query.Where(q => q.Deals.Count() == 0);
                else if (Filters.DealProgress == 1)
                {
                    query = query.Where(q => (q.Deals.Count() > 0));
                }
                else if (Filters.DealProgress == 2)
                {
                    query = query.Where(q => q.Deals.Any(d => d.IsClosed == false));
                }
                else if (Filters.DealProgress == 3)
                {
                    query = query.Where(q => q.Deals.Any(d => d.IsClosed == true && d.Income > 0));
                }

            }
            //lead 进展
            if (Filters != null && Filters.LeadProgress.HasValue)
            {
                if (Filters.LeadProgress == 1)
                    query = query.Where(q => (q.Company.Leads.Count == q.LeadCalls.Select(s => s.LeadID).Distinct().Count()) && q.LeadCalls.Count>0 && q.LeadCalls.All(a => a.LeadCallType.Code == 20));
                else if (Filters.LeadProgress == 3)
                    query = query.Where(q => q.Company.Leads.Count > q.LeadCalls.Select(s => s.LeadID).Distinct().Count());
                else if (Filters.LeadProgress == 5)
                {
                    var datestart = DateTime.Now.AddDays(-2);
                    var dateend = DateTime.Now.AddDays(2);
                    query = query.Where(q => q.LeadCalls.Any(a => a.CallBackDate >= datestart && a.CallBackDate <= dateend));
                }
            }
            //leadcall date
            if (Filters != null && Filters.CallProgress.HasValue)
            {
                var now = DateTime.Now;
                var day = now.AddDays(-Filters.CallProgress.Value);

                query = query.Where(q => q.LeadCalls.Any(a => a.CallDate >= day));

            }

            if (Filters != null && Filters.CategoryId.HasValue)
            {
                query = query.Where(q => q.Categorys.Any(c => c.ID == Filters.CategoryId.Value));
            }
            return query;
        }

        private IQueryable<AjaxGroupedCRM> GetCustomCrmGroupDataQuery()
        {
            var user = Employee.CurrentUserName;
            var filteredcrm = GetFilteredCRM();
            var data = from f in CH.DB.UserFavorsCrmGroups
                       where f.UserName == user
                       select new AjaxGroupedCRM()
                       {
                           ID = f.ID,
                           UserName = f.UserName,
                           DisplayName = f.DisplayName,
                           GroupedCRMs = (
                                         
                                          from c in filteredcrm
                                          where 
                                          //c.ID == u.CompanyRelationshipID.Value
                                           f.UserFavorsCRMs.Select(s => s.CompanyRelationshipID).Contains(c.ID)
                                          orderby c.CreatedDate descending
                                          select new AjaxCRM
                                          {
                                              CompanyID = c.CompanyID,
                                              CompanyNameEN = c.Company.Name_EN,
                                              CompanyNameCH = c.Company.Name_CH,
                                              CRMID = c.ID,
                                              AjaxLeads = (from l in c.Company.Leads
                                                           select new AjaxLead
                                                           {
                                                               LeadNameCH = l.Name_CH,
                                                               LeadNameEN = l.Name_EN,
                                                               LeadTitle = l.Title,
                                                               CRMID = c.ID,
                                                               LeadID = l.ID,
                                                               AjaxCalls = (from call in c.LeadCalls.Where(w => w.LeadID == l.ID)
                                                                            select new AjaxCall
                                                                            {
                                                                                CallDate = call.CallDate,
                                                                                CallBackDate = call.CallBackDate,
                                                                                CallType = call.LeadCallType.Name,
                                                                                Caller = call.Member.Name,
                                                                                LeadCallTypeCode = call.LeadCallType.Code
                                                                            })
                                                           })

                                          }
                                          )

                       };
            return data;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="alldata">true 为选择所有数据</param>
        /// <returns></returns>
        IQueryable<AjaxCRM> GetCrmDataQuery(bool? alldata = false)
        {

            var user = Employee.CurrentUserName;
            var custom = from cg in CH.DB.UserFavorsCRMs.Where(w => w.UserFavorsCrmGroup.UserName == user) select cg;
             var filteredcrm = GetFilteredCRM();
            var data = from c in filteredcrm
                       where  (!custom.Select(s => s.CompanyRelationshipID).Contains(c.ID))
                       orderby c.CreatedDate descending
                       select new AjaxCRM
                       {
                           ProjectID = c.ProjectID,
                           CompanyID = c.CompanyID,
                           CompanyNameEN = c.Company.Name_EN,
                           CompanyNameCH = c.Company.Name_CH,
                           CRMID = c.ID,
                           ProgressID = c.ProgressID,
                           AreaID = c.Company.AreaID,
                           CompanyTypeID = c.Company.CompanyTypeID,
                           AjaxLeads = (from l in c.Company.Leads
                                        select new AjaxLead
                                        {
                                            LeadNameCH = l.Name_CH,
                                            LeadNameEN = l.Name_EN,
                                            LeadTitle = l.Title,
                                            CRMID = c.ID,
                                            LeadID = l.ID,
                                            AjaxCalls = (from call in c.LeadCalls.Where(w => w.LeadID == l.ID)
                                                                            select new AjaxCall
                                                                            {
                                                                                CallDate = call.CallDate,
                                                                                CallBackDate = call.CallBackDate,
                                                                                CallType = call.LeadCallType.Name,
                                                                                Caller = call.Member.Name,
                                                                                LeadCallTypeCode = call.LeadCallType.Code
                                                                            })
                                        })

                       };
            string v = data.ToString();
            return data;
        }


        public CompanyFilters Filters { get; set; }
        public AjaxCrmTypedList(CompanyFilters filters)
        {
            Filters = filters;
            AllCRMs = GetCrmDataQuery();
            CustomCrmGroups = GetCustomCrmGroupDataQuery();
        }

        public IQueryable<AjaxCRM> AllCRMs { get; set; }
        public IQueryable<AjaxGroupedCRM> CustomCrmGroups { get; set; }
    }

    public class AjaxGroupedCRM
    {
        public int ID { get; set; }
        [Display(Name = "自定义分组所属用户")]
        public string UserName { get; set; }
        [Display(Name = "自定义分组名称")]
        public string DisplayName { get; set; }
        public IQueryable<AjaxCRM> GroupedCRMs { get; set; }

        [Display(Name = "描述")]
        public string Description { get; set; }
        [Display(Name = "排序")]
        public int? Sequence { get; set; }

        [Display(Name = "更改用户")]
        public string ModifiedUser { get; set; }

        [Display(Name = "更改时间")]
        public DateTime? ModifiedDate { get; set; }

        [Display(Name = "创建用户")]
        public string Creator { get; set; }

        [Display(Name = "创建时间")]
        public DateTime? CreatedDate { get; set; }
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
                if (CompanyCategories == null) return string.Empty;
                return String.Join(", ", CompanyCategories.Select(s => s.Name).ToArray());
            }
        }
        public int CRMID { get; set; }
        public DateTime? CrmCreateDate { get; set; }
        #endregion

        #region company
        [Required(ErrorMessage = " ")]
        [Display(Name = "中文名字")]
        public string CompanyNameCH { get; set; }
        [Display(Name = "英文名字")]
        public string CompanyNameEN { get; set; }
        public int LeadsCount { get; set; }
        public string CompanyName { get { return Utl.Utl.GetFullString(",",CompanyNameCH, CompanyNameEN); } }
        public string CompanyDisplayName { get { return Utl.Utl.GetFullString(" ", CompanyNameCH, CompanyNameEN, CategoryString, ProgressString); } }
        public int? CompanyID { get; set; }
        [Display(Name = "公司总机")]
        public string CompanyContact { get; set; }
        [Display(Name = "公司传真")]
        public string CompanyFax { get; set; }
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
              
                var nocontactleadcount = AjaxLeads.Count(c => c.AjaxCalls.Count() == 0);
                if (nocontactleadcount > 0)
                {
                    return Utl.Utl.GetFullString(", ", CompanyName, nocontactleadcount.ToString() + "人未打");
                }
                else
                {
                var name = string.IsNullOrEmpty(CompanyName) ? "未填" : CompanyName;
                return name;
              }
            }
        }
        public IEnumerable<AjaxLead> AjaxLeads { get; set; }

        //Added for Company Edit.
        [Display(Name = "区号/时差")]
        public int? DistrictNumberID { get; set; }
        [Required(ErrorMessage = " ")]
        [Display(Name = "成熟度")]
        public int? ProgressID { get; set; }
        [Display(Name = "行业类型")]
        public int? AreaID { get; set; }
        [Display(Name = "公司类型")]
        public int? CompanyTypeID { get; set; }
        [Required(ErrorMessage = " ")]
        [Display(Name = "公司邮编")]
        public string ZipCode { get; set; }
        [Display(Name = "公司网站")]
        public string WebSite { get; set; }
        [Display(Name = "公司地址")]
        public string Address { get; set; }
        [Display(Name = "主营业务")]
        public string Business { get; set; }
        [Display(Name = "公司业务")]
        public string Desc { get; set; }
        [Display(Name = "分类")]
        public IEnumerable<int> Categories { get; set; }
        [Display(Name = "分类")]
        public string CategoryString { get; set; }
    }

    public class AjaxLead
    {
        //最后通话状态代码
        public int? LastCallCode { get; set; }

        #region lead
        public string HasBlowed { get; set; }
        public string Gender { get; set; }
        public string LeadName { get { return Utl.Utl.GetFullName(LeadNameCH, LeadNameEN); } }

        public string LeadShowName
        {
            get
            {


                return Utl.Utl.GetFullString(" ", Gender, LeadNameCH, LeadNameEN, LeadTitle);
            }
        }






        public string LeadNameCH { get; set; }
        public string LeadNameEN { get; set; }
        public string LeadPersonalEmail { get; set; }
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
                var lastcall = "&0*";
                if (AjaxCalls != null && AjaxCalls.Count() > 0)
                {
                    var code = AjaxCalls.OrderByDescending(o => o.CallDate).FirstOrDefault().LeadCallTypeCode;
                    if (code == 20)
                        lastcall = "&2*";//blowed
                    if (code == 40 || code == 50)
                        lastcall = "&4*";//pitched
                    else if (code == 60)
                        lastcall = "&6*";//wait for approve
                    else if (code == 90)
                        lastcall = "&9*";//wait for approve
                    else if (code == 80)
                        lastcall = "&8*";//Qualified Decision
                    else if (code == 70)
                        lastcall = "&7*";//Waiting for Approval
                    else
                        lastcall = " ";
                }
              
                var leadname = string.IsNullOrEmpty(LeadName) ? "未填" : LeadName;
              
                return Utl.Utl.GetFullString(",", leadname, LeadTitle, lastcall);
            }
        }
        public string Status
        {
            get
            {

                string status = string.Empty;
                //var ajaxcallcount = AjaxCalls.Count();
                //if (ajaxcallcount > 0)
                //{
                //    status += "此人已打" + ajaxcallcount + "次";
                //    var lastcall = AjaxCalls.OrderByDescending(o => o.CallDate).FirstOrDefault();
                //    status += "，最后通话状态为" + lastcall.CallType;
                //    var haspitch = AjaxCalls.Where(w => w.LeadCallTypeCode > 30);
                //    if (haspitch.Count() > 0)
                //    {
                //        status += ", 此客户已经Pitched过了";
                //    }
                //    else
                //    {
                //        status += ", 此客户还未Pitched";
                //    }
                //}
                //else
                //{
                //    status += "此客户尚未联系";
                //}



                return status;
            }
        }
        [Display(Name = "微博账号")]
        public string WeiBo { get; set; }

        [Display(Name = "微信账号")]
        public string WeiXin { get; set; }

        [Display(Name = "LinkIn账号")]
        public string LinkIn { get; set; }

        [Display(Name = "FaceBook账号")]
        public string FaceBook { get; set; }

        [Display(Name = "博客地址")]
        public string Blog { get; set; }

        [Display(Name = "部门")]
        public string Department { get; set; }

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
        public string LeadAddress { get; set; }
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
        [Display(Name = "致电结果")]
        public string Result { get; set; }
        [Display(Name = "致电类型")]
        public string CallType { get; set; }
        [Display(Name = "致电日期")]
        public DateTime? CallDate { get; set; }
        [Display(Name = "预约CallBack")]
        public DateTime? CallBackDate { get; set; }
        [Display(Name = "致电人")]
        public string Caller { get; set; }
        public bool Editable { get; set; }

    }
}