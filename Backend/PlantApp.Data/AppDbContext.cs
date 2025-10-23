using Microsoft.EntityFrameworkCore;
using PlantApp.Data.Models;

namespace PlantApp.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Plant> Plants { get; set; }
    public DbSet<Aspect> Aspects { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<Exposure> Exposures { get; set; }
    public DbSet<Fragnance> Fragnances { get; set; }
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
    public DbSet<ReminderType> RemindersType { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<SoilType> Soils { get; set; }
    public DbSet<SpreadType> Spreads { get; set; }
    public DbSet<Sunlight> Sunlights { get; set; }
    public DbSet<TimeToFullHeight> TimeToFullHeight { get; set; }
    public DbSet<User> Users { get; set; }

}
