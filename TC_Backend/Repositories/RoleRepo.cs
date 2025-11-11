using Microsoft.EntityFrameworkCore;
using TC_Backend.Data;
using TC_Backend.Interfaces;
using TC_Backend.Models;

namespace TC_Backend.Repositories
{
    public class RoleRepo : IRoleRepo
    {
        private readonly TC_BackendDbContext _dbContext;
        private readonly ILogger<RoleRepo> _logger;

        public RoleRepo(TC_BackendDbContext dbContext, ILogger<RoleRepo> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IEnumerable<Role>> GetAllRoles()
        {
            return await _dbContext.Roles.ToListAsync();
        }

        public async Task<Role?> GetRoleById(int id)
        {
            return await _dbContext.Roles.FirstOrDefaultAsync(r => r.RoleId == id);
        }

        public async Task<Role> AddRole(Role role)
        {
            _dbContext.Roles.Add(role);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Added new role: {RoleName}", role.RoleName);
            return role;
        }

        public async Task<bool?> DeleteRole(int id)
        {
            var role = await _dbContext.Roles.FindAsync(id);
            if (role == null)
            {
                _logger.LogWarning("Attempt to delete non-existent role with ID {RoleId}", id);
                return null;
            }

            _dbContext.Roles.Remove(role);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Deleted role: {RoleName}", role.RoleName);
            return true;
        }

        public async Task<Role> UpdateRole(Role role)
        {
            var existingRole = await _dbContext.Roles.FindAsync(role.RoleId);
            if (existingRole == null)
            {
                _logger.LogWarning("Attempt to update non-existent role with ID {RoleId}", role.RoleId);
                throw new KeyNotFoundException($"Role with ID {role.RoleId} does not exist.");
            }

            _dbContext.Roles.Update(role);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Updated role: {RoleId}", role.RoleId);
            return existingRole;
        }
    }
}
