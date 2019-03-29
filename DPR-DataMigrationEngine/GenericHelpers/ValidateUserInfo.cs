using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DPR_DataMigrationEngine.GenericHelpers
{
    public class ValidateUserInfo : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var langCookie = filterContext.HttpContext.Request.Cookies["LanguagePref"];
            if (langCookie == null)
            {
                // cookie doesn't exist, either pull preferred lang from user profile
                // or just setup a cookie with the default language
                langCookie = new HttpCookie("LanguagePref", "en-gb");
                filterContext.HttpContext.Request.Cookies.Add(langCookie);
            }
            // do something with langCookie
            base.OnActionExecuting(filterContext);
        }
    }
    
}