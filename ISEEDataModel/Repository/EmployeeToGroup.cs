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
    
    public partial class EmployeeToGroup
    {
        public int EmployeeToGroupId { get; set; }
        public System.DateTime CreatDate { get; set; }
        public int EmployeeGroupId { get; set; }
        public int EmployeeId { get; set; }
        public int Status { get; set; }
    
        public virtual EmployeeGroup EmployeeGroup { get; set; }
    }
}
