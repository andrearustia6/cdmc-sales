using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
    /// <summary>
    /// 电话结果
    /// </summary>
    public class LeadCallSheetType : EntityBase
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
    public class LeadCallSheet : EntityBase
    {
        [Display(Name = "First Pitch"), Required]
        public bool IsFirstPitch { get; set; }
    
        public virtual Lead Lead { get; set; }
        [Display(Name = "致电客户"), Required]
        public int? LeadID { get; set; }


        [Display(Name = "所属项目"), Required]
        public int? ProjectID { get; set; }
        public virtual Project Project { get; set; }

        public virtual LeadCallSheetType LeadCallSheetType { get; set; }
        public int? LeadCallSheetTypeID { get; set; }

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
