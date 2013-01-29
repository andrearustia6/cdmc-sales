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
        public string ModifiedUser { get; set; }

        [Display(Name = "更改时间")]
        public DateTime? ModifiedDate { get; set; }

        [Display(Name = "创建用户")]
        public string Creator { get; set; }

        [Display(Name = "创建时间")]
        public DateTime? CreatedDate { get; set; }
    }

    public class NameEntity : EntityBase
    {
        public string _name;
        [Display(Name = "名称")]
        public string Name
        {
            get
            {

                if (string.IsNullOrEmpty(_name))
                    _name = EntityUtl.Utl.GetName(this);

                return _name;
            }
        }


        [Display(Name = "中文名称"), MaxLength(100)]
        public string Name_CH { get; set; }

        [Display(Name = "英文名称"), MaxLength(100)]
        public string Name_EN { get; set; }
       
    }

    public class CompanyRelationshipChildItem : EntityBase
    {
        public virtual CompanyRelationship CompanyRelationship { get; set; }
        [Display(Name = "客户公司"), Required]
        public int? CompanyRelationshipID { get; set; }
     
    }
}