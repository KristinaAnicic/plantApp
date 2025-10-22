using Microsoft.EntityFrameworkCore;

namespace PlantApp.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{

}
