using BloodHub.Api.Services;
using BloodHub.Shared.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodHub.Api.Controllers
{
    [Authorize(Policy = "ActiveUsersOnly")]
    [Route("api/[controller]")]
    [ApiController]
    public class ShiftController(IShiftService shiftService) : ControllerBase
    {
        #region Private Members

        private readonly IShiftService _shiftService = shiftService;

        #endregion Private Members

        #region Methods

        [HttpGet]
        public async Task<IActionResult> GetAllShifts()
        {
            var response = await _shiftService.GetAll();
            return Ok(response);
        }

        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetShiftById(int id)
        {
            var response = await _shiftService.GetById(id);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateShift([FromBody] ShiftRequest request)
        {
            var response = await _shiftService.Add(request);
            if (!response.Success || response.Data == null)
            {
                return BadRequest(response);
            }

            return CreatedAtAction(nameof(GetShiftById), new { id = response.Data.Id }, response.Data);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateShift(int id, [FromBody] ShiftRequest request)
        {
            var response = await _shiftService.Update(id, request);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPut("handover/{id}")]
        public async Task<IActionResult> Handover(int id, [FromBody] ShiftHandoverRequest request)
        {
            var response = await _shiftService.Handover(id, request);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPut("confirmhandover/{id}")]
        public async Task<IActionResult> ConfirmHandover(int id, [FromBody] ShiftConfirmHandoverRequest request)
        {
            var response = await _shiftService.ConfirmHandover(id, request);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteShift(int id)
        {
            var response = await _shiftService.Delete(id);
            if (!response.Success)
                return Forbid();

            return Ok(response);
        }

        #endregion
    }
}
