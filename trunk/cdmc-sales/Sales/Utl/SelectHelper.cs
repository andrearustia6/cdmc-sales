using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utl;
using Entity;
using BLL;

namespace Utl
{
    public static class SelectHelper
    {
        public static IEnumerable<SelectListItem> CrmProgessSelectList()
        {
            var ps = CH.GetAllData<Progress>().OrderBy(o => o.Code);
            List<SelectListItem> selectList = new List<SelectListItem>();
            // selectList.Add(new SelectListItem() { Text = "不指定", Value = "-1" });
            foreach (var p in ps)
            {
                selectList.Add(new SelectListItem() { Text = p.Name, Value = p.Code.ToString() });
            }


            return selectList;
        }

        public static IEnumerable<SelectListItem> LeadConditionSelectList()
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem() { Text = "Lead都已Blowed", Value = "1" });
            selectList.Add(new SelectListItem() { Text = "含有Lead未打", Value = "3" });
            selectList.Add(new SelectListItem() { Text = "2天内/2天后有CallBack计划", Value = "5" });

            return selectList;
        }

        public static IEnumerable<SelectListItem> CategoresSelectList(int? projectid)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();

            var ps = CH.GetAllData<Category>(o => o.ProjectID == projectid);
            foreach (var p in ps)
            {
                selectList.Add(new SelectListItem() { Text = p.Name, Value = p.ID.ToString() });
            }


            return selectList;
        }

        public static IEnumerable<SelectListItem> DealConditionSelectList()
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem() { Text = "未出单的公司", Value = "0" });
            selectList.Add(new SelectListItem() { Text = "已出单", Value = "1" });
            selectList.Add(new SelectListItem() { Text = "已出单未付款", Value = "2" });
            selectList.Add(new SelectListItem() { Text = "已付款", Value = "3" });

            return selectList;
        }

        public static IEnumerable<SelectListItem> CallConditionSelectList()
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem() { Text = "1天内打过", Value = "1" });
            selectList.Add(new SelectListItem() { Text = "3天内打过", Value = "3" });
            selectList.Add(new SelectListItem() { Text = "1周内打过", Value = "7" });
            selectList.Add(new SelectListItem() { Text = "2周内打过", Value = "14" });

            return selectList;
        }

        public static IEnumerable<SelectListItem> DurationSelectList()
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            //selectList.Add(new SelectListItem() { Text = "不指定", Value = "0" });
            selectList.Add(new SelectListItem() { Text = "1天内", Value = "1" });
            selectList.Add(new SelectListItem() { Text = "3天内", Value = "3" });
            selectList.Add(new SelectListItem() { Text = "7天内", Value = "7" });
            selectList.Add(new SelectListItem() { Text = "14天内", Value = "14" });
            selectList.Add(new SelectListItem() { Text = "30天内", Value = "30" });

            return selectList;
        }

        public static IEnumerable<SelectListItem> FilterSelectList()
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem() { Text = "请选择", Value = "" });
            //selectList.Add(new SelectListItem() { Text = "所有出单", Value = "0" });
            //selectList.Add(new SelectListItem() { Text = "24小时内出单", Value = "1" });
            //selectList.Add(new SelectListItem() { Text = "一周内出单", Value = "7" });
            //selectList.Add(new SelectListItem() { Text = "两周内出单", Value = "14" });
            //selectList.Add(new SelectListItem() { Text = "一月内出单", Value = "30" });
            if (Employee.CurrentRole.Level != 4)
            {
                selectList.Add(new SelectListItem() { Text = "未确认出单", Value = "2" });
            }
            selectList.Add(new SelectListItem() { Text = "未付款出单", Value = "3" });
            selectList.Add(new SelectListItem() { Text = "已付款出单", Value = "4" });

            return selectList;
        }

        public static IEnumerable<SelectListItem> TrueOrFalseSelectList()
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem() { Text = "True", Value = "True" });
            selectList.Add(new SelectListItem() { Text = "False", Value = "False" });
            return selectList;
        }

        public static IEnumerable<SelectListItem> MemberSelectListForProject(int projectID, string selectVal = "")
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            SelectListItem selectListItemNone = new SelectListItem() { Text = "所有未分配", Value = "-1" };
            SelectListItem selectListItemAll = new SelectListItem() { Text = "所有已分配", Value = "-2" };
            if (selectVal == "-1")
            {
                selectListItemNone.Selected = true;
            }

            if (selectVal == "-2")
            {
                selectListItemAll.Selected = true;
            }

            selectList.Add(selectListItemNone);
            selectList.Add(selectListItemAll);

            foreach (Member m in CH.GetAllData<Member>(c => c.ProjectID == projectID && c.IsActivated == true))
            {
                SelectListItem selectListItem = new SelectListItem { Text = m.Name, Value = m.ID.ToString() };
                if (m.ID.ToString() == selectVal)
                {
                    selectListItem.Selected = true;
                }
                selectList.Add(selectListItem);
            }
            return selectList;
        }

        public static IEnumerable<SelectListItem> SaleRelatedProjects(int? selectVal = null)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            foreach (Project p in CRM_Logical.GetUserInvolveProject())
            {
                SelectListItem selectListItem = new SelectListItem { Text = p.ProjectCode, Value = p.ID.ToString() };
                if (selectVal.HasValue && p.ID == selectVal.Value)
                {
                    selectListItem.Selected = true;
                }
                selectList.Add(selectListItem);
            }
            return selectList;
        }

        public static IEnumerable<SelectListItem> SaleCallListFilter(int selectVal = 0)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem { Text = "显示所有通话记录", Value = "0", Selected = (selectVal == 0) });
            selectList.Add(new SelectListItem { Text = "显示每个客户第一次联系，并且为Pitched记录(Faxout),选中此项，致电状态筛选将无效", Value = "1", Selected = (selectVal == 1) });
            selectList.Add(new SelectListItem { Text = "显示每个客户最后一次通话状态(Lead最后通话状态)", Value = "2", Selected = (selectVal == 2) });

            return selectList;
        }

        public static IEnumerable<SelectListItem> LeadCallTypeSelect(int? selectVal)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            foreach (LeadCallType leadCallType in CH.GetAllData<LeadCallType>())
            {
                SelectListItem selectListItem = new SelectListItem() { Text = leadCallType.Name, Value = leadCallType.ID.ToString() };
                if (selectVal.HasValue && leadCallType.ID == selectVal.Value)
                {
                    selectListItem.Selected = true;
                }
                selectList.Add(selectListItem);
            }
            return selectList;
        }

        public static IEnumerable<SelectListItem> CompanyIndustrySelectList(int? selectVal)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            foreach (Area area in CH.GetAllData<Area>())
            {
                SelectListItem selectListItem = new SelectListItem() { Text = area.Name, Value = area.ID.ToString() };
                if (selectVal.HasValue && area.ID == selectVal.Value)
                {
                    selectListItem.Selected = true;
                }
                selectList.Add(selectListItem);
            }
            return selectList;
        }

        public static IEnumerable<SelectListItem> CompanyTypeSelectList(int? selectVal)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            foreach (CompanyType companyType in CH.GetAllData<CompanyType>())
            {
                SelectListItem selectListItem = new SelectListItem() { Text = companyType.Name, Value = companyType.ID.ToString() };
                if (selectVal.HasValue && companyType.ID == selectVal.Value)
                {
                    selectListItem.Selected = true;
                }
                selectList.Add(selectListItem);
            }
            return selectList;
        }

        public static IEnumerable<SelectListItem> DistinctNumberSelectList(int? selectVal)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            foreach (DistrictNumber districtNumber in CH.GetAllData<DistrictNumber>())
            {
                SelectListItem selectListItem = new SelectListItem() { Text = districtNumber.Name, Value = districtNumber.ID.ToString() };
                if (selectVal.HasValue && districtNumber.ID == selectVal.Value)
                {
                    selectListItem.Selected = true;
                }
                selectList.Add(selectListItem);
            }
            return selectList;
        }

        public static IEnumerable<SelectListItem> CoreLVLSelectList(int? selectVal)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            foreach (CoreLVL coreLVL in CH.GetAllData<CoreLVL>())
            {
                SelectListItem selectListItem = new SelectListItem() { Text = coreLVL.CoreLVLName, Value = coreLVL.ID.ToString() };
                if (selectVal.HasValue && coreLVL.ID == selectVal.Value)
                {
                    selectListItem.Selected = true;
                }
                selectList.Add(selectListItem);
            }
            return selectList;
        }
        public static IEnumerable<SelectListItem> ProgressSelectList(int? selectVal)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            foreach (Progress progress in CH.GetAllData<Progress>())
            {
                SelectListItem selectListItem = new SelectListItem() { Text = progress.Name, Value = progress.ID.ToString() };
                if (selectVal.HasValue && progress.ID == selectVal.Value)
                {
                    selectListItem.Selected = true;
                }
                selectList.Add(selectListItem);
            }
            return selectList;
        }
        public static IEnumerable<SelectListItem> GenderSelectList(string selectVal)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();

            SelectListItem selectListItem = new SelectListItem() { Text = "Mr", Value = "Mr" };
            if (selectVal == "Mr")
            {
                selectListItem.Selected = true;
            }
            selectList.Add(selectListItem);

            selectListItem = new SelectListItem() { Text = "Ms", Value = "Ms" };
            if (selectVal == "Ms")
            {
                selectListItem.Selected = true;
            }
            selectList.Add(selectListItem);

            return selectList;
        }

        public static IEnumerable<SelectListItem> CallTypeSelectList(int? selectVal)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            foreach (LeadCallType leadCallType in CH.GetAllData<LeadCallType>())
            {
                SelectListItem selectListItem = new SelectListItem() { Text = leadCallType.Name, Value = leadCallType.ID.ToString() };
                if (selectVal.HasValue && leadCallType.ID == selectVal.Value)
                {
                    selectListItem.Selected = true;
                }
                selectList.Add(selectListItem);
            }
            return selectList;
        }

        public static IEnumerable<SelectListItem> ProjectSelectList(string currentUserName, int? selectVal=null)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();

            var user = Employee.CurrentUserName;
            var projects = CRM_Logical.GetUserInvolveProject();

            foreach (Project project in projects)
            {
                SelectListItem selectListItem = new SelectListItem() { Text = project.ProjectCode, Value = project.ID.ToString() };
                if (selectVal.HasValue && project.ID == selectVal.Value)
                {
                    selectListItem.Selected = true;
                }
                selectList.Add(selectListItem);
            }
            return selectList.OrderBy(s => s.Text);
        }

        public static IEnumerable<SelectListItem> CompanyScaleSelectList(string selectVal)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            string[] companyScales = new string[] { "1-50人", "51-100人", "101-500人", "501-1000人", "1001-5000人", "5001-10000人", "10000+人" };
            foreach (string companyScale in companyScales)
            {
                SelectListItem selectListItem = new SelectListItem() { Text = companyScale, Value = companyScale, Selected = companyScale == selectVal };
                selectList.Add(selectListItem);
            }
            return selectList;
        }

        public static IEnumerable<SelectListItem> AnnualSaleSelectList(string selectVal)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            string[] annualSales = new string[] { "人民币1000万以下", "人民币1000 - 5000万", "人民币5000万 - 1亿", "人民币1- 10亿", "人民币10 - 100亿", "人民币100-1000亿", "人民币1000亿以上" };
            foreach (string annualSale in annualSales)
            {
                SelectListItem selectListItem = new SelectListItem() { Text = annualSale, Value = annualSale, Selected = annualSale == selectVal };
                selectList.Add(selectListItem);
            }
            return selectList;
        }

        public static IEnumerable<SelectListItem> TargetOfMonthSelectList()
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem() { Text = "请选择", Value = "" });
            selectList.Add(new SelectListItem() { Text = "财务已确认", Value = "1" });
            selectList.Add(new SelectListItem() { Text = "版块已确认", Value = "2" });
            selectList.Add(new SelectListItem() { Text = "均未确认", Value = "3" });
            selectList.Add(new SelectListItem() { Text = "本月目标", Value = "4" });
            selectList.Add(new SelectListItem() { Text = "上月目标", Value = "5" });
            return selectList;
        }

        public static IEnumerable<SelectListItem> PrefixFilterSelectList(string selectVal)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            string[] prefixLetters = new string[] { "请选择","A", "B", "C", "D", "E", "F", "G", 
                                                  "H", "I", "J", "K", "L", "M", "N",
                                                  "O", "P", "Q", "R", "S", "T", "U", 
                                                  "V", "W", "X", "Y", "Z" };
            foreach (string prefixLetter in prefixLetters)
            {
                SelectListItem selectListItem = new SelectListItem() { Text = prefixLetter, Value = (prefixLetter == "请选择" ? "" : prefixLetter), Selected = prefixLetter == selectVal };
                selectList.Add(selectListItem);
            }
            return selectList;
        }

        public static bool IsTelephone(string val)
        {
            if (string.IsNullOrEmpty(val)) return false;
            for (int i = 0; i < val.Length; i++)
            {
                string tempchar = "";
                //tempchar = val.Substring(i, i + 1);
                tempchar = val.Substring(i, 1);
                if (!(tempchar == "0" || tempchar == "1" || tempchar == "2" || tempchar == "3" || tempchar == "4" || tempchar == "5" || tempchar == "6" || tempchar == "7" || tempchar == "8" || tempchar == "9" || tempchar == "-" || tempchar == " "))
                {
                    return false;
                }
            }
            return true;
        }

        public static IEnumerable<SelectListItem> SuperManagerProjectSelectList(int? selectVal)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            var projects = CH.GetAllData<Project>().FindAll(p => p.IsActived == true);
            foreach (Project project in projects)
            {
                SelectListItem selectListItem = new SelectListItem() { Text = project.ProjectCode, Value = project.ID.ToString() };
                if (selectVal.HasValue && project.ID == selectVal.Value)
                {
                    selectListItem.Selected = true;
                }
                selectList.Add(selectListItem);
            }
            return selectList;
        }

        public static IEnumerable<SelectListItem> DealPaymentSelectList()
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem() { Text = "请选择", Value = "" });
            selectList.Add(new SelectListItem() { Text = "0~3000", Value = "1" });
            selectList.Add(new SelectListItem() { Text = "3000~5000", Value = "2" });
            selectList.Add(new SelectListItem() { Text = "5000~8000", Value = "3" });
            selectList.Add(new SelectListItem() { Text = "8000~10000", Value = "4" });
            selectList.Add(new SelectListItem() { Text = "10000~15000", Value = "5" });
            selectList.Add(new SelectListItem() { Text = "15000以上", Value = "6" });
            return selectList;
        }

        public static IEnumerable<SelectListItem> DealParticipantSelectList()
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem() { Text = "请选择", Value = "" });
            selectList.Add(new SelectListItem() { Text = "无", Value = "0" });
            selectList.Add(new SelectListItem() { Text = "有", Value = "1" });

            return selectList;
        }

        public static IEnumerable<SelectListItem> MemberSelectListInProject(int projectID, string selectVal = "")
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            SelectListItem selectListItemNone = new SelectListItem() { Text = "请选择", Value = "" };
            if (selectVal == "")
            {
                selectListItemNone.Selected = true;
            }
            selectList.Add(selectListItemNone);

            foreach (Member m in CH.GetAllData<Member>(c => c.ProjectID == projectID && c.IsActivated == true))
            {
                SelectListItem selectListItem = new SelectListItem { Text = m.Name, Value = m.ID.ToString() };
                if (m.ID.ToString() == selectVal)
                {
                    selectListItem.Selected = true;
                }
                selectList.Add(selectListItem);
            }
            return selectList;
        }

        public static List<string> MemberSelectListInOwnProject()
        {
            if (Employee.CurrentRole.Level == Role.LVL_Sales)
            {
                return new List<string>() { Employee.CurrentUserName };
            }
            else
            {
                var selMember = (from p in CRM_Logical.GetUserInvolveProject()
                                 from m in CH.DB.Members
                                 where p.IsActived && p.ID == m.ProjectID
                                 select m).OrderBy(s => s.Name);
                return selMember.Select(s => s.Name).Distinct().ToList();
            }
        }

        public static IEnumerable<SelectListItem> TraineeSelectListInEmployee()
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem() { Text = "-请选择-", Value = "" });
            selectList.Add(new SelectListItem() { Text = "是", Value = "True" });
            selectList.Add(new SelectListItem() { Text = "否", Value = "False" });
            return selectList;
        }

        public static IEnumerable<SelectListItem> DealConditionSelectListInMarket(int? selectVal)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            SelectListItem selectListItemNull = new SelectListItem() { Text = "-请选择-", Value = "" };
            if (selectVal == null)
            {
                selectListItemNull.Selected = true;
            }
            selectList.Add(selectListItemNull);
            SelectListItem selectListItemHas = new SelectListItem() { Text = "未出单", Value = "0" };
            if (selectVal == 0)
            {
                selectListItemHas.Selected = true;
            }
            selectList.Add(selectListItemHas);
            SelectListItem selectListItemNone = new SelectListItem() { Text = "已出单", Value = "1" };
            if (selectVal == 1)
            {
                selectListItemNone.Selected = true;
            }
            selectList.Add(selectListItemNone);
            return selectList;
        }

        public static IEnumerable<SelectListItem> DistinctNumberFilterSelectList(int? selectVal)
        {
            var lists =  CH.GetAllData<DistrictNumber>();
            int max = lists.Max(d => d.ID);
            List<SelectListItem> selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem() { Text = "- 请选择 -", Value = "0" });
            selectList.Add(new SelectListItem() { Text = "中国(默认)", Value = "" });
            selectList.Add(new SelectListItem() { Text = "国外所有", Value = (max + 1).ToString() });
            foreach (DistrictNumber districtNumber in lists )
            {
                SelectListItem selectListItem = new SelectListItem() { Text = districtNumber.Name, Value = districtNumber.ID.ToString() };
                if (selectVal.HasValue && districtNumber.ID == selectVal.Value)
                {
                    selectListItem.Selected = true;
                }
                selectList.Add(selectListItem);
            }
            return selectList;
        }


        public static IEnumerable<SelectListItem> CompanyLeadsSelectList(int companyid, int? selectVal = null)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            //var query = from c in CH.DB.CompanyRelationships.Where(w => w.Project.ID == projectid && w.Company.ID == companyid) select c.Company.Leads;
            var leads = CH.GetAllData<Lead>().Where(l => l.CompanyID == companyid);
            foreach (Lead lead in leads)
            {
                SelectListItem selectListItem = new SelectListItem { Text = lead.Name, Value =lead.ID.ToString() };
                if (selectVal.HasValue && lead.ID == selectVal.Value)
                {
                    selectListItem.Selected = true;
                }
                selectList.Add(selectListItem);
            }
            return selectList;
        }

        public static List<SelectListItem> LeadSelectListByCRMID(int? crmid, int? selectVal = null)
        {
            int? companyid = CH.GetDataById<CompanyRelationship>(crmid).CompanyID;
            //var ls = from l in CH.DB.Leads where l.CompanyID == companyid select new { Text = l.Name_CH + " | " + l.Name_EN, Value = l.ID };
            List<SelectListItem> selectList = new List<SelectListItem>();
            var leads = CH.GetAllData<Lead>().Where(l => l.CompanyID == companyid);
            foreach (Lead lead in leads)
            {
                SelectListItem selectListItem = new SelectListItem { Text = lead.Name, Value = lead.ID.ToString() };
                if (selectVal.HasValue && lead.ID == selectVal.Value)
                {
                    selectListItem.Selected = true;
                }
                selectList.Add(selectListItem);
            }
            return selectList;
        }
        public static IEnumerable<SelectListItem> ProjectTypeSelectList()
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            //var query = from c in CH.DB.CompanyRelationships.Where(w => w.Project.ID == projectid && w.Company.ID == companyid) select c.Company.Leads;
            var types = CH.GetAllData<ProjectType>();
            foreach (ProjectType type in types)
            {
                SelectListItem selectListItem = new SelectListItem { Text = type.Name, Value = type.ID.ToString() };
                selectList.Add(selectListItem);
            }
            return selectList;
        }

        public static IEnumerable<SelectListItem> YearSelectList()
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem() { Text = "2012", Value = "2012" });
            selectList.Add(new SelectListItem() { Text = "2013", Value = "2013" });
            selectList.Add(new SelectListItem() { Text = "2014", Value = "2014" });
            selectList.Add(new SelectListItem() { Text = "2015", Value = "2015" });
            selectList.Add(new SelectListItem() { Text = "2016", Value = "2016" });
            return selectList;
        }
        public static IEnumerable<SelectListItem> MonthSelectList()
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            for (int i = 1; i <= 12; i++)
            {
                selectList.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
            }
            
            return selectList;
        }

        public static IEnumerable<SelectListItem> DaySelectList()
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            for (int i = 1; i <=31; i++)
            {
                selectList.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
            }
            return selectList;
        }

        public static IEnumerable<SelectListItem> ProjectStateSelectList()
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem() { Text = "老项目", Value = "老项目" });
            selectList.Add(new SelectListItem() { Text = "二届项目", Value = "二届项目" });
            selectList.Add(new SelectListItem() { Text = "次新项目", Value = "次新项目" });
            selectList.Add(new SelectListItem() { Text = "全新项目", Value = "全新项目" });
            return selectList;
        }
        public static IEnumerable<SelectListItem> InfoSelectList()
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem() { Text = "Info", Value = "Info" });
            selectList.Add(new SelectListItem() { Text = "会展", Value = "会展" });
            return selectList;
        }
        public static IEnumerable<SelectListItem> DeletedSelectList()
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem() { Text = "删除", Value = "1" });
            selectList.Add(new SelectListItem() { Text = "未删除", Value = "0" });
            return selectList;
        }
    }
}