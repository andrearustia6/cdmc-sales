using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Entity;

namespace Entity
{
     //[JsonIgnoreAttribute("ModifiedTime")]
    public abstract class EntityBase
    {
        public int ID { get; set; }
        [Display(Name = "描述")]
        public string Description { get; set; }
        [Display(Name = "排序")]
        public int? Sequence { get; set; }

        [Display(Name = "更改用户")]
        public string User { get; set; }

        [Display(Name = "更改时间")]
        public DateTime? ModifiedTime { get; set; }
    }
}