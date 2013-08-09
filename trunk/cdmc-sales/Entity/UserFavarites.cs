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

        [Display(Name = "英文名")]
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

        [Display(Name = "中文名"), Required]
        public string AccountNameCN { get; set; }

        [Display(Name = "座机")]
        public int? AgentNum { get; set; }

        [Display(Name = "是否实习")]
        public bool? IsTrainee { get; set; }

        [Display(Name = "是否激活")]
        public bool IsActivated { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "移动电话")]
        public string Mobile { get; set; }

        [Display(Name = "生日")]
        public DateTime? BirthDay { get; set; }

        [Display(Name = "员工性别")]
        public string Gender { get; set; }
    }
}
