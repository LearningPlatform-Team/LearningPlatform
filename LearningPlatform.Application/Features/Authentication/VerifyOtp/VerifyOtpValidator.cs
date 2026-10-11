using FluentValidation;

namespace LearningPlatform.Application.Features.Authentication.VerifyOtp;

public class VerifyOtpValidator : AbstractValidator<VerifyOtpCommand>
{
    public VerifyOtpValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email format is invalid")
            .When(x => !x.UserId.HasValue || x.UserId.Value == Guid.Empty);

        RuleFor(x => x.Otp)
            .NotEmpty().WithMessage("OTP is required")
            .Length(4, 6).WithMessage("OTP must be between 4 and 6 digits")
            .Matches(@"^\d{4,6}$").WithMessage("OTP must contain digits only");
    }
}
