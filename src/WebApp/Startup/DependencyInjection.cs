using Domain;
using Domain.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace WebApp.Startup
{
    internal static class DependencyInjection
    {
        public static IServiceCollection AddWebServices(this IServiceCollection services)
        {
            //services.AddDatabaseDeveloperPageExceptionFilter();
            //services.AddScoped<IUser, CurrentUser>();

            services.AddHttpContextAccessor();

            services.AddHealthChecks();
                //.AddDbContextCheck<ApplicationDbContext>();

            //services.AddExceptionHandler<CustomExceptionHandler>();
            

            // Customise default API behaviour
            services.Configure<ApiBehaviorOptions>(options =>
                options.SuppressModelStateInvalidFilter = true);

            services.AddEndpointsApiExplorer();

//            services.AddOpenApiDocument((configure, sp) =>
//            {
//                configure.Title = "CleanArchitecture API";

//#if (UseApiOnly)
//            // Add JWT
//            configure.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
//            {
//                Type = OpenApiSecuritySchemeType.ApiKey,
//                Name = "Authorization",
//                In = OpenApiSecurityApiKeyLocation.Header,
//                Description = "Type into the textbox: Bearer {your JWT token}."
//            });

//            configure.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
//#endif
//            });

            return services;
        }

        public static IServiceCollection RegisterDIAttributes(this IServiceCollection services, Assembly[] assembly)
        {
            //Do seource generators need to be in a seperate project? Dont think os
            //services.AddGeneratedDIRegistrations();

            var typesWithDiAttribute = assembly.SelectMany(a => a.GetTypes()
                .Where(type => (type.IsClass || type.IsInterface) && !type.IsAbstract && type.GetCustomAttribute<DIAttribute>() != null));

            foreach (var type in typesWithDiAttribute)
            {
                var attribute = type.GetCustomAttribute<DIAttribute>();
                var lifetime = attribute!.Lifetime;

                // Register the type based on the specified lifetime
                switch (lifetime)
                {
                    case ServiceLifetime.Singleton:
                        if (attribute.ImplimentingType != null)
                        {
                            services.AddSingleton(attribute.ImplimentingType, type);
                        }
                        else
                        {
                            services.AddSingleton(type);
                        }
                        break;
                    case ServiceLifetime.Transient:
                        if (attribute.ImplimentingType != null)
                            services.AddTransient(attribute.ImplimentingType, type);
                        else
                            services.AddTransient(type);
                        break;

                    case ServiceLifetime.Scoped:
                    default:
                        if (attribute.ImplimentingType != null)
                        {
                            services.AddScoped(attribute.ImplimentingType, type);
                        }
                        else
                        {
                            services.AddScoped(type);
                        }
                        break;
                }
            }

            return services;
        }

        //public static void RegisterExternalApis(this IServiceCollection services, ConfigurationManager configurationManager)
        //{
        //    string apiKey = configurationManager["ApplicationSettings:openAIKey"];
        //    //services.AddTransient(provider => new ChatGdpClient(apiKey));
        //}
    }
}
