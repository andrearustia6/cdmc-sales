﻿using System;
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

        [Display(Name = "上传人")]
        public String Creator { get; set; }

    }

    /// <summary>
    /// 职级
    /// </summary>
    public class Role : EntityBase
    {
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
    /// 项目
    /// </summary>
    public class Project : EntityBase
    {
        [Display(Name = "板块负责人")]
        public string Manager { get; set; }

        [Display(Name = "Leader")]
        public string Leader { get; set; }

        [Display(Name = "项目编号"), Required]
        public string ProjectCode { get; set; }

        [Display(Name = "市场部接口人")]
        public string Market { get; set; }


        [Display(Name = "产品部接口人")]
        public string Product { get; set; }

        public List<Member> Members { get; set; }

        [Display(Name = "正在进行"), Required]
        public bool IsActived { get; set; }

        [Display(Name = "项目名称"), Required]
        public String Name { get; set; }

        [Display(Name = "开始时间"), Required]
        public DateTime StartDate { get; set; }

        [Display(Name = "结束时间"), Required]
        public DateTime EndDate { get; set; }

        [Display(Name = "销售目标"), Required]
        public decimal Target { get; set; }

        public string TeamLeader { get; set; }

        public List<Template> Templates { get; set; }

        public List<Keyword> Keywords { get; set; }

        [Display(Name = "项目背景")]
        public string SaleBrief { get; set; }

        [Display(Name = "其它")]
        public string Others { get; set; }

        public List<News> News { get; set; }

        public List<Message> Messages { get; set; }

        public List<Progress> Progresses { get; set; }

        /// <summary>
        /// 被参考的项目的CoreList自动加入本项目的CoreList
        /// </summary>
        public string References { get; set; }

        public List<TargetOfMonth> TargetOfMonths { get; set; }

        public List<TargetOfWeek> TargetOfWeeks { get; set; }

        /// <summary>
        /// 所有项目公司
        /// </summary>
        public List<Company> Companys { get; set; }

        /// <summary>
        /// 项目公司 Core List
        /// </summary>
        public List<Company> CoreList { get; set; }

        //public List<Lead> Leads { get; set; }

        public List<Deal> Deals { get; set; }
        
        
    }

    /// <summary>
    /// 月目标管理
    /// </summary>
    public class TargetOfMonth : EntityBase
    {
        public virtual Project Project { get; set; }
        [Display(Name = "项目名称"), Required]
        public int ProjectID { get; set; }

        [Display(Name = "开始日期"), Required]
        public DateTime StartDate { get; set; }

        [Display(Name = "结束日期"), Required]
        public DateTime EndDate { get; set; }

        [Display(Name = "销售目标"), Required]
        public decimal Deal { get; set; }

        [Display(Name = "保底目标"), Required]
        public decimal BaseDeal { get; set; }

        [Display(Name = "入账目标"), Required]
        public decimal CheckIn { get; set; }
    }

    public class TargetOfPackage : EntityBase
    {
        public virtual Project Project { get; set; }
        [Display(Name = "项目名称")]
        public int? ProjectID { get; set; }

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

        [Display(Name = "开始日期"), Required]
        public DateTime StartDate { get; set; }

        [Display(Name = "结束日期"), Required]
        public DateTime EndDate { get; set; }

        [Display(Name = "销售目标"), Required]
        public decimal Deal { get; set; }

        [Display(Name = "入账目标"), Required]
        public decimal CheckIn { get; set; }

        public string Member { get; set; }
     
        [Display(Name = "关联的月目标"), Required]
        public int? TargetOfMonthID { get; set; }


    }

    /// <summary>
    /// 团队成员
    /// </summary>
    public class Member:EntityBase
    {
        [Display(Name = "成员")]
        public string Name { get; set; }

        [Display(Name = "字头")]
        public string Characters { get; set; }

        public List<TargetOfWeek> TargetOfWeeks { get; set; }

        [Display(Name = "所在项目"), Required]
        public int? ProjectID { get; set; }
        public virtual Project Project { get; set; }

        public string[] CharactersSet { get { return string.IsNullOrEmpty(Characters)?new string[]{}: Characters.Split('|'); } }
     
        //public List<Company> Companys { get; set; }
    }

    /// <summary>
    /// 出单
    /// </summary>
    public class Deal : EntityBase
    {
        public virtual Lead Lead { get; set; }
        [Display(Name = "Lead"), Required]
        public int? LeadID { get; set; }

        public virtual Package Package { get; set; }
        [Display(Name = "销售Package"), Required]
        public int? PackageID { get; set; }

        [Display(Name = "坏账")]
        public bool Abandoned { get; set; }

        [Display(Name = "坏账原因")]
        public string AbandonReason { get; set; } 

        [Display(Name = "合约付款日期"),Required]
        public DateTime ExpectedPaymentDate { get; set; }

        [Display(Name = "实际付款日期")]
        public DateTime? ActualPaymentDate { get; set; }

        [Display(Name = "是否付款")]
        public bool IsClosed { get; set; }

        [Display(Name = "实际入账")]
        public decimal Income { get; set; }

        [Display(Name = "出单人")]
        public string Sales { get; set; }

        [Display(Name = "应付款"), Required]
        public decimal Payment { get; set; }

        [Display(Name = "出单描述"), MaxLength(2000)]
        public string PaymentDetail { get; set; }

        public int? ProjectID { get; set; }
        [Display(Name = "所属项目")]
        public Project Project { get; set; }
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
   

