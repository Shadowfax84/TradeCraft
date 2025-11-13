using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Finance.Net.Interfaces;
using TC_Backend.Models;
using TC_Backend.Services.Interfaces;
using TC_Backend.Data;
using TC_Backend.DTOs;

namespace TC_Backend.BackgroundServices
{
    public class PriceSimulationEngine : BackgroundService
    {
        private readonly ILogger<PriceSimulationEngine> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHubContext<StockPriceHub> _hubContext;
        private readonly ConcurrentDictionary<string, StockQuoteData> _stockQuotes;
        private readonly Timer _timer;

        public PriceSimulationEngine(ILogger<PriceSimulationEngine> logger, IServiceProvider serviceProvider, IHubContext<StockPriceHub> hubContext)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _hubContext = hubContext;
            _stockQuotes = new ConcurrentDictionary<string, StockQuoteData>();
            _timer = new Timer(ExecuteSimulation, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Price Simulation Engine is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    _logger.LogInformation("Price Simulation Engine is stopping gracefully.");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled error in Price Simulation Engine.");
                }
            }

            _logger.LogInformation("Price Simulation Engine has stopped.");
        }

        private async void ExecuteSimulation(object? state)
        {
            try
            {
                _logger.LogDebug("Starting price simulation cycle");

                await GetExternalData();
                await SimulateChange();

                _logger.LogDebug("Price simulation cycle completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during price simulation cycle");
            }
        }

        private async Task GetExternalData()
        {
            try
            {
                _logger.LogDebug("Getting external data...");

                using var scope = _serviceProvider.CreateScope();
                var companyListService = scope.ServiceProvider.GetRequiredService<ICompanyListService>();
                var yahooFinanceService = scope.ServiceProvider.GetRequiredService<IYahooFinanceService>();

                // Get all companies from database
                var companies = await companyListService.GetAllCompaniesServ();
                var tickerSymbols = companies.Select(c => c.TickerSymbol).ToList();

                if (!tickerSymbols.Any())
                {
                    _logger.LogWarning("No ticker symbols found in database");
                    return;
                }

                _logger.LogDebug("Fetching quotes for {Count} symbols", tickerSymbols.Count);

                // Get quotes from Finance.NET
                var quotes = await yahooFinanceService.GetQuotesAsync(tickerSymbols);

                if (quotes == null || !quotes.Any())
                {
                    _logger.LogWarning("No quotes returned from Finance.NET");
                    return;
                }

                // Map and store quotes in ConcurrentDictionary
                foreach (var quote in quotes)
                {
                    if (quote?.Symbol != null)
                    {
                        var stockQuoteData = new StockQuoteData
                        {
                            Symbol = quote.Symbol,
                            RegularMarketPrice = (decimal?)quote.RegularMarketPrice,
                            FiftyTwoWeekHigh = (decimal?)quote.FiftyTwoWeekHigh,
                            FiftyTwoWeekLow = (decimal?)quote.FiftyTwoWeekLow,
                            RegularMarketVolume = quote.RegularMarketVolume,
                            LastUpdated = DateTime.UtcNow
                        };

                        _stockQuotes.AddOrUpdate(quote.Symbol, stockQuoteData, (key, oldValue) => stockQuoteData);
                    }
                }

                _logger.LogInformation("Successfully updated {Count} stock quotes", _stockQuotes.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching external data");
            }
        }

        private async Task SimulateChange()
        {
            _logger.LogDebug("Simulating price changes...");

            foreach (var stockQuote in _stockQuotes)
            {
                var tickerSymbol = stockQuote.Key;
                var quoteData = stockQuote.Value;

                try
                {
                    // Step 1: Establish Base Price and Boundaries
                    var basePrice = quoteData.RegularMarketPrice;
                    if (!basePrice.HasValue)
                    {
                        _logger.LogWarning("No base price available for {TickerSymbol}, skipping", tickerSymbol);
                        continue;
                    }

                    // Calculate price boundaries
                    var minPrice = (quoteData.FiftyTwoWeekLow ?? basePrice.Value) - 5;
                    var maxPrice = (quoteData.FiftyTwoWeekHigh ?? basePrice.Value) + 5;

                    // Ensure minPrice is not negative
                    minPrice = Math.Max(minPrice, 0.01m);

                    _logger.LogDebug("Price boundaries for {TickerSymbol}: Base={BasePrice}, Min={MinPrice}, Max={MaxPrice}", 
                        tickerSymbol, basePrice.Value, minPrice, maxPrice);

                    // Step 2: Calculate Weighted Average Prices for Buy and Sell Orders
                    var buyOrders = await GetBuyOrdersForStock(tickerSymbol);
                    var sellOrders = await GetSellOrdersForStock(tickerSymbol);

                    decimal? avgBuyPrice = null;
                    decimal? avgSellPrice = null;

                    // Calculate volume-weighted average buy price
                    if (buyOrders.Any())
                    {
                        var totalBuyValue = buyOrders.Sum(o => o.Price * o.Quantity);
                        var buyVolumeForAvg = buyOrders.Sum(o => o.Quantity);
                        avgBuyPrice = totalBuyValue / buyVolumeForAvg;
                    }

                    // Calculate volume-weighted average sell price
                    if (sellOrders.Any())
                    {
                        var totalSellValue = sellOrders.Sum(o => o.Price * o.Quantity);
                        var sellVolumeForAvg = sellOrders.Sum(o => o.Quantity);
                        avgSellPrice = totalSellValue / sellVolumeForAvg;
                    }

                    _logger.LogDebug("Weighted averages for {TickerSymbol}: AvgBuy={AvgBuy}, AvgSell={AvgSell}", 
                        tickerSymbol, avgBuyPrice, avgSellPrice);

                    // Step 3: Measure Market Pressure Using Net Volume
                    var totalBuyVolume = buyOrders.Sum(o => o.Quantity);
                    var totalSellVolume = sellOrders.Sum(o => o.Quantity);
                    var netVolume = totalBuyVolume - totalSellVolume;

                    // Calculate direction based on net volume
                    int direction = netVolume > 0 ? 1 : (netVolume < 0 ? -1 : 0);

                    // Calculate volume factor (normalized to MaxExpectedVolume)
                    const int maxExpectedVolume = 10000;
                    var volumeFactor = Math.Min(1.0m, Math.Abs(netVolume) / (decimal)maxExpectedVolume);

                    _logger.LogDebug("Market pressure for {TickerSymbol}: NetVolume={NetVolume}, Direction={Direction}, VolumeFactor={VolumeFactor}", 
                        tickerSymbol, netVolume, direction, volumeFactor);

                    // Step 4: Calculate Mid-Price and Price Change
                    decimal? midPrice = null;
                    if (avgBuyPrice.HasValue && avgSellPrice.HasValue)
                    {
                        midPrice = (avgBuyPrice.Value + avgSellPrice.Value) / 2;
                    }

                    // Generate random price step (0 to 5)
                    var random = new Random();
                    var randomStep = (decimal)(random.NextDouble() * 5);

                    // Step 5: Update Price with Controlled Randomness
                    var priceChange = direction * volumeFactor * randomStep;
                    var newPrice = Clamp(basePrice.Value + priceChange, minPrice, maxPrice);

                    // Only update and broadcast if price actually changed
                    if (newPrice != basePrice.Value)
                    {
                        // Update the stock quote with new price
                        quoteData.RegularMarketPrice = newPrice;
                        quoteData.LastUpdated = DateTime.UtcNow;

                        // Broadcast price update via SignalR
                        var priceUpdate = new StockPriceUpdateDTO
                        {
                            TickerSymbol = tickerSymbol,
                            CurrentPrice = newPrice,
                            Timestamp = DateTime.UtcNow
                        };

                        await _hubContext.Clients.Group($"Stock_{tickerSymbol}")
                            .SendAsync("ReceiveStockPriceUpdate", priceUpdate);

                        _logger.LogDebug("Price update for {TickerSymbol}: Old={OldPrice}, New={NewPrice}, Change={PriceChange}", 
                            tickerSymbol, basePrice.Value, newPrice, priceChange);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error calculating price boundaries for {TickerSymbol}", tickerSymbol);
                }
            }
        }

        private async Task<List<Order>> GetBuyOrdersForStock(string tickerSymbol)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<TC_BackendDbContext>();

                return await dbContext.Orders
                    .Where(o => o.TickerSymbol == tickerSymbol && 
                               o.OrderType == OrderType.Buy && 
                               o.OrderStatus == OrderStatus.Pending)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching buy orders for {TickerSymbol}", tickerSymbol);
                return new List<Order>();
            }
        }

        private async Task<List<Order>> GetSellOrdersForStock(string tickerSymbol)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<TC_BackendDbContext>();

                return await dbContext.Orders
                    .Where(o => o.TickerSymbol == tickerSymbol && 
                               o.OrderType == OrderType.Sell && 
                               o.OrderStatus == OrderStatus.Pending)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching sell orders for {TickerSymbol}", tickerSymbol);
                return new List<Order>();
            }
        }

        public Dictionary<string, decimal> GetCurrentPrices()
        {
            var prices = new Dictionary<string, decimal>();

            foreach (var kvp in _stockQuotes)
            {
                if (kvp.Value.RegularMarketPrice.HasValue)
                {
                    prices[kvp.Key] = kvp.Value.RegularMarketPrice.Value;
                }
            }

            return prices;
        }

        private static decimal Clamp(decimal value, decimal min, decimal max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        public override void Dispose()
        {
            _timer?.Dispose();
            base.Dispose();
        }
    }
}