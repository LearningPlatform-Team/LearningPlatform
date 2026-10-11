using LearningPlatform.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Authentication.ForgotPassword;

public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, ForgotPasswordResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IOtpService _otpService;

    public ForgotPasswordHandler(IApplicationDbContext context, IOtpService otpService)
    {
        _context = context;
        _otpService = otpService;
    }

    public async Task<ForgotPasswordResponse> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user == null || !user.IsActive)
        {
            return new ForgotPasswordResponse(true, "If an account with this email exists, a verification code has been sent.");
        }

        try
        {
            await _otpService.GenerateAndStoreOtpAsync(user.Id, request.Email, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            return new ForgotPasswordResponse(false, ex.Message);
        }

        return new ForgotPasswordResponse(true, "If an account with this email exists, a verification code has been sent.");
    }
}
