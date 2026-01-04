using Microsoft.EntityFrameworkCore;
using PlantApp.Data;
using PlantApp.Data.Models;
using PlantApp.Domain.Dtos.Plant;
using PlantApp.Domain.Interfaces.Repository;
using System.Xml.Linq;

namespace PlantApp.Domain.Repositories;

public class PlantRepository(AppDbContext context) : Repository<Plant>(context), IPlantRepository
{
    private const int pageSize = 25;
    public async Task<(int, List<Plant>)> GetPlantsFiltered(FilterByDto filter, int page)
    {
        var query = dbSet.Where(p =>
            (filter.IsLowMaintenance == null || p.IsLowMaintenance == filter.IsLowMaintenance) &&
            (filter.IsDroughtResistant == null || p.IsDroughtResistant == filter.IsDroughtResistant) &&
            (filter.Habits == null || p.Habits.Any(h => filter.Habits.Contains(h.Id))) &&
            (filter.SoilType == null || p.SoilTypes.Any(s => filter.SoilType.Contains(s.Id))) &&
            (filter.Spread == null || p.SpreadTypeId == filter.Spread) &&
            (filter.Height == null || p.HeightTypeId == filter.Height) &&
            (filter.TimeToFullHeight == null || p.TimeToFullHeightId == filter.TimeToFullHeight) &&
            (filter.Exposure == null || p.Exposures.Any(e => e.Id == filter.Exposure)) &&
            /*(string.IsNullOrEmpty(filter.Name) ||
                EF.Functions.ILike(p.CommonName, $"%{filter.Name}%") ||
                EF.Functions.ILike(p.BotanicalName, $"%{filter.Name}%")
            ) &&*/ p.DeletedAt == null
        );

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            var name = filter.Name.Trim();

            query = query
                .Where(p =>
                    EF.Functions.TrigramsSimilarity(p.CommonName, name) > 0.2 ||
                    EF.Functions.TrigramsSimilarity(p.BotanicalName, name) > 0.2
                )
                .OrderByDescending(p =>
                    Math.Max(
                        EF.Functions.TrigramsSimilarity(p.CommonName, name),
                        EF.Functions.TrigramsSimilarity(p.BotanicalName, name)
                    )
                );
        }
        else
        {
            query = query.OrderBy(p => p.CommonName);
        }

        var total = query.Count();

        return (total, await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync());
    }

    public async Task<List<Plant>> GetAllPlantsAsync(int page)
    {
        var query = dbSet
            .Where(q => q.DeletedAt == null)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
        
        return await query.ToListAsync();
    }
}
