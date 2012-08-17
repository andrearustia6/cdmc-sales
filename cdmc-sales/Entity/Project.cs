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
    public class RoleLevel : EntityBase
    {
        [Display(Name = "职级名称"), Required]
        public String Name { get; set; }

        [Display(Name = "职级分数"), Required]
        public int Level { get; set; }
    }

    /// <summary>
    /// 部门管理
    /// </summary>
    public class Department:EntityBase
    {
        [Display(Name = "部门名称"), Required]
        public String Name { get; set; }
    }

    /// <summary>
    /// 项目
    /// </summary>
     public class Project:EntityBase
    {
        [Display(Name = "项目名称"), Required]
        public String Name { get; set; }
    }
    
    /// <summary>
    /// 月目标管理
    /// </summary>
    public class Target_M : EntityBase
    {
        public virtual Project Project { get; set; }
        [Display(Name = "项目名称"), Required]
        public int ProjectID { get; set; }

        [Display(Name = "目标月份"), Required]
        public DateTime Month { get; set; }

        [Display(Name = "销售目标"), Required]
        public decimal Deal { get; set; }

        [Display(Name = "保底目标"), Required]
        public decimal BaseDeal { get; set; }

        [Display(Name = "入账目标"), Required]
        public decimal CheckIn { get; set; }
    }


    /// <summary>
    /// 月目标管理
    /// </summary>
    public class Target_M : EntityBase
    {
        public virtual Project Project { get; set; }
        [Display(Name = "项目名称"), Required]
        public int ProjectID { get; set; }

        [Display(Name = "目标月份"), Required]
        public DateTime Month { get; set; }

        [Display(Name = "销售目标"), Required]
        public decimal Deal { get; set; }

        [Display(Name = "保底目标"), Required]
        public decimal BaseDeal { get; set; }

        [Display(Name = "入账目标"), Required]
        public decimal CheckIn { get; set; }
    }

    /// <summary>
    /// 周目标管理
    /// </summary>
    public class Target_W : EntityBase
    {
        public virtual Project Project { get; set; }
        [Display(Name = "项目名称"), Required]
        public int ProjectID { get; set; }

        [Display(Name = "开始时间"), Required]
        public DateTime StartDay { get; set; }

        [Display(Name = "销售目标"), Required]
        public decimal Deal { get; set; }

        [Display(Name = "入账目标"), Required]
        public decimal CheckIn { get; set; }
    }

    /// <summary>
    /// 团队成员
    /// </summary>
    public class Member
    {
        [Display(Name = "成员姓名"), Required]
        public string Name { get; set; }


        public RoleLevel RoleLevel { get; set; }
        [Display(Name = "角色职级"), Required]
        public int? RoleLevelID { get; set; }

        [Display(Name = "字头")]
        public string Character { get; set; }

        public List<Target_W> Target_Ws{get;set;} 

        public Department Department { get; set; }
        [Display(Name = "所在部门"), Required]
        public int? DepartmentID { get; set; }

        [Display(Name = "所在项目"), Required]
        public int? ProjectID { get; set; }
        public virtual Project Project { get; set; }
    }

    /// <summary>
    /// 团队
    /// </summary>
    public class Team
    {
        [Display(Name = "团队名称"), Required]
        public string Name { get; set; }

        public Member LeaderID { get; set; }
        [Display(Name = "角色职级"), Required]
        public Member Leader { get; set; }

        public virtual Member Market { get; set; }
        [Display(Name = "市场部接口人")]
        public int? MarketID { get; set; }

        public Member Product { get; set; }
         [Display(Name = "市场部接口人")]
        public int? ProductID { get; set; }

        public List<Member> Members { get; set; }

        public int? ProjectID { get; set; }
        [Display(Name = "所属项目")]
        public Project Project { get; set; }
    }

    /// <summary>
    /// 出单
    /// </summary>
    public class Deal
    {
        public virtual ParticipantType ParticipantType { get; set; }
        public int? ParticipantTypeID { get; set; }

        public virtual CurrencyType CurrencyType { get; set; }
        public int? CurrencyTypeID { get; set; }

        [Display(Name = "是否付款")]
        public bool IsClosed { get; set; }

        [Display(Name = "实际入账")]
        public decimal Income { get; set; }

        [Display(Name = "应付款"),Required]
        public decimal Payment { get; set; }

        [Display(Name = "出单描述"), MaxLength(1000)]
        public string PaymentDetail { get; set; }

        public int? ProjectID { get; set; }
         [Display(Name = "所属项目")]
        public Project Project { get; set; }
    }

    
    public class Progress
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
   

