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
    
    public partial class ReportName
    {
        public ReportName()
        {
            this.ReportPanels = new HashSet<ReportPanel>();
        }
    
        public int ReporNametPk { get; set; }
        public string ReportName1 { get; set; }
        public int ReportParmeterTypeFk { get; set; }
        public int LeftRight { get; set; }
        public int ReportCategoryFk { get; set; }
        public string EN { get; set; }
        public string HE { get; set; }
        public string RU { get; set; }
        public string ES { get; set; }
        public string DE { get; set; }
    
        public virtual ReportCategory ReportCategory { get; set; }
        public virtual ICollection<ReportPanel> ReportPanels { get; set; }
    }
}
