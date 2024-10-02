using Docentify.Application.Authentication.Commands;
using FluentValidation;

namespace Docentify.Application.Authentication.Validators;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotNull()
            .NotEmpty()
            .EmailAddress();
        
        RuleFor(x => x.Password)
            .NotNull()
            .NotEmpty();
    }
}