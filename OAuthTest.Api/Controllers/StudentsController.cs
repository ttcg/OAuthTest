using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public ActionResult<IEnumerable<string>> SecuredGet()
        {
            if(User.IsInRole("Admin")) 
                return new string[] { "Text for Admin 1", "Text for Admin 2" };
            
            return new string[] { "Text for LoggedIn User 1", "Text for LoggedIn User 2" };
        }

        [HttpGet("UkOnly")]
        [Authorize("UKResidenceOnly")]
        public IActionResult UkOnly()
        {
            return new JsonResult("This string is only for UK Residence Students Only.");
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
