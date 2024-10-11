namespace Docentify.Application.Authentication.Commands;

public class RegisterInstitutionCommand
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Telephone { get; set; }
    public string? Document { get; set;  }
    public string? Address { get; set; }
}