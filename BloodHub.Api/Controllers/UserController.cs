using BloodHub.Api.Services;
using BloodHub.Shared.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodHub.Api.Controllers
{
    [Authorize(Policy = "ManagerOrAdmin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        #region Private Members

        private readonly IUserService _userService = userService;

        #endregion Private Members

        #region Methods

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _userService.GetAll();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var result = await _userService.GetById(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [Authorize(Policy = "ActiveUsersOnly")]
        [HttpGet("available-users-for-shift")]
        public async Task<IActionResult> GetAvailableUsersForShift()
        {
            var result = await _userService.GetAvailableUsersForShift();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] UserRequest request)
        {
            var result = await _userService.Add(request);
            if (!result.Success || result.Data == null)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetUserById), new { id = result.Data.Id }, result.Data);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserRequest request)
        {
            var result = await _userService.Update(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }
                
        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.Delete(id);
            return result.Success ? Ok(result) : Forbid();
        }

        #endregion
    }
}
