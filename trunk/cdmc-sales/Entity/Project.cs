using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Entity
{


    /// <summary>
    /// 调研
    /// </summary>
    public class Research : EntityBase
    {
        [Display(Name = "公司名称"), Required]
        public String Name { get; set; }

        [Display(Name = "内容")]
        public String Contents { get; set; }
    }

    /// <summary>
    /// 职级
    /// </summary>
    public class Role : EntityBase
    {
        public const int LVL_Director = 1000;
        public const int LVL_Manager = 500;
        public const int LVL_TeamLeader = 100;
        public const int LVL_Sales = 10;
        public const int LVL_ProductInterface = 5;
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
    }

    /// <summary>
    /// 部门管理
    /// </summary>
    public class SalesType : EntityBase
    {
        [Display(Name = "销售名称"), Required]
        public String Name { get; set; }
    }

    /// <summary>
    /// 项目
    /// </summary>
    public class Project : NameEntity
    {
        //public string OpeningDate { get { return EndDate.Year.ToString(); } }

        [Display(Name = "板块负责人")]
        public string Manager { get; set; }

        [Display(Name = "项目编号"), Required]
        public string ProjectCode { get; set; }

        [Display(Name = "市场部接口人")]
        public string Market { get; set; }


        [Display(Name = "产品部接口人")]
        public string Product { get; set; }

        public List<Member> Members { get; set; }

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

        [Display(Name = "团队负责人")]
        public string TeamLeader { get; set; }

        public virtual List<Template> Templates { get; set; }

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
    }

    public class Category : EntityBase
    {
        public virtual Project Project { get; set; }
        [Display(Name = "项目名称")]
        public int? ProjectID { get; set; }
        [Display(Name = "类型")]
        public string Name { get; set; }

        public virtual List<CompanyRelationship> CompanyRelationships { get; set; }
    }

    public class CompanyRelationship : EntityBase
    {
        string _categoryString;
        public string CategoryString { 
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
        public int? ProgressID { get; set; }
        public virtual List<LeadCall> LeadCalls { get; set; }

        public virtual List<Member> Members { get; set; }

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
    }

    /// <summary>
    /// 月目标管理
    /// </summary>
    public class TargetOfMonth : EntityBase
    {
        public virtual Project Project { get; set; }
        [Display(Name = "项目名称"), Required]
        public int ProjectID { get; set; }

        [Display(Name = "开始日期"), Required, DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime StartDate { get; set; }

        [Display(Name = "结束日期"), Required, DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime EndDate { get; set; }

        [Display(Name = "销售目标"), Required, DisplayFormat(DataFormatString = "{0:c}")]
        public decimal Deal { get; set; }

        [Display(Name = "保底目标"), Required, DisplayFormat(DataFormatString = "{0:c}")]
        public decimal BaseDeal { get; set; }

        [Display(Name = "入账目标"), Required, DisplayFormat(DataFormatString = "{0:c}")]
        public decimal CheckIn { get; set; }
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
        [Display(Name = "项目名称"), Required]
        public int ProjectID { get; set; }

        [Display(Name = "开始日期"), Required, DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime StartDate { get; set; }

        [Display(Name = "结束日期"), Required, DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime EndDate { get; set; }

        [Display(Name = "销售目标"), Required, DisplayFormat(DataFormatString = "{0:c}")]
        public decimal Deal { get; set; }


        [Display(Name = "入账目标"), Required, DisplayFormat(DataFormatString = "{0:c}")]
        public decimal CheckIn { get; set; }

        public string Member { get; set; }

        [Display(Name = "关联的月目标"), Required]
        public int? TargetOfMonthID { get; set; }


    }

    /// <summary>
    /// 团队成员
    /// </summary>
    public class Member : EntityBase
    {
        [Display(Name = "成员"),Required]
        public string Name { get; set; }

        [Display(Name = "字头")]
        public string Characters { get; set; }

        public  virtual List<TargetOfWeek> TargetOfWeeks { get; set; }

        public virtual List<LeadCall> LeadCalls { get; set; }

        [Display(Name = "销售类型"), Required]
        public int? SalesTypeID  { get; set; }
        public virtual SalesType SalesType { get; set; }

        [Display(Name = "所在项目"), Required]
        public int? ProjectID { get; set; }
        public virtual Project Project { get; set; }

        public string[] CharactersSet { get { return string.IsNullOrEmpty(Characters) ? new string[] { } : Characters.Split('|'); } }

        public virtual List<CompanyRelationship> CompanyRelationships { get; set; }
    }

    /// <summary>
    /// 出单
    /// </summary>
    public class Deal : CompanyRelationshipChildItem
    {
        //public virtual CompanyRelationship CompanyRelationship { get; set; }
        //[Display(Name = "客户公司"), Required]
        //public int? CompanyRelationshipID { get; set; }

        public virtual Package Package { get; set; }
        [Display(Name = "销售Package"), Required]
        public int? PackageID { get; set; }

        [Display(Name = "坏账")]
        public bool Abandoned { get; set; }

        [Display(Name = "坏账原因")]
        public string AbandonReason { get; set; }

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

        [Display(Name = "出单人"),Required]
        public string Sales { get; set; }

        [Display(Name = "应付款"), Required]
        public decimal Payment { get; set; }

        [Display(Name = "出单描述"), MaxLength(2000)]
        public string PaymentDetail { get; set; }
    }


    public class Progress : EntityBase
    {
        public Company Company { get; set; }
        [Display(Name = "对应公司"), Required]
        public int CompanyID { get; set; }

        [Display(Name = "完成度")]
        public int Complement { get; set; }

        public virtual Member Member { get; set; }
        [Display(Name = "销售员工")]
        public int? MemberID { get; set; }
    }


}
   

