namespace TC_Backend.Models
{
    public class StockQuoteData
    {
        public string Symbol { get; set; } = null!;
        public decimal? RegularMarketPrice { get; set; }
        public decimal? FiftyTwoWeekHigh { get; set; }
        public decimal? FiftyTwoWeekLow { get; set; }
        public long? RegularMarketVolume { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}