using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entity;
using Utl;
using System.Web.Security;
using Model;
using System.Data.Objects;
using Sales.Model;
using System.Web.Mvc;

namespace BLL
{
    public class Finance_Logical
    {
        public static class _PreCommissionBLL
        {
            public static IEnumerable<_PreCommission> GetPreCommission(int month, string fuzzyInput = "")
            {
                var lps = CH.DB.PreCommissions.Where(p => p.StartDate.Month == month);
                var ps = from pre in lps
                         join p in CH.DB.Projects on pre.ProjectID equals  p.ID
                        select new _PreCommission
                        {
                            CommID=pre.CommID,
                            StartDate=pre.StartDate,
                            EndDate=pre.EndDate,

                            ID = pre.ID,
                            ProjectID = pre.ProjectID,
                            ProjectName=p.ProjectCode,
                            Income = pre.Income,
                            TargetNameEN = pre.TargetNameEN,
                            TargetNameCN = pre.TargetNameCN,
                            InOut = pre.InOut,
                            DelegateLessIncome = pre.DelegateLessIncome,
                            DelegateMoreCount = pre.DelegateMoreCount,
                            DelegateMoreIncome = pre.DelegateMoreIncome,
                            SponsorIncome = pre.SponsorIncome == null ? 0 : pre.SponsorIncome.Value,
                            Commission=pre.Commission,
                            CommissionRate=pre.CommissionRate,
                            Tax=pre.Tax,
                            Bonus=pre.Bonus,
                            ReturnIncome=pre.ReturnIncome,
                            ReturnReason=pre.ReturnReason,
                            ActualCommission=pre.ActualCommission,
                            DelegateLessRate=pre.DelegateLessRate,
                            DelegateLessCommission=pre.DelegateLessCommission,
                            DelegateMoreRate=pre.DelegateMoreRate,
                            DelegateMoreCommission = pre.DelegateMoreCommission,
                            SponsorCommission = pre.SponsorCommission,
                            SponsorRate = pre.SponsorRate,
                            TotalCommission=pre.TotalCommission
                        };
                return ps;
            }

            public static IEnumerable<_CommissionProjects> GetProjects(int month, string sale = "")
            {
                var year = DateTime.Now.Year;
                var deals = from d in CH.DB.Deals.Where(o => o.Abandoned == false && o.Income>0 &&
                    o.ActualPaymentDate.Value.Month == month && o.ActualPaymentDate.Value.Year == year && o.Sales == sale)
                            select d;
                //var username = sale;
                //var emps = CH.DB.EmployeeRoles.Where(w => w.AccountName == username);
                //var displayname =emps.Select(s => s.AccountNameCN).FirstOrDefault();
                //var roleid  =emps.Select(s => s.RoleID).FirstOrDefault();
                var projects = from p in deals
                               group p by new { p.Project.ProjectCode,p.ProjectID } into grp
                               select new _CommissionProjects 
                               { ProjectCode = grp.Key.ProjectCode,
                               ID=grp.Key.ProjectID};
                //List<proj> strList = new List<string>();
                //foreach (var p in projects)
                //{
                //    strList.Add(p.ProjectCode);
                //}
                //strList.Add("test");
                return projects;
            }
            public static _PreCommission GetIncome(int month, string sale,int projectid)
            {
                var year = DateTime.Now.Year;
                var deals = from d in CH.DB.Deals.Where(o =>o.ProjectID==projectid && o.Abandoned == false && o.Income > 0 &&
                    o.ActualPaymentDate.Value.Month == month && o.ActualPaymentDate.Value.Year == year && o.Sales == sale)
                            select d;
                var username = sale;
                var emps = CH.DB.EmployeeRoles.Where(w => w.AccountName == username);
                var displayname = emps.Select(s => s.AccountNameCN).FirstOrDefault();
                var roleid = emps.Select(s => s.RoleID).FirstOrDefault();
                var projects = from p in deals
                               group p by new { p.Project.ProjectCode } into grp
                               select new { projectcode = grp.Key.ProjectCode };
                var proname = "";
                foreach (var name in projects)
                {
                    proname = proname + name.projectcode + ",";
                }
                proname = proname.TrimEnd(',');
                string inout = "海外";
                if (roleid != null)
                {
                    var name = CH.GetDataById<Role>(roleid).Name;
                    if (name.Contains("国内"))
                        inout = "国内";

                }
                decimal standard = 3000;
                var lps = new _PreCommission()
                {
                    RoleLevel = 1,
                    ID = 0,
                    //ProjectNames = proname,
                    Income = deals.Sum(s => (decimal?)s.Income),
                    TargetNameEN = sale,
                    TargetNameCN = displayname,
                    InOut = inout,
                    DelegateLessIncome = deals.Where(w => w.Poll > 0 && w.Income / w.Poll < standard).Sum(s => (decimal?)s.Income),
                    DelegateMoreCount = deals.Where(w => w.Poll > 0 && w.Income / w.Poll > standard).Sum(s => (int?)s.Poll),
                    DelegateMoreIncome = deals.Where(w => w.Poll > 0 && w.Income / w.Poll >= standard).Sum(s => (decimal?)s.Income),
                    SponsorIncome = deals.Where(w => w.Poll == 0).Sum(s => (decimal?)s.Income)
                };
                return lps;
            }

            public static IEnumerable<_CommissionSales> GetSalesDDL(int month)
            {
                
                var mems = CH.DB.Members.Where(w => w.IsActivated == true && w.Project.IsActived == true);
                var ret = from p in mems
                            select new _CommissionSales
                            {
                                salesid = p.Name,
                                sales = p.Name
                            };
                return ret;
                

            }
        }
    }
}