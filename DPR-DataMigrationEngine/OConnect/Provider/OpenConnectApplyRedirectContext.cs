// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

// OpenID is obsolete

using Microsoft.Owin;
using Microsoft.Owin.Security;
#pragma warning disable 618
using Microsoft.Owin.Security.Provider;

namespace UIAMS.OConnect.Provider
{
    /// <summary>
    /// Context passed when a Challenge causes a redirect to authorize endpoint in the OpenConnect OpenID middleware
    /// </summary>
    public class OpenConnectApplyRedirectContext : BaseContext<OpenConnectAuthenticationOptions>
    {
        /// <summary>
        /// Creates a new context object.
        /// </summary>
        /// <param name="context">The OWIN request context</param>
        /// <param name="options">The OpenConnect OpenID middleware options</param>
        /// <param name="properties">The authentication properties of the challenge</param>
        /// <param name="redirectUri">The initial redirect URI</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "3#",
            Justification = "Represents header value")]
        public OpenConnectApplyRedirectContext(IOwinContext context, OpenConnectAuthenticationOptions options, 
            AuthenticationProperties properties, string redirectUri)
            : base(context, options)
        {
            RedirectUri = redirectUri;
            Properties = properties;
        }

        /// <summary>
        /// Gets the URI used for the redirect operation.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Represents header value")]
        public string RedirectUri { get; private set; }

        /// <summary>
        /// Gets the authentication properties of the challenge
        /// </summary>
        public AuthenticationProperties Properties { get; private set; }
    }
}
#pragma warning restore 618
