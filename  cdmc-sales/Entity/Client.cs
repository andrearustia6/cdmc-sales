using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
    public class Client : EntityBase
    {
        [Display(Name = "客户职位"),Required]
        public string Title { get; set; }
        [Display(Name = "客户姓名"),Required]
        public string Name { get; set; }
        [Display(Name = "联系方式"),Required]
        public string Contact { get; set; }

        public virtual Company Company { get; set; }
        [Display(Name = "所属公司"),Required]
        public int? CompanyID { get; set; }

        public virtual ClientType ClientType { get; set; }
        [Display(Name = "客户类型"),Required]
        public int? ClientTypeID { get; set; }

        public virtual ClientImage ClientImage { get; set; }
        public int?  ClientImageID { get; set; }
    }

    public class ClientType : EntityBase
    {
        [Display(Name = "客户类型名称"), Required]
        public string Name { get; set; }
    }
}
