//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ISEEDataModel.Repository
{
    using System;
    using System.Collections.Generic;
    
    public partial class CustomerEmployeeContact
    {
        public int CustomerToEmployeeSysId { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public int CustomerToEmployeeStatus { get; set; }
        public int CustomerId { get; set; }
        public int EmployeeId { get; set; }
        public Nullable<int> EmployeeGroupId { get; set; }
    
        public virtual Customer Customer { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
