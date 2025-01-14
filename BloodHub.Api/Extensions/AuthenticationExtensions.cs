using BloodHub.Data.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BloodHub.Api.Extensions
{

    public static class AuthenticationExtensions
    {
        // Cấu hình JWT Authentication
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSection = configuration.GetSection("Jwt");
            services.Configure<JwtSettings>(jwtSection);

            var jwtSettings = jwtSection.Get<JwtSettings>();
            if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.SecurityKey))
            {
                throw new InvalidOperationException("JWT SecurityKey is not configured.");
            }

            var key = Encoding.UTF8.GetBytes(jwtSettings.SecurityKey);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwtSettings.ValidIssuer,
                    ValidAudience = jwtSettings.ValidAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };
            });

            return services;
        }


        public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                // Policy yêu cầu user phải có role "Admin"
                options.AddPolicy("AdminOnly", policy =>
                {
                    policy.RequireRole("Admin");
                });

                // Policy yêu cầu user phải có claim "IsActive" với giá trị "True"
                options.AddPolicy("ActiveUsersOnly", policy =>
                {
                    policy.RequireClaim("IsActive", "True");
                });

                // Policy cho phép user thuộc role "Manager" hoặc "Admin"
                options.AddPolicy("ManagerOrAdmin", policy =>
                {
                    policy.RequireRole("Manager", "Admin");
                });
            });

            return services;
        }
    }
}
