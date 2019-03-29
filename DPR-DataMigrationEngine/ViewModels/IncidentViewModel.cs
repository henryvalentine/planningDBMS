using System.Collections.Generic;
using DPR_DataMigrationEngine.EF.Models;
using DPR_DataMigrationEngine.GenericHelpers;

namespace DPR_DataMigrationEngine.ViewModels
{
    public class IncidentViewModel
    {
        public List<IncidentType> IncidentTypes { get; set; }
        public List<Company> Companies { get; set; }
        public List<IncidentHistory> Incidents { get; set; }
        public List<DocObject> Years { get; set; }
        public List<DocObject> Months { get; set; }
        public List<IncidentQuery> IncidentQueries { get; set; }
    }
}
