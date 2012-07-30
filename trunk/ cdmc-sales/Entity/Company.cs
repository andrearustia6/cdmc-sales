using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
    public class Company:EntityBase
    {
        [Display(Name = "公司名称"),Required,MaxLength(100)]
        public string ComapanyTitle { get; set; }
        [Display(Name = "联系方式"), Required, MaxLength(100)]
        public string Contact { get; set; }
        [Display(Name = "公司地址"), Required, MaxLength(100)]
        public string Address { get; set; }
        public List<Client> Clients;
        [Display(Name = "外资比率"),Range(0,100)]
        public double ForeignAssetPercentage { get; set; }

        [Display(Name = "内资比率")]
        public double DomesticAssetPercentage { get { return 100 - ForeignAssetPercentage; } }
    }

    public class CompanyType : EntityBase
    {
        [Display(Name = "公司类型名称"),Required,MaxLength(100)]
        public string ComapanyTypeName { get; set; }
        
    }
}
