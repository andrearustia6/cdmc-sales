using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Attributes;
namespace Entity
{
    [JsonIgnore("CompanyType")]
    public class Company:EntityBase
    {
        [Display(Name = "公司名称"),Required,MaxLength(100)]
        public string Name { get; set; }

        [Display(Name = "联系方式"), Required, MaxLength(100)]
        public string Contact { get; set; }

        [Display(Name = "公司地址"), MaxLength(100)]
        public string Address { get; set; }


        public List<Client> Clients { get; set; }
        
        [Display(Name = "公司类型")]
        public virtual CompanyType CompanyType { get; set; }
        [Required]
        public int? CompanyTypeID { get; set; }

        [Display(Name = "行业类型")]
        public virtual Area Area { get; set; }
        [Required]
        public int? AreaID { get; set; }

        [Display(Name = "外资比率"),Range(0,100)]
        public double ForeignAssetPercentage { get; set; }

        [Display(Name = "内资比率")]
        public double DomesticAssetPercentage { get { return 100 - ForeignAssetPercentage; } }
    }

    public class CompanyType : EntityBase
    {
        [Display(Name = "公司类型名称"),Required,MaxLength(100)]
        public string Name { get; set; }
        
    }

    public class Area : EntityBase
    {
        [Display(Name = "行业名称"), Required]
        public string Name { get; set; }
    }
}
