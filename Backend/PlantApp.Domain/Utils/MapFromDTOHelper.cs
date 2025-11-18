using PlantApp.Data.Models;
using PlantApp.Domain.Dtos.Plant;

namespace PlantApp.Domain.Utils;

public static class MapFromDTOHelper
{
    public static void MapValuesUpsertPlantDtoToPlant(UpsertPlantDto dto, Plant plant)
    {
        plant.BotanicalName = dto.BotanicalName;
        plant.CommonName = dto.CommonName;
        plant.SynonymParentPlantId = dto.SynonymParentPlantId;
        plant.FragranceId = dto.FragranceId;
        plant.HardinessLevelId = dto.HardinessLevelId;
        plant.IsSpecie = dto.IsSpecie;
        plant.IsGenus = dto.IsGenus;
        plant.IsPlantForPollinators = dto.IsPlantForPollinators;
        plant.IsLowMaintenance = dto.IsLowMaintenance;
        plant.IsDroughtResistant = dto.IsDroughtResistant;
        plant.SpreadTypeId = dto.SpreadTypeId;
        plant.HeightTypeId = dto.HeightTypeId;
        plant.TimeToFullHeightId = dto.TimeToFullHeightId;
        plant.Toxicity = dto.Toxicity;
        plant.Cultivation = dto.Cultivation;
        plant.PestResistance = dto.PestResistance;
        plant.DiseaseResistance = dto.DiseaseResistance;
        plant.Pruning = dto.Pruning;
        plant.Propagation = dto.Propagation;
        plant.FamilyId = dto.FamilyId;
        plant.EntityDescription = dto.EntityDescription;
        plant.GenusDescription = dto.GenusDescription;
    }

    public static Plant MapUpsertPlantDtoToPlant(this UpsertPlantDto dto, Plant? plant = null)
    {
        if (plant == null)
            plant = new Plant { BotanicalName = dto.BotanicalName, CommonName = dto.CommonName };

        MapValuesUpsertPlantDtoToPlant(dto, plant);
        return plant;
    }
    }

}
