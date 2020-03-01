using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OAuthTest.ApiCourses.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        // GET api/courses
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "text1 from courses api", "text2 from courses api" };
        }

        // GET api/courses/secured
        [HttpGet("Secured")]
        [Authorize]
        public ActionResult<string> Secured(int id)
        {
            return "This is a secured text";
        }
    }
}
