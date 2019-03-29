using System.Web;

namespace DPR_DataMigrationEngine.EF.Models
{
    public partial class Well
    {
       public string Error { get; set; }
       public int ErrorCode { get; set; }
       public string WellTypeName { get; set; }
       public string FieldName { get; set; }
       public string BlockName { get; set; }
       public string CompanyName { get; set; }
       public string Date { get; set; }
       public HttpPostedFileBase Template { get; set; }
       public int WellClassId { get; set; }
       public int WellClassificationId { get; set; }
       //public int BlockId { get; set; }
       public string WellClassName { get; set; }
       public string ZoneName { get; set; }
       public string TerrainName { get; set; }
    }
}  
