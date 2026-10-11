namespace LearningPlatform.Application.Common.Interfaces;

public interface IOtpService
{
    Task<string> GenerateAndStoreOtpAsync(Guid userId, string phoneNumber, CancellationToken cancellationToken = default);
    Task<bool> VerifyOtpAsync(Guid userId, string otp, CancellationToken cancellationToken = default);
    Task InvalidateOtpAsync(Guid userId, CancellationToken cancellationToken = default);
}
