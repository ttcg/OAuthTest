using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin.Security.Cookies;

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
            ViewBag.Message = "Your application description page.";
            var result = await HttpContext.GetOwinContext().Authentication.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationType);
            var accessToken = result.Properties.Dictionary[OpenIdConnectParameterNames.AccessToken];
            var refreshToken = result.Properties.Dictionary[OpenIdConnectParameterNames.RefreshToken];

            return View();
        }


        public ActionResult SignOut()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);

            return new EmptyResult();
        }
    }
}