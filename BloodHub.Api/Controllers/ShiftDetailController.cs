using BloodHub.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodHub.Api.Controllers
{
    [Authorize(Policy = "ActiveUsersOnly")]
    [Route("api/[controller]")]
    [ApiController]
    public class ShiftDetailController(IShiftDetailService shiftDetailService) : ControllerBase
    {
        #region Private Members

        private readonly IShiftDetailService _shiftDetailService = shiftDetailService;

        #endregion Private Members

        #region Methods

        [HttpGet("getlistbyshiftid/{shiftId}")]
        public async Task<IActionResult> GetShiftDetails(int shiftId)
        {
            var response = await _shiftDetailService.GetListByShiftId(shiftId);
            return Ok(response);
        }

        //[HttpGet("getbyid/{id}")]
        //public async Task<IActionResult> GetShiftById(int id)
        //{
        //    var response = await _shiftDetailService.GetById(id);
        //    if (!response.Success)
        //    {
        //        return BadRequest(response);
        //    }

        //    return Ok(response);
        //}

        //[HttpPost("create")]
        //public async Task<IActionResult> CreateShift([FromBody] ShiftAddRequest shift)
        //{
        //    var response = await _shiftDetailService.Add(shift);
        //    if (!response.Success || response.Data == null)
        //    {
        //        return BadRequest(response);
        //    }

        //    return CreatedAtAction(nameof(GetShiftById), new { id = response.Data.Id }, response.Data);
        //}

        //[HttpPut("update/{id}")]
        //public async Task<IActionResult> UpdateShift(int id, [FromBody] ShiftAddRequest request)
        //{
        //    var response = await _shiftDetailService.Update(id, request);
        //    if (!response.Success)
        //        return BadRequest(response);

        //    return Ok(response);
        //}

        //[HttpPut("handover/{id}")]
        //public async Task<IActionResult> Handover(int id, [FromBody] ShiftHandoverRequest request)
        //{
        //    var response = await _shiftDetailService.Handover(id, request);
        //    if (!response.Success)
        //        return BadRequest(response);

        //    return Ok(response);
        //}

        //[HttpPut("confirmhandover/{id}")]
        //public async Task<IActionResult> ConfirmHandover(int id, [FromBody] ShiftConfirmHandoverRequest request)
        //{
        //    var response = await _shiftDetailService.ConfirmHandover(id, request);
        //    if (!response.Success)
        //        return BadRequest(response);

        //    return Ok(response);
        //}

        //[HttpDelete("delete/{id}")]
        //public async Task<IActionResult> DeleteShift(int id)
        //{
        //    var response = await _shiftDetailService.Delete(id);
        //    if (!response.Success)
        //        return Forbid();

        //    return Ok(response);
        //}

        #endregion
    }
}
