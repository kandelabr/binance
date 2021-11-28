namespace Binance.API.Configuration
{
    /// <summary>
    /// API configuration
    /// </summary>
    public class EmailConfiguration
    {
        /// <summary>
        /// From 
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// To
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// User
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Pass
        /// </summary>
        public string Pass { get; set; }

        /// <summary>
        /// Is email enabled
        /// </summary>
        public string Enabled { get; set; }
    }
}
