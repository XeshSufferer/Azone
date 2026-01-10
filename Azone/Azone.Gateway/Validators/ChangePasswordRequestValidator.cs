using Azone.Contracts.Models.Generated;
using FluentValidation;

namespace Azone.Gateway;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(r => r.Password)
            .IsPassword();
        
        RuleFor(r => r.NewPassword)
            .IsPassword();

        RuleFor(r => r.Login)
            .IsLogin();
    }
}