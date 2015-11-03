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
    
    public partial class EmployeeGpsPoint
    {
        public EmployeeGpsPoint()
        {
            this.GpsEmployeeCustomers = new HashSet<GpsEmployeeCustomer>();
        }
    
        public int SysId { get; set; }
        public Nullable<int> EmployeeId { get; set; }
        public System.DateTime GpsDate { get; set; }
        public Nullable<System.TimeSpan> GpsTime { get; set; }
        public Nullable<double> Lat { get; set; }
        public Nullable<double> Long { get; set; }
        public Nullable<int> Speed { get; set; }
        public Nullable<double> Accuracy { get; set; }
        public Nullable<int> Heading { get; set; }
        public Nullable<int> PointStatus { get; set; }
        public Nullable<int> AverageOfCount { get; set; }
        public Nullable<System.DateTime> StopStartTime { get; set; }
        public Nullable<System.DateTime> StopUpdateTime { get; set; }
        public Nullable<System.TimeSpan> StopTime { get; set; }
        public Nullable<int> ProcessStatus { get; set; }
        public Nullable<System.DateTime> CreateUtc { get; set; }
    
        public virtual Employee Employee { get; set; }
        public virtual ICollection<GpsEmployeeCustomer> GpsEmployeeCustomers { get; set; }
    }
}
