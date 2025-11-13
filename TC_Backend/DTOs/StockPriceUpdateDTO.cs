namespace TC_Backend.DTOs
{
    public class StockPriceUpdateDTO
    {
        public string TickerSymbol { get; set; } = null!;
        public decimal CurrentPrice { get; set; }
        public DateTime Timestamp { get; set; }
    }
}