using TC_Backend.DTOs;
using TC_Backend.Interfaces;
using TC_Backend.Models;
using TC_Backend.Services.Interfaces;

namespace TC_Backend.Services
{
    public class CompanyListService : ICompanyListService
    {
        private readonly ICompanyListRepo _companyListRepo;
        private readonly ILogger<CompanyListService> _logger;

        public CompanyListService(ICompanyListRepo companyListRepo, ILogger<CompanyListService> logger)
        {
            _companyListRepo = companyListRepo;
            _logger = logger;
        }

        public async Task<IEnumerable<CompanyList>> GetAllCompaniesServ()
        {
            _logger.LogInformation("Retrieving all companies");
            return await _companyListRepo.GetAllCompanies();
        }

        public async Task<CompanyListDTO?> CreateCompanyServ(CompanyListDTO companyDto)
        {
            _logger.LogInformation("Creating company: {CompanyName}", companyDto.CompanyName);

            var companyEntity = new CompanyList
            {
                ClId = Guid.NewGuid(),
                CompanyName = companyDto.CompanyName,
                TickerSymbol = companyDto.TickerSymbol
            };

            var createdEntity = await _companyListRepo.CreateCompany(companyEntity);
            if (createdEntity == null)
            {
                _logger.LogError("Failed to create company: {CompanyName}", companyDto.CompanyName);
                return null;
            }
            var createdDto = new CompanyListDTO
            {
                CompanyName = createdEntity.CompanyName,
                TickerSymbol = createdEntity.TickerSymbol
            };

            _logger.LogInformation("Company created successfully: {CompanyName}", createdDto.CompanyName);
            return createdDto;
        }

        public async Task<CompanyList?> GetCompanyByIdServ(Guid clid)
        {
            _logger.LogInformation("Retrieving company by ClId: {ClId}", clid);
            return await _companyListRepo.GetCompanyById(clid);
        }

        public async Task<bool> DeleteCompanyServ(Guid clid)
        {
            _logger.LogInformation("Deleting company with ClId: {ClId}", clid);

            var result = await _companyListRepo.DeleteCompany(clid);

            if (result)
                _logger.LogInformation("Company deleted successfully: {ClId}", clid);
            else
                _logger.LogWarning("Delete failed. Company not found: {ClId}", clid);

            return result;
        }
    }
}
