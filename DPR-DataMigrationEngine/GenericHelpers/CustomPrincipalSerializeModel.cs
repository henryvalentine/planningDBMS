using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DPR_DataMigrationEngine.GenericHelpers
{
    public class CustomPrincipalSerializeModel
    {
        public long UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public List<string> UserRoles { get; set; }
    }
}