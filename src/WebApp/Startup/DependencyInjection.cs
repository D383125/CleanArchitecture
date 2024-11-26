using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace WebApp.Startup
{
    internal static class DependencyInjection
    {
        public static IServiceCollection AddWebServices(this IServiceCollection services)
        {
            //services.AddDatabaseDeveloperPageExceptionFilter();
            //services.AddScoped<IUser, CurrentUser>();

            services.AddHttpContextAccessor();

            services.AddHealthChecks()
                .AddDbContextCheck<ApplicationDbContext>();

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
    }
}
