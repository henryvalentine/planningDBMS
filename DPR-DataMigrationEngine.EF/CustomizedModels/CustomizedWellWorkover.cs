using System;

namespace DPR_DataMigrationEngine.EF.Models
{
    public partial class WellWorkover
    {
       public string Error { get; set; }
       public int ErrorCode { get; set; }
       public string EquipmentName { get; set; }
       public string WellName { get; set; }
       public string Reason { get; set; }
       public string ZoneName { get; set; }
        public string TerrainName { get; set; }
        public string WellTypeName { get; set; }
         public string CompanyName { get; set; }
         public string MonthStr { get; set; }
         public string YearStr { get; set; } 
         public string DatecomPletedString { get; set; }
         public string WorkoverPeriod { get; set; }
         public string BlockName { get; set; }
         public DateTime WorkoverDate { get; set; }
         public DateTime WorkoverDatestr { get; set; } 
    }
}
