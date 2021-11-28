using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.Threading.Tasks;

namespace Binance
{
    public sealed class Program
    {
        /// <summary>
        /// Main method.
        /// </summary>
        /// <param name="args">Arguments.</param>
        /// <returns>Exit code.</returns>
        public static async Task<int> Main(string[] args)
        {

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                .Enrich.FromLogContext()
                .WriteTo.EventLog(source: "BinanceLog",
                      logName: "BinanceLog",
                      manageEventSource: true)
                .CreateLogger();

            try
            {
                Log.Information("Starting web host");
                await CreateWebHostBuilder(args).Build()
                    .RunWithTasksAsync()
                    .ConfigureAwait(false);

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {

            }
        }

        /// <summary>
        /// Create a WebHost builder.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    ConfigureLogging(logging);
                })
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
                    builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .UseStartup<Startup>()
                .UseUrls("http://localhost:5001/");

        public static void ConfigureLogging(ILoggingBuilder logging)
        {
            logging.AddEventLog(eventLogSettings =>
            {
                eventLogSettings.LogName = "BinanceLog";
                eventLogSettings.SourceName = "BinanceLog";
            });
        }
    }
}
