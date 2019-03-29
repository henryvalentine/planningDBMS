using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace DPR_DataMigrationEngine.GenericHelpers
{
    public class CustomPrincipal : IPrincipal
    {
        public IIdentity Identity { get; private set; }
        public bool IsInRole(string role)
        {
            if (UserRoles.Any(role.Contains))
            {
                return true;
            }
            return false;
        }

        public CustomPrincipal(string username)
        {
            Identity = new GenericIdentity(username);
        }

        public long UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public List<string> UserRoles { get; set; }
    } 
}