using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;
using OAuthTest.ApiStudents.ViewModels;
using OAuthTest.ApiStudents.ViewModels.Api;
using OAuthTest.Constants;

namespace OAuthTest.ApiStudents.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        // GET api/students
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return TestData.Students.Select(x => $"{x.Forename} {x.Surname}").ToList();
        }

        // GET api/students/secured
        [HttpGet("Secured")]
        [Authorize]
        public async Task<IEnumerable<Student>> GetStudents()
        {
            var students = TestData.Students;

            foreach(var student in students)
            {
                var teacher = await GetDataFromApi<Teacher>($"teachers/{student.ClassTeacherId}", await GetAccessTokenFromContext());
                student.ClassTeacherName = $"{teacher.Forename} {teacher.Surname}";
            }

            return students;
        }

        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetStudentById(Guid id)
        {
            var student = TestData.Students.SingleOrDefault(x => x.Id == id);
            if (student == null)
                return NotFound();

            return Ok(student);
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

    public class TestData
    {
        public static List<Student> Students = new List<Student>
        {
            new Student
            {
                Id = Guid.Parse("f1e5f27b-8fbc-4baf-b769-f82c25ed551e"),
                Forename = "Arv",
                Surname = "Pidgley",
                DateOfBirth = DateTime.ParseExact("08/24/2009", "MM/dd/yyyy", CultureInfo.InvariantCulture),
                ClassTeacherId = Guid.Parse("ef1670f9-cf98-4738-b3ef-5ba176ce1549")
            },
            new Student
            {
                Id = Guid.Parse("d3ddbb4f-314d-4cf7-8e5e-3f520101b411"),
                Forename = "Chantal",
                Surname = "Bedow",
                DateOfBirth = DateTime.ParseExact("08/21/2009", "MM/dd/yyyy", CultureInfo.InvariantCulture),
                ClassTeacherId = Guid.Parse("4a0f81ab-982c-4c04-ac3f-d420d0a6641d")
            },
            new Student
            {
                Id = Guid.Parse("5ad4179e-cd23-463a-8730-6d699a6ee462"),
                Forename = "Harp",
                Surname = "Isgar",
                DateOfBirth = DateTime.ParseExact("10/21/2009", "MM/dd/yyyy", CultureInfo.InvariantCulture),
                ClassTeacherId = Guid.Parse("ef1670f9-cf98-4738-b3ef-5ba176ce1549")
            },
            new Student
            {
                Id = Guid.Parse("efa6295a-c3ca-4a75-b06e-60cb14dc8559"),
                Forename = "Marnie",
                Surname = "Hurrell",
                DateOfBirth = DateTime.ParseExact("12/15/2008", "MM/dd/yyyy", CultureInfo.InvariantCulture),
                ClassTeacherId = Guid.Parse("52e19655-c4d5-4b39-bcd3-68e70a549280")
            },
            new Student
            {
                Id = Guid.Parse("edfb5ce1-484b-42ba-914c-a5dc601bc73e"),
                Forename = "Danit",
                Surname = "Leadley",
                DateOfBirth = DateTime.ParseExact("11/08/2008", "MM/dd/yyyy", CultureInfo.InvariantCulture),
                ClassTeacherId = Guid.Parse("629f31f3-ad99-4d0d-ad77-a614dad93df1")
            }
        };
    }
}
