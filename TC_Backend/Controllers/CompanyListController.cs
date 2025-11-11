using Microsoft.AspNetCore.Mvc;
using TC_Backend.DTOs;
using TC_Backend.Services.Interfaces;
using TC_Backend.Models;

namespace TC_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyListController : ControllerBase
    {
        private readonly ICompanyListService _companyListService;
        private readonly ILogger<CompanyListController> _logger;

        public CompanyListController(ICompanyListService companyListService, ILogger<CompanyListController> logger)
        {
            _companyListService = companyListService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompanyList>>> GetAllCompanies()
        {
            _logger.LogInformation("GetAllCompanies endpoint called");
            var companies = await _companyListService.GetAllCompaniesServ();
            return Ok(companies);
        }

        [HttpPost]
        public async Task<ActionResult<CompanyListDTO>> CreateCompany([FromBody] CompanyListDTO companyDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _logger.LogInformation("CreateCompany endpoint called for: {CompanyName}", companyDto.CompanyName);

            var createdCompany = await _companyListService.CreateCompanyServ(companyDto);

            if (createdCompany == null)
            {
                _logger.LogWarning("Failed to create company: {CompanyName}", companyDto.CompanyName);
                throw new InvalidOperationException("Failed to create company.");
            }

            return Ok(createdCompany);
        }

        [HttpGet("{clid:guid}")]
        public async Task<ActionResult<CompanyList?>> GetCompanyById(Guid clid)
        {
            _logger.LogInformation("GetCompanyById endpoint called for ClId: {ClId}", clid);

            var company = await _companyListService.GetCompanyByIdServ(clid);
            if (company == null)
            {
                _logger.LogWarning("Company not found with ClId: {ClId}", clid);
                throw new KeyNotFoundException($"Company with ID {clid} not found.");
            }

            return Ok(company);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteCompany(Guid id)
        {
            _logger.LogInformation("DeleteCompany endpoint called for ClId: {ClId}", id);

            var deleted = await _companyListService.DeleteCompanyServ(id);
            if (!deleted)
            {
                _logger.LogWarning("Company not found for deletion with ClId: {ClId}", id);
                throw new KeyNotFoundException($"Company with ID {id} not found.");
            }

            _logger.LogInformation("Company deleted successfully: {ClId}", id);
            return Ok(new { Message = "Company deleted successfully" });
        }
    }
}
