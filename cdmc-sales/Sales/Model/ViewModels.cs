using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entity;

namespace Model
{
    public class ViewCallListChart
    {
      
        DateTime? StartDate { get; set; }
        DateTime? EndDate { get; set; }
        public Member Member { get; set; }
        public List<ViewCompanyCallSum> ViewCompanyCallSums { get; set; }
        public List<ViewCategoryCallSum> ViewCategoryCallSum { get; set; }
      
    }
    public class ViewCategoryCallSum
    {

        //打了几个Company
        public int CompanyCalledCountNumber { get; set; }

        //Category的名称 
        public string CategoryName { get; set; }

        public string CategoryCountName { get { return CategoryName + ": 已打" + CompanyCalledCountNumber + "个公司"; } }
    }

    public class ViewCompanyCallSum
    {
        //打了几个lead
        public int LeadCalledCountNumber { get; set; }

        //大了X个lead的公司的个数 
        public int CompanyCount { get; set; }

        public string CompanyCountName { get { return "已打" + LeadCalledCountNumber + "个Lead" + "(" + CompanyCount + "家 )"; } }
    }


    public class JosonSalesInputData
    {
        public string SubmitType { get; set; }
        //CRID 为空,为添加新的公司的数据,crid不为空，为存在的公司添加销售数据
        public int? CRID { get; set; }
        public int? ProjectID { get; set; }
        public Company Company { get; set; }
        public Lead Lead { get; set; }
        public LeadCall LeadCall { get; set; }
        public string Message { get; set; }
        public bool Satisfied { get; set; }
    }

    public class  JosonCompany
    {
        public string username { get; set; }
        public Company Company { get; set; }
        public List<Lead> Leads { get; set; }
        public CompanyRelationship CompanyRelationship { get; set; }
        public List<LeadCall> LeadCalls { get; set; }
    }
    public class ViewPhoneInfo
    {
        public TimeSpan Duration { get; set; }
        public string Phone { get; set; }
        public int CallSum { get; set; }
        

    }

    public class ViewContactedLead
    {
        public int? CompanyRelationshipID { get; set; }
        public Lead Lead { get; set; }
        public List<LeadCall> LeadCalls{get ;set;}
        public LeadCall LastCall { get; set; }
        public int ID { get; set; }
    }

    public class ViewLeadCallAmount
    {
        public Member Member { get; set; }
        public int Others { get; set; }
        public int Blowed { get; set; }
        public int Not_Pitched { get; set; }
        public int Pitched { get; set; }
        public int Full_Pitched { get; set; }
        public int Call_Backed { get; set; }
        public int Waiting_For_Approval { get; set; }
        public int Qualified_Decision { get; set; }
        public int Closed { get; set; }
        public int Cold_Calls { get; set; }
        public int CallListAmount { get; set; }
        public int DMs { get; set; }
        public int New_DMs { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime? startdate { get; set; }
        public DateTime? enddate { get; set; }
        public decimal DealInAmount { get; set; }
        public decimal CheckInAmount { get; set; }
        public string Phone { get; set; }
    }

    public class ViewLeadCallAmountInProject
    {
        public Project project { get; set; }
        public List<ViewLeadCallAmount> LeadCallAmounts { get; set; }
        //public List<Member> _topSales;
        //decimal? topSalesAmount;
        //public decimal TopSalesAmount { get {
        //    if (topSalesAmount==null)
        //            topSalesAmount = LeadCallAmounts.Count > 0 ? LeadCallAmounts.Max(d => d.DealInAmount) : 0;
        //    return (decimal)topSalesAmount;
        //}
        //}
        //public List<Member> TopSales
        //{
        //    get
        //    {
        //        if (_topSales == null)
        //        {
        //            _topSales = new List<Member>();
        //            var amounts = LeadCallAmounts.FindAll(m => m.DealInAmount == TopSalesAmount);
        //            amounts.ForEach(m =>
        //            {
        //                _topSales.Add(m.Member);
        //            });

        //        }
        //        return _topSales;

        //    }
        //}
        //public TimeSpan TopCallerAmount { get; set; }
        //public List<Member> _topCallers;
        //public List<Member> TopCallers
        //{
        //    get
        //    {
        //        if (_topCallers == null)
        //        {
        //            _topCallers = new List<Member>();
        //            var amounts = LeadCallAmounts.FindAll(m => m.Duration == LeadCallAmounts.Max(d => d.Duration));
        //            TopCallerAmount = amounts.FirstOrDefault().Duration;
        //            amounts.ForEach(m =>
        //            {
        //                _topCallers.Add(m.Member);
        //            });

        //        }
        //        return _topCallers;
        //    }
        //}
        //public TimeSpan WorstCallerAmount { get; set; }
        //public List<Member> _worstCallers;
        //public List<Member> WorstCallers
        //{
        //    get
        //    {
        //        if (_worstCallers == null)
        //        {
        //            _worstCallers = new List<Member>();
        //            //时间最短，入职超过2星期并且没有出单
        //            var amounts = LeadCallAmounts.FindAll(m => m.Duration == LeadCallAmounts.Min(d => d.Duration) && m.DealInAmount == 0 && m.Member.EmployeeDuration() > 0.5);
        //            var wa = amounts.FirstOrDefault();
        //            if (wa == null)
        //                WorstCallerAmount = new TimeSpan();
        //            else
        //                WorstCallerAmount = wa.Duration;

        //            amounts.ForEach(m =>
        //            {
        //                _worstCallers.Add(m.Member);
        //            });

        //        }
        //        return _worstCallers;
        //    }
        //}

    }

    public class TotalLeadCallAmount
    {
        public List<ViewLeadCallAmount> TopSales { get; set; }
        public decimal TopSalesAmount { get; set; }
        public List<ViewLeadCallAmount> WorstCallers { get; set; }
        public List<ViewLeadCallAmount> TopCallers { get; set; }
        public List<ViewLeadCallAmountInProject> ViewLeadCallAmountInProjects { get; set; }
        //List<Member> _topSales = new List<Member>();
        //public List<Member> TopSales
        //{
        //    get
        //    {

        //        if (_topSales.Count == 0)
        //        {
        //            ViewLeadCallAmountInProjects.ForEach(vp =>
        //            {
        //                if (_topSales.Count == 0)
        //                {
        //                    _topSales.AddRange(vp.TopSales);
        //                    TopSalesAmount = vp.TopSalesAmount;
        //                }
        //                else
        //                {
        //                    if (vp.TopSalesAmount == TopSalesAmount)
        //                    {
        //                        _topSales.AddRange(vp.TopSales);
        //                    }
        //                    else if (vp.TopSalesAmount > TopSalesAmount)
        //                    {
        //                        _topSales.Clear();
        //                        _topSales.AddRange(vp.TopSales);
        //                        TopSalesAmount = vp.TopSalesAmount;
        //                    }
        //                }
        //            });
        //        }
        //        return _topSales;
        //    }
        //}
        //List<Member> _topCallers = new List<Member>();
        //public List<Member> TopCallers
        //{
        //    get
        //    {
        //        if (_topCallers.Count == 0)
        //        {
        //            ViewLeadCallAmountInProjects.ForEach(vp =>
        //            {
        //                if (_topCallers.Count == 0)
        //                {
        //                    _topCallers.AddRange(vp.TopCallers);
        //                    TopCallerAmount = vp.TopCallerAmount;
        //                }
        //                else
        //                {
        //                    if (vp.TopCallerAmount == TopCallerAmount)
        //                    {
        //                        _topCallers.AddRange(vp.TopCallers);
        //                    }
        //                    else if (vp.TopCallerAmount > TopCallerAmount)
        //                    {
        //                        _topCallers.Clear();
        //                        _topCallers.AddRange(vp.TopCallers);
        //                    }
        //                }
        //            });
        //        }

        //        return _topSales;
        //    }
        //}
        //List<Member> _worstCallers = new List<Member>();
        //public List<Member> WorstCallers
        //{
        //    get
        //    {
        //        if (_worstCallers.Count == 0)
        //        {
        //            ViewLeadCallAmountInProjects.ForEach(vp =>
        //            {
        //                if (_worstCallers.Count == 0)
        //                {
        //                    _worstCallers.AddRange(vp.TopCallers);
        //                    WorstCallerAmount = vp.WorstCallerAmount;
        //                }
        //                else
        //                {
        //                    if (vp.WorstCallerAmount == WorstCallerAmount)
        //                    {
        //                        _worstCallers.AddRange(vp.WorstCallers);
        //                    }
        //                    else if (vp.WorstCallerAmount < TopCallerAmount)
        //                    {
        //                        _worstCallers.Clear();
        //                        _worstCallers.AddRange(vp.WorstCallers);
        //                    }
        //                }

        //            });
        //        }
        //        return _worstCallers;
        //    }



        //}

     
    }

    public class ViewProjectProgressAmount : ViewProgressAmount
    {
        public Project Project { get; set; }
        public int LeftDay { get; set; }
        public string NameWithCompletePercentage { get { 
            var v =   Project.Name+ "出单目标完成: " + DealInPercentage.ToString()+"%" +"  入账目标完成: " + CheckInPercentage.ToString()+"%";
            return v;
        } 
        }
    }

    public class ViewProjectMemberProgressAmount
    {
        public Project Project { get; set; }
        public List<ViewMemberProgressAmount> ViewMemberProgressAmounts { get; set; }
    }

    public class ViewProgressAmount 
    {
        public decimal TotalDealIn { get; set; }
        public decimal TotalCheckIn { get; set; }
        
        public decimal DealIn { get; set; }
        public decimal DealInTarget { get; set; }
        public decimal CheckIn { get; set; }
        public decimal CheckInTarget { get; set; }
        public int DealInPercentage { get; set; }
        public int CheckInPercentage { get; set; }
        public decimal NextDealInTarget { get; set; }
        public decimal NextCheckInTarget { get; set; }
    }

    public class ViewMemberProgressAmount : ViewProgressAmount
    {
        public Member Member { get; set; }
        public decimal? TotalDealinTarget{ get; set;}
    }

    public class ViewMemberLeadToCall
    {
        public LeadCall LeadCall { get; set; }
        public string LeadName { get { return LeadCall.Lead.Name; } }
        public string LeadTitle { get { return LeadCall.Lead.Title; } }
        public string Contact { get { return LeadCall.Lead.Contact; } }
        public string Mobile { get { return LeadCall.Lead.Mobile; } }
        public string Companyname { get { return LeadCall.CompanyRelationship.Company.Name; } }
        public string ProjectCode { get { return LeadCall.CompanyRelationship.Project.ProjectCode; } }
        public string ProjectName { get { return LeadCall.CompanyRelationship.Project.Name; } }
        public DateTime CallDateTime
        {
            get
            {
                var date = LeadCall.CallBackDate.Value;
                if (LeadCall.CompanyRelationship.Company.DistrictNumber != null)
                {
                    var differs = LeadCall.CompanyRelationship.Company.DistrictNumber.TimeDifference;
                    date = date.AddHours(-differs);
                }
                return date;
            }
        }

    }
}