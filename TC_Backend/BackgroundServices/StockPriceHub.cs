using Microsoft.AspNetCore.SignalR;

namespace TC_Backend.BackgroundServices
{
    public class StockPriceHub : Hub
    {
        private readonly ILogger<StockPriceHub> _logger;

        public StockPriceHub(ILogger<StockPriceHub> logger)
        {
            _logger = logger;
        }

        public async Task SubscribeToStock(string tickerSymbol)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Stock_{tickerSymbol}");
            _logger.LogInformation("Client {ConnectionId} subscribed to {TickerSymbol}", Context.ConnectionId, tickerSymbol);
        }

        public async Task UnsubscribeFromStock(string tickerSymbol)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Stock_{tickerSymbol}");
            _logger.LogInformation("Client {ConnectionId} unsubscribed from {TickerSymbol}", Context.ConnectionId, tickerSymbol);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation("Client {ConnectionId} disconnected", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}