using BloodHub.Api.Services;
using BloodHub.Shared.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodHub.Api.Controllers
{
    [Authorize(Policy = "ManagerOrAdmin")]
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController(IRoleService roleService) : ControllerBase
    {
        #region Private Members

        private readonly IRoleService _roleService = roleService;

        #endregion Private Members

        #region Methods

        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var response = await _roleService.GetAll();
            return Ok(response);
        }

        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetRoleById(int id)
        {
            var response = await _roleService.GetById(id);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize(Policy = "Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            var response = await _roleService.Add(roleName);
            if (!response.Success || response.Data == null)
            {
                return BadRequest(response);
            }

            return CreatedAtAction(nameof(GetRoleById), new { id = response.Data.Id }, response.Data);
        }
          

        [Authorize(Policy = "Admin")]
        [HttpDelete("delete/{roleName}")]
        public async Task<IActionResult> DeleteRole(string roleName)
        {
            var response = await _roleService.Delete(roleName);
            if (!response.Success)
                return Forbid();

            return Ok(response);
        }

        #endregion
    }
}
