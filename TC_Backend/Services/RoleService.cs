using TC_Backend.DTOs;
using TC_Backend.Interfaces;
using TC_Backend.Models;
using TC_Backend.Services.Interfaces;

namespace TC_Backend.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepo _roleRepo;
        private readonly ILogger<RoleService> _logger;

        public RoleService(IRoleRepo roleRepo, ILogger<RoleService> logger)
        {
            _roleRepo = roleRepo;
            _logger = logger;
        }

        public async Task<IEnumerable<Role>> GetAllRolesServ()
        {
            _logger.LogInformation("Retrieving all roles");
            return await _roleRepo.GetAllRoles();
        }

        public async Task<RoleDTO?> AddRoleServ(RoleDTO roleDto)
        {
            var roleEntity = new Role
            {
                RoleName = roleDto.RoleName,
                RequiredPoints = roleDto.RequiredPoints,
                RoleLevel = roleDto.RoleLevel
            };

            var newRole = await _roleRepo.AddRole(roleEntity);
            _logger.LogInformation("Added roel: {RoleName}", newRole.RoleName);

            return new RoleDTO
            {
                RoleName = newRole.RoleName,
                RequiredPoints = newRole.RequiredPoints,
                RoleLevel = newRole.RoleLevel
            };
        }

        public async Task<Role?> GetRoleByIdServ(int id)
        {
            _logger.LogInformation("Retrieving role by ID: {Id}", id);
            return await _roleRepo.GetRoleById(id);
        }

        public async Task<Role?> UpdateRoleServ(int id, RoleDTO roleDto)
        {
            var existingRole = await _roleRepo.GetRoleById(id);
            if (existingRole == null)
            {
                _logger.LogWarning("Role with ID {RoleId} not found for update", id);
                throw new KeyNotFoundException($"Role with ID {id} not found.");
            }

            existingRole.RoleName = roleDto.RoleName;
            existingRole.RequiredPoints = roleDto.RequiredPoints;
            existingRole.RoleLevel = roleDto.RoleLevel;
            _logger.LogInformation("Updated role: {RoleName}", existingRole.RoleName);

            return await _roleRepo.UpdateRole(existingRole);
        }

        public async Task<bool?> DeleteRoleServ(int id)
        {
            _logger.LogInformation("Deleting role with ID: {Id}", id);
            return await _roleRepo.DeleteRole(id);
        }
    }
}
