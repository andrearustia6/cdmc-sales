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
                        select new _PreCommission
                        {
                            CommID=pre.CommID,
                            StartDate=pre.StartDate,
                            EndDate=pre.EndDate,

                            ID = pre.ID,
                            ProjectNames = pre.ProjectNames,
                            Income = pre.Income,
                            TargetNameEN = pre.TargetNameEN,
                            TargetNameCN = pre.TargetNameCN,
                            InOut = pre.InOut,
                            //DelegateLessCount = pre.DelegateLessCount,
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
                            ActualCommission=pre.ActualCommission
                        };
                return ps;
            }

            public static _PreCommission GetIncome(int month, string sale = "")
            {
                var year = DateTime.Now.Year;
                var deals = from d in CH.DB.Deals.Where(o => o.Abandoned == false && o.Income>0 &&
                    o.ActualPaymentDate.Value.Month == month && o.ActualPaymentDate.Value.Year == year && o.Sales == sale)
                            select d;
                var username = Employee.CurrentUserName;
                var emps = CH.DB.EmployeeRoles.Where(w => w.AccountName == username);
                var displayname =emps.Select(s => s.AccountNameCN).FirstOrDefault();
                var roleid  =emps.Select(s => s.RoleID).FirstOrDefault();
                var projects = from p in deals
                               group p by new { p.Project.ProjectCode } into grp
                               select new { projectcode = grp.Key.ProjectCode };
                string inout = "海外";
                if (roleid != null)
                {
                    var name =  CH.GetDataById<Role>(roleid).Name;
                    if (name.Contains("国内"))
                        inout = "国内";

                }
                decimal standard = 3000;
                var lps =  new _PreCommission()
                            {
                                RoleLevel = 1,
                                ID = 0,
                                ProjectNames = string.Join(",", projects),
                                Income = deals.Sum(s=>(decimal?)s.Income),
                                TargetNameEN = sale,
                                TargetNameCN = displayname,
                                InOut = inout,
                                DelegateLessIncome = deals.Where(w => w.Poll > 0 && w.Income / w.Poll < standard).Sum(s => (decimal?)s.Income),
                                DelegateMoreCount = deals.Where(w => w.Poll > 0 && w.Income / w.Poll > standard).Sum(s => s.Poll),
                                DelegateMoreIncome = deals.Where(w => w.Poll > 0 && w.Income / w.Poll >= standard).Sum(s => (decimal?)s.Income),
                    
                                SponsorIncome = deals.Where(w => w.Poll == 0).Sum(s => (decimal?)s.Income)
                            };
                return lps;
            }


            public static IEnumerable<SelectListItem> GetSalesDDL(int month)
            {
                var rolelvl = Employee.CurrentRole.Level;
                rolelvl = PoliticsInterfaceRequired.LVL;
                if (rolelvl == PoliticsInterfaceRequired.LVL)
                {
                    rolelvl = DirectorRequired.LVL;
                }
                if (rolelvl >= SalesRequired.LVL)
                {
                    List<SelectListItem> selectList = new List<SelectListItem>();
                    IQueryable<string> sales = null;
                    var user = Employee.CurrentUserName;
                    var mems = CH.DB.Members.Where(w => w.IsActivated == true && w.Project.IsActived == true);
                    if (Employee.EqualToLeader() || Employee.EqualToManager())//版块或者lead查看
                    {
                        sales = mems.Where(w => w.Project.Manager == user || w.Project.TeamLeader == user).Select(s => s.Name).Distinct();

                    }
                    else if (Employee.EqualToSales())//销售查看
                    {
                        sales = mems.Where(w => w.Name == user).Select(s => s.Name).Distinct();
                    }
                    else if (rolelvl >= SuperManagerRequired.LVL)
                    {
                        sales = mems.Select(s => s.Name).Distinct();
                    }

                    if (sales != null)
                    {
                        if (Utl.Utl.DebugModel() != true)
                        {
                            var debugmembers = CH.DB.Members.Where(w => w.Test == true && w.IsActivated == true).Select(s => s.Name).Distinct();
                            sales = sales.Where(w => !debugmembers.Any(a => a == w));
                        }

                        foreach (var sale in sales)
                        {
                            SelectListItem selectListItem = new SelectListItem() { Text = sale, Value = sale };
                            selectList.Add(selectListItem);
                        }

                    }
                    return selectList;
                }

                return new List<SelectListItem>();

            }
        }
    }
}