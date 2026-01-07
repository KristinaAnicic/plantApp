using Microsoft.EntityFrameworkCore;
using PlantApp.Data;
using PlantApp.Data.Models;
using PlantApp.Domain.Interfaces.Repository;

namespace PlantApp.Domain.Repositories;

public class UserRepository(AppDbContext context) : Repository<User>(context), IUserRepository
{
    public async Task<List<User>> GetAllUsers()
    {
        var query = IncludeNavigations(dbSet.AsQueryable(), false);
        query = query.Include(q => q.RatingsReceived);

        return await query.Where(u => u.DeletedAt == null).ToListAsync();
    }

    public async Task<User?> GetUserById(int id)
    {
        var query = IncludeNavigations(dbSet.AsQueryable());
        query = query.Include(u => u.Places)
                        .ThenInclude(p => p.PlantedList)
                            .ThenInclude(pl => pl.Images)
                    .Include(u => u.Places)
                        .ThenInclude(p => p.Country)
                    .Include(u => u.PlantExchanges)
                        .ThenInclude(pe => pe.ExchangeType);

        return await query.FirstOrDefaultAsync(q => q.Id == id && q.DeletedAt == null);
    }
}
