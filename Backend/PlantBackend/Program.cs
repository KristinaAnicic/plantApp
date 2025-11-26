using Microsoft.EntityFrameworkCore;
using PlantApp.Data;
using PlantApp.Domain.Interfaces.Data;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Repositories;
using PlantApp.Domain.Services;
using PlantApp.Domain.Services.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IPlantRepository, PlantRepository>();
builder.Services.AddScoped<IPlantedRepository, PlantedRepository>();
builder.Services.AddScoped<IReminderRepository, ReminderRepository>();
builder.Services.AddScoped<IGrowthLogRepository, GrowthLogRepository>();

builder.Services.AddScoped<IPlantService, PlantService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPlantedService, PlantedService>();
builder.Services.AddScoped<IPlantPlaceService, PlantPlaceService>();
builder.Services.AddScoped<IReminderService, ReminderService>();
builder.Services.AddScoped<IGrowthLogService, GrowthLogService>();

builder.Services.AddScoped<SeedDataService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//await PlantDataFetcher.FetchAllDataAsync();
//await PlantDataFetcher.CheckIds(45);

/*using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var seeder = services.GetRequiredService<SeedDataService>();
    await seeder.SeedData();
}*/

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
