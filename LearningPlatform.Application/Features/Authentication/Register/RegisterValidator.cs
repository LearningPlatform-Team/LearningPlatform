using FluentValidation;

namespace LearningPlatform.Application.Features.Authentication.Register;

public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MinimumLength(2).WithMessage("Full name must be at least 2 characters")
            .MaximumLength(256).WithMessage("Full name must not exceed 256 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email format is invalid")
            .MaximumLength(256).WithMessage("Email must not exceed 256 characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("Passwords do not match")
            .When(x => !string.IsNullOrEmpty(x.ConfirmPassword));

        RuleFor(x => x.PhoneNumber)
            .Must(x => string.IsNullOrEmpty(x) || (x.Length >= 10 && x.Length <= 20))
            .WithMessage("Phone number must be between 10 and 20 characters")
            .Must(x => string.IsNullOrEmpty(x) || System.Text.RegularExpressions.Regex.IsMatch(x, @"^\+?[0-9\s\-]+$"))
            .WithMessage("Phone number format is invalid")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
    }
}
