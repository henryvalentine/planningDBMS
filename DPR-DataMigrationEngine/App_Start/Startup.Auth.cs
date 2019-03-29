using System;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace DPR_DataMigrationEngine
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Enable the application to use a cookie to store information for the signed in user
            app.SetDefaultSignInAsAuthenticationType(DefaultAuthenticationTypes.ApplicationCookie);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                //AuthenticationMode = AuthenticationMode.Active,
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                ExpireTimeSpan = TimeSpan.FromMinutes(10),
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider { OnApplyRedirect = ApplyRedirect }
            });

            // Enable the External Sign In Cookie.
            //app.SetDefaultSignInAsAuthenticationType(DefaultAuthenticationTypes.ExternalCookie);

            // //Use a cookie to temporarily store information about a user logging in with a third party login provider
            //app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            //app.UseCookieAuthentication(new CookieAuthenticationOptions
            //{
            //    AuthenticationType = DefaultAuthenticationTypes.ExternalCookie,
            //    AuthenticationMode = AuthenticationMode.Passive,
            //    CookieName = CookieAuthenticationDefaults.CookiePrefix + "External",
            //    ExpireTimeSpan = TimeSpan.FromMinutes(30),
            //    Provider = new CookieAuthenticationProvider { OnApplyRedirect = ApplyRedirect }
            //});
            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

           // app.UseOpenConnectAuthentication("3151987258", "5eb493e8-9afd-491f-baa4-b9e73ef09cc1", AuthenticationMode.Active);
        }

        private static void ApplyRedirect(CookieApplyRedirectContext context)
        {
            Uri absoluteUri;
            if (Uri.TryCreate(context.RedirectUri, UriKind.Absolute, out absoluteUri))
            {
                var path = PathString.FromUriComponent(absoluteUri);
                if (path == context.OwinContext.Request.PathBase + context.Options.LoginPath)
                    context.RedirectUri = "http://localhost:44302/Account/Login" +
                        new QueryString(
                            context.Options.ReturnUrlParameter,
                            context.Request.Uri.AbsoluteUri);
            }

            context.Response.Redirect(context.RedirectUri);
        }

        
    }
}