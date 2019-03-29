using System.Collections.Generic;

namespace DPR_DataMigrationEngine.EF.Models
{
    public class ProductionObject
    {
        public int ProductionId { get; set; }
        public string Error { get; set; }
        public int ErrorCode { get; set; }
        public string FieldName { get; set; }
        public string ProductName { get; set; }
        public string SheetName { get; set; }
        public string MonthName { get; set; }
        public string BlockName { get; set; }
        public string BlockTypeName { get; set; }
        public string TerrainName { get; set; }
        public string CompanyName { get; set; }
        public string ZoneName { get; set; }
        public float TechnicalAllowable { get; set; }
        public string Quantity { get; set; }
        public int Month { get; set; }
        public long Year { get; set; }
        public string Remark { get; set; }
        public string APIGravity { get; set; }

        public string Periodstr { get; set; }  
        public string Period { get; set; }  
    }
}
