using System;
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
    public class ParticipantType : EntityBase
    {
        [Display(Name = "参会类型")]
        public string Name { get; set; }
    }

     /// <summary>
    /// Package类型
    /// </summary>
    public class PackageType : EntityBase
    {
        [Display(Name = "Package类型")]
        public string Name { get; set; }
    }

    /// <summary>
    /// 参会助商参会服务
    /// </summary>
    public class PackageServiceType : EntityBase
    {
        [Display(Name = "服务名称")]
        public string Name { get; set; }
    }

    /// <summary>
    /// 套餐子项
    /// </summary>
    public class PackageItem : EntityBase
    {
        [Display(Name = "参考图片")]
        public Image Image { get; set; }

        public virtual ParticipantType ParticipantType { get; set; }
        [Display(Name = "参会类型"), Required]
        public int? ParticipantTypeID { get; set; }

        [Display(Name = "子项名称"), Required]
        public string Name { get; set; }

        [Display(Name = "子项内容"), Required]
        public string Content { get; set; }

        public virtual PackageType PackageType { get; set; }
        [Display(Name = "Package类型"), Required]
        public int? PackageTypeID { get; set; }

        public virtual PackageServiceType PackageServiceType { get; set; }
        [Display(Name = "参会服务类型"), Required]
        public int? PackageServiceTypeID { get; set; }

    }

    /// <summary>
    /// 参会套餐项目
    /// </summary>
    public class Package : EntityBase
    {
        public virtual ParticipantType ParticipantType { get; set; }
        [Display(Name = "参会类型"), Required]
        public int? ParticipantTypeID { get; set; }

        [Display(Name = "套餐名称"), Required]
        public string Name { get; set; }

        [Display(Name = "套餐描述"), Required]
        public string SubName { get; set; }

        public List<PackageItem> PackageItems { get; set; }
    }


    /// <summary>
    /// 参会客户
    /// </summary>
    public class Participant : EntityBase
    {
        [Display(Name = "参会客户"), Required]
        public Lead Client { get; set; }

        public virtual ParticipantType ParticipantType { get; set; }
        [Display(Name = "参会类型"),Required]
        public int? ParticipantTypeID { get; set; }

        [Display(Name = "套餐内容")]
        public string PackageID { get; set; }
        public Package Package { get; set; }
    }

    /// <summary>
    /// 会议管理
    /// </summary>
    public class Conference : EntityBase
    {
        [Display(Name = "会议名称"), Required]
        public string Name { get; set; }

        [Display(Name = "会议内容"), Required]
        public string Content { get; set; }

        public List<Package> Packages { get; set; }

        public ParticipantType ParticipantType { get; set; }
        [Display(Name = "参会类型"), Required]
        public int? ParticipantTypeID { get; set; }
    }
}
