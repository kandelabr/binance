using System;

namespace Binance.API.Models
{
    /// <summary>
    /// Custom exception class
    /// </summary>
    public class BinanceAPIException : Exception
    {
        private readonly Error _error;
        public Error Error => _error;

        public BinanceAPIException(Error error, string message, Exception innerException) : base(message, innerException)
        {
            _error = error;
        }

        public BinanceAPIException(Error error, string message) : this(error, message, null) { }

        public BinanceAPIException(Error error) : this(error, null, null) { }
    }
}
