namespace Binance.API.Models
{
    public enum DeleteStatus { Sold, Deleted, Error }

    /// <summary>
    /// The request
    /// </summary>
    public class DeleteOrderResponse
    {
        /// <summary>
        /// The status
        /// </summary>
        public DeleteStatus DeleteStatus { get; set; }
    }
}
