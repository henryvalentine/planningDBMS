namespace DPR_DataMigrationEngine.EF.Models
{
    public class PersonObject
    {
        public long PersonId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public long CompanyId { get; set; }
        public string Designation { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public string PersonName { get; set; }
    }
}
