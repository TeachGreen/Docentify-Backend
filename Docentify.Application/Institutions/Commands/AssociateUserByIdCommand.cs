using Docentify.Application.Users.ValueObject;

namespace Docentify.Application.Institutions.Commands;

public class AssociateUserByIdCommand
{
    public int UserId { get; set; }
}