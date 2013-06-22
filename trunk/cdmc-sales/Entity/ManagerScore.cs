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
        public int? Responsibility { get; set; }
        [Display(Name = "纪律性")]
        public int? Discipline { get; set; }
        [Display(Name = "执行能力")]
        public int? Excution { get; set; }
        [Display(Name = "目标意识")]
        public int? Targeting { get; set; }
        [Display(Name = "每天检查团队成员research,call list,on phone时间")]
        public int? Searching { get; set; }
        [Display(Name = "每周与研发人员的项目进度协调")]
        public int? Production { get; set; }
        [Display(Name = "每周更新Pitch paper/Email cover/EB内容，帮助组员找到针对不同客户的Pitch点")]
        public int? PitchPaper { get; set; }
        [Display(Name = "每周销售例会")]
        public int? WeeklyMeeting { get; set; }
        [Display(Name = "每月通话时间")]
        public int? MonthlyMeeting { get; set; }
       
        [Display(Name = "月")]
        public int? Month { get; set; }
        [Display(Name = "年")]
        public int? Year { get; set; }

        [Display(Name = "是否确认")]
        public bool? Confirmed { get; set; }
     
    }

    
}
