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
    
    public partial class WellWorkover
    {
        public int WellWorkOverId { get; set; }
        public int WellId { get; set; }
        public int WellWorkOverReasonId { get; set; }
        public int EquipmentId { get; set; }
        public int Month { get; set; }
        public long Year { get; set; }
        public string DateCompleted { get; set; }
        public string CreatedBy { get; set; }
        public byte[] TimeCreated { get; set; }
    
        public virtual Equipment Equipment { get; set; }
        public virtual WellWorkOverReason WellWorkOverReason { get; set; }
        public virtual Well Well { get; set; }
    }
}