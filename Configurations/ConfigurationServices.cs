using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyConsoleApp.Interfaces;
using MyConsoleApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyConsoleApp.Configurations
{
    public static class ConfigurationServices
    {
        public static IServiceCollection Config(this IServiceCollection services)
        {
            services.AddTransient<ITleService, TleService>();
            services.AddLogging(loggerBuilder =>
            {
                loggerBuilder.ClearProviders();
                loggerBuilder.AddConsole();
            });

            return services;
        }
    }
}
