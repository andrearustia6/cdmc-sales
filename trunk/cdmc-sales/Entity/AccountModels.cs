using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
//using System.Web.Mvc;
//using System.Web.Security;

namespace Entity{

    public class LogOnModel {
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


}
