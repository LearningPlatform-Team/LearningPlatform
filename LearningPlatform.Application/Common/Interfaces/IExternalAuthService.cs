namespace LearningPlatform.Application.Common.Interfaces;

public interface IExternalAuthService
{
    /// <summary>
    /// Validates a Google ID token and returns the user's info.
    /// </summary>
    Task<ExternalUserInfo?> ValidateGoogleTokenAsync(string idToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a Facebook access token and returns the user's info.
    /// </summary>
    Task<ExternalUserInfo?> ValidateFacebookTokenAsync(string accessToken, CancellationToken cancellationToken = default);
}

public record ExternalUserInfo(
    string Email,
    string FullName,
    string? AvatarUrl,
    string ProviderId
);
