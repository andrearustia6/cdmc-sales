using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
    public class PreCommission : EntityBase
    {
        [Display(Name = "提成单号")]
        public string CommID { get; set; }
        [Display(Name = "开始日期")]
        public DateTime StartDate { get; set; }
        [Display(Name = "结束日期")]
        public DateTime EndDate { get; set; }

        [Display(Name = "项目")]
        public string ProjectNames { get; set; }
        [Display(Name = "目标")]
        public string TargetNameEN { get; set; }
        [Display(Name = "中文名")]
        public string TargetNameCN { get; set; }
        [Display(Name = "国内外")]
        public string InOut { get; set; }
        [Display(Name = "小于3000参会人数")]
        public int? DelegateLessCount { get; set; }
        [Display(Name = "小于3000入账金额")]
        public double? DelegateLessIncome { get; set; }
        [Display(Name = "大于3000参会人数")]
        public int? DelegateMoreCount { get; set; }
        [Display(Name = "大于3000入账金额")]
        public double? DelegateMoreIncome { get; set; }
        [Display(Name = "Sponsor入账金额")]
        public double? SponsorIncome { get; set; }
        [Display(Name = "入账总额")]
        public double? Income { get; set; }
        [Display(Name = "冲销金额")]
        public double? ReturnIncome { get; set; }

        [Display(Name = "冲销原因")]
        public string ReturnReason { get; set; }

        [Display(Name = "提成比率")]
        public double? CommissionRate { get; set; }

        [Display(Name = "提成额")]
        public double? Commission { get; set; }

        [Display(Name = "扣税")]
        public double? Tax { get; set; }

        [Display(Name = "扣奖金")]
        public double? Bonus { get; set; }

        [Display(Name = "实际提成")]
        public double? ActualCommission { get; set; }

    }

    
}
