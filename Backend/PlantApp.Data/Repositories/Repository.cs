using Microsoft.EntityFrameworkCore;
using PlantApp.Domain.Interfaces.Repository;
using System.Linq.Expressions;

namespace PlantApp.Data.Repositories;

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

    /*public async Task<List<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
    {
        var query = dbSet.AsQueryable();

        foreach (var include in includes)
        {
            if (include != null)
                query = query.Include(include.Name);
        }    

        var deletedProperty = typeof(T).GetProperty("DeletedAt");
        if (deletedProperty != null)
        {
            query = query.Where(q => EF.Property<DateTime?>(q, "DeletedAt") == null);
        }

        return await query.ToListAsync();
    }*/

    public async Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[]? includes)
    {
        var query = dbSet.AsQueryable();

        if (includes != null)
        {
            foreach (var include in includes)
            {
                if (include != null)
                    query = query.Include(include);
            }
        }

        query = IncludeNavigations(query);

        var deletedProperty = typeof(T).GetProperty("DeletedAt");
        if (deletedProperty != null)
        {
            query = query.Where(q => EF.Property<DateTime?>(q, "DeletedAt") == null);
        }

        return await query.FirstOrDefaultAsync(q => EF.Property<int>(q, "Id") == id);
        
    }

    public async Task<T?> GetByKeyAsync(Expression<Func<T, bool>> key, bool includeNavigations = false, params Expression<Func<T, object>>[]? includes)
    {
        var query = dbSet.AsQueryable();

        if (includes != null)
        {
            foreach (var include in includes)
            {
                if (include != null)
                    query = query.Include(include);
            }
        }

        if (includeNavigations)
            query = IncludeNavigations(query);

        var deletedProperty = typeof(T).GetProperty("DeletedAt");
        if (deletedProperty != null)
        {
            query = query.Where(q => EF.Property<DateTime?>(q, "DeletedAt") == null);
        }

        return await query.FirstOrDefaultAsync(key);
    }

    public async Task<List<T>> GetAllByKeyAsync(
        Expression<Func<T, bool>> predicate, 
        bool includeNavigations = false,
        int? page = null,
        int? pageSize = null,
        params Expression<Func<T, object>>[]? includes)
    {
        var query = dbSet.AsQueryable();

        if (includes != null)
        {
            foreach (var include in includes)
            {
                if (include != null)
                    query = query.Include(include);
            }
        }

        if (includeNavigations)
            { query = IncludeNavigations(query); }

        query = query.Where(predicate);

        var deletedProperty = typeof(T).GetProperty("DeletedAt");
        if (deletedProperty != null)
        {
            query = query.Where(q => EF.Property<DateTime?>(q, "DeletedAt") == null);
        }

        if (page != null)
        {
            query = query.Skip((page.Value - 1) * (pageSize ?? 25))
                         .Take(pageSize ?? 25);
        }

        return await query.ToListAsync();
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
        var entry = context.Entry(entity);
        if (entry.State == EntityState.Detached)
        {
            dbSet.Attach(entity);
            entry.State = EntityState.Modified;
        }

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

    public async Task DeleteRangeAsync(IEnumerable<T> entities, bool softDelete = true)
    {
        if (entities == null || !entities.Any()) return;

        var deleteProperty = typeof(T).GetProperty("DeletedAt");

        if (deleteProperty != null && softDelete)
        {
            context.Entry(entities).State = EntityState.Modified;
            context.Entry(entities).Property("DeletedAt").CurrentValue = DateTime.UtcNow;
        }
        else
        {
            dbSet.RemoveRange(entities);
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

        var query = dbSet.AsQueryable();

        var deletedProperty = typeof(T).GetProperty("DeletedAt");
        if (deletedProperty != null)
        {
            query = query.Where(q => EF.Property<DateTime?>(q, "DeletedAt") == null);
        }

        return await query
            .Where(e =>
                ids.Contains(EF.Property<int>(e, "Id")))
            .ToListAsync();
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>>? count = null)
    {
        if (count == null)
            return await dbSet.CountAsync();

        return await dbSet.CountAsync(count);
    }

    /*public IQueryable<T> IncludeNavigations(IQueryable<T> query)
    {
        var navigationProperties = context.Model.FindEntityType(typeof(T))?.GetNavigations();

        if (navigationProperties != null) 
        {
            foreach (var property in navigationProperties) 
            {
                if (!property.DeclaringEntityType.IsOwned())
                {
                    query = query.Include(property.Name);
                }
            }
        }
        return query;
    }*/

    public IQueryable<T> IncludeNavigations(IQueryable<T> query, bool all = true)
    {
        var entityType = context.Model.FindEntityType(typeof(T));

        if (entityType == null)
            return query;

        foreach (var navigation in entityType.GetNavigations())
        {
            if (!navigation.DeclaringEntityType.IsOwned())
            {
                query = query.Include(navigation.Name);
            }
        }

        if (all)
        {
            foreach (var skipNavigation in entityType.GetSkipNavigations())
            {
                query = query.Include(skipNavigation.Name);
            }
        }

        return query;
    }

}
