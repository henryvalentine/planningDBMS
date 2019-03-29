namespace DPR_DataMigrationEngine.EF.Models
{
    public partial class ProjectMileStone
    {
       public string Error { get; set; }
       public int ErrorCode { get; set; }
       public string ParentName { get; set; }
       public string ProjectName { get; set; }
       public string DateDueString { get; set; }
       public string MileStoneName { get; set; }
    }
}

  