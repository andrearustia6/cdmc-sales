using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entity;
using System.ComponentModel.DataAnnotations;
namespace Sales.Model
{
    public class CompanyInfo
    {
        public int? CRMID { get; set; }
        public int? CompanyID { get; set; }
        public string CompanyNameCH { get; set; }
        public string CompanyNameEN { get; set; }
        public string CompanyName { get { return string.Join(",", CompanyNameEN, CompanyNameCH).Trim(','); }  }
        
    }
    public class _Company
    {
        public int? CRMID { get; set; }

        public int? ID { get; set; }
        [Display(Name = "中文名称"), MaxLength(100)]
        public string Name_CH { get; set; }

        [Display(Name = "英文名称"), MaxLength(100)]
        public string Name_EN { get; set; }
        [Display(Name = "公司介绍")]
        public string Description { get; set; }
        
        [Display(Name = "主营业务")]
        public string Business { get; set; }

        [Display(Name = "公司网站")]
        public string WebSite { get; set; }

        [Display(Name = "公司邮编")]
        public string ZIP { get; set; }

        [Display(Name = "公司总机"), MaxLength(100)]
        public string Contact { get; set; }

        [Display(Name = "公司地址"), MaxLength(100)]
        public string Address { get; set; }


        


        [Display(Name = "公司类型")]
        public int? CompanyTypeID { get; set; }

        [Display(Name = "区号/时差")]
        public int? DistrictNumberID { get; set; }


        [Display(Name = "行业类型")]
        public int? AreaID { get; set; }

        [Display(Name = "公司架构")]
        public int? ImageID { get; set; }

        [Display(Name = "外资比率"), Range(0, 100)]
        public double ForeignAssetPercentage { get; set; }

        [Display(Name = "内资比率")]
        public double DomesticAssetPercentage { get { return 100 - ForeignAssetPercentage; } }

        [Display(Name = "上传员工")]
        public string Cerator { get; set; }

        [Display(Name = "来源部门")]
        public string From { get; set; }

        [Display(Name = "公司传真")]
        public string Fax { get; set; }

        [Display(Name = "中文地址")]
        public string Address_EN { get; set; }

        [Display(Name = "省")]
        public string Province { get; set; }

        [Display(Name = "市")]
        public string City { get; set; }

        [Display(Name = "公司规模")]
        public string Scale { get; set; }

        [Display(Name = "年销售额")]
        public string AnnualSales { get; set; }

        [Display(Name = "主要产品")]
        public string MainProduct { get; set; }

        [Display(Name = "主要客户")]
        public string MainClient { get; set; }

        [Display(Name = "是否有效")]
        public bool? IsValid { get; set; }

        [Display(Name = "公司点评")]
        public string CompanyReviews { get; set; }

        [Display(Name = "公司客户")]
        public string Customers { get; set; }

        [Display(Name = "竞争对手")]
        public string Competitor { get; set; }

        [Display(Name = "VIP客户")]
        public bool? IsVIP { get; set; }

    }
    public class AjaxMergeCompany
    {
        public List<int> ids { get; set; }
        public _Company company { get; set; }
    }
}