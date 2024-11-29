using Application.Modules;
using Domain;
using Domain.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using WebApp.MinimalApiEndpoints;
using WebApp.Startup;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddWebServices();
builder.Services.RegisterExternalApis(builder.Configuration);
builder.Services.RegisterDIAttributes(Assembly.GetAssembly(typeof(ChatAssistantModule)));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("Database"))
    );

builder.Services.AddStackExchangeRedisCache(options =>
    options.Configuration = builder.Configuration.GetConnectionString("Cache")
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigrations();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseHealthChecks("/health");

//Map Minimal endpoints
app.MapAuthenticationEndpoints();
app.MapProductEndpoints();
app.MapChatEndpoints();

app.Run();
