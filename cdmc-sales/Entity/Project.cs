﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Attributes;

namespace Entity
{

    public class CrmCommentState : EntityBase
    {
        public int RoleFlag { get; set; }
        public int StateCode { get; set; }
        public string StateName { get; set; }
    }

    /// <summary>
    /// 领用或者
    /// </summary>
    public class CrmTrack : EntityBase
    {

        public virtual CompanyRelationship CompanyRelationship { get; set; }
        public int?  CompanyRelationshipID { get; set; }
        //如果为公海领用或者自己加， assigner为空
        public string Assigner { get; set; }
        public string Owner { get; set; }
        //分配 or 领用or 自加
        public string Type { get; set; }
        //得到公司的时间
        public DateTime? GetDate { get; set; }
        //释放公司的时间
        public DateTime? ReleaseDate { get; set; }
    }
    public class ProgressTrack : EntityBase
    {
        public virtual CompanyRelationship CompanyRelationship { get; set; }
        public int? CompanyRelationshipID { get; set; }
        public string Owner { get; set; }
        public virtual Progress Progress { get; set; }
        public int? ProgressID { get; set; }
        public DateTime ChangeDate { get; set; }
    }

    public class ExpLevel : EntityBase
    {
        [Display(Name = "级别"), Required]
        public String Name { get; set; }

        [Display(Name = "部门")]
        public virtual Department Department { get; set; }

        [Display(Name = "部门")]
        public int? DepartmentID { get; set; }
    }

    /// <summary>
    /// 调研
    /// </summary>
    public class Research : EntityBase
    {
        [Display(Name = "公司名称"), Required]
        public String Name { get; set; }

        [Display(Name = "内容")]
        public String Contents { get; set; }

        [Display(Name = "公司架构")]
        public virtual Image Image { get; set; }
        public int? ImageID { get; set; }

        public string AddPerson { get; set; }

        public virtual Project Project { get; set; }

        [Display(Name = "项目"), Required]
        public int? ProjectID { get; set; }
    }

    /// <summary>
    /// 职级
    /// </summary>
    public class Role : EntityBase
    {
        public const int LVL_SuperManager = 800;
        public const int LVL_Director = 1000;
        public const int LVL_Manager = 500;
        public const int LVL_TeamLeader = 100;
        public const int LVL_Sales = 10;
        public const int LVL_ImportingInterface = 6;
        public const int LVL_ProductInterface = 5;
        public const int LVL_FinancialInterface = 4;
        public const int LVL_ConferenceInterface = 3;
        public const int LVL_PoliticsInterface = 2;
        public const int LVL_MarketInterface = 1;


        [Display(Name = "职级名称"), Required]
        public String Name { get; set; }

        [Display(Name = "职级分数"), Required]
        public int Level { get; set; }
    }

    /// <summary>
    /// 部门管理
    /// </summary>
    public class Department : EntityBase
    {
        [Display(Name = "部门名称"), Required]
        public String Name { get; set; }

        [Display(Name = "是否激活")]
        public bool IsActivated { get; set; }
    }

    /// <summary>
    /// 部门管理
    /// </summary>
    public class SalesType : EntityBase
    {
        [Display(Name = "销售类别"), Required]
        public String Name { get; set; }
    }

    public class ProjectType : EntityBase
    {
        [Display(Name = "项目类型"), Required]
        public String Name { get; set; }
    }

    /// <summary>
    /// 项目
    /// </summary>
    public class Project : NameEntity
    {
        //唯一名称和唯一编号都是在项目报表统计使用的
        [Display(Name = "项目唯一名称"), Required]
        public string ProjectUnitName { get; set; }
        [Display(Name = "项目唯一编号"), Required]
        public string ProjectUnitCode { get; set; }

        [Display(Name = "项目类型")]
        public virtual ProjectType ProjectType { get; set; }
        [Display(Name = "项目类型")]
        public int? ProjectTypeID { get; set; }
        [Display(Name = "测试数据")]
        public bool? Test { get; set; }

        [Display(Name = "项目属性")]
        public string ProjectState { get; set; }

        [Display(Name = "销售简介文件下载显示名称")]
        public string SalesBriefName { get; set; }

        [Display(Name = "文件路径")]
        public string SalesBriefUrl { get; set; }



        public virtual List<ProjectRight> ProjectRights { get; set; }

        [Display(Name = "事业部负责人")]
        public string Manager { get; set; }

        [Display(Name = "项目编号"), Required]
        public string ProjectCode { get; set; }

        [Display(Name = "市场部接口人")]
        public string Market { get; set; }

        [Display(Name = "会务部接口人")]
        public string Conference { get; set; }

        [Display(Name = "产品部接口人")]
        public string Product { get; set; }

        public virtual List<Member> Members { get; set; }

        [Display(Name = "正在进行"), Required]
        public bool IsActived { get; set; }

        [Display(Name = "项目开始时间"), Required, DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime StartDate { get; set; }

        [Display(Name = "项目结束时间"), Required, DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime EndDate { get; set; }

        [Display(Name = "会议开始时间"), Required, DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime ConferenceStartDate { get; set; }

        [Display(Name = "会议结束时间"), Required, DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime ConferenceEndDate { get; set; }

        [Display(Name = "销售目标"), Required, DisplayFormat(DataFormatString = "{0:c}")]
        public decimal Target { get; set; }

        [Display(Name = "团队负责人(TL)")]
        public string TeamLeader { get; set; }

        public virtual List<Template> Templates { get; set; }

        [Display(Name = "会议地址")]
        public string Location { get; set; }

        [Display(Name = "公司类型")]
        public virtual List<Category> Categorys { get; set; }

        [Display(Name = "公司类型")]
        public string CategorysSet
        {
            get
            {
                if (Categorys != null)
                {
                    var val = string.Empty;
                    Categorys.ForEach(c =>
                    {
                        if (string.IsNullOrEmpty(val))
                            val = c.Name;
                        else
                            val += "|" + c.Name;
                    });
                    return val;
                }
                return string.Empty;
            }
        }

        [Display(Name = "项目背景")]
        public string SaleBrief { get; set; }

        [Display(Name = "其它")]
        public string Others { get; set; }

        public virtual List<News> News { get; set; }

        public virtual List<Message> Messages { get; set; }

        public virtual List<PhoneSaleSupport> PhoneSaleSupports { get; set; }


        /// <summary>
        /// 被参考的项目的crm自动加入本项目的crm
        /// </summary>
        public string References { get; set; }

        public virtual List<TargetOfMonth> TargetOfMonths { get; set; }

        public virtual List<TargetOfWeek> TargetOfWeeks { get; set; }

        /// <summary>
        /// 所有项目公司
        /// </summary>
        public virtual List<CompanyRelationship> CompanyRelationships { get; set; }


        public virtual Area Area { get; set; }
        [Display(Name = "默认行业"), Required]
        public int? AreaID { get; set; }


        public string InternalManager { get; set; }


        [Display(Name = "项目经理")]
        public string ProjectManager { get; set; }
        [Display(Name = "销售经理")]
        public string SalesManager { get; set; }
        [Display(Name = "国内TL")]
        public string ChinaTL { get; set; }
        
        
    }

    [JsonIgnoreAttribute("CompanyRelationships")]
    public class Category : EntityBase
    {
        public virtual Project Project { get; set; }
        [Display(Name = "项目名称")]
        public int? ProjectID { get; set; }
        [Display(Name = "细分行业")]
        public string Name { get; set; }

        [Display(Name = "显示名")]
        public string DisplayName { get { return Name + ": " + Details; } }

        [Display(Name = "Pitch 点")]
        public string Details { get; set; }


      

        public virtual List<CompanyRelationship> CompanyRelationships { get; set; }
    }

    public class CoreLVL : EntityBase
    {
        public string CoreLVLName { get; set; }
        public int CoreLVLCode { get; set; }
    }
    public class Comment : EntityBase
    {
        [Required]
        public string Contents { get; set; }
        public string Submitter { get; set; }
        public DateTime CommentDate { get; set; }
        public virtual CompanyRelationship CompanyRelationship { get; set; }
        public int? CompanyRelationshipID { get; set; }
        public bool? Deleted { get; set; }
    }

    public class CommentContent : EntityBase
    {
        [Display(Name = "内容"), Required]
        public string Contents { get; set; }
    }


    [JsonIgnoreAttribute("Deals", "Project", "Categorys", "LeadCalls", "Members")]
    public class CompanyRelationship : EntityBase
    {
        public List<CrmTrack> CrmTracks { get; set; }

        public virtual CrmCommentState CrmCommentState { get; set; }
        public int? CrmCommentStateID { get; set; }

        public virtual CoreLVL CoreLVL { get; set; }
        [Display(Name = "核心程度")]
        public int? CoreLVLID { get; set; }


        string _categoryString;
        [Display(Name = "细分行业")]
        public string CategoryString
        {
            set { _categoryString = value; }
            get { return _categoryString; }
        }
        public virtual Company Company { get; set; }
        [Display(Name = "目标公司")]
        public int? CompanyID { get; set; }

        [Display(Name = "公司名称")]
        public string CompanyName { get { return Company.Name; } }

        public virtual List<Category> Categorys { get; set; }

        public virtual List<Deal> Deals { get; set; }

        public virtual Project Project { get; set; }

        [Display(Name = "所属项目")]
        public int? ProjectID { get; set; }

        [Display(Name = "建议拨打人数"), Range(0, 20)]
        public int Importancy { get; set; }

        public virtual Progress Progress { get; set; }

        [Display(Name = "成熟度")]
        public int? ProgressID { get; set; }

        public virtual List<LeadCall> LeadCalls { get; set; }

        //public virtual List<LeadCall> HistoryCalls { get; set; }

        public virtual List<Member> Members { get; set; }

        [Display(Name = "可打销售")]
        public string SalesOnTheCompany
        {
            get
            {
                string v = string.Empty;
                if (Members != null)
                    Members.ForEach(s =>
                    {
                        if (string.IsNullOrEmpty(v))
                            v = s.Name;
                        else
                            v += "|" + s.Name;

                    });
                return v;
            }
        }

        [Display(Name = "销售Blow")]
        public string HasBlowed { get; set; }

        [Display(Name = "逻辑删除")]
        public bool? MarkForDelete { get; set; }

        [Display(Name = "Pitch点")]
        public string PitchedPoint { get; set; }

        public virtual List<Comment> Comments { get; set; }

        [Display(Name = "删除")]
        public bool? Deleted { get; set; }
       
    }

    /// <summary>
    /// 月目标管理
    /// </summary>
    public class TargetOfMonth : EntityBase
    {
        public int Month { get { return EndDate.Month; } }
        public virtual Project Project { get; set; }
        [Display(Name = "项目名称"), Required]
        public int? ProjectID { get; set; }

        [Display(Name = "开始日期"), DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime StartDate { get; set; }

        [Display(Name = "结束日期"), DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime EndDate { get; set; }

        [Display(Name = "出单目标"), Required, DisplayFormat(DataFormatString = "{0:c}")]
        public decimal Deal { get; set; }

        [Display(Name = "保底目标"), Required, DisplayFormat(DataFormatString = "{0:c}")]
        public decimal BaseDeal { get; set; }

        [Display(Name = "入账目标"), Required, DisplayFormat(DataFormatString = "{0:c}")]
        public decimal CheckIn { get; set; }

        [Display(Name = "是否财务确认")]
        public bool? IsConfirm { get; set; }

        [Display(Name = "财务确认人")]
        public string Confirmor { get; set; }

        [Display(Name = "是否版块确认")]
        public bool? IsAdminConfirm { get; set; }

        [Display(Name = "版块确认人")]
        public string AdminConfirmor { get; set; }

        [Display(Name = "第一周出单目标"), Required, DisplayFormat(DataFormatString = "{0:c}")]
        public decimal? TargetOf1stWeek { get; set; }

        [Display(Name = "第二周出单目标"), Required, DisplayFormat(DataFormatString = "{0:c}")]
        public decimal? TargetOf2ndWeek { get; set; }

        [Display(Name = "第三周出单目标"), Required, DisplayFormat(DataFormatString = "{0:c}")]
        public decimal? TargetOf3rdWeek { get; set; }

        [Display(Name = "第四周出单目标"), Required, DisplayFormat(DataFormatString = "{0:c}")]
        public decimal? TargetOf4thWeek { get; set; }

        [Display(Name = "第五周出单目标"), Required, DisplayFormat(DataFormatString = "{0:c}")]
        public decimal? TargetOf5thWeek { get; set; }

        [Display(Name = "第一周入账目标"), Required, DisplayFormat(DataFormatString = "{0:c}")]
        public decimal? CheckInOf1stWeek { get; set; }

        [Display(Name = "第二周入账目标"), Required, DisplayFormat(DataFormatString = "{0:c}")]
        public decimal? CheckInOf2ndWeek { get; set; }

        [Display(Name = "第三周入账目标"), Required, DisplayFormat(DataFormatString = "{0:c}")]
        public decimal? CheckInOf3rdWeek { get; set; }

        [Display(Name = "第四周入账目标"), Required, DisplayFormat(DataFormatString = "{0:c}")]
        public decimal? CheckInOf4thWeek { get; set; }

        [Display(Name = "第五周入账目标"), Required, DisplayFormat(DataFormatString = "{0:c}")]
        public decimal? CheckInOf5thWeek { get; set; }
    }

    /// <summary>
    /// 月目标管理
    /// </summary>
    public class TargetOfMonthForMember : EntityBase
    {
        public int Month { get { return StartDate.Month; } }

        public virtual Project Project { get; set; }
        [Display(Name = "项目名称"), Required]
        public int? ProjectID { get; set; }

        [Display(Name = "开始日期"), DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime StartDate { get; set; }

        [Display(Name = "结束日期"), DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime EndDate { get; set; }

        [Display(Name = "出单目标"), Required, DisplayFormat(DataFormatString = "{0:c}")]
        public decimal Deal { get; set; }

        [Display(Name = "保底目标"), Required, DisplayFormat(DataFormatString = "{0:c}")]
        public decimal BaseDeal { get; set; }

        [Display(Name = "入账目标"), Required, DisplayFormat(DataFormatString = "{0:c}")]
        public decimal CheckIn { get; set; }

        public virtual Member Member { get; set; }
        [Display(Name = "对应销售"), Required]
        public int? MemberID { get; set; }

        [Display(Name = "是否确认")]
        public bool? IsConfirm { get; set; }

        [Display(Name = "确认人")]
        public string Confirmor { get; set; }

        [Display(Name = "第一周出单目标"), Required, DisplayFormat(DataFormatString = "{0:c}")]
        public decimal? TargetOf1stWeek { get; set; }

        [Display(Name = "第二周出单目标"), Required, DisplayFormat(DataFormatString = "{0:c}")]
        public decimal? TargetOf2ndWeek { get; set; }

        [Display(Name = "第三周出单目标"), Required, DisplayFormat(DataFormatString = "{0:c}")]
        public decimal? TargetOf3rdWeek { get; set; }

        [Display(Name = "第四周出单目标"), Required, DisplayFormat(DataFormatString = "{0:c}")]
        public decimal? TargetOf4thWeek { get; set; }

        [Display(Name = "第五周出单目标"), Required, DisplayFormat(DataFormatString = "{0:c}")]
        public decimal? TargetOf5thWeek { get; set; }

        [Display(Name = "第一周入账目标"), Required, DisplayFormat(DataFormatString = "{0:c}")]
        public decimal? CheckInOf1stWeek { get; set; }

        [Display(Name = "第二周入账目标"), Required, DisplayFormat(DataFormatString = "{0:c}")]
        public decimal? CheckInOf2ndWeek { get; set; }

        [Display(Name = "第三周入账目标"), Required, DisplayFormat(DataFormatString = "{0:c}")]
        public decimal? CheckInOf3rdWeek { get; set; }

        [Display(Name = "第四周入账目标"), Required, DisplayFormat(DataFormatString = "{0:c}")]
        public decimal? CheckInOf4thWeek { get; set; }

        [Display(Name = "第五周入账目标"), Required, DisplayFormat(DataFormatString = "{0:c}")]
        public decimal? CheckInOf5thWeek { get; set; }
    }


    public class TargetOfPackage : EntityBase
    {
        public virtual CompanyRelationship CompanyRelationship { get; set; }
        [Required]
        public int? CompanyRelationshipID { get; set; }

        public virtual Lead Lead { get; set; }
        [Display(Name = "Lead")]
        public int? LeadID { get; set; }


        public Package Package { get; set; }
        [Display(Name = "销售目标")]
        public int? PackageID { get; set; }
    }



    /// <summary>
    /// 周目标管理
    /// </summary>
    public class TargetOfWeek : EntityBase
    {
        public virtual Project Project { get; set; }
        [Display(Name = "项目名称")]//, Required]
        public int? ProjectID { get; set; }

        [Display(Name = "开始日期"), Required, DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime StartDate { get; set; }

        [Display(Name = "结束日期"), Required, DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime EndDate { get; set; }

        [Display(Name = "销售目标"), Required, DisplayFormat(DataFormatString = "{0:c}")]
        public decimal Deal { get; set; }


        [Display(Name = "入账目标"), Required, DisplayFormat(DataFormatString = "{0:c}")]
        public decimal CheckIn { get; set; }

        public string Member { get; set; }

        [Display(Name = "关联的月目标")]//, Required]
        public int? TargetOfMonthID { get; set; }


    }

    public class CompanyRelationshipMember
    {
        public int Member_ID { get; set; }
        public virtual Member Member { get; set; }
        public int CompanyRelationship_ID { get; set; }
        public virtual CompanyRelationship CompanyRelationship { get; set; }
        public bool MarkForDelete { get; set; }
    }

    /// <summary>
    /// 团队成员
    /// </summary>
    [JsonIgnoreAttribute("CompanyRelationships", "LeadCalls", "Project", "TargetOfWeeks", "SalesType")]
    public class Member : EntityBase
    {
        [Display(Name = "测试数据")]
        public bool? Test { get; set; }

        [Display(Name = "成员"), Required]
        public string Name { get; set; }

        [Display(Name = "字头")]
        public string Characters { get; set; }

        public virtual List<TargetOfWeek> TargetOfWeeks { get; set; }

        public virtual List<LeadCall> LeadCalls { get; set; }

        [Display(Name = "销售类型"), Required]
        public int? SalesTypeID { get; set; }
        public virtual SalesType SalesType { get; set; }

        [Display(Name = "所在项目"), Required]
        public int? ProjectID { get; set; }
        public virtual Project Project { get; set; }

        public string[] CharactersSet { get { return string.IsNullOrEmpty(Characters) ? new string[] { } : Characters.Split('|'); } }

        public virtual List<CompanyRelationship> CompanyRelationships { get; set; }

        [Display(Name = "是否激活")]
        public bool IsActivated { get; set; }
    }

    /// <summary>
    /// 出单
    /// </summary>
    [JsonIgnoreAttribute("CompanyRelationship")]
    public class Deal : CompanyRelationshipChildItem
    {
        decimal? AveragePoll;
        //public decimal? AveragePoll
        //{
        //    get
        //    {
        //        if()
        //        if (Poll == 0) return Income;
        //        else
        //            return Income / Poll;
        //    }
        //}
        public virtual Project Project { get; set; }
        [Display(Name = "项目名称"), Required]
        public int? ProjectID { get; set; }

        [Display(Name = "出单号")]
        public string DealCode { get; set; }

        [Display(Name = "客户签单人"), Required]
        public string Committer { get; set; }

        [Display(Name = "参会客户")]
        public virtual List<Participant> Participants { get; set; }

        [Display(Name = "签单人联系方式"), Required]
        public string CommitterContect { get; set; }

        [Required]
        [Display(Name = "签单人邮箱")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "请输入的有效的签单人邮箱")]
        public string CommitterEmail { get; set; }

        [Display(Name = "会务须知权益描述"), MaxLength(2000)]
        public string TicketDescription { get; set; }

        public virtual Package Package { get; set; }
        [Display(Name = "销售Package"), Required]
        public int? PackageID { get; set; }


        [Display(Name = "坏账")]
        public bool Abandoned { get; set; }

        [Display(Name = "坏账原因")]
        public string AbandonReason { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }

        [Display(Name = "合约付款日期"), Required]
        public DateTime ExpectedPaymentDate { get; set; }

        [Display(Name = "实际付款日期")]
        public DateTime? ActualPaymentDate { get; set; }

        [Display(Name = "签约日期")]
        public DateTime? SignDate { get; set; }

        [Display(Name = "是否付款")]
        public bool IsClosed { get; set; }

        [Display(Name = "实际入账")]
        public decimal Income { get; set; }

        [Display(Name = "出单人"), Required]
        public string Sales { get; set; }

        [Display(Name = "应付款"), Required]
        [Range(1000.0, 100000000.0, ErrorMessage = "应付款必须大于等于1000")]
        public decimal Payment { get; set; }

        public virtual CurrencyType Currencytype { get; set; }
        [Display(Name = "币种")]
        [Required(ErrorMessage="必须选择币种")]
        public int CurrencyTypeID { get; set; }

        [Display(Name = "RMB应付款"), Required]
        public decimal RMBPayment { 
            get{
                if (Currencytype == null)
                    return 0;
                if (Currencytype.Name == "RMB")
                    return Payment;
                else
                    return 0;
            }
        }
        [Display(Name = "USD应付款"), Required]
        public decimal USDPayment
        {
            get
            {
                if (Currencytype == null)
                    return 0;
                if (Currencytype.Name == "USD")
                    return Payment;
                else
                    return 0;
            }
        }

        [Display(Name = "出单经验分享"), MaxLength(2000), Required]
        public string PaymentDetail { get; set; }

        [Display(Name = "是否确认")]
        public bool? IsConfirm { get; set; }

        [Display(Name = "确认人")]
        public string Confirmor { get; set; }

        
        [Display(Name = "票数"),Required]
        public int Poll { get; set; }


        //[Display(Name = "公司地址")]
        //public string Address_CH { get; set; }


        //[Display(Name = "英文地址")]
        //public string Address_EN { get; set; }

        [Display(Name = "删除")]
        public bool? Deleted { get; set; }

        [Display(Name = "出单类型")]
        public string DealType { get; set; }
    }

    public class Progress : EntityBase
    {
        [Display(Name = "成熟度")]
        public String Name { get; set; }

        [Display(Name = "编码")]
        public int? Code { get; set; }

    }

    public class ImportCompanyTrace : EntityBase
    {
        [Display(Name = "导入人")]
        public string ImportUserName { get; set; }

        [Display(Name = "导入时间")]
        public DateTime ImportDate { get; set; }

        [Display(Name = "导入公司数")]
        public int ImportCompanyCount { get; set; }

        [Display(Name = "导入Lead数")]
        public int ImportLeadCount { get; set; }

        [Display(Name = "导入目标项目")]
        public string ImportTargetProject { get; set; }

        [Display(Name = "导入项目源")]
        public string ImportSourceProject { get; set; }
    }

    public class CompanyMergeTrack: EntityBase
    {
        [Display(Name = "表名")]
        public string TableName { get; set; }

        [Display(Name = "删除ID")]
        public string OldID { get; set; }

        [Display(Name = "新ID")]
        public string NewID { get; set; }
        
    }
}


