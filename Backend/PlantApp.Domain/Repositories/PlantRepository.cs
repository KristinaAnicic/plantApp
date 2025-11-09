using Microsoft.EntityFrameworkCore;
using PlantApp.Data;
using PlantApp.Data.Models;
using PlantApp.Domain.Dtos.Plant;
using PlantApp.Domain.Interfaces.Repository;

namespace PlantApp.Domain.Repositories;

public class PlantRepository(AppDbContext context) : Repository<Plant>(context), IPlantRepository
{
    public async Task<List<Plant>> GetPlantsFiltered(FilterByDto filter, string? name = null)
    {
        return await dbSet.Where(p =>
            (filter.IsLowMaintenance == null || p.IsLowMaintenance == filter.IsLowMaintenance) &&
            (filter.IsDroughtResistant == null || p.IsDroughtResistant == filter.IsDroughtResistant) &&
            (filter.Habits == null || p.Habits.Any(h => filter.Habits.Contains(h.Id))) &&
            (filter.SoilType == null || p.SoilTypes.Any(s => filter.SoilType.Contains(s.Id))) &&
            (filter.Spread == null || p.SpreadTypeId == filter.Spread) &&
            (filter.Height == null || p.HeightTypeId == filter.Height) &&
            (filter.TimeToFullHeight == null || p.TimeToFullHeightId == filter.TimeToFullHeight) &&
            (filter.Exposure == null || p.Exposures.Any(e => e.Id == filter.Exposure)) &&
            (string.IsNullOrEmpty(name) || 
                EF.Functions.ILike(p.CommonName, $"%{name}%") || 
                EF.Functions.ILike(p.BotanicalName, $"%{name}%")
            ) && p.DeletedAt == null
        ).ToListAsync();
    }
}
