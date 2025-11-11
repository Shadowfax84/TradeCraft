using TC_Backend.DTOs;
using TC_Backend.Models;

namespace TC_Backend.Services.Interfaces
{
    public interface ICompanyListService
    {
        Task<IEnumerable<CompanyList>> GetAllCompaniesServ();
        Task<CompanyList?> GetCompanyByIdServ(Guid clid);
        Task<CompanyListDTO?> CreateCompanyServ(CompanyListDTO company);
        Task<bool> DeleteCompanyServ(Guid clid);
    }
}