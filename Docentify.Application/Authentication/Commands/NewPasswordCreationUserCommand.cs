namespace Docentify.Application.Authentication.Commands;

public class NewPasswordCreationUserCommand
{
    public int? Id { get; set; }
    
    public string? Code { get; set; }
    
    public string? Password { get; set; }
}