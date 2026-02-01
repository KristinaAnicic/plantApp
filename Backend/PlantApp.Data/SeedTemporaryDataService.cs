using Microsoft.EntityFrameworkCore;
using PlantApp.Data;
using PlantApp.Domain.Models;

namespace PlantApp.Domain.Services;

public class SeedTemporaryDataService(AppDbContext context)
{
    public async Task SeedAllData()
    {
        var users = await context.Users.ToListAsync();
        if (!users.Any())
        {
            await SeedUsers();
            users = await context.Users.ToListAsync();
        }
        var places = await context.Places.ToListAsync();
        if (!places.Any())
        {
            await SeedPlaces(users.Where(u => u.RoleId == 3).ToList());
            places = await context.Places.ToListAsync();
        }
        
        var planted = await context.Planteds.ToListAsync();
        if (!planted.Any())
        {
            await SeedPlanted(places);
            planted = await context.Planteds.ToListAsync();
        }

        var logs = await context.GrowthLogs.ToListAsync();
        if (!logs.Any()) {
            await SeedGrowthLogs(planted);
        }
    }
    public async Task SeedUsers()
    {
        var users = new List<User>
        {
            new User
            {
                Email = "admin@email.com",
                Password = "admin123",
                Username = "admin",
                DisplayName = "Admin",
                RoleId = 1,
                Gender = 'M',
                DateOfBirth = new DateOnly(1990,1,1)
            },
            new User
            {
                Email = "mod@email.com",
                Password = "mod123",
                Username = "moderator",
                DisplayName = "Moderator",
                RoleId = 2,
                Gender = 'F',
                DateOfBirth = new DateOnly(1992,5,10)
            },
            new User
            {
                Email = "user1@email.com",
                Password = "user123",
                Username = "user1",
                DisplayName = "User One",
                RoleId = 3,
                Gender = 'F',
                DateOfBirth = new DateOnly(1995,4,12)
            },
            new User
            {
                Email = "user2@email.com",
                Password = "user123",
                Username = "user2",
                DisplayName = "User Two",
                RoleId = 3,
                Gender = 'M',
                DateOfBirth = new DateOnly(1998,8,20)
            }
        };

        context.Users.AddRange(users);
        await context.SaveChangesAsync();
        //await SeedPlaces(users.Where(u => u.RoleId == 3).ToList());
    }

    private async Task SeedPlaces(List<User> users)
    {
        var places = new List<Place>();

        foreach (var user in users)
        {
            places.Add(new Place
            {
                Name = "Living room",
                City = "Zagreb",
                Address = "Street 1",
                UserId = user.Id,
                CountryId = 1
            });

            places.Add(new Place
            {
                Name = "Balcony",
                City = "Zagreb",
                Address = "Street 1",
                UserId = user.Id,
                CountryId = 1
            });
        }

        context.Places.AddRange(places);
        await context.SaveChangesAsync();

        //await SeedPlanted(places);
    }

    private async Task SeedPlanted(List<Place> places)
    {
        var plantedList = new List<Planted>();
        var random = new Random();

        foreach (var place in places)
        {
            var count = random.Next(2, 4);

            for (int i = 0; i < count; i++)
            {
                plantedList.Add(new Planted
                {
                    PlaceId = place.Id,
                    PlantId = random.Next(1, 60000),
                    Name = $"Plant {i + 1}",
                    DatePlanted = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-random.Next(10, 200))),
                    PlantStatusId = random.Next(1, 12),
                    IsOutside = false
                });
            }
        }

        context.Planteds.AddRange(plantedList);
        await context.SaveChangesAsync();

        await SeedReminders(plantedList);
        await SeedImages(plantedList);
    }

    private async Task SeedReminders(List<Planted> plantedList)
    {
        var reminders = new List<Reminder>();

        foreach (var planted in plantedList)
        {
            reminders.Add(new Reminder
            {
                PlantedId = planted.Id,
                ReminderTypeId = 1,
                FrequencyTypeId = 1,
                FrequencyNum = 7,
                NextDueDate = DateTime.UtcNow.AddDays(3)
            });

            reminders.Add(new Reminder
            {
                PlantedId = planted.Id,
                ReminderTypeId = 2,
                FrequencyTypeId = 1,
                FrequencyNum = 30,
                NextDueDate = DateTime.UtcNow.AddDays(10)
            });
        }

        context.Reminders.AddRange(reminders);
        await context.SaveChangesAsync();
    }

    private async Task SeedImages(List<Planted> plantedList)
    {
        var images = new List<Image>();

        foreach (var planted in plantedList)
        {
            var img = new Image
            {
                Url = $"https://picsum.photos/seed/{Guid.NewGuid()}/400",
                UserId = planted.Place.UserId
            };

            images.Add(img);
            planted.Images.Add(img);
        }

        context.Images.AddRange(images);
        await context.SaveChangesAsync();
    }

    private async Task SeedGrowthLogs(List<Planted> plantedList)
    {
        var random = new Random();
        var growthLogs = new List<GrowthLog>();

        foreach (var planted in plantedList)
        {
            var logCount = random.Next(1, 4); // 1–3 loga po biljci

            for (int i = 0; i < logCount; i++)
            {
                growthLogs.Add(new GrowthLog
                {
                    Title = $"Log update for {i + 1}",
                    PlantedId = planted.Id,
                    Note = $"Growth update {i + 1} for {planted.Name}",
                    PlantStatusId = random.Next(1, 12),
                    CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 90))
                });
            }
        }

        context.GrowthLogs.AddRange(growthLogs);
        await context.SaveChangesAsync();

        await SeedGrowthLogImages(growthLogs);
    }
    private async Task SeedGrowthLogImages(List<GrowthLog> growthLogs)
    {
        var random = new Random();

        var plantedDict = await context.Planteds.Include(p => p.Place)
                        .ToDictionaryAsync(p => p.Id);

        foreach (var log in growthLogs)
        {
            var imageCount = random.Next(0, 3); // 0–2 slike po logu
            if (imageCount == 0)
                continue;

            var planted = plantedDict[log.PlantedId];

            for (int i = 0; i < imageCount; i++)
            {
                var image = new Image
                {
                    Url = $"https://picsum.photos/seed/log-{Guid.NewGuid()}/400",
                    UserId = planted.Place.UserId
                };

                log.Images.Add(image);
                context.Images.Add(image);
            }
        }

        await context.SaveChangesAsync();
    }




}
