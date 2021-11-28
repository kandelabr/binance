using System;
using System.Collections.Generic;

namespace Binance.DB.Models
{
    public class Information
    {
        public int ID { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<Price> Prices { get; set; }

    }

    public class Price
    {
        public long ID { get; set; }
        public string Symbol { get; set; }
        public decimal Amount { get; set; }

        public int InformationId { get; set; }
        public Information Information { get; set; }
    }
}
