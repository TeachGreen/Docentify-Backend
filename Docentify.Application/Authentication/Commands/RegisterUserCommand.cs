namespace Docentify.Application.Authentication.Commands;

public class RegisterUserCommand
{
    public string? Name { get; set; }
    public DateTime BirthDate { get; set; } = DateTime.Now;
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Telephone { get; set; }
    public string? Document { get; set; }
    public string? Gender { get; set; }
}