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
    
    public partial class MilestoneStatu
    {
        public MilestoneStatu()
        {
            this.ProjectMileStones = new HashSet<ProjectMileStone>();
        }
    
        public int MilestoneStatusId { get; set; }
        public string Name { get; set; }
    
        public virtual ICollection<ProjectMileStone> ProjectMileStones { get; set; }
    }
}
