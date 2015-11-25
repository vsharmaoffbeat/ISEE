using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISEEDataModel.Repository
{
    public class CalendarEvent
    {
        //id, text, start_date and end_date properties are mandatory
        public int id { get; set; }
        public string text { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public string employeeId { get; set; }
        public string customerId { get; set; }
        public string color { get; set; }
        public string uniqueid { get; set; }
    }
}
