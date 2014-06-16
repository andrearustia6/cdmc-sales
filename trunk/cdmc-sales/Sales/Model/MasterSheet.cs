using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entity;
using System.ComponentModel.DataAnnotations;
using Model;
namespace Sales.Model
{
    public class _Speaker : EntityBase
    {
        public int? ConferenceID { get; set; }
        public virtual Conference Conference { get; set; }

        [Display(Name = "发言人"), Required]
        public string Name { get; set; }
        [Display(Name = "职位"), Required]
        public string Title { get; set; }
        [Display(Name = "发言单位"), Required]
        public string Company { get; set; }
        [Display(Name = "嘉宾重要性")]
        public string Content { get; set; }
        [Display(Name = "嘉宾背景")]
        public string Profile { get; set; }
        [Display(Name = "嘉宾演讲议题")]
        public string ContentDescription { get; set; }

        [Display(Name = "确认出席")]
        public bool? ConfirmedAttend { get; set; }

        [Display(Name = "照片")]
        public string ImgPath { get; set; }

        [Display(Name = "公司类型")]
        public int? CategoryID { get; set; }
        public virtual Category Category { get; set; }

        public virtual string SpeakerContent
        {
            get
            {
                return Name + "-" + Title + "-" + Company;
            }
        }

        [Display(Name = "是否VIP")]
        public bool IsVIP { get; set; }

        [Display(Name = "机构性质"), Required]
        public string InstitutionalNature { get; set; }

        [Display(Name = "助理/秘书")]
        public string Assistant { get; set; }

        [Display(Name = "电话")]
        public string Phone { get; set; }

        [Display(Name = "手机")]
        public string Mobile { get; set; }

        [Display(Name = "邮箱")]
        public string Email { get; set; }

        [Display(Name = "传真")]
        public string Fax { get; set; }

        [Display(Name = "邀请函发送")]
        public string CommunicationRecord { get; set; }

        [Display(Name = "备注信息")]
        public string NoteInformation { get; set; }

        [Display(Name = "官网")]
        public string WebSite { get; set; }

        [Display(Name = "调研新闻网址")]
        public string NewsWebSite { get; set; }

        [Display(Name = "地址")]
        public string Address { get; set; }

        [Display(Name = "稿费参考")]
        public string RoyaltiesReference { get; set; }

        [Display(Name = "到稿情况")]
        public string DraftCase { get; set; }

        [Display(Name = "重要程度"), Required]
        public string Importance { get; set; }

        [Display(Name = "类型")]
        public int? ClientDurationTypeID { get; set; }
        
    }
    public class _Organization : EntityBase
    {
        public int? ConferenceID { get; set; }
        public virtual Conference Conference { get; set; }

        [Display(Name = "机构名称")]
        public string OrgName { get; set; }

        [Display(Name = "联系人")]
        public string ContactPerson { get; set; }

        [Display(Name = "电话")]
        public string Contact { get; set; }

        [Display(Name = "手机")]
        public string Mobile { get; set; }

        [Display(Name = "邮箱")]
        public string Email { get; set; }

        [Display(Name = "机构类型")]
        public string OrgType { get; set; }

        [Display(Name = "合作形式")]
        public string OrgForm { get; set; }

        [Display(Name = "负责洽谈人")]
        public string Owner { get; set; }

        [Display(Name = "Logo")]
        public string ImgPath { get; set; }
    }
}