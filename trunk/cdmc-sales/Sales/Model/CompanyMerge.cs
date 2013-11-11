using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entity;
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
        public string Name_CH { get; set; }

        public string Name_EN { get; set; }

        public string Business { get; set; }
        public string Description { get; set; }
        

        public string WebSite { get; set; }

        public string ZIP { get; set; }

        public string Contact { get; set; }

        public string Address { get; set; }
        public int? CompanyTypeID { get; set; }

        public int? DistrictNumberID { get; set; }

        public int? AreaID { get; set; }

        public int? ImageID { get; set; }

        public double ForeignAssetPercentage { get; set; }

        public double DomesticAssetPercentage { get { return 100 - ForeignAssetPercentage; } }

        public string Cerator { get; set; }

        public string From { get; set; }

        public string Fax { get; set; }

        public string Address_EN { get; set; }

        public string Province { get; set; }

        public string City { get; set; }

        public string Scale { get; set; }

        public string AnnualSales { get; set; }

        public string MainProduct { get; set; }

        public string MainClient { get; set; }

        public bool? IsValid { get; set; }

        public string CompanyReviews { get; set; }

        public string Customers { get; set; }

        public string Competitor { get; set; }

        public bool? IsVIP { get; set; }

    }
    public class AjaxMergeCompany
    {
        public List<int> ids { get; set; }
        public _Company company { get; set; }
    }
}