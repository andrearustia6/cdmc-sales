using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entity;

namespace BLL
{
    public class CRM_Logical
    {
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