using Microsoft.Extensions.DependencyInjection;

namespace Binance.Handlers
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add clients.
        /// </summary>
        public static IServiceCollection AddHandlers(this IServiceCollection services)
        {
            services.AddSingleton<INotificationHandler, NotificationHandler>();
            services.AddSingleton<IOrdersHandler, OrdersHandler>();
            return services;
        }
    }
}
