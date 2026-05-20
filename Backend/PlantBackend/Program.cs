using Appwrite;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using PlantApp.Data;
using PlantApp.Data.Repositories;
using PlantApp.Domain;
using PlantApp.Domain.Interfaces;
using PlantApp.Domain.Interfaces.Data;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Models.Interfaces;
using PlantApp.Domain.Services;
using PlantApp.Domain.Services.Data;
using PlantApp.Domain.Utils;
using PlantApp.ML;
using PlantBackend.ExceptionHandlers;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.
var allowedOrigins = "AllowedOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(allowedOrigins, builder =>
    {
        builder
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .WithMethods("GET", "POST", "PUT", "DELETE", "PATCH")
            .AllowCredentials();
    });
});

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

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new EmptyStringToNullConverter());
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//appWrite
builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var endpoint = "https://fra.cloud.appwrite.io/v1";
    var projectId = "69711305002eed3e64c0";
    var apiKey = config["AppWrite:api_key"];

    return new Client()
        .SetEndpoint(endpoint)
        .SetProject(projectId)
        .SetKey(apiKey);
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
    );
    options.ConfigureWarnings(w =>
        w.Ignore(RelationalEventId.MultipleCollectionIncludeWarning));
});

builder.Services.AddExceptionHandler<ExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IPlantRepository, PlantRepository>();
builder.Services.AddScoped<IPlantedRepository, PlantedRepository>();
builder.Services.AddScoped<IReminderRepository, ReminderRepository>();
builder.Services.AddScoped<IGrowthLogRepository, GrowthLogRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPlantExchangeRepository, PlantExchangeRepository>();
builder.Services.AddScoped<IPlantGroupRepository, PlantGroupRepository>();
builder.Services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();
builder.Services.AddScoped<IMLRepository, MLRepository>();

builder.Services.AddScoped<IPlantService, PlantService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPlantedService, PlantedService>();
builder.Services.AddScoped<IPlantPlaceService, PlantPlaceService>();
builder.Services.AddScoped<IReminderService, ReminderService>();
builder.Services.AddScoped<IGrowthLogService, GrowthLogService>();
builder.Services.AddScoped<IPlantExchangeService, PlantExchangeService>();
builder.Services.AddScoped<IUserRatingService, UserRatingService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IPlantGroupService, PlantGroupService>();
builder.Services.AddScoped<IPlantNetService, PlantNetService>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMLHealthPredictionService, MLHealthPredictionService>();
builder.Services.AddScoped<IMLRecommendationService, MLRecommendationService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<ICurrentUserContext, CurrentUserContex>();
builder.Services.AddScoped<SeedCsvDataService>();
builder.Services.AddScoped<SeedTemporaryDataService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("My API")
            .WithOpenApiRoutePattern("/swagger/v1/swagger.json");
    });
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

    //await seeder.SeedPlantAttributeData();
}

app.UseCors(allowedOrigins);
app.UseHttpsRedirection();

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
