using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
    public class ManagerScore : EntityBase
    {
        [Display(Name = "员工")]
        public string TargetName { get; set; }
        [Display(Name = "评分人")]
        public string Assigner { get; set; }
        [Display(Name = "责任心与积极性")]
        public int? Item1Score { get; set; }
        [Display(Name = "纪律性")]
        public int? Item2Score { get; set; }
        [Display(Name = "执行能力")]
        public int? Item3Score { get; set; }
        [Display(Name = "目标意识")]
        public int? Item4Score { get; set; }
        [Display(Name = "每天检查团队成员research,call list,on phone时间")]
        public int? Item5Score { get; set; }
        [Display(Name = "每周与研发人员的项目进度协调")]
        public int? Item6Score { get; set; }
        [Display(Name = "每周更新Pitch paper/Email cover/EB内容，帮助组员找到针对不同客户的Pitch点")]
        public int? Item7Score { get; set; }
        [Display(Name = "每周销售例会")]
        public int? Item8Score { get; set; }
        [Display(Name = "每月通话时间")]
        public int? Item9Score { get; set; }
        [Display(Name = "团队Call List")]
        public int? Item10Score { get; set; }
        [Display(Name = "团队新增Leads")]
        public double? Item11Score { get; set; }
        [Display(Name = "团队业绩表现")]
        public double? Item12Score { get; set; }
       
        [Display(Name = "月")]
        public int? Month { get; set; }
        [Display(Name = "年")]
        public int? Year { get; set; }
     
    }

    
}
