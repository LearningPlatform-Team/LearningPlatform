using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Entities.Identity;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LearningPlatform.Application.Features.Authentication.VerifyOtp;

public class VerifyOtpHandler : IRequestHandler<VerifyOtpCommand, ResultT<VerifyOtpResponse>>
{
    private readonly IOtpService _otpService;
    private readonly UserManager<User> _userManager;

    public VerifyOtpHandler(IOtpService otpService, UserManager<User> userManager)
    {
        _otpService = otpService;
        _userManager = userManager;
    }

    public async Task<ResultT<VerifyOtpResponse>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
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
            return ResultT<VerifyOtpResponse>.Failure("Invalid email or user request.", ErrorType.NotFound);
        }

        var isValid = await _otpService.VerifyOtpAsync(user.Id, request.Otp, cancellationToken);

        if (!isValid)
        {
            return ResultT<VerifyOtpResponse>.Failure("Invalid or expired OTP. Please try again.", ErrorType.Validation);
        }

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

        return ResultT<VerifyOtpResponse>.Success(new VerifyOtpResponse(resetToken));
    }
}
