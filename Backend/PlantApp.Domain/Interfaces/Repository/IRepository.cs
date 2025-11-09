using System.Linq.Expressions;

namespace PlantApp.Domain.Interfaces.Repository;

public interface IRepository<T> where T : class
{
    public Task<List<T>> GetAllAsync(bool includeNavigations = false, Expression<Func<T, object>>? orderBy = null);
    public Task<T?> GetByIdAsync(int id);
    public Task AddAsync(T entity);
    public Task AddMultipleAsync(IEnumerable<T> entities);
    public Task UpdateAsync(T entity);
    public Task DeleteAsync(T entity, bool softDelete = true);
    public Task<bool> ExistsAsync(Expression<Func<T, bool>> exists);
    public Task<int> CountAsync(Expression<Func<T, bool>> count);
}
