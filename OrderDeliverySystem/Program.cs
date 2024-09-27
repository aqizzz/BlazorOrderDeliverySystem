using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OrderDeliverySystem.Client.Infrastructure.Services.Authentication;
using OrderDeliverySystem.Client.Infrastructure.Services.Profile;
using OrderDeliverySystem.Components;
using OrderDeliverySystem.Middleware;
using OrderDeliverySystem.Share.Data;
using OrderDeliverySystem.Share.Data.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using OrderDeliverySystem.Client.Infrastructure;
using MudBlazor.Services;
using OrderDeliverySystem.Client.Infrastructure.Services.Orders;
using OrderDeliverySystem.Client.Infrastructure.Services.Cart;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<OrderService>();
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey

    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    sqlOptions => sqlOptions.MigrationsAssembly("OrderDeliverySystem.Share")));

// Bind JWT settings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();

if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.Key))
{
    throw new InvalidOperationException("JWT settings are not properly configured.");
}

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var baseUrl = "https://orderdeliverysystemtesttestapi.azure-api.net";
if (environment == "Development")
{
    baseUrl = "https://localhost:7027/";
}

// Add HttpClient service for API calls
builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri(baseUrl);
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();

    if (environment == "Development")
    {
        // Ignore SSL certificate errors (only in development)
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
    }

    return handler;
});

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<ApiAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.Response.Redirect("/"); // Redirect to home page
            context.HandleResponse(); // Suppress the default behavior
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = "swagger";
    });
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseValidationExceptionHandler();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(OrderDeliverySystem.Client._Imports).Assembly);

app.MapControllers();


app.Run();
