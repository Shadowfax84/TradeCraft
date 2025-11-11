using TC_Backend.Models;

namespace TC_Backend.Interfaces
{
    public interface IRoleRepo
    {
        Task<IEnumerable<Role>> GetAllRoles();
        Task<Role?> GetRoleById(int id);
        Task<Role> AddRole(Role role);
        Task<Role> UpdateRole(Role role);
        Task<bool?> DeleteRole(int id);
    }
}