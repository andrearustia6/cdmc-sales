using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
    /// <summary>
    /// 公司
    /// </summary>
    public class Company : EntityBase
    {
        [Display(Name = "中文名称"), MaxLength(100)]
        public string Name_CH { get; set; }

        [Display(Name = "英文名称"), MaxLength(100)]
        public string Name_EN { get; set; }

        [Display(Name = "联系方式"), Required, MaxLength(100)]
        public string Contact { get; set; }

        [Display(Name = "公司地址"), MaxLength(100)]
        public string Address { get; set; }

        public List<Client> Clients { get; set; }

        [Display(Name = "关键字")]
        public string KeyWords { get; set; }

        [Display(Name = "可打时间")]
        public string Available{ get; set; }

        [Display(Name = "公司类型")]
        public virtual CompanyType CompanyType { get; set; }
        [Required]
        public int? CompanyTypeID { get; set; }

        [Display(Name = "区号")]
        public string DistrictNumber { get; set; }

        [Display(Name = "行业类型")]
        public virtual Category Category { get; set; }
        [Required]
        public int? CategoryID { get; set; }

        [Display(Name = "公司架构")]
        public virtual Image Image { get; set; }
        public int? ImageID { get; set; }

        [Display(Name = "外资比率"), Range(0, 100)]
        public double ForeignAssetPercentage { get; set; }

        [Display(Name = "内资比率")]
        public double DomesticAssetPercentage { get { return 100 - ForeignAssetPercentage; } }

        public virtual Member Cerator { get; set; }
        public int CeratorID  { get; set; }

        [Display(Name = "来源部门")]
        public int From { get; set; }
             
    
    }

    /// <summary>
    /// 公司类型
    /// </summary>
    public class CompanyType : EntityBase
    {
        [Display(Name = "公司类型名称"), Required, MaxLength(100)]
        public string Name { get; set; }

    }

    /// <summary>
    /// 客户
    /// </summary>
    public class Client : EntityBase
    {
        public virtual PositionType PositionType { get; set; }
        [Display(Name = "客户职位"),Required]
        public int? PositionTypeID { get; set; }

        [Display(Name = "中文名称"), MaxLength(100)]
        public string Name_CH { get; set; }

        [Display(Name = "英文名称"), MaxLength(100)]
        public string Name_EN { get; set; }

        [Display(Name = "联系方式"),Required]
        public string Contact { get; set; }

        public virtual Company Company { get; set; }
        [Display(Name = "所属公司"),Required]
        public int? CompanyID { get; set; }

        public virtual ClientType ClientType { get; set; }
        [Display(Name = "客户类型"),Required]
        public int? ClientTypeID { get; set; }

        public virtual Image Image { get; set; }
        public int?  ImageID { get; set; }
    }

    /// <summary>
    /// 客户类型
    /// </summary>
    public class ClientType : EntityBase
    {
        [Display(Name = "客户类型名称"), Required]
        public string Name { get; set; }
    }

    /// <summary>
    /// 公司区域
    /// </summary>
    public class Region : EntityBase
    {
        [Display(Name = "公司区域"), Required]
        public string Name { get; set; }
    }

    /// <summary>
    /// 公司区域
    /// </summary>
    public class Region : EntityBase
    {
        [Display(Name = "公司区域"), Required]
        public string Name { get; set; }
    }

    /// <summary>
    /// 职位类型
    /// </summary>
    public class PositionType : EntityBase
    {
        [Display(Name = "职位类型"), Required]
        public string Name { get; set; }
    }
}
