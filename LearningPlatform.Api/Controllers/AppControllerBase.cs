using LearningPlatform.Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.Api.Controllers
{
    [ApiController]
    public class AppControllerBase : ControllerBase
    {
        protected IActionResult HandleResult<T>(ResultT<T> result)
        {
            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return NoContent();

                return Ok(result.Data);
            }

            return result.ErrorType switch
            {
                ErrorType.NotFound => NotFound(new { Message = result.ErrorMessage }),
                ErrorType.Unauthorized => Unauthorized(new { Message = result.ErrorMessage }),
                ErrorType.Validation => BadRequest(new { Message = result.ErrorMessage }),
                ErrorType.Conflict => Conflict(new { Message = result.ErrorMessage }),
                _ => BadRequest(new { Message = result.ErrorMessage })
            };
        }
        protected IActionResult HandleResult(Result result)
        {
            if (result.IsSuccess)
                return NoContent();

            return result.ErrorType switch
            {
                ErrorType.NotFound => NotFound(new { Message = result.ErrorMessage }),
                ErrorType.Unauthorized => Unauthorized(new { Message = result.ErrorMessage }),
                ErrorType.Validation => BadRequest(new { Message = result.ErrorMessage }),
                ErrorType.Conflict => Conflict(new { Message = result.ErrorMessage }),
                _ => BadRequest(new { Message = result.ErrorMessage })
            };
        }
    }
}
