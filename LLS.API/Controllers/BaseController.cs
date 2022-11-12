using LLS.Common.Transfere_Layer_Object;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LLS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected IActionResult CheckResult(Result result)
        {
            if (result.Status == false)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
