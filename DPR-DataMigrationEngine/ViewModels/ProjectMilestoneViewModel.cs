using System.Collections.Generic;
using DPR_DataMigrationEngine.EF.Models;
using DPR_DataMigrationEngine.GenericHelpers;

namespace DPR_DataMigrationEngine.ViewModels
{
    public class ProjectMilestoneViewModel
    {
        public List<MilestoneStatu> MilestoneStatuses { get; set; }
        public List<ProjectMileStone> ProjectMileStones { get; set; }
        public List<Project> Projects { get; set; }
        public List<DocObject> CompletionStatuses { get; set; }
    }
}
