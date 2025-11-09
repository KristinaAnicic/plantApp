using Microsoft.EntityFrameworkCore;
using PlantApp.Data.Models;
using PlantApp.Domain.Dtos;
using PlantApp.Domain.Dtos.Plant;
using PlantApp.Domain.Dtos.Planted;
using PlantApp.Domain.Dtos.PlantExchange;
using PlantApp.Domain.Dtos.PlantPlace;
using PlantApp.Domain.Dtos.User;

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
            IsDroughtResistant = plant.IsDroughtResistant,
            SpreadType = plant.SpreadType?.Name ?? "Unknown",
            HeightType = plant.HeightType?.Name ?? "Unknown",
            TimeToFullHeight = plant.TimeToFullHeight?.Name ?? "Unknown",
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
            Seasons = plant.Seasons.Select(e =>e.Name).ToList()
        };
    }

    public static ImageDto MapImageToImageDto(this PlantApp.Data.Models.Image img)
    {
        return new ImageDto
        {
            Id = img.Id,
            Url = img.Name,
            Copyright = img.Copyright,
        };
    }
    public static PlaceDto MapPlaceToPlaceDto(this Place place)
    {
        return new PlaceDto
        {
            Id =place.Id,
            Name = place.Name,
            Address = $"{place.Address} ({place.City}, {place.Country?.Name})",
        };
    }

    public static PlaceGetDto MapPlaceToPlaceGetDto(this Place place)
    {
        return new PlaceGetDto
        {
            Id = place.Id,
            Name = place.Name,
            Address = place.Address,
            City = $"{place.City}, {place.Country?.Name}",
            Note = place.Note,
            Planted = place.PlantedList?.Select(p => p.MapPlantedToPlantedDto()).ToList()
        };
    }

    public static UserGetDto MapUserToUserGetDto(this User user)
    {
        return new UserGetDto
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            Role = user.Role?.Name ?? "Unknown",
            Gender = user.Gender,
            DateOfBirth = user.DateOfBirth,
            Places = user.Places?.Select(p  => p.MapPlaceToPlaceGetDto()).ToList(),
            PlantExchanges = user.PlantExchanges?.Select(pe => pe.MapPlantExchangeToPlantExchangeDto()).ToList(),
            Rating = user.RatingsReceived?.Average(r => r.Rating),
            NumOfRatings = user.RatingsReceived?.Count()
        };
    }

    public static PlantedDto MapPlantedToPlantedDto(this Planted planted)
    {
        return new PlantedDto
        {
            Plant = planted.Plant?.MapPlantToPlantDto(),
            DatePlanted = planted.DatePlanted,
            Source = planted.Source,
            Notes = planted.Notes,
            IsOutside = planted.IsOutside,
            PlantStatus = planted.PlantStatus
        };
    }

    public static PlantExchangeDto MapPlantExchangeToPlantExchangeDto(this PlantExchange exchange)
    {
        return new PlantExchangeDto
        {
            Id = exchange.Id,
            Title = exchange.Title,
            ExchangeType = exchange.ExchangeType,
            Place = $"{exchange.City}, {exchange.Country?.Name}",
            Image = exchange.MainImage,
            Price = exchange.Price
        };
    }

   /* public static UserRatingDto MapUserRatingToUserRatingDto(this UserRating userRating)
    {
        return UserRatingDto{

        }
    }*/

    public static ReferenceDto MapReferenceToDto<T>(this T reference)
    {
        int id = EF.Property<int>(reference, "Id");
        string? name = null;

        if (typeof(T) == typeof(HardinessLevel))
        {
            name = EF.Property<string>(reference, "Level");
        }
        else
        {
            name = EF.Property<string>(reference, "Name");
        }

        return new ReferenceDto
        {
            Id = id,
            Name = name
        };
    }
}
