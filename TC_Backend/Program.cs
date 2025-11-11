using CloudinaryDotNet;
using Finance.Net.Extensions;
using Serilog;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using TC_Backend.BackgroundServices;
using TC_Backend.Interfaces;
using TC_Backend.Repositories;
using TC_Backend.Models;
using TC_Backend.Data;
using TC_Backend.Services;
using TC_Backend.Services.Interfaces;
using TC_Backend.BackgroundServices.Helpers;
using TC_Backend.Helpers;
using TC_Backend.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Adding Logger configuration from appsettings.json
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Register Cloudinary settings
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));
builder.Services.AddSingleton<Cloudinary>(serviceProvider =>
{
    var cloudinaryConfig = serviceProvider.GetRequiredService<IOptions<CloudinarySettings>>().Value;
    return new Cloudinary(new Account(
        cloudinaryConfig.CloudName,
        cloudinaryConfig.ApiKey,
        cloudinaryConfig.ApiSecret));
});

// Configure DbContext
builder.Services.AddDbContext<TC_BackendDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DB")));

// Configure Controllers with JSON options to ignore cycles
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.ReferenceHandler
            = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);

//Register Finance.NET package
builder.Services.AddFinanceNet();

// Configure IOptions for Jwt Settings DI
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

//Register Cloudinary Service 
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

// Configure Identity (UserManager/SignInManager) with EF stores
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<TC_BackendDbContext>()
    .AddDefaultTokenProviders();
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredLength = 6;
});

// Register repositories and services
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<ICompanyListRepo, CompanyListRepo>();
builder.Services.AddScoped<ICompanyListService, CompanyListService>();

builder.Services.AddScoped<IRoleRepo, RoleRepo>();
builder.Services.AddScoped<IRoleService, RoleService>();

//Service for Background Services
builder.Services.AddSingleton<UserMapping>();

//Helper classes for background services
builder.Services.AddSingleton<ExternalToDtoMapper>();
builder.Services.AddSingleton<DtoToModelsMapper>();

// Register the background service
// builder.Services.AddHostedService<FinancialDataUpdService>();

//Exposing a method in background services so that it can u used in controller
builder.Services.AddSingleton<FinancialDataUpdService>();

var app = builder.Build();

// Add middleware for handling exceptions globally
app.UseMiddleware<ExceptHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();