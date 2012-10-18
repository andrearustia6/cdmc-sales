using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entity;

namespace Model
{

    public class ViewLeadCallAmount
    {
        public Member Member { get; set; }
        public int Others { get; set; }
        public int Blowed { get; set; }
        public int Not_Pitched { get; set; }
        public int Pitched { get; set; }
        public int Full_Pitched { get; set; }
        public int Call_Backed { get; set; }
        public int Waiting_For_Approval { get; set; }
        public int Qualified_Decision { get; set; }
        public int Closed { get; set; }
    }
}