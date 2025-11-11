using Microsoft.EntityFrameworkCore;
using TC_Backend.Data;
using TC_Backend.Models;
using Finance.Net.Interfaces;
using TC_Backend.BackgroundServices.Helpers;

namespace TC_Backend.BackgroundServices
{
    public class FinancialDataUpdService : BackgroundService
    {
        private readonly ILogger<FinancialDataUpdService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ExternalToDtoMapper _externalToDtoMapper;
        private readonly DtoToModelsMapper _dtoToModelsMapper;
        private IYahooFinanceService? _yahooService;
        private TC_BackendDbContext? _dbContext;

        public FinancialDataUpdService(ILogger<FinancialDataUpdService> logger, IServiceProvider serviceProvider, ExternalToDtoMapper externalToDtoMapper, DtoToModelsMapper dtoToModelsMapper)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _externalToDtoMapper = externalToDtoMapper;
            _dtoToModelsMapper = dtoToModelsMapper;
        }

        public async Task TriggerUpdateAsync(CancellationToken cancellationToken = default)
        {
            await UpdateFinancialDataIfNeeded(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Financial Data Update Service is starting.");

            await UpdateFinancialDataIfNeeded(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                    await UpdateFinancialDataIfNeeded(stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    _logger.LogInformation("Financial Data Update Service is stopping gracefully.");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled error in Financial Data Update Service.");
                }
            }

            _logger.LogInformation("Financial Data Update Service has stopped.");
        }

        private async Task UpdateFinancialDataIfNeeded(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            _dbContext = scope.ServiceProvider.GetRequiredService<TC_BackendDbContext>();
            _yahooService = scope.ServiceProvider.GetRequiredService<IYahooFinanceService>();

            try
            {
                _logger.LogInformation("Checking if financial data update is needed...");

                var latestRecordDate = await _dbContext.StockRecords
                    .OrderByDescending(r => r.Date)
                    .Select(r => r.Date)
                    .FirstOrDefaultAsync(stoppingToken);

                var now = DateTime.UtcNow;
                var shouldUpdateByTime = latestRecordDate == default ||
                                   (now - latestRecordDate).TotalHours >= 24;

                var allTickers = await _dbContext.CompanysList
                    .Select(cl => cl.TickerSymbol)
                    .ToListAsync(stoppingToken);

                var trackedTickers = await _dbContext.CompanyProfiles
                    .Select(cp => cp.TickerSymbol)
                    .ToListAsync(stoppingToken);

                var newTickers = allTickers.Except(trackedTickers).ToList();
                var hasNewStocks = newTickers.Count != 0;

                if (hasNewStocks)
                {
                    _logger.LogInformation("Detected {NewStockCount} new stocks: {NewTickers}. Triggering immediate update.",
                        newTickers.Count, string.Join(", ", newTickers));
                }

                var shouldUpdate = shouldUpdateByTime || hasNewStocks;

                if (!shouldUpdate)
                {
                    _logger.LogInformation("Financial data is up to date. Last update: {Date}. No new stocks detected.", latestRecordDate);
                    return;
                }

                if (shouldUpdateByTime && !hasNewStocks)
                {
                    _logger.LogInformation("Financial data update triggered by time (24 hours passed). Last update: {Date}", latestRecordDate);
                }

                var trackedTickersToUpdate = await GetTickersAsync(stoppingToken);
                _logger.LogInformation("Starting financial data update for {SymbolCount} stocks...", trackedTickers.Count);

                int successCount = 0;
                foreach (var symbol in trackedTickersToUpdate)
                {
                    if (stoppingToken.IsCancellationRequested)
                    {
                        _logger.LogInformation("Stock data update cancelled by user.");
                        break;
                    }

                    bool success = await FetchAndStoreStockData(symbol, stoppingToken);
                    if (success)
                    {
                        successCount++;
                        if (newTickers.Contains(symbol))
                        {
                            _logger.LogInformation("Successfully fetched data for newly added stock: {Symbol}", symbol);
                        }
                    }
                }

                await _dbContext.SaveChangesAsync(stoppingToken);
                _logger.LogInformation("Financial data update completed successfully. Updated {SuccessCount} out of {TotalCount} stocks.",
                    successCount, trackedTickers.Count);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning(ex, "Financial data update was cancelled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during financial data update.");
            }
        }
        private async Task<List<string>> GetTickersAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TC_BackendDbContext>();

            return await dbContext.CompanysList
                .Select(cl => cl.TickerSymbol)
                .ToListAsync(stoppingToken);
        }

        private async Task<bool> FetchAndStoreStockData(string symbol, CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Starting to fetch CompanyProfile for symbol {Symbol}", symbol);
                await FetchAndStoreCompanyProfile(symbol, stoppingToken);
                _logger.LogInformation("Successfully fetched CompanyProfile data for symbol {Symbol}", symbol);

                _logger.LogInformation("Starting to fetch StockRecords for symbol {Symbol}", symbol);
                await FetchAndStoreStockRecords(symbol, stoppingToken);
                _logger.LogInformation("Successfully fetched StockRecords data for symbol {Symbol}", symbol);

                _logger.LogInformation("Starting to fetch FinancialReports for symbol {Symbol}", symbol);
                await FetchAndStoreFinancialReports(symbol, stoppingToken);
                _logger.LogInformation("Successfully fetched FinancialReports for symbol {Symbol}", symbol);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching data for symbol {Symbol}", symbol);
                return false;
            }
        }

        private async Task FetchAndStoreCompanyProfile(string symbol, CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogDebug("Fetching company profile for {Symbol}", symbol);

                var externalProfile = await _yahooService!.GetProfileAsync(symbol, stoppingToken);

                if (externalProfile == null)
                {
                    _logger.LogWarning("No company profile data returned for {Symbol}", symbol);
                    return;
                }

                var dtoProfile = _externalToDtoMapper.MapProfileToDto(externalProfile);
                if (dtoProfile == null)
                {
                    _logger.LogWarning("Failed to map external profile to DTO for {Symbol}", symbol);
                    return;
                }

                var existingCompany = await _dbContext!.CompanyProfiles
                    .FirstOrDefaultAsync(c => c.TickerSymbol == symbol, stoppingToken);

                if (existingCompany == null)
                {
                    var companyProfile = _dtoToModelsMapper.MapDtoToCompanyProfile(dtoProfile);
                    if (companyProfile == null)
                    {
                        _logger.LogWarning("Failed to map DTO to company profile model for {Symbol}", symbol);
                        return;
                    }
                    companyProfile.CPId = Guid.NewGuid();
                    companyProfile.TickerSymbol = symbol;
                    _dbContext.CompanyProfiles.Add(companyProfile);
                    _logger.LogInformation("Added new company profile for {Symbol}", symbol);
                }
                else
                {
                    existingCompany.Address = dtoProfile.Address;
                    existingCompany.Phone = dtoProfile.Phone;
                    existingCompany.Website = dtoProfile.Website;
                    existingCompany.Sector = dtoProfile.Sector;
                    existingCompany.Industry = dtoProfile.Industry;
                    existingCompany.CntEmployees = dtoProfile.CntEmployees;
                    existingCompany.Description = dtoProfile.Description;
                    _logger.LogInformation("Updated company profile for {Symbol}", symbol);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching company profile for {Symbol}", symbol);
            }
        }


        private async Task FetchAndStoreStockRecords(string symbol, CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogDebug("Fetching stock records for {Symbol}", symbol);

                var endDate = DateTime.UtcNow;
                var startDate = endDate.AddYears(-1);
                var records = await _yahooService!.GetRecordsAsync(symbol, startDate, endDate, stoppingToken);

                if (records == null || !records.Any())
                {
                    _logger.LogWarning("No stock records returned for {Symbol}", symbol);
                    return;
                }

                int addedCount = 0;
                foreach (var record in records)
                {
                    var existingRecord = await _dbContext!.StockRecords
                        .FirstOrDefaultAsync(r => r.TickerSymbol == symbol && r.Date == record.Date, stoppingToken);

                    if (existingRecord == null)
                    {
                        if (record.Date != default && record.Close.HasValue && record.Close > 0)
                        {
                            var stockRecordDto = _externalToDtoMapper.MapRecordToDto(record);
                            if (stockRecordDto == null)
                                continue;

                            var stockRecord = _dtoToModelsMapper.MapDtoToStockRecord(stockRecordDto);
                            if (stockRecord == null)
                                continue;

                            stockRecord.RecordId = Guid.NewGuid();
                            stockRecord.TickerSymbol = symbol;

                            _dbContext.StockRecords.Add(stockRecord);
                            addedCount++;
                        }
                    }
                }

                _logger.LogInformation("Processed {TotalCount} stock records for {Symbol}. Added {AddedCount} new records.",
                    records.Count(), symbol, addedCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching stock records for {Symbol}", symbol);
            }
        }

        private async Task FetchAndStoreFinancialReports(string symbol, CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogDebug("Fetching financial reports for {Symbol}", symbol);

                var financials = await _yahooService!.GetFinancialsAsync(symbol, stoppingToken);

                if (financials == null || financials.Count == 0)
                {
                    _logger.LogWarning("No financial reports returned for {Symbol}", symbol);
                    return;
                }

                int addedCount = 0;
                int updatedCount = 0;

                foreach (var kvp in financials)
                {
                    var reportLabel = kvp.Key;
                    var report = kvp.Value;

                    try
                    {
                        var existingReport = await _dbContext!.FinancialReports
                            .FirstOrDefaultAsync(f => f.TickerSymbol == symbol && f.ReportLabel == reportLabel, stoppingToken);

                        var reportDto = _externalToDtoMapper.MapFinancialReportToDto(report);
                        if (reportDto == null)
                        {
                            _logger.LogWarning("Failed to map external report to DTO for {Symbol} with label {Label}", symbol, reportLabel);
                            continue;
                        }

                        var financialReport = _dtoToModelsMapper.MapDtoToFinancialReport(reportDto);
                        if (financialReport == null)
                        {
                            _logger.LogWarning("Failed to map DTO to financial report model for {Symbol} with label {Label}", symbol, reportLabel);
                            continue;
                        }

                        financialReport.FinancialReportId = existingReport?.FinancialReportId ?? Guid.NewGuid();
                        financialReport.TickerSymbol = symbol;
                        financialReport.ReportLabel = reportLabel;

                        if (existingReport == null)
                        {
                            _dbContext.FinancialReports.Add(financialReport);
                            _logger.LogDebug("Added new financial report for {Symbol} with label {Label}", symbol, reportLabel);
                            addedCount++;
                        }
                        else if (ShouldUpdateReport(existingReport, financialReport))
                        {
                            existingReport.TotalRevenue = financialReport.TotalRevenue;
                            existingReport.CostOfRevenue = financialReport.CostOfRevenue;
                            existingReport.GrossProfit = financialReport.GrossProfit;
                            existingReport.OperatingExpense = financialReport.OperatingExpense;
                            existingReport.OperatingIncome = financialReport.OperatingIncome;
                            existingReport.NetNonOperatingInterestIncomeExpense = financialReport.NetNonOperatingInterestIncomeExpense;
                            existingReport.OtherIncomeExpense = financialReport.OtherIncomeExpense;
                            existingReport.PretaxIncome = financialReport.PretaxIncome;
                            existingReport.TaxProvision = financialReport.TaxProvision;
                            existingReport.NetIncomeCommonStockholders = financialReport.NetIncomeCommonStockholders;
                            existingReport.DilutedNIAvailableToComStockholders = financialReport.DilutedNIAvailableToComStockholders;
                            existingReport.BasicEPS = financialReport.BasicEPS;
                            existingReport.DilutedEPS = financialReport.DilutedEPS;
                            existingReport.BasicAverageShares = financialReport.BasicAverageShares;
                            existingReport.DilutedAverageShares = financialReport.DilutedAverageShares;
                            existingReport.TotalOperatingIncomeAsReported = financialReport.TotalOperatingIncomeAsReported;
                            existingReport.TotalExpenses = financialReport.TotalExpenses;
                            existingReport.NetIncomeFromContinuingAndDiscontinuedOperation = financialReport.NetIncomeFromContinuingAndDiscontinuedOperation;
                            existingReport.NormalizedIncome = financialReport.NormalizedIncome;
                            existingReport.InterestIncome = financialReport.InterestIncome;
                            existingReport.InterestExpense = financialReport.InterestExpense;
                            existingReport.NetInterestIncome = financialReport.NetInterestIncome;
                            existingReport.EBIT = financialReport.EBIT;
                            existingReport.EBITDA = financialReport.EBITDA;
                            existingReport.ReconciledCostOfRevenue = financialReport.ReconciledCostOfRevenue;
                            existingReport.ReconciledDepreciation = financialReport.ReconciledDepreciation;
                            existingReport.NetIncomeFromContinuingOperationNetMinorityInterest = financialReport.NetIncomeFromContinuingOperationNetMinorityInterest;
                            existingReport.TotalUnusualItemsExcludingGoodwill = financialReport.TotalUnusualItemsExcludingGoodwill;
                            existingReport.TotalUnusualItems = financialReport.TotalUnusualItems;
                            existingReport.NormalizedEBITDA = financialReport.NormalizedEBITDA;
                            existingReport.TaxRateForCalcs = financialReport.TaxRateForCalcs;
                            existingReport.TaxEffectOfUnusualItems = financialReport.TaxEffectOfUnusualItems;

                            _logger.LogDebug("Updated financial report for {Symbol} with label {Label}", symbol, reportLabel);
                            updatedCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing financial report {Label} for {Symbol}", reportLabel, symbol);
                    }
                }

                _logger.LogInformation("Processed {TotalCount} financial reports for {Symbol}. Added {AddedCount}, Updated {UpdatedCount}.",
                    financials.Count, symbol, addedCount, updatedCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching financial reports for {Symbol}", symbol);
            }
        }

        private bool ShouldUpdateReport(FinancialReport existing, FinancialReport updated)
        {
            return existing.TotalRevenue != updated.TotalRevenue ||
                   existing.NetIncomeCommonStockholders != updated.NetIncomeCommonStockholders ||
                   existing.BasicEPS != updated.BasicEPS;
        }

    }
}