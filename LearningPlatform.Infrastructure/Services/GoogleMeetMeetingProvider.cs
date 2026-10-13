using LearningPlatform.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace LearningPlatform.Infrastructure.Services
{
    public class GoogleMeetMeetingProvider : IMeetingProvider
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<GoogleMeetMeetingProvider> _logger;

        public GoogleMeetMeetingProvider(IConfiguration configuration, ILogger<GoogleMeetMeetingProvider> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public Task<MeetingRoomResult> CreateMeetingRoomAsync(
            string title,
            string joinCode,
            DateTime? scheduledAt = null,
            int durationMinutes = 60,
            CancellationToken cancellationToken = default)
        {
            // Standard Google Meet URI pattern: meet.google.com/xxx-yyyy-zzz
            var meetCode = GenerateGoogleMeetCode();
            var joinUrl = $"https://meet.google.com/{meetCode}";

            _logger.LogInformation("Generated Google Meet room for meeting '{Title}' with join code {JoinCode}: {JoinUrl}",
                title, joinCode, joinUrl);

            return Task.FromResult(new MeetingRoomResult(
                RoomId: meetCode,
                JoinUrl: joinUrl,
                ProviderName: "Google Meet"
            ));
        }

        private static string GenerateGoogleMeetCode()
        {
            const string alphabet = "abcdefghijklmnopqrstuvwxyz";
            var part1 = GenerateRandomString(alphabet, 3);
            var part2 = GenerateRandomString(alphabet, 4);
            var part3 = GenerateRandomString(alphabet, 3);
            return $"{part1}-{part2}-{part3}";
        }

        private static string GenerateRandomString(string chars, int length)
        {
            var buffer = new char[length];
            for (var i = 0; i < length; i++)
            {
                var idx = RandomNumberGenerator.GetInt32(0, chars.Length);
                buffer[i] = chars[idx];
            }
            return new string(buffer);
        }
    }
}
