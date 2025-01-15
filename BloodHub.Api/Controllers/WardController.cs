using BloodHub.Api.Services;
using BloodHub.Shared.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodHub.Api.Controllers
{
    [Authorize(Policy = "ActiveUsersOnly")]
    [Route("api/[controller]")]
    [ApiController]
    public class WardController(IWardService wardService) : ControllerBase
    {
        #region Private Members

        private readonly IWardService _wardService = wardService;

        #endregion Private Members

        #region Methods

        [HttpGet]
        public async Task<IActionResult> GetAllWards()
        {
            var response = await _wardService.GetAll();
            return Ok(response);
        }

        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetWardById(int id)
        {
            var response = await _wardService.GetById(id);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateWard([FromBody] WardRequest ward)
        {
            var response = await _wardService.Add(ward);
            if (!response.Success || response.Data == null)
            {
                return BadRequest(response);
            }

            return CreatedAtAction(nameof(GetWardById), new { id = response.Data.Id }, response.Data);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateWard(int id, [FromBody] WardRequest request)
        {
            var response = await _wardService.Update(id, request);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [Authorize(Policy = "ManagerOrAdmin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteWard(int id)
        {
            var response = await _wardService.Delete(id);
            if (!response.Success)
                return Forbid();

            return Ok(response);
        }

        #endregion
    }
}
