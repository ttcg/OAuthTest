using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;
using OAuthTest.Constants;

namespace OAuthTest.ApiStudents.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "students1", "students2" };
        }

        [HttpGet("Secured")]
        [Authorize]
        public async Task<IEnumerable<string>> SecuredGet()
        {
            var data = await GetDataFromApi<string>("teachers/123", await GetAccessTokenFromContext());

            if (User.IsInRole("Admin")) 
                return new string[] { "Text for Admin 1", "Text for Admin 2", data };
            
            return new string[] { "Text for LoggedIn User 1", "Text for LoggedIn User 2", data };
        }

        [HttpGet("UkOnly")]
        [Authorize("UKResidenceOnly")]
        public IActionResult UkOnly()
        {
            return new JsonResult("This string is only for UK Residence Students Only.");
        }

        async Task<T> GetDataFromApi<T>(string apiRoute, string accessToken)
        {
            {
                var client = new HttpClient();

                if (string.IsNullOrWhiteSpace(accessToken) == false)
                    client.SetBearerToken(accessToken);

                client.BaseAddress = new Uri($"{Urls.ApiTeachersUrl}/api/");
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
        }

        async Task<string> GetAccessTokenFromContext()
        {        
            return await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);            
        }
    }
}
