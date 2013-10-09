using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Sales.Model
{
    public class _ProjectReview
    {
        public int ID { get; set; }
        public int? ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string ProjectType { get; set; }
        public string Summary { get; set; }
        public string ModifiedUser { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? CallCount { get; set; }
        public int? FaxOutCount { get; set; }
        public int? 出单数量 { get; set; }
        public decimal? delegatecount { get; set; }
        public decimal? sponsorcount { get; set; }
        public int? ConCount { get; set; }
        public int? LeadCount { get; set; }
    }
}