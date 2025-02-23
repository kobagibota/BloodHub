using BloodHub.Api.Services;
using BloodHub.Shared.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodHub.Api.Controllers
{
    [Authorize(Policy = "ActiveUsersOnly")]
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController(IDoctorService doctorService) : ControllerBase
    {
        #region Private Members

        private readonly IDoctorService _doctorService = doctorService;

        #endregion Private Members

        #region Methods

        [HttpGet]
        public async Task<IActionResult> GetAllDoctors()
        {
            var response = await _doctorService.GetAll();
            return Ok(response);
        }

        [HttpGet("getact")]
        public async Task<IActionResult> GetActDoctors()
        {
            var response = await _doctorService.GetAct();
            return Ok(response);
        }

        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetDoctorById(int id)
        {
            var response = await _doctorService.GetById(id);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateDoctor([FromBody] DoctorRequest doctor)
        {
            var response = await _doctorService.Add(doctor);
            if (!response.Success || response.Data == null)
            {
                return BadRequest(response);
            }

            return CreatedAtAction(nameof(GetDoctorById), new { id = response.Data.Id }, response.Data);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateDoctor(int id, [FromBody] DoctorRequest request)
        {
            var response = await _doctorService.Update(id, request);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [Authorize(Policy = "ManagerOrAdmin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var response = await _doctorService.Delete(id);
            if (!response.Success)
                return Forbid();

            return Ok(response);
        }

        #endregion
    }
}
