using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Profile;

namespace Entity {
    public class LogOnModel
    {
        [Required]
        [Display(Name = "用户名称")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "用户密码")]
        public string Password { get; set; }

        [Display(Name = "记录密码?")]
        public bool RememberMe { get; set; }
    }

    public class ChangePasswordModel {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "当前密码")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "密码长度最少为{0}.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "新设密码")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "密码确认")]
        [Compare("NewPassword", ErrorMessage = "密码确认和新设密码不匹配.")]
        public string ConfirmPassword { get; set; }
    }

    public class UserInfoModel {
        [Required]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "{0}的格式不正确")]
        [Display(Name = "邮件地址")]
        public string Email { get; set; }

        [Display(Name = "全名")]
        public string DisplayName { get; set; }

        [Display(Name = "是否激活")]
        public bool IsActivated { get; set; }

        [Display(Name = "销售职级")]
        public string SalesLevel { get; set; }

        [Display(Name = "入职时间")]
        public DateTime? StartDate { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "密码长度最少为{0}.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "用户密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "密码确认")]
        [Compare("Password", ErrorMessage = "密码确认和新设密码不匹配.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "联系电话")]
        [DataType(DataType.PhoneNumber)]
        public int Contact { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "移动电话")]
        public string Mobile { get; set; }

        [Display(Name = "生日")]
        public DateTime BirthDay { get; set; }

        [Display(Name = "工作岗位"),Required]
        public int RoleID { get; set; }
        public virtual Role Role { get; set; }

        [Display(Name = "员工性别")]
        public string Gender { get; set; }

        [Display(Name = "部门")]
        public int? DepartmentID { get; set; }
        public virtual Department Department { get; set; }

        [Display(Name = "工作职级")]
        public int? ExpLevelID { get; set; }
        public virtual ExpLevel ExpLevel { get; set; }


    }
}
