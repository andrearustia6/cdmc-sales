using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Attributes;
namespace Entity
{
    public class Speaker : EntityBase
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
    /// <summary>
    /// 合作机构
    /// </summary>
    public class Organization : EntityBase
    {
        public int? ConferenceID { get; set; }
        public virtual Conference Conference{ get; set; }

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

    public class Conference : EntityBase
    {

        #region basic

        [Display(Name = "项目编号")]
        public string ProjectCode { get; set; }

        [Display(Name = "是否激活")]
        public bool IsActived { get; set; }

        [Display(Name = "CP")]
        public string CP { get; set; }

        [Display(Name = "设计师")]
        public string Designer { get; set; }

        [Display(Name = "市场专员")]
        public string Market { get; set; }

        [Display(Name = "选择样式")]
        public int? StyleSelect { set; get; }

        #endregion

        #region 基本信息

        [Required(ErrorMessage = "The Conference Name Required")]
        [Display(Name = "会议名称")]
        public string ConferenceName { get; set; }

        // [Required(ErrorMessage = "The Conference SubTitle Required")]
        [Display(Name = "会议副标题")]
        public string ConferenceSubtitle { get; set; }

        // [Required(ErrorMessage = "The Conference Slogan Required")]
        [Display(Name = "会议口号")]
        public string ConferenceSlogan { get; set; }

        [Display(Name = "会议官网")]
        public string Website { get; set; }

        [Display(Name = "会议举办历史")]
        public string ConferenceAbbreviation { get; set; }

        [Display(Name = "会议简称")]
        public string ConferenceNameAbbreviation { get; set; }


        [Display(Name = "举办国家")]
        public string LocationCountry { get; set; }

        [Display(Name = "举办城市")]
        public string LocationCity { get; set; }

        //  [Required(ErrorMessage = "The Conference Location Required")]
        [Display(Name = "举办酒店")]
        public string ConferenceLocation { get; set; }

        [Display(Name = "举办地址")]
        public string ConferenceLocationFullAddress
        {
            get
            {
                return LocationCountry + LocationCity + ConferenceLocation;
            }

        }

        [Display(Name = "举办地址")]
        public string ConferenceLocationWithoutAddress
        {
            get
            {
                return LocationCountry + LocationCity;
            }

        }


        //[Required(ErrorMessage = "The Conference StartDate Required")]
        [Display(Name = "会议开始时间")]
        public DateTime? ConferenceStartDate { get; set; }


        // [Required(ErrorMessage = "The Conference EndDate Required")]
        [Display(Name = "会议结束时间")]
        public DateTime? ConferenceEndDate { get; set; }

        


        [Display(Name = "联系网址")]
        public string ContactUsWebSite
        {
            get
            {
                return "http://www.cdmc.org.cn/contact-us.asp";
            }
        }
        #endregion

        #region 折扣

        [Display(Name = "报名截止时间")]
        public DateTime? BeforeDate { get; set; }

        

        [Display(Name = "参会折扣金额")]
        public decimal Discount { get; set; }

        [Display(Name = "参会折扣失效时间")]
        public DateTime? AttendanceExpiration { get; set; }

        

        [Display(Name = "赞助折扣百分比")]
        public string DiscountRate { get; set; }

        [Display(Name = "赞助折扣失效时间")]
        public DateTime? SponsorExpiration { get; set; }

        

        #endregion

        #region 邀请函
        [Display(Name = "行业名称")]
        public string IndustryName { get; set; }

        [Display(Name = "地域名称")]
        public string FieldName { get; set; }

        [Display(Name = "行业机会")]
        public string IndustryOpportunities { get; set; }

        [Display(Name = "行业挑战")]
        public string IndustryChallenges { get; set; }



        [Display(Name = "会议愿景")]
        public string VisionOfConference { get; set; }

        [Display(Name = "已预定展位")]
        public int Reserved { get; set; }

        [Display(Name = "保留展位")]
        public int Available { get; set; }

        [Display(Name = "已预定席位")]
        public int ReservedSeats { get; set; }

        [Display(Name = "剩余席位")]
        public int AvailableSeats { get; set; }

        #endregion

        #region 颁奖

        [Display(Name = "颁奖名称")]
        public string AwardName { get; set; }

        [Display(Name = "颁奖时间")]
        public DateTime? AwardDate { get; set; }

        [Display(Name = "颁奖结束时间")]
        public DateTime? AwardEndDate { get; set; }

        [Display(Name = "免费申请截止时间")]
        public DateTime? ApplicationDeadline { get; set; }

        [Display(Name = "奖项申请截止时间")]
        public DateTime? AwardDeadline { get; set; }

        [Display(Name = "颁奖页面")]
        public string AwardWebsite { get; set; }


        

        #endregion

        #region 联系方式

        #region 注册联系方式

        [Display(Name = "联系电话")]
        public string RegContact { get; set; }

        [Display(Name = "传真")]
        public string RegFax { get; set; }

        [Display(Name = "电子邮箱")]
        public string RegEmail { get; set; }

        [Display(Name = "在线注册")]
        public string RegOnline { get; set; }




        #endregion

        #region 负责人联系方式

        [Display(Name = "姓名")]

        public string Name { get; set; }

        [Display(Name = "联系电话")]

        public string Contact { get; set; }

        [Display(Name = "职位")]

        public string Title { get; set; }

        [Display(Name = "电子邮箱")]

        public string Email { get; set; }

        #endregion

        #endregion

        
        public virtual List<Organization> Org { get; set; }

        public virtual List<Speaker> Speakers { get; set; }



        [Display(Name = "行业关键词")]
        public string Keywords { get; set; }

        [Display(Name = "中文手册路径")]
        public string BrochureCH { get; set; }

        [Display(Name = "英文手册路径")]
        public string BrochureEN { get; set; }

        //[Required(ErrorMessage = "The ConferenceCH Name Required")]
        [Display(Name = "会议中文名称")]
        public string ConferenceNameCH { get; set; }

        // [Required(ErrorMessage = "The Conference SubTitle Required")]
        [Display(Name = "会议中文副标语")]
        public string ConferenceSubtitleCH { get; set; }


    }
    public class ClientDurationType : EntityBase
    {
        [Display(Name = "名称")]
        public string Name { get; set; }
    }
}
