using TC_Backend.DTOs.ExternalDTOs;
using TC_Backend.Models;

namespace TC_Backend.BackgroundServices.Helpers
{
    public class DtoToModelsMapper
    {
        private readonly ILogger<DtoToModelsMapper> _logger;

        public DtoToModelsMapper(ILogger<DtoToModelsMapper> logger)
        {
            _logger = logger;
        }

        public CompanyProfile? MapDtoToCompanyProfile(CompanyProfileDTO? dto)
        {
            try
            {
                _logger.LogDebug("Starting mapping from CompanyProfileDTO to CompanyProfile model");

                if (dto == null)
                {
                    _logger.LogWarning("CompanyProfileDTO is null, returning null");
                    return null;
                }

                var model = new CompanyProfile
                {
                    Address = dto.Address,
                    Phone = dto.Phone,
                    Website = dto.Website,
                    Sector = dto.Sector,
                    Industry = dto.Industry,
                    CntEmployees = dto.CntEmployees,
                    Description = dto.Description
                };

                _logger.LogDebug("Successfully mapped CompanyProfileDTO to CompanyProfile model");
                return model;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error mapping CompanyProfileDTO to CompanyProfile model");
                return null;
            }
        }

        public FinancialReport? MapDtoToFinancialReport(FinancialReportDTO? dto)
        {
            try
            {
                _logger.LogDebug("Starting mapping from FinancialReportDTO to FinancialReport model");

                if (dto == null)
                {
                    _logger.LogWarning("FinancialReportDTO is null, returning null");
                    return null;
                }

                var model = new FinancialReport
                {
                    TotalRevenue = dto.TotalRevenue,
                    CostOfRevenue = dto.CostOfRevenue,
                    GrossProfit = dto.GrossProfit,
                    OperatingExpense = dto.OperatingExpense,
                    OperatingIncome = dto.OperatingIncome,
                    NetNonOperatingInterestIncomeExpense = dto.NetNonOperatingInterestIncomeExpense,
                    OtherIncomeExpense = dto.OtherIncomeExpense,
                    PretaxIncome = dto.PretaxIncome,
                    TaxProvision = dto.TaxProvision,
                    NetIncomeCommonStockholders = dto.NetIncomeCommonStockholders,
                    DilutedNIAvailableToComStockholders = dto.DilutedNIAvailableToComStockholders,
                    BasicEPS = dto.BasicEPS,
                    DilutedEPS = dto.DilutedEPS,
                    BasicAverageShares = dto.BasicAverageShares,
                    DilutedAverageShares = dto.DilutedAverageShares,
                    TotalOperatingIncomeAsReported = dto.TotalOperatingIncomeAsReported,
                    TotalExpenses = dto.TotalExpenses,
                    NetIncomeFromContinuingAndDiscontinuedOperation = dto.NetIncomeFromContinuingAndDiscontinuedOperation,
                    NormalizedIncome = dto.NormalizedIncome,
                    InterestIncome = dto.InterestIncome,
                    InterestExpense = dto.InterestExpense,
                    NetInterestIncome = dto.NetInterestIncome,
                    EBIT = dto.EBIT,
                    EBITDA = dto.EBITDA,
                    ReconciledCostOfRevenue = dto.ReconciledCostOfRevenue,
                    ReconciledDepreciation = dto.ReconciledDepreciation,
                    NetIncomeFromContinuingOperationNetMinorityInterest = dto.NetIncomeFromContinuingOperationNetMinorityInterest,
                    TotalUnusualItemsExcludingGoodwill = dto.TotalUnusualItemsExcludingGoodwill,
                    TotalUnusualItems = dto.TotalUnusualItems,
                    NormalizedEBITDA = dto.NormalizedEBITDA,
                    TaxRateForCalcs = dto.TaxRateForCalcs,
                    TaxEffectOfUnusualItems = dto.TaxEffectOfUnusualItems
                };

                _logger.LogDebug("Successfully mapped FinancialReportDTO to FinancialReport model");
                return model;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error mapping FinancialReportDTO to FinancialReport model");
                return null;
            }
        }

        public StockRecord? MapDtoToStockRecord(StockRecordDTO? dto)
        {
            try
            {
                _logger.LogDebug("Starting mapping from StockRecordDTO to StockRecord model");

                if (dto == null)
                {
                    _logger.LogWarning("StockRecordDTO is null, returning null");
                    return null;
                }

                var model = new StockRecord
                {
                    Date = dto.Date,
                    Open = dto.Open,
                    High = dto.High,
                    Low = dto.Low,
                    Close = dto.Close,
                    AdjustedClose = dto.AdjustedClose,
                    Volume = dto.Volume
                };

                _logger.LogDebug("Successfully mapped StockRecordDTO to StockRecord model");
                return model;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error mapping StockRecordDTO to StockRecord model");
                return null;
            }
        }
    }
}
