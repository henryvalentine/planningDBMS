namespace DPR_DataMigrationEngine.EF.Models
{
    public class FieldReportObject
    {
       public string Error { get; set; }
       public int ErrorCode { get; set; }
       public int FieldId { get; set; }
       public string FieldName { get; set; }
       public string BlockName { get; set; }
       public string TerrainName { get; set; }
       public string ZoneName { get; set; }
       public string CompanyName { get; set; }
       public double? Area { get; set; }
       public string Name { get; set; }
       public float? TechnicalAllowable { get; set; }
       public string TechnicalAllowableStr { get; set; }
    }
}
