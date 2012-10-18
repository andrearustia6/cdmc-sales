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
        public int Cold_Calls{get;set;}
        public int DMS{get;set;}
        public int New_DMS{get;set;}  
        public int Duration{get;set;} 
    }

    public class ViewProjectProgressAmount
    {
        public Project Project { get; set; }
        public decimal TotalDealIn { get; set; }
        public decimal TotalCheckIn { get; set; }
        public int LeftDay { get; set; }
        public decimal DealIn { get; set; }
        public decimal DealInTarget { get; set; }
        public decimal CheckIn { get; set; }
        public decimal CheckInTarget { get; set; }
        public int DealInPercentage { get; set; }
        public int CheckInPercentage { get; set; }
        public decimal NextDealInTarget { get; set; }
        public decimal NextCheckInTarget { get; set; }
    }
}