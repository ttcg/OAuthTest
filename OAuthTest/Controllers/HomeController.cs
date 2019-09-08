using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OAuthTest.Web.Models;
using IdentityModel.Client;
using System.Net.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace OAuthTest.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Privacy()
        {
            var currentIdentity = HttpContext.User;
            foreach(var claim in currentIdentity.Claims)
            {
                Debug.WriteLine($"xxxxxxxxxxxxxxxxxxxx Claim - {claim.Type} : {claim.Value} xxxxxxxxxxxxxxxxxxxxxxxx");
            }

            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            var client = new HttpClient();

            client.SetBearerToken(accessToken);

            var disco = await client.GetDiscoveryDocumentAsync("https://localhost:44378/");
            
            if (disco.IsError) throw new Exception(disco.Error);

            var response = await client.GetUserInfoAsync(new UserInfoRequest
            {
                Address = disco.UserInfoEndpoint,
                Token = accessToken
            });
            if (response.IsError) throw new Exception(response.Error);

            var model = new PrivacyViewModel
            {
                FirstName = response.Claims.FirstOrDefault(x => x.Type == "name")?.Value,
                Surname = response.Claims.FirstOrDefault(x => x.Type == "given_name")?.Value,
                StreetAddress = response.Claims.FirstOrDefault(x => x.Type == "address")?.Value
            };

            return View(model);
        }

        public async Task Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            await HttpContext.SignOutAsync("oidc");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
