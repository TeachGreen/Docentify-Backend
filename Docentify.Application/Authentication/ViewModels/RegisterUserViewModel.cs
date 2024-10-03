namespace Docentify.Application.Authentication.ViewModels;

public class RegisterUserViewModel
{
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
    public string Email { get; set; }
    public string? Telephone { get; set; }
    public string Document { get; set; }
    public string? Gender { get; set; }
}