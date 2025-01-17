using BloodHub.Api.Extensions;
using BloodHub.Api.Services;
using BloodHub.Data.Data;
using BloodHub.Data.Repositories;
using BloodHub.Shared.Entities;
using BloodHub.Shared.Interfaces;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.OpenApi.Models;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Settings for DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString)
                .ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning)));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// Setting for Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BloodHub API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Settings for Identity
builder.Services.AddIdentity<User, Role>(options =>
{
    options.User.RequireUniqueEmail = false;
    options.SignIn.RequireConfirmedEmail = false; 
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Settings Authentication and Authorization
builder.Services.AddAuthorizationPolicies();
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddHttpContextAccessor();

// Settings CORS
builder.Services.AddCors(option =>
{
    option.AddPolicy("AllowBlazorWasm", builder => builder
    .WithOrigins("https://localhost:17133", "https://localhost:7050")
    .AllowAnyMethod().AllowAnyHeader().AllowCredentials());
});

// Add application services.
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthTokenService, AuthTokenService>();
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IChangeLogService, ChangeLogService>();      // Log
builder.Services.AddScoped<RequestInfoProvider>();                      // RequestInfoProvider get UserId, IPAddress, UserAgent from Token

builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<INursingService, NursingService>();
builder.Services.AddScoped<IWardService, WardService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IPatientService, PatientService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) 
{ 
    app.UseDeveloperExceptionPage(); 
    app.UseSwagger(); 
    app.UseSwaggerUI(); 
} 
else 
{ 
    app.UseExceptionHandler(options => 
    { 
        options.Run(async context => 
        { 
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json"; 
            var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>(); 
            if (exceptionHandlerPathFeature != null)
            { 
                var error = new 
                { 
                    context.Response.StatusCode, 
                    Message = "An unexpected error occurred. Please try again later.", 
                    Detailed = exceptionHandlerPathFeature.Error.Message 
                }; 
                await context.Response.WriteAsJsonAsync(error); 
            } 
        }); 
    }); 
    app.UseHsts(); 
}
app.UseHttpsRedirection();

app.UseCors("AllowBlazorWasm");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
