using System.Collections.Generic;
using DPR_DataMigrationEngine.EF.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DPR_DataMigrationEngine.ViewModels
{
    public class AppUser
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public List<string> UserRoles { get; set; }
        public string Error { get; set; }
        public int Code { get; set; }
    }
}
