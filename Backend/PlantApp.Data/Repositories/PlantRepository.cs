using Microsoft.EntityFrameworkCore;
using PlantApp.Domain.Models;
using PlantApp.Domain.Dtos.Plant;
using PlantApp.Domain.Interfaces.Repository;

namespace PlantApp.Data.Repositories;

public class PlantRepository(AppDbContext context) : Repository<Plant>(context), IPlantRepository
{
    private const int pageSize = 25;
    public async Task<(int, List<Plant>)> GetPlantsFiltered(FilterByDto filter, int page)
    {
        var query = dbSet.Where(p =>
            p.DeletedAt == null &&
            (filter.IsLowMaintenance == null || p.IsLowMaintenance == filter.IsLowMaintenance) &&
            (filter.IsDroughtResistant == null || p.IsDroughtResistant == filter.IsDroughtResistant) &&
            (filter.Habits == null || p.Habits.Any(h => filter.Habits.Contains(h.Id))) &&
            (filter.SoilType == null || p.SoilTypes.Any(s => filter.SoilType.Contains(s.Id))) &&
            (filter.Spread == null || p.SpreadTypeId == filter.Spread) &&
            (filter.Height == null || p.HeightTypeId == filter.Height) &&
            (filter.TimeToFullHeight == null || p.TimeToFullHeightId == filter.TimeToFullHeight) &&
            (filter.Exposure == null || p.Exposures.Any(e => e.Id == filter.Exposure))
        );

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            var name = filter.Name.Trim();

            Expression<Func<Plant, bool>> matches = p =>
                EF.Functions.ILike(p.CommonName, $"%{name}%") ||
                EF.Functions.ILike(p.BotanicalName, $"%{name}%") ||
                (p.Family != null && EF.Functions.ILike(p.Family.Name, $"%{name}%"));

            query = query
                .Where(matches)             
                .OrderByDescending(p => p.SynonymParentPlantId != null)
                .ThenByDescending(matches)
                .ThenByDescending(p => p.Images.Any(i => i.Url != null && i.Url != ""))
                .ThenBy(p => p.Id);
        }
        else
        {
            query = query
                .OrderByDescending(p => p.Images.Any(i => i.Url != null && i.Url != ""))
                .ThenBy(p => p.SynonymParentPlantId != null)
                .ThenBy(p => p.CommonName)
                .ThenBy(p => p.Id);
        }

        var total = await query.CountAsync();

        return (total, await query
            .Include(p => p.Images)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync());
    }
    public async Task<(int, List<Plant>)> GetAllPlantsAsync(int page)
    {
        var baseQuery = dbSet.Where(q => q.DeletedAt == null);
        var total = await baseQuery.CountAsync();

        var plants = await baseQuery
            .Include(p => p.Images)
            //.OrderByDescending(p => p.Images.Any(i => i.Url != null && i.Url != ""))
            .OrderBy(p => p.SynonymParentPlantId != null)
            .ThenByDescending(p => p.Images.Count())            
            .ThenBy(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (total, plants);
    }
}
