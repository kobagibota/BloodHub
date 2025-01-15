using BloodHub.Api.Services;
using BloodHub.Shared.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodHub.Api.Controllers
{
    [Authorize(Policy = "ActiveUsersOnly")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IProductService productService) : ControllerBase
    {
        #region Private Members

        private readonly IProductService _productService = productService;

        #endregion Private Members

        #region Methods

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var response = await _productService.GetAll();
            return Ok(response);
        }

        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var response = await _productService.GetById(id);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductRequest product)
        {
            var response = await _productService.Add(product);
            if (!response.Success || response.Data == null)
            {
                return BadRequest(response);
            }

            return CreatedAtAction(nameof(GetProductById), new { id = response.Data.Id }, response.Data);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductRequest request)
        {
            var response = await _productService.Update(id, request);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [Authorize(Policy = "ManagerOrAdmin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var response = await _productService.Delete(id);
            if (!response.Success)
                return Forbid();

            return Ok(response);
        }

        #endregion
    }
}
