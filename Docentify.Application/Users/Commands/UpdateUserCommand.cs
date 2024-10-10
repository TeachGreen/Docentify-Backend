namespace Docentify.Application.Users.Commands;

public class UpdateUserCommand
{
    public string? Name { get; set; }
    public DateTime BirthDate { get; set; } = DateTime.Now;
    public string? Email { get; set; }
    public string? Telephone { get; set; }
    public string? Document { get; set; }
    public string? Gender { get; set; }
}