using AutoMapper;
using TC_Backend.Models;

namespace TC_Backend.DTOs.Mappers
{
    public class RoleMapper : Profile
    {
        public RoleMapper()
        {
            CreateMap<Role, RoleDTO>();
            CreateMap<RoleDTO, Role>()
                .ForMember(dest => dest.RoleId, opt => opt.Ignore());
        }
    }
}
