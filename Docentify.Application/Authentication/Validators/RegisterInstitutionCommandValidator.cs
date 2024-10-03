using Docentify.Application.Authentication.Commands;
using FluentValidation;

namespace Docentify.Application.Authentication.Validators;

public class RegisterInstitutionCommandValidator : AbstractValidator<RegisterInstitutionCommand>
{
    public RegisterInstitutionCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotNull()
            .NotEmpty();
        
        RuleFor(x => x.Email)
            .NotNull()
            .NotEmpty()
            .EmailAddress();
        
        RuleFor(x => x.Password)
            .NotNull()
            .NotEmpty();
        
        RuleFor(x => x.Telephone)
            .NotEmpty()
            .Matches(@"^\d{11}$");
    }
}