using Application.Modules;
using Domain.Extensions;
using Infrastructure;
using Infrastructure.AiProviders;
using Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using WebApp.MinimalApiEndpoints;
using WebApp.Startup;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddWebServices();

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.RegisterDIAttributes([Assembly.GetAssembly(typeof(ChatAssistantModule)), Assembly.GetAssembly(typeof(ChatGdpClient))]);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddSwaggerGen();


builder.Services.Configure<ApplicationOptions>(builder.Configuration.GetSection("Application"));
builder.Services.AddOptions<ApplicationOptions>()
    .Bind(builder.Configuration.GetSection("Application"))
    .ValidateDataAnnotations() // Validates [Required] and other annotations
    .Validate(options => !string.IsNullOrWhiteSpace(options.OpenAIKey), "OpenAIKey is required.");


builder.Services.AddStackExchangeRedisCache(options =>
    options.Configuration = builder.Configuration.GetConnectionString("Cache")
);


//Move to Confiruare Services extension
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactClient", policy =>
    {
        policy.WithOrigins(allowedOrigins!) // Trusted domain TrustedDomains
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigrations();
}

app.UseHttpsRedirection();
app.UseRouting();

// Apply the CORS middleware before the controllers

app.UseCors("AllowReactClient");

app.UseAuthorization();
app.MapControllers();
app.UseHealthChecks("/health");

//Map Minimal endpoints
app.MapAuthenticationEndpoints();
app.MapProductEndpoints();
app.MapChatEndpoints();

app.Run();
