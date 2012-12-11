using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Attributes;

namespace Entity
{
    /// <summary>
    /// 公司
    /// </summary>
    public class Company : NameEntity
    {
        [Display(Name = "公司座机"), MaxLength(100)]
        public string Contact { get; set; }

        [Display(Name = "公司地址"), MaxLength(100)]
        public string Address { get; set; }

        public List<Lead> Leads { get; set; }

        //[Display(Name = "业务范围")]
        //public string Areas { get; set; }

        [Display(Name = "可打时间")]
        public string Available 
        { 
            get 
            {
                if (DistrictNumber == null) return string.Empty;
                var dt1 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DBSR.WorkTimeStart,0,0);
                var dt2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DBSR.WorkTimeEnd, 0, 0);
                return dt1.AddHours(DistrictNumber.TimeDifference).ToShortTimeString()+"~"+dt2.AddHours(DistrictNumber.TimeDifference).ToShortTimeString();
            } 
        }

        [Display(Name = "公司类型")]
        public virtual CompanyType CompanyType { get; set; }
        public int? CompanyTypeID { get; set; }
 
        public virtual DistrictNumber DistrictNumber { get; set; }
        [Display(Name = "区号/时差")]
        public int? DistrictNumberID { get; set; }

        [Display(Name = "行业类型")]
        public virtual Area Area { get; set; }
        public int? AreaID { get; set; }

        [Display(Name = "公司架构")]
        public virtual Image Image { get; set; }
        public int? ImageID { get; set; }

        [Display(Name = "外资比率"), Range(0, 100)]
        public double ForeignAssetPercentage { get; set; }

        [Display(Name = "内资比率")]
        public double DomesticAssetPercentage { get { return 100 - ForeignAssetPercentage; } }

        [Display(Name = "上传员工")]
        public string  Cerator { get; set; }

        [Display(Name = "来源部门")]
        public string From { get; set; }

        [Display(Name = "传真")]
        public string Fax { get; set; }

        [Display(Name = "已离职")]
        public bool LeaveCompany { get; set; }

    
    }

    /// <summary>
    /// 公司类型
    /// </summary>
    public class CompanyType : EntityBase
    {
        [Display(Name = "公司类型名称"), Required, MaxLength(100)]
        public string Name { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    [JsonIgnoreAttribute("ModifiedTime","Company","Image")]
    public class Lead :NameEntity
    {
        [Display(Name = "个人邮箱")]
        [DataType(DataType.EmailAddress)]
        public string PersonalEmailAddress { get; set; }

        [Display(Name = "性别")]
        public string Gender { get; set; }

        [Display(Name = "职位")]
        public string Title { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "直线")]
        public string Contact { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "工作邮箱")]
        public string EMail { get; set; }

        [Display(Name = "传真")]
        public string Fax { get; set; }

        [DataType( DataType.PhoneNumber)]
        [Display(Name = "移动电话")]
        public string Mobile { get; set; }

        public virtual Company Company { get; set; }
        [Display(Name = "所属公司"),Required]
        public int? CompanyID { get; set; }

        public List<TargetOfPackage> TargetOfPackages { get; set; }

        public List<Project> Projects { get; set; }

        public virtual Image Image { get; set; }
        public int?  ImageID { get; set; }
    }

    /// <summary>
    /// 电话结果
    /// </summary>
    public class LeadCallType : EntityBase
    {
        [Display(Name = "状态"), Required]
        public string Name { get; set; }

        [Display(Name = "结果代码")]
        public int Code { get; set; }

        [Display(Name = "致电结果描述")]
        public string ResultDescription { get; set; }
    }



    /// <summary>
    /// 电话结果管理
    /// </summary>
    public class LeadCall : CompanyRelationshipChildItem
    {
        public int? ProjectID { get; set; }

        public virtual Lead Lead { get; set; }
        [Display(Name = "致电客户"), Required]
        public int? LeadID { get; set; }

        [Display(Name = "所属客户关系")]
        public int? CompanyRelationshipID { get; set; }
        public virtual CompanyRelationship CompanyRelationship { get; set; }

        public virtual LeadCallType LeadCallType { get; set; }
        public int? LeadCallTypeID { get; set; }

        public virtual Member Member { get; set; }
        [Required,Display( Name="拨打人")]
        public int? MemberID { get; set; }

       
        [Required, Display(Name = "致电时间")]
        public DateTime CallDate { get; set; }

        [Display(Name = "是否有效")]
        public bool FaxOut
        {
            get
            {
                if (LeadCallType == null || LeadCallType.Name == "Not Pitched" || LeadCallType.Name == "Blowed")
                    return false;
                else
                    return true;
            }
        }

        [Display(Name = "结果描述")]
        public string Result { get; set; }

        [Display(Name = "回电时间")]
        public DateTime? CallBackDate { get; set; }

    }
}
