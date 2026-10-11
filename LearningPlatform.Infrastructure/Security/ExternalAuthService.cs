using System.Text.Json;
using Google.Apis.Auth;
using LearningPlatform.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LearningPlatform.Infrastructure.Security;

public class ExternalAuthService : IExternalAuthService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ExternalAuthService> _logger;

    public ExternalAuthService(
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        ILogger<ExternalAuthService> logger)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<ExternalUserInfo?> ValidateGoogleTokenAsync(string idToken, CancellationToken cancellationToken = default)
    {
        try
        {
            var clientId = _configuration["Authentication:Google:ClientId"]
                ?? throw new InvalidOperationException("Google ClientId is not configured");

            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { clientId }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

            return new ExternalUserInfo(
                Email: payload.Email,
                FullName: payload.Name ?? payload.Email,
                AvatarUrl: payload.Picture,
                ProviderId: payload.Subject
            );
        }
        catch (InvalidJwtException ex)
        {
            _logger.LogWarning(ex, "Invalid Google ID token");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating Google token");
            return null;
        }
    }

    public async Task<ExternalUserInfo?> ValidateFacebookTokenAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        try
        {
            var appId = _configuration["Authentication:Facebook:AppId"]
                ?? throw new InvalidOperationException("Facebook AppId is not configured");
            var appSecret = _configuration["Authentication:Facebook:AppSecret"]
                ?? throw new InvalidOperationException("Facebook AppSecret is not configured");

            var httpClient = _httpClientFactory.CreateClient();

            // Step 1: Verify the token is valid and belongs to our app
            var debugUrl = $"https://graph.facebook.com/debug_token?input_token={accessToken}&access_token={appId}|{appSecret}";
            var debugResponse = await httpClient.GetAsync(debugUrl, cancellationToken);

            if (!debugResponse.IsSuccessStatusCode)
            {
                _logger.LogWarning("Facebook token debug request failed");
                return null;
            }

            var debugJson = await debugResponse.Content.ReadAsStringAsync(cancellationToken);
            var debugData = JsonDocument.Parse(debugJson);
            var data = debugData.RootElement.GetProperty("data");

            if (!data.GetProperty("is_valid").GetBoolean())
            {
                _logger.LogWarning("Facebook token is not valid");
                return null;
            }

            var tokenAppId = data.GetProperty("app_id").GetString();
            if (tokenAppId != appId)
            {
                _logger.LogWarning("Facebook token app_id mismatch");
                return null;
            }

            // Step 2: Get user info from Facebook Graph API
            var userInfoUrl = $"https://graph.facebook.com/me?fields=id,name,email,picture.type(large)&access_token={accessToken}";
            var userResponse = await httpClient.GetAsync(userInfoUrl, cancellationToken);

            if (!userResponse.IsSuccessStatusCode)
            {
                _logger.LogWarning("Facebook user info request failed");
                return null;
            }

            var userJson = await userResponse.Content.ReadAsStringAsync(cancellationToken);
            var userData = JsonDocument.Parse(userJson);
            var root = userData.RootElement;

            var email = root.TryGetProperty("email", out var emailProp) ? emailProp.GetString() : null;
            var name = root.TryGetProperty("name", out var nameProp) ? nameProp.GetString() : null;
            var id = root.GetProperty("id").GetString()!;

            string? avatarUrl = null;
            if (root.TryGetProperty("picture", out var pictureProp) &&
                pictureProp.TryGetProperty("data", out var pictureData) &&
                pictureData.TryGetProperty("url", out var urlProp))
            {
                avatarUrl = urlProp.GetString();
            }

            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("Facebook user {FacebookId} did not grant email permission", id);
                return null;
            }

            return new ExternalUserInfo(
                Email: email,
                FullName: name ?? email,
                AvatarUrl: avatarUrl,
                ProviderId: id
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating Facebook token");
            return null;
        }
    }
}
