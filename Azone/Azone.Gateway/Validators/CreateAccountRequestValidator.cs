using Azone.Contracts.Models.Generated;
using FluentValidation;

namespace Azone.Gateway.Validators;

public class CreateAccountRequestValidator : AbstractValidator<CreateAccountRequest>
{
    public CreateAccountRequestValidator()
    {
        RuleFor(x => x.Login)
            .IsLogin();
        
        RuleFor(x => x.Password)
            .IsPassword();
    }
}