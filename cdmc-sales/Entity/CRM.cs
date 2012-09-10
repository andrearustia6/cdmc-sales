using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Attributes;

namespace Entity
{
    /// <summary>
    /// 
    /// </summary>
    public class CRM : EntityBase
    {
        public virtual Lead Lead { get; set; }
        [Display(Name = "客户"), Required]
        public int? LeadID { get; set; }

        public List<LeadCall> LeadCallSheet { get; set; }

        public List<Deal> Deals { get; set; }

        public List<Conference> Conferences { get; set; }
    }

    /// <summary>
    /// 电话结果
    /// </summary>
    public class LeadCallType : EntityBase
    {
        [Display(Name = "电话结果"), Required]
        public string Name { get; set; }

        [Display(Name = "预备字段")]
        public int Code { get; set; }

        [Display(Name = "致电结果描述")]
        public string ResultDescription { get; set; }
    }


    /// <summary>
    /// 电话结果管理
    /// </summary>
    [JsonIgnoreAttribute("ModifiedTime", "Lead", "Project")]
    public class LeadCall : EntityBase
    {
        [Display(Name = "First Pitch"), Required]
        public bool IsFirstPitch { get; set; }


        public virtual Lead Lead { get; set; }
        [Display(Name = "致电客户"), Required]
        public int? LeadID { get; set; }


        [Display(Name = "所属项目")]
        public int? ProjectID { get; set; }
        public virtual Project Project { get; set; }

        public virtual LeadCallType LeadCallType { get; set; }
        public int? LeadCallTypeID { get; set; }

        [Display(Name = "是否有效")]
        public bool FaxOut
        {
            get
            {
                return true;
            }
        }

        [Display(Name = "结果描述")]
        public string Result { get; set; }

        [Display(Name = "回电时间")]
        public DateTime CallBackDate { get; set; }

    }

}
