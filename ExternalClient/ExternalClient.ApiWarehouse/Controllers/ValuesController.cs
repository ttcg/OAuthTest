using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExternalClient.ApiWarehouse.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExternalClient.ApiWarehouse.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "public value1", "public value2" };
        }

        [HttpGet("secure")]
        [Authorize]
        public ActionResult<string> Secure(int id)
        {
            return "This is a secured value.  Only authenticated users can view it";
        }

        [HttpGet("retailer")]
        [Authorize(nameof(RetailerOnlyRequirement))]
        public ActionResult<string> RetailerOnly()
        {                     
            return $"This is a retailer only text.  Retailer Id is: {User.FindFirst("client_retailer").Value}";
        }

        [HttpGet("supplier")]
        [Authorize(nameof(SupplierOnlyRequirement))]
        public ActionResult<string> SupplierOnly()
        {
            return $"This is a supplier only text.  Supplier Id is: {User.FindFirst("client_supplier").Value}";
        }
    }
}
