using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Entity
{

    /// <summary>
    /// 模板类型
    /// </summary>
    public class TemplateType : EntityBase
    {
        [Display(Name = "模板类型")]
        public string Name { get; set; }

        [Display(Name = "预备字段")]
        public int Code { get; set; }
    }

    /// <summary>
    /// 通用模板管理
    /// </summary>
    public class Template : EntityBase
    {
        [Display(Name = "模板名称")]
        public string Name { get; set; }

        [Display(Name = "模板副名称")]
        public string SubName { get; set; }

        [Display(Name = "模板副内容")]
        public string Content { get; set; }

        [Display(Name = "模板语言")]
        public string Language { get; set; }

        [Display(Name = "模板类型"),Required]
        public int? TemplateTypeID { get; set; }
        public virtual TemplateType TemplateType { get; set; }
    }

    /// <summary>
    /// 话术模板
    /// </summary>
    public class OnPhoneTemplate : EntityBase
    {
        [Display(Name = "话术名称")]
        public string Name { get; set; }

        [Display(Name = "拒绝理由"),Required]
        public string Block { get; set; }
        
        public virtual OnPhoneBlockType OnPhoneBlockType { get; set; }
        [Display(Name = "理由类型"), Required]
        public int? OnPhoneBlockTypeID { get; set; }
        
        public virtual Project Project { get; set; }

        [Display(Name = "所属项目")]
        public int? ProjectID { get; set; }

        [Display(Name = "答案"),Required,MaxLength(1000)]
        public string Answer { get; set; }
    }

    /// <summary>
    /// 话术模板
    /// </summary>
    public class OnPhoneBlockType : EntityBase
    {
        [Display(Name = "Block类型")]
        public string Name { get; set; }

        [Display(Name = "代码")]
        public int Code { get; set; }

    }
}
