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
                            TotalCommission=pre.TotalCommission,
                            DelegateIncome=pre.DelegateIncome,
                            DelegateRate=pre.DelegateRate,
                            DelegateCommission=pre.DelegateCommission
                        };
                return ps;
            }
            public static IEnumerable<_FinalCommission> GetFinalCommission(int? projectid,string sale)
            {
                var lps = from f in CH.DB.FinalCommissions select f;
                if (projectid != null)
                    lps = lps.Where(l => l.ProjectID == projectid);
                if(!string.IsNullOrWhiteSpace(sale))
                    lps = lps.Where(l => l.TargetNameEN == sale);
                var ps = from pre in lps
                         join p in CH.DB.Projects on pre.ProjectID equals p.ID
                         select new _FinalCommission
                         {
                             CommID = pre.CommID,
                             ID = pre.ID,
                             ProjectID = pre.ProjectID,
                             ProjectName = p.ProjectCode,
                             Income = pre.Income,
                             TargetNameEN = pre.TargetNameEN,
                             TargetNameCN = pre.TargetNameCN,
                             InOut = pre.InOut,
                             DelegateLessIncome = pre.DelegateLessIncome,
                             DelegateMoreCount = pre.DelegateMoreCount,
                             DelegateMoreIncome = pre.DelegateMoreIncome,
                             SponsorIncome = pre.SponsorIncome == null ? 0 : pre.SponsorIncome.Value,
                             Commission = pre.Commission,
                             CommissionA=pre.Commission,
                             CommissionRate = pre.CommissionRate,
                             Tax = pre.Tax,
                             Bonus = pre.Bonus,
                             ReturnIncome = pre.ReturnIncome,
                             ReturnReason = pre.ReturnReason,
                             ActualCommission = pre.ActualCommission,
                             DelegateLessRate = pre.DelegateLessRate,
                             DelegateLessCommission = pre.DelegateLessCommission,
                             DelegateLessPayed=pre.DelegateLessPayed,
                             DelegateMoreRate = pre.DelegateMoreRate,
                             DelegateMoreCommission = pre.DelegateMoreCommission,
                             DelegateMorePayed=pre.DelegateMorePayed,
                             SponsorCommission = pre.SponsorCommission,
                             SponsorPayed=pre.SponsorPayed,
                             SponsorRate = pre.SponsorRate,
                             TotalCommission = pre.TotalCommission,
                             CommissionPayed=pre.CommissionPayed,
                             ManageCommission=pre.ManageCommission,
                             DelegateIncome=pre.DelegateIncome,
                             DelegateRate=pre.DelegateRate,
                             DelegateCommission=pre.DelegateCommission,
                             DelegatePayed=pre.DelegatePayed

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
                    DelegateLessIncome = inout == "海外" ? 0 :deals.Where(w => w.Poll > 0 && w.Income / w.Poll < standard).Sum(s => (decimal?)s.Income),
                    DelegateMoreCount = inout ==  "海外" ? 0 : deals.Where(w => w.Poll > 0 && w.Income / w.Poll > standard).Sum(s => (int?)s.Poll),
                    DelegateMoreIncome = inout == "海外" ? 0 : deals.Where(w => w.Poll > 0 && w.Income / w.Poll >= standard).Sum(s => (decimal?)s.Income),
                    SponsorIncome = deals.Where(w => w.Poll == 0).Sum(s => (decimal?)s.Income),
                    DelegateIncome = inout == "海外" ? deals.Where(w => w.Poll > 0).Sum(s => (decimal?)s.Income) : 0
                };
                return lps;
            }

            public static IEnumerable<_CommissionSales> GetSalesDDL(int month)
            {

                var startdate = new DateTime(DateTime.Now.Year,month,1);
                var enddate = startdate.EndOfMonth();
                var enddateacutal = enddate.AddDays(1);
                var deals = CH.DB.Deals.Where(w => w.Abandoned == false && w.Income > 0
                    && w.ActualPaymentDate >= startdate && w.ActualPaymentDate < enddateacutal);
                var mems = from m in deals
                           select new
                           {
                               proid = m.ProjectID,
                               sales = m.Sales
                           };
                var alreadypaymen =  from m in CH.DB.PreCommissions.Where(w=>w.StartDate==startdate && w.EndDate==enddate) select m;
                         
                var targets  = mems.Where(w=>alreadypaymen.Any(a=>a.ProjectID ==w.proid && a.TargetNameEN==w.sales)==false).Select(s=>s.sales).Distinct();
               // var mems = CH.DB.Members.Where(w => w.IsActivated == true && w.Project.IsActived == true).Select(w=>w.Name).Distinct();
                var ret = from p in targets

                            select new _CommissionSales
                            {
                                salesid = p,
                                sales = p
                            };
                return ret;
            }
            public static IEnumerable<_CommissionSales> GetSalesByProDDL(int projectid)
            {
                List<SelectListItem> selectList = new List<SelectListItem>();
                var mems = CH.DB.Members.Where(w => w.IsActivated == true && w.Project.IsActived == true && w.ProjectID==projectid).Select(w => w.Name).Distinct();
               
                var alreadypaymen = from m in CH.DB.FinalCommissions.Where(w => w.ProjectID == projectid) select m;
                var ret = from p in mems.Where(w=>alreadypaymen.Any(a=>a.TargetNameEN==w)==false)
                          select new _CommissionSales
                          {
                              salesid = p,
                              sales = p
                          };
                return ret;
            }
            public static IEnumerable<SelectListItem> GetProjectsDDL()
            {
                List<SelectListItem> selectList = new List<SelectListItem>();
                var projects = CH.DB.Projects.Where(w => w.IsActived == true && w.Members.Where(m=>m.Name==Employee.CurrentUserName).Any()==true );
                var ret = from p in projects
                          select new _CommissionProjects
                          {
                              ID = p.ID,
                              ProjectCode = p.ProjectCode
                          };
                foreach (var r in ret)
                {
                    SelectListItem selectListItem = new SelectListItem() { Text = r.ProjectCode, Value = r.ID.ToString() };
                    selectList.Add(selectListItem);
                }
                return selectList;
            }

            public static _FinalCommission GetPreCommByProSales(int projectid, string sale)
            {
                var year = DateTime.Now.Year;
                var deals = from d in CH.DB.Deals.Where(o => o.ProjectID == projectid && o.Abandoned == false && o.Income > 0 && o.Sales == sale)
                            select d;
                var precommissions = from pre in CH.DB.PreCommissions.Where(p=>p.ProjectID==projectid && p.TargetNameEN==sale)
                                         select pre;
                var username = sale;
                var emps = CH.DB.EmployeeRoles.Where(w => w.AccountName == username);
                var displayname = emps.Select(s => s.AccountNameCN).FirstOrDefault();
                var roleid = emps.Select(s => s.RoleID).FirstOrDefault();
                string inout = "海外";
                if (roleid != null)
                {
                    var name = CH.GetDataById<Role>(roleid).Name;
                    if (name.Contains("国内"))
                        inout = "国内";

                }
                decimal standard = 3000;
                var lps = new _FinalCommission()
                {
                    RoleLevel = 1,
                    ID = 0,
                    Income = deals.Sum(s => (decimal?)s.Income),
                    TargetNameEN = sale,
                    TargetNameCN = displayname,
                    InOut = inout,
                    DelegateLessIncome = inout == "海外" ? 0 : deals.Where(w => w.Poll > 0 && w.Income / w.Poll < standard).Sum(s => (decimal?)s.Income),
                    DelegateLessPayed=precommissions.Sum(w=>(decimal?)w.DelegateLessCommission),
                    DelegateMoreCount = inout == "海外" ? 0 : deals.Where(w => w.Poll > 0 && w.Income / w.Poll > standard).Sum(s => (int?)s.Poll),
                    DelegateMoreIncome = inout == "海外" ? 0 : deals.Where(w => w.Poll > 0 && w.Income / w.Poll >= standard).Sum(s => (decimal?)s.Income),
                    DelegateMorePayed = precommissions.Sum(w => (decimal?)w.DelegateMoreCommission),
                    SponsorIncome = deals.Where(w => w.Poll == 0).Sum(s => (decimal?)s.Income),
                    SponsorPayed = precommissions.Sum(w => (decimal?)w.SponsorCommission),
                    DelegateIncome = inout == "海外" ? deals.Where(w => w.Poll > 0).Sum(s => (int?)s.Income) : 0,
                    DelegatePayed = precommissions.Sum(w => (decimal?)w.DelegateCommission)
                };
                return lps;
            }
        }
    }
}