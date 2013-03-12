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
                SelectListItem selectListItem = new SelectListItem { Text = p.Name, Value = p.ID.ToString() };
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
            selectList.Add(new SelectListItem { Text = "All Call", Value = "0", Selected = (selectVal == 0) });
            selectList.Add(new SelectListItem { Text = "Fax out", Value = "1", Selected = (selectVal == 1) });
            selectList.Add(new SelectListItem { Text = "最后状态", Value = "2", Selected = (selectVal == 2) });

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
    }
}