using System.Collections.Generic;

namespace DPR_DataMigrationEngine.EF.Models
{
    public class WellWorkoverReportObject
    {
        public string Error { get; set; }
        public int ErrorCode { get; set; }
        public int WellWorkOverId { get; set; }
        public string EquipmentName { get; set; }
        public string WellName { get; set; }
        public string Reason { get; set; }  
        public string ZoneName { get; set; }
        public string TerrainName { get; set; }
        public string WellClassName { get; set; }
        public string WellTypeName { get; set; }
        public string CompanyName { get; set; }
        public string DatecomPletedString { get; set; }
        public string WorkoverPeriod { get; set; }
        public int Month { get; set; }
        public string MonthStr { get; set; }
        public string YearStr { get; set; } 
        public long Year { get; set; }
        public string DateCompleted { get; set; }
    }
}
