using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
    public class UserFavorsCrmGroup : EntityBase
    {
        [Required]
        public string DisplayName { get; set; }

        public string UserName { get; set; }

        public virtual List<UserFavorsCRM> UserFavorsCRMs { get; set; }
    }

    public class UserFavorsCRM : EntityBase
    {
        public virtual CompanyRelationship CompanyRelationship { get; set; }
        [Required]
        public int? CompanyRelationshipID { get; set; }

        public virtual UserFavorsCrmGroup UserFavorsCrmGroup { get; set; }
        public int? UserFavorsCrmGroupID { get; set; }
    }

    public class EmployeeRole : EntityBase
    {
        public virtual Role Role { get; set; }
        public int? RoleID { get; set; }

        [Display(Name = "用户名")]
        public string AccountName { get; set; }

        [Display(Name = "部门")]
        public int? DepartmentID { get; set; }
        public virtual Department Department { get; set; }

        [Display(Name = "工作职级")]
        public int? ExpLevelID { get; set; }
        public virtual ExpLevel ExpLevel { get; set; }

        [Display(Name = "入职时间")]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "邮件地址")]
        public string Email { get; set; }
    }
}
