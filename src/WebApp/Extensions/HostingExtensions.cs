using Application.Mapping;
using Application.Modules;
using Infrastructure.AiProviders;
using Infrastructure.Configuration;
using Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.HttpOverrides;
using StackExchange.Redis;
using System.Reflection;
using WebApp.MinimalApiEndpoints;
using WebApp.Startup;

namespace WebApp.Extensions
{
    public static class HostingExtensions
    {
        public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
        {
            //1.  Add services to the container.
            //ConfigureServices method is optional and defined inside startup class as mentioned in above code. It gets called by the host before the 'Configure' method to configure the app's services.
            builder.Services.AddControllers();
            builder.Services.AddWebServices();
            //Dissabllow Https so docker building doesnt seek out a ssl cert. Https is maanged by a upstream reverse proxy
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(8080); // Listen on port 8080 for HTTP
            });
            builder.Services.AddAutoMapper(typeof(RequestMappingProfile));
            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.RegisterDIAttributes([Assembly.GetAssembly(typeof(ChatAssistantModule)), Assembly.GetAssembly(typeof(ChatGdpClient))]);
            //TODO: Use Source genertor over reflection
            //builder.Services.AddGeneratedDIRegistrations();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHealthChecks(); //TODO: Impleiment

            //2. Once services are added, configure the middleware pipleine
            //Configure method specifies how the app responds to HTTP request and response. ApplicationBuilder instance's 'Use...' extension method is used to add one or more middleware components to request pipeline.
            builder.Services.Configure<ApplicationOptions>(builder.Configuration.GetSection("Application"));
            //Options Pattern..
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
            //Validate Container config and scopes e.g. no scoped servies referenced as Singleton
            builder.Host.UseDefaultServiceProvider(c => c.ValidateScopes = true);
#if OpenIdConnect
            //Auth method has yet to be deretmeined. Ie Asp.Net Core Identity Service or a 3rd party OpenId Connect SSO one 
            //Confiure Duenabe SSO pipeline
            builder.Services.AddIdentityServer()
                .AddInMemoryIdentityResources(IdentityServerConfiguration.IdentityResources)
                .AddInMemoryApiScopes(IdentityServerConfiguration.Scopes)
                .AddInMemoryClients(IdentityServerConfiguration.Clients);

            //And the token mechism - this will be valid for non OpenId as an bearer auth token anyway
            builder.Services.AddAuthentication()
                .AddJwtBearer(options =>
            {
                options.Authority = "http://localhost:8080"; // read from config
                options.TokenValidationParameters.ValidateAudience = false;
            });
#endif
            return builder.Build();
        }

        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            //The Request handling pipeline is a sequence of middleware components where each component performs the operation on request and either call the next middleware component
            //or terminate the request. When a middleware component terminates the request, it's called Terminal Middleware as It prevents next middleware from processing the request.
            //You can add a middleware component to the pipeline by calling .Use... extension method as below.

            //Nginx is rediceting to here so we can disable UseHttpsRedirection
            //app.UseHttpsRedirection();
            //Configure Kestrel to listen only on HTTP
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseRouting();

            // Apply the CORS middleware before the controllers
            app.UseCors("AllowReactClient");
            app.UseIdentityServer();
            app.UseAuthorization();
            app.MapControllers();
            app.UseHealthChecks("/health");

            //Map Minimal endpoints
            app.MapAuthenticationEndpoints();
            app.MapChatEndpoints();

            System.Diagnostics.Trace.TraceInformation($"Running in '{app.Environment.EnvironmentName}' Environment.");

            return app;
        }
    }
}
