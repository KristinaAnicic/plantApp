using Microsoft.EntityFrameworkCore;
using PlantApp.Data;
using PlantApp.Domain.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<AppDbContext>();

    await dbContext.Database.MigrateAsync();

    var seeder = services.GetRequiredService<SeedDataService>();
    await seeder.SeedData();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
