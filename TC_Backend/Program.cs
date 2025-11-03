using Finance.Net.Extensions;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TC_Backend.Data;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Adding Logger configuration from appsettings.json
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

//Register auto mapper
builder.Services.AddAutoMapper(typeof(Program).Assembly); 

// Configure DbContext
builder.Services.AddDbContext<TC_BackendDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Controllers with JSON options to ignore cycles
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.ReferenceHandler
            = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);
            
builder.Services.AddFinanceNet();

var app = builder.Build();

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
