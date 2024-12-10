namespace Docentify.Application.Authentication.Commands;

public class IdentityConfirmationUserCommand
{
    public int? Id { get; set; }
    
    public string? Code { get; set; }
}