using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
    public class Research:EntityBase
    {
        [Display(Name = "公司名称")]
        public String CompanyName { get; set; }

        [Display(Name = "主营业务")]
        public String BussinessArea { get; set; }

        [Display(Name = "发展计划")]
        public String Plan { get; set; }

        [Display(Name = "新闻"),MaxLength(int.MaxValue)]
        public String News { get; set; }

        public virtual Image Image { get; set; }
        public int? ImageID { get; set; }

        [Display(Name = "联系电话")]
        public string Contact { get; set; }
    }
}
