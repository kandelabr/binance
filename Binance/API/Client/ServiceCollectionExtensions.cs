using Binance.API.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Binance.API.Client
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add clients.
        /// </summary>
        public static IServiceCollection AddClients(this IServiceCollection services)
        {
            services.AddSingleton<IBinanceBuilder, BinanceBuilder>();
            services.AddSingleton<IBinanceInvoker, BinanceInvoker>();
            services.AddSingleton<IBinanceClient, BinanceClient>();

            return services;
        }

        /// <summary>
        /// Add HTTP clients.
        /// </summary>
        public static IServiceCollection AddHttpClients(this IServiceCollection services)
        {
            services.AddHttpClient(Constants.HttpClientName)
                .ConfigureHttpClient((serviceProvider, client) =>
                {
                    client.BaseAddress = GetUri(serviceProvider.GetRequiredService<IOptions<PathConfiguration>>().Value);
                });

            return services;
        }

        private static Uri GetUri(PathConfiguration config)
        {
            return new Uri(config.BaseAdress, UriKind.Absolute);
        }
    }
}
