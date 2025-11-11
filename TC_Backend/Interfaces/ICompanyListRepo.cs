using TC_Backend.Models;

namespace TC_Backend.Interfaces
{
    public interface ICompanyListRepo
    {
        Task<IEnumerable<CompanyList>> GetAllCompanies();
        Task<CompanyList?> GetCompanyById(Guid clid);
        Task<CompanyList?> CreateCompany(CompanyList company);
        Task<bool> DeleteCompany(Guid clid);
    }
}