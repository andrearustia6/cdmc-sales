using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Mvc;
using System.Web.Security;

namespace Entity {

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

    public class RegisterModel {
        [Required]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "邮件地址")]
        public string Email { get; set; }

        //[Required]
        //[DataType(DataType.PhoneNumber)]
        //[Display(Name = "个人电话")]
        //public string Tel { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "密码长度最少为{0}.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "用户密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "密码确认")]
        [Compare("Password", ErrorMessage = "密码确认和新设密码不匹配.")]
        public string ConfirmPassword { get; set; }
    }
}
