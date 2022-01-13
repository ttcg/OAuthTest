using Microsoft.Owin;
using Owin;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.Security.Notifications;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Collections.Generic;
using IdentityModel;

[assembly: OwinStartup(typeof(OAuthTest.Enrolments.Startup))]

namespace OAuthTest.Enrolments
{
    public class Startup
    {        
        /// <summary>
        /// Configure OWIN to use OpenIdConnect
        /// </summary>
        /// <param name="app"></param>
        public void Configuration(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = Constants.Clients.Enrolments,
                    ClientSecret = Constants.Secrets.SharedSecret,
                    Authority = Constants.Urls.IdentityServerProviderUrl,
                    RedirectUri = Constants.Urls.EnrolmentsUrl,
                    PostLogoutRedirectUri = Constants.Urls.EnrolmentsUrl,
                    Scope = PopulateScopes(),
                    SignInAsAuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
                    ResponseType = OpenIdConnectResponseType.Code,
                   
                    UseTokenLifetime = false,
                    RedeemCode = true,
                    SaveTokens = true,


                    TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true 
                    },


                    // OpenIdConnectAuthenticationNotifications configures OWIN to send notification of failed authentications to OnAuthenticationFailed method
                    //Notifications = new OpenIdConnectAuthenticationNotifications
                    //{
                    //    AuthenticationFailed = OnAuthenticationFailed,
                    //    RedirectToIdentityProvider = n =>
                    //    {
                    //        if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.Authentication)
                    //        {
                    //            // set PKCE parameters
                    //            var codeVerifier = CryptoRandom.CreateUniqueId(32);

                    //            string codeChallenge;
                    //            using (var sha256 = SHA256.Create())
                    //            {
                    //                var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
                    //                codeChallenge = Base64Url.Encode(challengeBytes);
                    //            }

                    //            n.ProtocolMessage.SetParameter("code_challenge", codeChallenge);
                    //            n.ProtocolMessage.SetParameter("code_challenge_method", "S256");

                    //            // remember code_verifier (adapted from OWIN nonce cookie)
                    //            RememberCodeVerifier(n, codeVerifier);
                    //        }

                    //        return Task.CompletedTask;
                    //    },
                    //    AuthorizationCodeReceived = n =>
                    //    {
                    //        // get code_verifier
                    //        var codeVerifier = RetrieveCodeVerifier(n);

                    //        // attach code_verifier
                    //        n.TokenEndpointRequest.SetParameter("code_verifier", codeVerifier);

                    //        return Task.CompletedTask;
                    //    }
                    //}
                }
            );

            string PopulateScopes()
            {
                var scopes = new List<string>
                {
                    OpenIdConnectScope.OpenIdProfile,
                    OpenIdConnectScope.OfflineAccess,
                    OpenIdConnectScope.Email,
                    Constants.Clients.ApiStudents,
                    Constants.Clients.ApiTeachers,
                    Constants.Clients.ApiCourses,
                    "address",
                    "roles",
                    "country",
                    "custom_ids"
                };

                return string.Join(" ", scopes.ToArray());
            }
        }

        /// <summary>
        /// Handle failed authentication requests by redirecting the user to the home page with an error in the query string
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Task OnAuthenticationFailed(AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> context)
        {
            context.HandleResponse();
            context.Response.Redirect("/?errormessage=" + context.Exception.Message);
            return Task.FromResult(0);
        }

        private void RememberCodeVerifier(RedirectToIdentityProviderNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> n, string codeVerifier)
        {
            var properties = new AuthenticationProperties();
            properties.Dictionary.Add("cv", codeVerifier);
            n.Options.CookieManager.AppendResponseCookie(
                n.OwinContext,
                GetCodeVerifierKey(n.ProtocolMessage.State),
                Convert.ToBase64String(Encoding.UTF8.GetBytes(n.Options.StateDataFormat.Protect(properties))),
                new CookieOptions
                {
                    SameSite = SameSiteMode.None,
                    HttpOnly = true,
                    Secure = n.Request.IsSecure,
                    Expires = DateTime.UtcNow + n.Options.ProtocolValidator.NonceLifetime
                });
        }

        private string RetrieveCodeVerifier(AuthorizationCodeReceivedNotification n)
        {
            string key = GetCodeVerifierKey(n.ProtocolMessage.State);

            string codeVerifierCookie = n.Options.CookieManager.GetRequestCookie(n.OwinContext, key);
            if (codeVerifierCookie != null)
            {
                var cookieOptions = new CookieOptions
                {
                    SameSite = SameSiteMode.None,
                    HttpOnly = true,
                    Secure = n.Request.IsSecure
                };

                n.Options.CookieManager.DeleteCookie(n.OwinContext, key, cookieOptions);
            }

            var cookieProperties = n.Options.StateDataFormat.Unprotect(Encoding.UTF8.GetString(Convert.FromBase64String(codeVerifierCookie)));
            cookieProperties.Dictionary.TryGetValue("cv", out var codeVerifier);

            return codeVerifier;
        }

        private string GetCodeVerifierKey(string state)
        {
            using (var hash = SHA256.Create())
            {
                return OpenIdConnectAuthenticationDefaults.CookiePrefix + "cv." + Convert.ToBase64String(hash.ComputeHash(Encoding.UTF8.GetBytes(state)));
            }
        }
    }
}
