using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceAggregator.Shared
{
    public class Trade
    {
        public int Id { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal Volume { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
