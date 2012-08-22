using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
    /// <summary>
    /// 电话结果
    /// </summary>
    public class LeadSheetType : EntityBase
    {
        [Display(Name = "电话结果"), Required]
        public string Name { get; set; }

        [Display(Name = "预备字段"), Required]
        public int Code { get; set; }

        [Display(Name = "致电结果描述"),Required]
        public string ResultDescription { get; set; }
    }

    /// <summary>
    /// 电话结果管理
    /// </summary>
    public class LeadSheet : EntityBase
    {
        public virtual Lead Lead { get; set; }
        [Display(Name = "致电客户"), Required]
        public int? LeadID { get; set; }

     
        //[Display(Name = "所属项目"), Required]
        //public int? ProjectID { get; set; }

        public virtual LeadSheetType CallResultType { get; set; }
        public int? LeadSheetType { get; set; }

        [Display(Name = "致电客户")]
        public bool FaxOut
        {
            get
            {
                return true;
            }
        }

        [Display(Name = "结果描述")]
        public string Result { get; set; }

         [Display(Name = "回电时间")]
        public DateTime CallBackDate { get; set; }

    }

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

        [Display(Name = "备用字段")]
        public int Code { get; set; }
        
        public virtual Project Project { get; set; }

        [Display(Name = "所属项目")]
        public int? ProjectID { get; set; }

        [Display(Name = "答案"),Required,MaxLength(1000)]
        public string Answer { get; set; }
    }
}
