﻿using Application.Interfaces;
using Domain;
using Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var temp = configuration.GetConnectionString("Database");

            services.AddDbContext<ApplicationDbContext>(
                options =>                
                    options
                    .UseNpgsql(configuration.GetConnectionString("Database"))
                    .UseLowerCaseNamingConvention()                
            );

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            return services;
        }
    }
}
