namespace DPR_DataMigrationEngine.EF.Models
{
    public partial class Block
    {
       public string Error { get; set; }
       public int ErrorCode { get; set; }
       public string FieldName { get; set; }
       public string LeaseTypeName { get; set; }
        public string BlockTypeName { get; set; }
        public string CompanyName { get; set; }
    }
}
