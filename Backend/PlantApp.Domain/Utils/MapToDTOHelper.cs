using Microsoft.EntityFrameworkCore;
using PlantApp.Data.Models;
using PlantApp.Data.Models.Interfaces;
using PlantApp.Domain.Dtos;
using PlantApp.Domain.Dtos.GrowthLog;
using PlantApp.Domain.Dtos.Plant;
using PlantApp.Domain.Dtos.Planted;
using PlantApp.Domain.Dtos.PlantExchange;
using PlantApp.Domain.Dtos.PlantPlace;
using PlantApp.Domain.Dtos.Reminder;
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
            Image = plant.Images?.FirstOrDefault()?.Url ?? null,
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
            Image = plant.Images?.FirstOrDefault()?.Url ?? null,
            Fragrance = plant.Fragrance?.Name ?? "Not specified",
            HardinessLevel = plant.HardinessLevel != null ? $"{plant.HardinessLevel.Level} ({plant.HardinessLevel.Description})" : "Not specified",
            IsSpecie = plant.IsSpecie,
            IsGenus = plant.IsGenus,
            IsPlantForPollinators = plant.IsPlantForPollinators,
            IsLowMaintenance = plant.IsLowMaintenance,
            IsDroughtResistant = plant.IsDroughtResistant,
            SpreadType = plant.SpreadType?.Name ?? "Not specified",
            HeightType = plant.HeightType?.Name ?? "Not specified",
            TimeToFullHeight = plant.TimeToFullHeight?.Name ?? "Not specified",
            Toxicity = plant.Toxicity,
            Cultivation = plant.Cultivation,
            PestResistance = plant.PestResistance,
            DiseaseResistance = plant.DiseaseResistance,
            Pruning = plant.Pruning,
            Propagation = plant.Propagation,
            Family = plant.Family?.Name ?? "Not specified",
            GenusDescription = plant.GenusDescription,
            SoilTypes = string.Join(", ", plant.SoilTypes.Select(s => s.Name)) ?? "Not specified",
            Images = plant.Images?.Select(s => s.MapImageToImageDto()).ToList(),
            Sunlights = string.Join(", ", plant.Sunlights.Select(s =>s.Name)) ?? "Not specified",
            Aspects = string.Join(", ", plant.Aspects.Select(a =>a.Name)) ?? "Not specified",
            Moistures = string.Join(", ", plant.Moistures.Select(m =>m.Name)) ?? "Not specified",
            Phs = string.Join(", ", plant.Phs.Select(p =>p.Name)) ?? "Not specified",
            Exposures = string.Join(", ", plant.Exposures.Select(e =>e.Name)) ?? "Not specified",
            Habits = plant.Habits.Select(e =>e.Name).ToList(),
            Seasons = plant.Seasons.Select(e =>e.Name).ToList(),
            Synonyms = plant.Synonyms
                            .Select( p => new ReferenceDto { Id = p.Id, Name = p.BotanicalName })
                            .ToList(),
            ParentPlant = plant.SynonymParentPlant != null ? 
                          new ReferenceDto { 
                              Id = plant.SynonymParentPlant.Id, 
                              Name = plant.SynonymParentPlant.BotanicalName } :
                          null
        };
    }

    public static ImageDto MapImageToImageDto(this PlantApp.Data.Models.Image img)
    {
        return new ImageDto
        {
            Id = img.Id,
            Url = img.Url,
            Copyright = img.Copyright,
        };
    }
    public static PlaceDto MapPlaceToPlaceDto(this Place place)
    {
        var placeName = place.Country != null ?
            $"{place.City}, {place.Country?.Name}" :
            place.City;

        return new PlaceDto
        {
            Id = place.Id,
            Name = place.Name,
            Address = $"{place.Address} ({placeName})",
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
            Username = user.Username,
            DisplayName = user.DisplayName,
            Role = user.Role?.Name ?? "Unknown",
            RoleId = user.RoleId,
            Gender = user.Gender,
            DateOfBirth = user.DateOfBirth,
            Places = user.Places?.Select(p => p.MapPlaceToPlaceGetDto()).ToList(),
            PlantExchanges = user.PlantExchanges?.Select(pe => pe.MapPlantExchangeToPlantExchangeDto()).ToList(),
            Rating = user.RatingsReceived != null && user.RatingsReceived.Any()
                        ? user.RatingsReceived.Average(r => r.Rating)
                        : 0,
            NumOfRatings = user.RatingsReceived?.Count()
        };
    }

    public static UserDto MapUserToUserDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.Username,
            DisplayName = user.DisplayName,
            Role = user.Role?.Name ?? "Unknown",
            Gender = user.Gender,
            DateOfBirth = user.DateOfBirth,
            Rating = user.RatingsReceived != null && user.RatingsReceived.Any()
                        ? user.RatingsReceived.Average(r => r.Rating)
                        : 0,
            NumOfRatings = user.RatingsReceived?.Count(),
            RoleId = user.RoleId
        };
    }

    public static PlantedDto MapPlantedToPlantedDto(this Planted planted)
    {
        return new PlantedDto
        {
            Id = planted.Id,
            Place = planted.Place != null
                ? $"{planted.Place.Name} ({planted.Place.Address}, {planted.Place.City})"
                : "Not specified",
            PlantName = planted.Plant != null
                ? $"{planted.Plant.BotanicalName} ({planted.Plant.CommonName})"
                : "Not specified",
            PlantStatus = planted.PlantStatus?.Name ?? "Not specified",
            DatePlanted = planted.DatePlanted,
            Image = planted.Image ?? planted.Images?.FirstOrDefault()?.Url,
            Name = planted.Name
        };
    }

    public static PlantedGetDto MapPlantedToPlantedGetDto(this Planted planted)
    {
        return new PlantedGetDto
        {
            Id = planted.Id,
            Plant = planted.Plant?.MapPlantToPlantDto(),
            Place = planted.Place?.MapPlaceToPlaceDto(),
            DatePlanted = planted.DatePlanted,
            Source = planted.Source,
            Note = planted.Note,
            IsOutside = planted.IsOutside,
            PlantStatus = planted.PlantStatus?.Name ?? "Not specified",
            NextReminders = planted.Reminders?.Select(r => r.MapReminderToReminderDto()).ToList(),
            GrowthLogs = planted.GrowthLogs?.Select(gl => gl.MapGrowthLogToGrowthLogDto()).ToList(),
            Images = planted.Images?.Select(im => im.MapImageToImageDto()).ToList(),
            Name = planted.Name,
        };
    }

    /*public static GroupedPlantedDto MapPlantedToGroupedPlantedDto(this Dictionary<Place, List<Planted>> planted)
    {
        return new GroupedPlantedDto
        {
            Place = planted.K
        };
    }*/

    public static ReminderDto MapReminderToReminderDto(this Reminder reminder)
    {
        return new ReminderDto
        {
            Id = reminder.Id,
            Plant = reminder.Planted.Name,
            PlantedId = reminder.PlantedId,
            ReminderType = reminder.ReminderType?.Name,
            NextDueDate = reminder.NextDueDate.AddDays(reminder.DelayDays),
            Notes = reminder.Note,
            IsLate = (reminder.NextDueDate - DateTime.UtcNow).TotalDays < 0
        };
    }

    public static ReminderGetDto MapReminderToReminderGetDto(this Reminder reminder)
    {
        return new ReminderGetDto
        {
            Id = reminder.Id,
            PlantedId = reminder.PlantedId,
            ReminderType = reminder.ReminderType.MapReferenceToDto(),
            NextDueDate = reminder.NextDueDate.AddDays(reminder.DelayDays),
            DaysDelayed = reminder.DelayDays,
            Notes = reminder.Note,
            PlantedName = reminder.Planted.Name ?? $"{reminder.Planted.Plant?.BotanicalName} ({reminder.Planted.Plant?.CommonName})",
            //Frequency = $"every {reminder.FrequencyNum} {reminder.FrequencyType.Name}"
            FrequencyType = reminder.FrequencyType.MapReferenceToDto(),
            FrequencyNum = reminder.FrequencyNum,
            IsLate = (reminder.NextDueDate - DateTime.UtcNow).TotalDays < 0
        };
    }

    public static GrowthLogDto MapGrowthLogToGrowthLogDto(this GrowthLog log)
    {
        return new GrowthLogDto
        {
            Id = log.Id,
            Note = log.Note,
            PlantStatus = log.PlantStatus?.Name,
            CreatedAt = log.CreatedAt,
            Images = log.Images?.Select(im => im.MapImageToImageDto()).ToList(),
            PlantedId = log.PlantedId,
            Plant = log.Planted != null ? log.Planted.Name : null
        };
    }

    public static GrowthLogGetDto MapGrowthLogToGrowthLogGetDto(this GrowthLog log)
    {
        return new GrowthLogGetDto
        {
            Id = log.Id,
            Note = log.Note,
            PlantStatus = log.PlantStatus != null ? log.PlantStatus.MapReferenceToDto() : null,
            CreatedAt = log.CreatedAt,
            Images = log.Images?.Select(im => im.MapImageToImageDto()).ToList(),
            PlantedId = log.PlantedId,
            Plant = log.Planted != null ? log.Planted.Name : null,
        };
    }

    public static PlantExchangeDto MapPlantExchangeToPlantExchangeDto(this PlantExchange exchange)
    {
        return new PlantExchangeDto
        {
            Id = exchange.Id,
            Title = exchange.Title,
            ExchangeType = exchange.ExchangeType != null ? exchange.ExchangeType.MapReferenceToDto() : null,
            Place = $"{exchange.City}, {exchange.Country?.Name}",
            Image = exchange.MainImage,
            Price = exchange.Price,
            CreatedAt = exchange.CreatedAt
        };
    }

    public static PlantExchangeGetDto MapPlantExchangeToPlantExchangeGetDto(this PlantExchange exchange)
    {
        return new PlantExchangeGetDto
        {
            Id = exchange.Id,
            Title = exchange.Title,
            ExchangeType = exchange.ExchangeType != null ? exchange.ExchangeType.MapReferenceToDto() : null,
            Place = $"{exchange.City}, {exchange.Country?.Name}",
            Image = exchange.MainImage,
            Price = exchange.Price,
            CreatedAt = exchange.CreatedAt,
            User = new ReferenceDto { Id = exchange.UserId, Name = exchange.User != null ? exchange.User.Username : "Not specified" },
            Planted = exchange.Planted != null ? exchange.Planted.MapPlantedToPlantedDto() : null,
            Content = exchange.Content,
            PlantStatus = exchange.PlantStatus,
            ExchangeFor = exchange.ExchangeFor,
            Shipping = exchange.Shipping,
            Images = exchange.Images.Select(img => img.MapImageToImageDto()).ToList(),
            UserRating = exchange.User.RatingsReceived != null && exchange.User.RatingsReceived.Any()
                        ? exchange.User.RatingsReceived.Average(r => r.Rating)
                        : 0,
        };
    }


     public static UserRatingDto MapUserRatingToUserRatingDto(this UserRating userRating)
     {
        return new UserRatingDto
        {
            Rater = new ReferenceDto { Id = userRating.RaterId, Name = userRating.Rater != null ? userRating.Rater.Username : "Unknown" },
            Rated = new ReferenceDto { Id = userRating.RatedId, Name = userRating.Rated != null? userRating.Rated.Username : "Unknown" },
            Rating = userRating.Rating,
            Comment = userRating.Comment,
            CreatedAt = userRating.CreatedAt,
            UpdatedAt = userRating.UpdatedAt
        };
     }

    public static ReferenceDto MapReferenceToDto(this IReferenceEntity reference)
    {
        return new ReferenceDto
        {
            Id = reference.Id,
            Name = reference.Name
        };
    }
}
