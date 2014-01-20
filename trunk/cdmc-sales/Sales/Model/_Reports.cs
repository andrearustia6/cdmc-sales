using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Sales.Model;

namespace Model
{
    public class ProjectWeekPerformance
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? TotalTarget { get; set; }
        public decimal? TotalCheckIn{ get; set; }
        /// <summary>
        /// 总入账目标完成百分比
        /// </summary>
        public decimal? TotalCheckInDiffer {
            get
            {
                return TotalCheckIn - TotalTarget;
            }
        }
        public double TotalPercent { get { return Utl.Utl.GetPercent(TotalCheckIn, TotalTarget); } }
        public int LeftDay { get; set; } 
        /// <summary>
        /// 入账目标
        /// </summary>
        public decimal? Target { get; set; }
        /// <summary>
        /// 出单目标
        /// </summary>
        public decimal? DealTarget { get; set; }
        public string ProjectName { get; set; }
        public int? ProjectID { get; set; }
        /// <summary>
        /// 入账实际
        /// </summary>
        public decimal? Income { get; set; }
        public decimal? Payment { get; set; }
        /// <summary>
        /// RMB出单金额
        /// </summary>
        public decimal? RMBPayment { get; set; }
        /// <summary>
        /// USD出单金额
        /// </summary>
        public decimal? USDPayment { get; set; }
        /// <summary>
        /// 入账百分比
        /// </summary>
        public double percent { get { return Utl.Utl.GetPercent(Income, Target); } }
        /// <summary>
        /// 出单百分比
        /// </summary>
        public double dealpercent { 
            get 
            {
                if (RMBPayment == null)
                    RMBPayment = 0;
                if (USDPayment == null)
                    USDPayment = 0;
                if (DealTarget == null)
                    DealTarget = 0;
                if (rate == null)
                    rate = 0;
                return Utl.Utl.GetPercent((double)RMBPayment + (double)USDPayment * rate, (double)DealTarget); 
            } 
        }
        public string Manager { get; set; }
        public string Leader { get; set; }
        /// <summary>
        /// 项目人数
        /// </summary>
        public int? MemberCount { get; set; }
        /// <summary>
        /// 货币汇率
        /// </summary>
        public double rate { get; set; }
    }
    /// <summary>
    /// 项目周进度表
    /// </summary>
    public class AjaxProjectsProgressByWeek
    {
        public int? ProjectID { get; set; }
        public string ProjectUnitName { get; set; }
        public string ProjectUnitCode { get; set; }


        /// <summary>
        /// 上周入账
        /// </summary>
        public decimal? LastWeekCheckIn { get; set; }
        /// <summary>
        /// 上周入账目标
        /// </summary>
        public decimal? LastWeekCheckInTarget { get; set; }
        /// <summary>
        /// 上周入账完成率
        /// </summary>
        public double CheckInLastWeekPercent
        {
            get
            {
                return Utl.Utl.GetPercent(LastWeekCheckIn, LastWeekCheckInTarget);
            }
        }
        /// <summary>
        /// 上周项目业绩目标
        /// </summary>
        public decimal? LastWeekDealInTarget { get; set; }
        /// <summary>
        /// 上周项目业绩rmb
        /// </summary>
        public decimal? LastWeekRMBTotalDealIn { get; set; }
        /// <summary>
        /// 上周项目业绩usd
        /// </summary>
        public decimal? LastWeekUSDTotalDealIn { get; set; }
        /// <summary>
        /// 上周业绩usd+rmb
        /// </summary>
        public decimal? LastWeekDealIn
        {
            get
            {
                if (LastWeekRMBTotalDealIn == null)
                    LastWeekRMBTotalDealIn = 0;
                if (LastWeekUSDTotalDealIn == null)
                    LastWeekUSDTotalDealIn = 0;
                return LastWeekRMBTotalDealIn + LastWeekUSDTotalDealIn * (decimal)6.3;
            }
        }
        /// <summary>
        /// 上周业绩完成率
        /// </summary>
        public double LastWeekDealInPercent
        {
            get
            {
                return Utl.Utl.GetPercent(LastWeekDealIn, LastWeekDealInTarget);
            }
        }


        /// <summary>
        /// 当周入账
        /// </summary>
        public decimal? CurrentWeekCheckIn { get; set; }
        /// <summary>
        /// 当周入账目标
        /// </summary>
        public decimal? CurrentWeekCheckInTarget { get; set; }
        /// <summary>
        /// 当周入账完成率
        /// </summary>
        public double CheckInWeekPercent
        {
            get
            {
                return Utl.Utl.GetPercent(CurrentWeekCheckIn, CurrentWeekCheckInTarget);
            }
        }
        /// <summary>
        /// 当周项目业绩目标
        /// </summary>
        public decimal? CurrentWeekDealInTarget { get; set; }
        /// <summary>
        /// 当周项目业绩rmb
        /// </summary>
        public decimal? CurrentWeekRMBTotalDealIn { get; set; }
        /// <summary>
        /// 当周项目业绩usd
        /// </summary>
        public decimal? CurrentWeekUSDTotalDealIn { get; set; }
        /// <summary>
        /// 当周业绩usd+rmb
        /// </summary>
        public decimal? CurrentWeekDealIn
        {
            get
            {
                if (CurrentWeekRMBTotalDealIn == null)
                    CurrentWeekRMBTotalDealIn = 0;
                if (CurrentWeekUSDTotalDealIn == null)
                    CurrentWeekUSDTotalDealIn = 0;
                return CurrentWeekRMBTotalDealIn + CurrentWeekUSDTotalDealIn * (decimal)6.3;
            }
        }
        /// <summary>
        /// 当周业绩完成率
        /// </summary>
        public double CurrentWeekDealInPercent
        {
            get
            {
                return Utl.Utl.GetPercent(CurrentWeekDealIn, CurrentWeekDealInTarget);
            }
        }
        /// <summary>
        /// 货币汇率
        /// </summary>
        public double rate { get; set; }
    }
    /// <summary>
    /// 个人项目周进度表
    /// </summary>
    public class AjaxMemberProjectsProgressByWeek
    {
        public int? ProjectID { get; set; }
        public string Member { get; set; }
        public string ProjectUnitName { get; set; }
        public string ProjectUnitCode { get; set; }


        /// <summary>
        /// 上周入账
        /// </summary>
        public decimal? LastWeekCheckIn { get; set; }
        /// <summary>
        /// 上周入账目标
        /// </summary>
        public decimal? LastWeekCheckInTarget { get; set; }
        /// <summary>
        /// 上周入账完成率
        /// </summary>
        public double CheckInLastWeekPercent
        {
            get
            {
                return Utl.Utl.GetPercent(LastWeekCheckIn, LastWeekCheckInTarget);
            }
        }
        /// <summary>
        /// 上周项目业绩目标
        /// </summary>
        public decimal? LastWeekDealInTarget { get; set; }
        /// <summary>
        /// 上周项目业绩rmb
        /// </summary>
        public decimal? LastWeekRMBTotalDealIn { get; set; }
        /// <summary>
        /// 上周项目业绩usd
        /// </summary>
        public decimal? LastWeekUSDTotalDealIn { get; set; }
        /// <summary>
        /// 上周业绩usd+rmb
        /// </summary>
        public decimal? LastWeekDealIn
        {
            get
            {
                if (LastWeekRMBTotalDealIn == null)
                    LastWeekRMBTotalDealIn = 0;
                if (LastWeekUSDTotalDealIn == null)
                    LastWeekUSDTotalDealIn = 0;
                return LastWeekRMBTotalDealIn + LastWeekUSDTotalDealIn * (decimal)6.3;
            }
        }
        /// <summary>
        /// 上周业绩完成率
        /// </summary>
        public double LastWeekDealInPercent
        {
            get
            {
                return Utl.Utl.GetPercent(LastWeekDealIn, LastWeekDealInTarget);
            }
        }


        /// <summary>
        /// 当周入账
        /// </summary>
        public decimal? CurrentWeekCheckIn { get; set; }
        /// <summary>
        /// 当周入账目标
        /// </summary>
        public decimal? CurrentWeekCheckInTarget { get; set; }
        /// <summary>
        /// 当周入账完成率
        /// </summary>
        public double CheckInWeekPercent
        {
            get
            {
                return Utl.Utl.GetPercent(CurrentWeekCheckIn, CurrentWeekCheckInTarget);
            }
        }
        /// <summary>
        /// 当周项目业绩目标
        /// </summary>
        public decimal? CurrentWeekDealInTarget { get; set; }
        /// <summary>
        /// 当周项目业绩rmb
        /// </summary>
        public decimal? CurrentWeekRMBTotalDealIn { get; set; }
        /// <summary>
        /// 当周项目业绩usd
        /// </summary>
        public decimal? CurrentWeekUSDTotalDealIn { get; set; }
        /// <summary>
        /// 当周业绩usd+rmb
        /// </summary>
        public decimal? CurrentWeekDealIn
        {
            get
            {
                if (CurrentWeekRMBTotalDealIn == null)
                    CurrentWeekRMBTotalDealIn = 0;
                if (CurrentWeekUSDTotalDealIn == null)
                    CurrentWeekUSDTotalDealIn = 0;
                return CurrentWeekRMBTotalDealIn + CurrentWeekUSDTotalDealIn * (decimal)6.3;
            }
        }
        /// <summary>
        /// 当周业绩完成率
        /// </summary>
        public double CurrentWeekDealInPercent
        {
            get
            {
                return Utl.Utl.GetPercent(CurrentWeekDealIn, CurrentWeekDealInTarget);
            }
        }
        /// <summary>
        /// 货币汇率
        /// </summary>
        public double rate { get; set; }
    }
    public class AjaxEmployeeCheckInDistribution
    {
        [Key]
        [Display(Name = "范围")]
        public string Range { get; set; }
        [Display(Name = "数量")]
        public int? Count { get; set; }

    }

    public class AjaxEmployeeCheckInByMonth
    {
        [Key]
        [Display(Name="姓名")]
        public string Name { get; set; }
        public decimal? CurrentMonthChickIn { get; set; }
        public decimal? OneMonthBeforeChickIn { get; set; }

        public decimal? TwoMonthBeforeChickIn { get; set; }
        public decimal? ThreeMonthBeforeChickIn { get; set; }
        public decimal? FourthMonthBeforeChickIn { get; set; }
        public decimal? FifthMonthBeforeChickIn { get; set; }
        public decimal? SixMonthBeforeChickIn { get; set; }

        public decimal? CurrentMonthTarget { get; set; }
        public decimal? OneMonthBeforeTarget { get; set; }
        public decimal? TwoMonthBeforeTarget { get; set; }
        public decimal? ThreeMonthBeforeTarget { get; set; }
        public decimal? FourthMonthBeforeTarget { get; set; }
        public decimal? FifthMonthBeforeTarget { get; set; }
        public decimal? SixMonthBeforeTarget { get; set; }

        public double? CurrentMonthBeforePercent
        {
            get
            {
                if (CurrentMonthTarget == null || CurrentMonthTarget == 0) return 0;
                return Utl.Utl.GetPercent(CurrentMonthChickIn, CurrentMonthTarget);
            }
        }
        public double? OneMonthBeforePercent
        {
            get
            {
                if (OneMonthBeforeTarget == null || OneMonthBeforeTarget == 0) return 0;
                return Utl.Utl.GetPercent(OneMonthBeforeChickIn, OneMonthBeforeTarget);
            }
        }
        public double? TwoMonthBeforePercent
        {
            get
            {
                if (TwoMonthBeforeTarget == null || TwoMonthBeforeTarget == 0) return 0;
                return Utl.Utl.GetPercent(TwoMonthBeforeChickIn, TwoMonthBeforeTarget);
            }
        }
        public double? ThreeMonthBeforePercent
        {
            get
            {
                if (ThreeMonthBeforeTarget == null || ThreeMonthBeforeTarget == 0) return 0;
                return Utl.Utl.GetPercent(ThreeMonthBeforeChickIn, ThreeMonthBeforeTarget);
            }
        }
        public double? FourthMonthBeforePercent
        {
            get
            {
                if (FourthMonthBeforeTarget == null || FourthMonthBeforeTarget == 0) return 0;
                return Utl.Utl.GetPercent(FourthMonthBeforeChickIn, FourthMonthBeforeTarget);
            }
        }
        public double? FifthMonthBeforePercent
        {
            get
            {
                if (FifthMonthBeforeTarget == null || FifthMonthBeforeTarget == 0) return 0;
                return Utl.Utl.GetPercent(FifthMonthBeforeChickIn, FifthMonthBeforeTarget);
            }
        }
        public double? SixMonthBeforePercent
        {
            get
            {
                if (SixMonthBeforeTarget == null || SixMonthBeforeTarget == 0) return 0;
                return Utl.Utl.GetPercent(SixMonthBeforeChickIn, SixMonthBeforeTarget);
            }
        }

        //public IEnumerable<AjaxEmployeeInMonth> ProjectLines { get; set; }
    }

    class AjaxEmployeeInMonth
    {
        public decimal? TotalTarget { get; set; }
        public decimal? CheckinTarget { get; set; }
        public decimal? CheckIn { get; set; }
        public decimal CallCount { get; set; }
        public double Percent { get; set; }
        public DateTime Range { get { return new DateTime(Year, Month.Value, 1); } }
        public int Year { get; set; }
        public int? Month { get; set; }
    }

    public class AjaxProjectProcess
    {
        public string Manager { get; set; }
      
        public string DisplayProjectCheckIn
        {
            get
            {
                var target = TotalCheckInTarget == null ? "目标未设置" : TotalCheckInTarget.ToString();
                var checkin = TotalCheckIn == null ? "0" : TotalCheckIn.ToString();
                var percent = TotalCheckInPercent.ToString();
                return checkin + "/" + target + ":" + percent;

            }
        }
        public string DisplayMonthCheckIn
        {
            get
            {
                var target = CurrentMonthCheckInTarget == null ? "目标未设置" : CurrentMonthCheckInTarget.ToString();
                var checkin = CurrentMonthCheckIn == null ? "0" : CurrentMonthCheckIn.ToString();
                var percent = MonthPercent.ToString();
                return checkin + "/" + target + ":" + percent;

            }
        }
        public string DisplayWeekCheckIn
        {
            get
            {
                var target = CurrentWeekTarget == null ? "目标未设置" : CurrentWeekTarget.ToString();
                var checkin = CurrentWeekCheckIn == null ? "0" : CurrentWeekCheckIn.ToString();
                var percent = WeekPercent.ToString();
                return checkin + "/" + target + ":" + percent;

            }
        }

        public string Lead { get; set; }
        public int CurrentSales { get; set; }
        public string ProjectUnitName { get; set; }
        public string ProjectUnitCode { get; set; }
        public decimal? CurrentMonthCheckIn { get; set; }
        public decimal? CurrentMonthCheckInTarget { get; set; }
        public double MonthPercent
        {
            get
            {
                return Utl.Utl.GetPercent(CurrentMonthCheckIn, CurrentMonthCheckInTarget);
            }
        }
        public decimal? CurrentDayDealIn { get; set; }
        public decimal? CurrentWeekCheckIn { get; set; }
        public decimal? CurrentWeekTarget{ get; set; }
        public double WeekPercent
        {
            get
            {
                return Utl.Utl.GetPercent(CurrentWeekCheckIn, CurrentWeekTarget);
            }
        }
        public int PassedDay { get; set; }
        public int LeftedDay { get; set; }

        public decimal? TotalCheckInTarget { get; set; }
        public decimal? TotalCheckIn { get; set; }
        public double TotalCheckInPercent
        {
            get
            {
                return Utl.Utl.GetPercent(TotalCheckIn, TotalCheckInTarget);
            }
        }
        public decimal? TotalDealInTarget { get; set; }
        public decimal? RMBTotalDealIn { get; set; }
        public decimal? USDTotalDealIn { get; set; }
        public decimal? TotalDealIn
        {
            get
            {
                if (RMBTotalDealIn == null)
                    RMBTotalDealIn = 0;
                if (USDTotalDealIn == null)
                    USDTotalDealIn = 0;
                return RMBTotalDealIn + USDTotalDealIn*(decimal)6.3;
            }
        }
        public double TotalDealInPercent
        {
            get
            {
                return Utl.Utl.GetPercent(TotalDealIn, TotalDealInTarget);
            }
        }
    }
    public class AggregatedProjectProcessByMonth
    {
        public IEnumerable<AjaxProjectProcessByMonth> bymonthes { get; set; }
        public decimal? CurrentMonthDealInTotal { get; set; }
    }
    public class AjaxProjectProcessByMonth
    {
        public string Manager { get; set; }
        public string Lead { get; set; }
        public int CurrentSales { get; set; }
        public string ProjectUnitName { get; set; }
        public string ProjectUnitCode { get; set; }
        public DateTime ConferenceStartDate { get; set; }
        /// <summary>
        /// 当月入账
        /// </summary>
        public decimal? CurrentMonthCheckIn
        {
            get
            {
                if (CurrentMonthCheckInOri == null)
                    return 0;
                else
                    return CurrentMonthCheckInOri;
            }
        }
        public decimal? CurrentMonthCheckInOri { get; set; }
        /// <summary>
        /// 当月入账目标
        /// </summary>
        public decimal? CurrentMonthCheckInTarget
        {
            get
            {
                if (CurrentMonthCheckInTargetOri == null)
                    return 0;
                else
                    return CurrentMonthCheckInTargetOri;
            }
        }

        public decimal? CurrentMonthCheckInTargetOri { get; set; }
        /// <summary>
        /// 当月入账完成率
        /// </summary>
        public double CheckInMonthPercent
        {
            get
            {
                return Utl.Utl.GetPercent(CurrentMonthCheckIn, CurrentMonthCheckInTarget);
            }
        }
       
        public int PassedDay { get; set; }
        public int LeftedDay { get; set; }

        /// <summary>
        /// 项目入账目标
        /// </summary>
        public decimal? TotalCheckInTarget
        {
            get
            {
                if (TotalCheckInTargetOri == null)
                    return 0;
                else
                    return TotalCheckInTargetOri;
            }
        }
        public decimal? TotalCheckInTargetOri { get; set; }
        /// <summary>
        /// 项目入账
        /// </summary>
        public decimal? TotalCheckIn{get;set;}
        public decimal? DispTotalCheckIn
        {
            get
            {
                if (TotalCheckIn == null)
                    return 0;
                else
                    return TotalCheckIn;

            }
        }
        public double TotalCheckInPercent
        {
            get
            {
                return Utl.Utl.GetPercent(TotalCheckIn, TotalCheckInTarget);
            }
        }
        /// <summary>
        /// 项目业绩目标
        /// </summary>
        public decimal? TotalDealInTarget
        {
            get
            {
                if (TotalDealInTargetOri == null)
                    return 0;
                else
                    return TotalDealInTargetOri;
            }
        }
        public decimal? TotalDealInTargetOri { get; set; }
        /// <summary>
        /// 项目业绩rmb
        /// </summary>
        public decimal? RMBTotalDealIn
        {
            get
            {
                if (RMBTotalDealInOri == null)
                    return 0;
                else
                    return RMBTotalDealInOri;
            }
        }
        public decimal? RMBTotalDealInOri { get; set; }
        /// <summary>
        /// 项目业绩usd
        /// </summary>
        public decimal? USDTotalDealIn
        {
            get
            {
                if (USDTotalDealInOri == null)
                    return 0;
                else
                    return USDTotalDealInOri;
            }
        }
        public decimal? USDTotalDealInOri { get; set; }
        /// <summary>
        /// 项目业绩
        /// </summary>
        public decimal? TotalDealIn
        {
            get
            {
                if (RMBTotalDealInOri == null)
                    RMBTotalDealInOri = 0;
                if (USDTotalDealInOri == null)
                    USDTotalDealInOri = 0;
                return RMBTotalDealInOri + USDTotalDealInOri * (decimal)6.3;
            }
        }
        public double TotalDealInPercent
        {
            get
            {
                return Utl.Utl.GetPercent(TotalDealIn, TotalDealInTarget);
            }
        }


        /// <summary>
        /// 当月项目业绩目标
        /// </summary>
        public decimal? CurrentMonthDealInTarget
        {
            get
            {
                if (CurrentMonthDealInTargetOri == null)
                    return 0;
                else
                    return CurrentMonthDealInTargetOri;
            }

        }
        public decimal? CurrentMonthDealInTargetOri { get; set; }
        /// <summary>
        /// 当月项目业绩rmb
        /// </summary>
        public decimal? CurrentMonthRMBTotalDealIn
        {
            get
            {
                if (CurrentMonthRMBTotalDealInOri == null)
                    return 0;
                else
                    return CurrentMonthRMBTotalDealInOri;
            }
        }
        public decimal? CurrentMonthRMBTotalDealInOri { get; set; }
        /// <summary>
        /// 当月项目业绩usd
        /// </summary>
        public decimal? CurrentMonthUSDTotalDealIn
        {
            get
            {
                if (CurrentMonthUSDTotalDealInOri == null)
                    return 0;
                else
                    return CurrentMonthUSDTotalDealInOri;
            }
        }
        public decimal? CurrentMonthUSDTotalDealInOri { get; set; }
        /// <summary>
        /// 当月业绩usd+rmb
        /// </summary>
        public decimal? CurrentMonthDealIn
        {
            get
            {
                if (CurrentMonthRMBTotalDealInOri == null)
                    CurrentMonthRMBTotalDealInOri = 0;
                if (CurrentMonthUSDTotalDealInOri == null)
                    CurrentMonthUSDTotalDealInOri = 0;
                return CurrentMonthRMBTotalDealInOri + CurrentMonthUSDTotalDealInOri * (decimal)6.3;
            }
        }
        /// <summary>
        /// 当月业绩完成率
        /// </summary>
        public double CurrentMonthDealInPercent
        {
            get
            {
                return Utl.Utl.GetPercent(CurrentMonthDealIn, CurrentMonthDealInTarget);
            }
        }

    }
    /// <summary>
    /// 项目回款总表
    /// </summary>
    public class AjaxProjectsCheckInSummary
    {
        public string ProjectUnitName { get; set; }
        public string ProjectUnitCode { get; set; }
        public DateTime ConferenceStartDate { get; set; }
        /// <summary>
        /// PDM目标
        /// </summary>
        public decimal? TotalCheckInTarget { get; set; }
        public decimal? CheckInTotal
        {
            get
            {
                if (CheckInJanuary == null)
                    CheckInJanuary = 0;
                if (CheckInFebruary == null)
                    CheckInFebruary = 0;
                if (CheckInMarch == null)
                    CheckInMarch = 0;
                if (CheckInApril == null)
                    CheckInApril = 0;
                if (CheckInMay == null)
                    CheckInMay = 0;
                if (CheckInJune == null)
                    CheckInJune = 0;
                if (CheckInJuly == null)
                    CheckInJuly = 0;
                if (CheckInAugust == null)
                    CheckInAugust = 0;
                if (CheckInSeptember == null)
                    CheckInSeptember = 0;
                if (CheckInOctober == null)
                    CheckInOctober = 0;
                if (CheckInNovember == null)
                    CheckInNovember = 0;
                if (CheckInDecember == null)
                    CheckInDecember = 0;

                return CheckInJanuary +
                    CheckInFebruary +
                    CheckInMarch +
                    CheckInApril +
                    CheckInMay +
                    CheckInJune +
                    CheckInJuly +
                    CheckInAugust +
                    CheckInSeptember +
                    CheckInOctober +
                    CheckInNovember;
            }
        }
        /// <summary>
        /// 一月
        /// </summary>
        public decimal? CheckInJanuary { get; set; }
        public decimal? CheckInFebruary { get; set; }
        public decimal? CheckInMarch { get; set; }
        public decimal? CheckInApril { get; set; }
        public decimal? CheckInMay { get; set; }
        public decimal? CheckInJune { get; set; }
        public decimal? CheckInJuly { get; set; }
        public decimal? CheckInAugust { get; set; }
        public decimal? CheckInSeptember { get; set; }
        public decimal? CheckInOctober { get; set; }
        public decimal? CheckInNovember { get; set; }
        /// <summary>
        /// 12月
        /// </summary>
        public decimal? CheckInDecember { get; set; }

    }
    /// <summary>
    /// 销售人员回款总表
    /// </summary>
    public class AjaxSalesCheckInSummary
    {
        public string Sales { get; set; }
        public DateTime? SalesStartDate { get; set; }
        public string Stars { get; set; }
        public decimal? CheckInTotal
        {
            get
            {
                if (CheckInJanuary == null)
                    CheckInJanuary = 0;
                if (CheckInFebruary == null)
                    CheckInFebruary = 0;
                if (CheckInMarch == null)
                    CheckInMarch = 0;
                if (CheckInApril == null)
                    CheckInApril = 0;
                if (CheckInMay == null)
                    CheckInMay = 0;
                if (CheckInJune == null)
                    CheckInJune = 0;
                if (CheckInJuly == null)
                    CheckInJuly = 0;
                if (CheckInAugust == null)
                    CheckInAugust = 0;
                if (CheckInSeptember == null)
                    CheckInSeptember = 0;
                if (CheckInOctober == null)
                    CheckInOctober = 0;
                if (CheckInNovember == null)
                    CheckInNovember = 0;
                if (CheckInDecember == null)
                    CheckInDecember = 0;

                return CheckInJanuary +
                    CheckInFebruary +
                    CheckInMarch +
                    CheckInApril +
                    CheckInMay +
                    CheckInJune +
                    CheckInJuly +
                    CheckInAugust +
                    CheckInSeptember +
                    CheckInOctober +
                    CheckInNovember;
            }
        }
        /// <summary>
        /// 一月
        /// </summary>
        public decimal? CheckInJanuary { get; set; }
        public decimal? CheckInFebruary { get; set; }
        public decimal? CheckInMarch { get; set; }
        public decimal? CheckInApril { get; set; }
        public decimal? CheckInMay { get; set; }
        public decimal? CheckInJune { get; set; }
        public decimal? CheckInJuly { get; set; }
        public decimal? CheckInAugust { get; set; }
        public decimal? CheckInSeptember { get; set; }
        public decimal? CheckInOctober { get; set; }
        public decimal? CheckInNovember { get; set; }
        /// <summary>
        /// 12月
        /// </summary>
        public decimal? CheckInDecember { get; set; }

    }
    /// <summary>
    /// 销售人员月进度表
    /// </summary>
    public class AjaxMemberProjectProcessByMonth
    {
        public string Manager { get; set; }
        public string Member { get; set; }
        public string ProjectUnitName { get; set; }
        public string ProjectUnitCode { get; set; }
        public DateTime? ConferenceStartDate { get; set; }
        /// <summary>
        /// 个人项目到账目标
        /// </summary>
        public decimal? TotalCheckInTarget
        {
            get
            {
                if (TotalCheckInTargetOri == null)
                    return 0;
                else
                    return TotalCheckInTargetOri;
            }
        }
        public decimal? TotalCheckInTargetOri { get; set; }
        /// <summary>
        /// 个人项目业绩rmb
        /// </summary>
        public decimal? TotalDealInRMB
        {
            get
            {
                if (TotalDealInRMBOri == null)
                    return 0;
                else
                    return TotalDealInRMBOri;
            }
        }
        public decimal? TotalDealInRMBOri { get; set; }
        /// <summary>
        /// 个人项目业绩usd
        /// </summary>
        public decimal? TotalDealInUSD
        {
            get
            {
                if (TotalDealInUSDOri == null)
                    return 0;
                else
                    return TotalDealInUSDOri;
            }
        }
        public decimal? TotalDealInUSDOri { get; set; }
        /// <summary>
        /// 个人项目业绩usd+rmb
        /// </summary>
        public decimal? TotalDealIn
        {
            get
            {
                if (TotalDealInRMBOri == null)
                    TotalDealInRMBOri = 0;
                if (TotalDealInUSDOri == null)
                    TotalDealInUSDOri = 0;
                return TotalDealInRMBOri + TotalDealInUSDOri * (decimal)6.3;
            }
        }

        /// <summary>
        /// 个人项目入账
        /// </summary>
        public decimal? TotalCheckIn
        {
            get
            {
                if (TotalCheckInOri == null)
                    return 0;
                else
                    return TotalCheckInOri;
            }
        }
        public decimal? TotalCheckInOri { get; set; }
        /// <summary>
        /// 当月入账
        /// </summary>
        public decimal? CurrentMonthCheckIn
        {
            get
            {
                if (CurrentMonthCheckInOri == null)
                    return 0;
                else
                    return CurrentMonthCheckInOri;
            }
        }

        public decimal? CurrentMonthCheckInOri { get; set; }
        /// <summary>
        /// 当月入账目标
        /// </summary>
        public decimal? CurrentMonthCheckInTarget
        {
            get
            {
                if (CurrentMonthCheckInTargetOri == null)
                    return 0;
                else
                    return CurrentMonthCheckInTargetOri;
            }
        }
        public decimal? CurrentMonthCheckInTargetOri { get; set; }
        /// <summary>
        /// 当月入账完成率
        /// </summary>
        public double CheckInMonthPercent
        {
            get
            {
                return Utl.Utl.GetPercent(CurrentMonthCheckIn, CurrentMonthCheckInTarget);
            }
        }

        public int PassedDay { get; set; }
        public int LeftedDay { get; set; }

        
        


        /// <summary>
        /// 当月项目业绩目标
        /// </summary>
        public decimal? CurrentMonthDealInTarget
        {
            get
            {
                if (CurrentMonthDealInTargetOri == null)
                    return 0;
                else
                    return CurrentMonthDealInTargetOri;
            }
        }
        public decimal? CurrentMonthDealInTargetOri { get; set; }
        /// <summary>
        /// 当月项目业绩rmb
        /// </summary>
        public decimal? CurrentMonthRMBTotalDealIn
        {
            get
            {
                if (CurrentMonthRMBTotalDealInOri == null)
                    return 0;
                else
                    return CurrentMonthRMBTotalDealInOri;
            }
        }
        public decimal? CurrentMonthRMBTotalDealInOri { get; set; }
        /// <summary>
        /// 当月项目业绩usd
        /// </summary>
        public decimal? CurrentMonthUSDTotalDealIn
        {
            get
            {
                if (CurrentMonthUSDTotalDealInOri == null)
                    return 0;
                else
                    return CurrentMonthUSDTotalDealInOri;
            }
        }
        public decimal? CurrentMonthUSDTotalDealInOri { get; set; }
        /// <summary>
        /// 当月业绩usd+rmb
        /// </summary>
        public decimal? CurrentMonthDealIn
        {
            get
            {
                if (CurrentMonthRMBTotalDealInOri == null)
                    CurrentMonthRMBTotalDealInOri = 0;
                if (CurrentMonthUSDTotalDealInOri == null)
                    CurrentMonthUSDTotalDealInOri = 0;
                return CurrentMonthRMBTotalDealInOri + CurrentMonthUSDTotalDealInOri * (decimal)6.3;
            }
        }
        /// <summary>
        /// 当月业绩完成率
        /// </summary>
        public double CurrentMonthDealInPercent
        {
            get
            {
                return Utl.Utl.GetPercent(CurrentMonthDealIn, CurrentMonthDealInTarget);
            }
        }

    }
    /// <summary>
    /// 销售人员月进度表
    /// </summary>
    public class AjaxSalesMonthTargetSummary
    {
        public string Member { get; set; }
        public string ProjectUnitName { get; set; }
        public string ProjectUnitCode { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsConfirm { get; set; }
        public string dispConfirm
        {
            get
            {
                if (IsConfirm == null)
                    return "否";
                else if(IsConfirm==false)
                    return "否";
                else if(IsConfirm==true)
                    return "是";
                return "否";
            }
        }
        /// <summary>
        /// 业绩目标
        /// </summary>
        public decimal DealTarget { get; set; }
        /// <summary>
        /// 入账目标
        /// </summary>
        public decimal CheckInTarget { get; set; }
    }
    public class AjaxCompanyDailyReceivedPayment
    {
        /// <summary>
        /// 入账日期
        /// </summary>
        public DateTime? CheckInDate { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string Abstract{ get; set; }
        public string Sales { get; set; }
        public string CompanyName { get; set; }
        public string ProjectName { get; set; }
        public string Remark { get; set; }
        /// <summary>
        /// 入账金额
        /// </summary>
        public decimal? CheckIn {
            get
            {
                if (CheckInRMB == null)
                    CheckInRMB = 0;
                if (CheckInUSD == null)
                    CheckInUSD = 0;
                return CheckInRMB + CheckInUSD * (decimal)6.3;
            }
        }

        public decimal? CheckInRMB { get; set; }
        public decimal? CheckInUSD { get; set; }

    }
    public class AjaxProjectCheckInByMonth
    {
        [Key]
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string Manager { get; set; }

        public decimal? CurrentMonthChickIn { get; set; }
        public decimal? OneMonthBeforeChickIn { get; set; }
        public decimal? TwoMonthBeforeChickIn { get; set; }
        public decimal? ThreeMonthBeforeChickIn { get; set; }
        public decimal? FourthMonthBeforeChickIn { get; set; }
        public decimal? FifthMonthBeforeChickIn { get; set; }
        public decimal? SixMonthBeforeChickIn { get; set; }

        public decimal? CurrentMonthTarget { get; set; }
        public decimal? OneMonthBeforeTarget { get; set; }
        public decimal? TwoMonthBeforeTarget { get; set; }
        public decimal? ThreeMonthBeforeTarget { get; set; }
        public decimal? FourthMonthBeforeTarget { get; set; }
        public decimal? FifthMonthBeforeTarget { get; set; }
        public decimal? SixMonthBeforeTarget { get; set; }

        public double? CurrentMonthBeforePercent
        {
            get
            {
                if (CurrentMonthTarget == null || CurrentMonthTarget == 0) return 0;
                return Utl.Utl.GetPercent(CurrentMonthChickIn, CurrentMonthTarget);
            }
        }
        public double? OneMonthBeforePercent
        {
            get
            {
                if (OneMonthBeforeTarget == null || OneMonthBeforeTarget == 0) return 0;
                return Utl.Utl.GetPercent(OneMonthBeforeChickIn, OneMonthBeforeTarget);
            }
        }
        public double? TwoMonthBeforePercent
        {
            get
            {
                if (TwoMonthBeforeTarget == null || TwoMonthBeforeTarget == 0) return 0;
                return Utl.Utl.GetPercent(TwoMonthBeforeChickIn, TwoMonthBeforeTarget);
            }
        }
        public double? ThreeMonthBeforePercent
        {
            get
            {
                if (ThreeMonthBeforeTarget == null || ThreeMonthBeforeTarget == 0) return 0;
                return Utl.Utl.GetPercent(ThreeMonthBeforeChickIn, ThreeMonthBeforeTarget);
            }
        }
        public double? FourthMonthBeforePercent
        {
            get
            {
                if (FourthMonthBeforeTarget == null || FourthMonthBeforeTarget == 0) return 0;
                return Utl.Utl.GetPercent(FourthMonthBeforeChickIn, FourthMonthBeforeTarget);
            }
        }
        public double? FifthMonthBeforePercent
        {
            get
            {
                if (FifthMonthBeforeTarget == null || FifthMonthBeforeTarget == 0) return 0;
                return Utl.Utl.GetPercent(FifthMonthBeforeChickIn, FifthMonthBeforeTarget);
            }
        }
        public double? SixMonthBeforePercent
        {
            get
            {
                if (SixMonthBeforeTarget == null || SixMonthBeforeTarget == 0) return 0;
                return Utl.Utl.GetPercent(SixMonthBeforeChickIn, SixMonthBeforeTarget);
            }
        }

        public IEnumerable<AjaxProjectPerformanceInMonth> ProjectLines { get; set; }
    }

    public class AjaxProjectCheckInByWeek
    {
        [Key]
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string Manager { get; set; }
        public decimal? FirstWeekCheckIn { get; set; }
        public decimal? SencondWeekCheckIn { get; set; }
        public decimal? ThirdWeekCheckIn { get; set; }
        public decimal? FourWeekCheckIn { get; set; }
        public decimal? FifthWeekCheckIn { get; set; }
        public decimal? TotalCheckIn { get; set; }
    }

    

    public class AjaxProjectCheckInMonthByProjectType
    {
        [Display(Name = "考察入账")]
        public decimal? KaoChaCheckIn { get; set; }
        [Display(Name = "Events入账")]
        public decimal? EventsCheckIn { get; set; }
        [Display(Name = "杂志入账")]
        public decimal? MagazineCheckIn { get; set; }
        [Display(Name = "青岛分公司入账")]
        public decimal? QingDaoSubComanyCheckIn { get; set; }
        [Display(Name = "考察入账目标")]
        public decimal? KaoChaTarget { get; set; }
        [Display(Name = "Events入账目标")]
        public decimal? EventsTarget { get; set; }
        [Display(Name = "杂志入账目标")]
        public decimal? MagazineTarget { get; set; }
        [Display(Name = "青岛分公司入账目标")]
        public decimal? QingDaoSubComanyTarget { get; set; }

        [Display(Name = "考察入账完成百分比")]
        public decimal? KaoChaPercent { get; set; }
        [Display(Name = "Events入账完成百分比")]
        public decimal? EventsPercent { get; set; }
        [Display(Name = "杂志入账完成百分比")]
        public decimal? MagazinePercent { get; set; }
        [Display(Name = "青岛分公司入账完成百分比")]
        public decimal? QingDaoSubComanyPercent { get; set; }

        // public double Percent { get; set; }
        public DateTime Range { get { return new DateTime(Year, Month, 1); } }
        public int Year { get; set; }
        public int Month { get; set; }
    }


    public class AjaxProjectPerformanceInMonth
    {
        public decimal? TotalTarget { get; set; }
        public decimal? CheckinTarget { get; set; }
        public decimal? CheckIn { get; set; }
        public decimal CallCount { get; set; }
        public double Percent { get; set; }
        public DateTime Range { get { return new DateTime(Year, Month.Value, 1); } }
        public int Year { get; set; }
        public int? Month { get; set; }
        public string MonthString { get { return Month.ToString() + "月"; } }
    }

    public class AjaxProjectChartInMonthHeader
    {
        public string ProjectName{ get; set; }
        public string ProjectCode { get; set; }
        public IEnumerable<AjaxProjectChartInMonth> ProjectLines { get; set; }
    }
    public class AjaxProjectChartInMonth
    {
        public decimal? CheckInTarget { get; set; }
        public decimal? CheckIn { get; set; }
        public decimal? DealInTarget { get; set; }
        public decimal? DealIn { get; set; }
        public int? Month { get; set; }
        public string MonthString { get { return Month.ToString() + "月"; } }
    }
    public class AjaxMember
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public int? ProjectID { get; set; }
    }

    public class AjaxProjectPerformance
    {
        public double LeftDays { get; set; }
        public double Duration { get; set; }
        public decimal? Target { get; set; }
        public string ProjectCode { get; set; }
        public string Name_CH { get; set; }
        public IEnumerable<AjaxMember> Memebers { get; set; }
        public int ProjectID { get; set; }
        public IEnumerable<AjaxProjectPerformanceInMonth> AjaxProjectPerformanceInMonths { get; set; }

    }

    public class AjaxProject
    {
        public string ProjectName { get; set; }
        public string ProjectCode { get; set; }
        public int ProjectID { get; set; }
        public IEnumerable<AjaxCRM> CRMs { get; set; }
    }
}