using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            return new string[] { "teachers1", "teachers2" };
        }

        // GET api/secured
        [HttpGet("secured")]
        [Authorize]
        public ActionResult<IEnumerable<string>> Secured()
        {
            return new string[] { "Secured Text Teacher Api 1", "Secured Text Teacher Api 2" };
        }

        // GET api/teachers/5/students
        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<string> GetById(int id)
        {
            var teachers = new Dictionary<int, string>
            {
                { 123, "teacher name Jon from teachersApi" },
                { 456, "teacher name Rooney from teachersApi" }
            };
            return teachers[id];
        }
    }
}
