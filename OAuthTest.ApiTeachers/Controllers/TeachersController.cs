using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OAuthTest.ApiTeachers.ViewModels;

namespace OAuthTest.ApiTeachers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachersController : ControllerBase
    {
        // GET api/teachers
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return TestData.Teachers.Select(x => x.Forename).ToList();
        }

        // GET api/secured
        [HttpGet("secured")]
        [Authorize]
        public ActionResult<List<Teacher>> GetTeachers()
        {
            return TestData.Teachers;
        }

        // GET api/teachers/5
        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<Teacher> GetById(Guid id)
        {
            var teacher = TestData.Teachers.SingleOrDefault(x => x.Id == id);
            if (teacher == null)
                return NotFound();

            return teacher;
        }
    }

    public class TestData
    {
        public static List<Teacher> Teachers = new List<Teacher>
        {
            new Teacher
            {
                Id = Guid.Parse("ef1670f9-cf98-4738-b3ef-5ba176ce1549"),
                Forename = "Wendy",
                Surname = "Taylor",
                Students = new List<Guid>{
                    Guid.Parse("f1e5f27b-8fbc-4baf-b769-f82c25ed551e"),
                    Guid.Parse("d3ddbb4f-314d-4cf7-8e5e-3f520101b411")
                }
            },
            new Teacher
            {
                Id = Guid.Parse("4a0f81ab-982c-4c04-ac3f-d420d0a6641d"),
                Forename = "Thomas",
                Surname = "McAlroy",
                Students = new List<Guid>{
                    Guid.Parse("5ad4179e-cd23-463a-8730-6d699a6ee462")
                }
            },
            new Teacher
            {
                Id = Guid.Parse("629f31f3-ad99-4d0d-ad77-a614dad93df1"),
                Forename = "Bob",
                Surname = "Dylan",
                Students = new List<Guid>{
                    Guid.Parse("efa6295a-c3ca-4a75-b06e-60cb14dc8559"),
                    Guid.Parse("edfb5ce1-484b-42ba-914c-a5dc601bc73e")
                }
            },
            new Teacher
            {
                Id = Guid.Parse("52e19655-c4d5-4b39-bcd3-68e70a549280"),
                Forename = "Natalie",
                Surname = "Smith",
                Students = new List<Guid>{
                    Guid.Parse("5ad4179e-cd23-463a-8730-6d699a6ee462")
                }
            },
            new Teacher
            {
                Id = Guid.Parse("e94d6387-ae59-42fc-b97e-1d162f36834d"),
                Forename = "Shaun",
                Surname = "Connery"
            }
        };
    }
}
