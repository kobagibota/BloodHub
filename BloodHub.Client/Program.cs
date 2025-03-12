using BloodHub.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.LocalStorage;
using BloodHub.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using BloodHub.Client.Helpers;
using MudBlazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient
// builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7070") });

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopCenter;

    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = true;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 5000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();

// Add ThemeService
builder.Services.AddScoped<ThemeService>();

// Add Authentication Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

// Add HttpClientHelper
builder.Services.AddScoped<HttpClientHelper>();

// Add HttpClient with AutoRefreshTokenHandler
builder.Services.AddTransient<AutoRefreshTokenHandler>();
builder.Services.AddHttpClient("AuthorizedClient")
    .AddHttpMessageHandler<AutoRefreshTokenHandler>();

// Add AppService
builder.Services.AddScoped<DoctorService>();
builder.Services.AddScoped<NursingService>();
builder.Services.AddScoped<WardService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<PatientService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<ShiftService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<RoleService>();

await builder.Build().RunAsync();
