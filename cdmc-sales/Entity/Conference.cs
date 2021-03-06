﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Attributes;
namespace Entity
{

    /// <summary>
    /// 参会类型
    /// </summary>
    public class ParticipantType : NameEntity
    {
        //[Display(Name = "参会类型")]
        //public string Name { get; set; }
    }

     /// <summary>
    /// Package类型
    /// </summary>
    public class PackageType : NameEntity
    {

    }



    /// <summary>
    /// 套餐子项
    /// </summary>
    public class PackageItem : NameEntity
    {
        [Display(Name = "参考图片")]
        public Image Image { get; set; }

        [Display(Name = "英文内容"), Required]
        public string Content { get; set; }

        [Display(Name = "中文内容"), Required]
        public string ContentCH { get; set; }

        [Display(Name = "Package"), Required]
        public int? PackageID { get; set; }
        public virtual Package Package { get; set; }

        public int? CurrencyTypeID { get; set; }
        public virtual CurrencyType CurrencyType  { get; set; }
    }

    /// <summary>
    /// 参会套餐项目
    /// </summary>
    public class Package : NameEntity
    {
        public virtual ParticipantType ParticipantType { get; set; }
        [Display(Name = "参会类型"), Required]
        public int? ParticipantTypeID { get; set; }


        public virtual PackageType PackageType { get; set; }
        [Display(Name = "Package类型"), Required]
        public int? PackageTypeID { get; set; }

        [Display(Name = "英文描述"), Required]
        public string SubName { get; set; }

        [Display(Name = "中文描述"), Required]
        public string SubNameCH { get; set; }

        public virtual List<PackageItem> PackageItems { get; set; }

        [Display(Name = "美元费用"), Required]
        public decimal Prize { get; set; }


        [Display(Name = "人民币费用"), Required]
        public decimal PrizeCH { get; set; }
    }

    ///// <summary>
    ///// 参会客户
    ///// </summary>
    //public class Participant : EntityBase
    //{
    //    [Display(Name = "参会客户"), Required]
    //    public Lead Client { get; set; }

    //    public virtual ParticipantType ParticipantType { get; set; }
    //    [Display(Name = "参会类型"),Required]
    //    public int? ParticipantTypeID { get; set; }

    //    [Display(Name = "套餐内容")]
    //    public string PackageID { get; set; }
    //    public Package Package { get; set; }
    //}

    /// <summary>
    /// 会议管理
    /// </summary>
    //public class Conference : EntityBase
    //{
    //    [Display(Name = "会议名称"), Required]
    //    public string Name { get; set; }

    //    [Display(Name = "会议内容"), Required]
    //    public string Content { get; set; }

    //    public virtual  List<Package> Packages { get; set; }

    //    public ParticipantType ParticipantType { get; set; }
    //    [Display(Name = "参会类型"), Required]
    //    public int? ParticipantTypeID { get; set; }
    //}

    public class Participant : EntityBase
    {
        public virtual Deal Deal { get; set; }
        [Display(Name = "关联出单")]
        public int?  DealID { get; set; }

        [Display(Name = "参会人名称"), Required]
        public string Name { get; set; }

        [Display(Name = "职位")]
        public string Title { get; set; }

        [Display(Name = "性别")]
        public string Gender { get; set; }

        [Display(Name = "直线电话")]
        public string Contact { get; set; }

        [Display(Name = "移动电话")]
        public string Mobile { get; set; }

        [Display(Name="工作邮箱")]
        public string Email { get; set; }

        public virtual ParticipantType ParticipantType { get; set; }
        [Display(Name = "参会类型"), Required]
        public int? ParticipantTypeID { get; set; }


        [Display(Name = "国内邮编"), Required]
        public string ZIP { get; set; }

        [Display(Name = "国内地址"), Required]
        public string Address { get; set; }

        public virtual Project Project { get; set; }
        public int? ProjectID { get; set; }

        [Display(Name = "唯一ID"), Required]
        public string PID { get; set; }

        [Display(Name = "是否毁约")]
        public bool CancelAttended { get; set; }
        
    }

    public class AjaxParticipant : EntityBase
    {
        public virtual Deal Deal { get; set; }
        [Display(Name = "关联出单")]
        public int? DealID { get; set; }

        [Display(Name = "参会人名称")]
        [Required ]
        public string Name { get; set; }

        [Display(Name = "职位")]
        public string Title { get; set; }


        [Display(Name = "公司")]
        public string Company { get; set; }

        [Display(Name = "项目编号")]
        public string ProjectCode { get; set; }

        [Display(Name = "出单编号")]
        public string DealCode { get; set; }

        //[UIHint("Gender"), Display(Name = "性别")]
        [Required, Display(Name = "性别")]
        public string Gender { get; set; }

        [Display(Name = "直线电话")]
        [RegularExpression(@"[\d\s-]*", ErrorMessage = "请输入的有效的直线电话")]
        public string Contact { get; set; }

        [Display(Name = "移动电话")]
        [RegularExpression(@"[\d\s-]*", ErrorMessage = "请输入的有效的移动电话")]
        public string Mobile { get; set; }

        [Display(Name = "工作邮箱")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "请输入的有效的工作邮箱")]
        public string Email { get; set; }

        public virtual ParticipantType ParticipantType { get; set; }

        [Display(Name = "参会类型"), Required]
        public int? ParticipantTypeID { get; set; }

        //[UIHint("ParticipantTypeName"), Display(Name = "参会类型"), Required]
        [Display(Name = "参会类型")]
        public string ParticipantTypeName { get; set; }

        [Display(Name = "国内邮编")]
        [RegularExpression(@"[\d\s-]*", ErrorMessage = "请输入的有效的国内邮编")]
        public string ZIP { get; set; }

        [Display(Name = "国内地址")]
        public string Address { get; set; }

        public virtual Project Project { get; set; }
        public int? ProjectID { get; set; }

        [Display(Name = "是否毁约")]
        public bool CancelAttended { get; set; }
    }
    public class AjaxParticipantForDeal : EntityBase
    {
        public virtual Deal Deal { get; set; }
        [Display(Name = "关联出单")]
        public int? DealID { get; set; }

        [Display(Name = "参会人名称")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "职位")]
        public string Title { get; set; }

        //[UIHint("Gender"), Display(Name = "性别")]
        [Required, Display(Name = "性别")]
        public string Gender { get; set; }

        [Display(Name = "直线电话")]
        [RegularExpression(@"[\d\s-]*", ErrorMessage = "请输入的有效的直线电话")]
        public string Contact { get; set; }

        [Display(Name = "移动电话")]
        [RegularExpression(@"[\d\s-]*", ErrorMessage = "请输入的有效的移动电话")]
        public string Mobile { get; set; }

        [Display(Name = "工作邮箱")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "请输入的有效的工作邮箱")]
        public string Email { get; set; }

        public virtual ParticipantType ParticipantType { get; set; }

        [Display(Name = "参会类型"), Required]
        public int? ParticipantTypeID { get; set; }

        //[UIHint("ParticipantTypeName"), Display(Name = "参会类型"), Required]
        [Display(Name = "参会类型")]
        public string ParticipantTypeName { get; set; }

        [Display(Name = "国内邮编")]
        [RegularExpression(@"[\d\s-]*", ErrorMessage = "请输入的有效的国内邮编")]
        public string ZIP { get; set; }

        [Display(Name = "国内地址")]
        public string Address { get; set; }

        public virtual Project Project { get; set; }
        public int? ProjectID { get; set; }

        [Display(Name = "是否毁约")]
        public bool CancelAttended { get; set; }
    }
}
