using System;
using System.Collections.Generic;
using DPR_DataMigrationEngine.EF.Models;

namespace DPR_DataMigrationEngine.EF.CustomizedModels
{
    public class WellCompletionReportObject
    {
        public string Error { get; set; }
        public int ErrorCode { get; set; }
        public int WellCompletionId { get; set; }
        public int WellCompletionTypeId { get; set; }
        public string WellName { get; set; }
        public string EquipmentName { get; set; }
        public string WellCompletionTypeName { get; set; }
        public string DatecomPletedString { get; set; }
        public DateTime DatecomPleted { get; set; }
        public string CompanyName { get; set; }
        public string WellTypeName  { get; set; }
        public string  CompletionIntervalStr  { get; set; } 
        public string  TerrainName { get; set; }
        public string ZoneName { get; set; }
        public string WellClassName { get; set; }

        public string MonthName { get; set; }
        public string Year { get; set; }
        public List<WellCompletionInterval> Intervals { get; set; } 
    }
}
