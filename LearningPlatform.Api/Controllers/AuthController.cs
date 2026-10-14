using System.Security.Claims;
using LearningPlatform.Application.Features.Authentication.ExternalLogin;
using LearningPlatform.Application.Features.Authentication.ForgotPassword;
using LearningPlatform.Application.Features.Authentication.Login;
using LearningPlatform.Application.Features.Authentication.Logout;
using LearningPlatform.Application.Features.Authentication.TokenRefresh;
using LearningPlatform.Application.Features.Authentication.Register;
using LearningPlatform.Application.Features.Authentication.ResendOtp;
using LearningPlatform.Application.Features.Authentication.ResetPassword;
using LearningPlatform.Application.Features.Authentication.VerifyOtp;
using LearningPlatform.Application.Features.Authentication.VerifyRegistrationOtp;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.Api.Controllers;

[Route("api/[controller]")]
public class AuthController : AppControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Register a new student account (sends 4-digit verification OTP to email)
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Verify student registration OTP, activates account and returns JWT tokens
    /// </summary>
    [HttpPost("register/verify-otp")]
    public async Task<IActionResult> VerifyRegistrationOtp([FromBody] VerifyRegistrationOtpCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Login with email and password (returns access token and refresh token)
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Refresh access token using a valid refresh token
    /// </summary>
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Login or register with Google (send the ID token from Google Sign-In SDK)
    /// </summary>
    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Login or register with Facebook (send the access token from Facebook Login SDK)
    /// </summary>
    [HttpPost("facebook")]
    public async Task<IActionResult> FacebookLogin([FromBody] FacebookLoginCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Initiate password reset — sends OTP to user's email
    /// </summary>
    [HttpPost("password/forgot")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Resend OTP verification code to user's email
    /// </summary>
    [HttpPost("password/resend-otp")]
    public async Task<IActionResult> ResendOtp([FromBody] ResendOtpCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Verify OTP for password reset
    /// </summary>
    [HttpPost("password/verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Reset password with new password
    /// </summary>
    [HttpPost("password/reset")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Logout current session and revoke refresh token
    /// </summary>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutCommand command)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        Guid? userId = Guid.TryParse(userIdString, out var uid) ? uid : null;

        var request = command with { UserId = command.UserId ?? userId };
        var result = await _mediator.Send(request);
        return HandleResult(result);
    }

    /// <summary>
    /// Get current authenticated user profile (Requires Authorize Bearer Token)
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    public ActionResult GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = User.FindFirstValue(ClaimTypes.Email);
        var name = User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue("name");
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

        return Ok(new
        {
            UserId = userId,
            Email = email,
            FullName = name,
            Roles = roles,
            IsAuthenticated = User.Identity?.IsAuthenticated ?? false
        });
    }
}
