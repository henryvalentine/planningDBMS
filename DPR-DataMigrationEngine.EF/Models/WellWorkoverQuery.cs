//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DPR_DataMigrationEngine.EF.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class WellWorkoverQuery
    {
        public long WellWorkoverQueryId { get; set; }
        public Nullable<long> CompanyId { get; set; }
        public Nullable<int> TerrainId { get; set; }
        public Nullable<int> ZoneId { get; set; }
        public Nullable<int> WorkoverReasonId { get; set; }
        public Nullable<int> EquipmentId { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<int> WellId { get; set; }
        public string WellWorkoverQueryName { get; set; }
        public Nullable<int> WellClassId { get; set; }
        public Nullable<int> WellTypeId { get; set; }
    }
}
