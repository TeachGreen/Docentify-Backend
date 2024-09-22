namespace Docentify.Infrastructure.Common.Repositories;

public abstract class BaseRepository<T>(DatabaseContext context) : IRepository<T> where T : BaseEntity
{
    public virtual T? GetById(int id)
    {
        var query = context.Set<T>().Where(e => e.Id == id);

        return query.Any() ? query.FirstOrDefault() : null;
    }

    public virtual IEnumerable<T> GetAll()
    {
        var query = context.Set<T>();
        return query.Any() ? query.ToList() : [];
    }

    public virtual void Save(T entity)
    {
        context.Set<T>().Add(entity);
    }
}