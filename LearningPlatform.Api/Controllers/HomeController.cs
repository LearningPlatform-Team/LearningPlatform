using LearningPlatform.Api.Extensions;
using LearningPlatform.Application.Features.Home.GetAdminHome;
using LearningPlatform.Application.Features.Home.GetStudentHome;
using LearningPlatform.Application.Features.Home.GetTeachingStaffHome;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : AppControllerBase
    {
        private readonly ISender _sender;

        public HomeController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("student")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetStudentHome([FromQuery] int itemsCount = 5,CancellationToken cancellationToken = default)
        {
            var studentId = User.GetUserId();
            var query = new GetStudentHomeQuery(studentId,itemsCount);
            var result = await _sender.Send(query,cancellationToken);
            return HandleResult(result);
        }
        [HttpGet("teaching-staff")]
        [Authorize(Roles = "Doctor,TA")]
        public async Task<IActionResult> GetTeachingStaffHome([FromQuery] int itemsCount = 5,CancellationToken cancellationToken = default)
        {
            var currentUserId = User.GetUserId();
            var query = new GetTeachingStaffHomeQuery(
                currentUserId,
                User.IsDoctor(),
                User.IsTA(),
                itemsCount);
            var result = await _sender.Send(query,cancellationToken);
            return HandleResult(result);
        }
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdminHome([FromQuery] int itemsCount = 5,CancellationToken cancellationToken = default)
        {
            var currentUserId = User.GetUserId();
            var query = new GetAdminHomeQuery(currentUserId,User.IsAdmin(),itemsCount);
            var result = await _sender.Send(query,cancellationToken);
            return HandleResult(result);
        }
    }
}
