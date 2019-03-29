using System;

namespace DPR_DataMigrationEngine.EF.Models
{
    public partial class WellCompletion
    {
       public string Error { get; set; }
       public int ErrorCode { get; set; }
       public string WellName { get; set; }
       public string EquipmentName { get; set; }
       public string WellCompletionTypeName { get; set; }
       public string WellTypeName { get; set; }
       public string CompanyName { get; set; }
       public string TerrainName { get; set; }
       public string ZoneName { get; set; }
       public string CompletionIntervalStr { get; set; }
       public double L1 { get; set; }
       public double U1 { get; set; }
       public double L2 { get; set; }
       public double U2 { get; set; }
       public DateTime DateCompleted2 { get; set; }
       public String DatecomPletedString { get; set; }
       public int IntervalId1 { get; set; }
       public int IntervalId2 { get; set; }
    }
}
