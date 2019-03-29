
using System.Collections.Generic;
using DPR_DataMigrationEngine.EF.Models;

namespace DPR_DataMigrationEngine.GenericHelpers
{
   public class GenericValidator
    {
       public string Error { get; set; }
       public string UserName { get; set; } 
       public string Email { get; set; } 
       public int Code { get; set; }
       public List<WellQuery> WellQueries { get; set; }
       public List<string> UserRoles { get; set; }
       public List<ProductionQuery> ProductionQueries { get; set; }
    }
}
