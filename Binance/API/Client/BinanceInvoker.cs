using Binance.API.Configuration;
using Binance.API.Models;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Binance.API.Client
{
    /// <summary>
    /// Client class
    /// </summary>
    public class BinanceInvoker : IBinanceInvoker
    {
        private readonly IHttpClientFactory _factory;
        private readonly string _orderPath;
        private readonly string _serverTimePath;
        private readonly string _pricesPath;
        private readonly string _deletePath;
        private readonly string _exchangeInfoPath;
        private readonly string _appKey;

        public BinanceInvoker(IHttpClientFactory factory, IOptions<PathConfiguration> configuration, IOptions<KeysConfiguration> keysConfiguration)
        {
            _factory = factory;
            _orderPath = configuration.Value.OrderPath;
            _serverTimePath = configuration.Value.ServerTimePath;
            _pricesPath = configuration.Value.PricesPath;
            _deletePath = configuration.Value.DeletePath;
            _exchangeInfoPath = configuration.Value.ExchangeInfoPath;
            _appKey = keysConfiguration.Value.App;
        }

        /// <summary>
        /// Order crypto operation.
        /// </summary>
        public Task<HttpResponseMessage> Order(OrderRequest request)
        {
            HttpClient client = _factory.CreateClient(Constants.HttpClientName);
            var requestString = request.GetSecureParamsString();
            HttpRequestMessage httpRequest = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{_orderPath}?{requestString}", UriKind.Relative)
            };
            httpRequest.Headers.Add("X-MBX-APIKEY", _appKey);
            return client.SendAsync(httpRequest);
        }

        /// <summary>
        /// Order crypto operation.
        /// </summary>
        public Task<HttpResponseMessage> Delete(DeleteOrderRequest request)
        {
            HttpClient client = _factory.CreateClient(Constants.HttpClientName);
            var requestString = request.GetSecureParamsString();
            HttpRequestMessage httpRequest = new HttpRequestMessage()
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{_deletePath}?{requestString}", UriKind.Relative)
            };
            httpRequest.Headers.Add("X-MBX-APIKEY", _appKey);
            return client.SendAsync(httpRequest);
        }

        /// <summary>
        /// Get server time
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> GetServerTime()
        {
            HttpClient client = _factory.CreateClient(Constants.HttpClientName);
            return client.GetAsync(new Uri(_serverTimePath, UriKind.Relative));
        }

        /// <summary>
        /// Get dictionary of prices
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> GetPrices()
        {
            HttpClient client = _factory.CreateClient(Constants.HttpClientName);
            return client.GetAsync(new Uri(_pricesPath, UriKind.Relative));
        }

        /// <summary>
        /// Get exchange info of prices
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> GetExchangeInfo()
        {
            HttpClient client = _factory.CreateClient(Constants.HttpClientName);
            return client.GetAsync(new Uri(_exchangeInfoPath, UriKind.Relative));
        }
    }
}
