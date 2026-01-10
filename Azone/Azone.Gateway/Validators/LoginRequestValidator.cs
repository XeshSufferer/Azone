using Azone.Contracts.Models.Generated;
using FluentValidation;

namespace Azone.Gateway.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public  LoginRequestValidator()
    {
        RuleFor(x => x.Login)
            .IsLogin();
        
        RuleFor(x => x.Password)
            .IsPassword();
    }
}