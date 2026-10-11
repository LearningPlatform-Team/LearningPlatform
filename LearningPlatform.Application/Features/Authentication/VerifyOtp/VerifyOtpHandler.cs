using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LearningPlatform.Application.Features.Authentication.VerifyOtp;

public class VerifyOtpHandler : IRequestHandler<VerifyOtpCommand, VerifyOtpResponse>
{
    private readonly IOtpService _otpService;
    private readonly UserManager<User> _userManager;

    public VerifyOtpHandler(IOtpService otpService, UserManager<User> userManager)
    {
        _otpService = otpService;
        _userManager = userManager;
    }

    public async Task<VerifyOtpResponse> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
    {
        User? user = null;
        if (!string.IsNullOrEmpty(request.Email))
        {
            user = await _userManager.FindByEmailAsync(request.Email);
        }
        else if (request.UserId.HasValue && request.UserId.Value != Guid.Empty)
        {
            user = await _userManager.FindByIdAsync(request.UserId.Value.ToString());
        }

        if (user == null)
        {
            return new VerifyOtpResponse(false, "Invalid email or user request.");
        }

        var isValid = await _otpService.VerifyOtpAsync(user.Id, request.Otp, cancellationToken);

        if (!isValid)
        {
            return new VerifyOtpResponse(false, "Invalid or expired OTP. Please try again.");
        }

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

        return new VerifyOtpResponse(true, "OTP verified successfully. You can now reset your password.", resetToken);
    }
}
