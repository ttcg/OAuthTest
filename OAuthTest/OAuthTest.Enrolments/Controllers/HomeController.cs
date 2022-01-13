using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin.Security.Cookies;
using OAuthTest.Enrolments.Models;

namespace OAuthTest.Enrolments.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<ActionResult> Diagnostic()
        {
            var result = await HttpContext.GetOwinContext().Authentication.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationType);

            var viewModel = new DiagnosticViewModel
            {
                RefreshToken = result.Properties.Dictionary[OpenIdConnectParameterNames.RefreshToken],
                AccessToken = result.Properties.Dictionary[OpenIdConnectParameterNames.AccessToken],
                JwtClaims = new List<Claim>(),
                UserClaims = result.Identity.Claims.ToList()
            };

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(viewModel.AccessToken);

                viewModel.JwtClaims = token.Claims.ToList();
            }
            catch { }

            return View(viewModel);
        }


        public ActionResult SignOut()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);

            return new EmptyResult();
        }
    }
}