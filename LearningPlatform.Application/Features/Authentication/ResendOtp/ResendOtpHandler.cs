using LearningPlatform.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Authentication.ResendOtp;

public class ResendOtpHandler : IRequestHandler<ResendOtpCommand, ResendOtpResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IOtpService _otpService;

    public ResendOtpHandler(IApplicationDbContext context, IOtpService otpService)
    {
        _context = context;
        _otpService = otpService;
    }

    public async Task<ResendOtpResponse> Handle(ResendOtpCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user == null || !user.IsActive)
        {
            return new ResendOtpResponse(true, "If an account with this email exists, a new verification code has been sent.");
        }

        try
        {
            await _otpService.GenerateAndStoreOtpAsync(user.Id, request.Email, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            return new ResendOtpResponse(false, ex.Message);
        }

        return new ResendOtpResponse(true, "A new verification code has been sent to your email.");
    }
}
