using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;
using OAuthTest.Teachers.Models;

namespace OAuthTest.Teachers.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        
        public async Task<IActionResult> Privacy([FromQuery(Name = "secured")] string secured)
        {
            {                
                foreach (var claim in User.Claims)
                {
                    Debug.WriteLine($"xxxxxxxxxxxxxxxxxxxx Claim - {claim.Type} : {claim.Value} xxxxxxxxxxxxxxxxxxxxxxxx");
                }

                var accessToken = await GetAccessTokenFromContext();

                var dataFromApi = await GetDataFromApi<List<string>>(string.IsNullOrWhiteSpace(secured) ? "values" : "values/secured", accessToken);
                if (dataFromApi == null)
                    return RedirectToAction(nameof(AccessDenied));

                var model = new PrivacyViewModel
                {
                    FirstName = User.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value,
                    Surname = User.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value,
                    StreetAddress = User.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Address)?.Value,
                    Role = User.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Role)?.Value,
                    Values = dataFromApi
                };

                return View(model);
            }
        }

        public IActionResult AccessDenied()
        {
            return View();
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

        async Task<T> GetDataFromApi<T>(string apiRoute, string accessToken)
        {
            var client = new HttpClient();

            if (string.IsNullOrWhiteSpace(accessToken) == false)
                client.SetBearerToken(accessToken);

            client.BaseAddress = new Uri($"{Constants.Urls.ApiUrl}/api/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

            var response = await client.GetAsync(apiRoute);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                var model = JsonConvert.DeserializeObject<T>(json);

                return model;
            }
            return default(T);
        }

        async Task<string> RenewTokens()
        {
            {
                var currentContext = ControllerContext.HttpContext;
                var currentRefreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

                var disco = await GetDiscoveryDocumentResponse();

                var client = new HttpClient();
                var tokenResult = await client.RequestRefreshTokenAsync(new RefreshTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    RefreshToken = currentRefreshToken,
                    ClientSecret = Constants.Secrets.SharedSecret,
                    ClientId = Constants.Clients.Teachers
                });

                if (!tokenResult.IsError)
                {
                    // prepare the new tokens
                    var newTokens = new List<AuthenticationToken>
                    {
                        new AuthenticationToken
                        {
                            Name = OpenIdConnectParameterNames.IdToken,
                            Value = tokenResult.IdentityToken
                        },
                        new AuthenticationToken
                        {
                            Name = OpenIdConnectParameterNames.AccessToken,
                            Value = tokenResult.AccessToken
                        },
                        new AuthenticationToken
                        {
                            Name = OpenIdConnectParameterNames.RefreshToken,
                            Value = tokenResult.RefreshToken
                        },
                        new AuthenticationToken
                        {
                            Name = Constants.TokenSettings.ExpiresAt,
                            Value = GetExpiresAt(tokenResult.ExpiresIn).ToString("o", CultureInfo.InvariantCulture)
                        }
                    };

                    // reauthenticate and sign in the user by using new tokens
                    var currentAuthenticateResult = await currentContext.AuthenticateAsync("Cookies");

                    currentAuthenticateResult.Properties.StoreTokens(newTokens);

                    await currentContext.SignInAsync("Cookies", currentAuthenticateResult.Principal, currentAuthenticateResult.Properties);

                    return tokenResult.AccessToken;
                }

                throw new Exception("Renew Token failed.", tokenResult.Exception);
            }

            DateTime GetExpiresAt(int expireIn) => DateTime.UtcNow + TimeSpan.FromSeconds(expireIn);
        }

        async Task<string> GetAccessTokenFromContext()
        {
            string accessToken;

            var expires_at = await HttpContext.GetTokenAsync(Constants.TokenSettings.ExpiresAt);

            if (string.IsNullOrWhiteSpace(expires_at)
                || (DateTime.Parse(expires_at).AddSeconds(-60).ToUniversalTime() < DateTime.UtcNow))
            {
                accessToken = await RenewTokens();
            }
            else
            {
                accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            }

            return accessToken;
        }

        async Task<DiscoveryDocumentResponse> GetDiscoveryDocumentResponse()
        {
            var client = new HttpClient();

            var disco = await client.GetDiscoveryDocumentAsync(Constants.Urls.IdentityServerProviderUrl);
            if (disco.IsError) throw new Exception(disco.Error);

            return disco;
        }
    }
}
