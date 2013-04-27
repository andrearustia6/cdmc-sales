﻿using System;
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
            var ps = CH.GetAllData<Progress>();
            List<SelectListItem> selectList = new List<SelectListItem>();
            foreach(var p in ps)
            {
                selectList.Add(new SelectListItem() { Text = p.Name, Value = p.Code.ToString() });
            }
          

            return selectList;
        }

     

        public static IEnumerable<SelectListItem> DurationSelectList()
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem() { Text = "所有", Value = "0" });
            selectList.Add(new SelectListItem() { Text = "1天内", Value = "1" });
            selectList.Add(new SelectListItem() { Text = "3天内", Value = "3" });
            selectList.Add(new SelectListItem() { Text = "7天内", Value = "7" });
            selectList.Add(new SelectListItem() { Text = "30天内", Value = "30" });
            
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

            foreach (Member m in CH.GetAllData<Member>(c => c.ProjectID == projectID))
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
    }
}