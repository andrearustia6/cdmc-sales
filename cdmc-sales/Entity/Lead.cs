using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Attributes;

namespace Entity
{

    public class SubCompany : NameEntity
    {
        public virtual DistrictNumber DistrictNumber { get; set; }
        [Display(Name = "区号/时差")]
        public int? DistrictNumberID { get; set; }

        [Display(Name = "可打时间")]
        public string Available
        {
            get
            {
                if (DistrictNumber == null) return string.Empty;
                var dt1 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DBSR.WorkTimeStart, 0, 0);
                var dt2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DBSR.WorkTimeEnd, 0, 0);
                return dt1.AddHours(-DistrictNumber.TimeDifference).ToShortTimeString() + "~" + dt2.AddHours(-DistrictNumber.TimeDifference).ToShortTimeString();
            }
        }
        [Display(Name = "分公司传真")]
        public string Fax { get; set; }

        [Display(Name = "分公司网站")]
        public string WebSite { get; set; }

        [Display(Name = "分公司邮编")]
        public string ZIP { get; set; }

        [Display(Name = "分公司总机"), MaxLength(100)]
        public string Contact { get; set; }

        [Display(Name = "分公司地址"), MaxLength(100)]
        public string Address { get; set; }

        public virtual Company Company { get; set; }
        public int? CompanyID { get; set; }

    }


    /// <summary>
    /// 公司
    /// </summary>
    public class Company : NameEntity
    {
        public virtual List<SubCompany> SubCompanys { get; set; }

        [Display(Name = "主营业务")]
        public string Business { get; set; }

        [Display(Name = "公司网站")]
        public string WebSite { get; set; }

        [Display(Name = "公司邮编")]
        public string ZIP { get; set; }

        [Display(Name = "公司总机"), MaxLength(100)]
        public string Contact { get; set; }

        [Display(Name = "公司地址"), MaxLength(100)]
        public string Address { get; set; }

        public virtual List<Lead> Leads { get; set; }

        [Display(Name = "可打时间")]
        public string Available 
        { 
            get 
            {
                if (DistrictNumber == null) return string.Empty;
                var dt1 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DBSR.WorkTimeStart,0,0);
                var dt2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DBSR.WorkTimeEnd, 0, 0);
                return dt1.AddHours(-DistrictNumber.TimeDifference).ToShortTimeString()+"~"+dt2.AddHours(-DistrictNumber.TimeDifference).ToShortTimeString();
            } 
        }

        
        public virtual CompanyType CompanyType { get; set; }
        [Display(Name = "公司类型")]
        public int? CompanyTypeID { get; set; }
 
        public virtual DistrictNumber DistrictNumber { get; set; }
        [Display(Name = "区号/时差")]
        public int? DistrictNumberID { get; set; }

        
        public virtual Area Area { get; set; }
        [Display(Name = "行业类型")]
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

        [Display(Name = "公司传真")]
        public string Fax { get; set; }

        [Display(Name = "中文地址")]
        public string Address_EN { get; set; }

        [Display(Name = "省")]
        public string Province { get; set; }

        [Display(Name = "市")]
        public string City { get; set; }

        [Display(Name = "公司规模")]
        public string Scale { get; set; }   

        [Display(Name = "年销售额")]
        public string AnnualSales { get; set; }

        [Display(Name = "主要产品")]
        public string MainProduct { get; set; }

        [Display(Name = "主要客户")]
        public string MainClient { get; set; }

        [Display(Name = "是否有效")]
        public bool? IsValid { get; set; }

        [Display(Name = "公司点评")]
        public string CompanyReviews { get; set; }

        [Display(Name = "公司客户")]
        public string Customers { get; set; }

        [Display(Name = "竞争对手")]
        public string Competitor { get; set; }
    }

    /// <summary>
    /// 公司类型
    /// </summary>
    public class CompanyType : EntityBase
    {
        [Display(Name = "公司类型"), Required, MaxLength(100)]
        public string Name { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    [JsonIgnoreAttribute("ModifiedTime","Company","Image")]
    public class Lead :NameEntity
    {
        public virtual DistrictNumber DistrictNumber { get; set; }
        [Display(Name = "区号/时差")]
        public int? DistrictNumberID { get; set; }

        public virtual SubCompany SubCompany { get; set; }
        [Display(Name="所在分公司")]
        public int? SubCompanyID { get; set; }

        [Display(Name = "个人邮箱")]
        [DataType(DataType.EmailAddress)]
        public string PersonalEmailAddress { get; set; }

        [Display(Name = "客户性别")]
        public string Gender { get; set; }

        [Display(Name = "客户职位")]
        public string Title { get; set; }

        [Display(Name = "所在部门")]
        public string Department { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "客户直线")]
        public string Contact { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "工作邮箱")]
        public string EMail { get; set; }

        [Display(Name = "工作传真")]
        public string Fax { get; set; }

        [Display(Name = "客户生日")]
        public DateTime? Birthday { get; set; }

        [Display(Name = "联系地址")]
        public string Address { get; set; }

        [Display(Name = "联系邮编")]
        public string ZIP { get; set; }

        [DataType( DataType.PhoneNumber)]
        [Display(Name = "移动电话")]
        public string Mobile { get; set; }

        [Display(Name = "微博账号")]
        public string WeiBo { get; set; }

        [Display(Name = "微信账号")]
        public string WeiXin { get; set; }

        [Display(Name = "LinkIn账号")]
        public string LinkIn { get; set; }

        [Display(Name = "FaceBook账号")]
        public string FaceBook { get; set; }

        [Display(Name = "博客地址")]
        public string Blog { get; set; }

        public virtual Company Company { get; set; }
        [Display(Name = "所属公司"),Required]
        public int? CompanyID { get; set; }

        [Display(Name = "私人电话")]
        public string PersonalPhone { get; set; }
        [Display(Name = "私人手机")]
        public string PersonalCellPhone { get; set; }
        [Display(Name = "私人传真")]
        public string PersonalFax { get; set; }
        [Display(Name = "备注")]
        public string Comment { get; set; }
        [Display(Name = "lead角色")]
        public string LeadRoles { get; set; }
        [Display(Name = "QQ账号")]
        public string QQ { get; set; }
        [Display(Name = "Twitter账号")]
        public string Twitter { get; set; }
        [Display(Name = "所在分支机构")]
        public string Branch { get; set; }

        public virtual List<TargetOfPackage> TargetOfPackages { get; set; }

        public virtual List<Project> Projects { get; set; }



        public virtual Image Image { get; set; }
        public int?  ImageID { get; set; }
        [Display(Name = "逻辑删除"), Required]
        public bool? MarkForDelete { get; set; }

        [Display(Name = "是否有效")]
        public bool? IsValid { get; set; }
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

        [Display(Name = "显示名")]
        public string DisplayName { get { return Name + ": " + ResultDescription; } }

        [Display(Name = "致电结果描述")]
        public string ResultDescription { get; set; }
    }



    /// <summary>
    /// 电话结果管理
    /// </summary>
      [JsonIgnoreAttribute("CompanyRelationship")]
    public class LeadCall : CompanyRelationshipChildItem
    {
          public override string ToString()
          {
              return "客户：" + Lead.Name + "  致电结果：" + LeadCallType.Name + "  致电人：" + Member.Name + "  致电时间：" + CallDate + "  录入时间：" + CreatedDate+"  项目："+Project.ProjectCode;
          }
        public int? ProjectID { get; set; }
        public virtual Project Project { get; set; }

        public virtual Lead Lead { get; set; }
        [Display(Name = "致电客户"), Required]
        public int? LeadID { get; set; }

        [Display(Name = "所属客户关系")]
        public int? CompanyRelationshipID { get; set; }
        public virtual CompanyRelationship CompanyRelationship { get; set; }

        public virtual LeadCallType LeadCallType { get; set; }
        [Display(Name = "致电类型"), Required]
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

        [Display(Name = "逻辑删除")]
        public bool? MarkForDelete { get; set; }

        //public int? CompanyID { get; set; }

        //public bool? IsImport { get; set; }
    }
}
