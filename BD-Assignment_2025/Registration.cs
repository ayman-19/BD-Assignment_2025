using System.Collections.Concurrent;
using BD_Assignment_2025.BackgroundJobs;
using BD_Assignment_2025.IEndpoints;
using BD_Assignment_2025.IRepositories;
using BD_Assignment_2025.IServices;
using BD_Assignment_2025.Middlewares;
using BD_Assignment_2025.Repositories;
using BD_Assignment_2025.Services;

namespace BD_Assignment_2025
{
    public static class Registration
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services
                .AddSingleton(typeof(ConcurrentDictionary<,>))
                .AddSingleton(typeof(ConcurrentBag<>))
                .AddScoped(typeof(IRepository<,>), typeof(Repository<,>))
                .AddScoped<IBlockedCountryRepository, BlockedCountryRepository>()
                .AddScoped<IBlockedCountryService, BlockedCountryService>()
                .AddScoped<ExceptionHandler>()
                .AddHostedService<ExpiredBlockService>();
            return services;
        }

        public static void RegisterAllEndpoints(this IEndpointRouteBuilder app)
        {
            IEnumerable<IEndpoint>? endpoints = AppDomain
                .CurrentDomain.GetAssemblies()
                .SelectMany(assemply => assemply.GetTypes())
                .Where(type =>
                    typeof(IEndpoint).IsAssignableFrom(type)
                    && !type.IsInterface
                    && !type.IsAbstract
                )
                .Select(Activator.CreateInstance)
                .Cast<IEndpoint>();

            foreach (IEndpoint endpoint in endpoints)
                endpoint.RegisterEndpoints(app);
        }
    }
}
