using System;

namespace DPR_DataMigrationEngine.EF.Models
{
    public class IncidentReportObject
    {
       public string Error { get; set; }
       public int ErrorCode { get; set; }
       public string IncidentTypeName { get; set; }
       public string Description { get; set; }
       public string ReportedBy { get; set; }
       public string IncidentName { get; set; }
       public string CompanyName { get; set; }
       public string Location { get; set; }
       public int IncidentMonth { get; set; }
       public int IncidentYear { get; set; }
       public DateTime IncidentDate { get; set; }
       public string IncidentDateStr { get; set; }
    }
}
