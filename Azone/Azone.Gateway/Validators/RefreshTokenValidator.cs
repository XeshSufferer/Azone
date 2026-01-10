using Azone.Contracts.Models.Generated;
using FluentValidation;

namespace Azone.Gateway.Validators;

public class RefreshTokenValidator : AbstractValidator<LogoutRequest>
{
    public RefreshTokenValidator()
    {
        RuleFor(l => l.RefreshToken)
            .NotEmpty().WithMessage("RefreshToken is required")
            .IsGuid().WithMessage("RefreshToken is not a valid GUID");
    }
}