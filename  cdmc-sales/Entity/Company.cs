using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
    public class Company:EntityBase
    {
        public string ComapanyTitle { get; set; }
        public string Contact { get; set; }
        public string Address { get; set; }
        public List<Client> Clients;
        [Display(Name = "外资比率"),Range(0,100)]
        public double ForeignAssetPercentage { get; set; }

        [Display(Name = "内资比率")]
        public double DomesticAssetPercentage { get { return 100 - ForeignAssetPercentage; } }
    }

    public class CompanyType : EntityBase
    {
        public string ComapanyTypeName { get; set; }
        
    }
}
