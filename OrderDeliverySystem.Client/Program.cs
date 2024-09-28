using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OrderDeliverySystem.Client.Infrastructure.Services.Authentication;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using OrderDeliverySystem.Client.Infrastructure;
using MudBlazor.Services;
using OrderDeliverySystem.Client.Infrastructure.Services.Orders;
using OrderDeliverySystem.Client.Infrastructure.Services.Profile;
using OrderDeliverySystem.Client.Infrastructure.Services.Cart;
using OrderDeliverySystem.Client.Infrastructure.Services.Item;
var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddOptions();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<ItemService>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<ApiAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
builder.Services.AddScoped<TokenHelper>();
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<CartService>();
//builder.Services.AddHttpClient("API", client =>
//{
//    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
//});

await builder.Build().RunAsync();

