using TC_Backend.Models;
using TC_Backend.DTOs;

namespace TC_Backend.Services.Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<Role>> GetAllRolesServ();
        Task<Role?> GetRoleByIdServ(int id);
        Task<RoleDTO?> AddRoleServ(RoleDTO roleDto);
        Task<Role?> UpdateRoleServ(int id, RoleDTO roleDto);
        Task<bool?> DeleteRoleServ(int id);
    }
}
