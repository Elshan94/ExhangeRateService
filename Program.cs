using AspNetCoreRateLimit.Redis;
using AspNetCoreRateLimit;
using BambooExchangeRateService.Application.Models;
using BambooExchangeRateService.Helpers;
using BambooExchangeRateService.Persistence;
using BambooExhangeRateService.Application.Models;
using BambooExhangeRateService.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Polly;
using Polly.CircuitBreaker;
using Serilog;
using StackExchange.Redis;
using System.Text;
using BambooExhangeRateService.Application.External.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var jwtConfig = new JwtConfig();
builder.Configuration.Bind("JwtConfig", jwtConfig);

var dataBaseConfig = new DatabaseConfig();
builder.Configuration.Bind("DatabaseConfig", dataBaseConfig);

var redisConfig = new RedisConfig();
builder.Configuration.Bind("RedisConfig", redisConfig);

var frankfurterExchangeRateConfig = new FrankfurterExchangeRateConfig();
builder.Configuration.Bind("FrankfurterExchangeRateConfig", frankfurterExchangeRateConfig);

var forbiddenCurrencies = new ForbiddenCurrencies();
builder.Configuration.Bind("ForbiddenCurrencies", forbiddenCurrencies);

builder.Services.AddSingleton(dataBaseConfig);
builder.Services.AddSingleton(frankfurterExchangeRateConfig);
builder.Services.AddSingleton(forbiddenCurrencies);
builder.Services.AddSingleton(redisConfig);
builder.Services.AddSingleton(jwtConfig);



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwtConfig = builder.Configuration.GetSection("JwtConfig").Get<JwtConfig>();
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfig.Issuer,
                    ValidAudience = jwtConfig.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key))
                };
            });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
});

builder.Services.AddDbContext<ExchangeRateDbContext>(options =>
            options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=ExchangeRate;Trusted_Connection=True;"));

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConfig.BaseAddress;
});

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    return ConnectionMultiplexer.Connect(redisConfig.BaseAddress);
});



builder.Services.AddRedisRateLimiting();

builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));

builder.Services.AddFrankfurterHttpClient(frankfurterExchangeRateConfig.BaseAddress);

//Log.Logger = new LoggerConfiguration()
//    .WriteTo.Console()
//    .WriteTo.Seq("ElkUrl")  
//    .CreateLogger();

// Elk url needed

builder.Host.UseSerilog();

builder.Services.AddServices();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<LoggingMiddleware>();

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
