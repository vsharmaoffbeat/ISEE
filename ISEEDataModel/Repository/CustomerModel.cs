using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISEEDataModel.Repository
{
    public class CustomerModel
    {
        public int id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AreaPhone1 { get; set; }
        public string Phone1 { get; set; }
        public string CustomerNummber { get; set; }
        public string StreetDesc { get; set; }
        public string CityDesc { get; set; }
        public double? Lat { get; set; }
        public double? Long { get; set; }
    }
}
