namespace Docentify.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    T? GetById(int id);
    
    IEnumerable<T> GetAll();
    
    void Save(T entity);
}