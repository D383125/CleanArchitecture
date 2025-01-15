using Application.Mapping;
using Application.Modules;
using Infrastructure.AiProviders;
using Infrastructure.Configuration;
using Infrastructure.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Reflection;
using WebApp.MinimalApiEndpoints;
using WebApp.Startup;


var builder = WebApplication.CreateBuilder(args);

//1.  Add services to the container.
//ConfigureServices method is optional and defined inside startup class as mentioned in above code. It gets called by the host before the 'Configure' method to configure the app's services.
builder.Services.AddControllers();
builder.Services.AddWebServices();
builder.Services.AddAutoMapper(typeof(RequestMappingProfile));
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.RegisterDIAttributes([Assembly.GetAssembly(typeof(ChatAssistantModule)), Assembly.GetAssembly(typeof(ChatGdpClient))]);
//TODO: Use Source genertor for reflection
//builder.Services.AddGeneratedDIRegistrations();
builder.Services.AddSwaggerGen();

//2. Once services are added, configure the middle ware pipleine
//Configure method is used to add middleware components to the IApplicationBuilder instance that's available in Configure method.
//Configure method also specifies how the app responds to HTTP request and response. ApplicationBuilder instance's 'Use...' extension method is used to add one or more middleware components to request pipeline.
builder.Services.Configure<ApplicationOptions>(builder.Configuration.GetSection("Application"));
builder.Services.AddOptions<ApplicationOptions>()
    .Bind(builder.Configuration.GetSection("Application"))
    .ValidateDataAnnotations() // Validates [Required] and other annotations
    .Validate(options => !string.IsNullOrWhiteSpace(options.OpenAIKey), "OpenAIKey is required.");

builder.Services.AddStackExchangeRedisCache(options =>
    options.Configuration = builder.Configuration.GetConnectionString("Cache")
);
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = builder.Configuration.GetConnectionString("Cache");
    return ConnectionMultiplexer.Connect(configuration);
});

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactClient", policy =>
    {
        policy.WithOrigins(allowedOrigins!) // Allow specific host
             .SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost") // Dynamically allow any port for localhost
             .AllowAnyHeader()
             .AllowAnyMethod();
    });
});
//Validate Container config and scopes e.f. no scoped ssercies referenced as Singleton
builder.Host.UseDefaultServiceProvider(c => c.ValidateScopes = true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//The Request handling pipeline is a sequence of middleware components where each component performs the operation on request and either call the next middleware component
//or terminate the request. When a middleware component terminates the request, it's called Terminal Middleware as It prevents next middleware from processing the request.
//You can add a middleware component to the pipeline by calling .Use... extension method as below.
app.UseHttpsRedirection();
app.UseRouting();

// Apply the CORS middleware before the controllers

app.UseCors("AllowReactClient");

app.UseAuthorization();
app.MapControllers();
app.UseHealthChecks("/health");

//Map Minimal endpoints
app.MapAuthenticationEndpoints();
app.MapChatEndpoints();

//Terminate the request pipleine with app.run
app.Run();
