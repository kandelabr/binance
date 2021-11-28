using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Binance.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<GeneralConfiguration>()
                .Bind(configuration.GetSection(nameof(GeneralConfiguration)));

            return services;
        }
    }
}
