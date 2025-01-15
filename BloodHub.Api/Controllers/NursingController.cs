using BloodHub.Api.Services;
using BloodHub.Shared.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodHub.Api.Controllers
{
    [Authorize(Policy = "ActiveUsersOnly")]
    [Route("api/[controller]")]
    [ApiController]
    public class NursingController(INursingService nursingService) : ControllerBase
    {
        #region Private Members

        private readonly INursingService _nursingService = nursingService;

        #endregion Private Members

        #region Methods

        [HttpGet]
        public async Task<IActionResult> GetAllNursings()
        {
            var response = await _nursingService.GetAll();
            return Ok(response);
        }

        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetNursingById(int id)
        {
            var response = await _nursingService.GetById(id);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateNursing([FromBody] NursingRequest nursing)
        {
            var response = await _nursingService.Add(nursing);
            if (!response.Success || response.Data == null)
            {
                return BadRequest(response);
            }

            return CreatedAtAction(nameof(GetNursingById), new { id = response.Data.Id }, response.Data);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateNursing(int id, [FromBody] NursingRequest request)
        {
            var response = await _nursingService.Update(id, request);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [Authorize(Policy = "ManagerOrAdmin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteNursing(int id)
        {
            var response = await _nursingService.Delete(id);
            if (!response.Success)
                return Forbid();

            return Ok(response);
        }

        #endregion
    }
}
