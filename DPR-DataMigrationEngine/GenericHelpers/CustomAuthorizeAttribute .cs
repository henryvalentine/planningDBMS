using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DPR_DataMigrationEngine.GenericHelpers
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public string UsersConfigKey { get; set; }
        public string RolesConfigKey { get; set; }

        protected virtual ClaimsPrincipal CurrentUser
        {
            get {
                
                return (ClaimsPrincipal)Thread.CurrentPrincipal; 
            }
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                if (!String.IsNullOrEmpty(Roles))
                {
                    //var dty = Roles.Split(',');

                    //var principal = HttpContext.Current.User as ClaimsPrincipal;
                    //if (principal != null)
                    //{
                    //    var ddf = principal.Claims.Where(m => m.Type == ClaimTypes.Role);
                    //    if(ddf.Any(j => j.Value == ))
                    //}

                    if (!CurrentUser.IsInRole(Roles))
                    {
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Login" }));
                    }
                }

                if (!String.IsNullOrEmpty(Users))
                {
                    if (!Users.Contains(CurrentUser.Identity.Name))
                    {
                        filterContext.Result = new RedirectToRouteResult(new
                        RouteValueDictionary(new
                        {
                            controller = "Account", action = "Login"
                        }));
                    }
                }
            }
        }
    }
}