using LearningPlatform.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace LearningPlatform.Infrastructure.Messaging.Email;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendOtpEmailAsync(string email, string otp, string fullName, CancellationToken cancellationToken = default)
    {
        try
        {
            var host = _configuration["EmailSettings:Host"] ?? throw new InvalidOperationException("Email Host is not configured");
            var port = _configuration.GetValue<int>("EmailSettings:Port", 587);
            var username = _configuration["EmailSettings:Username"] ?? throw new InvalidOperationException("Email Username is not configured");
            var password = _configuration["EmailSettings:Password"] ?? throw new InvalidOperationException("Email Password is not configured");
            var fromEmail = _configuration["EmailSettings:FromEmail"] ?? throw new InvalidOperationException("From Email is not configured");

            using var client = new SmtpClient(host, port);
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(username, password);

            var subject = "Your Verification Code - Learnora";
            var body = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1'>
    <title>Verification Code</title>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
        <div style='background-color: #407BFA; padding: 30px; text-align: center; border-radius: 10px 10px 0 0;'>
            <h1 style='color: white; margin: 0;'>Learnora</h1>
        </div>
        <div style='background-color: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px;'>
            <h2 style='color: #407BFA;'>Hello {fullName},</h2>
            <p>Your verification code is:</p>
            <div style='background-color: #E5EDFF; padding: 20px; text-align: center; borderradius: 5px; margin: 20px 0;'>
                <span style='font-size: 32px; font-weight: bold; color: #407BFA; letter-spacing: 5px;'>{otp}</span>
            </div>
            <p>This code will expire in 5 minutes.</p>
            <p>If you didn't request this code, please ignore this email.</p>
            <p style='margin-top: 30px; font-size: 12px; color: #666;'>
                This is an automated message from Learnora. Please do not reply to this email.
            </p>
        </div>
    </div>
</body>
</html>";

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, "Learnora"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage, cancellationToken);
            _logger.LogInformation("OTP email sent successfully to {Email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send OTP email to {Email}", email);
            throw;
        }
    }
}
