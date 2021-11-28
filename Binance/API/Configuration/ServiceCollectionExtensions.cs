using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Binance.API.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAPIConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<PathConfiguration>()
                .Bind(configuration.GetSection(nameof(PathConfiguration)));

            services.AddOptions<KeysConfiguration>()
                .Bind(configuration.GetSection(nameof(KeysConfiguration)));

            services.AddOptions<EmailConfiguration>()
                .Bind(configuration.GetSection(nameof(EmailConfiguration)));

            return services;
        }
    }
}
