using System.Security.Claims;

namespace BloodHub.Api.Extensions
{
    public class RequestInfoProvider(IHttpContextAccessor httpContextAccessor)
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public int GetUserIdFromToken() 
        { 
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null)
            {
                return 0;
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim == null ? 0 : int.Parse(userIdClaim.Value);
        }

        public string GetIpAddress() 
        {
            return _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown"; 
        }

        public string GetUserAgent()
        { 
            return _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString() ?? "Unknown"; 
        }
    }
}
