using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PlantApp.Data;
using PlantApp.Domain;
using PlantApp.Domain.Interfaces;
using PlantApp.Domain.Interfaces.Data;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Repositories;
using PlantApp.Domain.Services;
using PlantApp.Domain.Services.Data;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = config["Jwt:Issuer"],
        ValidAudience = config["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IPlantRepository, PlantRepository>();
builder.Services.AddScoped<IPlantedRepository, PlantedRepository>();
builder.Services.AddScoped<IReminderRepository, ReminderRepository>();
builder.Services.AddScoped<IGrowthLogRepository, GrowthLogRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IPlantService, PlantService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPlantedService, PlantedService>();
builder.Services.AddScoped<IPlantPlaceService, PlantPlaceService>();
builder.Services.AddScoped<IReminderService, ReminderService>();
builder.Services.AddScoped<IGrowthLogService, GrowthLogService>();
builder.Services.AddScoped<IPlantExchangeService, PlantExchangeService>();
builder.Services.AddScoped<IUserRatingService, UserRatingService>();
builder.Services.AddScoped<IImageService, ImageService>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICurrentUserContext, CurrentUserContex>();
builder.Services.AddScoped<SeedCsvDataService>();
builder.Services.AddScoped<SeedTemporaryDataService>();

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
    var seeder = services.GetRequiredService<SeedCsvDataService>();
    await seeder.SeedData();

    var seedTemp = services.GetRequiredService<SeedTemporaryDataService>();
    await seedTemp.SeedAllData();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
