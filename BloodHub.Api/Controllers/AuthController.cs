using BloodHub.Api.Services;
using BloodHub.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BloodHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        #region Private Members

        private readonly IAuthService _authService = authService;

        #endregion Private Members

        #region Methods

        [AllowAnonymous] // Không yêu cầu xác thực
        [HttpPost("login")] 
        public async Task<IActionResult> Login([FromBody] LoginDto request) 
        { 
            var response = await _authService.LoginAsync(request);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response); 
        }

        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto request)
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var response = await _authService.ChangePassword(userId, request);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        // Đăng xuất - Yêu cầu xác thực
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var response = await _authService.LogoutAsync(userId, accessToken);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        // Làm mới token - Không yêu cầu xác thực
        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var response = await _authService.RefreshTokenAsync(refreshToken);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        // Thu hồi tất cả token của người dùng - Yêu cầu xác thực
        [Authorize]
        [HttpPost("revoke-all-tokens")]
        public async Task<IActionResult> RevokeAllTokens()
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

            var response = await _authService.RevokeAllTokensAsync(userId);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        #endregion

    }
}
