using Binance.API.Client;
using Binance.API.Configuration;
using Binance.Configuration;
using Binance.DB;
using Binance.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Binance
{
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// The configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddAPIConfigurations(Configuration);
            services.AddConfigurations(Configuration);
            services.AddClients();
            services.AddHttpClients();
            services.AddHandlers();
            services.AddDatabase();
            services.AddStartupTask<ExchangeInfoStartupTask>();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">An application builder.</param>
        public static void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            //app.UseMiddleware<TrafficLoggerMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
