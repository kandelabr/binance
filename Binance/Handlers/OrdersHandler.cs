using Binance.API.Client;
using Binance.Configuration;
using Binance.Helpers;
using Binance.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Binance.Handlers
{
    /// <summary>
    /// Orders class
    /// </summary>
    public class OrdersHandler : IOrdersHandler
    {
        private readonly IBinanceClient _binanceClient;
        private readonly IBinanceBuilder _binanceBuilder;
        private readonly bool _useLimit;
        private readonly decimal _margin;

        public OrdersHandler(IBinanceClient binanceClient, IBinanceBuilder binanceBuilder,
            IOptions<GeneralConfiguration> configuration)
        {
            _binanceClient = binanceClient;
            _binanceBuilder = binanceBuilder;
            _useLimit = bool.Parse(configuration.Value.UseLimit);
            _margin = decimal.Parse(configuration.Value.Margin);
        }


        /// <summary>
        /// Buys array of coin requests
        /// </summary>
        public async Task<OrderResponse[]> Buy(OrderRequest[] orderRequest)
        {
            IList<Task<OrderResponse>> tasks = new List<Task<OrderResponse>>();
            foreach (var r in orderRequest)
                tasks.Add(Buy(r));

            var response = await Task.WhenAll(tasks.ToArray()).ConfigureAwait(false);
            return response;
        }

        /// <summary>
        /// Sells array of coin requests
        /// </summary>
        public async Task<OrderResponse[]> Sell(OrderRequest[] orderRequest)
        {
            IList<Task<OrderResponse>> tasks = new List<Task<OrderResponse>>();
            foreach (var r in orderRequest)
                tasks.Add(Sell(r));

            var response = await Task.WhenAll(tasks.ToArray()).ConfigureAwait(false);
            return response;
        }

        /// <summary>
        /// Buys coin requests
        /// </summary>
        private async Task<OrderResponse> Buy(OrderRequest orderRequest)
        {
            byte decimalPlaces;
            if (!ExchangeInfoStartupTask.ExchangeInfoLotDictionary.ContainsKey(orderRequest.Symbol)
                || !ExchangeInfoStartupTask.ExchangeInfoPriceDictionary.TryGetValue(orderRequest.Symbol, out decimalPlaces))
            {
                string value = $"{orderRequest.Symbol} not found in exchange dictionary";
                return OrdersHandlerHelper.GetErrorResponse(System.Net.HttpStatusCode.NotFound, value, orderRequest.Symbol);
            }

            //buy coin
            var buyClientReq = _binanceBuilder.BuildBuyOrderRequest(orderRequest.Symbol, orderRequest.Amount);
            var buyClientResponse = await _binanceClient.Buy(buyClientReq).ConfigureAwait(false);

            if (buyClientResponse == default)
            {
                string value = $"Buy failed for {orderRequest.Symbol} and amount {orderRequest.Amount}";
                return OrdersHandlerHelper.GetErrorResponse(System.Net.HttpStatusCode.InternalServerError, value, orderRequest.Symbol);
            }

            OrderResponse orderResponse = new OrderResponse()
            {
                OrderId = buyClientResponse.OrderId,
                Symbol = buyClientResponse.Symbol,
                Amount = decimal.Parse(buyClientResponse.ExecutedQty),
                Price = buyClientResponse.Fills.Max(x => decimal.Parse(x.Price))
            };

            //sell coin if limit is ON
            Task.Run(() => SellLimitCoin(orderResponse, decimalPlaces));

            return orderResponse;
        }

        /// <summary>
        /// Sells coin requests
        /// </summary>
        private async Task<OrderResponse> Sell(OrderRequest orderRequest)
        {
            byte decimalPlaces;
            if (!ExchangeInfoStartupTask.ExchangeInfoLotDictionary.TryGetValue(orderRequest.Symbol, out decimalPlaces))
            {
                string value = $"{orderRequest.Symbol} not found in exchange dictionary";
                return OrdersHandlerHelper.GetErrorResponse(System.Net.HttpStatusCode.NotFound, value, orderRequest.Symbol);
            }

            //try and delete
            var deleteReq = new API.Models.DeleteOrderRequest(orderRequest.Symbol);
            var deleteResponse = new API.Models.DeleteOrderResponse() { DeleteStatus = API.Models.DeleteStatus.Deleted };
            if (_useLimit)
                deleteResponse = await _binanceClient.Delete(deleteReq).ConfigureAwait(false);

            //if sold => it is done, just ignore
            if (deleteResponse?.DeleteStatus == API.Models.DeleteStatus.Sold)
            {
                OrderResponse orderResponse = new OrderResponse()
                { Symbol = orderRequest.Symbol };

                return orderResponse;
            }
            else
            {
                //if deleted => MARKET SELL
                //if NULL or something went wrong => try to SELL anyway (should not happen)
                decimal amount = Math.Floor(orderRequest.Amount * (decimal)Math.Pow(10, decimalPlaces)) / (decimal)Math.Pow(10, decimalPlaces);

                var clientReq = _binanceBuilder.BuildMarketSellOrderRequest(orderRequest.Symbol, amount);
                var clientResponse = await _binanceClient.Sell(clientReq).ConfigureAwait(false);

                if (clientResponse == default)
                {
                    string value = $"Sell failed for {orderRequest.Symbol} and amount {orderRequest.Amount} - {amount}";
                    return OrdersHandlerHelper.GetErrorResponse(System.Net.HttpStatusCode.InternalServerError, value, orderRequest.Symbol);
                }

                return new OrderResponse()
                {
                    Symbol = clientResponse.Symbol,
                    Amount = decimal.Parse(clientResponse.CummulativeQuoteQty)
                };
            }
        }

        private void SellLimitCoin(Models.OrderResponse buyOrderResponse, byte decimalPlaces)
        {
            if (!_useLimit)
                return;

            decimal price = Math.Round(buyOrderResponse.Price * _margin, decimalPlaces);
            var sellClientReq = _binanceBuilder.BuildLimitSellOrderRequest(buyOrderResponse.Symbol, buyOrderResponse.Amount, price);
            _binanceClient.Sell(sellClientReq);
        }
    }
}
