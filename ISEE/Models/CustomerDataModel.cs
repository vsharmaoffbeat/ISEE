using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISEE.Models
{
    public class CustomerDataModel
    {
        public string Apartment { get; set; }
        public string Phone2 { get; set; }
        public string Mail { get; set; }
        public string Fax { get; set; }
        public string CustomerNumber { get; set; }
        public string Floor { get; set; }
        public string Phone1 { get; set; }
        public string PhoneArea1 { get; set; }
        public string PhoneArea2 { get; set; }
        public int VisitInterval { get; set; }
        public int VisitTime { get; set; }
        public DateTime NextVisit { get; set; }
        public int BuldingCode { get; set; }
        public string ContactName { get; set; }
    }
}