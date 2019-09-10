﻿using System;
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
using System.Security.Claims;
using IdentityModel;
using System.Net.Mime;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace OAuthTest.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Privacy([FromQuery(Name = "secured")] string secured)
        {
            {
                foreach (var claim in User.Claims)
                {
                    Debug.WriteLine($"xxxxxxxxxxxxxxxxxxxx Claim - {claim.Type} : {claim.Value} xxxxxxxxxxxxxxxxxxxxxxxx");
                }

                UserInfoResponse response = await GetUserInfo();

                var dataFromApi = await GetDataFromApi(string.IsNullOrWhiteSpace(secured) ? "api/values" : "api/values/secured");
                if (dataFromApi == null)
                    return RedirectToAction(nameof(AccessDenied));

                var model = new PrivacyViewModel
                {
                    FirstName = response.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value,
                    Surname = response.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value,
                    StreetAddress = response.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Address)?.Value,
                    Role = response.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Role)?.Value,
                    Values = dataFromApi
                };

                return View(model);
            }

            async Task<UserInfoResponse> GetUserInfo()
            {
                var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

                var client = new HttpClient();
                client.SetBearerToken(accessToken);

                var disco = await client.GetDiscoveryDocumentAsync(Constants.IdentityServerProviderUrl);
                if (disco.IsError) throw new Exception(disco.Error);

                var response = await client.GetUserInfoAsync(new UserInfoRequest
                {
                    Address = disco.UserInfoEndpoint,
                    Token = accessToken
                });
                if (response.IsError) throw new Exception(response.Error);

                return response;
            }

            async Task<List<string>> GetDataFromApi(string apiRoute)
            {
                var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

                var client = new HttpClient();

                if (string.IsNullOrWhiteSpace(accessToken) == false)
                    client.SetBearerToken(accessToken);

                client.BaseAddress = new Uri($"{Constants.ApiUrl}");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

                var response = await client.GetAsync(apiRoute);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    var model = JsonConvert.DeserializeObject<List<string>>(json);

                    return model;
                }
                return null;
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminOnly()
        {
            return View();
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
    }
}
