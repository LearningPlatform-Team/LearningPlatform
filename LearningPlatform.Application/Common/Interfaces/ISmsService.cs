namespace LearningPlatform.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendOtpEmailAsync(string email, string otp, string fullName, CancellationToken cancellationToken = default);
}
