using FluentValidation;

namespace LearningPlatform.Application.Features.Authentication.ExternalLogin;

public class GoogleLoginValidator : AbstractValidator<GoogleLoginCommand>
{
    public GoogleLoginValidator()
    {
        RuleFor(x => x.IdToken)
            .NotEmpty().WithMessage("Google ID token is required");
    }
}

public class FacebookLoginValidator : AbstractValidator<FacebookLoginCommand>
{
    public FacebookLoginValidator()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty().WithMessage("Facebook access token is required");
    }
}
