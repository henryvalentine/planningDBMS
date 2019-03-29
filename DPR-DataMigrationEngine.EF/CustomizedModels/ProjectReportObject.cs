
namespace DPR_DataMigrationEngine.EF.Models
{
    public class ProjectReportObject
    {
        public string Error { get; set; }
        public int ErrorCode { get; set; }
        public long ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string CompanyName { get; set; }
        public string TerrainName { get; set; }
        public string SectorName { get; set; }
        public string Description { get; set; }
        public string ProjectTypeName { get; set; }
        public string Duration { get; set; }
        public string Cost { get; set; }
        public string DateCompleted { get; set; }
        public string CompletionStatus { get; set; }
        public string Objectives { get; set; }
    }
}
