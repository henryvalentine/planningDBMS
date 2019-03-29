using System.Collections.Generic;
using DPR_DataMigrationEngine.EF.Models;
using DPR_DataMigrationEngine.GenericHelpers;

namespace DPR_DataMigrationEngine.ViewModels
{
    public class ProjectViewModel
    {
        public List<Terrain> Terrains { get; set; }
        public List<Company> Companies { get; set; }
        public List<ProjectType> ProjectTypes { get; set; }
        public List<Sector> Sectors { get; set; }
        public List<Project> Projects { get; set; }
    }
}
