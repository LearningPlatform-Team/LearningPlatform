using FluentValidation;

namespace LearningPlatform.Application.Features.Authentication.ResendOtp;

public class ResendOtpValidator : AbstractValidator<ResendOtpCommand>
{
    public ResendOtpValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email format is invalid");
    }
}
