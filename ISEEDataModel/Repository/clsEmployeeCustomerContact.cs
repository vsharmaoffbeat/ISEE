using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ISEEDataModel.Repository
{
    [DataContract]
    public partial class clsEmployeeCustomerContact
    {
        [DataMember]
        [Key]
        public int ID { get; set; }
        [DataMember]
        public DateTime CreateDate { get; set; }

        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public int EmpID { get; set; }
        [DataMember]
        public string EmpNumber { get; set; }


        public clsEmployeeCustomerContact()
        {

        }
    }

}
