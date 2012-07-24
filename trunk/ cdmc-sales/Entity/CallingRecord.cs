using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entity
{
    public class CallingRecord:EntityBase
    {
        public virtual Comapany Comapany { get; set; }
        public int? CompanyID { get; set; }
        public Client Client { get; set; }
        public int? ClientID { get; set; }
    }
}
