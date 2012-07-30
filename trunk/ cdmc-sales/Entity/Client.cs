using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entity
{
    public class Client : EntityBase
    {
        public string Title { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }
        public virtual Company Company { get; set; }
        public int? CompanyID { get; set; }
    }
}
