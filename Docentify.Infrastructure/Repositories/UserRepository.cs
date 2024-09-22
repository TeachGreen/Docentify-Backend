namespace Docentify.Infrastructure.Repositories;

public class UserRepository(DatabaseContext context) : BaseRepository<UserEntity>(context) 
{
}