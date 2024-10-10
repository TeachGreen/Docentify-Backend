using Docentify.Application.Users.ValueObject;

namespace Docentify.Application.Users.Commands;

public class UpdateUserPreferencesCommand
{
    public List<PreferenceValueObject> Preferences { get; set; } = [];
}