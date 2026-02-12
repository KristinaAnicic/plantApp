using Microsoft.EntityFrameworkCore;
using PlantApp.Domain.Dtos.Planted;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Models;
using System.Globalization;

namespace PlantApp.Data.Repositories;

public class PlantGroupRepository(AppDbContext context) : Repository<PlantGroup>(context), IPlantGroupRepository
{
    public async Task<PlantGroup?> GetPlantGroupById(int id)
    {
        var query = dbSet.AsQueryable();
        query = IncludeNavigations(query);
        query = query
            .Include(q => q.GrowthLogs)
                .ThenInclude(g => g.Images)
            .Include(q => q.GrowthLogs)
                .ThenInclude(g => g.PlantStatus)
            .Include(q => q.User)
            .Include(q => q.PlantedList)
                .ThenInclude(p => p.Plant)
                    .ThenInclude(pl => pl.Images)
            .Include(q => q.PlantedList)
                .ThenInclude(p => p.Images)
            .Include(q => q.PlantedList)
                .ThenInclude(p => p.Place)
            .Where(p => p.DeletedAt == null);

        return await query.FirstOrDefaultAsync(q => q.Id == id);
    }

}
