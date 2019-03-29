using System;
using System.Web.Security;

namespace DPR_DataMigrationEngine.GenericHelpers
{
    public class SystemUserHelper
    {
        public int GetLoggedOnUserId()
        {
            var membershipUser = Membership.GetUser();
            if (membershipUser != null)
            {
                return Convert.ToInt32(membershipUser.ProviderUserKey);
            }
            return 0;
        }
    }
}