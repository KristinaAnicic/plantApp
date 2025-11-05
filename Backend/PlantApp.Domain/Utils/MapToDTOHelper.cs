using PlantApp.Data.Models;
using PlantApp.Domain.Dtos;
using PlantApp.Domain.Dtos.Plant;

namespace PlantApp.Domain.Utils;

public static class MapToDTOHelper
{
    public static PlantDto MapPlantToPlantDto(this Plant plant)
    {
        return new PlantDto
        {
            PlantId = plant.Id,
            BotanicalName = plant.BotanicalName,
            CommonName = plant.CommonName,
            EntityDescription = plant.EntityDescription,
            Image = plant.Images?.FirstOrDefault()?.Name ?? null,
        };
    }

    public static PlantGetDto MapPlantToPlantGetDto(this Plant plant)
    {
        return new PlantGetDto
        {
            PlantId = plant.Id,
            BotanicalName = plant.BotanicalName,
            CommonName = plant.CommonName,
            EntityDescription = plant.EntityDescription,
            Image = plant.Images?.FirstOrDefault()?.Name ?? null,
            Fragrance = plant.Fragrance?.Name ?? "Unknown",
            HardinessLevel = plant.HardinessLevel != null ? $"{plant.HardinessLevel.Level} ({plant.HardinessLevel.Description})" : "Unknown",
            IsSpecie = plant.IsSpecie,
            IsGenus = plant.IsGenus,
            IsPlantForPollinators = plant.IsPlantForPollinators,
            IsLowMaintenance = plant.IsLowMaintenance,
            IsDroughtResistance = plant.IsDroughtResistance,
            SpreadType = plant.SpreadType?.Type ?? "Unknown",
            HeightType = plant.HeightType?.Type ?? "Unknown",
            TimeToFullHeight = plant.TimeToFullHeight?.Time ?? "Unknown",
            Toxicity = plant.Toxicity,
            Cultivation = plant.Cultivation,
            PestResistance = plant.PestResistance,
            DiseaseResistance = plant.DiseaseResistance,
            Pruning = plant.Pruning,
            Propagation = plant.Propagation,
            Family = plant.Family?.Name ?? "Unknown",
            GenusDescription = plant.GenusDescription,
            SoilTypes = string.Join(", ", plant.SoilTypes.Select(s => s.Name)) ?? "Unknown",
            Images = plant.Images?.Select(s => s.MapImageToImageDto()).ToList(),
            Sunlights = string.Join(", ", plant.Sunlights.Select(s =>s.Name)) ?? "Unknown",
            Aspects = string.Join(", ", plant.Aspects.Select(a =>a.Name)) ?? "Unknown",
            Moistures = string.Join(", ", plant.Moistures.Select(m =>m.Name)) ?? "Unknown",
            Phs = string.Join(", ", plant.Phs.Select(p =>p.Name)) ?? "Unknown",
            Exposures = string.Join(", ", plant.Exposures.Select(e =>e.Name)) ?? "Unknown",
            Habits = plant.Habits.Select(e =>e.Name).ToList(),
        };
    }

    public static ImageDto MapImageToImageDto(this PlantApp.Data.Models.Image img)
    {
        return new ImageDto
        {
            Id = img.Id,
            Url = $"https://apps.rhs.org.uk/plantselectorimages/detail/{img.Name}",
            Copyright = img.Copyright,
        };
    }
}
