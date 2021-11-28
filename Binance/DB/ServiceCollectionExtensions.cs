using Microsoft.Extensions.DependencyInjection;

namespace Binance.DB
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add clients.
        /// </summary>
        public static IServiceCollection AddDatabase(this IServiceCollection services)
        {
            services.AddSingleton<IDatabaseProvider, EFDatabaseProvider>();

            return services;
        }
    }
}
