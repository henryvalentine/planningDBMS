using System;

namespace DPR_DataMigrationEngine.EF.Models
{
    public partial class Production
    {
       public string Error { get; set; }
       public int ErrorCode { get; set; }
       public string FieldName { get; set; }
       public string ProductName { get; set; }
       public string SheetName { get; set; }
       public string MonthName { get; set; }
       public string BlockName { get; set; }
       public string ZoneName  { get; set; }
       public string TerrainName  { get; set; }
       public string CompanyName  { get; set; }
       public string Periodstr { get; set; }
       public float TechnicalAllowable { get; set; }
        public DateTime ProductionDate { get; set; }
        public string ProductionDateString { get; set; }
        
    }
}
