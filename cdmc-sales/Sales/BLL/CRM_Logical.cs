using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entity;
using Utl;
using System.Web.Security;

namespace BLL
{
    public class CRM_Logical
    {
        public static TargetOfWeek GetTargetOfWeek(int projectid,int targetofmonth,string member,DateTime startdate,DateTime enddate)
        {
            var data = CH.GetAllData<TargetOfWeek>(tw => tw.ProjectID == projectid && tw.TargetOfMonthID == targetofmonth&&tw.Member==member);
            var result = data.FirstOrDefault(t => t.StartDate == startdate && t.EndDate == enddate);
            return result;
        }

        public static decimal GetTargetOfWeekValue(int projectid, int targetofmonth, string member, DateTime startdate, DateTime enddate)
        {
            var data = GetTargetOfWeek(projectid, targetofmonth, member, startdate, enddate);
            if (data != null) return data.Deal;
            return 0;
        }

        public static List<string> GetDefaultCharatracter()
        {
            return new List<string>() { "A", "B", "C", "D", "E", "F", "G", "H", "I", "Z", "K", "L", "M", "N", 
                "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        }
       
        public static bool IsCompanySelectedForProject(Company c, int projectid)
        {
            var p = CH.GetAllData<Project>(i=>i.ID == projectid,"Companys").FirstOrDefault();

            if (p != null && p.Companys.FirstOrDefault(child => child.ID == c.ID) != null)
            {
                return true;
            }

            return false;
        }

        public static bool IsSameMemberExistInProject(string name, int? projectid)
        {
            var p = CH.GetAllData<Project>(i => i.ID == projectid, "Members").FirstOrDefault();
            if (p != null && p.Members.FirstOrDefault(c => c.Name == name)!=null)
                return true;
            else
                return false;
        }

        public static bool IsMemberExist(string name)
        {
            var user = Membership.GetUser(name);
            return user == null ? false : true;
        }

         public static bool IsFullPitched(LeadCall call)
        {
            return call.LeadCallType.Name=="Full Pitched"?true:false;
        }
         public static bool IsPitched(LeadCall call)
        {
            return call.LeadCallType.Name == "Pitched" ? true : false;
        }
        
        
        public static Target_Package GetTarget_Package(Lead lead,object projectid)
        {
            if (projectid == null)
                return new Target_Package() { LeadID = lead.ID };

            var pid = (int)projectid;

            if (lead.Target_Packages != null)
            {
                var tp = lead.Target_Packages.FirstOrDefault(i => i.ProjectID == pid && i.LeadID == lead.ID);
                if (tp == null) tp = new Target_Package() { LeadID = lead.ID, ProjectID = pid };
                return tp;
            }
            return new  Target_Package() { LeadID = lead.ID, ProjectID = pid };
        }
        public static bool IsDMS(string leadCallType)
        {
            if (leadCallType == "Others" || leadCallType == "Blowed" || leadCallType == "Not Pitched")
                return false;
            else
                return true;
        }

        public static bool IsNewDMS(LeadCall call)
        {
            return call.IsFirstPitch;
        }

        public static bool IsDMS(LeadCall call)
        {
            var type = call.LeadCallType.Code;
            if (type > 3)
                return true;
            else
                return false;
        }

        public static bool IsDealClosed(LeadCall call)
        {
            return call.LeadCallType.Name == "Closed" ? true : false;
        }

        public static bool IsQualifiedDecision(LeadCall call)
        {
            return call.LeadCallType.Name == "Qualified Decision" ? true : false;
        }

        
    }
}