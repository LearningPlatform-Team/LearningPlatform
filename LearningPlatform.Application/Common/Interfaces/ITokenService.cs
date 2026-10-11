namespace LearningPlatform.Application.Common.Interfaces;
public interface ITokenService
{
    string GenerateAccessToken(Guid userId, string email, string fullName, IEnumerable<string>? roles = null);
    string GenerateRefreshToken();
    bool ValidateAccessToken(string token);
    Guid? GetUserIdFromToken(string token);
}
