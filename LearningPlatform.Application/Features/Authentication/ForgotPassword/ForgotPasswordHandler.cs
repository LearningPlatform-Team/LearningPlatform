using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Authentication.ForgotPassword;

public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IOtpService _otpService;

    public ForgotPasswordHandler(IApplicationDbContext context, IOtpService otpService)
    {
        _context = context;
        _otpService = otpService;
    }

    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user == null || !user.IsActive)
        {
            return Result.Success();
        }

        try
        {
            await _otpService.GenerateAndStoreOtpAsync(user.Id, request.Email, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message, ErrorType.Validation);
        }

        return Result.Success();
    }
}
