using Microsoft.EntityFrameworkCore;
using PlantApp.Domain.Dtos.ML;
using PlantApp.Domain.Models;
using PlantApp.Domain.Models.Categories;

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
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<PlantGroup> PlantGroups { get; set; }
    public DbSet<PlaceHistory> PlaceHistory { get; set; }
    public DbSet<PlantAttributeType> PlantAttributeTypes { get; set; }
    public DbSet<PlantSeasonAttribute> PlantSeasonAttributes { get; set; }
    public DbSet<PlantedGrowthLogOverviewDto> PlantedGrowthLogOverview => Set<PlantedGrowthLogOverviewDto>();


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

        modelBuilder.Entity<Plant>()
            .HasMany(p => p.Synonyms)
            .WithOne(p => p.SynonymParentPlant)
            .HasForeignKey(p => p.SynonymParentPlantId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Place>(entity =>
        {
            entity.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Place_SunlightIntensity_Range", "sunlight_intensity BETWEEN 1 AND 5");
                t.HasCheckConstraint("CK_Place_HumidityIntensity_Range", "humidity_intensity BETWEEN 1 AND 5");
            });
        });

        modelBuilder.Entity<UserRating>(entity =>
        {
            entity.ToTable(t => t.HasCheckConstraint("CK_UserRating_Comment_Length", "char_length(\"comment\") BETWEEN 10 AND 500"));
        });


        modelBuilder.Entity<Plant>()
            .Property<uint>("xmin")
            .IsRowVersion()          
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        modelBuilder.Entity<PlantedGrowthLogOverviewDto>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("vw_planted_growth_overview");

            entity.Property(v => v.PlantedId).HasColumnName("planted_id");
            entity.Property(v => v.SunlightIntensity).HasColumnName("sunlight_intensity");
            entity.Property(v => v.HumidityIntensity).HasColumnName("humidity_intensity");
            entity.Property(v => v.IsOutside).HasColumnName("is_outside");
            entity.Property(v => v.Family).HasColumnName("family");
            entity.Property(v => v.Hardiness).HasColumnName("hardiness");
            entity.Property(v => v.PlantStatusId).HasColumnName("plant_status_id");
            entity.Property(v => v.SunlightList).HasColumnName("sunlight_list");
            entity.Property(v => v.MoistureList).HasColumnName("moisture_list");
            entity.Property(v => v.Seasons).HasColumnName("seasons");
            entity.Property(v => v.LowMaintenance).HasColumnName("low_maintenance");
            entity.Property(v => v.DroughtResistant).HasColumnName("drought_resistant");
            entity.Property(v => v.Month).HasColumnName("month");
            entity.Property(v => v.DaysSincePlanted).HasColumnName("days_since_planted");
            entity.Property(v => v.ReminderDelay).HasColumnName("reminder_delay");
        });

        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            if (entity.ClrType == typeof(PlantedGrowthLogOverviewDto))
                continue;

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
