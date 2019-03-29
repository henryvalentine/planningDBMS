using System.Collections.Generic;

namespace DPR_DataMigrationEngine.EF.Models
{
    public class CompanyObject
    {
       public string Error { get; set; }
       public long ErrorCode { get; set; }
       public long CompanyId { get; set; }
       public string Name { get; set; }
       public string Address { get; set; }
       public string Contact { get; set; }
       public string RCNumber { get; set; }
       public string CanonialName { get; set; }
       public string PersonName { get; set; }
       public virtual PersonObject PersonObj { get; set; }
    }
}
