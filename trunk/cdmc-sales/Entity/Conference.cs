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
    /// 参会套餐
    /// </summary>
    public class Package : EntityBase
    {
        [Display(Name = "套餐名称")]
        public string Name { get; set; }

        [Display(Name = "套餐描述")]
        public string SubName { get; set; }

        [Display(Name = "套餐内容")]
        public string Content { get; set; }
    }

    /// <summary>
    /// 参会客户
    /// </summary>
    public class Participant : EntityBase
    {
        [Display(Name = "参会客户"), Required]
        public Lead Client { get; set; }

        public ParticipantType ParticipantType { get; set; }
        [Display(Name = "参会类型"),Required]
        public ParticipantType ParticipantType { get; set; }

        [Display(Name = "套餐内容")]
        public string Content { get; set; }
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
        public ParticipantType ParticipantType { get; set; }

        [Display(Name = "套餐内容")]
        public string Content { get; set; }
    }

}
