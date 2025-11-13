using CloudinaryDotNet;
using Finance.Net.Extensions;
using Serilog;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
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
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

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

// Add SignalR
builder.Services.AddSignalR();

//Register Finance.NET package
builder.Services.AddFinanceNet();

// Configure IOptions for Jwt Settings DI
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
        if (jwtSettings == null)
            throw new InvalidOperationException("JwtSettings configuration is missing");
            
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret))
        };
    });

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

// Register the price simulation engine
builder.Services.AddHostedService<PriceSimulationEngine>();

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("ng", pol => pol.WithOrigins("http://localhost:4200")
        .AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

// Add middleware for handling exceptions globally
app.UseMiddleware<ExceptHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("ng");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// Map SignalR hub
app.MapHub<StockPriceHub>("/stockPriceHub");

app.Run();