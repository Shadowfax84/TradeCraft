using Microsoft.EntityFrameworkCore;
using TC_Backend.Data;
using TC_Backend.Interfaces;
using TC_Backend.Models;

namespace TC_Backend.Repositories
{
    public class CompanyListRepo : ICompanyListRepo
    {
        private readonly TC_BackendDbContext _dbcontext;
        private readonly ILogger<CompanyListRepo> _logger;

        public CompanyListRepo(TC_BackendDbContext dbcontext, ILogger<CompanyListRepo> logger)
        {
            _dbcontext = dbcontext;
            _logger = logger;
        }

        public async Task<IEnumerable<CompanyList>> GetAllCompanies()
        {
            _logger.LogInformation("Fetching all companies from database");
            return await _dbcontext.CompanysList.ToListAsync();
        }

        public async Task<CompanyList?> GetCompanyById(Guid clid)
        {
            _logger.LogInformation("Fetching company by ClId: {ClId}", clid);
            return await _dbcontext.CompanysList.FirstOrDefaultAsync(c => c.ClId == clid);
        }

        public async Task<CompanyList?> CreateCompany(CompanyList company)
        {
            _dbcontext.CompanysList.Add(company);
            await _dbcontext.SaveChangesAsync();
            _logger.LogInformation("Created company: {CompanyName} with Ticker: {TickerSymbol}",
                company.CompanyName, company.TickerSymbol);
            return company;
        }

        public async Task<bool> DeleteCompany(Guid clid)
        {
            var companyToDelete = await _dbcontext.CompanysList.FindAsync(clid);
            if (companyToDelete == null)
            {
                _logger.LogWarning("Attempt to delete non-existent company with ClId: {ClId}", clid);
                return false;
            }

            _dbcontext.CompanysList.Remove(companyToDelete);
            await _dbcontext.SaveChangesAsync();
            _logger.LogInformation("Deleted company: {CompanyName}", companyToDelete.CompanyName);
            return true;
        }
    }
}
