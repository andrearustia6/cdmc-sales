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
        public int? ProjectID { get; set; }
        [Display(Name = "目标")]
        public string TargetNameEN { get; set; }
        [Display(Name = "中文名")]
        public string TargetNameCN { get; set; }
        [Display(Name = "国内外")]
        public string InOut { get; set; }
        [Display(Name = "小于3000入账金额")]
        public decimal? DelegateLessIncome { get; set; }
        [Display(Name = "小于3000入账提成比率")]
        public double? DelegateLessRate { get; set; }
        [Display(Name = "小于3000入账提成额")]
        public decimal? DelegateLessCommission { get; set; }
        [Display(Name = "大于3000参会人数")]
        public int? DelegateMoreCount { get; set; }
        [Display(Name = "大于3000入账金额")]
        public decimal? DelegateMoreIncome { get; set; }
        [Display(Name = "大于3000入账提成率")]
        public double? DelegateMoreRate { get; set; }
        [Display(Name = "大于3000入账提成额")]
        public decimal? DelegateMoreCommission{ get; set; }

        [Display(Name = "Sponsor入账金额")]
        public decimal? SponsorIncome { get; set; }

        [Display(Name = "Sponsor入账提成额")]
        public double? SponsorRate { get; set; }

        [Display(Name = "Sponsor入账提成额")]
        public decimal? SponsorCommission{ get; set; }

        [Display(Name = "Delegate入账金额")]
        public decimal? DelegateIncome { get; set; }

        [Display(Name = "Delegate入账提成额")]
        public double? DelegateRate { get; set; }

        [Display(Name = "Delegate入账提成额")]
        public decimal? DelegateCommission { get; set; }



        [Display(Name = "入账总额")]
        public decimal? Income { get; set; }
        [Display(Name = "冲销金额")]
        public decimal? ReturnIncome { get; set; }

        [Display(Name = "冲销原因")]
        public string ReturnReason { get; set; }

        [Display(Name = "提成比率")]
        public double? CommissionRate { get; set; }

        [Display(Name = "提成额")]
        public decimal? Commission { get; set; }

        [Display(Name = "扣税")]
        public decimal? Tax { get; set; }

        [Display(Name = "扣奖金")]
        public decimal? Bonus { get; set; }

        [Display(Name = "实际提成")]
        public decimal? ActualCommission { get; set; }

        [Display(Name = "应发提成")]
        public decimal? TotalCommission { get; set; }


    }
    public class FinalCommission : EntityBase
    {
        [Display(Name = "提成单号")]
        public string CommID { get; set; }

        [Display(Name = "项目")]
        public int? ProjectID { get; set; }
        [Display(Name = "目标")]
        public string TargetNameEN { get; set; }
        [Display(Name = "中文名")]
        public string TargetNameCN { get; set; }
        [Display(Name = "国内外")]
        public string InOut { get; set; }
        [Display(Name = "小于3000入账金额")]
        public decimal? DelegateLessIncome { get; set; }
        [Display(Name = "小于3000入账提成比率")]
        public double? DelegateLessRate { get; set; }
        [Display(Name = "小于3000入账提成额")]
        public decimal? DelegateLessCommission { get; set; }
        [Display(Name = "小于3000已发放额")]
        public decimal? DelegateLessPayed { get; set; }

        [Display(Name = "大于3000参会人数")]
        public int? DelegateMoreCount { get; set; }
        [Display(Name = "大于3000入账金额")]
        public decimal? DelegateMoreIncome { get; set; }
        [Display(Name = "大于3000入账提成率")]
        public double? DelegateMoreRate { get; set; }
        [Display(Name = "大于3000入账提成额")]
        public decimal? DelegateMoreCommission { get; set; }
        [Display(Name = "大于3000已发放额")]
        public decimal? DelegateMorePayed { get; set; }

        [Display(Name = "Sponsor入账金额")]
        public decimal? SponsorIncome { get; set; }

        [Display(Name = "Sponsor入账提成率")]
        public double? SponsorRate { get; set; }

        [Display(Name = "Sponsor入账提成额")]
        public decimal? SponsorCommission { get; set; }
        [Display(Name = "Sponsor已发放额")]
        public decimal? SponsorPayed { get; set; }

        [Display(Name = "Delegate入账金额")]
        public decimal? DelegateIncome { get; set; }

        [Display(Name = "Delegate入账提成率")]
        public double? DelegateRate { get; set; }

        [Display(Name = "Delegate入账提成额")]
        public decimal? DelegateCommission { get; set; }
        [Display(Name = "Delegate已发放额")]
        public decimal? DelegatePayed { get; set; }



        [Display(Name = "入账总额")]
        public decimal? Income { get; set; }
        [Display(Name = "冲销金额")]
        public decimal? ReturnIncome { get; set; }

        [Display(Name = "冲销原因")]
        public string ReturnReason { get; set; }

        [Display(Name = "提成比率")]
        public double? CommissionRate { get; set; }

        [Display(Name = "提成额")]
        public decimal? Commission { get; set; }
        [Display(Name = "已发放额")]
        public decimal? CommissionPayed { get; set; }
        [Display(Name = "管理提成")]
        public decimal? ManageCommission { get; set; }

        [Display(Name = "扣税")]
        public decimal? Tax { get; set; }

        [Display(Name = "扣奖金")]
        public decimal? Bonus { get; set; }

        [Display(Name = "实际提成")]
        public decimal? ActualCommission { get; set; }

        [Display(Name = "应发提成")]
        public decimal? TotalCommission { get; set; }
    }
    
}
