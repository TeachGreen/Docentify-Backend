namespace Docentify.Application.Authentication.Commands;

public class ConfirmMFACodeCommand
{
    public int? Id { get; set; }
    public int? Code { get; set; }
}