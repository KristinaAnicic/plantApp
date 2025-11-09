using PlantApp.Data.Models;
using PlantApp.Domain.Dtos.Plant;

namespace PlantApp.Domain.Utils;

public static class MapFromDTOHelper
{
    public static Plant MapUpsertPlantDtoToPlant(this UpsertPlantDto dto)
    {
        return new Plant
        {
            BotanicalName = dto.BotanicalName,
            CommonName = dto.CommonName,
            SynonymParentPlantId = dto.SynonymParentPlantId,
            FragranceId = dto.FragranceId,
            HardinessLevelId = dto.HardinessLevelId,
            IsSpecie = dto.IsSpecie,
            IsGenus = dto.IsGenus,
            IsPlantForPollinators = dto.IsPlantForPollinators,
            IsLowMaintenance = dto.IsLowMaintenance,
            IsDroughtResistant = dto.IsDroughtResistant,
            SpreadTypeId = dto.SpreadTypeId,
            HeightTypeId = dto.HeightTypeId,
            TimeToFullHeightId = dto.TimeToFullHeightId,
            Toxicity = dto.Toxicity,
            Cultivation = dto.Cultivation,
            PestResistance = dto.PestResistance,
            DiseaseResistance = dto.DiseaseResistance,
            Pruning = dto.Pruning,
            Propagation = dto.Propagation,
            FamilyId = dto.FamilyId,
            EntityDescription = dto.EntityDescription,
            GenusDescription = dto.GenusDescription
        };
    }

    public static Plant MapUpsertPlantDtoToPlant(this UpsertPlantDto dto, Plant existingPlant)
    {
        existingPlant.BotanicalName = dto.BotanicalName;
        existingPlant.CommonName = dto.CommonName;
        existingPlant.SynonymParentPlantId = dto.SynonymParentPlantId;
        existingPlant.FragranceId = dto.FragranceId;
        existingPlant.HardinessLevelId = dto.HardinessLevelId;
        existingPlant.IsSpecie = dto.IsSpecie;
        existingPlant.IsGenus = dto.IsGenus;
        existingPlant.IsPlantForPollinators = dto.IsPlantForPollinators;
        existingPlant.IsLowMaintenance = dto.IsLowMaintenance;
        existingPlant.IsDroughtResistant = dto.IsDroughtResistant;
        existingPlant.SpreadTypeId = dto.SpreadTypeId;
        existingPlant.HeightTypeId = dto.HeightTypeId;
        existingPlant.TimeToFullHeightId = dto.TimeToFullHeightId;
        existingPlant.Toxicity = dto.Toxicity;
        existingPlant.Cultivation = dto.Cultivation;
        existingPlant.PestResistance = dto.PestResistance;
        existingPlant.DiseaseResistance = dto.DiseaseResistance;
        existingPlant.Pruning = dto.Pruning;
        existingPlant.Propagation = dto.Propagation;
        existingPlant.FamilyId = dto.FamilyId;
        existingPlant.EntityDescription = dto.EntityDescription;
        existingPlant.GenusDescription = dto.GenusDescription;

        return existingPlant;
    }

}
