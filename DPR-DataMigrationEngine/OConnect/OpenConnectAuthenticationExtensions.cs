// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

// OpenID is obsolete

using Microsoft.Owin.Security;
using Owin;
#pragma warning disable 618
using System;

namespace UIAMS.OConnect
{
    /// <summary>
    /// Extension methods for using <see cref="OpenConnectAuthenticationMiddleware"/>
    /// </summary>
    public static class OpenConnectAuthenticationExtensions
    {
        /// <summary>
        /// Authenticate users using OpenConnect OpenId
        /// </summary>
        /// <param name="app">The <see cref="IAppBuilder"/> passed to the configuration method</param>
        /// <param name="options">Middleware configuration options</param>
        /// <returns>The updated <see cref="IAppBuilder"/></returns>
        [Obsolete("OpenConnect is discontinuing support for the OpenId. Use OAuth2 instead.", error: false)]
        public static IAppBuilder UseOpenConnectAuthentication(this IAppBuilder app, OpenConnectAuthenticationOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            app.Use(typeof(OpenConnectAuthenticationMiddleware), app, options);
            return app;
        }

        /// <summary>
        /// Authenticate users using OpenConnect OpenId
        /// </summary>
        /// <param name="app">The <see cref="IAppBuilder"/> passed to the configuration method</param>
        /// <returns>The updated <see cref="IAppBuilder"/></returns>
        [Obsolete("OpenConnect is discontinuing support for the OpenId. Use OAuth2 instead.", error: false)]
        public static IAppBuilder UseOpenConnectAuthentication(
            this IAppBuilder app)
        {
            return UseOpenConnectAuthentication(
                app,
                new OpenConnectAuthenticationOptions());
        }

        /// <summary>
        /// Authenticate users using OpenConnect OAuth 2.0
        /// </summary>
        /// <param name="app">The <see cref="IAppBuilder"/> passed to the configuration method</param>
        /// <param name="options">Middleware configuration options</param>
        /// <returns>The updated <see cref="IAppBuilder"/></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Auth",
            Justification = "OAuth2 is a valid word.")]
        public static IAppBuilder UseOpenConnectAuthentication(this IAppBuilder app, OpenConnectOAuth2AuthenticationOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            app.Use(typeof(OpenConnectOAuth2AuthenticationMiddleware), app, options);
            return app;
        }

        /// <summary>
        /// Authenticate users using OpenConnect OAuth 2.0
        /// </summary>
        /// <param name="app">The <see cref="IAppBuilder"/> passed to the configuration method</param>
        /// <param name="clientId">The OpenConnect assigned client id</param>
        /// <param name="clientSecret">The OpenConnect assigned client secret</param>
        /// <param name="authMode">authMode: usually set to 'Active' to suppress the redirections from ExternalLogin action of account controllers</param>
        /// <returns>The updated <see cref="IAppBuilder"/></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Auth",
            Justification = "OAuth2 is a valid word.")]
        public static IAppBuilder UseOpenConnectAuthentication(
            this IAppBuilder app,
            string clientId,
            string clientSecret,
            AuthenticationMode authMode)
        {
            return UseOpenConnectAuthentication(
                app,
                new OpenConnectOAuth2AuthenticationOptions 
                { 
                    ClientId = clientId,
                    ClientSecret = clientSecret
                });
        }
    }
}
#pragma warning restore 618