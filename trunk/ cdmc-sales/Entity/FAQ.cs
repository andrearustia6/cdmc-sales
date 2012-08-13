using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
    public class FAQ : EntityBase
    {
        [Display(Name = "问题标题"), StringLength(200), Required]
        public String Name { get; set; }
        [Display(Name = "问题类型"), Required]
        public int Code { get; set; }
        [Display(Name = "副标题"), StringLength(200)]
        public string SubName { get; set; }

        [Display(Name = "问题描述"), Required]
        public string Question { get; set; }

        [Display(Name = "问题答案"), Required]
        public string Answer { get; set; }
    }
}
