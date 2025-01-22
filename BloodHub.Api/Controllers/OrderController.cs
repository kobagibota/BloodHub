using BloodHub.Api.Services;
using BloodHub.Shared.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodHub.Api.Controllers
{
    [Authorize(Policy = "ActiveUsersOnly")]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController(IOrderService orderService) : ControllerBase
    {
        #region Private Members

        private readonly IOrderService _orderService = orderService;

        #endregion Private Members

        #region Methods

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var response = await _orderService.GetAll();
            return Ok(response);
        }

        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var response = await _orderService.GetById(id);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("getbypatientid/{id}")]
        public async Task<IActionResult> GetOrderByPatientId(int id)
        {
            var response = await _orderService.GetByPatientId(id);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequest order)
        {
            var response = await _orderService.Add(order);
            if (!response.Success || response.Data == null)
            {
                return BadRequest(response);
            }

            return CreatedAtAction(nameof(GetOrderById), new { id = response.Data.Id }, response.Data);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderRequest request)
        {
            var response = await _orderService.Update(id, request);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var response = await _orderService.Delete(id);
            if (!response.Success)
                return Forbid();

            return Ok(response);
        }

        #endregion
    }
}
