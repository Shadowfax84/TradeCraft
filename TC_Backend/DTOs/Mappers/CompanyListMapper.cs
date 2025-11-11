using AutoMapper;
using TC_Backend.Models;
namespace TC_Backend.DTOs.Mappers
{
    public class CompanyListMapper : Profile
    {
        public CompanyListMapper()
        {
            CreateMap<CompanyList, CompanyListDTO>();
            CreateMap<CompanyListDTO, CompanyList>()
                .ForMember(ignr => ignr.ClId, opt => opt.Ignore());
        }
    }
}