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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.Api.Controllers;

/// <summary>
/// Authentication and Identity Management | إدارة الهوية وتسجيل الدخول
/// </summary>
/// <remarks>
/// Handles user registration (Students only), OTP verification, login, token refresh, Google/Facebook OAuth, and password recovery.
/// يدير تسجيل حسابات الطلاب، التحقق من رمز OTP، تسجيل الدخول، تجديد التوكن، وتسجيل الدخول الاجتماعي عبر Google/Facebook، واستعادة كلمة المرور.
/// </remarks>
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : AppControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Register a new student account | تسجيل حساب طالب جديد
    /// </summary>
    /// <remarks>
    /// **Public endpoint strictly for Students.**
    /// Creates a student account and automatically dispatches a 6-digit OTP code to the provided email.
    /// The account remains pending until confirmed via the `/api/Auth/register/verify-otp` endpoint.
    ///
    /// **ملاحظات لمطور الـ UI:**
    /// - مخصص للطلاب فقط؛ لا يمكن لأعضاء هيئة التدريس التسجيل من هنا.
    /// - يتم إرسال رمز OTP مكون من 6 أرقام إلى البريد الإلكتروني المدخل فوراً.
    /// - يجب توجيه المستخدم إلى شاشة إدخال رمز OTP بعد نجاح هذا الطلب.
    /// </remarks>
    /// <param name="command">Student registration details (FullName, Email, Password, ConfirmPassword, PhoneNumber).</param>
    /// <response code="200">Registration initiated successfully; returns user ID and confirmation message.</response>
    /// <response code="400">Validation failure (duplicate email, passwords mismatch, or weak password).</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Verify student registration OTP &amp; activate account | تفعيل حساب الطالب برمز OTP
    /// </summary>
    /// <remarks>
    /// **Confirms the student's email, activates their account, and automatically logs them in.**
    /// Returns the JWT `accessToken` and `refreshToken` so the app can navigate directly to Home.
    ///
    /// **ملاحظات لمطور الـ UI:**
    /// - بعد إدخال الرمز المكون من 6 أرقام بنجاح، يتم إرجاع التوكنات مباشرة.
    /// - احفظ `accessToken` و `refreshToken` في التخزين الآمن (Flutter Secure Storage) وانتقل للشاشة الرئيسية.
    /// </remarks>
    /// <param name="command">User ID (received from register) and the 6-digit OTP code.</param>
    /// <response code="200">Account activated successfully; returns JWT access token, refresh token, and user profile.</response>
    /// <response code="400">Invalid or expired OTP code.</response>
    [HttpPost("register/verify-otp")]
    [ProducesResponseType(typeof(VerifyRegistrationOtpResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyRegistrationOtp([FromBody] VerifyRegistrationOtpCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Login with email and password | تسجيل الدخول بالبريد الإلكتروني وكلمة المرور
    /// </summary>
    /// <remarks>
    /// Authenticates any user (Student, Doctor, TA, Admin) and returns a JWT access token (valid for 60 min) and refresh token (valid for 7 days).
    ///
    /// **ملاحظات لمطور الـ UI:**
    /// - يرجع التوكن والأدوار (`Roles`).
    /// - بناءً على قيمة `Roles` (مثال: `Student` أو `Doctor` أو `Admin`) يتم توجيه المستخدم للواجهة الخاصة به.
    /// </remarks>
    /// <param name="command">User credentials (Email and Password).</param>
    /// <response code="200">Login successful; returns tokens, user ID, full name, and roles list.</response>
    /// <response code="400">Invalid credentials or deactivated account.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Refresh access token | تجديد رمز الدخول المنتهي
    /// </summary>
    /// <remarks>
    /// Call this silently when the `accessToken` expires (HTTP 401) using the stored `refreshToken`.
    /// Rotates the refresh token (the old refresh token is revoked, and a new pair is issued).
    ///
    /// **ملاحظات لمطور الـ UI:**
    /// - استخدم هذا الـ Endpoint عبر Dio Interceptor عند تلقي 401 لتجديد الجلسة في الخلفية دون تسجيل خروج المستخدم.
    /// </remarks>
    /// <param name="command">Existing active refresh token string.</param>
    /// <response code="200">Tokens refreshed successfully; returns new access token and new refresh token.</response>
    /// <response code="400">Invalid, expired, or already-revoked refresh token.</response>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(RefreshTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Login or register with Google | تسجيل الدخول أو إنشاء حساب عبر جوجل
    /// </summary>
    /// <remarks>
    /// Pass the Google `idToken` obtained from the Google Sign-In SDK (`google_sign_in` package in Flutter).
    /// If the user does not exist, an account is automatically created with the `Student` role and pre-verified email.
    ///
    /// **ملاحظات لمطور الـ UI:**
    /// - احصل على `idToken` من Google Sign-In في التطبيق وأرسله في الـ Body.
    /// - يتم إرجاع توكنات المنصة مباشرة مثل تسجيل الدخول العادي.
    /// </remarks>
    /// <param name="command">Google OAuth ID token string.</param>
    /// <response code="200">Google authentication successful; returns JWT tokens and profile.</response>
    /// <response code="400">Invalid Google ID token.</response>
    [HttpPost("google")]
    [ProducesResponseType(typeof(ExternalLoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Login or register with Facebook | تسجيل الدخول أو إنشاء حساب عبر فيسبوك
    /// </summary>
    /// <remarks>
    /// Pass the Facebook `accessToken` obtained from the Facebook Login SDK (`flutter_facebook_auth` package in Flutter).
    /// If the user does not exist, an account is automatically created with the `Student` role.
    /// </remarks>
    /// <param name="command">Facebook OAuth access token string.</param>
    /// <response code="200">Facebook authentication successful; returns JWT tokens and profile.</response>
    /// <response code="400">Invalid Facebook access token or email permission was not granted.</response>
    [HttpPost("facebook")]
    [ProducesResponseType(typeof(ExternalLoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> FacebookLogin([FromBody] FacebookLoginCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Request password reset OTP | طلب رمز استعادة كلمة المرور
    /// </summary>
    /// <remarks>
    /// Sends a 6-digit OTP code to the registered email address for password recovery.
    /// Always returns success to protect against email enumeration.
    ///
    /// **ملاحظات لمطور الـ UI:**
    /// - ينقل المستخدم لشاشة إدخال OTP لنسيان كلمة السر.
    /// - الرمز صالح لمدة 5 دقائق.
    /// </remarks>
    /// <param name="command">The registered email address.</param>
    /// <response code="200">OTP dispatched to email if the account exists.</response>
    /// <response code="204">Request accepted; OTP dispatched.</response>
    [HttpPost("password/forgot")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Resend OTP code | إعادة إرسال رمز التحقق
    /// </summary>
    /// <remarks>
    /// Resends a fresh OTP code to the provided email, invalidating previous codes.
    ///
    /// **ملاحظات لمطور الـ UI:**
    /// - استخدم هذا الـ Endpoint عند الضغط على "Resend Code" / "إعادة إرسال الرمز".
    /// </remarks>
    /// <param name="command">Email address of the user.</param>
    /// <response code="200">New OTP dispatched to email.</response>
    /// <response code="204">New OTP dispatched to email.</response>
    [HttpPost("password/resend-otp")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ResendOtp([FromBody] ResendOtpCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Verify password reset OTP | التحقق من رمز OTP لنسيان كلمة المرور
    /// </summary>
    /// <remarks>
    /// Verifies the 6-digit OTP entered by the user. Returns a temporary `resetToken` required in the next step.
    ///
    /// **ملاحظات لمطور الـ UI:**
    /// - بعد نجاح هذا الطلب، احتفظ بـ `resetToken` المرسل في الرد وأرسله في الخطوة التالية لتغيير كلمة المرور.
    /// </remarks>
    /// <param name="command">Email and 6-digit OTP code.</param>
    /// <response code="200">OTP verified; returns resetToken.</response>
    /// <response code="400">Invalid or expired OTP code.</response>
    [HttpPost("password/verify-otp")]
    [ProducesResponseType(typeof(VerifyOtpResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Reset password with new password | تعيين كلمة المرور الجديدة
    /// </summary>
    /// <remarks>
    /// Sets a new password for the user and revokes all their existing active sessions/refresh tokens.
    ///
    /// **ملاحظات لمطور الـ UI:**
    /// - بعد نجاح هذا الطلب، أظهر شاشة النجاح ووجّه المستخدم لشاشة تسجيل الدخول (`Login`).
    /// </remarks>
    /// <param name="command">Email, ResetToken, NewPassword, and optional ConfirmPassword.</param>
    /// <response code="200">Password reset successfully.</response>
    /// <response code="204">Password reset successfully.</response>
    /// <response code="400">Validation failure or password complexity not met.</response>
    [HttpPost("password/reset")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Logout current session | تسجيل الخروج وإلغاء الجلسة
    /// </summary>
    /// <remarks>
    /// Revokes the provided refresh token so it can no longer be used.
    ///
    /// **ملاحظات لمطور الـ UI:**
    /// - أرسل الـ `refreshToken` والـ `userId` لإلغاء الجلسة على السيرفر، ثم احذف التوكنات محلياً من الهاتف.
    /// </remarks>
    /// <param name="command">Logout command containing refreshToken and optional userId.</param>
    /// <response code="200">Logged out successfully.</response>
    /// <response code="204">Logged out successfully.</response>
    /// <response code="400">Invalid refresh token.</response>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout([FromBody] LogoutCommand command)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        Guid? userId = Guid.TryParse(userIdString, out var uid) ? uid : null;

        var request = command with { UserId = command.UserId ?? userId };
        var result = await _mediator.Send(request);
        return HandleResult(result);
    }

    /// <summary>
    /// Get current authenticated user profile | استعلام بيانات المستخدم الحالي
    /// </summary>
    /// <remarks>
    /// **Requires Bearer Token authentication (use Authorize button in Swagger).**
    /// Returns the authenticated user's ID, email, full name, and assigned roles.
    ///
    /// **ملاحظات لمطور الـ UI:**
    /// - استخدم هذا الـ Endpoint للتأكد من صلاحية التوكن وجلب معلومات وبيانات المستخدم المسجل حالياً.
    /// </remarks>
    /// <response code="200">Returns user profile and roles.</response>
    /// <response code="401">Unauthorized — token is missing, invalid, or expired.</response>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
