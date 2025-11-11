using Microsoft.AspNetCore.Mvc;
using TC_Backend.DTOs;
using TC_Backend.Models;
using TC_Backend.Services.Interfaces;

namespace TC_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService, ILogger<RoleController> logger)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Role>>> GetAllRoles()
        {
            var roles = await _roleService.GetAllRolesServ();
            return Ok(roles);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<RoleDTO>> GetRoleById(int id)
        {
            var role = await _roleService.GetRoleByIdServ(id);
            if (role == null)
                throw new KeyNotFoundException($"Role with ID {id} not found.");

            return Ok(role);
        }

        [HttpPost]
        public async Task<ActionResult<RoleDTO>> CreateRole([FromBody] RoleDTO roleDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdRole = await _roleService.AddRoleServ(roleDto);
            return Ok(createdRole);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Role>> UpdateRole(int id, [FromBody] RoleDTO roleDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedRole = await _roleService.UpdateRoleServ(id, roleDto);
            return Ok(updatedRole);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<bool?>> DeleteRole(int id)
        {
            var deleted = await _roleService.DeleteRoleServ(id);
            return Ok(deleted);
        }
    }
}
