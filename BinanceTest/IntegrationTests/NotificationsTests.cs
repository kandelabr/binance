using Binance.API.Configuration;
using Binance.Configuration;
using Binance.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace BinanceTest.IntegrationTests
{
    [TestClass]
    public class NotificationsTests
    {
        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void TestSendEmail()
        {
            IConfiguration configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false)
               .Build();

            ServiceCollection services = new ServiceCollection();
            services.AddConfigurations(configuration);
            services.AddAPIConfigurations(configuration);
            services.AddHandlers();

            ServiceProvider serviceProvider = services.BuildServiceProvider();
            INotificationHandler notificationHandler = serviceProvider.GetService<INotificationHandler>();
            notificationHandler.SendEmail("test", "test").Wait();
        }
    }
}
