using Finance.Net.Models.Yahoo;
using TC_Backend.DTOs.ExternalDTOs;

namespace TC_Backend.BackgroundServices.Helpers
{
    public class ExternalToDtoMapper
    {
        private readonly ILogger<ExternalToDtoMapper> _logger;

        public ExternalToDtoMapper(ILogger<ExternalToDtoMapper> logger)
        {
            _logger = logger;
        }

        public CompanyProfileDTO? MapProfileToDto(Profile? externalProfile)
        {
            try
            {
                _logger.LogDebug("Starting mapping from external Profile to CompanyProfileDTO");

                if (externalProfile == null)
                {
                    _logger.LogWarning("External Profile is null, returning null");
                    return null;
                }

                var dto = new CompanyProfileDTO
                {
                    Address = externalProfile.Adress,
                    Phone = externalProfile.Phone,
                    Website = externalProfile.Website,
                    Sector = externalProfile.Sector,
                    Industry = externalProfile.Industry,
                    CntEmployees = externalProfile.CntEmployees,
                    Description = externalProfile.Description
                };

                _logger.LogDebug("Successfully mapped external Profile to CompanyProfileDTO");
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error mapping external Profile to CompanyProfileDTO");
                return null;
            }
        }

        public FinancialReportDTO? MapFinancialReportToDto(FinancialReport? externalReport)
        {
            try
            {
                _logger.LogDebug("Starting mapping from external FinancialReport to FinancialReportDTO");

                if (externalReport == null)
                {
                    _logger.LogWarning("External FinancialReport is null, returning null");
                    return null;
                }

                var dto = new FinancialReportDTO
                {
                    TotalRevenue = externalReport.TotalRevenue,
                    CostOfRevenue = externalReport.CostOfRevenue,
                    GrossProfit = externalReport.GrossProfit,
                    OperatingExpense = externalReport.OperatingExpense,
                    OperatingIncome = externalReport.OperatingIncome,
                    NetNonOperatingInterestIncomeExpense = externalReport.NetNonOperatingInterestIncomeExpense,
                    OtherIncomeExpense = externalReport.OtherIncomeExpense,
                    PretaxIncome = externalReport.PretaxIncome,
                    TaxProvision = externalReport.TaxProvision,
                    NetIncomeCommonStockholders = externalReport.NetIncomeCommonStockholders,
                    DilutedNIAvailableToComStockholders = externalReport.DilutedNIAvailableToComStockholders,
                    BasicEPS = externalReport.BasicEPS,
                    DilutedEPS = externalReport.DilutedEPS,
                    BasicAverageShares = externalReport.BasicAverageShares,
                    DilutedAverageShares = externalReport.DilutedAverageShares,
                    TotalOperatingIncomeAsReported = externalReport.TotalOperatingIncomeAsReported,
                    TotalExpenses = externalReport.TotalExpenses,
                    NetIncomeFromContinuingAndDiscontinuedOperation = externalReport.NetIncomeFromContinuingAndDiscontinuedOperation,
                    NormalizedIncome = externalReport.NormalizedIncome,
                    InterestIncome = externalReport.InterestIncome,
                    InterestExpense = externalReport.InterestExpense,
                    NetInterestIncome = externalReport.NetInterestIncome,
                    EBIT = externalReport.EBIT,
                    EBITDA = externalReport.EBITDA,
                    ReconciledCostOfRevenue = externalReport.ReconciledCostOfRevenue,
                    ReconciledDepreciation = externalReport.ReconciledDepreciation,
                    NetIncomeFromContinuingOperationNetMinorityInterest = externalReport.NetIncomeFromContinuingOperationNetMinorityInterest,
                    TotalUnusualItemsExcludingGoodwill = externalReport.TotalUnusualItemsExcludingGoodwill,
                    TotalUnusualItems = externalReport.TotalUnusualItems,
                    NormalizedEBITDA = externalReport.NormalizedEBITDA,
                    TaxRateForCalcs = externalReport.TaxRateForCalcs,
                    TaxEffectOfUnusualItems = externalReport.TaxEffectOfUnusualItems
                };

                _logger.LogDebug("Successfully mapped external FinancialReport to FinancialReportDTO");
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error mapping external FinancialReport to FinancialReportDTO");
                return null;
            }
        }

        public StockRecordDTO? MapRecordToDto(Record? externalRecord)
        {
            try
            {
                _logger.LogDebug("Starting mapping from external Record to StockRecordDTO");

                if (externalRecord == null)
                {
                    _logger.LogWarning("External Record is null, returning null");
                    return null;
                }

                var dto = new StockRecordDTO
                {
                    Date = externalRecord.Date,
                    Open = externalRecord.Open,
                    High = externalRecord.High,
                    Low = externalRecord.Low,
                    Close = externalRecord.Close,
                    AdjustedClose = externalRecord.AdjustedClose,
                    Volume = externalRecord.Volume
                };

                _logger.LogDebug("Successfully mapped external Record to StockRecordDTO");
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error mapping external Record to StockRecordDTO");
                return null;
            }
        }
    }
}
