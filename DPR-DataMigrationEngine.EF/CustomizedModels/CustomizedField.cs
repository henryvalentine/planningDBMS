namespace DPR_DataMigrationEngine.EF.Models
{
    public partial class Field
    {
       public string Error { get; set; }
       public int ErrorCode { get; set; }
       public string BlockName { get; set; }
       public string ZoneName { get; set; }
       public string CompanyName { get; set; }
       public string CanonialName { get; set; }
       public string TerrainName { get; set; }
    }
}
