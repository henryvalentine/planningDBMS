using System.Collections.Generic;

namespace DPR_DataMigrationEngine.GenericHelpers
{
    public class ServiceResponse
    {
        public int ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public long UserId { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public List<string> UserRoles { get; set; }
        public string AccessToken { get; set; }
        public string State { get; set; }
        public string AuthorizationScheme { get; set; }
        public string ExpiresIn { get; set; }
        public string RefreshToken { get; set; }
    }
}
