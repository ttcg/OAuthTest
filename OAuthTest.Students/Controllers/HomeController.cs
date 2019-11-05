using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OAuthTest.Students.Models;
using IdentityModel.Client;
using System.Net.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Security.Claims;
using IdentityModel;
using System.Net.Mime;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Globalization;
using OAuthTest.Constants;

namespace OAuthTest.Students.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize("UKStudentOnly")]
        public async Task<IActionResult> UKOnly()
        {
            var dataFromApi = await GetDataFromApi<string>(ApiTypes.Student, "students/ukonly", await GetAccessTokenFromContext());
            if (dataFromApi == null)
                return RedirectToAction(nameof(AccessDenied));

            return View(new UkOnlyViewModel { ApiReturnText = dataFromApi });
        }

        [Authorize]
        public IActionResult Test()
        {
            {
                foreach (var claim in User.Claims)
                {
                    Debug.WriteLine($"xxxxxxxxxxxxxxxxxxxx Claim - {claim.Type} : {claim.Value} xxxxxxxxxxxxxxxxxxxxxxxx");
                }

                return Ok("Test successful");
            }
        }

        [Authorize]
        public async Task<IActionResult> Students([FromQuery(Name = "secured")] string securedApi)
        {
            {
                var accessToken = await GetAccessTokenFromContext();

                UserInfoResponse response;
                try
                {
                    response = await GetUserInfo(accessToken);
                }
                catch (UnauthorizedAccessException)
                {
                    return RedirectToAction(nameof(AccessDenied));
                }

                var studentsList = await GetDataFromApi<List<string>>(ApiTypes.Student, "students", accessToken);
                if (studentsList == null)
                    return RedirectToAction(nameof(AccessDenied));

                var model = new StudentsViewModel
                {
                    FirstName = response.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value,
                    Surname = response.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value,
                    StreetAddress = response.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Address)?.Value,
                    Role = response.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Role)?.Value,
                    Students = studentsList
                };

                return View(model);
            }

            async Task<UserInfoResponse> GetUserInfo(string accessToken)
            {
                var disco = await GetDiscoveryDocumentResponse();

                var client = new HttpClient();
                client.SetBearerToken(accessToken);

                var response = await client.GetUserInfoAsync(new UserInfoRequest
                {
                    Address = disco.UserInfoEndpoint,
                    Token = accessToken
                });
                if (response.IsError) throw new Exception(response.Error);

                return response;
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminStudents()
        {
                var accessToken = await GetAccessTokenFromContext();
                

                var studentsList = await GetDataFromApi<List<Student>>(ApiTypes.Student, "students/secured", accessToken);
                if (studentsList == null)
                    return RedirectToAction(nameof(AccessDenied));

                var model = new AdminStudentsViewModel
                {
                    Students = studentsList.Select(x => new AdminStudentsViewModel.Student
                    {
                        Id = x.Id,
                        Forename = x.Forename,
                        Surname = x.Surname,
                        ClassTeacherId = x.ClassTeacherId,
                        DateOfBirth = x.DateOfBirth,
                        TeacherName = x.ClassTeacherName
                    }).ToList()
                };

                return View(model);
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

        async Task<T> GetDataFromApi<T>(ApiTypes apiType, string apiRoute, string accessToken)
        {
            {
                var client = new HttpClient();

                if (string.IsNullOrWhiteSpace(accessToken) == false)
                    client.SetBearerToken(accessToken);

                client.BaseAddress = new Uri($"{GetApiBaseAddress()}/api/");
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

            string GetApiBaseAddress() => apiType == ApiTypes.Student ? Urls.ApiStudentsUrl : Urls.ApiTeachersUrl;
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
                    ClientId = Constants.Clients.Students
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

    public class Student
    {
        public Guid Id { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Guid ClassTeacherId { get; set; }
        public string ClassTeacherName { get; set; }
    }
}
