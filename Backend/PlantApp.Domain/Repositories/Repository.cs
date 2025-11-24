using Microsoft.EntityFrameworkCore;
using PlantApp.Data;
using PlantApp.Domain.Interfaces.Repository;
using System.Linq.Expressions;

namespace PlantApp.Domain.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    public readonly AppDbContext context;
    public readonly DbSet<T> dbSet;
    public Repository(AppDbContext context){
        this.context = context;
        dbSet = this.context.Set<T>();
    }

    public async Task<List<T>> GetAllAsync(bool includeNavigations = false, Expression<Func<T, object>>? orderBy = null)
    {
        var query = dbSet.AsQueryable();

        if (includeNavigations) {
            query = IncludeNavigations(query);
        }
        if (orderBy != null) {
            query = query.OrderBy(orderBy);
        }

        var deletedProperty = typeof(T).GetProperty("DeletedAt");
        if (deletedProperty != null) {
            query = query.Where(q => EF.Property<DateTime?>(q, "DeletedAt") == null);
        }

        return await query.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        var query = dbSet.AsQueryable();
        query = IncludeNavigations(query);
            
       return await query.FirstOrDefaultAsync(q => EF.Property<int>(q, "Id") == id);
        
    }

    public async Task<List<T>> GetAllByKeyAsync(Expression<Func<T, bool>> predicate, bool includeNavigations = false)
    {
        var query = dbSet.AsQueryable();

        if (includeNavigations)
            { query = IncludeNavigations(query); }

        return await query.Where(predicate).ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await dbSet.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task AddMultipleAsync(IEnumerable<T> entities)
    {
        await dbSet.AddRangeAsync(entities);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        dbSet.Attach(entity);
        context.Entry(entity).State = EntityState.Modified;

        var updatedProperty = context.Entry(entity).Metadata.FindProperty("UpdatedAt");

        if (updatedProperty != null)
        {
            context.Entry(entity).Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
        }

        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity, bool softDelete = true)
    {
        var deleteProperty = context.Entry(entity).Metadata.FindProperty("DeletedAt");

        if (deleteProperty != null && softDelete)
        {
            dbSet.Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
            context.Entry(entity).Property("DeletedAt").CurrentValue = DateTime.UtcNow;
        }
        else
        {
            dbSet.Remove(entity);
        }

        await context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> exists)
    {
        return await dbSet.AnyAsync(exists);
    }

    public async Task<bool> IdExistsAsync(int id)
    {
        return await dbSet.AnyAsync(e => EF.Property<int>(e, "Id") == id);
    }

    public async Task<List<T>> GetByIdsAsync(List<int> ids)
    {
        if (ids == null || ids.Count == 0) return new List<T>();

        return await dbSet
            .Where(e =>
                ids.Contains(EF.Property<int>(e, "Id")))
            .ToListAsync();
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>> count)
    {
        return await dbSet.CountAsync(count);
    }

    private IQueryable<T> IncludeNavigations(IQueryable<T> query)
    {
        var navigationProperties = context.Model.FindEntityType(typeof(T))?.GetNavigations();

        if (navigationProperties != null) 
        {
            foreach (var property in navigationProperties) 
            {
                if (!property.DeclaringEntityType.IsOwned())
                {
                    query.Include(property.Name);
                }
            }
        }
        return query;
    }
}
