using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISEEDataModel.Repository
{
    public class ReportFilterCriteria
    {
        public string FilterType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CustomerNumber { get; set; }
        public bool Active { get; set; }
    }
}
