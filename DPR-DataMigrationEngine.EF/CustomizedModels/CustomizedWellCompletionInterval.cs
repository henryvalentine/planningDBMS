using System;

namespace DPR_DataMigrationEngine.EF.Models
{
    public partial class WellCompletionInterval
    {
       public string Error { get; set; }
       public int ErrorCode { get; set; }
       public string WellName { get; set; }
       public DateTime LastUpdatedDate { get; set; }
       public string DatecomPletedString { get; set; }
    }
}
