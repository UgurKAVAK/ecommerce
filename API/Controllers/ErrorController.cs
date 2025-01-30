using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [HttpGet("not-found")]
        public IActionResult NotFoundError()
        {
            return NotFound(); // 404
        }
        [HttpGet("bad-request")]
        public IActionResult BadRequestError()
        {
            return BadRequest(); // 400
        }
        [HttpGet("unauthorized")]
        public IActionResult UnAuthorizedError()
        {
            return Unauthorized(); // 401
        }
        [HttpGet("validation-error")]
        public IActionResult ValidationError()
        {
            ModelState.AddModelError("Validation Error 1", "Validation Error Details");
            ModelState.AddModelError("Validation Error 2", "Validation Error Details");
            return ValidationProblem();
        }
        [HttpGet("server-error")]
        public IActionResult ServerError()
        {
            throw new Exception("Server Error"); // 500
        }
    }
}
