using Docentify.Application.Authentication.Commands;
using FluentValidation;

namespace Docentify.Application.Authentication.Validators;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.BirthDate)
            .NotNull()
            .NotEmpty()
            .LessThan(DateTime.Now)
            .GreaterThan(DateTime.Now.AddYears(18));
        
        RuleFor(x => x.Email)
            .NotNull()
            .NotEmpty()
            .EmailAddress();
        
        RuleFor(x => x.Password)
            .NotNull()
            .NotEmpty();
        
        RuleFor(x => x.Telephone)
            .Null()
            .NotEmpty()
            .Matches(@"^\d{11}$");

        RuleFor(x => x.Document)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.Gender)
            .NotEmpty();
    }
}