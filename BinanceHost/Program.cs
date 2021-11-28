using Binance.API.Client;
using Binance.API.Configuration;
using Binance.Configuration;
using Binance.DB;
using Binance.Handlers;
using Binance.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;

namespace BinanceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            //start web service
            Binance.Program.Main(args);

            Thread.Sleep(5000);

            //start win service
            CreateHostBuilder(args).Build().Run();

            Console.Read();
        }

        //console
        //win svc
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var host = CreateHost(args);
            if (!args.Any(x => x == "console=true"))
            {
                host.UseWindowsService();
            }
            return host;
        }
        private static IHostBuilder CreateHost(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

            var host = Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    Binance.Program.ConfigureLogging(logging);
                })
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
                    builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddAPIConfigurations(config);
                    services.AddConfigurations(config);
                    services.AddClients();
                    services.AddHttpClients();
                    services.AddHandlers();
                    services.AddDatabase();
                    services.AddHostedService<PriceWorker>();
                });

            return host;
        }
    }
}
