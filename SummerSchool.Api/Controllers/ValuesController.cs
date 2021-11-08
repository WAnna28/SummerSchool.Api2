using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SummerSchool.Api.Controllers
{
    // This code sets the route for the controller using a value (api) and a token ([controller]).
    // This route template will match URLs like www.skimedic.com/api/values.
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // There are several ways to return content as JSON from an action method.
        // The following examples all return the same JSON along with a 200 status code.
        // The differences are largely stylistic.
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new string[] { "value1", "value2" });
        }

        [HttpGet("one")]
        public IEnumerable<string> Get1()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("two")]
        public ActionResult<IEnumerable<string>> Get2()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("three")]
        public string[] Get3()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("four")]
        public IActionResult Get4()
        {
            return new JsonResult(new string[] { "value1", "value2", "value3" });
        }
    }
}