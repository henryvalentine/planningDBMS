using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace DPR_DataMigrationEngine.GenericHelpers
{
    public class BaseController : Controller
    {
        protected virtual new ClaimsPrincipal User
        {
            get { return (ClaimsPrincipal)Thread.CurrentPrincipal; }
        }
    }
}