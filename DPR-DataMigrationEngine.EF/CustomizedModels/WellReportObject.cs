using System;
using System.Collections.Generic;

namespace DPR_DataMigrationEngine.EF.Models
{
    public class WellReportObject
    {
       public string Error { get; set; }
       public int ErrorCode { get; set; }
       public int WellId { get; set; }
       public string TerrainName { get; set; }
        public string Name { get; set; }
        public string Remarks { get; set; }
        public string BlockName { get; set; }
        public double TechnicalAllowable { get; set; }
        public double TotalDept { get; set; }
        public string WellTypeName { get; set; }
        public string FieldName { get; set; }
        public string CompanyName { get; set; }
        public string Date { get; set; }
        public string WellClassName { get; set; }
        public string ZoneName { get; set; }
        public DateTime? SpudDate { get; set; }
        public List<Company> Companies { get; set; }
    }
}
