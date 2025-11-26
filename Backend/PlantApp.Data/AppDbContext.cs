using Microsoft.EntityFrameworkCore;
using PlantApp.Data.Models;
using PlantApp.Data.Models.Categories;

namespace PlantApp.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Plant> Plants { get; set; }
    public DbSet<Aspect> Aspects { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<Exposure> Exposures { get; set; }
    public DbSet<Fragrance> Fragrances { get; set; }
    public DbSet<GrowthLog> GrowthLogs { get; set; }
    public DbSet<Habit> Habits { get; set; }
    public DbSet<HardinessLevel> HardinessLevels { get; set; }
    public DbSet<HeightType> HeightTypes { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<Moisture> Moistures { get; set; }
    public DbSet<Ph> Phs { get; set; }
    public DbSet<Place> Places { get; set; }
    public DbSet<Planted> Planteds { get; set; }
    public DbSet<PlantFamily> PlantFamilies { get; set; }
    public DbSet<PlantStatus> PlantStatuses { get; set; }
    public DbSet<Reminder> Reminders { get; set; }
    public DbSet<ReminderType> ReminderTypes { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<SoilType> Soils { get; set; }
    public DbSet<SpreadType> Spreads { get; set; }
    public DbSet<Sunlight> Sunlights { get; set; }
    public DbSet<Season> Seasons { get; set; }
    public DbSet<TimeToFullHeight> TimeToFullHeight { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<PlantExchange> PlantExchanges { get; set; }
    public DbSet<ReasonOfDeath> ReasonsOfDeath { get; set; }
    public DbSet<ExchangeType> ExchangeTypes { get; set; }
    public DbSet<UserRating> UserRatings { get; set; }
    public DbSet<Frequency> Frequencies { get; set; }
    public DbSet<ReminderHistory> ReminderHistory { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasMany(u => u.RatingsGiven)
            .WithOne(r => r.Rater)
            .HasForeignKey(r => r.RaterId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasMany(u => u.RatingsReceived)
            .WithOne(r => r.Rated)
            .HasForeignKey(r => r.RatedId)
            .OnDelete(DeleteBehavior.Restrict);

        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.SetTableName(ToSnakeCase(entity.GetTableName()!));

            foreach (var property in entity.GetProperties())
            {
                // Set column names to snake_case
                property.SetColumnName(ToSnakeCase(property.GetColumnName()!));
            }
        }

        base.OnModelCreating(modelBuilder);
    }

    private static string ToSnakeCase(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        var builder = new System.Text.StringBuilder();
        builder.Append(char.ToLowerInvariant(input[0]));

        for (int i = 1; i < input.Length; ++i)
        {
            var c = input[i];
            if (char.IsUpper(c))
            {
                builder.Append('_');
                builder.Append(char.ToLowerInvariant(c));
            }
            else
            {
                builder.Append(c);
            }
        }
        return builder.ToString();
    }
}
