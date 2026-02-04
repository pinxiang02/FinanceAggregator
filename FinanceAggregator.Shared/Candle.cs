using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceAggregator.Shared
{
    public class Candle
    {
        public int Id { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public DateTime BucketTime { get; set; }
    }
}
