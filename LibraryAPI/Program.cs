using FastEndpoints;
using LibraryAPI.Endpoints.Books;
using LibraryAPI.Auth;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Serilog;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/library-api-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add Serilog to the logging pipeline
builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddFastEndpoints();

// Register application services
builder.Services.AddScoped<IBookService, BookService>();

// add authentication and authorization to container
builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});
builder.Services.AddAuthorizationBuilder().AddPolicy(AuthPolicies.BeyondTrust, policy => policy.RequireRole(AuthRoles.BeyondTrust));

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints();

try
{
    Log.Information("Starting Library API with Windows Authentication");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
