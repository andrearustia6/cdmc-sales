using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utl;
using Entity;

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
    }
}