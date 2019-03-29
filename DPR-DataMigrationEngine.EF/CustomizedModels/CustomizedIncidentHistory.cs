namespace DPR_DataMigrationEngine.EF.Models
{
    public partial class IncidentHistory
    {
       public string Error { get; set; }
       public int ErrorCode { get; set; }
       public string IncidentTypeName { get; set; }
       public string CompanyName { get; set; }
       public string Date { get; set; }
    }
}
