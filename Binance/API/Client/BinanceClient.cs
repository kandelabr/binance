using Binance.API.Models;
using Binance.Handlers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Binance.API.Client
{
    /// <summary>
    /// Client class
    /// </summary>
    public class BinanceClient : IBinanceClient
    {
        private readonly IBinanceBuilder _builder;
        private readonly IBinanceInvoker _invoker;
        private readonly INotificationHandler _notificationHandler;
        private readonly ILogger<BinanceClient> _logger;

        public BinanceClient(IBinanceBuilder builder, IBinanceInvoker invoker, INotificationHandler notificationHandler, ILogger<BinanceClient> logger)
        {
            _builder = builder;
            _invoker = invoker;
            _notificationHandler = notificationHandler;
            _logger = logger;
        }

        /// <summary>
        /// Buy crypto.
        /// </summary>
        /// <remarks>
        /// In case of SHIBUSDT we will buy SHIB, sell USDT
        /// </remarks>
        public async Task<OrderResponse> Buy(OrderRequest request)
        {
            try
            {
                request.Timestamp = _builder.BuildTimestamp();
                request.Signature = _builder.BuildSignature(request.GetUnsecureParamsString());

                var response = await _invoker.Order(request).ConfigureAwait(false);
                return await GetResult<OrderResponse>(response).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Buy failed for symbol {request.Symbol}");
                SendEmail($"HIGH buy failed: {request.GetUnsecureParamsString()}", ex);
                return default;
            }
        }

        /// <summary>
        /// Sell crypto.
        /// </summary>
        /// <remarks>
        /// In case of SHIBUSDT we will sell SHIB, buy USDT
        /// </remarks>
        public async Task<OrderResponse> Sell(OrderRequest request)
        {
            try
            {
                request.Timestamp = _builder.BuildTimestamp();
                request.Signature = _builder.BuildSignature(request.GetUnsecureParamsString());

                var response = await _invoker.Order(request).ConfigureAwait(false);
                return await GetResult<OrderResponse>(response).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Sell failed for symbol {request.Symbol}");
                SendEmail($"CRITICAL sell failed: {request.GetUnsecureParamsString()}", ex);
                return default;
            }
        }

        /// <summary>
        /// Sell crypto.
        /// </summary>
        /// <remarks>
        /// In case of SHIBUSDT we will sell SHIB, buy USDT
        /// </remarks>
        public async Task<DeleteOrderResponse> Delete(DeleteOrderRequest request)
        {
            try
            {
                request.Timestamp = _builder.BuildTimestamp();
                request.Signature = _builder.BuildSignature(request.GetUnsecureParamsString());

                var response = await _invoker.Delete(request).ConfigureAwait(false);
                if (await HandleResult(response).ConfigureAwait(false))
                    return new DeleteOrderResponse() { DeleteStatus = DeleteStatus.Deleted };
            }
            catch (BinanceAPIException bex)
            {
                if (CheckIfValidMarketSellException(bex.Error, request))
                {
                    //all is well it was SOLD => return true
                    return new DeleteOrderResponse() { DeleteStatus = DeleteStatus.Sold };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Delete failed for symbol {request.Symbol}");
                SendEmail($"CRITICAL delete failed: {request.GetUnsecureParamsString()}", ex);
            }

            return new DeleteOrderResponse() { DeleteStatus = DeleteStatus.Error };
        }

        /// <summary>
        /// Get server time
        /// </summary>
        /// <returns></returns>
        public async Task<ServerTimeResponse> GetServerTime()
        {
            try
            {
                var response = await _invoker.GetServerTime().ConfigureAwait(false);
                return await GetResult<ServerTimeResponse>(response).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return default;
            }
        }

        /// <summary>
        /// Get dictionary of prices
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, string>[]> GetPrices()
        {
            try
            {
                var response = await _invoker.GetPrices().ConfigureAwait(false);
                return await GetResult<Dictionary<string, string>[]>(response).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                SendEmail("HIGH get prices failed", ex);
                return default;
            }
        }

        /// <summary>
        /// Get exchange info of prices
        /// </summary>
        /// <returns></returns>
        public async Task<ExchangeInfoResponse> GetExchangeInfo()
        {
            try
            {
                var response = await _invoker.GetExchangeInfo().ConfigureAwait(false);
                var result = await GetResult<ExchangeInfoResponse>(response).ConfigureAwait(false);
                return result;
            }
            catch (Exception ex)
            {
                SendEmail("CRITICAL get exchange info failed", ex);
                return default;
            }
        }

        private async Task<T> GetResult<T>(HttpResponseMessage response)
        {
            var stringResult = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (IsValidResponse(response))
            {
                return JsonConvert.DeserializeObject<T>(stringResult);
            }
            else
            {
                if (TryGetError(stringResult, out Error error))
                {
                    throw new BinanceAPIException(error, stringResult);
                }

                throw new Exception(stringResult);
            }
        }

        private async Task<bool> HandleResult(HttpResponseMessage response)
        {
            var stringResult = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (IsValidResponse(response))
            {
                return true;
            }
            else
            {
                if (TryGetError(stringResult, out Error error))
                {
                    throw new BinanceAPIException(error, stringResult);
                }
                else
                {
                    throw new Exception(stringResult);
                }
            }
        }

        private static bool IsValidResponse(HttpResponseMessage response)
        {
            return response.StatusCode == System.Net.HttpStatusCode.OK
                || response.StatusCode == System.Net.HttpStatusCode.Created;
        }

        private static bool TryGetError(string stringResult, out Error error)
        {
            error = null;
            try
            {
                error = JsonConvert.DeserializeObject<Error>(stringResult);
            }
            catch { }

            return error != null;
        }

        private async Task SendEmail(string subject, Exception ex)
        {
            Task.Run(() => _notificationHandler.SendEmail(subject, ex.ToString()).ConfigureAwait(false));
        }


        /// <summary>
        /// Can occur when we order was handled by LIMIT sell
        /// {
        /// "code": -2011,
        /// "msg": "Unknown order sent."
        /// }
        /// </summary>
        private static bool CheckIfValidMarketSellException(Error error, DeleteOrderRequest request)
        {
            return error?.Code == -2011;
        }
    }
}
