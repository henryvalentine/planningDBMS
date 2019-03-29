namespace DPR_DataMigrationEngine.EF.Models
{
    public partial class Project
    {
       public string Error { get; set; }
       public int ErrorCode { get; set; }
       public string CompanyName { get; set; }
       public string TerrainName { get; set; }
       public string SectorName { get; set; }
       public string ProjectTypeName { get; set; }
       public string Completion { get; set; }
       public string DatecomPletedString { get; set; }
    }
}
